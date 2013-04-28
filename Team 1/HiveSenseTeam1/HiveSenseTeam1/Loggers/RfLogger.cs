using System;
using Gadgeteer.Modules.IngenuityMicro;
using HiveSenseTeam1.Model;
using Microsoft.SPOT;

namespace HiveSenseTeam1.Loggers
{
    class RfLogger : ILogger
    {
        private readonly RfPipe _rfPipe;

        public RfLogger(RfPipe rfPipe)
        {
            _rfPipe = rfPipe;
        }

        public void OnLogItem(Measurement measurement)
        {
            var timestamp = measurement.TimeStamp.TimeOfDay.ToString();
            var data = measurement.Key + measurement.Value.ToString();
            _rfPipe.SendData(MeasureType.Time + timestamp);
            _rfPipe.SendData(data);
        }

        public void OnLogItem(Alert alert)
        {
            _rfPipe.SendData(MeasureType.Alert + alert.Message);
        }
    }
}
