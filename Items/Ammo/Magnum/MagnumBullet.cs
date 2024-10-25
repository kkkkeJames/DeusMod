using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
namespace DeusMod.Items.Ammo.Magnum
{
	// Token: 0x0200127D RID: 4733
	public class MagnumBullet : ModItem
	{
		// Token: 0x06006F6D RID: 28525 RVA: 0x004D9C29 File Offset: 0x004D7E29
		public override string Texture => "DeusMod/Assets/Items/Ammo/MagnumBullet";
		public override void SetStaticDefaults()
		{
			/*DisplayName.SetDefault("Magnum");
			DisplayName.AddTranslation(7, "马格南弹");
			Tooltip.SetDefault("3* crit damage\nCan only be used by a few guns");
			Tooltip.AddTranslation(7, "3倍暴击伤害\n仅能被几种枪使用");*/
		}

		// Token: 0x06006F6E RID: 28526 RVA: 0x004D9C4C File Offset: 0x004D7E4C
		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 8;
			Item.height = 12;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.knockBack = 3;
			Item.value = Item.buyPrice(0, 0, 10, 0);
			Item.rare = ItemRarityID.Orange;
			Item.shoot = ModContent.ProjectileType<MagnumProj>();
			Item.shootSpeed = 10f;
			Item.ammo = Item.type;
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.FossilOre, 20)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
	public class MagnumProj : ModProjectile
	{
		public override string Texture => "DeusMod/Assets/Items/Ammo/MagnumProj";
		public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Magnum projectile");
			//DisplayName.AddTranslation(7, "马格南弹");
		}
		public override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.aiStyle = 1;
			Projectile.penetrate = 1;
			AIType = ProjectileID.Bullet;
		}
		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, (float)(255 - Projectile.alpha) * 0f / 155f, (float)(255 - Projectile.alpha) * 0.25f / 155f, (float)(255 - Projectile.alpha) * 0f / 155f);
		}
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += 1;
		}
    }
}