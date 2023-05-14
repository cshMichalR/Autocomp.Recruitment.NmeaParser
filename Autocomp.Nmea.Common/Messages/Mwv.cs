using System;

namespace Autocomp.Nmea.Common.Messages
{
    public class Mwv
    {
        #region Properties
        public double WindAngle
        {
            get => windAngle;
            set
            {
                if (value < 0 || value > 359)
                {
                    throw new NotSupportedException("Kąt wiatru musi być większy od 0 oraz mniejszy równy 359");
                }            
                    windAngle = value;
            }
        }

        public string Reference
        {
            get => reference;
            set
            {
                if (value == "R" || value == "T")
                {
                    reference = value;
                }
                else
                {
                    throw new NotSupportedException("Niepoprawny typ referencji.");
                }

            }
        }

        public double WindSpeed { get; }

        public string WindSpeedUnits
        {
            get => windSpeedUnits;
            set
            {

                if (value == "K" || value == "M" || value == "N" || value == "S")
                {
                    windSpeedUnits = value;
                }
                else
                {
                    throw new NotSupportedException("Niepoprawna jednostka prędkości wiatru.");
                }

            }
        }

        public string Status
        {
            get => status;
            set
            {
                if (value == "A" || value == "V")
                {
                    status = value;
                }
                else
                {
                    throw new NotSupportedException("Niepoprawny status wiadomości.");
                }
            }

        }

        private double windAngle;
        private string reference;
        private string windSpeedUnits;
        private string status;
        #endregion

        public Mwv(double windAngle, string reference, double windSpeed, string windSpeedUnits, string status)
        {
            this.WindAngle = windAngle;
            this.Reference = reference;
            this.WindSpeed = windSpeed;
            this.WindSpeedUnits = windSpeedUnits;
            this.Status = status;
        }
    }
}
