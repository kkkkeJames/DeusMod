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
    public class CultistArcherBlack : ModNPC
    {
        public override string Texture => "DeusMod/Assets/NPCs/PHM/Cultist/CultistArcherBlack";
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.CultistArcherBlue);
            NPC.lifeMax = 56;
            NPC.damage = 12;
            NPC.defense = 8;
            AIType = NPCID.CultistArcherBlue;
            AnimationType = NPCID.CultistArcherBlue;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                new FlavorTextBestiaryInfoElement("Those archers in black robes are not as devout as their superiors, but their accuracy are still good."),

                new FlavorTextBestiaryInfoElement("这些身着黑色长袍的弓箭手并不如他们的上司虔诚，但是他们的准头依旧不错。")
            });
        }
    }
}
