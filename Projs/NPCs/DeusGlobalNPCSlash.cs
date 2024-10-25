using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using DeusMod.Dusts;
using DeusMod.Core.Systems;
using ReLogic.Content;
using DeusMod.Helpers;
using DeusMod.Core;

namespace DeusMod.Projs.NPCs
{
    public abstract class DeusGlobalNPCSlash : ProjStateMachine, IDrawWarp
    {
        public static Asset<Texture2D> SlashTexture;
        public static Asset<Texture2D> WarpTexture;
        public override void Load()
        {
            SlashTexture = ModContent.Request<Texture2D>("DeusMod/Effects/Textures/Slash");
            WarpTexture = ModContent.Request<Texture2D>("DeusMod/Effects/Textures/WarpTex");
        }
        public override void Unload()
        {
            SlashTexture = null;
            WarpTexture = null;
        }
        #region 全局变量
        public Texture2D SwordTexture;
        public NPC Parent; //拥有射弹的NPC
        public bool DrawInverse = false; //全局变量——绘制武器贴图时是否反向绘制
        public float ProjRadius; //全局变量——弹幕本体从左下到右上计算得出的半径
        public Vector2 SwordArmPosAdd; //全局变量——剑本体的“尾端”和玩家手臂的相对位置
        public float ArmRotation; //全局变量——敌人手臂的角度
        public bool DrawBehindNPC = false; //全局变量——剑是否绘制在敌人图层后
        public float DamageScale = 1; //伤害倍率——武器伤害的倍率，在NPC基础攻击上乘算伤害
        public float AngleFix; //攻击等等的角度修正
        public bool Clashable = false; //全局变量——本次攻击是否可以拼刀
        public int ShootProj;
        #endregion
        #region 状态机变量
        public struct PosSet //记录剑的位置信息的结构体
        {
            public Vector2 SwordArmAdd; //剑柄尾端和玩家手臂的向量差
            public float Rot; //剑的角度
            public float ArmRot; //真手臂的角度
            public float Scale; //剑的大小倍率
            public void Set(Vector2 add, float rot, float armrot, float scale) //储存剑的位置
            {
                SwordArmAdd = add;
                Rot = rot;
                ArmRot = armrot;
                Scale = scale;
            }
        }
        public PosSet IniSet; //初始位置的结构体
        public PosSet TargetSet; //目标位置的结构体
        public PosSet IdleSet; //待机时位置的结构体
        public float TimeMax; //最大执行时长
        public float StopTime; //停止攻击动作的时间（特指射弹的位置变化，攻击状态本身不结束）
        public float FixRot; //动作状态——NPC攻击的角度修正
        public bool CouldHit = false; //射弹是否可以攻击
        #endregion
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.extraUpdates = 3;
            Projectile.hide = true;
            Projectile.GetGlobalProjectile<DeusGlobalProjs>().ClashType = 2;
        }
        public override void OnSpawn(IEntitySource source)
        {
        }
        public float SwordLogisticHelper(float time, float maxtime) //求出剑类的特有前期快速增长后期平滑逻辑斯蒂模型，数据分别为当前时间和最大时间
        {
            return DeusModMathHelper.LogisticHelper(3.14f, 1.57f, -0.23f / 4, 1.57f, time * (80 / maxtime)) / DeusModMathHelper.LogisticHelper(3.14f, 1.57f, -0.23f / 4, 1.57f, 80);
        }
        //将射弹的三项数据进行平滑移动，因此不适合有函数变化的位移
        //将射弹位置变为target位置
        public void PosSetTarget(Projectile proj, float timer, bool shortestrot = false, bool shortestarmrot = false)
        {
            DeusGlobalNPCSlash projmod = (DeusGlobalNPCSlash)proj.ModProjectile;
            projmod.SwordArmPosAdd = new Vector2(MathHelper.Lerp(projmod.IniSet.SwordArmAdd.X, projmod.TargetSet.SwordArmAdd.X, timer), MathHelper.Lerp(projmod.IniSet.SwordArmAdd.Y, projmod.TargetSet.SwordArmAdd.Y, timer));
            proj.scale = MathHelper.Lerp(projmod.IniSet.Scale, projmod.TargetSet.Scale, timer);
            projmod.ArmRotation = MathHelper.Lerp(projmod.IniSet.ArmRot, projmod.TargetSet.ArmRot, timer);
            if (shortestrot)
            {
                while (projmod.IniSet.Rot - projmod.TargetSet.Rot >= Math.PI * 2)
                    projmod.IniSet.Rot -= (float)Math.PI * 2;
                while (projmod.IniSet.Rot - projmod.TargetSet.Rot <= -Math.PI * 2)
                    projmod.IniSet.Rot += (float)Math.PI * 2;
                if (projmod.IniSet.Rot - projmod.TargetSet.Rot >= Math.PI)
                    projmod.IniSet.Rot -= (float)Math.PI * 2;
                if (projmod.IniSet.Rot - projmod.TargetSet.Rot <= -Math.PI)
                    projmod.IniSet.Rot += (float)Math.PI * 2;
            }
            if (shortestarmrot)
            {
                while (projmod.IniSet.ArmRot - projmod.TargetSet.ArmRot >= Math.PI * 2)
                    projmod.IniSet.ArmRot -= (float)Math.PI * 2;
                while (projmod.IniSet.ArmRot - projmod.TargetSet.ArmRot <= -Math.PI * 2)
                    projmod.IniSet.ArmRot += (float)Math.PI * 2;
                if (projmod.IniSet.ArmRot - projmod.TargetSet.ArmRot >= Math.PI)
                    projmod.IniSet.ArmRot -= (float)Math.PI * 2;
                if (projmod.IniSet.ArmRot - projmod.TargetSet.ArmRot <= -Math.PI)
                    projmod.IniSet.ArmRot += (float)Math.PI * 2;
            }
            proj.rotation = MathHelper.Lerp(projmod.IniSet.Rot, projmod.TargetSet.Rot, timer);
        }
        //将射弹的角度通过剑的逻辑斯蒂模型进行变换
        public void RotSetTargetLogistic(Projectile proj, float time, float maxtime)
        {
            DeusGlobalNPCSlash projmod = (DeusGlobalNPCSlash)proj.ModProjectile;
            proj.rotation = projmod.IniSet.Rot + (projmod.TargetSet.Rot - projmod.IniSet.Rot) * SwordLogisticHelper(time, maxtime);
        }
        //先于一切状态机执行的AI
        //初始化数据和各类state
        public abstract void RegisterVariables();
        public abstract void Appear();
        public override void Initialize()
        {
            RegisterVariables();
            Projectile.rotation = IdleSet.Rot;
            Appear();
            RegisterState(new Idle());
            RegisterState(new Wield());
            RegisterState(new Clash());
            RegisterState(new Recover());
        }

        public override void AIBefore()
        {
            SwordTexture = ModContent.Request<Texture2D>(Texture).Value;
            if (!Parent.active) Projectile.Kill();
            else Projectile.timeLeft = 10;
            Projectile.width = (int)(Projectile.scale * SwordTexture.Width);
            Projectile.height = (int)(Projectile.scale * SwordTexture.Height); //射弹长度 = 材质长度*射弹大小
            ProjRadius = DeusModMathHelper.PythagoreanHelper(Projectile.width, Projectile.height) / 2; //勾股算半径
            DrawBehindNPC = false;
            int damscale = 2;
            if (Main.expertMode) damscale = 4;
            if (Main.masterMode) damscale = 6;
            Projectile.damage = (int)(Parent.damage / damscale * DamageScale); //同步攻击
        }
        //状态：待机
        public class Idle : ProjState
        {
            public override void AI(ProjStateMachine projectile)
            {
                #region 变量简记
                Projectile proj = projectile.Projectile;
                DeusGlobalNPCSlash projmod = (DeusGlobalNPCSlash)proj.ModProjectile;
                NPC Parent = projmod.Parent;
                #endregion
                #region 此AI数据
                Parent.GetGlobalNPC<GlobalSwordNPC>().AllowAttack = true;
                Parent.GetGlobalNPC<GlobalSwordNPC>().IsRecover = false;
                #endregion
                #region 待机时剑的位置
                projmod.Draw_SwordShouldDraw = true; //固定绘制剑身
                projmod.CouldHit = false;
                projmod.DrawBehindNPC = true; //剑绘制在玩家图层后
                projmod.SwordArmPosAdd = new Vector2(projmod.IdleSet.SwordArmAdd.X * Parent.direction, projmod.IdleSet.SwordArmAdd.Y);
                proj.rotation = (float)Math.PI / 2 - projmod.Parent.direction * ((float)Math.PI / 2 - projmod.IdleSet.Rot);
                projmod.ArmRotation = (float)Math.PI / 2 - projmod.Parent.direction * ((float)Math.PI / 2 - projmod.IdleSet.ArmRot);
                proj.scale = projmod.IdleSet.Scale;
                projmod.DrawInverse = Parent.direction == 1 ? false : true;
                #endregion
            }
        }
        #region 挥剑变量
        public float WieldStandardScale; //挥剑椭圆的长半径的放大倍率值
        public float WieldThinScale; //椭圆短半径和长半径的比例值
        public float WieldHoldRot; //挥剑的举剑角度
        public float WieldFinalRot; //挥剑的最终角度
        public float[] WieldDrawRadius = new float[10001]; //顶点绘制的旧半径记录
        public bool WieldDrawArmBefore; //挥剑之前的AI是不是在绘制手臂
        public bool WieldAttack = false; //是否绘制剑气
        public int WieldTimer; //计时器
        public int WieldStopTime; //停止动作的事件
        public float HandleHoldLength = 0; //手和剑柄的距离
        public float WieldDrawLessLength = 0; //顶点绘制距离剑尖的距离
        #endregion
        /*状态：挥剑
        挥剑由举剑和挥剑两者组成
        举剑时的目标角度为独有的HoldSet.Rot，需要用IniSet.Rot配合HoldSet.Rot求出该角度的倍率
        挥剑时的目标角度为targrot，需要用targrot配合mouserot求出该角度的倍率
         */
        //读入挥剑的变量
        public void WieldTrigger(float standardscale, float thinscale, float holdrot, float targrot, float timemax, int projtype = 0, int recovertimer = 240, float anglefix = 0, float handlelength = 0, float stoptime = 0)
        {
            Parent.GetGlobalNPC<GlobalSwordNPC>().AllowAttack = false;
            Timer = 0; //重置计时器
            TimeMax = timemax;
            RecoverTimer = recovertimer;
            HandleHoldLength = handlelength;
            IniSet.Set(SwordArmPosAdd, Projectile.rotation, ArmRotation, Projectile.scale); //获取iniset
            WieldHoldRot = holdrot; //获取举剑的目标角度
            WieldFinalRot = targrot; //获取挥剑的目标角度
            DrawInverse = Parent.direction < 0 ? true : false; //改变射弹绘制方向
            if (targrot < holdrot) DrawInverse = !DrawInverse; //如果从下向上挥则改变射弹方向
            if (Parent.direction < 0) //如果玩家为负方向
            {
                WieldHoldRot = (float)Math.PI - WieldHoldRot; //改变两个角度使得角度变成正向
                WieldFinalRot = (float)Math.PI - WieldFinalRot;
            }
            float scl = DeusModMathHelper.EllipseRadiusHelper(standardscale, standardscale * thinscale, WieldHoldRot);
            //加入预设的角度修正
            WieldHoldRot += anglefix;
            WieldFinalRot += anglefix;
            WieldStandardScale = standardscale; //解决半径问题
            WieldThinScale = thinscale;
            TargetSet.Set(new Vector2(-HandleHoldLength, 0).RotatedBy(WieldHoldRot), WieldHoldRot, WieldHoldRot, scl);
            ShootProj = projtype;
            ((DeusGlobalNPCSlash)Projectile.ModProjectile).SetState<Wield>();
        }
        private class Wield : ProjState
        {
            public override void AI(ProjStateMachine projectile)
            {
                #region 基础设置
                Projectile proj = projectile.Projectile;
                DeusGlobalNPCSlash projmod = (DeusGlobalNPCSlash)proj.ModProjectile;
                int HoldupTimeMax = (int)(projmod.TimeMax / 3); //举剑的时长等于使用武器时间的1/3
                int WieldTimeMax = (int)projmod.TimeMax - HoldupTimeMax; //挥剑的时长等于使用武器时间的2/3
                projmod.Timer++; //射弹计时器
                #endregion
                #region 总时长的前1/3：举剑
                if (projmod.Timer <= HoldupTimeMax) //如果总时长小于举剑的时长
                {
                    projmod.PosSetTarget(proj, (float)projmod.Timer / HoldupTimeMax, true, true);
                }
                #endregion
                #region 后2/3：挥剑
                else
                {
                    projmod.Clashable = true;
                    projmod.WieldAttack = true;
                    projmod.CouldHit = true;
                    projmod.DrawTimer = projmod.WieldTimer = projmod.Timer - HoldupTimeMax;
                    #region 记录初始数据
                    if (projmod.WieldTimer == 1) //当时间到了刚开始计算挥剑的位置
                    {
                        projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //重新记录Ini类原始数据
                        #region 一部分需要计算得出的数据
                        //通过挥剑标准倍率、挥剑缩小倍率、挥剑初始角度、挥剑最终角度、挥剑角度是否考虑鼠标和鼠标位置等计算得出最终的倍率大小
                        float targscale = DeusModMathHelper.EllipseRadiusHelper(projmod.WieldStandardScale, projmod.WieldStandardScale * projmod.WieldThinScale, projmod.WieldFinalRot - projmod.AngleFix);
                        projmod.TargetSet.Set(new Vector2(-projmod.HandleHoldLength, 0).RotatedBy(projmod.WieldFinalRot), projmod.WieldFinalRot, projmod.WieldFinalRot - (float)Math.PI / 2, targscale);//重新记录targ数据
                        #endregion
                    }
                    #endregion
                    #region 角度、位置、大小变化，不是平滑变化
                    projmod.RotSetTargetLogistic(proj, projmod.Timer - HoldupTimeMax, (float)(projmod.TimeMax - HoldupTimeMax)); //角度变化
                    projmod.SwordArmPosAdd = new Vector2(-projmod.HandleHoldLength, 0).RotatedBy(proj.rotation);
                    proj.scale = DeusModMathHelper.EllipseRadiusHelper(projmod.WieldStandardScale, projmod.WieldStandardScale * projmod.WieldThinScale, projmod.Projectile.rotation - projmod.AngleFix);
                    projmod.WieldDrawRadius[projmod.WieldTimer] = projmod.ProjRadius; //像oldpos一样记录绘制半径
                    projmod.ArmRotation = proj.rotation;
                    #endregion
                    #region 挥剑到正中间位置（也就是挥剑的1/4时间点）时发射弹幕
                    if (projmod.ShootProj != 0 && projmod.Timer == HoldupTimeMax + WieldTimeMax / 4)
                    {
                        Projectile shootproj = Projectile.NewProjectileDirect(proj.GetSource_FromThis(), projmod.Parent.Center, Vector2.Zero, projmod.ShootProj, proj.damage, proj.knockBack, Main.myPlayer);
                        shootproj.direction = projmod.Parent.direction;
                    }
                    #endregion
                    #region 状态机的处理
                    if (projmod.Timer >= projmod.TimeMax) //结束
                    {
                        Vector2 swordarmposadd = new Vector2(projmod.IdleSet.SwordArmAdd.X * projmod.Parent.direction, projmod.IdleSet.SwordArmAdd.Y);
                        float rot = (float)Math.PI / 2 - projmod.Parent.direction * ((float)Math.PI / 2 - projmod.IdleSet.Rot);
                        float armrot = (float)Math.PI / 2 - projmod.Parent.direction * ((float)Math.PI / 2 - projmod.IdleSet.ArmRot);
                        projmod.Clashable = false;
                        projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //设置iniset
                        projmod.TargetSet.Set(swordarmposadd, rot, armrot, projmod.IdleSet.Scale);
                        projmod.Timer = 0; //复原计时器
                        projmod.TimeMax = projmod.RecoverTimer; //设置最大时间
                        projmod.SetState<Recover>(); //设置AI
                        return;
                    }
                    #endregion
                }
                #endregion
            }
        }
        public void ImperfectClash()
        {
            Clashable = false;
            CouldHit = false;
            WieldAttack = false;
        }
        public void PerfectClash()
        {
            Clashable = false;
            Timer = 0;
            TimeMax = 120;
            SetState<Clash>();
        }
        public class Clash : ProjState
        {
            public override void AI(ProjStateMachine projectile)
            {
                Projectile proj = projectile.Projectile;
                DeusGlobalNPCSlash projmod = (DeusGlobalNPCSlash)proj.ModProjectile;
                projmod.Clashable = false;
                projmod.WieldAttack = false;
                projmod.CouldHit = false;
                projmod.Timer++;
                float add = !projmod.DrawInverse ? MathHelper.Lerp(0.04f, 0, projmod.Timer / 120f) : -MathHelper.Lerp(0.04f, 0, projmod.Timer / 120f);
                proj.rotation -= add;
                projmod.ArmRotation -= add;
                projmod.SwordArmPosAdd = new Vector2(-projmod.HandleHoldLength, 0).RotatedBy(proj.rotation);
                if (projmod.Timer >= projmod.TimeMax)
                {
                    Vector2 swordarmposadd = new Vector2(projmod.IdleSet.SwordArmAdd.X * projmod.Parent.direction, projmod.IdleSet.SwordArmAdd.Y);
                    float rot = (float)Math.PI / 2 - projmod.Parent.direction * ((float)Math.PI / 2 - projmod.IdleSet.Rot);
                    float armrot = (float)Math.PI / 2 - projmod.Parent.direction * ((float)Math.PI / 2 - projmod.IdleSet.ArmRot);
                    projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //设置iniset
                    projmod.TargetSet.Set(swordarmposadd, rot, armrot, projmod.IdleSet.Scale);
                    projmod.Timer = 0; //复原计时器
                    projmod.TimeMax = projmod.RecoverTimer; //设置最大时间
                    projmod.SetState<Recover>(); //设置AI
                }
            }
        }
        public int RecoverTimer = 240;
        public class Recover : ProjState
        {
            public override void AI(ProjStateMachine projectile)
            {
                Projectile proj = projectile.Projectile;
                DeusGlobalNPCSlash projmod = (DeusGlobalNPCSlash)proj.ModProjectile;
                projmod.WieldAttack = false;
                projmod.CouldHit = false;
                projmod.Timer++;
                projmod.Parent.GetGlobalNPC<GlobalSwordNPC>().AllowAttack = true;
                projmod.Parent.GetGlobalNPC<GlobalSwordNPC>().IsRecover = true;
                #region 状态机的处理
                if (projmod.Timer > projmod.TimeMax / 5 * 4)
                    projmod.PosSetTarget(proj, (projmod.Timer - (projmod.TimeMax / 5 * 4)) / (projmod.TimeMax / 5), true);
                if (projmod.Timer > projmod.TimeMax) //时间到，结束状态
                {
                    projmod.Timer = 0;
                    projmod.SetState<Idle>();
                    return;
                }
                #endregion
            }
        }
        public override void AIAfter()
        {
            Parent.GetGlobalNPC<GlobalSwordNPC>().ArmRotation = ArmRotation;
            Projectile.Center = Parent.Center + Parent.GetGlobalNPC<GlobalSwordNPC>().ArmOffset + new Vector2(Parent.GetGlobalNPC<GlobalSwordNPC>().ArmLength, 0).RotatedBy(ArmRotation) + SwordArmPosAdd + new Vector2(ProjRadius, 0).RotatedBy(Projectile.rotation) + new Vector2(Parent.GetGlobalNPC<GlobalSwordNPC>().HandOffset.X * Parent.direction, Parent.GetGlobalNPC<GlobalSwordNPC>().HandOffset.Y); //表现为当剑和手臂的距离为0时剑刚好拿在手上
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            if (CouldHit)
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + new Vector2(-ProjRadius, 0).RotatedBy(Projectile.rotation), Projectile.Center + new Vector2(ProjRadius, 0).RotatedBy(Projectile.rotation), 8, ref point);
            //前两个参数没必要动，第三个是头，第四个是尾，第五个是宽度，第六个别动，这样从左下角到右上角规定哪里有碰撞伤害
            else return false;
        }
        public override bool? CanCutTiles()
        {
            return CouldHit;
        }

        #region 命中特效变量
        public bool HitEffect_MegaShock; //命中特效——攻击的屏幕震动是否是大的震动
        public int HitEffect_Debuff1; //命中特效——对敌人释放的debuff
        public int HitEffect_Debuff1time; //命中特效——debuff1的持续时间，以秒计算
        public int HitEffect_Debuff1rate; //命中特效——debuff1的释放几率，是1分之几这个分数的分母
        public int HitEffect_Debuff2; //命中特效——debuff2
        public int HitEffect_Debuff2time; //命中特效——debuff2的时间
        public int HitEffect_Debuff2rate; //命中特效——debuff2的几率
        public int HitEffect_Dust1; //命中特效——尘埃1，在目标上以血式溅出
        public int HitEffect_Dust2; //命中特效——尘埃2，在目标上以孢子式飞出
        public int HitEffect_Dust3; //命中特效——尘埃3，在剑周围逸散
        #endregion
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            #region 击退
            if (target.Center.X - (target.width / 2) > Parent.Center.X - (Parent.width / 2)) Projectile.velocity.X = 1; //调整方向以处理击退方向
            else if (target.Center.X + (target.width / 2) < Parent.Center.X + (Parent.width / 2)) Projectile.velocity.X = -1;
            #endregion
            #region 施加debuff
            if (HitEffect_Debuff1 != 0)
            {
                if (Main.rand.NextBool(HitEffect_Debuff1rate))
                    target.AddBuff(HitEffect_Debuff1, HitEffect_Debuff1time * 60);
            }
            if (HitEffect_Debuff2 != 0)
            {
                if (Main.rand.NextBool(HitEffect_Debuff2rate))
                    target.AddBuff(HitEffect_Debuff2, HitEffect_Debuff2time * 60);
            }
            #endregion
            #region 命中尘埃
            Dust dust = Dust.NewDustDirect(target.Center - new Vector2(21, 5), 0, 0, ModContent.DustType<SwordSlash>());
            dust.velocity = Vector2.Zero;
            dust.scale = Main.rand.NextFloat(0.3f, 0.4f);
            dust.rotation = Main.rand.NextFloat(-0.5f, 0.5f);
            #endregion
            #region 剑命中斩切尘埃，玩家固定飙血灰尘
            if (HitEffect_Dust1 != 0)
            {
                int num1 = Main.rand.Next(4, 7);
                for (int i = 0; i < num1; i++)
                {
                    Dust dust2 = Dust.NewDustDirect(target.position, 0, 0, HitEffect_Dust1);
                    if (Parent.position.X < target.position.X)
                        dust2.velocity = new Vector2(Main.rand.NextFloat(3.6f, 7.2f), Main.rand.NextFloat(-2.4f, 3.6f));
                    else dust2.velocity = new Vector2(Main.rand.NextFloat(-7.2f, -3.6f), Main.rand.NextFloat(-2.4f, 3.6f));
                    dust2.scale = Main.rand.NextFloat(0.8f, 1f);
                    dust2.noGravity = false;
                }
            }
            if (HitEffect_Dust2 != 0)
            {
                int num2 = Main.rand.Next(4, 7);
                for (int i = 0; i < num2; i++)
                {
                    Dust dust2 = Dust.NewDustDirect(target.position, 0, 0, HitEffect_Dust2);
                    dust2.velocity = new Vector2(Main.rand.NextFloat(-3.6f, 3.6f), Main.rand.NextFloat(-3.6f, 3.6f));
                    dust2.scale = Main.rand.NextFloat(0.8f, 1.2f);
                    dust2.noGravity = true;
                }
            }
            int num = Main.rand.Next(4, 7);
            for (int i = 0; i < num; i++)
            {
                Dust dust2 = Dust.NewDustDirect(target.position, 0, 0, DustID.Blood);
                if (Parent.position.X < target.position.X)
                    dust2.velocity = new Vector2(Main.rand.NextFloat(3.6f, 7.2f), Main.rand.NextFloat(-2.4f, 3.6f));
                else dust2.velocity = new Vector2(Main.rand.NextFloat(-7.2f, -3.6f), Main.rand.NextFloat(-2.4f, 3.6f));
                dust2.scale = Main.rand.NextFloat(0.8f, 1f);
                dust2.noGravity = false;
            }
            #endregion
        }
        #region 绘制变量
        public Color SlashColor; //绘制特效——剑气顶点绘制的颜色
        public int DrawTimer; //绘制计时器
        public int DrawTimeMax; //绘制计时器的上限
        public bool Draw_SwordShouldDraw = true; //绘制特效——武器本体是否绘制
        public bool Draw_SlashShouldDraw = true; //绘制特效——武器斩击特效是否绘制
        #endregion
        public override bool PreDraw(ref Color lightColor)
        {
            //如果剑本体可以被绘制
            if (Draw_SwordShouldDraw)
            {
                #region 剑绘制
                Rectangle rect = new Rectangle(0, 0, SwordTexture.Width, SwordTexture.Height);
                float anglefix = !DrawInverse ? 0 : (float)(0.5 * Math.PI);
                SpriteEffects effects = !DrawInverse ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                //origin代表从图像的center所在贴图处的位置
                //position才代表绘图位置
                //rectangle代表从材质原图的哪个点扣多少的距离作为实际图片
                //因为所有的剑都是剑尖向右上角放置，剑尖方向并不是和射弹0度是一个指向，所以角度需要加一个偏转角度使其和射弹角度一致，这里也是顺时针角度是正角度
                Main.EntitySpriteDraw(SwordTexture, Projectile.Center - Main.screenPosition, rect, lightColor,
                Projectile.rotation + anglefix + (float)Math.Atan((float)SwordTexture.Height / (float)SwordTexture.Width),
                new Vector2(SwordTexture.Width / 2, SwordTexture.Height / 2), Projectile.scale, effects, 0);
                #endregion
            }
            #region 挥剑状态的顶点绘制
            //slash存储的所有坐标都是在屏幕上的坐标
            //上面的add是绘制的上顶点，下面的add是绘制的下顶点
            if (WieldAttack) //状态机正在绘制
            {
                List<VertexInfo2> slash = new List<VertexInfo2>();
                int iTimer = DrawTimer < 39 ? DrawTimer + 1 : Projectile.oldPos.Length;
                for (int i = 0; i < iTimer; i++) //挥砍时长小于31时，挥砍时长小于oldpos记录长度，则oldpos和oldrot的最大值不能被记录，大于31就可以直接回溯最远的oldpos了
                {
                    Vector2 pos = Projectile.Center - new Vector2(ProjRadius, 0).RotatedBy(Projectile.rotation) - Main.screenPosition; //挥砍的中心都是以剑柄为中心
                    slash.Add(new VertexInfo2(pos + new Vector2(WieldDrawRadius[DrawTimer - i] * 2 - WieldDrawLessLength * Projectile.scale, 0).RotatedBy(Projectile.oldRot[i]), new Vector3(1 - (float)i / DrawTimer, 0, 1), SlashColor * MathHelper.Lerp(1, 0, (float)i / DrawTimer)));
                    slash.Add(new VertexInfo2(pos + new Vector2(WieldDrawRadius[DrawTimer - i], 0).RotatedBy(Projectile.oldRot[i]), new Vector3(1 - (float)i / DrawTimer, 0.12f, 1), SlashColor * MathHelper.Lerp(1, 0, (float)i / DrawTimer)));
                }
                #region 顶点绘制配件
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                if (slash.Count >= 3)
                {
                    Main.graphics.GraphicsDevice.Textures[0] = SlashTexture.Value;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, slash.ToArray(), 0, slash.Count - 2);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                #endregion
            }
            #endregion
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Main.instance.DrawCacheProjsBehindProjectiles.Add(index);
        }
        public void DrawWarp()
        {
            if (WieldAttack) //状态机正在绘制
            {
                List<VertexInfo2> slash = new List<VertexInfo2>();
                int iTimer = DrawTimer < 39 ? DrawTimer + 1 : Projectile.oldPos.Length;
                for (int i = 0; i < iTimer; i++) //挥砍时长小于31时，挥砍时长小于oldpos记录长度，则oldpos和oldrot的最大值不能被记录，大于31就可以直接回溯最远的oldpos了
                {
                    Vector2 pos = Projectile.Center - new Vector2(ProjRadius, 0).RotatedBy(Projectile.rotation) - Main.screenPosition; //挥砍的中心都是以剑柄为中心
                    slash.Add(new VertexInfo2(pos + new Vector2(WieldDrawRadius[DrawTimer - i] * 2 - WieldDrawLessLength * Projectile.scale, 0).RotatedBy(Projectile.oldRot[i]), new Vector3(1 - (float)i / DrawTimer, 0, 1), SlashColor * MathHelper.Lerp(1, 0, (float)i / DrawTimer)));
                    slash.Add(new VertexInfo2(pos + new Vector2(WieldDrawRadius[DrawTimer - i], 0).RotatedBy(Projectile.oldRot[i]), new Vector3(1 - (float)i / DrawTimer, 0.12f, 1), SlashColor * MathHelper.Lerp(1, 0, (float)i / DrawTimer)));
                }
                #region 顶点绘制配件
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                if (slash.Count >= 3)
                {
                    Main.graphics.GraphicsDevice.Textures[1] = WarpTexture.Value;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, slash.ToArray(), 0, slash.Count - 2);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                #endregion
            }
        }
    }
    public class GlobalSwordNPC : GlobalNPC
    {
        public bool AllowAttack = false;
        public bool IsRecover = false;
        public Vector2 ArmOffset;
        public Vector2 HandOffset;
        public float ArmRotation;
        public float ArmLength;
        public int PerfectClashTimer = 0;
        public override bool InstancePerEntity => true;
    }
}