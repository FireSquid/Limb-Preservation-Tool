using SQLite;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using SkiaSharp;

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

            dataHolder = DBWoundData.Create(Guid.Empty);
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
            await DeletePatientsWoundData(patient);

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
            // Delete associated image
            if (!string.IsNullOrEmpty(woundData.Img) && woundData.PatientID != null)
                DeleteImage(woundData.PatientID, woundData.Img);

            return await dbConnection.DeleteAsync(woundData);
        }

        public async Task DeletePatientsWoundData(DBPatient patient)
        {
            var woundData = await dbConnection.Table<DBWoundData>().Where(data => data.PatientID.Equals(patient.PatientID)).ToListAsync();

            foreach (var wound in woundData)
            {
                // Delete associated images
                if (!string.IsNullOrEmpty(wound.Img) && wound.PatientID != null)
                    DeleteImage(wound.PatientID, wound.Img);

                await dbConnection.DeleteAsync(wound);
            }
        }

        public async Task<DBWoundData> CheckDuplicateData(DBWoundData woundData)
        {
            try
            {
                var result = await dbConnection.Table<DBWoundData>().Where(data => (
                !data.DataID.Equals(woundData.DataID)
                && data.Date.Equals(woundData.Date)
                && data.PatientID.Equals(woundData.PatientID)
                && data.WoundGroup.Equals(woundData.WoundGroup)
                )).FirstAsync();

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<int> SetWoundData(DBWoundData woundData, SKImage imageData = null)
        {
            System.Diagnostics.Debug.WriteLine($"Setting Wound Data");

            var saveData = DBWoundData.CopyFrom(woundData);

            if (!string.IsNullOrEmpty(saveData.WoundGroup) && !StringIsSafe(saveData.WoundGroup))
                throw new NonAlphaNumericInsertException(saveData.WoundGroup, "DBWoundData");

            if ((await CheckDuplicateData(saveData)) == null)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Checking for duplicates");
                    var oldData = await GetWoundData(saveData.DataID);

                    

                    if (imageData != null)
                    {
                        if (oldData.Img != saveData.Img)
                        {
                            System.Diagnostics.Debug.WriteLine("Handling Image Swap");
                            // Make sure to delete old image to avoid filling storage
                            if (!string.IsNullOrEmpty(oldData.Img) && oldData.PatientID != null)
                                DeleteImage(oldData.PatientID, oldData.Img);

                            if (!string.IsNullOrEmpty(saveData.Img) && saveData.PatientID != null && imageData != null)
                            {
                                System.Diagnostics.Debug.WriteLine("Saving New Image");
                                SaveImage(saveData.PatientID, saveData.Img, imageData);
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Exchanging Img Properties");
                        saveData.Img = oldData.Img;
                    }

                    System.Diagnostics.Debug.WriteLine("Updating Wound Entry");
                    return await dbConnection.UpdateAsync(saveData);
                }
                catch (Exception)
                {
                    if (!string.IsNullOrEmpty(saveData.Img) && saveData.PatientID != null && imageData != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Saving new Image");
                        SaveImage(saveData.PatientID, saveData.Img, imageData);
                    }

                    System.Diagnostics.Debug.WriteLine($"Inserting Wound Entry - Group {saveData.WoundGroup}");
                    return await dbConnection.InsertAsync(saveData);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Data already exists for the patient {saveData.PatientID} in the group '{saveData.WoundGroup}' on the date {saveData.Date}");

                return 0;
            }
        }

        public async Task<Dictionary<string, List<DBWoundData>>> GetAllPatientWoundData(Guid patientID)
        {
            List<DBWoundData> patientData = await dbConnection.Table<DBWoundData>().Where(data => data.PatientID.Equals(patientID)).ToListAsync();

            System.Diagnostics.Debug.WriteLine($"Found {patientData.Count} data entries for {patientID}");

            Dictionary<string, List<DBWoundData>> woundDict = new Dictionary<string, List<DBWoundData>>();

            foreach (var data in patientData)
            {
                if (!woundDict.ContainsKey(data.WoundGroup))
                {
                    woundDict.Add(data.WoundGroup, new List<DBWoundData>());                    
                }

                woundDict[data.WoundGroup].Add(data);
            }

            foreach (var woundDataList in woundDict.Values)
            {
                woundDataList.Sort(new WoundDataDateComparer());
            }

            return woundDict;
        }

        public static bool StringIsSafe(string str) // Checks if string can safely be inserted into the database (stricter than necessary)
        {
            return str.All(c => {
                return (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
                });
        }

        public async Task<int> DeleteAllPatients()
        {
            throw new NotImplementedException("DeleteAllPatients needs to delete image files before it can be used");// Make sure to delete all stored images before removing this

            return await dbConnection.DeleteAllAsync<DBPatient>();
        }

        public async Task<int> DeleteAllWoundData()
        {
            throw new NotImplementedException("DeleteAllWoundData needs to delete image files before it can be used");// Make sure to delete all stored images before removing this

            return await dbConnection.DeleteAllAsync<DBWoundData>();
        }

        public async Task<List<DBPatient>> GetClosestPatient(string tgtName)
        {
            return await GetPatientsList();
        }

        public static int LevenshteinDist(string source, string target)
        {
            string rSource = String.Concat(source.ToLower().Where((c) => !char.IsWhiteSpace(c)));
            string rTarget = String.Concat(target.ToLower().Where((c) => !char.IsWhiteSpace(c)));

            string strA = rSource.Substring(0, Math.Min(rSource.Length, rTarget.Length));
            string strB = rTarget.Substring(0, Math.Min(rTarget.Length, rSource.Length));

            int[,] dists = new int[strA.Length + 1, strB.Length + 1];

            for (int i = 0; i <= strA.Length; i++) dists[i, 0] = i;
            for (int i = 0; i <= strB.Length; i++) dists[0, i] = i;

            for (int b = 1; b <= strB.Length; b++)
                for (int a = 1; a <= strA.Length; a++)
                {
                    int subCost = (strA[a - 1] == strB[b - 1]) ? 0 : 1;

                    dists[a, b] = new int[] {
                        dists[a - 1, b] + 1,
                        dists[a, b - 1] + 1,
                        dists[a - 1, b - 1] + subCost
                    }.Min();
                }

            return dists[strA.Length, strB.Length];
        }

        public static void DeleteImage(Guid patientID, string imgName)
        {
            string deletePath = Constants.GetImagePath(patientID, imgName);
            System.Diagnostics.Debug.WriteLine($"Deleting {deletePath}");
            File.Delete(deletePath);
            System.Diagnostics.Debug.WriteLine($"Finished Deleting");
        }

        public static void SaveImage(Guid patientID, string imgName, SKImage saveImage)
        {
            string savePath = Constants.GetImagePath(patientID, imgName);
            System.Diagnostics.Debug.WriteLine($"Saving to {savePath}");

            using (FileStream imgFileStream = new FileStream(savePath, FileMode.Create))
            {                
                using (var imgStream = saveImage.Encode(SKEncodedImageFormat.Png, 100).AsStream())
                {
                    System.Diagnostics.Debug.WriteLine("Started Writing");
                    imgStream.CopyTo(imgFileStream);
                }                
            }
            System.Diagnostics.Debug.WriteLine($"Finished Saving");
        }

        public static bool LoadImage(Guid patientID, string imgName, out Stream imgStream)
        {
            string loadPath = Constants.GetImagePath(patientID, imgName);
            System.Diagnostics.Debug.WriteLine($"Loading {loadPath}");

            if (!File.Exists(loadPath))
            {
                System.Diagnostics.Debug.WriteLine($"Error: Could not locate the image file.");
                imgStream = Stream.Null;
                return false;
            }

            imgStream = File.OpenRead(loadPath);
            return true;
        }

        public DBWoundData dataHolder { get; set; }
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
            data.SetWifi(-1, -1, -1);
            data.SetWound(-1, null);
            return data;
        }

        public static DBWoundData Create(Guid dataID)
        {
            DBWoundData data = new DBWoundData();
            data.DataID = dataID;
            data.Date = DateTime.Today.Ticks;
            data.SetWifi(-1, -1, -1);
            data.SetWound(-1, null);
            return data;
        }

        public void ResetDetail()
        {
            SetWifi(-1, -1, -1);
            SetWound(-1, null);
        }

        public static DBWoundData CopyFrom(DBWoundData other)
        {
            DBWoundData data = new DBWoundData();
            data.DataID = other.DataID;
            data.PatientID = other.PatientID;
            data.Date = other.Date;
            data.WoundGroup = other.WoundGroup;
            data.SetWifi(other.Wound, other.Ischemia, other.Infection);
            data.SetWound(other.Size, other.Img);
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

    internal class WoundDataDateComparer : IComparer<DBWoundData>
    {
        public int Compare(DBWoundData A, DBWoundData B)
        {
            return A.Date.CompareTo(B.Date);
        }
    }
}
