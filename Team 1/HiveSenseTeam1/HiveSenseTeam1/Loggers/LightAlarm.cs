using System;
using Microsoft.SPOT;
using GT = Gadgeteer;

namespace HiveSenseTeam1.Loggers
{
    class LightAlarm : ILogger
    {
        Gadgeteer.Modules.GHIElectronics.MulticolorLed multicolorLed_;

        public LightAlarm(Gadgeteer.Modules.GHIElectronics.MulticolorLed multicolorLed)
        {
            multicolorLed_ = multicolorLed;
        }

        public void OnLogItem(Model.Measurement measurement)
        {
            throw new NotImplementedException();
        }

        public void OnLogItem(Model.Alert alert)
        {
            // determine the type
            switch (alert.Key)
            {
                case "LightSense":
                    multicolorLed_.BlinkOnce(GT.Color.Green);
                    break;
                case "Accelerometer":
                    multicolorLed_.BlinkOnce(GT.Color.Red);
                    break;
            }
        }
    }
}
