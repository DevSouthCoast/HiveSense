using System;
using Microsoft.SPOT;
using System.Net;

namespace HiveSenseTeam1.Loggers
{
    class HTTPLogger : ILogger
    {
        public void OnLogItem(Model.Measurement measurement)
        {

            WebRequest wreq = WebRequest.Create("http://aurora:5984/hive-sense/");
            wreq.ContentType = "application/json";
//            wreq.ContentLength
        }

        public void OnLogItem(Model.Alert alert)
        {
        }
    }
}
