using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DeusMod.Projs;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Collections.Generic;

namespace DeusMod.Items.Weapon.Melee.Spears.Vanilla
{
    public class SpearRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public int damage;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.Spear;
        }
        public override void SetDefaults(Item item)
        {
            item.damage = 16;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<SpearSlash>();
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline1 = new TooltipLine(Mod, "Tooltip3", "\"一把普通的长矛，但是依旧是退敌的利器。\"");
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SpearSlash>()] < 1)
            {
                var proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<SpearSlash>(), item.damage, item.knockBack, player.whoAmI);
            }
            else
            {
                foreach(Projectile proj in Main.projectile)
                {
                    if (proj.type == ModContent.ProjectileType<SpearSlash>() && proj.owner == player.whoAmI)
                        ((SpearSlash)proj.ModProjectile).SwordTexture = ModContent.Request<Texture2D>("DeusMod/Assets/Items/Weapon/Melee/Spear/PHM/AttackSpear").Value;
                }
            }
            if (normalend == true) //在一斩结束后开始计算后摇
            {
                timer++; //后摇计时器
                if (timer >= 60) //后摇超过60帧了
                {
                    normalend = false; //重置攻击段数
                }
            }
            else timer = 0; //后摇计时器不动
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 0)
            {
                timer = 0;
                if (normalend)
                {
                    phase++; //在60f的攻击后摇之内再次攻击的话，段数指示器加一，表示这一打是下一打
                    if (phase == 3)
                        phase = 0; //如果第三段打完则重置到第一段
                }
                else
                {
                    phase = 0; //60f后摇接受后攻击，则直接返回第一段
                }
                normalend = true;
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.type == ModContent.ProjectileType<SpearSlash>() && proj.owner == player.whoAmI && proj != null)
                    {
                        switch (phase)
                        {
                            case 0:
                                //((SpearSlash)proj.ModProjectile).Stinger_enterstats(1, 1.6f, false, 1f, 1f);
                                break;
                            case 1:
                                //((SpearSlash)proj.ModProjectile).Stinger_enterstats(1, 1.6f, false, 1f, 1f);
                                break;
                            case 2:
                                //((SpearSlash)proj.ModProjectile).Stinger_enterstats(1, 1.6f, true, 1.2f, 1.2f);
                                break;
                        }
                    }
                }
            }
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ThrowSpear>(), damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
    public class SpearSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "DeusMod/Assets/Items/Weapon/Melee/Spear/PHM/AttackSpear";
        public SpearSlash()
        {

        }
        public override void RegisterVariables()
        {
            SlashColor = Color.White;
        }
        public override void Appear()
        {
            throw new NotImplementedException();
        }
    }
    public class ThrowSpear : GlobalThrowSpear
    {
        public override string Texture => "DeusMod/Assets/Items/Weapon/Melee/Spear/PHM/AttackSpear";
        public override void SetDefaults()
        {
            base.SetDefaults();
            minvelocity = 4;
            maxfall = 0.5f;
            maxtimer = 240;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 1, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default(Color), 0.75f);
            }
        }
    }
}
