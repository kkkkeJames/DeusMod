using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using DeusMod.Dusts;

namespace DeusMod.Buffs
{
    public class SlimedTier2 : ModBuff
    {
        public override string Texture => "DeusMod/Assets/Buffs/SlimedTier2";
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Slimed Tier 2");
            //DisplayName.AddTranslation(7, "史莱姆 2级");
            //Description.SetDefault("You are slimy and sticked to the ground");
            //Description.AddTranslation(7, "你黏糊糊的，似乎被钉在了地上");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.jumpSpeedBoost -= 2.5f;
            player.moveSpeed -= 0.25f;
            //player.controlJump = false;
            //player.controlDown = false;
            //player.stairFall = false;
            player.buffImmune[BuffID.Slimed] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.boss)
                npc.velocity *= 0.98f;
            else npc.velocity *= 0.9f;
        }
    }
}
