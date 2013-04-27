using System;
using Microsoft.SPOT;

namespace HiveSenseTeam1.Model
{
    public class Measurement
    {
        public DateTime TimeStamp { get; set; }
        public string Key { get; set; }
        public double Value { get; set; }
        public string ToJSon()
        {
            return "{"
                + "\"TimeStamp\": " + TimeStamp.ToLocalTime().ToString("yyyy-MM-dd hh:mm:ss") + ", "
                + "\"Key\": \"" + Key + "\", "
                + "\"Value\": " + Value.ToString()
                + "}";
        }
    }
}
