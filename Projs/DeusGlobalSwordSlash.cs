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
using ReLogic.Content;
using DeusMod.Core.Systems;
using DeusMod.Helpers;
using Terraria.Audio;
using DeusMod.Projs.NPCs;
using DeusMod.Core;
using Terraria.UI;
using ReLogic.Graphics;
using ParticleLibrary.Core;
using DeusMod.Particles;

namespace DeusMod.Projs
{
    public class ChargeEffect
    {
        public static Effect ChargeBorder = ModContent.Request<Effect>("DeusMod/Effects/Border", AssetRequestMode.ImmediateLoad).Value;
    }
    public abstract class DeusGlobalSwordSlash : ProjStateMachine, IDrawWarp
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
        public Texture2D SwordTexture; //全局变量——绘制武器贴图时，玩家所持有武器的材质，也决定弹幕本体的大小
        public bool SwordOwnTexture = false; //全局变量——绘制武器贴图时是直接使用武器贴图还是使用自己的贴图
        public int ShouldHeldItem; //全局变量——玩家拿着武器时生成的弹幕——因为切换武器时切换弹幕，因此需要判断是否是同一个武器
        public bool DrawInverse = false; //全局变量——绘制武器贴图时是否反向绘制
        public float ProjRadius; //全局变量——弹幕本体从左下到右上计算得出的半径
        public Vector2 SwordArmPosAdd; //全局变量——剑本体的“尾端”和玩家手臂的相对位置
        public float ArmRotation; //全局变量——玩家手臂的角度
        public float DrawArmRotation; //全局变量——玩家手臂的绘制角度
        public bool ShouldDrawArm; //全局变量——玩家手臂绘制角度是否变化
        public bool DrawBehindPlayer = false; //全局变量——剑是否绘制在玩家图层后
        public bool IsShortSword = false; //全局变量——是否是短剑，影响拿剑方式
        public float DamageScale = 1; //伤害倍率——武器伤害的倍率，在武器基础攻击上乘算伤害
        public bool Clashable = false; //全局变量——是否可以拼刀
        public bool PerfectClashable = false; //全局变量——是否可以完美拼刀
        public bool ChargeShader = false; //全局变量——是否绘制蓄力时的描边shader
        public int ShootProj = 0; //全局变量——射出的射弹
        public float ShootProjDamScale = 1; //全局变量——射出射弹的伤害倍率
        public bool ApplyStuck = false; //全局变量——攻击是否卡肉
        public bool ApplySlashDust = false; //全局变量——攻击是否产生剑形尘埃
        public bool AttackHit = false; //全局变量——攻击是否命中，用于判定单次攻击命中之内只生效一次的攻击
        public float SwordPowerAdd = 0; //全局变量——攻击补充的剑意
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
        public float TimeMax; //最大执行时长
        public float StopTime; //停止攻击动作的时间（特指射弹的位置变化，攻击状态本身不结束）
        public bool ShouldCountMouse = true; //动作状态——是否考虑鼠标和人的向量角度
        public Vector2 MousePos; //动作状态——鼠标的位置
        public bool CouldHit = false; //射弹是否可以攻击
        #endregion
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40; //最多记录40帧前的弹幕位置
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.hide = true;
            Projectile.GetGlobalProjectile<DeusGlobalProjs>().ClashType = 2;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            DeusPlayer DeusPlayer = Main.LocalPlayer.GetModPlayer<DeusPlayer>();
            if (source is EntitySource_ItemUse Use) //记录射弹生成时所拿的武器
            {
                ShouldHeldItem = Use.Item.type;
            }
            DeusPlayer.SwordPower = 0; //清空剑意值，剑意值上限在RegisterVariables里定义
        }
        public float SwordLogisticHelper(float time, float maxtime) //求出剑类的特有前期快速增长后期平滑逻辑斯蒂模型，数据分别为当前时间和最大时间
        {
            return DeusModMathHelper.LogisticHelper(3.14f, 1.57f, -0.23f / 4, 1.57f, time * (80 / maxtime)) / DeusModMathHelper.LogisticHelper(3.14f, 1.57f, -0.23f / 4, 1.57f, 80);
        }
        //将射弹的三项数据进行平滑移动，因此不适合有函数变化的位移
        //将射弹位置变为target位置
        public void PosSetTarget(Projectile proj, float timer, bool shortestrot = false, bool shortestarmrot = false)
        {
            DeusGlobalSwordSlash projmod = (DeusGlobalSwordSlash)proj.ModProjectile;
            Player player = Main.player[proj.owner];
            projmod.SwordArmPosAdd = new Vector2(MathHelper.Lerp(projmod.IniSet.SwordArmAdd.X, projmod.TargetSet.SwordArmAdd.X, timer), MathHelper.Lerp(projmod.IniSet.SwordArmAdd.Y, projmod.TargetSet.SwordArmAdd.Y, timer));
            proj.scale = MathHelper.Lerp(projmod.IniSet.Scale, projmod.TargetSet.Scale, timer);
            projmod.ArmRotation = MathHelper.Lerp(projmod.IniSet.ArmRot, projmod.TargetSet.ArmRot, timer);
            if(shortestrot)
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
            DeusGlobalSwordSlash projmod = (DeusGlobalSwordSlash)proj.ModProjectile;
            proj.rotation = projmod.IniSet.Rot + (projmod.TargetSet.Rot - projmod.IniSet.Rot) * SwordLogisticHelper(time, maxtime);
        }
        //先于一切状态机执行的AI
        //初始化数据和各类state
        public abstract void RegisterVariables();
        public abstract void Appear();
        public override void Initialize()
        {
            RegisterVariables();
            Projectile.rotation = -(float)Math.PI / 2;
            Appear();
            RegisterState(new AppearAnim());
            RegisterState(new Idle());
            RegisterState(new SpecialIdle());
            RegisterState(new Wield());
            RegisterState(new Stinger());
            RegisterState(new Clash());
            RegisterState(new Recover());
        }
        public override void AIBefore()
        {
            Player player = Main.player[Projectile.owner];
            DeusPlayer DeusPlayer = Main.LocalPlayer.GetModPlayer<DeusPlayer>();
            SwordTexture = ModContent.Request<Texture2D>(Texture).Value;
            if (player.HeldItem.type != ShouldHeldItem) Projectile.Kill();
            else Projectile.timeLeft = 10; //如果玩家换了手持武器则杀射弹，否则射弹一直保持
            Projectile.width = (int)(Projectile.scale * SwordTexture.Width);
            Projectile.height = (int)(Projectile.scale * SwordTexture.Height); //射弹长度 = 材质长度*射弹大小
            ProjRadius = DeusModMathHelper.PythagoreanHelper(Projectile.width, Projectile.height) / 2; //勾股算半径
            DrawBehindPlayer = false;
            Clashable = PerfectClashable = false;
            Projectile.damage = (int)(player.HeldItem.damage * DamageScale * (1 + DeusPlayer.SwordPower / 2f)); //同步攻击+剑意伤害加成(1+剑意*0.5的倍率)
            Projectile.CritChance = player.HeldItem.crit; //同步暴击
            Projectile.knockBack = player.HeldItem.knockBack; //同步击退
            Projectile.localNPCHitCooldown = (int)(player.HeldItem.useTime / player.GetAttackSpeed(DamageClass.Melee)) * 4;
        }
        //近战武器出场动画
        public class AppearAnim : ProjState
        {
            int dir = 1;
            public override void AI(ProjStateMachine projectile)
            {
                #region 变量简记
                Projectile proj = projectile.Projectile;
                DeusGlobalSwordSlash projmod = (DeusGlobalSwordSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner];
                #endregion
                #region 此AI数据
                if (projmod.Timer == 0) dir = player.direction; //将玩家方位固定在拿起剑时的方位
                player.direction = dir;
                if (dir == -1) projmod.DrawInverse = true;
                else projmod.DrawInverse = false;
                projmod.ShouldDrawArm = true; //自定义手臂角度
                projmod.DrawBehindPlayer = false; //让剑在玩家前绘制
                projmod.TimeMax = 300; //时间记录
                projmod.Timer++;
                //记录此AI结束时切换的目标位置
                if (!projmod.IsShortSword)
                    projmod.TargetSet.Set(new Vector2(-8 * dir, -16), 0.5f * (float)Math.PI, dir * (float)Math.PI, 1.2f);
                else projmod.TargetSet.Set(new Vector2(-4 * dir, -4), 0.5f * (float)Math.PI - 1.2f * dir, 0, 1.2f);
                #endregion
                #region 剑出现
                if (projmod.Timer <= 80) //20帧
                {
                    if (projmod.Timer <= 40)
                        proj.scale = MathHelper.Lerp(0, 1.6f, projmod.Timer / 40f); //在前10帧内让剑从小大出现
                    proj.rotation = -0.5f * (float)Math.PI; //在此期间一直旋转-90度使得剑头冲上
                    projmod.DrawArmRotation = projmod.ArmRotation = proj.rotation - (float)Math.PI / 2; //手臂角度，用于将剑放在玩家手上
                    projmod.SwordArmPosAdd = Vector2.Zero; //剑的尾部一直保持在玩家手中（这样才能拿起来嘛）
                }
                #endregion
                #region 挥剑
                else if (projmod.Timer <= 240) //前60帧
                {
                    #region 音效
                    if (projmod.Timer == 81)
                        SoundEngine.PlaySound(DeusModSoundHelper.Sword_Wield, player.Center);
                    #endregion
                    #region 只有前50帧在挥剑
                    if (projmod.Timer <= 200) //前50帧
                    {
                        proj.rotation = -0.5f * (float)Math.PI + dir * ((float)Math.PI * 5 / 6 + 0.5f * (float)Math.PI) * projmod.SwordLogisticHelper(projmod.Timer - 80, 80);
                        projmod.WieldAttack = true;
                        //projmod.WieldTimer = projmod.Timer - 80;
                        projmod.DrawTimer = projmod.Timer - 80;
                        projmod.WieldDrawRadius[projmod.DrawTimer] = projmod.ProjRadius;
                    }
                    #endregion
                    #region 记录最终位置
                    if (projmod.Timer == 240)
                        projmod.IniSet.Set(Vector2.Zero, proj.rotation, projmod.ArmRotation, proj.scale);
                    #endregion
                    projmod.DrawArmRotation = projmod.ArmRotation = proj.rotation - (float)Math.PI / 2; //手臂角度，用于将剑放在玩家手上
                    projmod.SwordArmPosAdd = Vector2.Zero; //剑的尾部一直保持在玩家手中（这样才能拿起来嘛）
                }
                #endregion
                #region 收剑，手放回原位
                else
                {
                    projmod.WieldAttack= false;
                    #region 位置变化
                    if (!projmod.IsShortSword)
                    {
                        if (projmod.Timer <= 270)
                        {
                            projmod.PosSetTarget(proj, (projmod.Timer - 240f) / 30f, true);
                            projmod.DrawArmRotation = player.direction * MathHelper.Lerp((float)Math.PI / 3, (float)Math.PI, (projmod.Timer - 240f) / 30f);
                        }
                        else
                            projmod.DrawArmRotation = player.direction * MathHelper.Lerp((float)Math.PI, (float)Math.PI * 2, (projmod.Timer - 270f) / 30f);
                    }
                    else
                    {
                        projmod.PosSetTarget(proj, (projmod.Timer - 240f) / 60f, true);
                        projmod.DrawArmRotation = projmod.ArmRotation;
                    }
                    #endregion
                    #region 状态机变化
                    if (projmod.Timer == projmod.TimeMax) //时间到结束状态
                    {
                        projmod.Timer = 0;
                        projmod.SetState<Idle>();
                        return;
                    }
                    #endregion
                }
                #endregion
            }
        }
        //状态：背剑
        public class Idle : ProjState
        {
            public override void AI(ProjStateMachine projectile)
            {
                #region 变量简记
                Projectile proj = projectile.Projectile;
                DeusGlobalSwordSlash projmod = (DeusGlobalSwordSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner];
                #endregion
                #region 此AI数据
                #endregion
                #region 如果不是短剑就背在身后
                if (!projmod.IsShortSword)
                {
                    #region 和玩家方向无关的变量——角度，大小，是否绘制剑身
                    proj.rotation = 0.5f * (float)Math.PI; //固定的旋转90度使剑头向下
                    proj.scale = 1.2f; //大小倍率固定1.6倍
                    projmod.Draw_SwordShouldDraw = true; //固定绘制剑身
                    projmod.CouldHit = false;
                    projmod.ShouldDrawArm = false; //手臂自然摆动
                    projmod.DrawBehindPlayer = true; //剑绘制在玩家图层后
                    #endregion
                    #region 设置和方向有关的变量——是否反转，中心
                    if (player.direction == -1) projmod.DrawInverse = true;
                    else projmod.DrawInverse = false;
                    projmod.SwordArmPosAdd = new Vector2(-8 * player.direction, -16);
                    projmod.DrawArmRotation = 0;
                    #endregion
                }
                #endregion
                #region 如果是短剑就拿在手上
                else
                {
                    #region 和玩家方向无关的变量——角度，大小，是否绘制剑身
                    proj.scale = 1.2f; //大小倍率固定1.2倍
                    projmod.Draw_SwordShouldDraw = true; //固定绘制剑身
                    projmod.CouldHit = false;
                    projmod.ShouldDrawArm = true; //手臂保持下垂
                    projmod.DrawBehindPlayer = false; //剑绘制在玩家图层前
                    #endregion
                    #region 设置和方向有关的变量——是否反转，中心
                    proj.rotation = 0.5f * (float)Math.PI - 1.2f * player.direction; //固定的旋转大约15度
                    if (player.direction == -1) projmod.DrawInverse = true;
                    else projmod.DrawInverse = false;
                    projmod.SwordArmPosAdd = new Vector2(-4 * player.direction, -4);
                    projmod.DrawArmRotation = 0;
                    #endregion
                }
                #endregion
                #region 随机出现的特殊待机动作
                if (projmod.Timer <= 2400)
                    projmod.Timer += 1;
                else if (Main.rand.NextBool(14400))
                {
                    projmod.Timer = 0;
                    projmod.TimeMax = 900;
                    projmod.SetState<SpecialIdle>();
                }
                #endregion
            }
        }
        //状态：待机特殊动画，在idle下有几率执行此ai
        public class SpecialIdle : ProjState
        {
            int dir = 1;
            public override void AI(ProjStateMachine projectile)
            {
                #region 变量简记
                Projectile proj = projectile.Projectile;
                DeusGlobalSwordSlash projmod = (DeusGlobalSwordSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner];
                #endregion
                #region 未计时时记录数据
                if (projmod.Timer == 0)
                {
                    projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale);
                    dir = player.direction;
                    float targrot = player.direction == 1 ? 3.4f : (float)Math.PI - 3.4f;
                    projmod.TargetSet.Rot = player.direction == 1 ? 3f : (float)Math.PI * 2 - 3f;
                    projmod.TargetSet.Set(Vector2.Zero, targrot, ((float)Math.PI + 1.4f) * player.direction, 1.6f);
                }
                #endregion
                #region 此AI数据
                player.direction = dir;
                projmod.Timer++;
                projmod.ShouldDrawArm = true; //自定义手臂角度
                #endregion
                if (projmod.Timer <= 140) //前35帧
                {
                    #region 非短剑需要先摸到剑
                    if (!projmod.IsShortSword)
                    {
                        #region 前20帧：手臂摸到剑上
                        if (projmod.Timer <= 80)
                        {
                            projmod.DrawBehindPlayer = true;
                            projmod.DrawArmRotation = player.direction * MathHelper.Lerp(0, (float)Math.PI, projmod.Timer / 80f);
                        }
                        #endregion
                        #region ~35帧：手臂把剑放到拿剑位置上
                        else
                        {
                            projmod.DrawBehindPlayer = true;
                            projmod.PosSetTarget(proj, (projmod.Timer - 80f) / 60f);
                            projmod.DrawArmRotation = projmod.ArmRotation;
                        }
                        #endregion
                    }
                    #endregion
                    #region 短剑不需要这一步
                    else
                    {
                        projmod.PosSetTarget(proj, projmod.Timer / 140f);
                        projmod.DrawArmRotation = player.direction * MathHelper.Lerp(0, (float)Math.PI, projmod.Timer / 140f);
                    }
                    #endregion
                }
                #region ~180帧：转剑，手臂略微上下摆动
                else if (projmod.Timer <= 720)
                {
                    #region 重新记录角度
                    if (projmod.Timer == 141)
                    {
                        projmod.IniSet.Rot = proj.rotation;
                        projmod.TargetSet.Rot = player.direction == 1 ? 2.9f : (float)Math.PI - 2.9f;
                    }
                    #endregion
                    #region ~50帧：往逆时针转一点剑
                    if (projmod.Timer <= 200)
                        projmod.RotSetTargetLogistic(proj, projmod.Timer - 140f, 60f);
                    #endregion
                    #region ~70帧：顺时针转剑起步
                    else if (projmod.Timer <= 280)
                        proj.rotation += 0.001f * (projmod.Timer - 200) * dir;
                    #endregion
                    #region ~180帧：顺时针匀速转剑
                    else
                    {
                        proj.rotation += 0.08f * dir;
                        if ((projmod.Timer - 280) % 60 == 0) SoundEngine.PlaySound(DeusModSoundHelper.Sword_Wield_Vanilla, player.Center);
                    }
                    #endregion
                    #region 手臂位置——70帧之后开始上下摆动
                    if (projmod.Timer <= 280)
                    {
                        projmod.DrawArmRotation = projmod.ArmRotation = player.direction * ((float)Math.PI + 1.4f);
                    }
                    else
                    {
                        projmod.DrawArmRotation = projmod.ArmRotation = player.direction * ((float)Math.PI + 1.4f + 0.2f * (float)Math.Sin((projmod.Timer - 280) / 16f));
                    }
                    #endregion
                }
                #endregion
                #region ~225帧：停止转剑，剑背回去
                else if (projmod.Timer <= 900)
                {
                    #region ~200帧：逐渐停止转剑
                    if (projmod.Timer <= 800)
                    {
                        proj.rotation += 0.001f * (800 - projmod.Timer) * dir;
                        projmod.DrawArmRotation = projmod.ArmRotation;
                        if (projmod.Timer == 800)
                        {
                            projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale);
                            if (!projmod.IsShortSword)
                                projmod.TargetSet.Set(new Vector2(-8 * dir, -16), 0.5f * (float)Math.PI, dir * (float)Math.PI, 1.2f);
                            else projmod.TargetSet.Set(new Vector2(-4 * dir, -4), 0.5f * (float)Math.PI - 1.2f * dir, 0, 1.2f);
                        }
                    }
                    #endregion
                    else
                    {
                        if (!projmod.IsShortSword)
                        {
                            #region ~215帧：把剑背回去
                            if (projmod.Timer <= 860)
                            {
                                projmod.PosSetTarget(proj, (projmod.Timer - 800f) / 60f, true);
                                projmod.DrawArmRotation = projmod.ArmRotation;
                            }
                            #endregion
                            #region ~225帧：手放回
                            else
                            {
                                projmod.DrawArmRotation = MathHelper.Lerp(projmod.DrawArmRotation, 0, (projmod.Timer - 860f) / 40f);
                            }
                            #endregion
                        }
                        else
                        {
                            //直接把剑放到位置
                            projmod.PosSetTarget(proj, (projmod.Timer - 800f) / 100f, true, true);
                            projmod.DrawArmRotation = projmod.ArmRotation;
                        }
                    }
                }
                #endregion
                #region 状态机转换
                else
                {
                    projmod.Timer = 0;
                    projmod.SetState<Idle>();
                }
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
        public int WieldStuckTimer = 0; //卡肉时间
        public int WieldStopTime; //停止动作的事件
        public float WieldHandleLength; //手握剑的握柄长度
        public float WieldDrawLessLength = 0; //顶点绘制距离剑尖的距离
        #endregion
        /*状态：挥剑
        挥剑由举剑和挥剑两者组成
        举剑时的目标角度为独有的HoldSet.Rot，需要用IniSet.Rot配合HoldSet.Rot求出该角度的倍率
        挥剑时的目标角度为targrot，需要用targrot配合mouserot求出该角度的倍率
         */
        //读入挥剑的变量
        public void WieldTrigger(bool shouldcountmouse, float standardscale, float thinscale, float holdrot, float targrot, float swordpoweradd, float handlelength = 0, float stoptime = 0, float damscale = 1f, int projtype = 0, float projdamscale = 1f)
        {
            Player player = Main.player[Projectile.owner];
            WieldDrawArmBefore = ShouldDrawArm;
            Timer = 0; //重置计时器
            IniSet.Set(SwordArmPosAdd, Projectile.rotation, ArmRotation, Projectile.scale); //获取iniset
            ShouldCountMouse = shouldcountmouse; //获取此次挥剑角度是否由鼠标角度决定，也即是否此时需要获取鼠标位置
            if(shouldcountmouse) MousePos = Main.MouseWorld - player.Center; //若需要，获取此时的鼠标位置
            WieldHoldRot = holdrot; //获取举剑的目标角度
            WieldFinalRot = targrot; //获取挥剑的目标角度
            DrawInverse = player.direction < 0 ? true : false; //改变射弹绘制方向
            if (targrot < holdrot) DrawInverse = !DrawInverse; //如果从下向上挥则改变射弹方向
            if (player.direction < 0) //如果玩家为负方向
            {
                WieldHoldRot = (float)Math.PI - WieldHoldRot; //改变两个角度使得角度变成正向
                WieldFinalRot = (float)Math.PI - WieldFinalRot;
            }
            float scl = DeusModMathHelper.EllipseRadiusHelper(standardscale, standardscale * thinscale, WieldHoldRot);
            if (shouldcountmouse) //加入鼠标角度的影响
            {
                WieldHoldRot += (float)Math.Atan(MousePos.Y / MousePos.X);
                WieldFinalRot += (float)Math.Atan(MousePos.Y / MousePos.X);
            }
            WieldStandardScale = standardscale; //解决半径问题
            WieldThinScale = thinscale;
            WieldHandleLength = handlelength;
            TargetSet.Set(new Vector2(-WieldHandleLength, 0).RotatedBy(WieldHoldRot), WieldHoldRot, WieldHoldRot - (float)Math.PI / 2f, scl);
            ((DeusGlobalSwordSlash)Projectile.ModProjectile).SetState<Wield>();
            DamageScale = damscale;
            ShootProj = projtype;
            ApplyStuck = true;
            ApplySlashDust = true;
            SwordPowerAdd = swordpoweradd;
        }
        private class Wield : ProjState
        {
            public override void AI(ProjStateMachine projectile)
            {
                #region 基础设置
                Projectile proj = projectile.Projectile;
                DeusGlobalSwordSlash projmod = (DeusGlobalSwordSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner]; //基础设置
                projmod.ShouldDrawArm = true; //自定义手臂角度
                player.itemAnimation = player.itemTime = 2; //使得玩家始终处于使用武器的状态
                projmod.TimeMax = (player.HeldItem.useTime / player.GetAttackSpeed(DamageClass.Melee)) * 4; //状态总时长
                int HoldupTimeMax = (int)(projmod.TimeMax / 3); //举剑的时长等于使用武器时间的1/3
                int WieldTimeMax = (int)projmod.TimeMax - HoldupTimeMax; //挥剑的时长等于使用武器时间的2/3
                if(projmod.WieldStuckTimer > 0) //如果卡肉
                    projmod.WieldStuckTimer--; //在计时器减少之前减少卡肉计时器
                else projmod.Timer++; //然后再减少射弹计时器
                #endregion
                #region 总时长的前1/3：举剑
                if (projmod.Timer <= HoldupTimeMax) //如果总时长小于举剑的时长
                {
                    projmod.WieldAttack = false;
                    if (!projmod.WieldDrawArmBefore)
                    {
                        if(projmod.Timer <= HoldupTimeMax / 3f)
                        {
                            projmod.DrawBehindPlayer = true;
                            projmod.DrawArmRotation = MathHelper.Lerp(0, (float)Math.PI, (float)(projmod.Timer / (HoldupTimeMax / 3f)));
                        }
                        else
                        {
                            projmod.PosSetTarget(proj, (float)(projmod.Timer - HoldupTimeMax / 3f) / (2f * HoldupTimeMax / 3f), true, true);
                            projmod.DrawArmRotation = projmod.ArmRotation;
                        }
                    }
                    else
                    {
                        projmod.PosSetTarget(proj, (float)projmod.Timer / HoldupTimeMax, true, true);
                        projmod.DrawArmRotation = projmod.ArmRotation;
                    }
                    if (projmod.Timer == HoldupTimeMax)
                    {
                        //播放音效
                        SoundStyle[] SwordSound = new SoundStyle[3] { DeusModSoundHelper.Sword_Wield, DeusModSoundHelper.Sword_Wield2, DeusModSoundHelper.Sword_Wield3 };
                        SoundEngine.PlaySound(SwordSound[Main.rand.Next(3)], player.Center);
                    }
                }
                #endregion
                #region 后2/3：挥剑
                else
                {
                    projmod.Clashable = true;
                    projmod.WieldAttack = true;
                    projmod.CouldHit = true;
                    int WieldTimer;
                    WieldTimer = projmod.DrawTimer = projmod.Timer - HoldupTimeMax;
                    #region 记录初始数据
                    if (WieldTimer == 1) //当时间到了刚开始计算挥剑的位置
                    {
                        projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //重新记录Ini类原始数据
                        #region 一部分需要计算得出的数据
                        //通过挥剑标准倍率、挥剑缩小倍率、挥剑初始角度、挥剑最终角度、挥剑角度是否考虑鼠标和鼠标位置等计算得出最终的倍率大小
                        int targscaleflag = projmod.ShouldCountMouse ? 1 : 0;
                        float targscale = DeusModMathHelper.EllipseRadiusHelper(projmod.WieldStandardScale, projmod.WieldStandardScale * projmod.WieldThinScale, projmod.WieldFinalRot - (float)Math.Atan(projmod.MousePos.Y / projmod.MousePos.X) * targscaleflag);
                        projmod.TargetSet.Set(Vector2.Zero, projmod.WieldFinalRot, projmod.WieldFinalRot - (float)Math.PI / 2, targscale);//重新记录targ数据
                        #endregion
                    }
                    #endregion
                    #region 角度、位置、大小变化，不是平滑变化
                    projmod.RotSetTargetLogistic(proj, projmod.Timer - HoldupTimeMax, (float)(projmod.TimeMax - HoldupTimeMax)); //角度变化
                    projmod.SwordArmPosAdd = new Vector2(-projmod.WieldHandleLength, 0).RotatedBy(proj.rotation);
                    proj.scale = DeusModMathHelper.EllipseRadiusHelper(projmod.WieldStandardScale, projmod.WieldStandardScale * projmod.WieldThinScale, projmod.Projectile.rotation - (projmod.ShouldCountMouse ? (float)Math.Atan(projmod.MousePos.Y / projmod.MousePos.X) : 0));
                    projmod.DrawArmRotation = projmod.ArmRotation = proj.rotation - (float)Math.PI / 2;
                    projmod.WieldDrawRadius[WieldTimer] = projmod.ProjRadius; //像oldpos一样记录绘制半径
                    #endregion
                    #region 挥剑到正中间位置（也就是挥剑的1/4时间点）时发射弹幕
                    if (projmod.ShootProj != 0 && projmod.Timer == HoldupTimeMax + WieldTimeMax / 4)
                    {
                        Projectile shootproj = Projectile.NewProjectileDirect(proj.GetSource_FromThis(), player.Center, Vector2.Zero, projmod.ShootProj, (int)(proj.damage * projmod.ShootProjDamScale), proj.knockBack, Main.myPlayer);
                        shootproj.direction = player.direction;
                    }
                    #endregion
                    #region 格挡射弹
                    //时间在最长帧的前后范围内，大约6帧
                    if (projmod.Timer >= -12 + HoldupTimeMax + WieldTimeMax / 4 && projmod.Timer <= 12 + HoldupTimeMax + WieldTimeMax / 4)
                    {
                        projmod.PerfectClashable = true;
                        foreach (Projectile counterproj in Main.projectile)
                        {
                            Rectangle projhitbox = new Rectangle((int)(proj.Center.X + new Vector2(projmod.ProjRadius, 0).RotatedBy(proj.rotation).X) - 10, (int)(proj.Center.Y + new Vector2(projmod.ProjRadius, 0).RotatedBy(proj.rotation).Y) - 10, 20, 20);
                            Rectangle counterhitbox = counterproj.Hitbox;
                            //敌对射弹、可以拼刀、重合
                            if (counterproj.hostile == true && counterproj.GetGlobalProjectile<DeusGlobalProjs>().ClashType <= 1 && projhitbox.Intersects(counterhitbox))
                            {
                                counterproj.Kill();
                                SoundEngine.PlaySound(DeusModSoundHelper.Sword_Clash_Normal, player.Center);
                            }
                        }
                    }
                    #endregion
                    #region 拼刀
                    //完美拼刀帧在最长帧的前后范围内，大约6帧，其它时间拼刀均为非完美拼刀
                    if (projmod.Timer >= -12 + HoldupTimeMax + WieldTimeMax / 4 && projmod.Timer <= 12 + HoldupTimeMax + WieldTimeMax / 4)
                        projmod.PerfectClashable = true;
                    foreach (Projectile counterproj in Main.projectile)
                    {
                        Rectangle projhitbox = proj.Hitbox;
                        Rectangle counterhitbox = counterproj.Hitbox;
                        //敌对剑、重合
                        if (counterproj.ModProjectile is DeusGlobalNPCSlash && projhitbox.Intersects(counterhitbox))
                        {
                            if ((counterproj.ModProjectile as DeusGlobalNPCSlash).Clashable)
                            {
                                Rectangle Intersect = Rectangle.Intersect(projhitbox, counterhitbox);
                                //尘埃
                                Dust dust = Dust.NewDustDirect(new Vector2(Intersect.Center.X, Intersect.Center.Y) - new Vector2(21, 5), 0, 0, ModContent.DustType<SwordSlash>());
                                dust.velocity = Vector2.Zero;
                                dust.scale = Main.rand.NextFloat(0.3f, 0.4f);
                                dust.rotation = proj.rotation + (float)Math.PI / 2;
                                if (projmod.PerfectClashable)
                                {
                                    SoundEngine.PlaySound(DeusModSoundHelper.Sword_Clash_Perfect, player.Center);
                                    (counterproj.ModProjectile as DeusGlobalNPCSlash).PerfectClash(); 
                                }
                                else
                                {
                                    SoundEngine.PlaySound(DeusModSoundHelper.Sword_Clash_Normal, player.Center);
                                    player.velocity.X -= 6 * player.direction; //玩家向后退
                                    projmod.Timer = 0; //复原计时器
                                    projmod.TimeMax = 120; //设置最大时间
                                    projmod.SetState<Clash>();
                                    (counterproj.ModProjectile as DeusGlobalNPCSlash).ImperfectClash();
                                }
                            }
                        }
                    }
                    #endregion
                    #region 状态机的处理
                    if (projmod.Timer >= projmod.TimeMax) //结束
                    {
                        projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //设置iniset
                        projmod.TargetSet.Set(new Vector2(-8 * player.direction, -16), 0.5f * (float)Math.PI, player.direction * (float)Math.PI, 1.2f);
                        projmod.Timer = 0; //复原计时器
                        projmod.TimeMax = 240; //设置最大时间
                        projmod.SetState<Recover>(); //设置AI
                        return;
                    }
                    #endregion
                }
                #endregion
            }
        }
        public bool StingerAttack = false; //刺剑攻击，触发刺剑的碰撞箱和顶点绘制方式
        public bool StingerAttackStopDraw = false; //刺剑攻击停止绘制的淡出
        public bool StingerDrawArmBefore;
        public Vector2 StingerStartPosAdd;
        public Vector2 StingerEndPosAdd;
        public void StingerTrigger(bool shouldcountmouse, float swordpoweradd, float damscale = 1f)
        {
            Player player = Main.player[Projectile.owner];
            StingerDrawArmBefore = ShouldDrawArm;
            Timer = 0;
            IniSet.Set(SwordArmPosAdd, Projectile.rotation, ArmRotation, Projectile.scale); //获取iniset
            MousePos = Main.MouseWorld - player.Center;
            float exrot = MousePos.X > 0 ? 0 : (float)Math.PI;
            if(shouldcountmouse)
            {
                StingerStartPosAdd = new Vector2(-ProjRadius / 2, 0).RotatedBy((float)Math.Atan(MousePos.Y / MousePos.X) + exrot);
                StingerEndPosAdd = new Vector2(0, 0).RotatedBy((float)Math.Atan(MousePos.Y / MousePos.X) + exrot);
                TargetSet.Set(StingerStartPosAdd, (float)Math.Atan(MousePos.Y / MousePos.X) + exrot, (float)Math.Atan(MousePos.Y / MousePos.X) + exrot - (float)Math.PI / 2, 1.6f);
            }
            else
            {
                StingerStartPosAdd = new Vector2(-ProjRadius / 2, 0).RotatedBy(exrot);
                StingerEndPosAdd = new Vector2(0, 0).RotatedBy(exrot);
                TargetSet.Set(StingerStartPosAdd, exrot, exrot - (float)Math.PI / 2, 1.6f);
            }
            ((DeusGlobalSwordSlash)Projectile.ModProjectile).SetState<Stinger>();
            ApplyStuck = true;
            ApplySlashDust = true;
            SwordPowerAdd = swordpoweradd;
            DamageScale = damscale;
        }
        private class Stinger : ProjState
        {
            public override void AI(ProjStateMachine projectile)
            {
                #region 基础设置
                Projectile proj = projectile.Projectile;
                DeusGlobalSwordSlash projmod = (DeusGlobalSwordSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner]; //基础设置
                player.itemAnimation = player.itemTime = 2; //使得玩家始终处于使用武器的状态
                projmod.TimeMax = (player.HeldItem.useTime / player.GetAttackSpeed(DamageClass.Melee)) * 4; //状态总时长
                int HoldupTimeMax = (int)(projmod.TimeMax / 2); //举剑的时长等于使用武器时间的1/2
                int StingerTimeMax = (int)(projmod.TimeMax / 4); //刺剑的时长等于使用武器时间的1/4
                int RecoverTimeMax = (int)projmod.TimeMax - HoldupTimeMax - StingerTimeMax; //收剑的时长等于使用武器时间的1/4
                projmod.Timer++; //射弹计时器
                #endregion
                #region 举剑
                if (projmod.Timer <= HoldupTimeMax)
                {
                    if (!projmod.StingerDrawArmBefore)
                    {
                        if (projmod.Timer <= HoldupTimeMax / 3f)
                        {
                            projmod.DrawBehindPlayer = true;
                            projmod.DrawArmRotation = MathHelper.Lerp(0, (float)Math.PI, (float)(projmod.Timer / (HoldupTimeMax / 3f)));
                        }
                        else
                        {
                            projmod.PosSetTarget(proj, (float)(projmod.Timer - HoldupTimeMax / 3f) / (2f * HoldupTimeMax / 3f), true, true);
                            projmod.DrawArmRotation = projmod.ArmRotation;
                        }
                    }
                    else
                    {
                        projmod.PosSetTarget(proj, (float)projmod.Timer / HoldupTimeMax, true, true);
                        projmod.DrawArmRotation = projmod.ArmRotation;
                    }
                    if (projmod.Timer == HoldupTimeMax)
                    {
                        //播放音效
                        SoundStyle[] SwordSound = new SoundStyle[3] { DeusModSoundHelper.Sword_Wield, DeusModSoundHelper.Sword_Wield2, DeusModSoundHelper.Sword_Wield3 };
                        SoundEngine.PlaySound(SwordSound[Main.rand.Next(3)], player.Center);
                    }
                }
                #endregion
                #region 刺剑
                else if (projmod.Timer <= HoldupTimeMax + StingerTimeMax)
                {
                    projmod.StingerAttack = true;
                    projmod.CouldHit = true;
                    projmod.DrawTimer = projmod.Timer - HoldupTimeMax;
                    projmod.DrawTimeMax = StingerTimeMax;
                    if (projmod.Timer == HoldupTimeMax + 1)
                    {
                        projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //获取iniset
                        projmod.TargetSet.Set(projmod.StingerEndPosAdd, proj.rotation, projmod.ArmRotation, proj.scale);
                    }
                    projmod.DrawArmRotation = projmod.ArmRotation;
                    projmod.PosSetTarget(proj, (projmod.Timer - HoldupTimeMax) / (float)StingerTimeMax);
                }
                #endregion
                #region 收剑
                else
                {
                    projmod.StingerAttack = false;
                    projmod.StingerAttackStopDraw = true;
                    projmod.DrawTimer = projmod.Timer - HoldupTimeMax - StingerTimeMax;
                    projmod.DrawTimeMax = RecoverTimeMax;
                    if (projmod.Timer == HoldupTimeMax + StingerTimeMax + 1)
                    {
                        projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //获取iniset
                        projmod.TargetSet.Set(projmod.StingerStartPosAdd, proj.rotation, projmod.ArmRotation, proj.scale);
                    }
                    projmod.DrawArmRotation = projmod.ArmRotation;
                    projmod.PosSetTarget(proj, (projmod.Timer - HoldupTimeMax - StingerTimeMax) / (float)RecoverTimeMax);
                }
                #endregion
                #region 状态机变化
                if (projmod.Timer > projmod.TimeMax) //结束
                {
                    projmod.StingerAttackStopDraw = false;
                    projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //设置iniset
                    if (!projmod.IsShortSword)
                        projmod.TargetSet.Set(new Vector2(-8 * player.direction, -16), 0.5f * (float)Math.PI, player.direction * (float)Math.PI, 1.2f);
                    else projmod.TargetSet.Set(new Vector2(-4 * player.direction, -4), 0.5f * (float)Math.PI - 1.2f * player.direction, 0, 1.2f);
                    projmod.Timer = 0; //复原计时器
                    projmod.TimeMax = 240; //设置最大时间
                    projmod.SetState<Recover>(); //设置AI
                    return;
                }
                #endregion
            }
        }
        //不完美拼刀，剑向后，无攻击判定
        public class Clash : ProjState
        {
            public override void AI(ProjStateMachine projectile)
            {
                Projectile proj = projectile.Projectile;
                DeusGlobalSwordSlash projmod = (DeusGlobalSwordSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner]; //基础设置
                player.itemAnimation = player.itemTime = 2; //使得玩家始终处于使用武器的状态
                projmod.Clashable = false;
                projmod.WieldAttack = false;
                projmod.CouldHit = false;
                projmod.Timer++;
                projmod.ShouldDrawArm = true;
                float addrot = !projmod.DrawInverse ? MathHelper.Lerp(0.04f, 0, projmod.Timer / 120f) : -MathHelper.Lerp(0.04f, 0, projmod.Timer / 120f);
                proj.rotation -= addrot;
                projmod.SwordArmPosAdd = new Vector2(-projmod.WieldHandleLength, 0).RotatedBy(proj.rotation);
                projmod.DrawArmRotation = projmod.ArmRotation -= addrot;
                if (projmod.Timer >= projmod.TimeMax)
                {
                    projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //设置iniset
                    projmod.TargetSet.Set(new Vector2(-8 * player.direction, -16), 0.5f * (float)Math.PI, player.direction * (float)Math.PI, 1.2f);
                    projmod.Timer = 0; //复原计时器
                    projmod.TimeMax = 240; //设置最大时间
                    projmod.SetState<Recover>(); //设置AI
                }
            }
        }
        /*状态：恢复
        因为一般而言连招结束/攻击结束后不会立刻把剑背回去
        所以现在会握着剑握1s
        1s之内不攻击则背回去
        */
        public class Recover : ProjState
        {
            public override void AI(ProjStateMachine projectile)
            {
                Projectile proj = projectile.Projectile;
                DeusGlobalSwordSlash projmod = (DeusGlobalSwordSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner];
                projmod.ShouldDrawArm = true;
                projmod.WieldAttack = false;
                projmod.StingerAttack = false;
                projmod.StingerAttackStopDraw = false;
                projmod.ChargeShader = false;
                projmod.CouldHit = false;
                projmod.DamageScale = 1f;
                projmod.ShootProjDamScale = 1f;
                projmod.ApplyStuck = false;
                projmod.ApplySlashDust = false;
                projmod.AttackHit = false;
                projmod.SwordPowerAdd = 0;
                projmod.Timer++;
                #region 状态机的处理
                if (!projmod.IsShortSword)
                {
                    if (projmod.Timer > projmod.TimeMax / 5 * 3 && projmod.Timer <= projmod.TimeMax / 5 * 4)
                    {
                        projmod.PosSetTarget(proj, (projmod.Timer - projmod.TimeMax / 5 * 3) / (projmod.TimeMax / 5f), true, true);
                        projmod.DrawArmRotation = projmod.ArmRotation;
                    }
                    else if (projmod.Timer > projmod.TimeMax / 5 * 4)
                    {
                        projmod.DrawArmRotation = player.direction * MathHelper.Lerp((float)Math.PI, (float)Math.PI * 2, (projmod.Timer - projmod.TimeMax / 5 * 4) / (projmod.TimeMax / 5f));
                    }
                }
                else
                {
                    if (projmod.Timer > projmod.TimeMax / 5 * 3)
                    projmod.PosSetTarget(proj, (projmod.Timer - (projmod.TimeMax / 5 * 3)) / (projmod.TimeMax / 5 * 2), true);
                    projmod.DrawArmRotation = projmod.ArmRotation;
                }
                if(projmod.Timer > projmod.TimeMax) //时间到，结束状态
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
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center + new Vector2(-4 * player.direction, -2) + new Vector2(0, 12).RotatedBy(ArmRotation) + SwordArmPosAdd + new Vector2(ProjRadius, 0).RotatedBy(Projectile.rotation); //表现为当剑和手臂的距离为0时剑刚好拿在手上
            if(ShouldDrawArm) player.SetCompositeArmFront(true, 0, DrawArmRotation - player.fullRotation);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) //弹幕的碰撞箱调整，只有武器绘制的弹幕拥有碰撞箱
        {
            float point = 0f;
            if (CouldHit)
            {
                if(StingerAttack) //刺剑有独特的攻击判定
                    return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + new Vector2(-ProjRadius, 0).RotatedBy(Projectile.rotation), Projectile.Center + new Vector2(ProjRadius * MathHelper.Lerp(1f, 3f, (Timer - TimeMax / 2f) / (TimeMax / 4f)), 0).RotatedBy(Projectile.rotation), 8, ref point);
                else return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + new Vector2(-ProjRadius, 0).RotatedBy(Projectile.rotation), Projectile.Center + new Vector2(ProjRadius, 0).RotatedBy(Projectile.rotation), 8, ref point);
            }//前两个参数没必要动，第三个是头，第四个是尾，第五个是宽度，第六个别动，这样从左下角到右上角规定哪里有碰撞伤害
            else return false;
        }
        public override bool? CanCutTiles() //弹幕是否能够破坏物块（如草皮和罐子）
        {
            return CouldHit;
        }
        #region 命中特效变量
        public bool IsMegaShock; //命中特效——攻击的屏幕震动是否是大的震动
        public int SwordDebuff1; //命中特效——对敌人释放的debuff
        public int SwordDebuff1time; //命中特效——debuff1的持续时间，以秒计算
        public int SwordDebuff1rate; //命中特效——debuff1的释放几率，是1分之几这个分数的分母
        public int SwordDebuff2; //命中特效——debuff2
        public int SwordDebuff2time; //命中特效——debuff2的时间
        public int SwordDebuff2rate; //命中特效——debuff2的几率
        public int SwordDust1; //命中特效——尘埃1，在目标上以血式溅出
        public int SwordDust2; //命中特效——尘埃2，在目标上以孢子式飞出
        public int SwordDust3; //命中特效——尘埃3，在剑周围逸散
        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            #region 卡肉
            if (ApplyStuck) WieldStuckTimer = 8; //重设卡肉计时器，3帧足矣
            #endregion
            #region 击退和屏幕抖动
            Player player = Main.player[Projectile.owner];
            Vector2 dis = target.position - Projectile.Center;
            int length = (IsMegaShock) ? 48 : 24;
            player.GetModPlayer<ScreenShake>().ScreenShakeShort(length, (float)Math.Atan(dis.Y / dis.X));
            Projectile.velocity.X = player.direction == 1 ? 1 : -1; //调整方向以处理击退方向
            #endregion
            #region 剑意值改动
            if (!AttackHit)
            {
                player.GetModPlayer<DeusPlayer>().SwordPower += SwordPowerAdd; //先更改
                //再调整范围使其不会大于最大值或小于0
                if (player.GetModPlayer<DeusPlayer>().SwordPower > player.GetModPlayer<DeusPlayer>().SwordPowerMax)
                    player.GetModPlayer<DeusPlayer>().SwordPower = player.GetModPlayer<DeusPlayer>().SwordPowerMax;
                if (player.GetModPlayer<DeusPlayer>().SwordPower < 0)
                    player.GetModPlayer<DeusPlayer>().SwordPower = 0;
                player.GetModPlayer<DeusPlayer>().SwordPower = (float)Math.Round(player.GetModPlayer<DeusPlayer>().SwordPower, 1);
            }
            #endregion
            #region 施加debuff
            if (SwordDebuff1 != 0) //当debuff被赋值的时候攻击造成debuff
            {
                if (Main.rand.NextBool(SwordDebuff1rate))
                    target.AddBuff(SwordDebuff1, SwordDebuff1time * 60);
            }
            if (SwordDebuff2 != 0)
            {
                if (Main.rand.NextBool(SwordDebuff2rate))
                    target.AddBuff(SwordDebuff2, SwordDebuff2time * 60);
            }
            #endregion
            #region 命中切割特效
            if(ApplySlashDust)
            {
                for (int i = 0; i < 3; i++)
                {
                    float Rot = Main.rand.NextFloat(-0.5f, 0.5f);
                    float Vel = Main.rand.NextFloat(1.5f, 3f);
                    Vector2 direction = target.DirectionFrom(player.Center);
                    Vector2 position = target.Center - direction * 10;
                    ParticleSystem.NewParticle(position, direction.RotatedBy(Rot) * Vel * 12, new Slash(), Color.LightGoldenrodYellow, 0.8f);
                }

                //Rectangle Intersect = Rectangle.Intersect(Projectile.Hitbox, target.Hitbox);
                //Dust dust = Dust.NewDustPerfect(new Vector2(Intersect.Center.X - 36 * player.direction, Intersect.Center.Y - 18 - 18 * player.direction), ModContent.DustType<SwordSlash>(), Vector2.Zero, 0, Color.LightGoldenrodYellow, Main.rand.NextFloat(0.6f, 0.8f));
                //dust.rotation = player.direction == 1 ? 0.5f : (float)Math.PI - 0.5f;
            }
            //Dust.NewDustPerfect(Projectile.Bottom + Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(15), ModContent.DustType<GlowDust>(), Vector2.Zero, 0, new Color(50, 50, 255), 0.4f).fadeIn = 10;
            #endregion
            #region 命中产生尘埃
            if (SwordDust1 != 0) //Dust1受重力影响
            {
                int num1 = Main.rand.Next(4, 7);
                for (int i = 0; i < num1; i++)
                {
                    Dust dust2 = Dust.NewDustDirect(target.position, 0, 0, SwordDust1);
                    if (player.position.X < target.position.X)
                        dust2.velocity = new Vector2(Main.rand.NextFloat(3.6f, 7.2f), Main.rand.NextFloat(-2.4f, 3.6f));
                    else dust2.velocity = new Vector2(Main.rand.NextFloat(-7.2f, -3.6f), Main.rand.NextFloat(-2.4f, 3.6f));
                    dust2.scale = Main.rand.NextFloat(0.8f, 1f);
                    dust2.noGravity = false;
                }
            }
            if (SwordDust2 != 0) //Dust2自由逸散
            {
                int num2 = Main.rand.Next(4, 7);
                for (int i = 0; i < num2; i++)
                {
                    Dust dust2 = Dust.NewDustDirect(target.position, 0, 0, SwordDust2);
                    dust2.velocity = new Vector2(Main.rand.NextFloat(-3.6f, 3.6f), Main.rand.NextFloat(-3.6f, 3.6f));
                    dust2.scale = Main.rand.NextFloat(0.8f, 1.2f);
                    dust2.noGravity = true;
                }
            }
            #endregion
            #region 攻击判定
            if (!AttackHit) AttackHit = true;
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
            Player player = Main.player[Projectile.owner];
            //如果剑本体可以被绘制
            if (Draw_SwordShouldDraw)
            {
                #region 剑绘制
                if(ChargeShader)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                    //使用shader来进行蓄力判定
                    float alphacalc = 1;
                    if (Timer % 64 > 0 && Timer % 64 <= 16) alphacalc = 0.25f;
                    if ((Timer % 64 > 16 && Timer % 64 <= 32) || (Timer % 64 > 48 && Timer % 64 <= 63) || Timer % 64 == 0) alphacalc = 0.5f;
                    ChargeEffect.ChargeBorder.Parameters["uImageSize"].SetValue(SwordTexture.Size() * Projectile.scale);
                    ChargeEffect.ChargeBorder.Parameters["alp"].SetValue(alphacalc);
                    ChargeEffect.ChargeBorder.CurrentTechnique.Passes[0].Apply();
                }
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
                    slash.Add(new VertexInfo2(pos + new Vector2(WieldDrawRadius[DrawTimer - i] * 2 - (WieldDrawLessLength * Projectile.scale), 0).RotatedBy(Projectile.oldRot[i]), new Vector3(1 - (float)i / DrawTimer, 0, 1), SlashColor * MathHelper.Lerp(1, 0, (float)i / iTimer)));
                    slash.Add(new VertexInfo2(pos + new Vector2(WieldDrawRadius[DrawTimer - i], 0).RotatedBy(Projectile.oldRot[i]), new Vector3(1 - (float)i / DrawTimer, 0.12f, 1), SlashColor * MathHelper.Lerp(1, 0, (float)i / iTimer)));
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
            #region 刺剑状态的顶点绘制
            //slash存储的所有坐标都是在屏幕上的坐标
            //上面的add是绘制的上顶点，下面的add是绘制的下顶点
            if (StingerAttack) //状态机正在绘制
            {
                List<VertexInfo2> slash = new List<VertexInfo2>();
                for (int i = 0; i < 5; i++)
                {
                    float width = 16;
                    switch (i)
                    {
                        case 0: width = 12; break;
                        case 1: width = 12 + 4 / 3; break;
                        case 2: width = 12 + 8 / 3; break;
                        case 3: width = 16; break;
                        case 4: width = 0; break;
                    }
                    width *= MathHelper.Lerp(1, 0.5f, (float)DrawTimer / DrawTimeMax);
                    Vector2 pos = Projectile.Center - Main.screenPosition - new Vector2(ProjRadius, 0).RotatedBy(Projectile.rotation);
                    slash.Add(new VertexInfo2(pos + new Vector2(ProjRadius * i * MathHelper.Lerp(0, 1, (float)DrawTimer / DrawTimeMax), -width).RotatedBy(Projectile.rotation), new Vector3(i / 4f, 0.9f, 1), SlashColor));
                    slash.Add(new VertexInfo2(pos + new Vector2(ProjRadius * i * MathHelper.Lerp(0, 1, (float)DrawTimer / DrawTimeMax), width).RotatedBy(Projectile.rotation), new Vector3(i / 4f, 1, 1), SlashColor));
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
            if(StingerAttackStopDraw)
            {
                List<VertexInfo2> slash = new List<VertexInfo2>();
                for (int i = 0; i < 5; i++)
                {
                    float width = 16;
                    switch (i)
                    {
                        case 0: width = 12; break;
                        case 1: width = 12 + 4 / 3; break;
                        case 2: width = 12 + 8 / 3; break;
                        case 3: width = 16; break;
                        case 4: width = 0; break;
                    }
                    width *= 0.5f;
                    Vector2 pos = Projectile.Center - Main.screenPosition - new Vector2(ProjRadius, 0).RotatedBy(Projectile.rotation);
                    slash.Add(new VertexInfo2(pos + new Vector2(ProjRadius * i * MathHelper.Lerp(1, 0, (float)DrawTimer / DrawTimeMax), -width).RotatedBy(Projectile.rotation), new Vector3(i / 4f, 0.9f, 1), SlashColor));
                    slash.Add(new VertexInfo2(pos + new Vector2(ProjRadius * i * MathHelper.Lerp(1, 0, (float)DrawTimer / DrawTimeMax), width).RotatedBy(Projectile.rotation), new Vector3(i / 4f, 1, 1), SlashColor));
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
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();//先要关闭画布
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if(DrawBehindPlayer)
                Main.instance.DrawCacheProjsBehindProjectiles.Add(index);
            else Main.instance.DrawCacheProjsOverPlayers.Add(index);
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.GetModPlayer<DeusPlayer>().SwordPowerMax = 0;
        }
        public void DrawWarp()
        {
            Player player = Main.player[Projectile.owner];
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
    
    public class SwordPowerUI : SmartUIState
    {
        public static bool visible = false;
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        public override bool Visible => visible;
        public static SwordPowerBar SwordPowerBar = new SwordPowerBar();
        public override void OnInitialize()
        {
            SwordPowerBar.Width.Set(36, 0f);
            SwordPowerBar.Height.Set(14, 0f);
            SwordPowerBar.Left.Set(Main.screenWidth / 2, 0f);
            SwordPowerBar.Top.Set(Main.screenWidth / 4 - 16, 0f);
            Append(SwordPowerBar);
            base.OnInitialize();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

    }
    public class SwordPowerBar : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 center = new Vector2(GetDimensions().X, GetDimensions().Y);
            DeusPlayer DeusPlayer = Main.LocalPlayer.GetModPlayer<DeusPlayer>();
            Texture2D backTex = ModContent.Request<Texture2D>("DeusMod/Assets/UI/SwordPowerBackgroundUI").Value;
            Texture2D barTex = ModContent.Request<Texture2D>("DeusMod/Assets/UI/SwordPowerUI").Value;

            if (DeusPlayer.SwordPowerMax > 0)
            {
                //绘制背景
                spriteBatch.Draw(backTex, center, backTex.Frame(), Color.White, 0f, new Vector2(backTex.Width / 2, backTex.Height / 2), 1f, SpriteEffects.None, 0f);

                float chargePercent = DeusPlayer.SwordPower / DeusPlayer.SwordPowerMax;
                Rectangle barSource = new Rectangle(0, 0, (int)(chargePercent * barTex.Width), barTex.Height);
                spriteBatch.Draw(barTex, center, barSource, Color.White, 0f, new Vector2(barTex.Width / 2, barTex.Height / 2), 1f, SpriteEffects.None, 0f);

                //绘制文本
                spriteBatch.DrawString(FontAssets.MouseText.Value, DeusPlayer.SwordPower + "/" + DeusPlayer.SwordPowerMax, center + new Vector2(-8, -24), Color.White);
            }
        }
    }

    //突刺会释放的远程剑气
    public class GlobalSwordLight : ModProjectile
    {
        public Color color = Color.White;
        public override string Texture => "DeusMod/Effects/Textures/Slash";
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 16;
            Projectile.timeLeft = 15;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
        }
        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan(Projectile.velocity.Y / Projectile.velocity.X);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo2> slash = new List<VertexInfo2>();
            for (int i = 0; i < 4; i++)
            {
                int width = 8;
                switch (i)
                {
                    case 0: width = 6; break;
                    case 1: width = 7; break;
                    case 2: width = 8; break;
                    case 3: width = 0; break;
                }
                Vector2 pos = Projectile.Center - Main.screenPosition;
                slash.Add(new VertexInfo2(pos + new Vector2(-Projectile.width + Projectile.width / 2f * i, -width).RotatedBy(Projectile.rotation), new Vector3(i / 3f, 0.9f, 1), color));
                slash.Add(new VertexInfo2(pos + new Vector2(-Projectile.width + Projectile.width / 2f * i, width).RotatedBy(Projectile.rotation), new Vector3(i / 3f, 1, 1), color));
            }
            #region 顶点绘制配件
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (slash.Count >= 3)
            {
                Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(Texture).Value;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, slash.ToArray(), 0, slash.Count - 2);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            #endregion
            return false;
        }
    }
    public class GlowDustSword : ModDust
    {
        public override string Texture => "DeusMod/Assets/Dusts/GlowDust";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 64, 64);
            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(DeusMod.Instance.Assets.Request<Effect>("Effects/GlowDust", AssetRequestMode.ImmediateLoad).Value), "GlowingDust");
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            dust.scale *= 0.95f;
            dust.position += dust.velocity;
            dust.shader.UseColor(dust.color);
            if (!dust.noGravity)
                dust.velocity.Y += 0.1f;

            dust.velocity *= 0.99f;
            dust.color *= 0.95f;

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3());

            if (dust.scale < 0.05f)
                dust.active = false;

            return false;
        }
    }
}
