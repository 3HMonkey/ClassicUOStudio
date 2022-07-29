using System.IO;

namespace ClassicUO.IO
{
    public static class PathUtility
    {
        public static string EnsureDirectory(string dir)
        {
            Directory.CreateDirectory(dir);

            return dir;
        }


    }
}