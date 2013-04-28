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
            ConstructConfigurationExpectNotNull();
            InitialiseConfigWithSingleEntryExpectArrayListWithSingleKVP();
            InitialiseConfigWith2EntriesExpectArrayListWith2KVPs();
            GetValue();
        }

        private static void GetValue()
        {
            var config = new Configuration("MotionSensorAlerts=1" + '\r' + '\n' + "TemperatureThresholdExceededAlerts=0");
            ThrowIfNot(config["MotionSensorAlerts"] == 1);
            ThrowIfNot(config["TemperatureThresholdAlerts"] == 1);
        }



        private static void InitialiseConfigWith2EntriesExpectArrayListWith2KVPs()
        {
            var config = new Configuration("MotionSensorAlerts=1" + '\r' + '\n' + "TemperatureThresholdExceededAlerts=0");
            ThrowIfNot(config.Values.Count == 2);
        }

        private static void InitialiseConfigWithSingleEntryExpectArrayListWithSingleKVP()
        {
            var config = new Configuration("MotionSensorAlerts=1");
            ThrowIfNot(config.Values.Count == 1);
        }

        private static void ConstructConfigurationExpectNotNull()
        {
            ThrowIfNot(new Configuration() != null);
        }

        private static void ThrowIfNot(bool test)
        {
            if (!test)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
