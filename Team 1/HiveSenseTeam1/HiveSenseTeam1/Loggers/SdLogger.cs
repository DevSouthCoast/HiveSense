using System;
using Microsoft.SPOT;
using HiveSenseTeam1.Model;
using System.IO;

namespace HiveSenseTeam1.Loggers
{
    class SdLogger : ILogger
    {
        private Gadgeteer.Modules.GHIElectronics.SDCard sdCard_;
        const string MEASUREMENT_FILE_NAME = "measurements.json";

        public SdLogger(Gadgeteer.Modules.GHIElectronics.SDCard sdCard)
        {
            sdCard_ = sdCard;
        }

        public void OnLogItem(Measurement measurement)
        {
            if (sdCard_.IsCardInserted)
            {
                var storageDevice = sdCard_.GetStorageDevice();
                using (FileStream fs =
                    storageDevice.Open(
                        MEASUREMENT_FILE_NAME,
                        FileMode.OpenOrCreate,
                        FileAccess.ReadWrite))
                {
                    WriteString(measurement, fs);
                }
            }
        }

        private static void WriteString(Measurement measurement, FileStream fs)
        {
            fs.Seek(0, SeekOrigin.End);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(measurement.ToJSon());
            fs.Write(bytes, 0, bytes.Length);
        }

    }
}
