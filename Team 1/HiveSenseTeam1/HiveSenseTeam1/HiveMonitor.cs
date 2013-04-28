using System;
using Microsoft.SPOT;
using HiveSenseTeam1.Model;

namespace HiveSenseTeam1
{
    class HiveMonitor
    {
        private Configuration config_;
        public delegate void AlarmReadyHandler(Alert measurement);
        public delegate void MeasurementReadyHandler(Measurement measurement);
        public event MeasurementReadyHandler MeasurementReady;
        public event AlarmReadyHandler AlarmReady;

        public HiveMonitor(Configuration config)
        {
            config_ = config;
        }

        public void TestEvents()
        {
            if (MeasurementReady != null)
            {
                MeasurementReady(new Measurement { Key = "TempDegC", TimeStamp = System.DateTime.UtcNow, Value = 0.0 });
            }
            if (AlarmReady != null)
            {
                AlarmReady(new Alert { Key = "Shaken", Message = "It's wobbling", RecordedValue = 3, Threshold = 2, TimeStamp = System.DateTime.UtcNow });
            }
        }
    }
}
