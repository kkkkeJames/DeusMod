using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using ReLogic.Content;
using DeusMod.Projs.Melee;

namespace DeusMod.Items.Weapon.Melee
{
    /*
    对于永夜刃的重做是基于顶点绘制+shader（计划中）进行真近战剑气的重绘，以及一整套逻辑链条进行动作重做。
    重绘在学习中，无需多言。
    动作逻辑如下：
                                                                                                            {<1阈值——斩击第三下，向刚才的方向用力下挥，大抖屏，结束
                                                                                                            {
                                     {时间小于某个阈值——斩击正常第二下，向刚才挥剑的鼠标方向向上挥出第二下{>1<2阈值——进入变式二，向鼠标方向斜刺剑，而后如果小于阈值则手持式挥砍数次，次数小于刺剑，伤害等于中等刺剑，最后大挥剑，结束
                                     {                                                                      { 
                                     {                                                                      {>2阈值——重新开始连击判定
                                     {
                            {没有长按{时间大于第一阈值，小于第二阈值——进入变式一，向鼠标方向向上挑剑，而后如果小于阈值则打出手持弹幕式的连续刺剑，刺剑判定越来越大，伤害越来越高，最后一击大抖屏，结束
                            {        {
                            {        {时间大于第二阈值——重新开始连击判定
    向下向鼠标位置挥出第一下{
                            {                {完美长按，剑发出银光时松开鼠标——屏幕变暗，向鼠标方向无敌位移，留下一道斩击剑气，并且直接留下大量斩击剑气造成伤害
                            {长按达到一定时间{
                                             {非完美长按——向鼠标方向无敌位移，留下一道斩击剑气，并且斩击目标后会在目标范围斩击几道小剑气，造成较小伤害
    */
    public class NightsEdgeRE : GlobalItem
    {
        public static bool NEnormalend = false;
        public static bool NEcomboA = false;
        public static bool NEcomboB = false;
        public static int NEphase = 0;
        public static int NEcomboAphase = 0;
        public static int NEcomboBphase = 0;
        public static int NEdamage;
        public static float NEtimer;
        public static IEntitySource hold;
        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.NightsEdge:
                    item.noUseGraphic = true;
                    item.noMelee = true;
                    item.useStyle = 5;
                    item.useTime = item.useAnimation = 30;
                    item.shoot = ModContent.ProjectileType<NightsEdgeTarg>();
                    item.channel = true;
                    item.autoReuse = false;
                    break;
            }
        }
        public override void HoldItem(Item item, Player player)
        {
            if (item.type == ItemID.NightsEdge)
            {
                if (NEnormalend == true) //在一斩结束后开始计算后摇
                {
                    NEtimer++; //计时器增加后摇时间
                    if (NEtimer >= 60)
                    {
                        NEcomboA = NEcomboB = false;
                        NEnormalend = false; //四十五f是极限时间，过去了就是重新开始
                    }
                }
                else NEtimer = 0;
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (item.type == ItemID.NightsEdge)
            {
                if (NEnormalend)
                {
                    NEphase++; //在限定时间内斩击指示器加一，表示前往下一击打
                    if (NEphase == 3) NEphase = 0; //重置状态机

                    if (NEcomboA == true)
                    {
                        NEcomboAphase++;
                        if (NEcomboAphase == 4) NEcomboAphase = 0;
                    }

                    //ComboA变招，在第一下且在一定范围时间内攻击触发
                    if (NEtimer >= 45 && NEtimer < 60 && NEphase == 1 && NEcomboA == false)
                    {
                        NEcomboA = true;
                        NEcomboAphase++;
                        if (NEcomboAphase == 4) NEcomboAphase = 0;
                        NEphase = 0;
                    }
                }
                else
                {
                    NEphase = NEcomboAphase = NEcomboBphase = 0; //否则返回第一击
                    NEcomboA = NEcomboB = false;
                }

                Vector2 mousepos = player.Center - Main.screenPosition - Main.MouseScreen;
                
                //根据状态进行不同的斩击
                /*if (NEcomboA == false && NEcomboB == false) //正常
                {
                    Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.position, new Vector2(0, 0), ModContent.ProjectileType<NightsEdgeTarg>(), 0, 0, player.whoAmI);
                    switch (NEphase)
                    {
                        case 0:
                            DeusGlobalSlash.EnterStatics(ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.NightsEdge).Value, 3f, 2.7f, 2.6f, 0.7f, false, false, mousepos, item.damage, player.whoAmI, player.GetSource_ItemUse(item), new Color(153, 0, 204, 255), 4);
                            break;
                        case 1:
                            DeusGlobalSlash.EnterStatics(ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.NightsEdge).Value, 2.4f, 2.1f, 1.4f, 1.5f, true, false, mousepos, item.damage, player.whoAmI, player.GetSource_ItemUse(item), new Color(153, 0, 204, 255), 6);
                            break;
                        case 2:
                            DeusGlobalSlash.EnterStatics(ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.NightsEdge).Value, 3.2f, 3f, 2.4f, 0.9f, false, true, mousepos, item.damage, player.whoAmI, player.GetSource_ItemUse(item), new Color(153, 0, 204, 255));
                            break;
                    }
                }
                else if (NEcomboA)
                {
                    switch (NEcomboAphase)
                    {
                        case 1:
                            Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.position, new Vector2(0, 0), ModContent.ProjectileType<NightsEdgeTarg>(), 0, 0, player.whoAmI);
                            DeusGlobalSlash.EnterStatics(ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.NightsEdge).Value, 1.1f, 2.6f, 0f, 0.3f, true, false, mousepos, item.damage, player.whoAmI, player.GetSource_ItemUse(item), new Color(153, 0, 204, 255));
                            break;
                        case 2:
                            Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.position, new Vector2(0, 0), ModContent.ProjectileType<NightsEdgeTarg>(), 0, 0, player.whoAmI, 0, 1);
                            break;
                        case 3:
                            Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.position, new Vector2(0, 0), ModContent.ProjectileType<NightsEdgeTarg>(), 0, 0, player.whoAmI);
                            DeusGlobalSlash.EnterStatics(ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.NightsEdge).Value, 1.5f, 4f, 0.3f, -1f, false, true, mousepos, item.damage, player.whoAmI, player.GetSource_ItemUse(item), new Color(153, 0, 204, 255));
                            NEcomboA = false;
                            NEcomboAphase = 0;
                            break;
                    }
                }*/

                //鼠标和人的坐标关系是一个以人为中心的、左右翻转的平面直角坐标系，在人物左边x坐标大于零，在上边y坐标大于零
                NEdamage = item.damage;
                hold = player.GetSource_ItemUse(item);
                return false;
            }
            else return true;
        }
    }






    //这个是手持弹幕，用来计算攻击和comboA的突刺，如果ai[1]是1则A，是2则B
    public class NightsEdgeTarg : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_79";
        public override void SetStaticDefaults()
        {
            
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.noEnchantments = true;
            Projectile.friendly = true;
            Projectile.hide = true;
            base.SetDefaults();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 mousepos = player.Center - Main.screenPosition - Main.MouseScreen;
            if (mousepos.X >= 0)
            {
                if (mousepos.Y > 0) (Projectile.rotation) = -(float)Math.Atan(mousepos.Y / -mousepos.X) + (float)Math.PI;
                else (Projectile.rotation) = (float)Math.Atan(-mousepos.Y / -mousepos.X) + (float)Math.PI;
            }
            else
            {
                if (mousepos.Y > 0) (Projectile.rotation) = (float)Math.Atan(mousepos.Y / mousepos.X);
                else (Projectile.rotation) = -(float)Math.Atan(-mousepos.Y / mousepos.X);
            }
            NightsEdgeRE.NEnormalend = false; //一旦鼠标重新点击则停止计时并归零
            if (Main.player[Projectile.owner].channel)
            {
                Projectile.ai[0] += 1f;//作者特有的将ai[0]作为计时器
                Projectile.timeLeft = 10;

                if(Projectile.ai[1] == 1) //如果是特殊的刺击
                {
                    if (Projectile.ai[0] % 2 == 0)
                    {
                        float randnum = Main.rand.Next(-16, 16);
                        NightsEdgeCrack.EnterStatics(NightsEdgeRE.hold, player.whoAmI, new Vector2(0, randnum).RotatedBy(Projectile.rotation) + player.Center, new Vector2(240, randnum * 2).RotatedBy(Projectile.rotation) + player.Center, 0.5f, 1.6f, true);
                    }
                }
            }
            else
            {
                Projectile.Kill();
            }
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 mousepos = player.Center - Main.screenPosition - Main.MouseScreen;
            if (Projectile.ai[1] == 0) //只有普通的才会有冲刺
            {
                if (Projectile.ai[0] < 60) NightsEdgeRE.NEnormalend = true; //只要没进入冲刺准备，则一直都能重新斩击
                //if (Projectile.ai[0] >= 60 && Projectile.ai[0] <= 65) NightsEdgeEXRapidSlash.EnterStatics(NightsEdgeRE.hold, player.whoAmI, mousepos);
                //else if (Projectile.ai[0] > 65) NightsEdgeRapidSlash.EnterStatics(NightsEdgeRE.hold, player.whoAmI, mousepos);
            }
            else
            {
                NightsEdgeRE.NEnormalend = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[1] == 0)
            {
                if (Projectile.ai[0] == 60 || Projectile.ai[0] == 61)//可以斩了
                {
                    Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, player.Center - Main.screenPosition, null, Color.White, 0, new Vector2(Texture.Length, Texture.Length), 1, SpriteEffects.None, 0);
                }
                if (Projectile.ai[0] == 62)//可以斩了
                {
                    Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, player.Center - Main.screenPosition, null, Color.White * 0.5f, 0, new Vector2(Texture.Length, Texture.Length), 1, SpriteEffects.None, 0);
                }
            }
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Main.instance.DrawCacheProjsOverPlayers.Add(index);
        }
    }






    public class NightsEdgeRapidSlash : ModProjectile //冲刺冲刺！（otto饼干）
    {
        public override string Texture => "Terraria/Images/Extra_197";
        public static void EnterStatics(IEntitySource source, int owner, Vector2 mousepos)
        {
            var proj = Projectile.NewProjectileDirect(source, Main.player[owner].Center, new Vector2(0, 0), ModContent.ProjectileType<NightsEdgeRapidSlash>(), 0, 0f, owner);
            (proj.ModProjectile as NightsEdgeRapidSlash).mousepos = mousepos;
        }
        public Vector2 mousepos;
        public Vector2 slashstart;
        public Vector2 slashend;
        public override void SetDefaults()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.width = player.width / 3 * 2;
            Projectile.height = player.height / 3 * 2;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.immune = true;
            player.immuneTime = 10;
            if (Projectile.ai[0] == 0) slashstart = player.Center;
            Projectile.ai[0] += 1f;
            player.position = Projectile.position;
            player.velocity = Vector2.Zero;
            Projectile.rotation = (float)Math.Atan(mousepos.Y / mousepos.X);
            if (mousepos.X > 0)
            {
                if(Projectile.ai[0] <= 3)
                {
                    Projectile.velocity.X = -100 * (float)Math.Cos(Projectile.rotation);
                    Projectile.velocity.Y = -100 * (float)Math.Sin(Projectile.rotation);
                }
                else
                {
                    Projectile.velocity.X = -100 / (Projectile.ai[0] - 2) * (float)Math.Cos(Projectile.rotation);
                    Projectile.velocity.Y = -100 / (Projectile.ai[0] - 2) * (float)Math.Sin(Projectile.rotation);
                }
            }
            else
            {
                if (Projectile.ai[0] <= 3)
                {
                    Projectile.velocity.X = 100 * (float)Math.Cos(Projectile.rotation);
                    Projectile.velocity.Y = 100 * (float)Math.Sin(Projectile.rotation);
                }
                else
                {
                    Projectile.velocity.X = 100 / (Projectile.ai[0] - 2) * (float)Math.Cos(Projectile.rotation);
                    Projectile.velocity.Y = 100 / (Projectile.ai[0] - 2) * (float)Math.Sin(Projectile.rotation);
                }
            }
        }
        public override void Kill(int timeLeft) //输入剑气的数据
        {
            Player player = Main.player[Projectile.owner];
            player.immune = false;
            slashend = player.Center;
            Projectile.velocity = Vector2.Zero;
            player.velocity = Vector2.Zero;
            NightsEdgeRapidAttack.EnterStatics(NightsEdgeRE.hold, player.whoAmI, slashstart, slashend);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }






    /*public class NightsEdgeEXRapidSlash : ModProjectile //开大啊！开大啊！cnm开大啊！（otto饼干）
    {
        public static void EnterStatics(IEntitySource source, int owner, Vector2 mousepos)
        {
            var proj = Projectile.NewProjectileDirect(source, Main.player[owner].Center, new Vector2(0, 0), ModContent.ProjectileType<NightsEdgeEXRapidSlash>(), 0, 0f, owner);
            (proj.ModProjectile as NightsEdgeEXRapidSlash).mousepos = mousepos;
        }
        public Vector2 mousepos;
        public Vector2 slashstart;
        public Vector2 slashend;
        public override void SetDefaults()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.width = player.width / 3 * 2;
            Projectile.height = player.height / 3 * 2;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 66;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            //设置玩家冲刺无敌，射弹位置，方向，玩家方向等
            Player player = Main.player[Projectile.owner];
            player.immune = true;
            player.immuneTime = 10;
            if (Projectile.ai[0] == 0) slashstart = player.Center;
            Projectile.ai[0] += 1f;
            if(Projectile.ai[0] <= 10) player.position = Projectile.position;
            if (Projectile.ai[0] < 10) Projectile.rotation = (float)Math.Atan(mousepos.Y / mousepos.X);
            DeusPlayer.shock = DeusPlayer.megashock = false;
            if (mousepos.X > 0)
            {
                if (Projectile.ai[0] <= 3)
                {
                    Projectile.velocity.X = -150 * (float)Math.Cos(Projectile.rotation);
                    Projectile.velocity.Y = -150 * (float)Math.Sin(Projectile.rotation);
                }
                else if (Projectile.ai[0] <= 10)
                {
                    Projectile.velocity.X = -150 / (Projectile.ai[0] - 2) * (float)Math.Cos(Projectile.rotation);
                    Projectile.velocity.Y = -150 / (Projectile.ai[0] - 2) * (float)Math.Sin(Projectile.rotation);
                }
            }
            else
            {
                if (Projectile.ai[0] <= 3)
                {
                    Projectile.velocity.X = 150 * (float)Math.Cos(Projectile.rotation);
                    Projectile.velocity.Y = 150 * (float)Math.Sin(Projectile.rotation);
                }
                else if(Projectile.ai[0] <= 10)
                {
                    Projectile.velocity.X = 150 / (Projectile.ai[0] - 2) * (float)Math.Cos(Projectile.rotation);
                    Projectile.velocity.Y = 150 / (Projectile.ai[0] - 2) * (float)Math.Sin(Projectile.rotation);
                }
            }
            if (!Filters.Scene["Test"].IsActive())
            {
                Filters.Scene.Activate("Test");
            }

            if(Projectile.ai[0] >= 10) //冲刺结束，停止无敌，设立终止点，然后射弹停下
            {
                player.immune = false;
                slashend = player.Center;
                Projectile.velocity = Vector2.Zero;
            }

            if(Projectile.ai[0] == 10) //pause pause pause
            {
                player.velocity = Vector2.Zero;
                NightsEdgeCrack.EnterStatics(NightsEdgeRE.hold, player.whoAmI, slashstart, slashend, 1.5f, 2, true);
                DeusPlayer.shock = true;
            }
            if (Projectile.ai[0] == 16)
            {
                NightsEdgeCrack.EnterStatics(NightsEdgeRE.hold, player.whoAmI, slashstart + new Vector2(80, -160).RotatedBy((Projectile.rotation)), slashend + new Vector2(120, 200).RotatedBy((Projectile.rotation)), 1.5f, 2, true);
                DeusPlayer.shock = true;
            }
            if (Projectile.ai[0] == 22)
            {
                NightsEdgeCrack.EnterStatics(NightsEdgeRE.hold, player.whoAmI, slashstart + new Vector2(20, 80).RotatedBy((Projectile.rotation)), slashend + new Vector2(-40, -120).RotatedBy((Projectile.rotation)), 1.5f, 2, true);
                DeusPlayer.shock = true;
            }
            if (Projectile.ai[0] == 28)
            {
                NightsEdgeCrack.EnterStatics(NightsEdgeRE.hold, player.whoAmI, slashend + new Vector2(-20, 160).RotatedBy((Projectile.rotation)), slashstart + new Vector2(-140, -240).RotatedBy((Projectile.rotation)), 1.5f, 2, true);
                DeusPlayer.shock = true;
            }
            if (Projectile.ai[0] == 34)
            {
                NightsEdgeCrack.EnterStatics(NightsEdgeRE.hold, player.whoAmI, slashend + new Vector2(-60, -160).RotatedBy((Projectile.rotation)), slashstart + new Vector2(100, 120).RotatedBy((Projectile.rotation)), 1.5f, 2, true);
                DeusPlayer.shock = true;
            }
            if (Projectile.ai[0] == 65)
            {
                NightsEdgeCrack.EnterStatics(NightsEdgeRE.hold, player.whoAmI, slashstart + new Vector2(-200, -280).RotatedBy((Projectile.rotation)), slashend + new Vector2(160, 200).RotatedBy((Projectile.rotation)), 2, 3, true);
                DeusPlayer.megashock = true;
            }
        }
        public override void Kill(int timeLeft) //结束屏幕变暗效果的同时输入剑气的数据
        {
            Player player = Main.player[Projectile.owner];
            DeusPlayer.shock = DeusPlayer.megashock = false;
            if (Filters.Scene["Test"].IsActive())
            {
                Filters.Scene.Deactivate("Test");
            }
        }
        public override bool PreDraw(ref Color lightColor)//冲刺时人变成暗影恶鬼
        {
            if (Projectile.ai[0] <= 10)
            {
                Player player = Main.player[Projectile.owner];
                Main.EntitySpriteDraw(ModContent.Request<Texture2D>("DeusMod/Items/Weapon/Melee/Swords/Vanilla/NightsEdgeEXRapidSlash").Value, player.Center - Main.screenPosition, null, Color.White, (Projectile.rotation), new Vector2(Projectile.width / 2, Projectile.height / 2), 1, SpriteEffects.FlipHorizontally, 0);
            }
            return false;
        }
    }*/






    public class NightsEdgeRapidAttack : ModProjectile //冲刺留下的剑气
    {
        public override string Texture => "Terraria/Images/Projectile_927";
        //startpoint和endpoint为剑气和起始和终点，normal代表两点之间的向量，射弹的中心位于两点连线的中心
        public static void EnterStatics(IEntitySource source, int owner, Vector2 startpoint, Vector2 endpoint)
        {
            Vector2 normal = endpoint - startpoint;
            var proj = Projectile.NewProjectileDirect(source, startpoint + (normal/2), Vector2.Zero, ModContent.ProjectileType<NightsEdgeRapidAttack>(), NightsEdgeRE.NEdamage, 0f, owner);
            (proj.ModProjectile as NightsEdgeRapidAttack).normal = normal;
            (proj.ModProjectile as NightsEdgeRapidAttack).startpoint = startpoint;
            (proj.ModProjectile as NightsEdgeRapidAttack).endpoint = endpoint;
        }
        public Vector2 normal;
        public Vector2 startpoint;
        public Vector2 endpoint;
        public float dec;
        public override void SetDefaults()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.width = (int)normal.Length();
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 100;
        }
        public override void AI()
        {
            //从左上角开始往右、往下坐标
            (Projectile.rotation) = (float)Math.Atan2(endpoint.Y - startpoint.Y, endpoint.X - startpoint.X);
            Projectile.width = (int)normal.Length();
            Projectile.Center += normal / 10;
            Projectile.ai[0] += 1;
            if (Projectile.ai[0] <= 9) dec = 10;
            else dec = 10 - (Projectile.ai[0] - 9) * 5;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            //前两个参数没必要动，第三个是头，第四个是尾，第五个是宽度，第六个别动
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), startpoint, endpoint, Projectile.height, ref point);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            //player.GetModPlayer<DeusPlayer>().shock = true;
            NightsEdgeCrack.EnterStatics(NightsEdgeRE.hold, player.whoAmI, target.Center + new Vector2(-64, -128), target.Center, 1, 1, false);
            NightsEdgeCrack.EnterStatics(NightsEdgeRE.hold, player.whoAmI, target.Center + new Vector2(-112, 192), target.Center, 1, 1, false);
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            //player.GetModPlayer<DeusPlayer>().shock = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            List<VertexInfo2> slash = new List<VertexInfo2>(); //slash存储的所有坐标都是在屏幕上的坐标
            for (int i = 0; i <= 1; i++)
            {
                Vector2 pos0 = startpoint + normal / 10 * Projectile.ai[0] + new Vector2(Projectile.width * i, (-Projectile.height / 2) / 10 * dec).RotatedBy((Projectile.rotation)) - Main.screenPosition;
                slash.Add(new VertexInfo2(pos0, new Vector3(1 - i, 0, 1), new Color(153, 0, 204, 255)));
                Vector2 pos1 = startpoint + normal / 10 * Projectile.ai[0] + new Vector2(Projectile.width * i, (Projectile.height / 2) / 10 * dec).RotatedBy((Projectile.rotation)) - Main.screenPosition;
                slash.Add(new VertexInfo2(pos1, new Vector3(1 - i, 1f, 1), new Color(153, 0, 204, 255)));
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Projectile.type].Value;
            if (slash.Count >= 3)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, slash.ToArray(), 0, slash.Count - 2);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }






    public class NightsEdgeCrack : ModProjectile //剑气切割的统一的模板
    {
        public override string Texture => "Terraria/Images/Projectile_927";
        public static void EnterStatics(IEntitySource source, int owner, Vector2 startpoint, Vector2 endpoint, float damagescale, float widthscale, bool slowdown)
        {
            Vector2 normal = endpoint - startpoint;
            var proj = Projectile.NewProjectileDirect(source, startpoint + (normal / 2), Vector2.Zero, ModContent.ProjectileType<NightsEdgeCrack>(), (int)(NightsEdgeRE.NEdamage * damagescale), 0f, owner);
            (proj.ModProjectile as NightsEdgeCrack).normal = normal;
            (proj.ModProjectile as NightsEdgeCrack).startpoint = startpoint;
            (proj.ModProjectile as NightsEdgeCrack).endpoint = endpoint;
            (proj.ModProjectile as NightsEdgeCrack).widthscale = widthscale;
            (proj.ModProjectile as NightsEdgeCrack).slowdown = slowdown;
        }
        public Vector2 normal;
        public Vector2 startpoint;
        public Vector2 endpoint;
        public float widthscale;
        public float dec;
        public bool slowdown;
        public override void SetDefaults()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.width = (int)normal.Length();
            Projectile.height = 40;
            Projectile.penetrate = -1; 
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 100;
        }
        public override void AI()
        {
            //从左上角开始往右、往下坐标
            (Projectile.rotation) = (float)Math.Atan2(endpoint.Y - startpoint.Y, endpoint.X - startpoint.X);
            Projectile.width = (int)normal.Length();
            Projectile.height = (int)(32 * widthscale);
            if(slowdown) Projectile.Center += normal / (float)Math.Pow(5, Projectile.ai[0]);
            else Projectile.Center += normal / 10;
            Projectile.ai[0] += 1;
            if (Projectile.ai[0] <= 9) dec = 10;
            else dec = 10 - (Projectile.ai[0] - 9) * 5;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            //前两个参数没必要动，第三个是头，第四个是尾，第五个是宽度，第六个别动
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), startpoint, endpoint, Projectile.height, ref point);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            List<VertexInfo2> slash = new List<VertexInfo2>(); //slash存储的所有坐标都是在屏幕上的坐标
            for (int i = 0; i <= 1; i++)
            {
                if (slowdown)
                {
                    Vector2 pos0 = startpoint + normal / (float)Math.Pow(5, Projectile.ai[0]) * Projectile.ai[0] + new Vector2(Projectile.width * i, (-Projectile.height / 2) / 10 * dec).RotatedBy((Projectile.rotation)) - Main.screenPosition;
                    slash.Add(new VertexInfo2(pos0, new Vector3(1 - i, 0, 1), new Color(153, 0, 204, 255)));
                    Vector2 pos1 = startpoint + normal / (float)Math.Pow(5, Projectile.ai[0]) * Projectile.ai[0] + new Vector2(Projectile.width * i, (Projectile.height / 2) / 10 * dec).RotatedBy((Projectile.rotation)) - Main.screenPosition;
                    slash.Add(new VertexInfo2(pos1, new Vector3(1 - i, 1f, 1), new Color(153, 0, 204, 255)));
                }
                else
                {
                    Vector2 pos0 = startpoint + normal / 10 * Projectile.ai[0] + new Vector2(Projectile.width * i, (-Projectile.height / 2) / 10 * dec).RotatedBy((Projectile.rotation)) - Main.screenPosition;
                    slash.Add(new VertexInfo2(pos0, new Vector3(1 - i, 0, 1), new Color(153, 0, 204, 255)));
                    Vector2 pos1 = startpoint + normal / 10 * Projectile.ai[0] + new Vector2(Projectile.width * i, (Projectile.height / 2) / 10 * dec).RotatedBy((Projectile.rotation)) - Main.screenPosition;
                    slash.Add(new VertexInfo2(pos1, new Vector3(1 - i, 1f, 1), new Color(153, 0, 204, 255)));
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Projectile.type].Value;
            if (slash.Count >= 3)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, slash.ToArray(), 0, slash.Count - 2);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
