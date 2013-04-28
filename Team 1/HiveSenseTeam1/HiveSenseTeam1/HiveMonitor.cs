using System;
using Gadgeteer.Modules.Seeed;
using Microsoft.SPOT;
using HiveSenseTeam1.Model;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;

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
        private TemperatureHumidity temperatureHumidity_;
        private GPS gps_;
        private DateTime gpsFixTimeUTC;
        private LightSensor lightSensor_;

        public HiveMonitor(
            Configuration config,
            TemperatureHumidity temperatureHumidity,
            GPS gps,
            LightSensor lightSensor)
        {
            config_ = config;
            temperatureHumidity_ = temperatureHumidity;
            gps_ = gps;
            lightSensor_ = lightSensor;

            loggingTimer_ = new GT.Timer(5000);
            loggingTimer_.Stop();
            loggingTimer_.Tick += new GT.Timer.TickEventHandler(loggingTimer_Tick);

            if (config_[Configuration.LogIfNoGPSFix] == 1)
            {
                Debug.Print("Starting logging timer...");
                loggingTimer_.Start();
            }

            temperatureHumidity.MeasurementComplete += new TemperatureHumidity.MeasurementCompleteEventHandler(temperatureHumidity_MeasurementComplete);

            gps.PositionReceived += new GPS.PositionReceivedHandler(gps_PositionReceived);

            StartCheckingLightLevels();
        }

        void loggingTimer_Tick(GT.Timer timer)
        {
            temperatureHumidity_.RequestMeasurement();
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
            Debug.Print("Starting logging timer...");
            loggingTimer_.Start();
            gpsFixTimeUTC = gps_.LastPosition.FixTimeUtc;
        }

        private void StartCheckingLightLevels()
        {
            GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
            timer.Tick += new GT.Timer.TickEventHandler(TimerCheckLightLevelsTick);
            timer.Start();
        }

        void TimerCheckLightLevelsTick(GT.Timer timer)
        {
            double lightLevel = lightSensor_.ReadLightSensorPercentage();
            // TODO: Replace this with config.
            if (lightLevel > 60)
            {
                if (AlarmReady != null)
                {
                    AlarmReady(
                        new HiveSenseTeam1.Model.Alert
                        {
                            Key = "LightSense",
                            Message = "Too much light in the hive!",
                            RecordedValue = lightLevel,
                            Threshold = 60.0,
                            TimeStamp = gpsFixTimeUTC
                        });
                }
            }
        }

    }
}
