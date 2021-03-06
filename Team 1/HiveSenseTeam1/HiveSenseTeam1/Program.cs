﻿using System;
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
using HiveSenseTeam1.Loggers;
using GHIElectronics.Gadgeteer;
using System.Text;
using Gadgeteer.Modules.IngenuityMicro;

namespace HiveSenseTeam1
{
    public partial class Program
    {
        const string CONFIG_FILE_NAME = "HiveSense.ini";

        private HiveMonitor monitor_;
        private SdLogger sdLogger_;
        private DisplayLogger dispLogger_;
        private RfLogger rfLogger_;
        private LightAlarm lightAlarm_;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            Configuration config = InitialiseConfiguration();

            monitor_ = new HiveMonitor(config, temperatureHumidity, gps, lightSensor, accelerometer);

            sdLogger_ = new SdLogger(sdCard);
            dispLogger_ = new DisplayLogger(char_Display);
            rfLogger_ = new RfLogger(rfPipe);
            lightAlarm_ = new LightAlarm(multicolorLed);

            monitor_.MeasurementReady += new HiveMonitor.MeasurementReadyHandler(sdLogger_.OnLogItem);
            monitor_.MeasurementReady += new HiveMonitor.MeasurementReadyHandler(dispLogger_.OnLogItem);
            monitor_.MeasurementReady += new HiveMonitor.MeasurementReadyHandler(rfLogger_.OnLogItem);
            
            monitor_.AlarmReady += new HiveMonitor.AlarmReadyHandler(sdLogger_.OnLogItem);
            monitor_.AlarmReady += new HiveMonitor.AlarmReadyHandler(lightAlarm_.OnLogItem);
            monitor_.AlarmReady += new HiveMonitor.AlarmReadyHandler(rfLogger_.OnLogItem);
        }

        private Configuration InitialiseConfiguration()
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
                    return new Configuration(new string(fileChars));
                }
            }

            return new Configuration();
        }
    }
}
