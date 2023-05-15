using Autocomp.Nmea.Common;
using Autocomp.Nmea.Common.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocomp.Nmea.Desktop.ViewModels
{
    public partial class ParserViewModel : ObservableObject
    {
        #region Properties
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ParseCommand))]
        [NotifyCanExecuteChangedFor(nameof(CalculateCrcCommand))]
        private char prefix = '$';
        
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ParseCommand))]
        [NotifyCanExecuteChangedFor(nameof(CalculateCrcCommand))]
        private char sufix = '*';
        
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ParseCommand))]
        [NotifyCanExecuteChangedFor(nameof(CalculateCrcCommand))]
        private char seprator = ',';
        
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ParseCommand))]
        [NotifyCanExecuteChangedFor(nameof(CalculateCrcCommand))]
        private string terminator = @"\r\n";
        
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ParseCommand))]
        [NotifyCanExecuteChangedFor(nameof(CalculateCrcCommand))]
        private string userMessage = string.Empty;
        
        [ObservableProperty]
        public string errorMessage = string.Empty;
        
        [ObservableProperty]
        private bool mwvVisible = false;
        
        [ObservableProperty]
        private bool gllVisibile = false;

        [ObservableProperty]
        Gll? gllMessage;
        
        [ObservableProperty]
        Mwv? mwvMessage;
        #endregion

        private bool CanParse
        {
            get =>
                UserMessage != string.Empty &&
                UserMessage.Any(c => c == Prefix) &&
                UserMessage.Any(c=>c==Sufix) && 
                UserMessage.Any(c=>c==Seprator)&&
                (UserMessage.Contains(Terminator) || (Terminator==@"\r\n") && UserMessage.Contains(Environment.NewLine));
        }

        [RelayCommand(CanExecute = nameof(CanParse))]
        private void Parse()
        {
            try
            {
                var parseResult=NmeaParser.Parse(NmeaMessage.FromString(UserMessage, new NmeaFormat(Prefix, Sufix, Seprator, Terminator)));
                if(ErrorMessage!=string.Empty)
                {
                    ErrorMessage = string.Empty;
                }
                if(parseResult is Mwv)
                {
                    MwvMessage = (Mwv)parseResult;
                    MwvVisible= true;
                    GllVisibile = false;
                }
                else if(parseResult is Gll)
                {
                    GllMessage= (Gll)parseResult;
                    GllVisibile = true;
                    MwvVisible = false;
                }
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (NotSupportedException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Metoda ustawiająca wiadomość na predefiniowaną. 
        /// </summary>
        /// <param name="messageNum"></param>
        [RelayCommand]
        private void SetPredefiniedMessage(string messageNum)
        {
            var format = new NmeaFormat(Prefix, Sufix, Seprator, Terminator);
            switch (messageNum)
            {
                case "1":
                    UserMessage = new NmeaMessage("--GLL", new string[] { "5528.58", "N", "12335.16", "E", "162301.25", "A", "D" },format).ToString();
                    break;
                case "2":
                    UserMessage = new NmeaMessage("--GLL", new string[] { "2528.38", "S", "11005.16", "W", "112301.25", "A", "A" },format).ToString();
                    break;
                case "3":
                    UserMessage = new NmeaMessage("--MWV", new string[] { "120.5", "R", "50.5", "K", "A" },format).ToString();
                    break;
                case "4":
                    UserMessage = new NmeaMessage("--MWV", new string[] { "20.5", "T", "10.5", "M", "A" }, format).ToString();
                    break;
                    
            }
            if(UserMessage.Contains("\r\n"))
                UserMessage=UserMessage.Replace("\r\n", @"\r\n");

        }

        /// <summary>
        /// Metoda obliczająca sumę kontrolną i aktualizująca jej wartość we właściwości UserMessage.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanParse))]
        private void CalculateCrc()
        {
            try
            {
               var crc= NmeaCrcCalculator.CRC(NmeaMessage.FromString(UserMessage, new NmeaFormat(Prefix, Sufix, Seprator, Terminator)));
                var sufixIndex = UserMessage.IndexOf(Sufix);
                if (sufixIndex > 0 )
                {
                    if(UserMessage.Length>=sufixIndex+1)
                    {
                        UserMessage = UserMessage.Remove(sufixIndex + 1, UserMessage.Length - sufixIndex-1-Terminator.Length);
                    }

                    UserMessage = UserMessage.Insert(UserMessage.Length-Terminator.Length, crc.ToString("X02"));

                }
                else
                {
                    UserMessage += $"{Sufix}{crc.ToString("X02")}";
                }
            }
            catch(Exception ex)
            {
                
                ErrorMessage = ex.Message;
            }
 
        }
    }
}
