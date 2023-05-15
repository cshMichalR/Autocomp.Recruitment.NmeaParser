using Autocomp.Nmea.Common.Messages;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Diagnostics;
using System.Globalization;

namespace Autocomp.Nmea.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Parse_WrongHeader_ThrowsArgumentException()
        {
            var messageWithNotImplementedHeader = new NmeaMessage("WRONG_HEADER",
                new string[] { "5528.58", "N", "1835.16", "E", "110501.35", "A" });
            Assert.Throws<ArgumentException>(() => NmeaParser.Parse(messageWithNotImplementedHeader));
        }

        [Test]
        public void Parse_CorrectGllMessage_CompareWithDefinedResult_ShouldBeEqual()
        {
            var nmeaMessage = new NmeaMessage("GLL",
             new string[] { "5528.58", "N", "1835.16", "E", "162301.25", "A","D" });
            
            var parseResult = NmeaParser.Parse<Gll>(nmeaMessage);

            var excpectedResult=new Gll(
                latitude: 5528.58,
                latitudeDirection:"N",
                longitude: 1835.16,
                longitudeDirection:"E",
                positonUtc: new TimeSpan(0,16,23,01,250),
                dataStatus:"A",
                modeIndicator:"D");

            parseResult.Should().BeEquivalentTo(excpectedResult);
        }

        [Test]
        public void Parse_CorrectMWVMessage_CompareWithDefinedResult_ShouldBeEqual()
        {
            var nmeaMessage = new NmeaMessage("MWV",
            new string[] { "126.5", "R", "56.5", "K", "A"});

            var parseResult = NmeaParser.Parse<Mwv>(nmeaMessage);
            var excpectedResult = new Mwv(
                windAngle:126.5,
                reference:"R",
                windSpeed:56.5,
                windSpeedUnits:"K",
                status:"A");

            parseResult.Should().BeEquivalentTo(excpectedResult);
        }

        [Test]
        public void Parse_MWVMessageWithInvalidFieldsLength_ThrowsArgumentException()
        {
            var nmeaMessage = new NmeaMessage("MWV",
            new string[] { "126.5", "R", "56.5", "K", "A","AdditionalField" });
            Assert.Throws<ArgumentException>(()=>NmeaParser.Parse(nmeaMessage));
        }

        [Test]
        public void Parse_GLLMessageWithInvalidFieldsLength_ThrowsArgumentException()
        {
            //Jedno brakuj¹ce pole.
            var nmeaMessage = new NmeaMessage("GLL",
             new string[] { "5528.58", "N", "1835.16", "162301.25", "A", "D" });
            Assert.Throws<ArgumentException>(() => NmeaParser.Parse(nmeaMessage));
        }

        [Test]
        public void Parse_Latitude_CompareWithCorrectValue_1()
        {
            var parsedLatitude=NmeaParser.LatitudeToString(0232.12);
            var correctLatitude = "02°19'16.3''";
            Assert.IsTrue(parsedLatitude == correctLatitude);
        }
        [Test]
        public void Parse_Latitude_CompareWithCorrectValue_2()
        {
            var parsedLatitude = NmeaParser.LatitudeToString(0.0);
            var correctLatitude = "00°00'00.0''";
            Assert.IsTrue(parsedLatitude == correctLatitude);
        }
        [Test]
        public void Parse_Latitude_CompareWithCorrectValue_3()
        {
            var parsedLatitude = NmeaParser.LatitudeToString(-0232.12);
            var correctLatitude = "-02°19'16.3''";
            Assert.IsTrue(parsedLatitude == correctLatitude);
        }
        [Test]
        public void Parse_Longitude_CompareWithCorrectValue_1()
        {
            var parsedLongitude = NmeaParser.LongitudeToString(12350.32);
            var correctLongitude = "123°30'11.5''";
            Assert.IsTrue(parsedLongitude == correctLongitude);
        }
        [Test]
        public void Parse_Longitude_CompareWithCorrectValue_2()
        {
            var parsedLongitude = NmeaParser.LongitudeToString(0.0);
            var correctLongitude = "000°00'00.0''";
            Assert.IsTrue(parsedLongitude == correctLongitude);
        }
        [Test]
        public void Parse_Longitude_CompareWithCorrectValue_3()
        {
            var parsedLongitude = NmeaParser.LongitudeToString(-12350.32);
            var correctLongitude = "-123°30'11.5''";
            Assert.IsTrue(parsedLongitude == correctLongitude);
        }

        [Test]
        public void Parse_Degrees_CompareWithCorrectValue_1()
        {
            var parsedDegrees = NmeaParser.DegreesToString(320.5);
            var correctDegrees = "320°30'";
            Assert.IsTrue(parsedDegrees == correctDegrees);
        }

        [Test]
        public void Parse_Degrees_CompareWithCorrectValue_2()
        {
            var parsedDegrees = NmeaParser.DegreesToString(-121.3);
            var correctDegrees = "-121°18'";
            Assert.IsTrue(parsedDegrees == correctDegrees);
        }

        [Test]
        public void Parse_Degrees_CompareWithCorrectValue_3()
        {
            var parsedDegrees = NmeaParser.DegreesToString(-0.0);
            var correctDegrees = "0°0'";
            Assert.IsTrue(parsedDegrees == correctDegrees);
        }
    }
}