using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LimbPreservationTool.Models
{
    public static class Constants
    {
        public const string DatabaseFilename = "LPTDatabase.db3";

        public const string ImageDirectoryName = "SavedImages";

        public const SQLite.SQLiteOpenFlags Flags = SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache | SQLite.SQLiteOpenFlags.ProtectionComplete;

        public static string DatabasePath
        {
            get
            {
                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }

        }

        public static string GetImagePath(Guid patientID, string imgName)
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string patientDirectory = Path.Combine(basePath, ImageDirectoryName, patientID.ToString());
            if (!Directory.Exists(patientDirectory))
            {
                Directory.CreateDirectory(patientDirectory);
            }

            return Path.Combine(patientDirectory, imgName);
        }

        public static DirectoryInfo GetImageDirectory()
        {
            return new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ImageDirectoryName));
        }
    }
}
