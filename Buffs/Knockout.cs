﻿using Terraria;
using Terraria.ModLoader;
using DeusMod.NPCs;

namespace DeusMod.Buffs
{
    public class KnockoutDebuff : ModBuff
    {
        public override string Texture => "DeusMod/Assets/Buffs/SlimedTier2";
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.wingTimeMax = 0;
            player.wingTime = 0;
            player.wings = 0;
            player.wingsLogic = 0;
            player.noFallDmg = true;
            player.noBuilding = true;
            player.RemoveAllGrapplingHooks();
            player.controlJump = false;
            player.controlDown = false;
            player.controlLeft = false;
            player.controlRight = false;
            player.controlUp = false;
            player.controlUseItem = false;
            player.controlUseTile = false;
            player.controlThrow = false;
            player.gravDir = 1f;
            for (int i = 0; i < 4; i++)
                player.doubleTapCardinalTimer[i] = 0;

            player.velocity.Y += player.gravity;
            player.sandStorm = false;
            player.blockExtraJumps = true;
            if (player.mount.Active)
                player.mount.Dismount(player);
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<DeusGlobalBuffNPC>().knockoutTime++;
            npc.GetGlobalNPC<DeusGlobalBuffNPC>().knockoutState = true;
        }
    }
}