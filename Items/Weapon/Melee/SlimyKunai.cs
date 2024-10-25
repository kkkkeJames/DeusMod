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

namespace DeusMod.Items.Weapon.Melee
{
	public class SlimyKunai : ModItem
	{
		public override string Texture => "DeusMod/Assets/Items/Weapon/Melee/SlimyKunai";
		public override void SetStaticDefaults()
		{
			/*DisplayName.SetDefault("Slimy Kunai");
			DisplayName.AddTranslation(7, "史莱姆苦无");
			Tooltip.SetDefault("Stick enemies with slime\nSticking enenimes with slime for a period would slower their speed");
			Tooltip.AddTranslation(7, "将敌怪黏上史莱姆\n史莱姆黏着达到一定时长后会减缓黏着敌人的速度");*/
		}
		public override void SetDefaults()
		{
			Item.damage = 20;
			Item.DamageType = DamageClass.Melee;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 18;
			Item.rare = ItemRarityID.Blue;
			Item.knockBack = 3;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.value = Item.sellPrice(0, 0, 1, 0);
			Item.shoot = ModContent.ProjectileType<PlayerSlimyKunai>();
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 vec = Vector2.Normalize(Main.MouseWorld - player.Center);
			Projectile.NewProjectileDirect(source, player.Center, vec * 10, type, damage, knockback, player.whoAmI);
			return false;
		}
		public override void AddRecipes()
		{
			var recipe = CreateRecipe(50);
			recipe.AddIngredient(ModContent.ItemType<Kunai>(), 50);
			recipe.AddIngredient(ItemID.Gel, 100);
			recipe.AddTile(TileID.Solidifier);
			recipe.Register();
		}
	}
	public class PlayerSlimyKunai : ModProjectile
	{
		public override string Texture => "DeusMod/Assets/Items/Weapon/Melee/SlimyKunai";
		public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Slimy Kunai");
			//DisplayName.AddTranslation(7, "史莱姆苦无");
		}
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.ignoreWater = false;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.hostile = false;
		}
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.AddBuff(BuffID.Slimed, 2400);
		}
        public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				Dust dust;
				int d = Dust.NewDust(Projectile.Center, 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 1.5f);
				Main.dust[d].noGravity = true;
				dust = Main.dust[d];
				dust.velocity *= 2f;
			}
		}
	}
}