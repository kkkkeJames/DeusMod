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
    public class CultistSorcererBlack : ModNPC
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
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                new FlavorTextBestiaryInfoElement("One of the most mysterious secrets about the Lunatic Cult is their secret spells. It is said that the sorceres who master those spells all become inhuman existence."),

                new FlavorTextBestiaryInfoElement("拜月教最为神秘的秘密是其秘密的法术。据说掌握这些法术的术士已成为非人的存在。")
            });
        }
        /*
        不断地随机移动，不会主动追击玩家，每隔8s左右释放一个法术：
        1.如果场地上肉前教徒过少（少于2）则召唤2-3个镰刀或弓手
        2.将随机1-2个镰刀或弓手传送到更靠近玩家的位置，如果自己里玩家过近则优先释放此法术，且效果变为瞬移离开玩家并拉开Y轴距离，如果离玩家过远，则瞬移到相对靠近位置
        3.将最靠近玩家的1-2个镰刀或弓手转化为蛭子人
        */
    }
}
