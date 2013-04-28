using System;
using Microsoft.SPOT;
using HiveSenseTeam1.Model;
using System.Collections;

namespace MFConsoleApplication1
{
    public class Program
    {
        public static void Main()
        {
            InitialiseConfigWithSingleEntryExpectArrayListWithSingleKVP();
            InitialiseConfigWith2EntriesExpectArrayListWith2KVPs();
            GetValueWithMissingKeyValuesExpectDefaultValue();
            Debug.Print("");
            Debug.Print("******************************************");
            Debug.Print("All done, no errors");
            Debug.Print("******************************************");
            Debug.Print("");
        }

        private static void GetValueWithExistingKeyExpectValues()
        {
            var config = BuildStandardConfig();
            ThrowIfNot(config["MotionSensorAlerts"] == 1, "GetValueWithExistingKeyExpectValues");
            ThrowIfNot(config["TemperatureThresholdAlerts"] == 1, "GetValueWithExistingKeyExpectValues");
        }

        private static Configuration BuildStandardConfig()
        {
            var config = new Configuration("MotionSensorAlerts=1" + '\r' + '\n' + "TemperatureThresholdExceededAlerts=0");
            return config;
        }

        private static void GetValueWithMissingKeyValuesExpectDefaultValue()
        {
            var config = BuildStandardConfig();
            ThrowIfNot(config["MissingSetting"] == 1, "GetValueWithMissingKeyValuesExpectDefaultValue");
        }

        private static void InitialiseConfigWith2EntriesExpectArrayListWith2KVPs()
        {
            var config = BuildStandardConfig();
            ThrowIfNot(config.Values.Count == 2,"InitialiseConfigWith2EntriesExpectArrayListWith2KVPs");
        }

        private static void InitialiseConfigWithSingleEntryExpectArrayListWithSingleKVP()
        {
            var config = new Configuration("MotionSensorAlerts=1");
            ThrowIfNot(config.Values.Count == 1, "InitialiseConfigWithSingleEntryExpectArrayListWithSingleKVP");
        }

        private static void ThrowIfNot(bool test, string message)
        {
            if (!test)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
