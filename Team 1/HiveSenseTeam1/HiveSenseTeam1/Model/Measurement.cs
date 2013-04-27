using System;
using Microsoft.SPOT;

namespace HiveSenseTeam1.Model
{
    public class Measurement
    {
        public long Id { get; set; }
        public long TimeOffset { get; set; }
        public string Key { get; set; }
        public double Value { get; set; }
        public string ToJSon()
        {
            return "{"
                + "\"Id\": " + Id.ToString() + ", "
                + "\"TimeOffset\": " + TimeOffset.ToString() + ", "
                + "\"Key\": \"" + Key + "\", "
                + "\"Value\": " + Value.ToString()
                + "}";
        }
    }
}
