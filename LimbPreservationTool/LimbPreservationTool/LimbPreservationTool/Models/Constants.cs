using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LimbPreservationTool.Models
{
    public static class Constants
    {
        public const string DatabaseFilename = "LPTDatabase.db3";

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
    }
}
