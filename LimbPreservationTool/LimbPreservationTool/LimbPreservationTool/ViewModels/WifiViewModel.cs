using System;

using Xamarin.Forms;
using Xamarin.Essentials;

using System.Windows.Input;
using System.Threading.Tasks;
using LimbPreservationTool.Views;

using System.Collections.Generic;
using System.Text;
using LimbPreservationTool.Models;

namespace LimbPreservationTool.ViewModels
{
    enum Amputation
    {
        VeryLow,
        Low,
        Medium,
        High,
    }

    enum Revascularization
    {
        VeryLow,
        Low,
        Medium,
        High
    }
    public class WifiViewModel : BaseViewModel
    {

        public WifiViewModel()
        {
            ClearStartInfo();
            WifiStatus = "Please Enter The Information Below";
            WifiColor = Color.Black;
            CalculateWiFICommand = new Command(async () => await ClickWifiSubmit());
            BacktoHome = new Command(async () => await Shell.Current.GoToAsync($"//{nameof(HomePage)}"));

        }

        public void ClearStartInfo()
        {
            WoundGrade = "";
            InfectionGrade = "";
            IschemiaGrade = "";
            ToePressureGrade = "";
            AnkleBrachialIndex = "";
            AnkleSystolicPressure = "";
            SaveDateInternal = DateTime.Parse("1/1/2022");
            wifiStatus = "Please Enter The Information Below";
            wifiColor = Color.Black;

        }

        public void updateInstrucText()
        {
            WifiStatus = $"Please fill out all tests";
            WifiColor = Color.Red;
        }

        async Task ClickWifiSubmit()
        {
            // convert user input from string
            int woundGrade;
            int infectionGrade;
            int ischemiaGrade;
            int toePressureGrade;
            double ankleBrachialIndex;
            int ankleSystolicPressure;
            try
            {
                woundGrade = Int32.Parse(woundGradeString);
                infectionGrade = Int32.Parse(infectionGradeString);
                ischemiaGrade = Int32.Parse(ischemiaGradeString);

            }
            catch (Exception ex)
            {
                WifiStatus = $"Please fill out all fields";
                WifiColor = Color.Red;
                Console.WriteLine(ex.Message);
                return;
            }

            // check for vadility
            bool validation = validateGrades(woundGrade, infectionGrade, ischemiaGrade);
            if (!validation)
            {
                WifiStatus = $"Invalid Input, grades should range between 1-3";
                return;
            }
            else
            {
                // calculate ischemiaGrade if necessary
                if (ischemiaGrade == -1)
                {
                    try
                    {
                        toePressureGrade = Int32.Parse(toePressureGradeString);
                        ankleBrachialIndex = Double.Parse(ankleBrachialIndexString);
                        ankleSystolicPressure = Int32.Parse(ankleSystolicPressureString);
                    }
                    catch (Exception ex)
                    {
                        WifiStatus = $"Please fill out all fields. Keep in mind, grades should fall within a range of 0-3, unless you need your ischemia grade calculated with further tests";
                        Console.WriteLine(ex.Message);
                        return;
                    }
                    ischemiaGrade = CalculateIschemia(toePressureGrade, ankleBrachialIndex, ankleSystolicPressure);
                }
            }



            // calculate and output risk
            amputationRisk = calculateAmputationRisk(woundGrade, infectionGrade, ischemiaGrade, amputationRisk);
            revascularizationRisk = calculateRevascularizationRisk(woundGrade, infectionGrade, ischemiaGrade, revascularizationRisk);

            // Keep reference to wifi data
            var db = (WoundDatabase.Database).GetAwaiter().GetResult();
            if (db.dataHolder == null)
                db.dataHolder = DBWoundData.Create(Guid.Empty);
            db.dataHolder.SetWifi(woundGrade, infectionGrade, ischemiaGrade);

            Color ampColor = Color.Black;
            Color revascColor = Color.Black;

            if (amputationRisk == Amputation.VeryLow)
            {
                ampColor = Color.Green;
            }
            else if (amputationRisk == Amputation.Low)
            {
                ampColor = Color.Yellow;
            }
            else if (amputationRisk == Amputation.Medium)
            {
                ampColor = Color.Orange;
            }
            else if (amputationRisk == Amputation.High)
            {
                ampColor = Color.Red;
            }

            if (revascularizationRisk == Revascularization.VeryLow)
            {
                revascColor = Color.Green;
            }
            else if (revascularizationRisk == Revascularization.Low)
            {
                revascColor = Color.Yellow;
            }
            else if (revascularizationRisk == Revascularization.Medium)
            {
                revascColor = Color.Orange;
            }
            else if (revascularizationRisk == Revascularization.High)
            {
                revascColor = Color.Red;
            }

            WifiStatus = $"Please Enter The Information Below";
            WifiColor = Color.Black;


            await App.Current.MainPage.Navigation.PushAsync(new WifiResultPage(amputationRisk.ToString(), revascularizationRisk.ToString(), ampColor, revascColor));

            ClearStartInfo();
        }


        public ICommand CalculateWiFICommand { get; }

        Amputation amputationRisk = Amputation.VeryLow;
        Revascularization revascularizationRisk = Revascularization.VeryLow;

        // grades for WIFI score
        private string woundGradeString;
        public string WoundGrade { get => woundGradeString; set => SetProperty(ref woundGradeString, value); }

        private string infectionGradeString;
        public string InfectionGrade { get => infectionGradeString; set => SetProperty(ref infectionGradeString, value); }

        private string ischemiaGradeString;
        public string IschemiaGrade { get => ischemiaGradeString; set => SetProperty(ref ischemiaGradeString, value); }

        private DateTime saveDateString;
        public DateTime SaveDateInternal { get => saveDateString; set => SetProperty(ref saveDateString, value); }

        // grades to calculate ischemia
        private string toePressureGradeString;
        public ICommand BacktoHome { get; }
        public string ToePressureGrade { get => toePressureGradeString; set => SetProperty(ref toePressureGradeString, value); }

        private string ankleBrachialIndexString;
        public string AnkleBrachialIndex { get => ankleBrachialIndexString; set => SetProperty(ref ankleBrachialIndexString, value); }

        private string ankleSystolicPressureString;
        public string AnkleSystolicPressure { get => ankleSystolicPressureString; set => SetProperty(ref ankleSystolicPressureString, value); }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //lets the Entry be empty
            if (string.IsNullOrEmpty(e.NewTextValue)) return;

            var temp = e.NewTextValue.ToCharArray();

            for (int i = 0; i < temp.Length; i++)
            {
                if (string.Equals(temp[i], '.') || string.Equals(temp[i], ','))
                {
                    e.NewTextValue.Remove(e.NewTextValue.Length - 1);
                    return;
                }
            }
            //}

            if (!int.TryParse(e.NewTextValue, out int value))
            {
                ((Entry)sender).Text = e.OldTextValue;
                return;
            }

            int checkRange = 0;
            checkRange = int.Parse(e.NewTextValue);
            if (checkRange > 3 || checkRange < -1)
            {
                ((Entry)sender).Text = e.OldTextValue;
            }
        }

        // function to validate that each grade falls within ranges we calculate for

        private static bool validateGrades(int woundGrade, int infectionGrade, int ischemiaGrade)
        {
            bool woundGradeValid = (woundGrade > -1) && (woundGrade < 4);
            bool infectionGradeValid = (infectionGrade > -1) && (infectionGrade < 4);
            bool ischemiaGradeValid = (ischemiaGrade > -2) && (ischemiaGrade < 4);
            return woundGradeValid && infectionGradeValid && ischemiaGradeValid;
        }


        // function to calculate ischemia grade if it wasn't yet provided
        private static int CalculateIschemia(int toePressure, double ankleBrachialIndex, int ankleSystolicPressure)
        {
            int ischemiaGrade;
            if ((toePressure > 60) && (ankleBrachialIndex > 0.8) && (ankleSystolicPressure > 100))
            {
                ischemiaGrade = 0;
            }
            else if ((toePressure > 40) && (toePressure < 59) && (ankleBrachialIndex > 0.6) && (ankleBrachialIndex < 0.79) && (ankleSystolicPressure > 70) && (ankleSystolicPressure < 100))
            {
                ischemiaGrade = 1;
            }
            else if ((toePressure > 30) && (toePressure < 39) && (ankleBrachialIndex > 0.4) && (ankleBrachialIndex < 0.59) && (ankleSystolicPressure > 50) && (ankleSystolicPressure < 70))
            {
                ischemiaGrade = 2;
            }
            else if ((toePressure < 30) && (ankleBrachialIndex < 0.39) && (ankleSystolicPressure < 30))
            {
                ischemiaGrade = 3;
            }
            else if (toePressure > 60)
            {
                ischemiaGrade = 0;
            }
            else if ((toePressure > 40) && (toePressure < 59))
            {
                ischemiaGrade = 1;
            }
            else if ((toePressure > 30) && (toePressure < 39))
            {
                ischemiaGrade = 2;
            }
            else if (toePressure < 30)
            {
                ischemiaGrade = 3;
            }
            else
            {
                ischemiaGrade = -1;
            }
            return ischemiaGrade;
        }

        private static Amputation calculateAmputationRisk(int woundGrade, int infectionGrade, int ischemiaGrade, Amputation ampScore)
        {
            if (ischemiaGrade == 0)
            {
                if ((woundGrade == 0) && (infectionGrade == 0) || (infectionGrade == 1))
                {
                    ampScore = Amputation.VeryLow;
                }
                else if ((woundGrade == 1) && (infectionGrade == 0) || (infectionGrade == 1))
                {
                    ampScore = Amputation.VeryLow;
                }
                else if ((infectionGrade == 2) && (woundGrade == 0) || (woundGrade == 1))
                {
                    ampScore = Amputation.Low;
                }
                else if ((woundGrade == 2) && (infectionGrade == 0) || (infectionGrade == 1))
                {
                    ampScore = Amputation.Low;
                }
                else if ((woundGrade == 3) && (infectionGrade == 2) || (infectionGrade == 3))
                {
                    ampScore = Amputation.High;
                }
                else if ((woundGrade == 2) && (infectionGrade == 3))
                {
                    ampScore = Amputation.High;
                }
            }

            if (ischemiaGrade == 1)
            {
                if ((infectionGrade == 0) && (woundGrade == 0) || (woundGrade == 1))
                {
                    ampScore = Amputation.VeryLow;
                }
                else if ((infectionGrade == 1) && (woundGrade == 0) || (woundGrade == 1))
                {
                    ampScore = Amputation.Low;
                }
                else if ((infectionGrade == 2) && (woundGrade == 0) || (woundGrade == 1))
                {
                    ampScore = Amputation.Medium;
                }
                else if ((woundGrade == 2) && (infectionGrade == 0) || (infectionGrade == 1))
                {
                    ampScore = Amputation.Medium;
                }
                else
                {
                    ampScore = Amputation.High;
                }
            }

            if (ischemiaGrade == 2)
            {
                if ((woundGrade == 0) && (infectionGrade == 0) || (infectionGrade == 1))
                {
                    ampScore = Amputation.Low;
                }
                else if ((woundGrade == 1) && (infectionGrade == 0))
                {
                    ampScore = Amputation.Low;
                }
                else if ((woundGrade == 0) && (infectionGrade == 2))
                {
                    ampScore = Amputation.Medium;
                }
                else if ((woundGrade == 1) && (infectionGrade == 1))
                {
                    ampScore = Amputation.Medium;
                }
                else if ((woundGrade == 2) && (infectionGrade == 0))
                {
                    ampScore = Amputation.Medium;
                }
                else
                {
                    ampScore = Amputation.High;
                }
            }

            if (ischemiaGrade == 3)
            {
                if ((woundGrade == 2) || (woundGrade == 3))
                {
                    ampScore = Amputation.High;
                }
                else if ((woundGrade == 1) && (infectionGrade == 2) || (infectionGrade == 3))
                {
                    ampScore = Amputation.High;
                }
                else if ((woundGrade == 1) && (infectionGrade == 0) || (infectionGrade == 1))
                {
                    ampScore = Amputation.Medium;
                }
                else if ((woundGrade == 0) && (infectionGrade == 0))
                {
                    ampScore = Amputation.Low;
                }
                else if ((woundGrade == 0) && (infectionGrade == 1) || (infectionGrade == 2))
                {
                    ampScore = Amputation.Medium;
                }
                else if ((woundGrade == 0) && (infectionGrade == 3))
                {
                    ampScore = Amputation.High;
                }
            }
            return ampScore;
        }

        private static Revascularization calculateRevascularizationRisk(int woundGrade, int infectionGrade, int ischemiaGrade, Revascularization revScore)
        {
            if (ischemiaGrade == 0)
            {
                revScore = Revascularization.VeryLow;
            }

            if (ischemiaGrade == 1)
            {
                if ((woundGrade == 0) && (infectionGrade == 0))
                {
                    revScore = Revascularization.VeryLow;
                }
                else if ((woundGrade == 0) && (infectionGrade == 1) || (infectionGrade == 2))
                {
                    revScore = Revascularization.Low;
                }
                else if ((woundGrade == 1) && (infectionGrade == 0))
                {
                    revScore = Revascularization.Low;
                }
                else if ((woundGrade == 2) && (infectionGrade == 2) || (infectionGrade == 3))
                {
                    revScore = Revascularization.High;
                }
                else if ((woundGrade == 3) && (infectionGrade == 3))
                {
                    revScore = Revascularization.High;
                }
                else
                {
                    revScore = Revascularization.Medium;
                }
            }

            if (ischemiaGrade == 2)
            {
                if ((woundGrade == 0) && (infectionGrade == 0) || (infectionGrade == 1))
                {
                    revScore = Revascularization.Low;
                }
                else if ((woundGrade == 0) && (infectionGrade == 2) || (infectionGrade == 3))
                {
                    revScore = Revascularization.Medium;
                }
                else if ((woundGrade == 1) && (infectionGrade == 0))
                {
                    revScore = Revascularization.Medium;
                }
                else
                {
                    revScore = Revascularization.High;
                }
            }

            if ((ischemiaGrade == 3) && (infectionGrade == 0))
            {
                revScore = Revascularization.Medium;
            }
            else if (ischemiaGrade == 3)
            {
                revScore = Revascularization.High;
            }

            return revScore;
        }


        public string wifiStatus;
        public string WifiStatus { get => wifiStatus; private set => SetProperty(ref wifiStatus, value); }

        private string testText;
        public string TestText { get => testText; private set => SetProperty(ref testText, value); }

        private Color wifiColor;
        public Color WifiColor { get => wifiColor; private set => SetProperty(ref wifiColor, value); }
    }
}
