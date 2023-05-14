using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocomp.Nmea.Common.Messages
{
    public class Gll
    {
        #region Properties
        public double Latitude
        {
            get => latitude;
            private set
            {
                if (value > 9000 || value < -9000)
                {
                    throw new NotSupportedException("Szerokość geograficzna nie może być większa od 90 stopni.");
                }
                if (value != latitude)
                    latitude = value;
            }
        }


        public string LatitudeDirection
        {
            get => latitudeDirection;
            private set
            {
                if (value == "N" || value == "S")
                {
                    if (value != latitudeDirection)
                        latitudeDirection = value;
                }
                else
                {
                    throw new NotSupportedException("Niepoprawny kierunek szerokości geograficznej.");
                }
            }

        }

        public double Longitude
        {
            get => longitude;
            set
            {
                if (value > 18000 || value < -18000)
                {
                    throw new NotSupportedException("Niepoprawna długość geograficzna.");
                }
                else
                {
                    if (value != longitude)
                        longitude = value;
                }
            }
        }

        public string LongitudeDirection
        {
            get => longitudeDirection;
            set
            {
                if (value == "W" || value == "E")
                {
                    longitudeDirection = value;
                }
                else
                {
                    throw new NotSupportedException("Niepoprawny kierunek długości geograficznej.");
                }  
            }
        }

        public TimeSpan PositionUtc { get; }
        public string ModeIndicator 
        {
            get => modeIndicator;
            set
            {
                if (value == "A" || value == "D" || value == "E" || value == "M" || value == "S" || value == "N")
                {
                    modeIndicator = value;
                }
                else
                {
                    throw new NotSupportedException("Nieznany tryb systemu pozycjonowania.");
                }
            }
        }

        public string DataStatus
        {
            get => dateStatus;
            set
            {
                if (value == "A" || value == "V")
                {
                    dateStatus = value;
                }
                else
                {
                    throw new NotSupportedException("Niepoprawny status.");
                }
            }
        }

        private double latitude;
        private double longitude;
        private string latitudeDirection;
        private string longitudeDirection;
        private string dateStatus;
        private string modeIndicator;
        #endregion
        public Gll(double latitude, string latitudeDirection, double longitude,
            string longitudeDirection, TimeSpan positonUtc, string dataStatus, string modeIndicator)
        {
            this.Latitude = latitude;
            this.LatitudeDirection = latitudeDirection;
            this.Longitude = longitude;
            this.LongitudeDirection = longitudeDirection;
            this.PositionUtc = positonUtc;
            this.DataStatus = dataStatus;
            this.ModeIndicator = modeIndicator;
        }

 
    }
}
