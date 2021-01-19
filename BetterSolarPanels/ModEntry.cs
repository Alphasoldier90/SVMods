using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterSolarPanels
{
    class ModEntry : Mod
    {
        private ModConfig config;

        private readonly int minimumTime = 1440; //24 hours
        private readonly decimal minimumRate = 0.01M;
        private readonly decimal maximumRate = 10.0M;

        private int defaultTime;
        private decimal desertRate;
        private decimal sunnyRate;
        private decimal debrisRate;
        private decimal snowRate;
        private decimal rainRate;
        private decimal lightningRate;
        private Dictionary<string, List<StardewValley.Object>> solarPanelDictionary;

        public override void Entry(IModHelper helper)
        {
            solarPanelDictionary = new Dictionary<string, List<StardewValley.Object>>();
            config = helper.ReadConfig<ModConfig>();
            defaultTime = Math.Max(minimumTime, config.DefaultTimeInMinutes);
            desertRate = Math.Max(minimumRate, Math.Min(maximumRate, config.DesertRate));
            sunnyRate = Math.Max(minimumRate, Math.Min(maximumRate, config.SunnyRate)); //0
            rainRate = Math.Max(minimumRate, Math.Min(maximumRate, config.RainRate)); //1
            debrisRate = Math.Max(minimumRate, Math.Min(maximumRate, config.DebrisRate)); //2
            lightningRate = Math.Max(minimumRate, Math.Min(maximumRate, config.LightningRate)); //3
            snowRate = Math.Max(minimumRate, Math.Min(maximumRate, config.SnowRate)); //5


            //Check for newly added solar panels
            Helper.Events.World.ObjectListChanged += World_ObjectListChanged;
            //Adjust remaining time upon knowing weather of new day
            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            //Normalize adjusted rate times
            Helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
        }


        #region Events
        private void World_ObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            foreach (var pair in e.Added)
            {
                //New Solar Panel added?
                if (pair.Value.Name == "Solar Panel")
                {
                    AddToSolarPanelDictionary(e.Location,pair.Value);
                    //Apply default time
                    pair.Value.MinutesUntilReady = defaultTime;
                    //Apply rate to just placed panel
                    ApplyRate(pair.Value, RateFromLocationName(e.Location.Name));
                }
            }
            foreach (var pair in e.Removed)
            {
                if (pair.Value.Name == "Solar Panel")
                {
                    RemoveFromSolarPanelDictionary(e.Location, pair.Value);
                }
            }
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (solarPanelDictionary.Count == 0)
            {
                CreateSolarPanelDictionary();
            }
            SetSolarPanelTimers();
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            NormalizeSolarPanelTimers();
        }
        #endregion

        #region Solar Panel Dictionary
        private void CreateSolarPanelDictionary()
        {
            solarPanelDictionary.Clear();

            //Loop through all outdoor locations
            foreach (var location in Game1.locations.Where(l => l.IsOutdoors).ToList())
            {
                var objects = new List<StardewValley.Object>();
                //Select all "Solar Panels"
                objects = location.Objects.Values.Where(o => o.Name.Equals("Solar Panel")).ToList();
                solarPanelDictionary.Add(location.Name, objects);
            }
            this.Monitor.Log($"Initialized SPD with {solarPanelDictionary.Count} items.", LogLevel.Debug);
        }

        private void AddToSolarPanelDictionary(GameLocation location, StardewValley.Object obj)
        {
            solarPanelDictionary[location.Name].Add(obj);
            this.Monitor.Log($"Added SP({location.Name}[{obj.TileLocation}]) to the list({solarPanelDictionary[location.Name].Count}).", LogLevel.Debug);
        }

        private void RemoveFromSolarPanelDictionary(GameLocation location, StardewValley.Object obj)
        {
            this.Monitor.Log($"Removing SP({location.Name}[{obj.TileLocation}]) from the list({solarPanelDictionary[location.Name].Count-1}).", LogLevel.Debug);
            solarPanelDictionary[location.Name].Remove(obj);
        }
        #endregion

        #region Solar Panel Timer Adjustments
        private void SetSolarPanelTimers()
        {
            //Iterate through dictionary
            foreach(var pair in solarPanelDictionary)
            {
                //Get rate for location
                decimal rate = RateFromLocationName(pair.Key);
                //Iterate through all solar panels
                foreach(var obj in pair.Value)
                {
                    this.Monitor.Log($"SP({pair.Key}[{obj.TileLocation}]) rate {rate} applying to {obj.MinutesUntilReady} time left.", LogLevel.Debug);

                    ApplyRate(obj, rate);
                    
                    this.Monitor.Log($"SP({pair.Key}[{obj.TileLocation}]) applied {rate} with {obj.MinutesUntilReady} left.", LogLevel.Debug);
                }
            }
        }

        private void ApplyRate(StardewValley.Object obj, decimal rate)
        {
            if (!obj.readyForHarvest)
            {
                int timeLeft = Math.Min(obj.MinutesUntilReady, defaultTime);
                int timeAdjusted = Convert.ToInt32(timeLeft / rate);
                obj.MinutesUntilReady = timeAdjusted;
            }
        }

        private void NormalizeSolarPanelTimers()
        {
            //Iterate through dictionary
            foreach (var pair in solarPanelDictionary)
            {
                //Get rate for location
                decimal rate = RateFromLocationName(pair.Key);
                //Iterate through all solar panels
                foreach (var obj in pair.Value)
                {
                    this.Monitor.Log($"SP({pair.Key}[{obj.TileLocation}]) normalizing from {rate} with {obj.MinutesUntilReady} time left.", LogLevel.Debug);

                    NormalizeRate(obj, rate);

                    this.Monitor.Log($"SP({pair.Key}[{obj.TileLocation}]) normalized with {rate} to {obj.MinutesUntilReady} left.", LogLevel.Debug);
                }
            }
        }

        private void NormalizeRate(StardewValley.Object obj, decimal rate)
        {
            if (!obj.readyForHarvest)
            {
                int timeLeft = obj.MinutesUntilReady;
                int timeNormalized = Convert.ToInt32(timeLeft * rate);
                obj.MinutesUntilReady = timeNormalized;
            }
        }
        #endregion

        #region Solar Panel Rates
        private decimal RateFromLocationName(string locationName)
        {
            if (locationName == "Desert")
            {
                return desertRate;
            }

            int weather = GetWeather(locationName);
            switch (weather)
            {
                case Game1.weather_rain:
                    return rainRate;
                case Game1.weather_debris:
                    return debrisRate;
                case Game1.weather_lightning:
                    return lightningRate;
                case Game1.weather_snow:
                    return snowRate;
                default:
                    return sunnyRate;
            }
        }

        private int GetWeather(string locationName)
        {
            GameLocation location = Game1.getLocationFromName(locationName);
            int weather = Game1.weather_sunny; //0
            
            if(Game1.IsRainingHere(location))
            weather = Game1.weather_rain; //1
            else if(Game1.IsDebrisWeatherHere(location))
            weather = Game1.weather_debris; //2
            else if(Game1.IsLightningHere(location))
            weather = Game1.weather_lightning; //3
            //weather = Game1.weather_festival; //4
            else if(Game1.IsSnowingHere(location))
            weather = Game1.weather_snow; //5
            //weather = Game1.weather_wedding; //6
            return weather;
        }
        #endregion

    }
}
