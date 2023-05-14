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
        /// <summary>
        /// Test weryfikuj¹cy, czy parser w przypadku wiadomoœci z nieobs³ugiwanym nag³ówkiem zwróci ArgumentException. 
        /// </summary>
        [Test]
        public void Parse_WrongHeader_ThrowsArgumentException()
        {
            var messageWithNotImplementedHeader = new NmeaMessage("WRONG_HEADER",
                new string[] { "5528.58", "N", "1835.16", "E", "110501.35", "A" });
            Assert.Throws<ArgumentException>(() => NmeaParser.Parse(messageWithNotImplementedHeader));
        }
        /// <summary>
        /// Test weryfikuj¹cy poprawnoœæ parsowania porównuj¹c wynik parsowania ze zdefiniowanym obiektem.
        /// </summary>
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
        /// <summary>
        /// Test weryfikuj¹cy poprawnoœæ parsowania porównuj¹c wynik parsowania ze zdefiniowanym obiektem.
        /// </summary>
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
        /// <summary>
        /// Weryfikuje, czy parser zwróci argument exception w przypadku wiadomoœci o nieodpowiedniej iloœci pól.
        /// </summary>
        [Test]
        public void Parse_MWVMessageWithInvalidFieldsLength_ThrowsArgumentException()
        {
            var nmeaMessage = new NmeaMessage("MWV",
            new string[] { "126.5", "R", "56.5", "K", "A","AdditionalField" });
            Assert.Throws<ArgumentException>(()=>NmeaParser.Parse(nmeaMessage));
        }
        /// <summary>
        /// Weryfikuje, czy parser zwróci argument exception w przypadku wiadomoœci o nieodpowiedniej iloœci pól.
        /// </summary>
        [Test]
        public void Parse_GLLMessageWithInvalidFieldsLength_ThrowsArgumentException()
        {
            //Jedno brakuj¹ce pole.
            var nmeaMessage = new NmeaMessage("GLL",
             new string[] { "5528.58", "N", "1835.16", "162301.25", "A", "D" });
            Assert.Throws<ArgumentException>(() => NmeaParser.Parse(nmeaMessage));
        }
    }
}