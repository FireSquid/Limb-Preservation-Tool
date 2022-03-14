using System;

using Xamarin.Forms;
using Xamarin.Essentials;

using System.Windows.Input;
using System.Threading.Tasks;
using LimbPreservationTool.Views;

using System.Collections.Generic;
using System.Text;

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
            WifiStatus = "Please Enter The Information Below:";
            CalculateWiFICommand = new Command(ClickWifiSubmit);
        }

        void ClickWifiSubmit(object obj)
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

            } catch (Exception ex)
            {
                WifiStatus = $"Please fill out all fields";
                Console.WriteLine(ex.Message);
                return;
            }

            // check for vadility
            bool validation = validateGrades(woundGrade, infectionGrade, ischemiaGrade);
            if (!validation)
            {
                WifiStatus = $"Invalid Input, grades should range between 1-3";
                return;
            } else
            {
                // calculate ischemiaGrade if necessary
                if (ischemiaGrade == -1)
                {
                    try
                    {
                        toePressureGrade = Int32.Parse(toePressureGradeString);
                        ankleBrachialIndex = Double.Parse(ankleBrachialIndexString);
                        ankleSystolicPressure = Int32.Parse(ankleSystolicPressureString);
                    } catch (Exception ex)
                    {
                        WifiStatus = $"Please fill out all fields";
                        Console.WriteLine(ex.Message);
                        return;
                    }
                    ischemiaGrade = CalculateIschemia(toePressureGrade, ankleBrachialIndex, ankleSystolicPressure);
                }
            }

            // calculate and output risk
            amputationRisk = calculateAmputationRisk(woundGrade, infectionGrade, ischemiaGrade, amputationRisk);
            revascularizationRisk = calculateRevascularizationRisk(woundGrade, infectionGrade, ischemiaGrade, revascularizationRisk);
            WifiStatus = $"Your estimate risk for amputation at 1 year is: \n" + amputationRisk + "\n" + "\nYour estimate requirement for revascularization is: \n" + revascularizationRisk;
            // longer message: "Your estimate likelihood of benefit of/requirement for revascularization (assuming your infection can first be controlled) is: 
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

        // grades to calculate ischemia
        private string toePressureGradeString;
        public string ToePressureGrade { get => toePressureGradeString; set => SetProperty(ref toePressureGradeString, value); }

        private string ankleBrachialIndexString;
        public string AnkleBrachialIndex { get => ankleBrachialIndexString; set => SetProperty(ref ankleBrachialIndexString, value); }

        private string ankleSystolicPressureString;
        public string AnkleSystolicPressure { get => ankleSystolicPressureString; set => SetProperty(ref ankleSystolicPressureString, value); }

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
            else if ((toePressure >40) && (toePressure < 59))
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
                else if ((woundGrade == 1)  && (infectionGrade == 0))
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


        private string wifiStatus;
        public string WifiStatus { get => wifiStatus; private set => SetProperty(ref wifiStatus, value); }
    }
}
