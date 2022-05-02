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

        public static readonly AsyncLazy<WoundDatabase> Database = new AsyncLazy<WoundDatabase>(async () =>
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

        public Task<List<DBPatient>> GetPatientsList()
        {
            return dbConnection.Table<DBPatient>().ToListAsync();
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
            if (!patientName.All(char.IsLetterOrDigit)) throw new NonAlphaNumericInsertException(patientName, "dbPatient");

            var newPatient = DBPatient.Create(patientName);

            return await dbConnection.InsertAsync(newPatient);
        }

        public async Task<int> DeletePatient(DBPatient patient)
        {
            return await dbConnection.DeleteAsync(patient);
        }
    }

    [Table("DBWound")]
    public class DBWound
    {
        [Column("woundID")]
        [PrimaryKey]
        [NotNull]
        [AutoIncrement]
        public long WoundID { get; set; }
    }

    [Table("DBPatient")]
    public class DBPatient
    {

        [Column("patientID")]
        [PrimaryKey]
        [NotNull]
        public Guid PatientID { get; set; }

        [Column("patientName")]
        [NotNull]
        public string PatientName { get; set; }

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
