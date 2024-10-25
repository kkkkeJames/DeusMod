using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using DeusMod.Dusts;
using DeusMod.NPCs.Vanilla.VanillaBoss.KingSlime;

namespace DeusMod.NPCs.Vanilla.VanillaBoss
{
    public class DeusGlobalVanillaBoss : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            if (npc.boss)
            {
                if (npc.type == NPCID.KingSlime || npc.type == NPCID.EyeofCthulhu)
                {
                    npc.aiStyle = -1;
                    return false;
                }
                else return true;
            }
            else return true;
        }
    }
}
