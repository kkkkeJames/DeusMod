using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeusMod.Items
{
	public class BloodyEdge : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("BloodyEdge"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			//Tooltip.SetDefault("This is a basic modded sword.");
		}

		public override void SetDefaults()
		{
			Item.damage = 50;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<BaseProj>();
			Item.autoReuse = true;
		}
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
    }
	public class BaseProj : ModProjectile
    {
		public override string Texture => "Terraria/Images/Projectile_0";
		public override void SetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			base.SetDefaults();
		}

		public override void AI()
		{
			if (Projectile.timeLeft < 240 && Projectile.timeLeft > 200)
				Projectile.velocity = Projectile.velocity.RotatedBy(0.1f);

			Dust d = Dust.NewDustDirect(Projectile.Center, 40, 40, ModContent.DustType<CosmicFlame>(), 0, 0, 0, default, 1.2f);
			if (Projectile.velocity != Vector2.Zero)
				d.velocity = Vector2.Zero;
			else
				d.velocity = new Vector2(0, -4);
		}
        public override bool PreDraw(ref Color lightColor)
        {
			foreach (Dust d in Main.dust)
			{
				if (d.type == ModContent.DustType<CosmicFlame>() && d.active)
				{
					Texture2D tex = ModContent.Request<Texture2D>("DeusMod/CosmicFlame").Value;
					Main.EntitySpriteDraw(tex, d.position - Main.screenPosition, null, Color.White, 0, tex.Size() / 2, d.scale, SpriteEffects.None, 0);
				}
			}
			return false;
		}
    }
	public class CosmicFlame : ModDust
	{
		public override string Texture => "DeusMod/CosmicFlame";
		public override void OnSpawn(Dust dust)
		{
			dust.alpha = 255;
			dust.noLight = true;
			dust.noGravity = true;
			base.OnSpawn(dust);
		}
		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.scale -= 0.03f;
			dust.velocity *= 0.99f;
			if (dust.scale <= 0)
				dust.active = false;
			return false;
		}
	}
}