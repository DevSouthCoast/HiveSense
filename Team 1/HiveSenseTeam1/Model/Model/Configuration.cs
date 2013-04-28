using System;
using Microsoft.SPOT;
using System.Collections;
using System.IO;
using System.Text;

namespace HiveSenseTeam1.Model
{
    public class Configuration
    {
        public const string MotionSensorAlertsSetting = "MotionSensorAlerts";
        public const string TemperatureThresholdExceededAlertsSetting = "TemperatureThresholdExceededAlerts";
        public const string LogIfNoGPSFix = "LogIfNoGPSFix";

        char[] newLine = new[] { '\r', '\n' };

        public Configuration(string configValues)
        {
            Values = Initialise(configValues);
        }

        public Configuration()
        {
            Values = new ArrayList();
        }

        public ArrayList Initialise(string configValues)
        {
            ArrayList values = new ArrayList();
            foreach (var configLine in configValues.Split(newLine))
            {
                if (configLine == null || configLine == string.Empty)
                {
                    continue;
                }
                values.Add(ParseLine(configLine));
            }
            return values;
        }

        private DictionaryEntry ParseLine(string configLine)
        {
            var kvp = configLine.Split('=');
            return new DictionaryEntry(kvp[0], kvp[1]);
        }

        public ArrayList Values
        {
            get;
            private set;
        }

        public int this[string key]
        {
            get
            {
                foreach (var obj in Values)
                {
                    var kvp = (DictionaryEntry)obj;
                    if ((string)kvp.Key == key)
                    {
                        return (int)kvp.Value;
                    }
                }
                //Default to "on"
                return 1;
            }
        }
    }
}
