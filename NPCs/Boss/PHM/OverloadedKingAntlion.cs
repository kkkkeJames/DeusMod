using DeusMod.Core.Systems;
using DeusMod.Projs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeusMod.NPCs.Boss.PHM
{
    public class ChargeEffect
    {
        public static Effect Grey = ModContent.Request<Effect>("DeusMod/Effects/Grey", AssetRequestMode.ImmediateLoad).Value;
    }
    /*
    基本设计思路：
    boss半血之前会保持距离并释放远程射弹攻击
    攻击之间会落地/原地不动吸取电能
    半血之后则电能吸取速度变快，且如果没满电则会一直吸
    过载模式下boss的攻击频率变快，免疫晕眩，且增加更多体术攻击
    boss的背后全程作为弱点，受到一定攻击次数后会出硬直，退出过载模式
    如果boss背后受到过量攻击，则弱点被击破，无法进入过载模式（无论血量），并且当即进大硬直
    */
    public class OverloadedKingAntlion : ModNPC
    {
        public override string Texture => "DeusMod/Assets/NPCs/Boss/PHM/OverloadedKingAntlion";
        public int electricity = 0;
        public int weakpointHealth = 750;
        public bool overloaded = false;
        public Vector2 attackpos;
        public int damscale;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 2250;
            NPC.damage = 30;
            NPC.defense = 14;
            NPC.knockBackResist = 0f;
            NPC.width = 130;
            NPC.height = 58;
            NPC.value = Item.buyPrice(0, 4, 0, 0);
            NPC.npcSlots = 20f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.buffImmune[BuffID.Confused] = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                new FlavorTextBestiaryInfoElement("The king of antlion species which gained the power of controlling lightning. Since it congregates lightening in its wings, the back of it becomes a weak point."),

                new FlavorTextBestiaryInfoElement("蚁狮族的王，不知通过何种方式获得了操纵雷电的力量。因为将雷电聚集在翅膀处，背部成为了弱点。")
            });
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(true);
            }
            Player player = Main.player[NPC.target];
            NPC.Center = player.Center + new Vector2(0, -600);
            NPC.ai[1] = -1;
        }
        public bool fall = true;
        public int fallstoptime = 0;
        public override void AI()
        {
            //ai[0]是计数器，ai[1]是攻击模式变化，ai[2]是充电及其相关异常状态
            //ai[2] = 0为正常，= 1为充电，= 2为小气绝，= 3为翅被破坏的大气绝
            #region 玩家死亡处理
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(true);
            }
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC NPC = base.NPC;
                    NPC.noTileCollide = true;
                    if (NPC.velocity.Y < -3f)
                    {
                        NPC.velocity.Y = -5f;
                    }
                    NPC.velocity.X = 0f;
                    NPC.velocity.Y -= 0.5f;
                    if (NPC.timeLeft > 60)
                    {
                        NPC.timeLeft = 60;
                    }
                    if (NPC.ai[0] != 1f)
                    {
                        NPC.ai[0] = NPC.ai[1] = NPC.ai[2] = 0f;
                        NPC.localAI[0] = NPC.localAI[1] = 0f;
                        NPC.netUpdate = true;
                    }
                    return;
                }
            }
            #endregion
            #region 一般数据
            int damscale = 2;
            if (Main.expertMode) damscale = 4;
            if (Main.masterMode) damscale = 6;
            float lifeRatio = (float)NPC.life / (float)NPC.lifeMax;
            if (electricity >= 2400)
            {
                overloaded = true;
            }
            if (lifeRatio <= 0.5f) //半血往下
            {
            }
            NPC.ai[0] += 1; //计时器
            #endregion
            #region 入场
            if (NPC.ai[1] == -1)
            {
                if (NPC.ai[0] == 1)
                {
                    //生成预警线
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), player.Center + new Vector2(0, -500), Vector2.Zero, ModContent.ProjectileType<EnterThunder>(), 0, 0, player.whoAmI);
                }
                //降  下  作  战
                if (NPC.ai[0] > 40) 
                {
                    NPC.noTileCollide = false;
                    NPC.velocity.Y = 20;
                }
                //连续下降240f或者落地以后停止
                if (fall && NPC.ai[0] > 300)
                {
                    NPC.velocity.Y = 0;
                    NPC.noTileCollide = true;
                    if(NPC.ai[0] == 360)
                    {
                        NPC.ai[0] = NPC.ai[1] = 0;
                    }
                }
                else
                {
                    if (NPC.oldPos[1].Y == NPC.position.Y && NPC.ai[0] > 41) fall = false;
                    if (!fall)
                    {
                        if(fallstoptime == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -30), Vector2.Zero, ModContent.ProjectileType<LargeLightningExplosion>(), NPC.damage / damscale, 5f, player.whoAmI);
                            player.GetModPlayer<ScreenShake>().ScreenShakeShort(36, (float)Math.PI / 2); 
                        }
                        fallstoptime++;
                        if (fallstoptime == 60)
                        {
                            NPC.velocity.Y = 0;
                            NPC.noTileCollide = true;
                            NPC.ai[0] = NPC.ai[1] = 1;
                            fallstoptime = 0;
                        }
                    }
                }
            }
            #endregion
            #region 正常移动
            if (NPC.ai[1] == 1) //三连雷：正常移动
            {
                Vector2 position = new Vector2(player.Center.X - 250 * player.direction, player.Center.Y - 250);
                Vector2 velocity = Vector2.Normalize(position - NPC.Center);
                if (Math.Abs(position.X - NPC.Center.X) >= 500)
                {
                    if (player.velocity.X * player.direction > velocity.X * player.direction)
                    {
                        NPC.velocity = Vector2.Lerp(NPC.velocity, 27f * velocity, 0.1f);
                    }
                    else
                    {
                        NPC.velocity = Vector2.Lerp(NPC.velocity, 4.8f * velocity, 0.1f);
                    }
                }
                else
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, 4.8f * velocity, 0.1f);
                }
                if (NPC.velocity.X > 0) NPC.direction = NPC.spriteDirection = 1;
                else NPC.direction = NPC.spriteDirection = -1;
            }
            #endregion
            #region 后撤
            #endregion
            #region 常态技能
            if (NPC.ai[1] == 1) //三连锁定雷劈
            {
                if (NPC.ai[0] % 80 == 40)
                {
                    //生成雷电
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), player.Center + new Vector2(0, -600), Vector2.Zero, ModContent.ProjectileType<StraightThunder>(), NPC.damage / damscale, 5f, player.whoAmI);
                }
                if (NPC.ai[0] == 240) //重置计时器和状态机，增加一次雷电技能
                {
                    NPC.ai[0] = NPC.ai[1] = 0;
                    NPC.ai[2]++;
                }
            }
            if (NPC.ai[1] == 2) //发射电球
            {
                if (NPC.ai[0] <= 40) //停稳
                {
                    if (NPC.velocity.Length() > 1) NPC.velocity *= 0.9f;
                    else NPC.velocity = Vector2.Zero;
                }
                if (NPC.ai[0] == 40 || NPC.ai[0] == 60 || NPC.ai[0] == 80) //发射电球
                {
                    int num = Main.rand.Next(3, 6);
                    for (int i = 0; i < num; i++)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, ((player.Center - NPC.Center) / (player.Center - NPC.Center).Length()).RotatedBy(Main.rand.NextFloat(-0.4f, 0.3f) * NPC.direction) * 10, ModContent.ProjectileType<LightningBall>(), NPC.damage / damscale, 5f, player.whoAmI);
                    }
                }
                if (NPC.ai[0] == 120)
                {
                    NPC.ai[0] = NPC.ai[1] = 0;
                    NPC.ai[2]++;
                }
            }
            if (NPC.ai[1] == 3) //下砸，在随即区域产生大范围落雷
            {
                if (NPC.ai[0] <= 40) //上升
                {
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = MathHelper.Lerp(-4, 0, NPC.ai[0] / 40);
                }
                if (NPC.ai[0] > 40) //下降
                {
                    NPC.noTileCollide = false;
                    NPC.velocity.Y = 20;
                    //连续下降240f或者落地以后停止
                    if (fall && NPC.ai[0] > 240)
                    {
                        NPC.velocity.Y = 0;
                        NPC.noTileCollide = true;
                        if (NPC.ai[0] == 280)
                        {
                            NPC.ai[0] = NPC.ai[1] = 0;
                            NPC.ai[2]++;
                        }
                    }
                    else
                    {
                        if (NPC.oldPos[1].Y == NPC.position.Y && NPC.ai[0] > 41) fall = false;
                        if (!fall)
                        {
                            if (fallstoptime == 0)
                            {
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -30), Vector2.Zero, ModContent.ProjectileType<LargeLightningExplosion>(), NPC.damage / damscale, 5f, player.whoAmI);
                                player.GetModPlayer<ScreenShake>().ScreenShakeShort(36, (float)Math.PI / 2);
                                int num = Main.rand.Next(6, 9);
                                for (int i = 0; i < num; i++)
                                {
                                    int xrange = Main.rand.Next(-1200, 1200);
                                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(xrange, -600), Vector2.Zero, ModContent.ProjectileType<StraightThunder>(), NPC.damage / damscale, 5f, player.whoAmI);
                                }
                            }
                            fallstoptime++;
                            if (fallstoptime == 40)
                            {
                                NPC.velocity.Y = 0;
                                NPC.noTileCollide = true;
                                NPC.ai[0] = NPC.ai[1] = 1;
                                NPC.ai[2]++;
                                fallstoptime = 0;
                            }
                        }
                    }
                }
            }
            if (NPC.ai[1] == 4) //线控雷球
            {
                if (NPC.ai[0] == 120)
                {
                    //投出两拍雷球移动一段时间后停止
                }
                if (NPC.ai[0] == 360)
                {
                    NPC.ai[0] = NPC.ai[1] = 0;
                }
            }
            if (NPC.ai[1] == 5) //通电沙尘
            {
                if (NPC.ai[0] == 0)
                {
                    //释放大量的通电沙尘
                }
                if (NPC.ai[0] == 240)
                {
                    NPC.ai[0] = NPC.ai[1] = 0;
                }
            }
            #endregion
            #region 过载技能
            #endregion
            #region 常态
            if (!overloaded)
            {
                if (NPC.ai[1] == 0) //释放完技能的空闲
                {
                    if (NPC.ai[2] >= 4) //如果可以充电
                    {
                        NPC.velocity = Vector2.Zero;
                        electricity += 2;
                        if(NPC.ai[0] >= 300) //每次充能600，也就是1/4
                        {
                            NPC.ai[0] = NPC.ai[1] = NPC.ai[2] = 0;
                        }
                    }
                    else
                    {
                        if (NPC.ai[2] % 2 == 0) NPC.ai[1] = 1; //如果是第一次或者第三次则释放雷电
                        else NPC.ai[1] = Main.rand.Next(2, 4); //否则释放一个除三连雷电的随机技能
                    }
                }
            }
            #endregion
            #region 过载
            else
            {
                electricity -= 1; //每秒减少60电，也就是40秒以后结束过载
            }
            #endregion
        }
        //如果受到来自后方的攻击则扣除翅膀的生命
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (Math.Sign(NPC.direction) == Math.Sign(NPC.Center.X - player.Center.X)) weakpointHealth -= damageDone;
            base.OnHitByItem(player, item, hit, damageDone);
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (Math.Sign(NPC.direction) == Math.Sign(NPC.Center.X - projectile.Center.X)) weakpointHealth -= damageDone;
            base.OnHitByProjectile(projectile, hit, damageDone);
        }
    }
    //入场的预警线
    public class EnterThunder : BaseWarningProj
    {
        public override string Texture => "Terraria/Images/Extra_193";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 60;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 40;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            WarningPos = Projectile.Center;
            WarningWidth = 1200;
            WarningHeight = 58;
            WarningRot = (float)Math.PI / 2;
            WarningTimeMax = 40;
            WarningColor = Color.Red;
        }
        public override void AI()
        {
            base.AI();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawWarningLine();
            return false;
        }
    }
    //大范围的砸地等效果
    public class LargeLightningExplosion : ModProjectile
    {
        public override string Texture => "DeusMod/Assets/NPCs/Boss/PHM/LargeLighteningExplosion";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 10;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter == 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Tex = ModContent.Request<Texture2D>(Texture).Value;

            int frameWidth = Tex.Width;
            int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            Rectangle MainFrame = new Rectangle(0, Projectile.frame * frameHeight, frameWidth, frameHeight);

            SpriteEffects effects = SpriteEffects.FlipHorizontally;
            Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);

            if (Projectile.spriteDirection != 1)
                effects = SpriteEffects.None;

            Main.EntitySpriteDraw(Tex, Projectile.Center - Main.screenPosition, MainFrame, Color.White, Projectile.rotation, origin, Projectile.scale, effects, 0f);
            return false;
        }
    }
    //预警40f后霹雷
    public class StraightThunder : BaseWarningProj
    {
        public override string Texture => "Terraria/Images/Extra_193";
        public Vector2[] LighteningPos = new Vector2[20];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 60;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 52;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 19; i++)
            {
                LighteningPos[i] = Projectile.Center + new Vector2(Main.rand.NextFloat(-10, 10), 60 * i);
            }
            LighteningPos[19] = Projectile.Center + new Vector2(0, 60 * 19);
            WarningPos = Projectile.Center;
            WarningWidth = 1200;
            WarningHeight = 30;
            WarningRot = (float)Math.PI / 2;
            WarningTimeMax = 40;
            WarningColor = Color.Red;
        }
        public override void AI()
        {
            base.AI();
            if (Projectile.timeLeft == 8 || Projectile.timeLeft == 4)
            {
                for (int i = 0; i < 19; i++)
                {
                    LighteningPos[i] = Projectile.Center + new Vector2(Main.rand.NextFloat(-10, 10), 60 * i);
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft <= 12)
            {
                float point = 0f;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, LighteningPos[19], 30, ref point);
            }
            else return false;
            //前两个参数没必要动，第三个是头，第四个是尾，第五个是宽度，第六个别动，这样从左下角到右上角规定哪里有碰撞伤害
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawWarningLine();
            if (Projectile.timeLeft <= 12) 
            {
                List<VertexInfo2> DrawPos = new List<VertexInfo2>();
                for (int i = 0; i < 20; i++)
                {
                    DrawPos.Add(new VertexInfo2(LighteningPos[i] + new Vector2(-4 * (20 - i) / 20f, 0) - Main.screenPosition, new Vector3(0, 0, 1), Color.Cyan));
                    DrawPos.Add(new VertexInfo2(LighteningPos[i] + new Vector2(4 * (20 - i) / 20f, 0) - Main.screenPosition, new Vector3(1, 1, 1), Color.Cyan));
                }
                #region 顶点绘制配件
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                if (DrawPos.Count >= 3)
                {
                    Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(Texture).Value;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, DrawPos.ToArray(), 0, DrawPos.Count - 2);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                #endregion
            }
            return false;
        }
    }
    //受重力影响的雷球
    public class LightningBall : BaseWarningProj
    {
        public override string Texture => "DeusMod/Assets/Projs/Bloom";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }
        public override void AI()
        {
            base.AI();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D BloomTex = ModContent.Request<Texture2D>("DeusMod/Assets/Projs/Bloom").Value;
            Color BloomColor = Color.Cyan;
            BloomColor.A = 0;
            Main.EntitySpriteDraw(BloomTex, (Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY)), null, BloomColor * 0.66f, Projectile.rotation, BloomTex.Size() * 0.5f, Projectile.scale * (1 + 0.2f * (float)Math.Sin((120 - Projectile.timeLeft) / 30f)), SpriteEffects.None);
            return false;
        }
    }
}