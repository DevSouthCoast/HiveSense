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
        public event EventHandler AlarmReady;

        public HiveMonitor(Configuration config)
        {
            config_ = config;
        }
    }


}
