using System;
using Microsoft.SPOT;
using HiveSenseTeam1.Model;

namespace HiveSenseTeam1.Loggers
{
    public interface ILogger
    {
        void OnLogItem(Measurement measurement);
    }
}
