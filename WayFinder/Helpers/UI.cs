using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WayFinder.Helpers
{
    public static class UI
    {
        /// <summary>
        /// Displays a dialog with "Yes" and "No" options and returns a boolean value based on the user's selection.
        /// </summary>
        /// <remarks>This method uses a task dialog to present the options to the user. Ensure that the
        /// application is running in a context where dialogs can be displayed.</remarks>
        /// <param name="title">The title of the dialog window.</param>
        /// <param name="text">The message or prompt displayed in the dialog.</param>
        /// <returns><see langword="true"/> if the user selects "Yes"; otherwise, <see langword="false"/>.</returns>
        public static bool BoolFromYesNoDialog(string title, string text)
        {
            TaskDialog dialog = YesNoDialogue(title, text);
            TaskDialogResult result = dialog.Show();

            return result == TaskDialogResult.Yes;
        }

        /// <summary>
        /// Creates a dialog box with "Yes" and "No" options, allowing the user to make a binary choice.
        /// </summary>
        /// <remarks>The dialog box supports cancellation, enabling the user to close it without selecting
        /// "Yes" or "No." Use the returned <see cref="TaskDialog"/> instance to display the dialog and handle user
        /// input.</remarks>
        /// <param name="title">The title displayed at the top of the dialog box. Cannot be null or empty.</param>
        /// <param name="text">The main content displayed in the dialog box. Cannot be null or empty.</param>
        /// <returns>A <see cref="TaskDialog"/> instance configured with "Yes" and "No" buttons, allowing cancellation.</returns>
        public static TaskDialog YesNoDialogue(string title, string text)
        {
            TaskDialog dialog = new TaskDialog(title);
            dialog.MainContent = text;
            dialog.AllowCancellation = true;
            dialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;

            return dialog;
        }
    }
}
