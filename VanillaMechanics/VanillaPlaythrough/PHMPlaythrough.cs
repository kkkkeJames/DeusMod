using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using DeusMod.Projs;
using DeusMod.Projs.Melee.SkillUI;
using System.Collections.Generic;
using System;
using DeusMod.Helpers;
using DeusMod.Core;
using DeusMod.Projs.NPCs;
using DeusMod.Dusts;

namespace DeusMod.VanillaMechanics.VanillaPlaythrough
{
    public class PHMPlaythrough : GlobalTile
    {
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (type == TileID.Demonite || type == TileID.Crimtane)
                return NPC.downedBoss1;
            else return base.CanKillTile(i, j, type, ref blockDamaged);
        }
    }
    public class PHMBossLoot : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.EyeofCthulhu:
                    if (!NPC.downedBoss1)
                    {
                        if (WorldGen.crimson)
                            Main.NewText("The ore of evil is revealing crimson light!", new Color(255, 25, 25));
                        else Main.NewText("The ore of evil is unleashing dark energy!", new Color(175, 75, 255));
                    }
                    break;
            }
            base.OnKill(npc);
        }
    }
}
