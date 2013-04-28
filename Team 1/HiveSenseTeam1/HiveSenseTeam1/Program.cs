using System;
using System.Collections;
using System.IO;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.Seeed;
using Gadgeteer.Modules.GHIElectronics;
using HiveSenseTeam1.Model;
using GHIElectronics.Gadgeteer;
using System.Text;

namespace HiveSenseTeam1
{
    public partial class Program
    {
        const string MEASUREMENT_FILE_NAME = "measurements.json";
        const string ALERTS_FILE_NAME = "alerts.json";
        const string CONFIG_FILE_NAME = "HiveSense.ini";

        private DateTime gpsFixTimeUTC;
        private GT.Timer loggingTimer_;

        private Configuration config;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            Debug.Print("Program Started");

            InitialiseConfiguration();

            gps.PositionReceived += new GPS.PositionReceivedHandler(gps_PositionReceived);

            temperatureHumidity.MeasurementComplete += new TemperatureHumidity.MeasurementCompleteEventHandler(temperatureHumidity_MeasurementComplete);

            accelerometer.EnableThresholdDetection(4, true, true, true, true, false, true);
            accelerometer.ThresholdExceeded += new Accelerometer.ThresholdExceededEventHandler(accelerometer_ThresholdExceeded);

            StartCheckingLightLevels();

            loggingTimer_ = new GT.Timer(60000);
            loggingTimer_.Stop();
            loggingTimer_.Tick += new GT.Timer.TickEventHandler(loggingTimer_Tick);
        }

        private void InitialiseConfiguration()
        {
            if (sdCard.IsCardInserted)
            {
                var storageDevice = sdCard.GetStorageDevice();
                using (FileStream fs =
                    storageDevice.Open(
                        CONFIG_FILE_NAME,
                        FileMode.OpenOrCreate,
                        FileAccess.ReadWrite))
                {
                    var fileBytes = new byte[fs.Length];
                    fs.Read(fileBytes, 0, (int)fs.Length);
                    var fileChars = Encoding.UTF8.GetChars(fileBytes);
                    var config = new Configuration(new string(fileChars));
                }
            }
            else
            {
                config = new Configuration();
            }
        }

        void loggingTimer_Tick(GT.Timer timer)
        {
            temperatureHumidity.RequestMeasurement();
        }

        private void StartCheckingLightLevels()
        {
            GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
            timer.Tick += new GT.Timer.TickEventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(GT.Timer timer)
        {
            if (lightSensor.ReadLightSensorPercentage() > 60)
            {
                multicolorLed.BlinkOnce(GT.Color.Green);
            }
        }

        void accelerometer_ThresholdExceeded(Accelerometer sender)
        {
            multicolorLed.BlinkOnce(GT.Color.Red);
        }

        void gps_PositionReceived(GPS sender, GPS.Position position)
        {
            loggingTimer_.Start();
            gpsFixTimeUTC = gps.LastPosition.FixTimeUtc;
            temperatureHumidity.RequestMeasurement();
        }

        void temperatureHumidity_MeasurementComplete(TemperatureHumidity sender, double temperature, double relativeHumidity)
        {
            var measurementTemp = new Measurement { TimeStamp = gpsFixTimeUTC, Key = "TempDegC", Value = temperature };
            var measurementHumidity = new Measurement { TimeStamp = gpsFixTimeUTC, Key = "HumidityPc", Value = relativeHumidity };

            if (sdCard.IsCardInserted)
            {
                var storageDevice = sdCard.GetStorageDevice();
                using (FileStream fs =
                    storageDevice.Open(
                        MEASUREMENT_FILE_NAME,
                        FileMode.OpenOrCreate,
                        FileAccess.ReadWrite))
                {
                    WriteString(measurementTemp, fs);
                    WriteString(measurementHumidity, fs);
                }
            }

        }

        private static void WriteString(Measurement measurementTemp, FileStream fs)
        {
            fs.Seek(0, SeekOrigin.End);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(measurementTemp.ToJSon());
            fs.Write(bytes, 0, bytes.Length);
        }
    }
}
