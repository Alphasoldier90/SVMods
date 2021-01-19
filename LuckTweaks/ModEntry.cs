using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace LuckTweaks
{
    public class ModEntry : Mod
    {
        private ModConfig config;

        private bool buffActive = false;

        private Buff luckyBootsBuff;
        private string luckyBootsType;

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();

            luckyBootsType = config.LuckyBootsType;

            luckyBootsBuff = new Buff(0, 0, 0, 0, config.LuckyBootsLuck, 0, 0, 0, 0, 0, 0, 0, 10, "Lucky Boots!", "You're wearing your lucky boots!");
            luckyBootsBuff.which = 181;

            Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            StopLuckyBuff();
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            CheckBoots();
        }

        private void CheckBoots()
        {
            if (Game1.player.boots.Value != null)
            {
                if (!buffActive && Game1.player.boots.Value.Name == luckyBootsType)
                {
                    this.Monitor.Log($"{Game1.player.Name} is wearing their lucky {Game1.player.boots.Value.Name}", LogLevel.Debug);
                    StartLuckyBuff();
                }
                else if (buffActive && Game1.player.boots.Value.Name != luckyBootsType)
                {
                    this.Monitor.Log($"{Game1.player.Name} is wearing {Game1.player.boots.Value.Name}, not their lucky {luckyBootsType}", LogLevel.Debug);
                    StopLuckyBuff();
                }
            }
            else
            {
                if (buffActive)
                {
                    this.Monitor.Log($"{Game1.player.Name} is not wearing boots.", LogLevel.Debug);
                    StopLuckyBuff();
                }
            }

            #region Old Code
            //if (!buffActive)
            //{
            //    if (Game1.player.boots.Value != null && Game1.player.boots.Value.Name == luckyBootsType)
            //    {
            //        this.Monitor.Log($"{Game1.player.Name} is wearing (Name: {Game1.player.boots.Value.Name})", LogLevel.Debug);
            //        StartLuckyBuff();
            //    }
            //}
            //else if (buffActive)
            //{
            //    if (Game1.player.boots.Value == null)
            //    {
            //        this.Monitor.Log($"{Game1.player.Name} is not wearing boots.", LogLevel.Debug);
            //        StopLuckyBuff();
            //    }
            //    else if (Game1.player.boots.Value != null && Game1.player.boots.Value.Name != luckyBootsType)
            //    {
            //        this.Monitor.Log($"{Game1.player.Name} is not wearing lucky boots.", LogLevel.Debug);
            //        StopLuckyBuff();
            //    }
            //}
            #endregion
        }

        private void StartLuckyBuff()
        {
            luckyBootsBuff.millisecondsDuration = int.MaxValue;
            Game1.buffsDisplay.addOtherBuff(luckyBootsBuff);
            buffActive = true;
        }

        private void StopLuckyBuff()
        {
            luckyBootsBuff.millisecondsDuration = 0;
            buffActive = false;
        }
    }
}
