using System;
using Gadgeteer.Modules.Seeed;
using Microsoft.SPOT;
using HiveSenseTeam1.Model;
using GT = Gadgeteer;

namespace HiveSenseTeam1
{
    class HiveMonitor
    {
        private Configuration config_;
        public delegate void AlarmReadyHandler(Alert measurement);
        public delegate void MeasurementReadyHandler(Measurement measurement);
        public event MeasurementReadyHandler MeasurementReady;
        public event AlarmReadyHandler AlarmReady;
        private GT.Timer loggingTimer_;
        private TemperatureHumidity temperatureHumidity;
        private GPS gps;
        private DateTime gpsFixTimeUTC;

        public HiveMonitor(Configuration config, TemperatureHumidity temperatureHumidity, GPS gps)
        {
            config_ = config;
            this.temperatureHumidity = temperatureHumidity;
            this.gps = gps;

            loggingTimer_ = new GT.Timer(5000);
            loggingTimer_.Stop();
            loggingTimer_.Tick += new GT.Timer.TickEventHandler(loggingTimer_Tick);
            //TODO Use config
            loggingTimer_.Start();

            temperatureHumidity.MeasurementComplete += new TemperatureHumidity.MeasurementCompleteEventHandler(temperatureHumidity_MeasurementComplete);

            gps.PositionReceived += new GPS.PositionReceivedHandler(gps_PositionReceived);
        }

        void loggingTimer_Tick(GT.Timer timer)
        {
            temperatureHumidity.RequestMeasurement();
        }

        void temperatureHumidity_MeasurementComplete(TemperatureHumidity sender, double temperature, double relativeHumidity)
        {
            var measurementTemp = new Measurement { TimeStamp = gpsFixTimeUTC, Key = "TempDegC", Value = temperature };
            var measurementHumidity = new Measurement { TimeStamp = gpsFixTimeUTC, Key = "HumidityPc", Value = relativeHumidity };

            if (MeasurementReady != null)
                {
                MeasurementReady(measurementTemp);
                MeasurementReady(measurementHumidity);
            }
        }

        void gps_PositionReceived(GPS sender, GPS.Position position)
        {
            loggingTimer_.Start();
            gpsFixTimeUTC = gps.LastPosition.FixTimeUtc;
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
