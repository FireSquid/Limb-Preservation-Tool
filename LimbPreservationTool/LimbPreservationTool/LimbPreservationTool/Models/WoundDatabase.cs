﻿using SQLite;
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
            CreateTableResult woundTable = await dbConnection.CreateTableAsync<DBWoundData>();
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
            if (!StringIsSafe(patient.PatientName)) throw new NonAlphaNumericInsertException(patient.PatientName, "dbPatient");

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
            if (!StringIsSafe(patientName)) throw new NonAlphaNumericInsertException(patientName, "dbPatient");

            var newPatient = DBPatient.Create(patientName);

            return await dbConnection.InsertAsync(newPatient);
        }

        public async Task<int> DeletePatient(DBPatient patient)
        {
            return await dbConnection.DeleteAsync(patient);
        }


        public async Task<DBWoundData> GetWoundData(Guid dataID)
        {
            try
            {
                return await dbConnection.Table<DBWoundData>().Where(data => data.DataID.Equals(dataID)).FirstAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteWoundData(DBWoundData woundData)
        {
            return await dbConnection.DeleteAsync<int>(woundData.DataID);
        }

        public async Task<bool> CheckDuplicateData(DBWoundData woundData)
        {
            try
            {
                await dbConnection.Table<DBWoundData>().Where(data => (
                !data.DataID.Equals(woundData.DataID)
                && data.Date.Equals(woundData.Date)
                && data.PatientID.Equals(woundData.PatientID)
                && data.WoundGroup.Equals(woundData.WoundGroup)
                )).FirstAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> SetWoundData(DBWoundData woundData)
        {
            if (!string.IsNullOrEmpty(woundData.Img) && !StringIsSafe(woundData.Img))
                throw new NonAlphaNumericInsertException(woundData.Img, "DBWoundData");
            if (!string.IsNullOrEmpty(woundData.WoundGroup) && !StringIsSafe(woundData.WoundGroup))
                throw new NonAlphaNumericInsertException(woundData.WoundGroup, "DBWoundData");

            if (!(await CheckDuplicateData(woundData)))
            { 
                try
                {
                    await GetWoundData(woundData.DataID);

                    return await dbConnection.UpdateAsync(woundData);
                }
                catch(Exception)
                {
                    return await dbConnection.InsertAsync(woundData);
                }                
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Data already exists for the patient {woundData.PatientID} in the group '{woundData.WoundGroup}' on the date {woundData.Date}");

                return 0;
            }
        }

        public async Task<Dictionary<string, List<DBWoundData>>> GetAllPatientWoundData(Guid patientID)
        {
            List<DBWoundData> patientData = await dbConnection.Table<DBWoundData>().Where(data => data.PatientID.Equals(patientID)).ToListAsync();

            System.Diagnostics.Debug.WriteLine($"Found {patientData.Count} data entries");

            Dictionary<string, List<DBWoundData>> woundDict = new Dictionary<string, List<DBWoundData>>();

            foreach (var data in patientData)
            {
                if (!woundDict.ContainsKey(data.WoundGroup))
                {
                    woundDict.Add(data.WoundGroup, new List<DBWoundData>());
                    
                }
                woundDict[data.WoundGroup].Add(data);
            }

            return woundDict;
        }

        public static bool StringIsSafe(string str)
        {
            return str.All(c => {
                return (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
                });
        }

        public async Task<int> DeleteAllPatients()
        {
            return await dbConnection.DeleteAllAsync<DBPatient>();
        }

        public async Task<int> DeleteAllWoundData()
        {
            return await dbConnection.DeleteAllAsync<DBWoundData>();
        }
    }

    [Table("DBWoundData")]
    public class DBWoundData
    {
        [Column("dataID")]
        [PrimaryKey]
        [NotNull]
        public Guid DataID { get; set; }

        [Column("patientID")]
        [NotNull]
        public Guid PatientID { get; set; }

        [Column("group")]
        public string WoundGroup { get; set; }

        [Column("date")]
        [NotNull]
        public long Date { get; set; }

        [Column("size")]
        public float Size { get; set; }

        [Column("image")]
        public string Img { get; set; }

        [Column("wound")]
        public int Wound { get; set; }

        [Column("ischemia")]
        public int Ischemia { get; set; }

        [Column("infection")]
        public int Infection { get; set; }

        public DBWoundData() { }

        public static DBWoundData Create()
        {
            DBWoundData data = new DBWoundData();
            data.DataID = Guid.NewGuid();
            data.Date = DateTime.Today.Ticks;
            return data;
        }

        public static DBWoundData Create(Guid dataID)
        {
            DBWoundData data = new DBWoundData();
            data.DataID = dataID;
            data.Date = DateTime.Today.Ticks;
            return data;
        }

        public DBWoundData SetDate(DateTime date)
        {
            Date = date.Date.Ticks;
            return this;
        }

        public DBWoundData SetBase(Guid patient, string group)
        {
            PatientID = patient;
            WoundGroup = group;
            return this;
        }

        public DBWoundData SetWound(float size, string image)
        {
            Size = size;
            Img = image;
            return this;
        }

        public DBWoundData SetWifi(int wound, int ischemia, int infection)
        {
            Wound = wound;
            Ischemia = ischemia;
            Infection = infection;
            return this;
        }
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
