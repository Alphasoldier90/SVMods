using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;

namespace FishingTweaks
{
    public class ModEntry : Mod
    {
        private ModConfig config;

        private bool buffActive = false;

        private Buff fishingTier0Buff;
        private Buff fishingTier1Buff;
        private Buff fishingTier2Buff;

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();

            fishingTier0Buff = new Buff(0, config.BambooRodSkill, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, "Bad rod...", "Bad rod...");
            fishingTier1Buff = new Buff(0, config.FiberRodSKill, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, "Good rod.", "Good rod.");
            fishingTier2Buff = new Buff(0, config.IridiumRodSkill, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, "Expensive rod!", "Expensive rod!");
            fishingTier0Buff.which = 170;
            fishingTier1Buff.which = 171;
            fishingTier2Buff.which = 172;

            Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.player.CurrentTool is FishingRod rod && !Context.CanPlayerMove)
                RodBoostStart(rod);
            else if(buffActive)
            {
                fishingTier0Buff.millisecondsDuration = 0;
                fishingTier1Buff.millisecondsDuration = 0;
                fishingTier2Buff.millisecondsDuration = 0;
                buffActive = false;
            }
        }

        private void RodBoostStart(FishingRod rod)
        {
            if (rod.attachmentSlots() == 2 && !Game1.buffsDisplay.hasBuff(fishingTier2Buff.which))
            {
                fishingTier2Buff.millisecondsDuration = 600000;
                Game1.buffsDisplay.addOtherBuff(fishingTier2Buff);
                buffActive = true;
            }
            else if (rod.attachmentSlots() == 1 && !Game1.buffsDisplay.hasBuff(fishingTier1Buff.which))
            {
                fishingTier1Buff.millisecondsDuration = 600000;
                Game1.buffsDisplay.addOtherBuff(fishingTier1Buff);
                buffActive = true;
            }
            else if (rod.attachmentSlots() == 0 && !Game1.buffsDisplay.hasBuff(fishingTier0Buff.which))
            {
                fishingTier0Buff.millisecondsDuration = 600000;
                Game1.buffsDisplay.addOtherBuff(fishingTier0Buff);
                buffActive = true;
            }
        }
    }
}
