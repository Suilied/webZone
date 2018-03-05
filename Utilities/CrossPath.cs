using System.Runtime.InteropServices;

namespace webZone.Utilities
{
    public static class CrossPath
    {
        public static string FixPath(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Fix filepaths for Ubuntu & Win
                path = path.Replace(@"/", @"\");
            }
            return path;
        }
    }
}
