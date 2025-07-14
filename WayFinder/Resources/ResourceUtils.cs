using System;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WayFinder.Resources
{
    public static class ResourceUtils
    {
        /// <summary>
        /// Retrieves an embedded image resource from the executing assembly.
        /// </summary>
        /// <remarks>This method attempts to locate and return an image resource embedded within the
        /// assembly. If the specified resource is not found or an error occurs during retrieval, the method returns
        /// <see langword="null"/>.</remarks>
        /// <param name="name">The name of the embedded resource to retrieve. This should include the full namespace and resource name.</param>
        /// <returns>An <see cref="ImageSource"/> representing the embedded image if found; otherwise, <see langword="null"/>.</returns>
        public static ImageSource GetEmbeddedImage(string name)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = name; // Ensure the resource name is correctly passed
                System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName);

                if (stream == null)
                {
                    // If the stream is null, the resource was not found. 
                    return null;
                }

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
                return image;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
