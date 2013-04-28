using System;
using Microsoft.SPOT;

namespace HiveSenseTeam1.Loggers
{
    class DisplayLogger : ILogger
    {
        private Gadgeteer.Modules.GHIElectronics.Display_HD44780 charDisplay_;
        private bool line = false;
        private const int DIGIT_COUNT = 5;

        public DisplayLogger(Gadgeteer.Modules.GHIElectronics.Display_HD44780 charDisplay)
        {
            charDisplay_ = charDisplay;
            charDisplay_.Clear();
        }

        public void OnLogItem(Model.Measurement measurement)
        {
            if( line )
            {
                charDisplay_.SetCursor(0, 0);
            }
            else
            {
                charDisplay_.SetCursor(1, 0);
            }

            string dataString = measurement.Key + ": " + measurement.Value.ToString().Substring(0, DIGIT_COUNT);

            // Padd the data string with spaces to the length of the character
            // display, 16 characters. This will wipe out any rogue characters
            // from previous longer strings on that line.
            if (dataString.Length < 16)
            {
                string emptyString = new string(' ', 16 - dataString.Length);
                dataString += emptyString;
            }
            charDisplay_.PrintString(dataString);

            line = !line;
        }

        public void OnLogItem(Model.Alert alert)
        {
            throw new NotImplementedException();
        }
    }
}
