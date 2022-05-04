﻿using LimbPreservationTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using LimbPreservationTool.Models;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LimbPreservationTool.ViewModels
{
    public class WoundSaveViewModel : BaseViewModel
    {
        public WoundSaveViewModel()
        {
            WoundSaveDate = DateTime.Today;
        }

        private async Task UpdatePatientList()
        {
            if (PatientName.Length > 0)
            {
                PatientsListIsVisible = true;
                System.Diagnostics.Debug.WriteLine($"Updating List");
                var patients = (await (await WoundDatabase.Database).GetClosestPatient(PatientName));
                patients.Sort((pA, pB) => WoundDatabase.LevenshteinDist(pA.PatientName, PatientName) - WoundDatabase.LevenshteinDist(pB.PatientName, PatientName));
                PatientsList = patients;
            }
            else
            {
                PatientsListIsVisible = false;
            }
        }

        private async Task UpdateWoundGroupList()
        {
            if (PatientName.Length > 0)
            {
                WoundGroupsAreVisible = true;
                System.Diagnostics.Debug.WriteLine($"Updating List");
                WoundGroupsList = (await (await WoundDatabase.Database).GetAllPatientWoundData(Patient.PatientID)).Keys.ToList().ConvertAll((w) => new WoundGroup(w));
            }
            else
            {
                WoundGroupsAreVisible = false;
            }
        }

        public void CreateNewWound()
        {
            WoundData = DBWoundData.Create().SetBase(Patient.PatientID, WoundGroupName);
        }

        public async Task ConfirmSaveData()
        {
            System.Diagnostics.Debug.WriteLine($"Starting Saving Data");

            var db = await WoundDatabase.Database;

            DBWoundData saveData = DBWoundData.Create().SetBase(Patient.PatientID, WoundGroupName).SetDate(WoundSaveDate);

            System.Diagnostics.Debug.WriteLine($"Checking for Existing Data");
            DBWoundData existingData = await db.CheckDuplicateData(saveData);
            if (existingData == null)
            {
                System.Diagnostics.Debug.WriteLine($"No Existing Data Found");
                existingData = DBWoundData.Create(Guid.Empty);
            }
            else
            {
                saveData.DataID = existingData.DataID;
                System.Diagnostics.Debug.WriteLine($"Found Existing Data");
            }

            saveData.Wound =    (WoundData.Wound >= 0)      ? WoundData.Wound       : existingData.Wound;
            saveData.Ischemia = (WoundData.Ischemia >= 0)   ? WoundData.Ischemia    : existingData.Ischemia;
            saveData.Wound =    (WoundData.Wound >= 0)      ? WoundData.Wound       : existingData.Wound;

            saveData.Size = (WoundData.Size >= 0)                               ? WoundData.Size    : existingData.Size;
            saveData.Img =  (WoundData.Img != null && WoundData.Img.Length > 0) ? WoundData.Img     : existingData.Img;

            await db.SetWoundData(saveData);

            System.Diagnostics.Debug.WriteLine($"Finished Saving Data");
        }

        private string _patientName;
        public string PatientName { 
            get => _patientName;
            set
            {
                SetProperty(ref _patientName, value);

                if (Patient == null || !value.Equals(Patient.PatientName))
                {
                    AsyncRunner.Run(UpdatePatientList());
                    PatientNameColor = "Black";
                }
                else
                {
                    PatientsListIsVisible = false;
                    PatientNameColor = "Green";
                }
            }
        }

        private DBPatient _patient;
        public DBPatient Patient { 
            get => _patient;
            set
            {
                SetProperty(ref _patient, value);
                PatientName = value.PatientName;

                if (Patient != null && Patient.PatientName.Equals(PatientName))
                    WoundGroupIsVisible = true;
            }
        }


        private List<DBPatient> _patientList;
        public List<DBPatient> PatientsList { get => _patientList; set => SetProperty(ref _patientList, value); }

        private string _patientNameColor;
        public string PatientNameColor { get => _patientNameColor; set => SetProperty(ref _patientNameColor, value); }

        private bool _patientsListIsVisible;
        public bool PatientsListIsVisible { get => _patientsListIsVisible; set => SetProperty(ref _patientsListIsVisible, value); }

        private List<WoundGroup> _woundGroupsList;
        public List<WoundGroup> WoundGroupsList { get => _woundGroupsList; set => SetProperty(ref _woundGroupsList, value); }

        public ICommand OnSaveNewWound;

        private DBWoundData _woundData;
        public DBWoundData WoundData
        {
            get => _woundData;
            set
            {
                SetProperty(ref _woundData, value);

                if (WoundData != null && WoundData.WoundGroup.Length > 0)
                {
                    WoundGroupName = WoundData.WoundGroup;
                }
            }
        }

        private string _woundGroupNameColor;
        public string WoundGroupNameColor { get => _woundGroupNameColor; set => SetProperty(ref _woundGroupNameColor, value); }

        private string _woundGroupName;
        public string WoundGroupName
        {
            get => _woundGroupName;
            set
            {
                SetProperty(ref _woundGroupName, value);

                if (WoundData == null || !value.Equals(WoundData.WoundGroup))
                {
                    AsyncRunner.Run(UpdateWoundGroupList());
                    WoundGroupNameColor = "Black";
                    WoundSaveButtonEnabled = true;
                    FinalIsVisible = false;
                }
                else
                {
                    WoundGroupsAreVisible = false;
                    WoundGroupNameColor = "Green";
                    WoundSaveButtonEnabled = false;
                    FinalIsVisible = true;
                }
            }
        }

        private bool _woundSaveButtonEnabled;
        public bool WoundSaveButtonEnabled { get => _woundSaveButtonEnabled; set => SetProperty(ref _woundSaveButtonEnabled, value); }

        private bool _woundGroupIsVisible;
        public bool WoundGroupIsVisible { get => _woundGroupIsVisible; set => SetProperty(ref _woundGroupIsVisible, value); }

        private bool _woundGroupsAreVisible;
        public bool WoundGroupsAreVisible { get => _woundGroupsAreVisible; set => SetProperty(ref _woundGroupsAreVisible, value); }

        private DateTime _woundSaveDate;
        public DateTime WoundSaveDate { get => _woundSaveDate; set => SetProperty(ref _woundSaveDate, value); }

        private bool _finalIsVisible;
        public bool FinalIsVisible
        {
            get => _finalIsVisible; set 
            { 
                SetProperty(ref _finalIsVisible, value);

                var dh = WoundDatabase.Database.GetAwaiter().GetResult().dataHolder;

                if (value)
                {
                    WifiIsVisible = false;

                    if (dh.Wound >= 0)
                    {
                        WifiIsVisible = true;
                        WoundData.Wound = dh.Wound;
                        WifiWound = $"Wound Grade: {WoundData.Wound}";
                    }
                    if (dh.Ischemia >= 0)
                    {
                        WifiIsVisible = true;
                        WoundData.Ischemia = dh.Ischemia;
                        WifiIschemia = $"Ischemia Grade: {WoundData.Ischemia}";
                    }
                    if (dh.Infection >= 0)
                    {
                        WifiIsVisible = true;
                        WoundData.Infection = dh.Infection;
                        WifiInfection = $"Infection Grade: {WoundData.Infection}";
                    }

                    SizeIsVisible = false;

                    if (dh.Size >= 0)
                    {
                        SizeIsVisible = true;
                        WoundData.Size = dh.Size;
                        WoundSize = $"Wound Size: {WoundData.Size} sq. in.";
                    }
                    if (dh.Img != null)
                    {
                        SizeIsVisible = true;
                        WoundData.Img = dh.Img;
                    }

                    ConfirmButtonVisible = true;
                    ConfirmButtonVisible = (WifiIsVisible || SizeIsVisible);
                    System.Diagnostics.Debug.WriteLine($"Confirm: {ConfirmButtonVisible} - Wifi: {WifiIsVisible} - Size: {SizeIsVisible}");
                }
            }
        }

        private bool _wifiIsVisible;
        public bool WifiIsVisible { get => _wifiIsVisible; set => SetProperty(ref _wifiIsVisible, value); }

        private string _wifiWound;
        public string WifiWound { get => _wifiWound; set => SetProperty(ref _wifiWound, value); }

        private string _wifiIschemia;
        public string WifiIschemia { get => _wifiIschemia; set => SetProperty(ref _wifiIschemia, value); }

        private string _wifiInfection;
        public string WifiInfection { get => _wifiInfection; set => SetProperty(ref _wifiInfection, value); }

        private bool _sizeIsVisible;
        public bool SizeIsVisible { get => _sizeIsVisible; set => SetProperty(ref _sizeIsVisible, value); }

        private string _woundSize;
        public string WoundSize { get => _woundSize; set => SetProperty(ref _woundSize, value); }

        private bool _confirmButtonVisible;
        public bool ConfirmButtonVisible { get => _confirmButtonVisible; set => SetProperty(ref _confirmButtonVisible, value); }
    }


    public class WoundGroup
    {
        public WoundGroup() { }

        public WoundGroup(string n)
        {
            Name = n;
        }

        public string Name { get; set; }
    }
}