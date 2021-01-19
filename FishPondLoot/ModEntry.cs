using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Buildings;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace FishPondLoot
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            //foreach (FishPond fishPond in Game1.farm)
            {

            }
        }

        private List<FishPond> AllFishPonds
        {
            get
            {
                List<FishPond> allFishPonds = new List<FishPond>();
                return allFishPonds;
            }
        }
    }
}
