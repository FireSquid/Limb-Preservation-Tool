using SQLite;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Guid;

namespace LimbPreservationTool.Models
{
    [Serializable]
    class NonAlphaNumericInsertException : Exception
    {
        public NonAlphaNumericInsertException() { }

        public NonAlphaNumericInsertException(string item, string dbname) : base("Attempted to insert non-alphanumeric string '{0}' into the database '{1}", item, dbname) { }
    }

    internal class WoundDatabase
    {
        private static SQLiteAsyncConnection dbConnection;

        private static readonly AsyncLazy<WoundDatabase> Database = new AsyncLazy<WoundDatabase>(async () =>
        {
            var instance = new WoundDatabase();
            CreateTableResult result = await dbConnection.CreateTableAsync<dbWound>();
            return instance;
        });
    }

    internal class PatientDatabase
    {
        private static SQLiteAsyncConnection dbConnection;

        private static readonly AsyncLazy<PatientDatabase> Database = new AsyncLazy<PatientDatabase>(async () =>
        {
            var instance = new PatientDatabase();
            CreateTableResult result = await dbConnection.CreateTableAsync<dbPatient>();
            return instance;
        });

        public Task<dbPatient> GetPatient(string name)
        {
            return dbConnection.Table<dbPatient>().Where(patient => patient.PatientName.Equals(name)).FirstOrDefaultAsync();
        }

        public async Task<dbPatient> GetPatient(Guid id)
        {
            try
            {
                return await dbConnection.Table<dbPatient>().Where(patient => patient.PatientID.Equals(id)).FirstAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public async Task<int> SetPatient(dbPatient patient)
        {
            try
            {
                await GetPatient(patient.PatientID);

                return await dbConnection.UpdateAsync(patient);
            }
            catch (Exception ex)
            {
                return await dbConnection.InsertAsync(patient);
            }
        }

        public async Task<int> CreatePatient(string patientName)
        {
            if (string.IsNullOrEmpty(patientName)) throw new ArgumentNullException(nameof(patientName));
            if (patientName.All(char.IsLetterOrDigit)) throw new NonAlphaNumericInsertException(patientName, "dbPatient");

            var newPatient = dbPatient.Create(patientName);

            return await dbConnection.InsertAsync(newPatient);
        }

        public async Task<int> DeletePatient(dbPatient patient)
        {
            return await dbConnection.DeleteAsync(patient);
        }
    }

    internal class dbWound
    {
        public Guid PatientID { get; }
        public DateTime WoundDate { get; }
    }

    [Table("dbPatient")]
    internal class dbPatient
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
        public string PatientName { 
            get { return _patientName; }
            private set { if (value != _patientName) _patientName = value; }
        }

        public static dbPatient Create(string name)
        {
            dbPatient dbPatient = new dbPatient();
            dbPatient.PatientName = name;
            dbPatient.PatientID = Guid.NewGuid();
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
