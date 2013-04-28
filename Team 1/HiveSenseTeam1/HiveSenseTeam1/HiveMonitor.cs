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
        private Accelerometer accelerometer_;
        private TimeSpan alarmRaisedSpan_;
        private bool alarmRaised_ = false;

        protected double CurrentHiveTemperature { get; set; }

        public HiveMonitor(
            Configuration config,
            TemperatureHumidity temperatureHumidity,
            GPS gps,
            LightSensor lightSensor,
            Accelerometer accelerometer)
        {
            config_ = config;
            CurrentHiveTemperature = 0;
            LightThreshold = (double)config_[Configuration.LightLevelThresholdSetting];
            TemperatureThreshold = (double)config_[Configuration.TemperatureThresholdSetting];
            
            temperatureHumidity_ = temperatureHumidity;
            gps_ = gps;
            lightSensor_ = lightSensor;
            accelerometer_ = accelerometer;

            alarmRaisedSpan_ = GT.Timer.GetMachineTime();

            loggingTimer_ = new GT.Timer(10000);
            loggingTimer_.Stop();
            loggingTimer_.Tick += new GT.Timer.TickEventHandler(loggingTimer_Tick);

            if (config_[Configuration.LogIfNoGPSFixSetting] == 1)
            {
                Debug.Print("Starting logging timer...");
                loggingTimer_.Start();
            }

            temperatureHumidity.MeasurementComplete += new TemperatureHumidity.MeasurementCompleteEventHandler(temperatureHumidity_MeasurementComplete);

            // TODO: Replace this with config.
            accelerometer_.EnableThresholdDetection(4, true, true, true, true, false, true);
            accelerometer_.ThresholdExceeded += new Accelerometer.ThresholdExceededEventHandler(accelerometer_ThresholdExceeded);

            gps.PositionReceived += new GPS.PositionReceivedHandler(gps_PositionReceived);

            StartCheckingHiveForAlerts();
        }

        void loggingTimer_Tick(GT.Timer timer)
        {
            TimeSpan diff = GT.Timer.GetMachineTime() - alarmRaisedSpan_;
            if (diff.Ticks > 100000000 && alarmRaised_)
            {
                alarmRaised_ = false;
                if (AlarmReady != null)
                {
                    AlarmReady(new Alert { Key = MeasureType.Cleared, Message = "", RecordedValue = 0.0, Threshold = 0.0, TimeStamp = gpsFixTimeUTC });
                }
            }

            temperatureHumidity_.RequestMeasurement();
        }

        void temperatureHumidity_MeasurementComplete(TemperatureHumidity sender, double temperature, double relativeHumidity)
        {
            var measurementTemp = new Measurement { TimeStamp = gpsFixTimeUTC, Key = MeasureType.Tempdegc, Value = temperature };
            var measurementHumidity = new Measurement { TimeStamp = gpsFixTimeUTC, Key = MeasureType.Humidity, Value = relativeHumidity };

            CurrentHiveTemperature = temperature;

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

        private void StartCheckingHiveForAlerts()
        {
            GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
            timer.Tick += new GT.Timer.TickEventHandler(CheckStatusOfHive);
            timer.Start();
        }

        protected double TemperatureThreshold { get; set; }
        protected double LightThreshold { get; set; }

        void CheckStatusOfHive(GT.Timer timer)
        {
            double lightLevel = lightSensor_.ReadLightSensorPercentage();
            
            if (lightLevel > LightThreshold)
            {
                alarmRaised_ = true;
                alarmRaisedSpan_ = GT.Timer.GetMachineTime();

                if (AlarmReady != null)
                {
                    AlarmReady(
                        new Alert
                        {
                            Key = MeasureType.Light,
                            Message = "Too much light in the hive!",
                            RecordedValue = lightLevel,
                            Threshold = LightThreshold,
                            TimeStamp = gpsFixTimeUTC
                        });
                }
            }

            if (CurrentHiveTemperature > TemperatureThreshold)
            {
                if (AlarmReady != null)
                {
                    AlarmReady(
                        new Alert
                        {
                            Key = MeasureType.Tempdegc,
                            Message = "The hive's at " + CurrentHiveTemperature + " degrees C!",
                            RecordedValue = CurrentHiveTemperature,
                            Threshold = TemperatureThreshold,
                            TimeStamp = gpsFixTimeUTC
                        });
                }
            }
        }

        void accelerometer_ThresholdExceeded(Accelerometer sender)
        {
            alarmRaised_ = true;
            alarmRaisedSpan_ = GT.Timer.GetMachineTime();
            if (AlarmReady != null)
            {
                AlarmReady(
                    new Alert
                    {
                        Key = "Accelerometer",
                        Message = "She fell over!",
                        RecordedValue = 0.0,
                        Threshold = 4.0,
                        TimeStamp = gpsFixTimeUTC
                    });
            }
        }
    }
}
