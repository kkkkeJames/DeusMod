using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using DeusMod;
using Terraria.Audio;

namespace DeusMod.Projs
{
    public class SlashTest : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.NightsEdge;
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.TerraBlade);
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.damage = 5000;
            Item.channel = true;
            base.SetDefaults();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //射弹ExampleSlashWeapon是蓄力的剑，武器攻击从蓄力开始，这里看到ai[0]纯粹是随机数
            Projectile.NewProjectile(source,player.Center,Vector2.Zero, ModContent.ProjectileType<ExampleSlashWeapon>(), damage,0,player.whoAmI,Main.rand.NextBool(2)?-1.9f:1.9f);
            return false;
        }
    }
    public class HeroSlashMethod: ModProjectile
    {
        /// <summary>
        /// ·Startingrot决定这个东西挥刀的起始点，以水平向右为基准啊，别搞错了<br />
        /// ·rotation决定这玩意往哪个方向挥刀。<br />
        /// ·Length决定这个东西能在那个方向上挥出多远<br />
        /// ·thick决定这个挥刀的刀光宽度，纯纯的视觉参数罢了,0~1自己调<br />
        /// ·Yscale决定这个斩击有多扁，控制在0~1就行，别整多了<br />
        /// ·owner是isplayerproj真的时候存玩家whoami，ownernpc是为假时存的NPC.whoami<br />
        /// ·weapontex指的是这个刀光的挥舞武器是什么材质，记住要是剑柄在左下角奥<br />
        /// ·ShouldDoNextSlash指的是连击数，至少为1才会有连击<br />
        /// </summary>
        public static void Slash(bool IsPlayerProj,IEntitySource source,float rotation, float Startingrot, float Length, float Thick, float YScale,int ExtraSpeed = 0, int damage = 50, float Knockback = 0, int owner = 0,int ownerNPC = 0,Color color = default,Texture2D weaponTex = default,int ShouldDoNextSlash = 0)
        {
            //根据各种输入值生成弹幕p/p1，p是hero，p1是hero2，但是基本绘制输入都一样
            if (IsPlayerProj)
            {
                var p = Projectile.NewProjectileDirect(source, Main.player[owner].Center, Vector2.UnitX * Length, ModContent.ProjectileType<HeroSlashMethod>(), damage, Knockback, owner, 0, rotation);
                p.rotation = Startingrot;
                (p.ModProjectile as HeroSlashMethod).Reverse = Startingrot > 0;//和武器弹幕的reverse判定方式一致
                p.localAI[0] = Thick;p.localAI[1] = YScale;//localai[0]是厚度，ai[1]是Y扁度
                if (color != default) (p.ModProjectile as HeroSlashMethod).c = color;//颜色
                p.extraUpdates = ExtraSpeed;
                (p.ModProjectile as HeroSlashMethod).ShouldDoNextSlash = ShouldDoNextSlash;
                if (weaponTex != default) (p.ModProjectile as HeroSlashMethod).t = weaponTex;//挥剑texture和蓄力剑的texture一致
                
            }
            if (!IsPlayerProj)
            {
                var p = Projectile.NewProjectileDirect(source, Main.npc[ownerNPC].Center, Vector2.UnitX * Length, ModContent.ProjectileType<HeroSlashMethod>(), damage, Knockback, 0, ownerNPC, rotation);
                p.rotation = Startingrot;
                (p.ModProjectile as HeroSlashMethod).Reverse = Startingrot > 0; 
                (p.ModProjectile as HeroSlashMethod).IsNPCproj = true;
                p.localAI[0] = Thick; p.localAI[1] = YScale; 
                if (color != default) (p.ModProjectile as HeroSlashMethod).c = color;
                p.extraUpdates = ExtraSpeed;
                
            }
        }
        //ai0决定主人Npc,ai1决定方向，速度长度决定斩击范围，localai0决定斩击厚度，localai1决定斩击扁度，自定义变量决定斩击正反位
        public bool Reverse = false;public bool IsNPCproj = false;public Color c = Color.White;public Texture2D t = null;public int ShouldDoNextSlash = 0;
        public override string Texture => "Terraria/Images/Extra_189";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
             base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.timeLeft =100;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -11;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1000;
            Projectile.noEnchantments = true;
            base.SetDefaults();
        }
        public override bool ShouldUpdatePosition()
        {
            return false ;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)//剑判定
        {
            float point = 0f;
            Vector2 endPoint = Projectile.Center + Projectile.velocity.Length() * Projectile.ai[1].ToRotationVector2();
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPoint, Projectile.velocity.Length() * Projectile.localAI[1], ref point);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 22; i++)
            {
                Dust d = Dust.NewDustDirect(target.position, target.width, target.height, DustID.PurpleTorch, 0, 0, 0, default, Main.rand.NextFloat(1.2f, 3f));
                d.velocity = (Projectile.ai[1] + Main.rand.NextFloat(-0.45f, 0.45f)).ToRotationVector2() * Main.rand.NextFloat(0.4f, 10);
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void AI()
        {
            if (Projectile.timeLeft >40)//在60f之内挥完剑，加上r来转剑
            {
                float r = MathHelper.Lerp(0.14f, 0, 1 - (Projectile.timeLeft - 40) / 60f);//角度增加渐变
                if (!Reverse)
                {
                    Projectile.rotation += r;
                }
                else
                {
                    Projectile.rotation -= r;
                }
            }
            if (!IsNPCproj)
            {
                Projectile.friendly = true;
                Projectile.DamageType = DamageClass.Melee;
            }
            /*if(Projectile.timeLeft == 60 && !IsNPCproj && ShouldDoNextSlash >=1)
            {
                Player player = Main.player[Projectile.owner];
                float rd = Main.rand.NextFloat(-0.4f, 0.4f);
                    HeroSlashMethod.Slash(true, Projectile.GetSource_FromAI(), (Main.MouseWorld - player.Center).ToRotation(),Reverse? -1.9f +rd: 1.9f + rd, 353, 0.45f, 0.35f, 5,Projectile.damage, 5, player.whoAmI, 0,c,t,ShouldDoNextSlash - 1);
                
            }*/
            base.AI();
        }
        public override bool PreDraw(ref Color lightColor)//画剑气
        {
            Vector2 ownerPos = Vector2.Zero;
            if (IsNPCproj)
            {
                ownerPos = Main.npc[(int)Projectile.ai[0]].Center - Main.screenPosition;
            }
            else
            {
                ownerPos = Main.player[Projectile.owner].Center - Main.screenPosition;
            }
            List<VertexInfo2> slash = new List<VertexInfo2>();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldRot[i] != 0)
                {
                    Vector2 pos0 = Projectile.oldRot[i].ToRotationVector2() * Projectile.velocity.Length();//圆周半径，剑气的外圆弧列举
                    pos0.Y *= Projectile.localAI[1];//压扁，通过乘Y轴压缩值
                    pos0 = pos0.RotatedBy(Projectile.ai[1]);
                    slash.Add(new VertexInfo2(ownerPos + pos0, new Vector3(1 - i / 40f, 0, 1), c * (1 - i/40f)));
                    Vector2 pos1 = Projectile.oldRot[i].ToRotationVector2() * Projectile.velocity.Length() * (1 - Projectile.localAI[0] + Projectile.localAI[0] * i / 40f);//圆周半径，剑气的内圆弧
                    pos1.Y *= Projectile.localAI[1];
                    pos1 = pos1.RotatedBy(Projectile.ai[1]);
                    slash.Add(new VertexInfo2(ownerPos + pos1, new Vector3(1 - i / 40f, 1, 1), c * (1 - i / 40f)));
                }
            }
            if (t != null)
            {
                Vector2 Directions = Projectile.rotation.ToRotationVector2();Directions.Y *= Projectile.localAI[1];Directions = Directions.RotatedBy(Projectile.ai[1]);
                float RealDirection = Directions.ToRotation();
                if (Reverse && Projectile.ai[1] > -MathHelper.Pi/2f && Projectile.ai[1] < MathHelper.Pi/2f)
                    Main.EntitySpriteDraw(t, ownerPos, null, Color.Purple, RealDirection + 0.785f + 1.5707f, new Vector2(t.Width, t.Height), 1, SpriteEffects.FlipHorizontally, 0);
                if (!Reverse && Projectile.ai[1] > -MathHelper.Pi / 2f && Projectile.ai[1] < MathHelper.Pi / 2f)
                    Main.EntitySpriteDraw(t, ownerPos, null, Color.Purple, RealDirection + 0.785f, new Vector2(0, t.Height), 1, SpriteEffects.None, 0);
                else if (Reverse && Projectile.ai[1] <= -MathHelper.Pi / 2f || Projectile.ai[1] >= MathHelper.Pi / 2f)
                    Main.EntitySpriteDraw(t, ownerPos, null, Color.Purple, RealDirection + 0.785f + 1.5707f, new Vector2(t.Width, t.Height), 1, SpriteEffects.FlipHorizontally, 0);
                else if (!Reverse && Projectile.ai[1] <= -MathHelper.Pi / 2f || Projectile.ai[1] >= MathHelper.Pi / 2f)
                    Main.EntitySpriteDraw(t, ownerPos, null, Color.Purple, RealDirection + 0.785f, new Vector2(0, t.Height), 1, SpriteEffects.None, 0);
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
    public class HeroSlash2 : ModProjectile
    {
        public bool Reverse = false; public bool IsNPCproj = false; public Color c = Color.White;
        public override string Texture => "Terraria/Images/Extra_193";
        //public override string Texture => "Tetrad/Textures/EX112";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.timeLeft =100;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -11;
            base.SetDefaults();
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if (Projectile.timeLeft > 40)
            {
                float r = MathHelper.Lerp(0.14f, 0, 1 - (Projectile.timeLeft - 40) / 60f);
                if (!Reverse)
                {
                    Projectile.rotation += r;
                }
                else
                {
                    Projectile.rotation -= r;
                }
            }

            base.AI();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 ownerPos = Vector2.Zero;
            if (IsNPCproj)
            {
                ownerPos = Main.npc[(int)Projectile.ai[0]].Center - Main.screenPosition;
            }
            else
            {
                ownerPos = Main.player[Projectile.owner].Center - Main.screenPosition;
            }
            List<VertexInfo2> slash = new List<VertexInfo2>();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldRot[i] != 0)
                {
                    Color color = new Color(18,3,47);
                    Vector2 pos0 = Projectile.oldRot[i].ToRotationVector2() * Projectile.velocity.Length();//圆周半径
                    pos0.Y *= Projectile.localAI[1];
                    pos0 = pos0.RotatedBy(Projectile.ai[1]);
                    slash.Add(new VertexInfo2(ownerPos + pos0, new Vector3(1 - i / 40f, 0, 1), color));
                    Vector2 pos1 = Projectile.oldRot[i].ToRotationVector2() * Projectile.velocity.Length() * (1 - Projectile.localAI[0] + Projectile.localAI[0] * i / 40f);//圆周半径
                    pos1.Y *= Projectile.localAI[1];
                    pos1 = pos1.RotatedBy(Projectile.ai[1]);
                    slash.Add(new VertexInfo2(ownerPos + pos1, new Vector3(1 - i / 40f, 1, 1), color));
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
    //手持的武器图像，记录蓄力时间并且判定是否挥出近战剑气
    public class ExampleSlashWeapon : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.NightsEdge;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;Projectile.timeLeft = 100;Projectile.friendly = false;Projectile.tileCollide = false;Projectile.ignoreWater = true;
            base.SetDefaults();
        }
        public override void AI()
        {
            //角色手持弹幕的状态，ai[0]参与剑的把持角度计算
            Projectile.ai[1]++;
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = 2; // Define the duration the projectile will exist in frames
            player.itemAnimation = 3;player.itemTime = 3;
            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id
            Projectile.rotation = Projectile.ai[0] + (Main.MouseWorld - player.Center).ToRotation();
            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }
            if (player.channel)
            {
                Projectile.timeLeft = 2;//手持
            }
            Projectile.velocity *= 0;Projectile.Center = player.Center;//维持速度为0，中心和人物一致

            //蓄力后可以挥出了！
            if(Projectile.ai[1] < 30)
            {
                Vector2 P = Main.rand.NextVector2CircularEdge(50, 50);
                Dust.NewDust(player.Center + P, 0, 0, DustID.ShadowbeamStaff, -P.X / 5, -P.Y / 5, 0, default, 1.5f);
            }
            //30f以内不让挥出，产生粒子
            if(Projectile.ai[1] == 30)
            {
                SoundEngine.PlaySound(SoundID.Item4);
                for(int i = 0; i < 30; i++)
                {
                    Vector2 P = Main.rand.NextVector2CircularEdge(50, 50);
                    Dust.NewDust(player.Center + P, 0, 0, DustID.ShadowbeamStaff,0,0, 0, default, 1.5f);
                }
            }
            //30f时产生圆圈粒子代表可以挥剑了
            player.direction = Math.Sign(Main.MouseWorld.X - player.Center.X);
            base.AI();
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] >= 30)
            {
                Player player = Main.player[Projectile.owner];
                //如果蓄力大于30f则挥剑时触发Slash函数，进行绘制，第三个输入是根据鼠标位置决定的挥刀角度，第四个位置是预设的起始角度
                HeroSlashMethod.Slash(true, Projectile.GetSource_FromAI(), (Main.MouseWorld - player.Center).ToRotation(), Projectile.ai[0], 250, 0.65f, 0.65f, 5,Projectile.damage, 5, player.whoAmI, 0, Color.Purple, TextureAssets.Projectile[Projectile.type].Value,0);
            }
            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            bool Reverse = Projectile.ai[0] > 0; //如果ai[0]代表的初始角度大于0则反转
            if(!Reverse)
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value,Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + 0.785f, new Vector2(0, TextureAssets.Projectile[Projectile.type].Value.Height), 1, SpriteEffects.None, 0);
            else
            {
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + 0.785f + 1.5707f, new Vector2 (TextureAssets.Projectile[Projectile.type].Value.Width, TextureAssets.Projectile[Projectile.type].Value.Height), 1, SpriteEffects.FlipHorizontally, 0);
            }
            return false;
        }
    }
}
