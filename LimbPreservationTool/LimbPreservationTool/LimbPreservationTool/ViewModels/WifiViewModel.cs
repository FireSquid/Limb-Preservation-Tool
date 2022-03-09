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
            WifiStatus = "Please Enter The Information Below";
            CalculateWiFICommand = new Command(ClickWifiSubmit);
        }

        void ClickWifiSubmit(object obj)
        {
            
            amputationRisk = calculateAmputationRisk(woundGrade, infectionGrade, ischemiaGrade, amputationRisk);
            revascularizationRisk = calculateRevascularizationRisk(woundGrade, infectionGrade, ischemiaGrade, revascularizationRisk);
            WifiStatus = $"Your estimate risk for amputation at 1 year is: \n" + amputationRisk + "\n" + "Your estimate likelihood of benefit of/requirement for revascularization (assuming your infection can first be controlled) is: \n" + revascularizationRisk;
            // output risks
            // Console.WriteLine("Your estimate risk for amputation at 1 year is: " + amputationRisk);
            // Console.WriteLine("Your estimate likelihood of benefit of/requirement for revascularization (assuming your infection can first be controlled) is: " + revascularizationRisk);
        }


        public ICommand CalculateWiFICommand { get; }

        Amputation amputationRisk = Amputation.VeryLow;
        Revascularization revascularizationRisk = Revascularization.VeryLow;

        // grades for WIFI score
        private int woundGrade;
        public int WoundGrade { get => woundGrade; set => SetProperty(ref woundGrade, value); }

        private int infectionGrade;
        public int InfectionGrade { get => infectionGrade; set => SetProperty(ref infectionGrade, value); }

        private int ischemiaGrade;
        public int IschemiaGrade { get => ischemiaGrade; set => SetProperty(ref ischemiaGrade, value); }

        // grades to calculate ischemia
        private int toePressureGrade;
        public int ToePressureGrade { get => toePressureGrade; set => SetProperty(ref toePressureGrade, value); }

        private double ankleBrachialIndex;
        public double AnkleBrachialIndex { get => ankleBrachialIndex; set => SetProperty(ref ankleBrachialIndex, value); }

        private int ankleSystolicPressure;
        public int AnkleSystolicPressure { get => ankleSystolicPressure; set => SetProperty(ref ankleSystolicPressure, value); }


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