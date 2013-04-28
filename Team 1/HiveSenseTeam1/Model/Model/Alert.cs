using System;
using Microsoft.SPOT;

namespace HiveSenseTeam1.Model
{
    public class Alert
    {
        public DateTime TimeStamp { get; set; }
        public string Key { get; set; }
        public string Message {get; set; }
        public double Threshold { get; set; }
        public double RecordedValue { get; set; }

        public string ToJSon()
        {
            return "{"
                + "\"TimeStamp\": " + TimeStamp.ToLocalTime().ToString("yyyy-MM-dd hh:mm:ss") + ", "
                + "\"Key\": \"" + Key + "\", "
                + "\"Message\": \"" + Message + "\", "
                + "\"Threshold\": " + Threshold.ToString()
                + "\"RecordedValue\": " + RecordedValue.ToString()
                + "}\n";
        }
    }
}
