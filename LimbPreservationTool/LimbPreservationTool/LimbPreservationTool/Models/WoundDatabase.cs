using SQLite;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace LimbPreservationTool.Models
{
    [Serializable]
    class NonAlphaNumericInsertException : Exception
    {
        public NonAlphaNumericInsertException() : base("Attempted to insert non-alphanumeric string into the database") { }

        public NonAlphaNumericInsertException(string item, string dbname) : base($"Attempted to insert non-alphanumeric string '{item}' into the database '{dbname}") { }
    }

    internal class WoundDatabase
    {

        private static SQLiteAsyncConnection dbConnection;

        private static readonly AsyncLazy<WoundDatabase> Database = new AsyncLazy<WoundDatabase>(async () =>
        {
            var instance = new WoundDatabase();
            CreateTableResult woundTable = await dbConnection.CreateTableAsync<DBWound>();
            CreateTableResult patientTable = await dbConnection.CreateTableAsync<DBPatient>();
            return instance;
        });

        private WoundDatabase()
        {
            dbConnection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            dbConnection.EnableWriteAheadLoggingAsync();
        }

        public Task<DBPatient> GetPatient(string name)
        {
            try
            {
                return dbConnection.Table<DBPatient>().Where(patient => patient.PatientName.Equals(name)).FirstAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DBPatient> GetPatient(Guid id)
        {
            try
            {
                return await dbConnection.Table<DBPatient>().Where(patient => patient.PatientID.Equals(id)).FirstAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> SetPatient(DBPatient patient)
        {
            try
            {
                await GetPatient(patient.PatientID);

                return await dbConnection.UpdateAsync(patient);
            }
            catch (Exception)
            {
                return await dbConnection.InsertAsync(patient);
            }
        }

        public async Task<int> CreatePatient(string patientName)
        {
            if (string.IsNullOrEmpty(patientName)) throw new ArgumentNullException(nameof(patientName));
            if (patientName.All(char.IsLetterOrDigit)) throw new NonAlphaNumericInsertException(patientName, "dbPatient");

            var newPatient = DBPatient.Create(patientName);

            return await dbConnection.InsertAsync(newPatient);
        }

        public async Task<int> DeletePatient(DBPatient patient)
        {
            return await dbConnection.DeleteAsync(patient);
        }
    }

    [Table("DBWound")]
    internal class DBWound
    {
        private long _woundID;
        [Column("woundID")]
        [PrimaryKey]
        [NotNull]
        [AutoIncrement]
        public long WoundID {
            get { return _woundID; }
            private set { if (value != _woundID) _woundID = value; }
        }

        private Guid _woundGroup;
        [Column("group")]
        [NotNull]
        public Guid WoundGroup {
            get { return _woundGroup; }
            private set { if (value != _woundGroup) _woundGroup = value; }
        }

        private Guid _patientID;
        [Column("patientID")]
        [NotNull]
        public Guid PatientID { 
            get { return _patientID; }
            private set { if (value != _patientID) _patientID = value; }
        }

        private DateTime _woundDate;
        [Column("date")]
        public DateTime WoundDate { 
            get { return _woundDate.Date; } 
            private set { if (value.Date != _woundDate.Date) _woundDate = WoundDate.Date; }
        }

        private float _woundSize;
        [Column("size")]
        public float WoundSize
        {
            get { return _woundSize; }
            private set { if (WoundSize != _woundSize) _woundSize = WoundSize; }
        }

        private string _woundImgName;
        [Column("img")]
        public string WoundImgName
        {
            get { return _woundImgName; }
            private set { if (WoundImgName != _woundImgName) _woundImgName = WoundImgName; }
        }

        private int _wifiWound;
        [Column("wifiWound")]
        public int WIfiWound
        {
            get { return _wifiWound; }
            private set { if (WIfiWound != _wifiWound) _wifiWound = WIfiWound; }
        }

        private int _wifiIschemia;
        [Column("wifiIschemia")]
        public int WIfiIschemia
        {
            get { return _wifiIschemia; }
            private set { if (WIfiIschemia != _wifiIschemia) _wifiIschemia = WIfiIschemia; }
        }

        private int _wifiInfection;
        [Column("wifiInfection")]
        public int WIfiInfection
        {
            get { return _wifiInfection; }
            private set { if (WIfiInfection != _wifiInfection) _wifiInfection = WIfiInfection; }
        }

        public static DBWound Create(Guid patientID, float size, string img)
        {
            DBWound db = new DBWound();
            db.PatientID = patientID;
            db.WoundSize = size;
            db.WoundImgName = img;
            db.WoundGroup = Guid.NewGuid();
            return db;
        }

        public static DBWound Create(long woundID, Guid patientID, float size, string img)
        {
            DBWound db = new DBWound();
            db.WoundID = woundID;
            db.PatientID = patientID;
            db.WoundSize = size;
            db.WoundImgName = img;
            db.WoundGroup = Guid.NewGuid();
            return db;
        }

        public static DBWound Create(Guid patientID, int wound, int ischemia, int infection)
        {
            DBWound db = new DBWound();
            db.PatientID = patientID;
            db.WIfiWound = wound;
            db.WIfiIschemia = ischemia;
            db.WIfiInfection = infection;
            db.WoundGroup = Guid.NewGuid();
            return db;
        }

        public static DBWound Create(long woundID, Guid patientID, int wound, int ischemia, int infection)
        {
            DBWound db = new DBWound();
            db.WoundID = woundID;
            db.PatientID = patientID;
            db.WIfiWound = wound;
            db.WIfiIschemia = ischemia;
            db.WIfiInfection = infection;
            db.WoundGroup = Guid.NewGuid();
            return db;
        }

        public static DBWound Create(Guid patientID, float size, string img, int wound, int ischemia, int infection)
        {
            DBWound db = new DBWound();
            db.PatientID = patientID;
            db.WoundSize = size;
            db.WoundImgName = img;
            db.WIfiWound = wound;
            db.WIfiIschemia = ischemia;
            db.WIfiInfection = infection;
            db.WoundGroup = Guid.NewGuid();
            return db;
        }

        public static DBWound Create(long woundID, Guid patientID, float size, string img, int wound, int ischemia, int infection)
        {
            DBWound db = new DBWound();
            db.WoundID = woundID;
            db.PatientID = patientID;
            db.WoundSize = size;
            db.WoundImgName = img;
            db.WIfiWound = wound;
            db.WIfiIschemia = ischemia;
            db.WIfiInfection = infection;
            db.WoundGroup = Guid.NewGuid();
            return db;
        }

        public void SetGroup(Guid group)
        {
            WoundGroup = group;
        }
    }

    [Table("DBPatient")]
    internal class DBPatient
    {

        private Guid _patientID;
        [Column("patientID")]
        [PrimaryKey]
        [NotNull]
        public Guid PatientID { 
            get { return _patientID; }
            private set { if (value != _patientID) _patientID = value; }
        }

        private string _patientName;
        [Column("patientName")]
        [NotNull]
        public string PatientName { 
            get { return _patientName; }
            private set { if (value != _patientName) _patientName = value; }
        }

        public static DBPatient Create(string name)
        {
            DBPatient dbPatient = new DBPatient();
            dbPatient.PatientName = name;
            dbPatient.PatientID = Guid.NewGuid();
            return dbPatient;
        }

        public static DBPatient Create(Guid id, string name)
        {
            DBPatient dbPatient = new DBPatient();
            dbPatient.PatientName = name;
            dbPatient.PatientID = id;
            return dbPatient;
        }

    }

    internal class AsyncLazy<T>
    {
        readonly Lazy<Task<T>> instance;

        public AsyncLazy(Func<T> factory)
        {
            instance = new Lazy<Task<T>>(() => Task.Run(factory));
        }

        public AsyncLazy(Func<Task<T>> factory)
        {
            instance = new Lazy<Task<T>>(() => Task.Run(factory));
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return instance.Value.GetAwaiter();
        }
    }
}
