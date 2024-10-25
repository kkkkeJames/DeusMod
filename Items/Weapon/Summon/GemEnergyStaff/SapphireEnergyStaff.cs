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

namespace DeusMod.Items.Weapon.Summon.GemEnergyStaff
{
    public class SapphireEnergyStaff : ModItem
    {
		public override string Texture => "DeusMod/Assets/Items/Weapon/Summon/PHM/SapphireEnergyStaff/SapphireEnergyStaff";
		public override void SetStaticDefaults()
		{
			/*DisplayName.SetDefault("Sapphire Energy Staff");
			DisplayName.AddTranslation(7, "蓝玉能源法杖");
			Tooltip.SetDefault("Summons a sapphire energy aggregate to attack enemies");
			Tooltip.AddTranslation(7, "召唤一个蓝玉能量聚集体攻击敌怪");*/
		}
		public override void SetDefaults()
		{
			Item.mana = 6;
			Item.damage = 22;
			Item.DamageType = DamageClass.Summon;
			Item.sentry = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = Item.useAnimation = 36;
			Item.rare = ItemRarityID.Blue;
			Item.knockBack = 4;
			Item.noUseGraphic = false;
			Item.noMelee = true;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.autoReuse = true;
			Item.UseSound = SoundID.MaxMana;
			Item.shoot = ModContent.ProjectileType<SapphireEnergyAggregate>();
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
			player.UpdateMaxTurrets();
			return false;
		}
		public override void AddRecipes()
		{
			var recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.SilverBar, 10);
			recipe.AddIngredient(ItemID.Sapphire, 8);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}

	public class SapphireEnergyStaffPlayer : ModPlayer
	{
		public bool SapphireEnergyAggregate;
		public override void ResetEffects()
		{
			SapphireEnergyAggregate = false;
		}
	}

	public class SapphireEnergyAggregate : ModProjectile
	{
		public override string Texture => "DeusMod/Assets/Items/Weapon/Summon/PHM/SapphireEnergyStaff/SapphireEnergyAggregate";
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
			Main.projFrames[Projectile.type] = 8;
		}
		public override void SetDefaults()
		{
			Projectile.width = 28;
			Projectile.height = 44;
			Projectile.netImportant = true;
			Projectile.friendly = true;
			Projectile.minionSlots = 1f;
			Projectile.timeLeft = 18000;
			Projectile.penetrate = -1;
			Projectile.minion = true;
			Projectile.sentry = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.tileCollide = false;
		}
		public override void AI()
		{
			Projectile.frameCounter++;
			if (Projectile.frameCounter % 3 == 0)
				Projectile.frame++;
			if (Projectile.frame >= Main.projFrames[Projectile.type])
				Projectile.frame = 0;

			Player player = Main.player[Projectile.owner];
			NPC target = null;
			float distanceMax = 600f;
			if (target == null)
			{
				foreach (NPC npc in Main.npc)
				{
					if (npc.active && !npc.friendly)
					{
						float currentDistance = Vector2.Distance(npc.Center, Projectile.Center);
						if (currentDistance < distanceMax) //比较目标和当前距离
						{
							distanceMax = currentDistance;
							target = npc;
						}
					}
				}
			}
			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 54 && target != null)
			{
				Vector2 vec = Vector2.Normalize(target.Center - Projectile.Center);
				Projectile proj = Projectile.NewProjectileDirect(Entity.GetSource_FromAI(), Projectile.Center, vec * 7.5f, ProjectileID.SapphireBolt, Projectile.damage, Projectile.knockBack, player.whoAmI);
				proj.CritChance = 0;
				proj.DamageType = DamageClass.Summon;
				proj.penetrate = 1;
				Projectile.ai[0] = 0;
			}
			Projectile.ai[1]++;
			Projectile.velocity.Y = 0.5f * (float)Math.Sin(Projectile.ai[1] / 30);
			var modPlayer = player.GetModPlayer<SapphireEnergyStaffPlayer>();
			if (player.dead)
			{
				modPlayer.SapphireEnergyAggregate = false;
			}
			if (modPlayer.SapphireEnergyAggregate)
			{
				Projectile.timeLeft = 2;
			}
			player.AddBuff(ModContent.BuffType<SapphireEnergyStaffBuff>(), 2);
		}
		public override bool MinionContactDamage()
		{
			return false;
		}
	}

	public class SapphireEnergyStaffBuff : ModBuff
	{
		public override string Texture => "DeusMod/Assets/Items/Weapon/Summon/PHM/SapphireEnergyStaff/SapphireEnergyStaffBuff";
		public override void SetStaticDefaults()
		{
			/*DisplayName.SetDefault("Sapphire Energy Aggregate");
			DisplayName.AddTranslation(7, "蓝玉能量聚合体");
			Description.SetDefault("The energy from sapphire is protecting you");
			Description.AddTranslation(7, "源自蓝玉的能量在保护你");*/
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			SapphireEnergyStaffPlayer modPlayer = player.GetModPlayer<SapphireEnergyStaffPlayer>();
			// 如果当前有属于玩家的僚机的弹幕
			if (player.ownedProjectileCounts[ModContent.ProjectileType<SapphireEnergyAggregate>()] > 0)
			{
				modPlayer.SapphireEnergyAggregate = true;
			}
			// 如果玩家取消了这个召唤物就让buff消失
			if (!modPlayer.SapphireEnergyAggregate)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
			else
			{
				// 无限buff时间
				player.buffTime[buffIndex] = 9999;
			}
		}
	}
}