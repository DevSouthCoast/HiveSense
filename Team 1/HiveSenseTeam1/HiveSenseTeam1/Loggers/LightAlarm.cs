using System;
using Gadgeteer.Modules.GHIElectronics;
using GT = Gadgeteer;

namespace HiveSenseTeam1.Loggers
{
    class LightAlarm : ILogger
    {
        MulticolorLed multicolorLed_;

        public LightAlarm(MulticolorLed multicolorLed)
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
