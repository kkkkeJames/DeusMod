using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Terraria.ID;
using DeusMod.Projs;
using Terraria.Localization;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using DeusMod.Projs.NPCs;
using System;

namespace DeusMod.NPCs.PHM.Cultist
{
    public class BloodyLament : ModNPC
    {
        public override string Texture => "DeusMod/Assets/NPCs/PHM/Cultist/CultistVindicatorBlack";
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Zombie);
            NPC.lifeMax = 360;
            NPC.damage = 40;
            NPC.defense = 16;
            NPC.knockBackResist = 0.2f;
            AIType = NPCID.Zombie;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                new FlavorTextBestiaryInfoElement("\"\""),

                new FlavorTextBestiaryInfoElement("\"圣神之泪垂自天际时，吾等奴仆当奉上血肉......\"")
            });
        }
        /*
        只在克眼后血月出现的精英怪，血高防高，大幅免疫击退，平常是常规战士，一旦靠近玩家则会连扑，靠近玩家则秒杀之而后消失
        */
    }
}
