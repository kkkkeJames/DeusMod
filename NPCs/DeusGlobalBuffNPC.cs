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
using DeusMod.Buffs;

namespace DeusMod.NPCs
{
    public class DeusGlobalBuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int knockoutgauge = 0; //击晕累计值
        public int knockoutMax = 6000; //击晕上限值
        public bool knockoutImmune = false; //是否免疫击晕
        public bool knockoutState = false; //是否是击晕状态
        public float knockoutTime = 0; //击晕时长
        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            switch (npc.type)
            {
                case NPCID.GreenSlime:
                case NPCID.BlueSlime:
                case NPCID.PurpleSlime:
                case NPCID.Pinky:
                case NPCID.RedSlime:
                case NPCID.YellowSlime:
                case NPCID.BlackSlime:
                case NPCID.MotherSlime:
                case NPCID.BabySlime:
                case NPCID.SandSlime:
                case NPCID.IceSlime:
                case NPCID.SpikedIceSlime:
                case NPCID.JungleSlime:
                case NPCID.SpikedJungleSlime:
                case NPCID.DungeonSlime:
                case NPCID.LavaSlime:
                case NPCID.GoldenSlime:
                case NPCID.UmbrellaSlime:
                case NPCID.SlimeMasked:
                case NPCID.SlimeRibbonWhite:
                case NPCID.SlimeRibbonYellow:
                case NPCID.SlimeRibbonGreen:
                case NPCID.SlimeRibbonRed:
                case NPCID.BigCrimslime:
                case NPCID.LittleCrimslime:
                case NPCID.CorruptSlime:
                case NPCID.Slimeling:
                case NPCID.Slimer2:
                case NPCID.IlluminantSlime:
                case NPCID.RainbowSlime:
                case NPCID.ToxicSludge:
                case NPCID.HoppinJack:
                case NPCID.SlimeSpiked:
                case NPCID.QueenSlimeMinionBlue:
                case NPCID.QueenSlimeMinionPink:
                case NPCID.TargetDummy:
                    knockoutImmune = true;
                    break;
            }
        }
        public override void ResetEffects(NPC npc) //重置buff类
        {
            knockoutState = false;
        }
        public override bool PreAI(NPC npc) //击晕效果
        {
            if (!knockoutState && knockoutgauge < knockoutMax)
            {
                if (knockoutgauge > 0) knockoutgauge -= 1;
            }
            if (!knockoutState && knockoutgauge >= knockoutMax)
            {
                string text = "A KNOCKOUT!"; //击晕字幕
                CombatText.NewText(new Rectangle((int)npc.Center.X, (int)npc.Center.Y, npc.width, npc.height), Color.Yellow, Language.GetTextValue(text), true, false);
                npc.AddBuff(ModContent.BuffType<KnockoutDebuff>(), 180);
            }
            if (knockoutState)
            {
                knockoutgauge = 0;
                switch(npc.aiStyle)
                {
                    #region 战士AI击晕——大部分原地不动
                    case 3: //战士AI击晕后原地不动
                        npc.velocity.X = 0; //原地不动
                        int newdir;
                        if (stunCounter == 0)
                        {
                            newdir = Main.rand.NextFromList(1, -1);
                            if (newdir != npc.direction)
                                npc.direction = newdir;
                        }
                        break;
                    #endregion
                    #region 法师AI击晕——大部分原地不动
                    case 8:
                        npc.velocity.X = 0;
                        break;
                    #endregion
                }
                return false;
            }
            else return true;
        }
        private int stunFrame;
        private int stunCounter;
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (knockoutState) return false;
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }
        public override void FindFrame(Terraria.NPC npc, int frameHeight)
        {
            if (++stunCounter > 4)
            {
                stunCounter = 0;
                if (stunFrame++ >= 3)
                    stunFrame = 0;
            }
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (knockoutState)
            {
                Texture2D starTex = ModContent.Request<Texture2D>("DeusMod/Assets/Visuals/KnockoutVisual").Value;
                int height = starTex.Height / 4;
                int y = height * stunFrame;
                Vector2 drawOrigin = new(starTex.Width / 2, height / 2);

                spriteBatch.Draw(starTex, npc.position + new Vector2(npc.width / 2, -10) - screenPos, new Rectangle?(new Rectangle(0, y, starTex.Width, height)), Color.White * npc.Opacity, 0, drawOrigin, 1, 0, 0);
            }
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByItem(npc, player, item, hit, damageDone);
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByProjectile(npc, projectile, hit, damageDone);
        }
    }
}