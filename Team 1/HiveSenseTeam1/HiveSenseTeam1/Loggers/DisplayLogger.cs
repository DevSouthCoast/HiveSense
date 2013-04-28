using System;
using Microsoft.SPOT;

namespace HiveSenseTeam1.Loggers
{
    class DisplayLogger : ILogger
    {
        private Gadgeteer.Modules.GHIElectronics.Display_HD44780 charDisplay_;
        private bool line = false;

        public DisplayLogger(Gadgeteer.Modules.GHIElectronics.Display_HD44780 charDisplay)
        {
            charDisplay_ = charDisplay;
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
            charDisplay_.PrintString(measurement.Key + ": " + measurement.Value.ToString().Substring(0,5));
            line = !line;
        }

        public void OnLogItem(Model.Alert alert)
        {
            throw new NotImplementedException();
        }
    }
}
