using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Windows.Forms;
using System.IO;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Autodesk.Revit.UI;

namespace WayFinder.Helpers
{
    public static class FileHelpers
    {
        /// <summary>
        /// Constructs the full file path to a packaged asset based on the specified asset name.
        /// </summary>
        /// <remarks>This method is useful for locating assets that are packaged alongside the
        /// application's executable. The returned path is constructed using the directory of the currently executing
        /// assembly.</remarks>
        /// <param name="assetName">The name of the asset file, including its relative path within the application's directory.</param>
        /// <returns>The full file path to the specified asset, combining the directory of the executing assembly and the
        /// provided asset name.</returns>
        public static String GetPackagedAssetPath(String assetName)
        {
            // get the location of the executing DLL
            string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string assemblyDir = Path.GetDirectoryName(assemblyLocation);

            string assetPath = Path.Combine(assemblyDir, assetName);

            return assetPath;
        }


        /// <summary>
        /// Retrieves the path to the shared parameters file associated with the given application.
        /// </summary>
        /// <remarks>This method checks whether the shared parameters file path is set and non-empty.  If
        /// the path is empty or null, the method returns <see langword="false"/>.</remarks>
        /// <param name="app">The application instance from which to retrieve the shared parameters file path.</param>
        /// <param name="path">When the method returns, contains the path to the shared parameters file if one is set;  otherwise, contains
        /// an empty string.</param>
        /// <returns><see langword="true"/> if the shared parameters file path is successfully retrieved and is not empty; 
        /// otherwise, <see langword="false"/>.</returns>
        public static bool GetSharedParamFile(Application app, ref String path)
        {
            path = app.SharedParametersFilename; // get the file path
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Determines whether the shared parameter file associated with the specified application contains a group with
        /// the specified name.
        /// </summary>
        /// <remarks>This method checks if the shared parameter file is available and contains a group
        /// named according to the global shared parameter group value. It returns <see langword="false"/> if the file
        /// is not available or the group does not exist.</remarks>
        /// <param name="app">The application instance used to access the shared parameter file.</param>
        /// <returns><see langword="true"/> if the shared parameter file contains a group with the specified name; otherwise,
        /// <see langword="false"/>.</returns>
        public static bool SharedParamsContainSignHelper(Application app)
        {
            DefinitionFile defFile = app.OpenSharedParameterFile();
            if (defFile == null)
            {
                return false;
            }
            if (defFile.Groups.get_Item(Settings.FamilySettings.SharedParamGroup) == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Adds a shared parameter group to the shared parameter file and populates it with predefined parameters.
        /// </summary>
        /// <remarks>This method attempts to open the shared parameter file associated with the provided
        /// <paramref name="app"/> instance. If the file cannot be loaded, the method sets <paramref name="errMsg"/> to
        /// an appropriate error message and returns <see langword="false"/>.  If the specified shared parameter group
        /// does not exist in the file, it is created. The method then ensures that all predefined shared parameters are
        /// added to the group. If a parameter already exists, it is not duplicated.  Any exceptions encountered during
        /// the operation are caught, and <paramref name="errMsg"/> is updated with the exception details.</remarks>
        /// <param name="app">The <see cref="Application"/> instance used to access the shared parameter file.</param>
        /// <param name="errMsg">A reference to a string that will contain an error message if the operation fails. If the operation
        /// succeeds, the string remains unchanged.</param>
        /// <returns><see langword="true"/> if the shared parameter group was successfully added or already exists;  otherwise,
        /// <see langword="false"/> if an error occurred.</returns>
        public static bool AddSharedParamGroupToFile(Application app, ref string errMsg, ref DefinitionGroup defGroup)
        {
            try
            {
                DefinitionFile defFile = app.OpenSharedParameterFile();
                if (defFile == null)
                {
                    errMsg = "Can't load shared parameter file.";
                    return false;
                }

                // find the sharedparamgroup or add it if it's not there.
                string newGroup = Settings.FamilySettings.SharedParamGroup;
                defGroup = defFile.Groups.get_Item(newGroup) ?? defFile.Groups.Create(newGroup);

                // add the shared parameters to the group
                foreach (string paramName in Settings.FamilySettings.sharedParams.Keys)
                {
                    ForgeTypeId paramType = Settings.FamilySettings.sharedParams[paramName];

                    // does the parameter already exist in the group?
                    Definition def = defGroup.Definitions.get_Item(paramName);
                    if (def == null)
                    {
                        // create a new parameter if it's not present
                        var options = new ExternalDefinitionCreationOptions(paramName, paramType);
                        defGroup.Definitions.Create(options);
                    }
                    // if not, we're good to go!
                }

                return true;
            }
            catch (Exception ex)
            {
                errMsg = $"Couldn't modify the shared parameter file. \n{ex.Message}";
                return false;
            }
        }


        /// <summary>
        /// Binds shared parameters from the specified <see cref="DefinitionGroup"/> to the given <see cref="Category"/>
        /// in the provided <see cref="Document"/>.
        /// </summary>
        /// <remarks>This method uses a transaction to bind shared parameters to the specified category. 
        /// If the binding operation fails, the transaction is rolled back, and an error message is provided via the
        /// <paramref name="errMsg"/> parameter.</remarks>
        /// <param name="doc">The <see cref="Document"/> in which the shared parameters will be bound.</param>
        /// <param name="category">The <see cref="Category"/> to which the shared parameters will be bound.</param>
        /// <param name="defGroup">The <see cref="DefinitionGroup"/> containing the shared parameters to bind.</param>
        /// <param name="errMsg">A reference to a string that will contain an error message if the operation fails.  If the operation
        /// succeeds, this will be set to an empty string.</param>
        /// <returns><see langword="true"/> if all shared parameters were successfully bound to the specified category; 
        /// otherwise, <see langword="false"/>.</returns>
        public static bool BindSharedParamToType(Document doc, Category category, DefinitionGroup defGroup, ref string errMsg)
        {
            using (Transaction trans = new Transaction(doc, $"bind shared parameter to {category.ToString()}"))
            {
                bool success = true;

                errMsg = "";
                trans.Start();

                CategorySet categories = doc.Application.Create.NewCategorySet();
                categories.Insert(category);

                InstanceBinding instBinding = doc.Application.Create.NewInstanceBinding(categories);
                BindingMap bindingMap = doc.ParameterBindings;

                foreach (Definition def in defGroup.Definitions)
                {
                    if (!bindingMap.Contains(def))
                    {
                        success = bindingMap.Insert(def, instBinding, GroupTypeId.Text) && success;
                    }
                }

                if (!success)
                {
                    errMsg = "Couldn't bind parameters!";
                    return success;
                }
                trans.Commit();
                return success;
            }
        }

        /// <summary>
        /// Creates a new shared parameter file and sets it as the active shared parameter file for the application.
        /// </summary>
        /// <remarks>This method displays a save file dialog to allow the user to specify the location and
        /// name of the shared parameter file. If the user cancels the dialog, the method returns <see
        /// langword="null"/>. If the file creation fails, an error message is displayed to the user.</remarks>
        /// <param name="app">The <see cref="Application"/> instance for which the shared parameter file will be created.</param>
        /// <returns>The full file path of the newly created shared parameter file, or <see langword="null"/> if the operation
        /// was canceled.</returns>
        public static string CreateSharedParamFile(Application app)
        {
            string filePath = null;

            //Set up the save file dialog
            SaveFileDialog sfDog = new SaveFileDialog()
            {
                Title = "Create a new shared parameter file",
                Filter = "Text Files (*.txt)|*.txt",
                FileName = "SharedParameters.txt"
            };

            if (sfDog.ShowDialog() == DialogResult.OK)
            {
                filePath = sfDog.FileName;
            }

            try
            {
                File.Create(filePath).Close();

                app.SharedParametersFilename = filePath;
            }
            catch (System.Exception ex)
            {
                TaskDialog.Show("Error", $"Could not create shared parameter file. {ex.Message}");
            }

            return filePath;
        }

    }
