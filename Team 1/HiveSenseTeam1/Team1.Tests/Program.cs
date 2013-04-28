using System;using Microsoft.SPOT;
using HiveSenseTeam1.Model;
using System.Collections;

namespace MFConsoleApplication1
{
    public class Program
    {
        public static void Main()
        {
            Test_InitialiseConfigWithSingleEntryExpectArrayListWithSingleKVP();
            Test_InitialiseConfigWith3EntriesExpectArrayListWith3KVPs();
            Test_GetValueWithMissingKeyValuesExpectDefaultValue();
            Debug.Print("");
            Debug.Print("******************************************");
            Debug.Print("All done, no errors");
            Debug.Print("******************************************");
            Debug.Print("");
        }

        private static void Test_GetValueWithExistingKeyExpectValues()
        {
            var config = BuildStandardConfig();
            ThrowIfNot(config[Configuration.MotionSensorAlertsSetting] == 1, "Test_GetValueWithExistingKeyExpectValues");
            ThrowIfNot(config[Configuration.TemperatureThresholdExceededAlertsSetting] == 1, "Test_GetValueWithExistingKeyExpectValues");
        }

        private static void Test_GetValueWithMissingKeyValuesExpectDefaultValue()
        {
            var config = BuildStandardConfig();
            ThrowIfNot(config["MissingSetting"] == 1, "Test_GetValueWithMissingKeyValuesExpectDefaultValue");
        }

        private static void Test_InitialiseConfigWith3EntriesExpectArrayListWith3KVPs()
        {
            var config = BuildStandardConfig();
            ThrowIfNot(config.Values.Count == 3, "Test_InitialiseConfigWith3EntriesExpectArrayListWith3KVPs");
        }

        private static void Test_InitialiseConfigWithSingleEntryExpectArrayListWithSingleKVP()
        {
            var config = new Configuration("MotionSensorAlerts=1");
            ThrowIfNot(config.Values.Count == 1, "Test_InitialiseConfigWithSingleEntryExpectArrayListWithSingleKVP");
        }

        private static Configuration BuildStandardConfig()
        {
            var config = new Configuration("MotionSensorAlerts=1" + '\r' + '\n' + "TemperatureThresholdExceededAlerts=0" + '\r' + '\n' + "LogIfNoGPSFix=1");
            return config;
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
