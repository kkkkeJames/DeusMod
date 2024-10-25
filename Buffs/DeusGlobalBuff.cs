using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.DataStructures;
using System.Collections.Generic;

namespace DeusMod.Buffs
{
    public class DeusGlobalBuff : GlobalBuff
    {
        public override void Update(int type, Player player, ref int buffIndex)
        {
            switch (type)
            {
                case BuffID.Slimed:
                    Main.buffNoTimeDisplay[type] = false;
                    if(player.buffTime[buffIndex] > 3600)
                    {
                        player.AddBuff(ModContent.BuffType<SlimedTier2>(), 300);
                        player.buffTime[buffIndex] = 0;
                    }
                    break;
            }
        }
        public override void Update(int type, NPC npc, ref int buffIndex)
        {
            switch (type)
            {
                case BuffID.Slimed:
                    Main.buffNoTimeDisplay[type] = false;
                    if (npc.buffTime[buffIndex] > 36000)
                    {
                        npc.AddBuff(ModContent.BuffType<SlimedTier2>(), 300);
                        npc.buffTime[buffIndex] = 0;
                    }
                    break;
            }
        }
        public override bool ReApply(int type, Player player, int time, int buffIndex)
        {
            switch (type)
            {
                case BuffID.Slimed:
                    player.buffTime[buffIndex] += time;
                    return true;
            }
            return base.ReApply(type, player, time, buffIndex);
        }
        public override bool ReApply(int type, NPC npc, int time, int buffIndex)
        {
            switch (type)
            {
                case BuffID.Slimed:
                    npc.buffTime[buffIndex] += time;
                    return true;
            }
            return base.ReApply(type, npc, time, buffIndex);
        }
    }
}
