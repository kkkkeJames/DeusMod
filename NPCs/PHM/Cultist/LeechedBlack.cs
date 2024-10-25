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
    public class LeechedBlack : ModNPC
    {
        public override string Texture => "DeusMod/Assets/NPCs/PHM/Cultist/CultistVindicatorBlack";
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Zombie);
            NPC.lifeMax = 80;
            NPC.damage = 17;
            NPC.defense = 12;
            NPC.knockBackResist = 0;
            AIType = NPCID.Zombie;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                new FlavorTextBestiaryInfoElement("Disgusting and twisted creature of spells. Such an abberation is beyond the imagination of creatures."),

                new FlavorTextBestiaryInfoElement("恶心而扭曲的法术造物。这样的存在已然超出人们对于生物的想象。")
            });
        }
        /*
        移动极慢，但是血高防高，免疫击退，除此之外就是常规战士
        */
    }
}
