using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using System;
using System.Collections.Generic;
using DeusMod.Dusts;
using DeusMod.Items.Ammo;
using DeusMod.Items.Ammo.Magnum;
using DeusMod.Core.Systems;

namespace DeusMod.Items.Weapon.Ranged.Guns.LighteningHawk
{
    public class LighteningHawk : ModItem
    {
        public bool shootS;
        public float shootangle = 0;
        public override void SetStaticDefaults()
		{
			/*DisplayName.SetDefault("Lightening Hawk"); 
			DisplayName.AddTranslation(7, "闪电鹰");
			Tooltip.SetDefault("Use magnum as ammo\n'Go to hell!'");
			Tooltip.AddTranslation(7, "使用马格南弹作为子弹\n“下地狱去吧！”");*/
		}
		public override void SetDefaults()
		{
			Item.damage = 45;
            Item.DamageType = DamageClass.Ranged;
			Item.crit = 6;
			Item.width = 44;
			Item.height = 30;
			Item.useTime = Item.useAnimation = 50;
			Item.useStyle = 87;
			Item.noUseGraphic = true;
			Item.UseSound = null;
			Item.knockBack = 1;
			Item.value = Item.buyPrice(0, 2, 0, 0);
			Item.rare = 1;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = false;
			Item.maxStack = 1;
			Item.shootSpeed = 15f;
			Item.useAmmo = ModContent.ItemType<MagnumBullet>();
			Item.shoot = ModContent.ProjectileType<MagnumProj>();
			Item.noMelee = true;
		}
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            float n = (player.direction == 1) ? 0 : (float)Math.PI;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<LighteningHawkShootS>()] >= 1)
                velocity = new Vector2(Item.shootSpeed, 0).RotatedBy(shootangle + n);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<LighteningHawkShootS>(), 0, 0, player.whoAmI);
                proj.rotation = (float)Math.Atan(velocity.Y / velocity.X);
                return false;
            }
            else
            {
                player.GetModPlayer<ScreenShake>().ScreenShakeShort(48, (float)(Math.Atan(velocity.Y / velocity.X) + Math.PI));
                player.HeldItem.GetGlobalItem<GlobalGun>().bulletnum -= 1;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<LighteningHawkShootS>()] == 0)
                {
                    Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<LighteningHawkShootH>(), 0, 0, player.whoAmI);
                    proj.rotation = (float)Math.Atan(velocity.Y / velocity.X);
                }
                else shootS = true;
                return true;
            }
        }
    }






    public class LighteningHawkShootS : ModProjectile //更稳定的肩射，需要瞄准
    {
        private float rot = 0;
        private float lim;
        private float vel;
        private float vellim;
        private int shoottime;
        public override string Texture => "DeusMod/Items/Weapon/Ranged/Guns/LighteningHawk/LighteningHawk";
        public override void SetStaticDefaults()
        {
            //Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.hide = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects effects = (player.direction == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle frame = new Rectangle(0, Projectile.height * Projectile.frameCounter, Projectile.width, Projectile.height);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, new Vector2(Projectile.width / 2, Projectile.height / 2), 1, effects, 0);
            return false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            DeusPlayer.shootS = true;
            player.direction = (Main.MouseWorld - player.position).X > 0 ? 1 : -1;
            var p = player.HeldItem;
            ((LighteningHawk)p.ModItem).shootangle = Projectile.rotation;
            #region 射击
            if (((LighteningHawk)p.ModItem).shootS)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(DeusMod)}/Sounds/Ranged/Guns/Undertaker/UndertakerShoot"), player.Center);
                }

                if (Projectile.ai[1] <= 18)
                {
                    Projectile.rotation -= (0.001f * (18 - Projectile.ai[1]) * (18 - Projectile.ai[1])) * player.direction;
                }
                else if (Projectile.ai[1] <= 36)
                {
                    Projectile.rotation += (0.001f * (19 - Projectile.ai[1]) * (19 - Projectile.ai[1])) * player.direction;
                }
                Projectile.Center = player.Center + new Vector2(Projectile.width * player.direction / 2, 0).RotatedBy(Projectile.rotation);

                //if (Projectile.ai[1] == 18) Projectile.frameCounter = 1;
                //if (Projectile.ai[1] == 22) Projectile.frameCounter = 0;

                //子弹射击特效
                if (Projectile.ai[1] == 1)
                {
                    int dust = Dust.NewDust(Projectile.Center + new Vector2(Projectile.width / 2, 0).RotatedBy(Projectile.rotation), 22, 12, ModContent.DustType<Gunfire_L>(), 0, 0);
                    for (int i = 0; i < 9; i++)
                    {
                        Dust.NewDustDirect(Projectile.Center + new Vector2(Projectile.width * player.direction, 0).RotatedBy(Projectile.rotation), 4, 4, DustID.Smoke, Main.rand.NextFloat(player.direction, 2 * player.direction), Main.rand.NextFloat(-1.5f, 1.5f));
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        Dust.NewDustDirect(Projectile.Center + new Vector2(Projectile.width * player.direction, 0).RotatedBy(Projectile.rotation), 4, 4, DustID.Torch, Main.rand.NextFloat(player.direction, 1.2f * player.direction), Main.rand.NextFloat(0.8f, 1.2f));
                    }
                    Dust.NewDustDirect(Projectile.Center + new Vector2(Projectile.width * player.direction, 0).RotatedBy(Projectile.rotation), 3, 3, DustID.Smoke, player.direction, 0);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Projectile.width * player.direction, 0).RotatedBy(Projectile.rotation), new Vector2(player.direction, 1), Main.rand.Next(61, 64));
                }
                if (Projectile.ai[1] > shoottime)
                {
                    Projectile.ai[1] = 0;
                    ((LighteningHawk)p.ModItem).shootS = false;
                }
            }
            #endregion
            #region 瞄准角度处理
            else
            {
                Projectile.ai[0]++;
                rot += vel;
                if (vel > 0)
                {
                    if (rot < lim * 0.67f)
                    {
                        if (vel < vellim) vel *= 1.1f;
                        else if (vel > vellim) vel = vellim;
                    }
                    else
                    {
                        vel *= 0.9f;
                        if (vel < 0.001f || rot >= lim) vel = -0.01f;
                    }
                }
                if (vel < 0)
                {
                    if (rot > -lim * 0.67f)
                    {
                        if (vel > -vellim) vel *= 1.1f;
                        else if (vel < -vellim) vel = -vellim;
                    }
                    else
                    {
                        vel *= 0.9f;
                        if (vel > -0.001f || rot <= -lim) vel = 0.01f;
                    }
                }
                Vector2 mousepos = Main.MouseWorld - player.position;
                Projectile.rotation = rot + (float)Math.Atan(mousepos.Y / mousepos.X);
            }
            float n = player.direction == 1 ? -1 : 1;
            #endregion
            #region 相关判定
            Projectile.Center = player.Center + new Vector2(Projectile.width * player.direction / 2, 0).RotatedBy(Projectile.rotation);
            player.SetCompositeArmFront(true, 0, Projectile.rotation + n);
            if (Projectile.ai[0] > 1 && Projectile.ai[1] == 0 && Main.mouseRightRelease == true) Projectile.Kill();
            else Projectile.timeLeft = 2;
            if (player.HeldItem.type != ModContent.ItemType<LighteningHawk>()) Projectile.Kill();
            #endregion
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Main.instance.DrawCacheProjsOverPlayers.Add(index);
        }
    }






    public class LighteningHawkShootH : ModProjectile //腰射
    {
        public override string Texture => "DeusMod/Items/Weapon/Ranged/Guns/LighteningHawk/LighteningHawk";
        public override void SetStaticDefaults()
        {
            //Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 50;
            Projectile.hide = true;
            Projectile.friendly = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects effects = (player.direction == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle frame = new Rectangle(0, Projectile.height * Projectile.frameCounter, Projectile.width, Projectile.height);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, new Vector2(Projectile.width / 2, Projectile.height / 2), 1, effects, 0);
            return false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            //射击贴图和效果
            Projectile.ai[1]++;
            if (Projectile.ai[1] == 1)
            {
                SoundEngine.PlaySound(new SoundStyle($"{nameof(DeusMod)}/Sounds/Ranged/Guns/Undertaker/UndertakerShoot"), player.Center);
            }
            if (Projectile.ai[1] <= 18)
            {
                Projectile.rotation -= (0.001f * (18 - Projectile.ai[1]) * (18 - Projectile.ai[1])) * player.direction;
            }
            else if (Projectile.ai[1] <= 36)
            {
                Projectile.rotation += (0.001f * (18 - Projectile.ai[1]) * (18 - Projectile.ai[1])) * player.direction;
            }
            Projectile.Center = player.Center + new Vector2(Projectile.width * player.direction / 2, 0).RotatedBy(Projectile.rotation);

            //if (Projectile.ai[0] == 18) Projectile.frameCounter = 1;
            //if (Projectile.ai[0] == 22) Projectile.frameCounter = 0;

            //子弹射击特效
            if (Projectile.ai[0] == 1)
            {
                int dust = Dust.NewDust(Projectile.Center + new Vector2(Projectile.width / 2, 0).RotatedBy(Projectile.rotation), 22, 12, ModContent.DustType<Gunfire_L>(), 0, 0);
                for (int i = 0; i < 9; i++)
                {
                    Dust.NewDustDirect(Projectile.Center + new Vector2(Projectile.width * player.direction, 0).RotatedBy(Projectile.rotation), 4, 4, DustID.Smoke, Main.rand.NextFloat(player.direction, 2 * player.direction), Main.rand.NextFloat(-1.5f, 1.5f));
                }
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDustDirect(Projectile.Center + new Vector2(Projectile.width * player.direction, 0).RotatedBy(Projectile.rotation), 4, 4, DustID.Torch, Main.rand.NextFloat(player.direction, 1.2f * player.direction), Main.rand.NextFloat(0.8f, 1.2f));
                }
                Dust.NewDustDirect(Projectile.Center + new Vector2(Projectile.width * player.direction, 0).RotatedBy(Projectile.rotation), 3, 3, DustID.Smoke, player.direction, 0);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Projectile.width * player.direction, 0).RotatedBy(Projectile.rotation), new Vector2(player.direction, 1), Main.rand.Next(61, 64));
            }
            player.SetCompositeArmFront(true, 0, Projectile.rotation - 1f * player.direction);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Main.instance.DrawCacheProjsOverPlayers.Add(index);
        }
    }






    public class LighteningHawkReload : ModProjectile
    {
        public override string Texture => "DeusMod/Items/Weapon/Ranged/Guns/LighteningHawk/LighteningHawk";
        private int BodyFrameCounter = 0;
        private Vector2 FullCasePosition;
        private Vector2 PumpPosition;
        public override void SetStaticDefaults()
        {
            //Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.friendly = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects effects = (player.direction == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D PumpTex = ModContent.Request<Texture2D>(Texture + "_Pump").Value;
            Rectangle PumpFrame = new Rectangle(0, 0, PumpTex.Width, PumpTex.Height);
            int PumpWidth = (player.direction == 1) ? 0 : PumpTex.Width;
            Texture2D BodyTex = ModContent.Request<Texture2D>(Texture + "_Body").Value;
            int BodyFrameNum = 2;
            Rectangle BodyFrame = new Rectangle(0, BodyFrameCounter * BodyTex.Height / BodyFrameNum, BodyTex.Width, BodyTex.Height / BodyFrameNum);
            int BodyWidth = (player.direction == 1) ? 0 : BodyTex.Width;
            Texture2D FullCaseTex = ModContent.Request<Texture2D>(Texture + "_FullCase").Value;
            Rectangle FullCaseFrame = new Rectangle(0, 0, FullCaseTex.Width, FullCaseTex.Height);
            int FullCaseWidth = (player.direction == 1) ? 0 : FullCaseTex.Width;
            if (Projectile.ai[0] > 60)
                Main.EntitySpriteDraw(FullCaseTex, player.Center - Main.screenPosition + FullCasePosition, FullCaseFrame, Color.White, 0f, new Vector2(FullCaseWidth, Projectile.height / 2), 1, effects, 0);
            Main.EntitySpriteDraw(BodyTex, player.Center - Main.screenPosition, BodyFrame, Color.White, 0f, new Vector2(BodyWidth, Projectile.height / 2), 1, effects, 0);
            Main.EntitySpriteDraw(PumpTex, player.Center - Main.screenPosition + PumpPosition, PumpFrame, Color.White, 0f, new Vector2(PumpWidth, Projectile.height / 2), 1, effects, 0);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.NewProjectile(source, player.Center + new Vector2(2, 2), new Vector2(-0.4f * player.direction, 0.6f), ModContent.ProjectileType<LighteningHawkCase>(), 0, 0, player.whoAmI);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;
            Projectile.Center = player.Center;
            player.GetModPlayer<DeusPlayer>().reload = true;
            player.SetCompositeArmFront(true, 0, (Projectile.rotation - 0.8f) * player.direction);
            if(Projectile.ai[0] > 60)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[0] <= 65)
                    FullCasePosition = new Vector2(2 * player.direction, 28 - Projectile.ai[1] * 4);
                else if (Projectile.ai[0] < 100)
                    FullCasePosition = new Vector2(2 * player.direction, 8);
                else
                    FullCasePosition = new Vector2(2 * player.direction, 4);
            }
            if(Projectile.ai[0] <= 120)
                PumpPosition = new Vector2(-4 * player.direction, 0);
            if(Projectile.ai[0] > 120)
            {
                if (Projectile.ai[0] <= 125)
                    PumpPosition -= new Vector2(1.2f, 0) * player.direction;
                else if(Projectile.ai[0] > 150 && Projectile.ai[0] <= 155)
                    PumpPosition += new Vector2(3.2f, 0) * player.direction;
            }
            player.itemTime = player.itemAnimation = 2;
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.itemAnimation = player.itemTime = 0;
            player.HeldItem.GetGlobalItem<GlobalGun>().bulletnum = player.HeldItem.GetGlobalItem<GlobalGun>().bulletcapacity;
            player.GetModPlayer<DeusPlayer>().reload = false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Main.instance.DrawCacheProjsOverPlayers.Add(index);
        }
    }
    public class LighteningHawkCase : ModProjectile
    {
        public override string Texture => "DeusMod/Items/Weapon/Ranged/Guns/LighteningHawk/LighteningHawk_Case";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 26;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Projectile.rotation -= 0.016f;
            Projectile.velocity.Y += 0.2f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.rotation += 0.014f;
            Projectile.velocity.X *= 0.95f;
            Projectile.velocity.Y = 0;
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
    }
}
