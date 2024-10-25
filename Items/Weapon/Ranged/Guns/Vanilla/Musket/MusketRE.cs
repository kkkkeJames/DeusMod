using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using System;
using System.Collections.Generic;
using DeusMod.Dusts;
using DeusMod.NPCs;
using DeusMod.Projs;
using DeusMod.Core.Systems;
using DeusMod.Core;
using Terraria.UI;
using Terraria.GameContent;

namespace DeusMod.Items.Weapon.Ranged.Guns.Vanilla.Musket
{
    public class MusketRE : GlobalGun
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            //这个GlobalItem只改动火枪
            return entity.type == ItemID.Musket;
        }
        public override void SetDefaults(Item item)//所有的重置枪械的装载弹和射击数据在globalgun里
        {
            item.autoReuse = false;
            reloadTimeMax = item.useTime * 3;
            bulletnum = 0;
            bulletcapacity = 1;
            base.SetDefaults(item);
        }
        public override void NeedReload(Item item)
        {
            if (Main.mouseRight) needreload = true;
            else needreload = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var line1 = new TooltipLine(Mod, "Tooltip1", "Mod Shooting: Hold mouse right key to aim bullets of higher damage");
            var line2 = new TooltipLine(Mod, "Tooltip2", "This weapon is banned in a period after mod shooting");
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "模组射击：长按右键以瞄准射击高伤害的特殊子弹");
            var Cline2 = new TooltipLine(Mod, "Tooltip2", "模组射击后一段时间内该武器不可使用");
            tooltips.Add(Cline1);
            tooltips.Add(Cline2);
        }
        //后坐力/换弹动画
        public override void HoldItem(Item item, Player player)
        {
            reloadTimeMax = item.useTime * 3;
            base.HoldItem(item, player);
        }
        public override void SetRecoil(Item item, Player player)
        {
            int recoiltime = item.useTime / 8;
            if (useTime <= recoiltime) player.itemRotation -= player.direction * 0.1f;
            else if (useTime <= recoiltime * 2) player.itemRotation += player.direction * 0.1f;
        }
        public override bool CanUseItem(Item item, Player player)
        {
            if (reloadTime > 0) return false; //换弹时非模组射击也无法使用
            return base.CanUseItem(item, player);
        }
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (Main.mouseRight) damage = (int)(damage * 3.6f);
            base.ModifyShootStats(item, player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }
        public override void ModShooting(Item item)
        {
            bulletnum -= 1;
            reloadTime = reloadTimeMax;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            base.Shoot(item, player, source, position, velocity, type, damage, knockback);
            float rot = player.direction == 1 ? 0 : (float)Math.PI;
            Dust Gunfire = Dust.NewDustPerfect(position + new Vector2(item.width, 0).RotatedBy(player.itemRotation + rot), ModContent.DustType<Gunfire_L>());
            Gunfire.rotation = player.itemRotation + rot;
            CameraSystem.FixedAngleShake += 24;
            CameraSystem.ShakeAngle = player.itemRotation + rot;
            CameraSystem.ShakeDecrease = 8;
            //player.GetModPlayer<ScreenShake>().ScreenShakeShort(24, (float)(Math.Atan(velocity.Y / velocity.X) + Math.PI));
            used = true;
            return true;
        }
    }
    public class MusketModPlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            if (Player.HeldItem.type == ItemID.Musket) MusketModUI.visible = true;
            else MusketModUI.visible = false;
        }
    }
    public class MusketModUI : SmartUIState
    {
        public static bool visible = false;
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        public override bool Visible => visible;
        public static MusketModBar MusketModBar = new MusketModBar();
        public override void OnInitialize()
        {
            MusketModBar.Width.Set(8, 0f);
            MusketModBar.Height.Set(16, 0f);
            MusketModBar.Left.Set(Main.screenWidth / 2, 0f);
            MusketModBar.Top.Set(Main.screenWidth / 4 - 16, 0f);
            Append(MusketModBar);
            base.OnInitialize();
        }

    }
    public class MusketModBar : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 center = new Vector2(GetDimensions().X, GetDimensions().Y);
            Texture2D backTex = ModContent.Request<Texture2D>("DeusMod/Assets/UI/SniperModBackgroundUI").Value;
            Texture2D barTex = ModContent.Request<Texture2D>("DeusMod/Assets/UI/SniperModUI").Value;
            
            //绘制背景
            spriteBatch.Draw(backTex, center, backTex.Frame(), Color.Black, 0, new Vector2(backTex.Width / 2, backTex.Height / 2), 1f, SpriteEffects.None, 0f);

            float chargePercent = (Main.LocalPlayer.HeldItem.GetGlobalItem<MusketRE>().reloadTimeMax - Main.LocalPlayer.HeldItem.GetGlobalItem<MusketRE>().reloadTime) / (float)(Main.LocalPlayer.HeldItem.GetGlobalItem<MusketRE>().reloadTimeMax);
            Rectangle barSource = new Rectangle(0, 0, (int)(chargePercent * barTex.Width), barTex.Height);
            spriteBatch.Draw(barTex, center, barSource, Color.White, 0, new Vector2(barTex.Width / 2, barTex.Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}
