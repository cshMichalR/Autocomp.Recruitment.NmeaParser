﻿using Autocomp.Nmea.Common.Messages;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocomp.Nmea.Common
{
    public static partial class NmeaParser
    {
        public const string timeFormat = "HHmmss.FFF";

        /// <summary>
        /// Parsuje wiadomość NMEA do klasy reprezentującej wiadomość.
        /// </summary>
        /// <param name="message"> Wiadomość NMEA</param>
        /// <returns>Instancje klasy reprezentującej typ wiadomości.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static object Parse(NmeaMessage message)
        {
            switch (message.Header.ToUpper())
            {
                case "--MWV":
                    return Parse<Mwv>(message);
                case "--GLL":
                    return Parse<Gll>(message);
                default:
                    throw new ArgumentException($"Parsowanie wiadomości z nagłówkiem {message.Header} jest nie zaimplementowane.");
            }
        }
        /// <summary>
        /// Parsuje wiadomość NMEA do klasy reprezentującej wiadomość.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="message"></param>
        /// <returns>Instancje klasy reprezentującej typ wiadomości.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static TValue Parse<TValue>(NmeaMessage message)
        {
            var typeName = typeof(TValue).Name;
            if (!message.Header.Trim('-').Equals(typeName,StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Wiadomość ma inny nagłówek niż żądany typ. Parsowanie jest niemożliwe.");
            }

            switch (typeName)
            {
                case nameof(Mwv):
                    return (TValue)ParseToMwv(message);
                case nameof(Gll):
                    return (TValue)ParseToGll(message);
                default:
                    throw new ArgumentException($"Parsowanie do typu {typeof(TValue).Name} jest niemożliwe.");
            }
        }
        /// <summary>
        /// Metoda konwertująca szerokość geograficzną do zapisu w postaci stopni, minut, sekund. 
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public static string LatitudeToString(double latitude)
        {
            var sign = latitude < 0 ? "-" : string.Empty;
            latitude = Math.Abs(latitude);
            var latitudeString = Math.Round(latitude,2).ToString("0000.00");
            var degrees = latitudeString.Substring(0, 2);
            var minutes = Convert.ToDouble(latitudeString.Substring(2, 5))*0.6;
            var seconds = Math.Round(Convert.ToDouble(Math.Round(minutes, 4).ToString("00.0000").Substring(2,5))*60.0,1);
            return $"{sign}{degrees}°{minutes.ToString("00")}'{seconds.ToString("00.0", CultureInfo.InvariantCulture)}''";
        }

        /// <summary>
        /// Metoda konwertująca długość geograficzną z zapisu dziesiętnego do zapisu w postaci stopni, minut i sekund. 
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public static string LongitudeToString(double longitude)
        {
            var sign = longitude < 0 ? "-" : string.Empty;
            longitude=Math.Abs(longitude);
            var longitudeString = longitude.ToString("00000.00");
            var degrees=longitudeString.Substring(0, 3);
            var minutes = Convert.ToDouble(longitudeString.Substring(3, 5)) * 0.6;
            var seconds = Math.Round(Convert.ToDouble(Math.Round(minutes, 4).ToString("00.0000").Substring(2, 5)) * 60, 1);
            return $"{sign}{degrees}°{minutes.ToString("00")}'{seconds.ToString("00.0", CultureInfo.InvariantCulture)}''";
        }

        /// <summary>
        /// Metoda konwertująca wartość kątową zapisaną w sposób dziesiętny do zapisu w stopniach i minutach.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static string DegreesToString(double degrees)
        {
            var sign= degrees<0?"-":string.Empty;
            degrees=Math.Abs(degrees);
            var deg = Math.Truncate(degrees);
            var min = Math.Round((degrees - deg)*60);
            return $"{sign}{deg}°{min}'";
        }
    }

    public static partial class NmeaParser
    {
        /// <summary>
        /// Metoda parsująca wiadomość typu MWV do instancji klasy.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Instancje klasy MWV</returns>
        /// <exception cref="ArgumentException"></exception>
        private static object ParseToMwv(NmeaMessage message)
        {
            if (message.Fields.Length != 5)
            {
                throw new ArgumentException($"Niepoprawna ilość pól w wiadomości typu {message.Header}");
            }
            double windAngle;
            double windSpeed;

            if (!double.TryParse(message.Fields[0], NumberStyles.Number, CultureInfo.InvariantCulture, out windAngle))
            {
                throw new NotSupportedException("Niepoprawny format kąta wiatru.");
            }

            if (!double.TryParse(message.Fields[2], NumberStyles.Number, CultureInfo.InvariantCulture, out windSpeed))
            {
                throw new NotSupportedException("Niepoprawny format prędkości wiatru.");
            }

            return new Mwv(
                windAngle,
                reference: message.Fields[1],
                windSpeed: Convert.ToDouble(message.Fields[2], CultureInfo.InvariantCulture),
                windSpeedUnits: message.Fields[3],
                status: message.Fields[4]);
        }
        /// <summary>
        ///  Metoda parsująca wiadomość typu Gll do instancji klasy.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static object ParseToGll(NmeaMessage message)
        {
            if (message.Fields.Length != 7)
            {
                throw new ArgumentException($"Niepoprawna ilość pól w wiadomości typu {message.Header}");
            }
            if (message.Fields[0].Length > 8)
            {
                throw new ArgumentException("Niepoprawny format szerokości geograficznej");
            }
            if (message.Fields[2].Length > 9)
            {
                throw new ArgumentException("Niepoprawny format długości geograficznej");
            }

            double latitude;
            double longitude;
            DateTime positionUtc;

            if (!double.TryParse(message.Fields[0], NumberStyles.Number, CultureInfo.InvariantCulture, out latitude))
            {
                throw new NotSupportedException("Niepoprawny format szerokości geograficznej.");
            }

            if (!double.TryParse(message.Fields[2], NumberStyles.Number, CultureInfo.InvariantCulture, out longitude))
            {
                throw new NotSupportedException("Niepoprawny format długości geograficznej.");
            }
            if (!DateTime.TryParseExact(message.Fields[4], timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out positionUtc))
            {
                throw new NotSupportedException("Niepoprawny format czasu UTC.");
            }

            return new Gll(
            latitude,
            latitudeDirection: message.Fields[1],
            longitude,
            longitudeDirection: message.Fields[3],
            positionUtc.TimeOfDay,
            dataStatus: message.Fields[5],
            modeIndicator: message.Fields[6]);
        }

    }
}
