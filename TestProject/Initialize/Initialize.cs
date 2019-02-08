using System;
using System.IO;
using System.Linq;

namespace Initialize
{
    public static class Initialize
    {
        public static DirectoryInfo TryGetSolutionDirectoryInfo()
        {
            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            return directory;
        }

        public static string GetDriverFolder()
        {
            var solutionPath = TryGetSolutionDirectoryInfo().FullName;

            return Path.Combine(solutionPath, @"Drivers\");
        }
    }
}
