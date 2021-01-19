using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterSolarPanels
{
    class ModConfig
    {
        public int DefaultTimeInMinutes { get; set; } = 2880; //16800 default time (11.6days) 1440 for 1 day
        public decimal DesertRate { get; set; } = 2.0M;
        public decimal SunnyRate { get; set; } = 1.0M; //0
        public decimal DebrisRate { get; set; } = 0.7M; //2
        public decimal SnowRate { get; set; } = 0.5M; //5
        public decimal RainRate { get; set; } = 0.3M; //1
        public decimal LightningRate { get; set; } = 0.2M; //3
    }
}
