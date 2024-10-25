using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using DeusMod.Projs.Melee;
using DeusMod.Projs.Melee.SkillUI;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.GameContent;
using System;
using DeusMod.NPCs;

namespace DeusMod.Items.Weapon.Melee
{
    public class Kunai : ModItem
    {
		public override string Texture => "DeusMod/Assets/Items/Weapon/Melee/Kunai";
		public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Kunai");
            DisplayName.AddTranslation(7, "苦无");*/
        }
        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 20;
			Item.rare = ItemRarityID.White;
            Item.knockBack = 3;
			Item.mana = 10;
            Item.maxStack = 999;
			Item.consumable = true;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.value = Item.sellPrice(0, 0, 0, 10);
			Item.shoot = ModContent.ProjectileType<PlayerKunai>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Vector2 vec = Vector2.Normalize(Main.MouseWorld - player.Center);
			Projectile.NewProjectileDirect(source, player.Center, vec * 10, type, damage, knockback, player.whoAmI);
			return false;
		}
    }
    public class PlayerKunai : ModProjectile
    {
		public override string Texture => "DeusMod/Assets/Items/Weapon/Melee/Kunai";
		public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Kunai");
			//DisplayName.AddTranslation(7, "苦无");
		}
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 42;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.ignoreWater = false;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.hostile = false;
		}
        public override void Kill(int timeLeft)
        {
			for (int i = 0; i < 10; i++)
			{
				Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 1, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default(Color), 0.75f);
			}
		}
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}