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
using DeusMod.Helpers;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using DeusMod.Items.Weapon.Melee.Scythes;

namespace DeusMod.NPCs.PHM.Cultist
{
    public class CultistVindicatorBlack : ModNPC
    {
        public override string Texture => "DeusMod/Assets/NPCs/PHM/Cultist/CultistVindicatorBlack";
        private Player target => Main.player[NPC.target];
        private int aiCount = 0;
        private int aiTimer = 0;
        private float JumpCount;
        private float JumpSpeed = -8f;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
        }
        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 40;
            NPC.damage = 10; //接触伤害
            NPC.defense = 10;
            NPC.lifeMax = 60;
            //NPC.HitSound = SoundID.NPCHit42;
            //NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 10f;
            NPC.knockBackResist = 0.7f;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				new FlavorTextBestiaryInfoElement("Though declined for decades, the Lunatic Cult continues to devote for its goal. These vindicators with terrifying scythes are good proofs of that."),

				new FlavorTextBestiaryInfoElement("即使已经没落了数十年，拜月教仍致力于实现其目标。这些拿着骇人的镰刀的护教者就是很好的证明。")
            });
        }
        public override void OnSpawn(IEntitySource source)
        {
            aiCount = 0;
            NPC.frame.Y = 2;
            int damscale = 2;
            if (Main.expertMode) damscale = 4;
            if (Main.masterMode) damscale = 6;
            Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CultistVindicatorBlackSword>(), NPC.damage / damscale, -0.5f, Main.myPlayer, 0);
            ((CultistVindicatorBlackSword)proj.ModProjectile).Parent = NPC;
        }
        public override void AI()
        {
            if (target.dead || !target.active || (target.Center - NPC.Center).Length() > 2000f)
                NPC.EncourageDespawn(10);
            NPC.netUpdate = true;

            float XVelMax = 1f;
            float XAccel = 0.07f;
            float XSlow = 0.8f;
            float frameVel = 15;
            JumpSpeed = -8f;
            if (aiCount == 0 && DeusModMathHelper.EucilidDistanceHelper(NPC.Center, target.Center) <= 400f)
                aiCount = 2;
            if ((aiCount == 0 || aiCount == 1) && DeusModMathHelper.EucilidDistanceHelper(NPC.Center, target.Center) > 400f)
            {
                aiCount = 0;
                foreach(Projectile proj in Main.projectile)
                {
                    if(proj.type == ModContent.ProjectileType<CultistVindicatorBlackSword>() && proj.owner == Main.myPlayer)
                        ((CultistVindicatorBlackSword)proj.ModProjectile).NormalSetIdle();
                }
            }
            if (aiCount == 1)
            {
                XVelMax = 6f;
                XAccel = 0.1f;
                frameVel = 8;
                JumpSpeed = -4f;
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.type == ModContent.ProjectileType<CultistVindicatorBlackSword>() && proj.owner == Main.myPlayer)
                        ((CultistVindicatorBlackSword)proj.ModProjectile).RunSetIdle();
                }
            }
            if (aiCount != 3 && DeusModMathHelper.EucilidDistanceHelper(NPC.Center, target.Center) <= 60f && Math.Abs(NPC.Bottom.Y - target.Bottom.Y) <= 10) //距离过近则近战攻击
            { 
                aiCount = 3;
                aiTimer = 0;
            }

            switch (aiCount)
            {
                case 0: case 1:
                    NPC.knockBackResist = 0.7f;
                    NPC.spriteDirection = Math.Sign(NPC.Center.DirectionTo(target.Center).X);
                    NPC.TargetClosest(true);
                    if (Math.Abs(NPC.velocity.X) > XVelMax)
                    {
                        if (NPC.velocity.Y == 0f)
                            NPC.velocity.X *= XSlow;
                    }
                    else
                    {
                        NPC.velocity.X += NPC.direction * XAccel;
                        if (Math.Abs(NPC.velocity.X) > XVelMax)
                            NPC.velocity.X = NPC.direction * XVelMax;
                    }
                    aiTimer++;
                    if(aiCount == 1 && aiTimer >= 120 && Main.rand.NextBool(120) && NPC.velocity.Y == 0)
                    {
                        aiTimer = 0;
                        aiCount = 4;
                    }
                    JumpCheck();
                    if (JumpCount > 3)
                    {
                        NPC.direction *= -1;
                        JumpCount = 0;
                        //aiCount = 5;
                    }
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= frameVel)
                    {
                        NPC.frame.Y++;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frame.Y >= Main.npcFrameCount[NPC.type]) NPC.frame.Y = 2;
                    if (NPC.velocity.Y != 0) NPC.frame.Y = 1;
                    else if (NPC.frame.Y == 1) NPC.frame.Y = 2;
                    break;
                case 2:
                    NPC.knockBackResist = 0.7f;
                    NPC.spriteDirection = Math.Sign(NPC.Center.DirectionTo(target.Center).X);
                    NPC.TargetClosest(true);
                    if (Math.Abs(NPC.velocity.X) > 0.1f)
                    {
                        NPC.velocity.X *= 0.8f;
                    }
                    else
                    {
                        NPC.velocity.X = 0;
                    }
                    aiTimer++;
                    if (aiTimer > 60)
                    {
                        aiTimer = 0;
                        aiCount = 1;
                    }
                    break;
                case 3:
                    NPC.knockBackResist = 0;
                    NPC.TargetClosest(false);
                    if (Math.Abs(NPC.velocity.X) > 0.1f)
                    {
                        NPC.velocity.X *= 0.8f;
                    }
                    else
                    {
                        NPC.velocity.X = 0;
                    }
                    aiTimer++;
                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.type == ModContent.ProjectileType<CultistVindicatorBlackSword>() && ((CultistVindicatorBlackSword)proj.ModProjectile).Parent == NPC)
                        {
                            if (aiTimer > 1)
                            {
                                if (NPC.GetGlobalNPC<GlobalSwordNPC>().AllowAttack && !NPC.GetGlobalNPC<GlobalSwordNPC>().IsRecover)
                                {
                                    NPC.TargetClosest(true);
                                    NPC.spriteDirection = Math.Sign(NPC.Center.DirectionTo(target.Center).X);
                                    if (DeusModMathHelper.EucilidDistanceHelper(NPC.Center, target.Center) > 60f || Math.Abs(NPC.Bottom.Y - target.Bottom.Y) > 10)
                                        aiCount = 1;
                                    else ((CultistVindicatorBlackSword)proj.ModProjectile).WieldTrigger(1f, 1f, -2.2f, 2.2f, 180, 0, 180, 0, 16);
                                }
                            }
                            else
                            {
                                ((CultistVindicatorBlackSword)proj.ModProjectile).RunSetIdle();
                                NPC.GetGlobalNPC<GlobalSwordNPC>().AllowAttack = true;
                            }
                        }
                    }
                    if (NPC.frame.Y == 1 && NPC.velocity.Y == 0) NPC.frame.Y = 2;
                    break;
                case 4:
                    NPC.knockBackResist = 0;
                    NPC.TargetClosest(false);
                    if (Math.Abs(NPC.velocity.X) > 0.1f)
                    {
                        NPC.velocity.X *= 0.8f;
                    }
                    else
                    {
                        NPC.velocity.X = 0;
                    }
                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.type == ModContent.ProjectileType<CultistVindicatorBlackSword>() && ((CultistVindicatorBlackSword)proj.ModProjectile).Parent == NPC)
                        {
                            if (aiTimer > 0)
                            {
                                NPC.TargetClosest(true);
                                NPC.spriteDirection = Math.Sign(NPC.Center.DirectionTo(target.Center).X);
                                if (NPC.GetGlobalNPC<GlobalSwordNPC>().AllowAttack && !NPC.GetGlobalNPC<GlobalSwordNPC>().IsRecover)
                                {
                                    if(aiTimer > 1)
                                    {
                                        aiTimer = 0;
                                        aiCount = 1;
                                    }
                                    else 
                                    {
                                        ((CultistVindicatorBlackSword)proj.ModProjectile).WieldTrigger(1f, 0.5f, -3f, 3f, 180, ModContent.ProjectileType<ScytheOfRetributionProjHostile>(), 180, 0, 16);
                                        aiTimer++; 
                                    }
                                }
                            }
                            else
                            {
                                aiTimer++;
                                ((CultistVindicatorBlackSword)proj.ModProjectile).NormalSetIdle();
                                NPC.GetGlobalNPC<GlobalSwordNPC>().AllowAttack = true;
                            }
                        }
                    }
                    if (NPC.frame.Y == 1 && NPC.velocity.Y == 0) NPC.frame.Y = 2;
                    break;
                case 5:
                    NPC.knockBackResist = 0.7f;
                    NPC.TargetClosest(false);
                    break;
            }
            
        }
        public void JumpCheck()
        {
            bool FaceSolidTile = false;
            bool HeadSolidTile = false;
            bool StepEmptyTile = true;
            Vector2 HeadTile = new Vector2((int)(NPC.direction == 1 ? NPC.TopRight.X : NPC.TopLeft.X) / 16, (int)(NPC.Top.Y / 16));
            Vector2 CenterTile = new Vector2((int)(NPC.direction == 1 ? NPC.BottomRight.X : NPC.BottomLeft.X) / 16, (int)NPC.Center.Y / 16);
            if (WorldGen.SolidTile((int)(CenterTile.X + (NPC.direction == 1 ? 0 : -1)), (int)CenterTile.Y))
                FaceSolidTile = true;
            for (int i = -1; i <= 0; i++)
            {
                //左侧：1，0 右侧：-2，-1
                if (WorldGen.SolidTile((int)HeadTile.X + i * NPC.direction + (NPC.direction == 1 ? -1 : 0), (int)HeadTile.Y - 1))
                {
                    HeadSolidTile = true;
                    break;
                }
            }
            //if (!WorldGen.SolidTile((int)(CenterTile.X + (NPC.direction == 1 ? 0 : -1)), (int)CenterTile.Y + 2))
            if (Main.tile[(int)CenterTile.X + (NPC.direction == 1 ? 0 : -1), (int)CenterTile.Y + 2] != null)
                StepEmptyTile = false;
            if (HeadSolidTile && FaceSolidTile)
            {
                JumpCount += 0.1f;
                return;
            }

            if (NPC.velocity.Y == 0 && (FaceSolidTile || StepEmptyTile))
            {
                NPC.velocity.Y = JumpSpeed;
                JumpCount += 1f;
            }
        }
        public override bool? CanFallThroughPlatforms()
        {
            if (aiCount == 3 && aiCount == 4)
            {
                return false;
            }
            else
            {
                if (NPC.Bottom.Y / 16 < target.Bottom.Y / 16 - 1)
                    return true;
                
                else return false;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteEffects armeffects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D MainTex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D ArmTex = ModContent.Request<Texture2D>(Texture + "Arm").Value;
            Vector2 ArmOrigin = NPC.spriteDirection == 1 ? Vector2.Zero : new Vector2(0, ArmTex.Height);
            int frameheight = MainTex.Height / Main.npcFrameCount[NPC.type];
            Rectangle Mainframe = new Rectangle((MainTex.Width - NPC.width) / 2, NPC.frame.Y * frameheight, NPC.width, frameheight);
            NPC.GetGlobalNPC<GlobalSwordNPC>().ArmOffset = new Vector2(-8 * NPC.direction, NPC.frame.Y == 3 || NPC.frame.Y == 4 ? -6 : -4);
            NPC.GetGlobalNPC<GlobalSwordNPC>().ArmLength = ArmTex.Width;
            NPC.GetGlobalNPC<GlobalSwordNPC>().HandOffset = Vector2.Zero;
            Main.EntitySpriteDraw(MainTex, NPC.Center - screenPos, Mainframe, drawColor, 0f, NPC.Size / 2 + new Vector2(0, 12), NPC.scale, effects, default);
            Main.EntitySpriteDraw(ArmTex, NPC.Center - screenPos + NPC.GetGlobalNPC<GlobalSwordNPC>().ArmOffset, null, drawColor, NPC.GetGlobalNPC<GlobalSwordNPC>().ArmRotation, ArmOrigin, NPC.scale, armeffects, default);
            return false;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScytheOfRetribution>(), 4));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneNormalUnderground) 
                return 0.25f;
            return 0f;
        }
    }
    public class CultistVindicatorBlackSword : DeusGlobalNPCSlash
    {
        public override string Texture => "DeusMod/Assets/Items/Weapon/Melee/Scythe/ScytheOfRetribution";
        public CultistVindicatorBlackSword()
        {

        }
        public override void RegisterVariables()
        {
            SlashColor = Color.DarkGray;
            IdleSet.Set(new Vector2(-24, 0).RotatedBy(-3f), -3f, 0, 1);
            WieldDrawLessLength = 12;
        }
        public void RunSetIdle()
        {
            IdleSet.Set(new Vector2(-16, 0).RotatedBy(-2.2f), -2.2f, -2.2f, 1);
        }
        public void NormalSetIdle()
        {
            IdleSet.Set(new Vector2(-24, 0).RotatedBy(-3f), -3f, 0, 1);
        }
        public override void Appear()
        {
        }

    }
    public class ScytheOfRetributionProjHostile : ModProjectile
    {
        public override string Texture => "DeusMod/Assets/Items/Weapon/Melee/Scythe/ScytheOfRetribution";
        public static Asset<Texture2D> SlashTexture;
        private float rotinc;
        public override void Load()
        {
            SlashTexture = ModContent.Request<Texture2D>("DeusMod/Effects/Textures/Slash");
        }
        public override void Unload()
        {
            SlashTexture = null;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 2 * (int)DeusModMathHelper.PythagoreanHelper(ModContent.Request<Texture2D>(Texture).Value.Width, ModContent.Request<Texture2D>(Texture).Value.Height);
            Projectile.height = (int)DeusModMathHelper.PythagoreanHelper(ModContent.Request<Texture2D>(Texture).Value.Width, ModContent.Request<Texture2D>(Texture).Value.Height);
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 1440;
        }
        public override void AI()
        {
            Projectile.velocity = new Vector2(2 * Projectile.direction, 0);
            Projectile.ai[0]++;
            rotinc = (float)Math.PI / 45 * Math.Sign(Projectile.velocity.X);
            Projectile.ai[1] += rotinc;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo2> slash = new List<VertexInfo2>();
            List<VertexInfo2> line = new List<VertexInfo2>();
            int iTimer = Projectile.ai[0] < 39 ? (int)Projectile.ai[0] + 1 : Projectile.oldPos.Length;
            for (int i = 0; i < iTimer; i++) //挥砍时长小于31时，挥砍时长小于oldpos记录长度，则oldpos和oldrot的最大值不能被记录，大于31就可以直接回溯最远的oldpos了
            {
                Vector2 pos = Projectile.Center - Main.screenPosition; //挥砍的中心都是以剑柄为中心
                slash.Add(new VertexInfo2(pos + new Vector2(DeusModMathHelper.EllipseRadiusHelper(Projectile.width, Projectile.height, Projectile.ai[1] - rotinc * i) / 2, 0).RotatedBy(Projectile.ai[1] - rotinc * i), new Vector3(1 - (float)i / iTimer, 0, 1), Color.DarkGray * MathHelper.Lerp(1, 0, (float)i / iTimer)));
                slash.Add(new VertexInfo2(pos + new Vector2(DeusModMathHelper.EllipseRadiusHelper(Projectile.width, Projectile.height, Projectile.ai[1] - rotinc * i) / 4, 0).RotatedBy(Projectile.ai[1] - rotinc * i), new Vector3(1 - (float)i / iTimer, 0.12f, 1), Color.DarkGray * MathHelper.Lerp(1, 0, (float)i / iTimer)));
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = Projectile.Center - Main.screenPosition; //挥砍的中心都是以剑柄为中心
                line.Add(new VertexInfo2(pos + new Vector2(DeusModMathHelper.EllipseRadiusHelper(Projectile.width, Projectile.height, Projectile.ai[1]) / 2, -1 + i * 2).RotatedBy(Projectile.ai[1]), new Vector3(0.8f, 0, 1), Color.DarkGray * 0.8f));
                line.Add(new VertexInfo2(pos + new Vector2(0, -1 + i * 2).RotatedBy(Projectile.ai[1]), new Vector3(0.8f, 0.01f, 1), Color.DarkGray * 0.8f));
            }
            #region 顶点绘制配件
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (slash.Count >= 3)
            {
                Main.graphics.GraphicsDevice.Textures[0] = SlashTexture.Value;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, slash.ToArray(), 0, slash.Count - 2);
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, line.ToArray(), 0, line.Count - 2);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            #endregion
            return false;
        }
    }
}
