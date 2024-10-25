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
            //�䵯ExampleSlashWeapon�������Ľ�������������������ʼ�����￴��ai[0]�����������
            Projectile.NewProjectile(source,player.Center,Vector2.Zero, ModContent.ProjectileType<ExampleSlashWeapon>(), damage,0,player.whoAmI,Main.rand.NextBool(2)?-1.9f:1.9f);
            return false;
        }
    }
    public class HeroSlashMethod: ModProjectile
    {
        /// <summary>
        /// ��Startingrot������������ӵ�����ʼ�㣬��ˮƽ����Ϊ��׼����������<br />
        /// ��rotation�������������ĸ�����ӵ���<br />
        /// ��Length����������������Ǹ������ϻӳ���Զ<br />
        /// ��thick��������ӵ��ĵ����ȣ��������Ӿ���������,0~1�Լ���<br />
        /// ��Yscale�������ն���ж�⣬������0~1���У���������<br />
        /// ��owner��isplayerproj���ʱ������whoami��ownernpc��Ϊ��ʱ���NPC.whoami<br />
        /// ��weapontexָ�����������Ļ���������ʲô���ʣ���סҪ�ǽ��������½ǰ�<br />
        /// ��ShouldDoNextSlashָ����������������Ϊ1�Ż�������<br />
        /// </summary>
        public static void Slash(bool IsPlayerProj,IEntitySource source,float rotation, float Startingrot, float Length, float Thick, float YScale,int ExtraSpeed = 0, int damage = 50, float Knockback = 0, int owner = 0,int ownerNPC = 0,Color color = default,Texture2D weaponTex = default,int ShouldDoNextSlash = 0)
        {
            //���ݸ�������ֵ���ɵ�Ļp/p1��p��hero��p1��hero2�����ǻ����������붼һ��
            if (IsPlayerProj)
            {
                var p = Projectile.NewProjectileDirect(source, Main.player[owner].Center, Vector2.UnitX * Length, ModContent.ProjectileType<HeroSlashMethod>(), damage, Knockback, owner, 0, rotation);
                p.rotation = Startingrot;
                (p.ModProjectile as HeroSlashMethod).Reverse = Startingrot > 0;//��������Ļ��reverse�ж���ʽһ��
                p.localAI[0] = Thick;p.localAI[1] = YScale;//localai[0]�Ǻ�ȣ�ai[1]��Y���
                if (color != default) (p.ModProjectile as HeroSlashMethod).c = color;//��ɫ
                p.extraUpdates = ExtraSpeed;
                (p.ModProjectile as HeroSlashMethod).ShouldDoNextSlash = ShouldDoNextSlash;
                if (weaponTex != default) (p.ModProjectile as HeroSlashMethod).t = weaponTex;//�ӽ�texture����������textureһ��
                
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
        //ai0��������Npc,ai1���������ٶȳ��Ⱦ���ն����Χ��localai0����ն����ȣ�localai1����ն����ȣ��Զ����������ն������λ
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
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)//���ж�
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
            if (Projectile.timeLeft >40)//��60f֮�ڻ��꽣������r��ת��
            {
                float r = MathHelper.Lerp(0.14f, 0, 1 - (Projectile.timeLeft - 40) / 60f);//�Ƕ����ӽ���
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
        public override bool PreDraw(ref Color lightColor)//������
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
                    Vector2 pos0 = Projectile.oldRot[i].ToRotationVector2() * Projectile.velocity.Length();//Բ�ܰ뾶����������Բ���о�
                    pos0.Y *= Projectile.localAI[1];//ѹ�⣬ͨ����Y��ѹ��ֵ
                    pos0 = pos0.RotatedBy(Projectile.ai[1]);
                    slash.Add(new VertexInfo2(ownerPos + pos0, new Vector3(1 - i / 40f, 0, 1), c * (1 - i/40f)));
                    Vector2 pos1 = Projectile.oldRot[i].ToRotationVector2() * Projectile.velocity.Length() * (1 - Projectile.localAI[0] + Projectile.localAI[0] * i / 40f);//Բ�ܰ뾶����������Բ��
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
                    Vector2 pos0 = Projectile.oldRot[i].ToRotationVector2() * Projectile.velocity.Length();//Բ�ܰ뾶
                    pos0.Y *= Projectile.localAI[1];
                    pos0 = pos0.RotatedBy(Projectile.ai[1]);
                    slash.Add(new VertexInfo2(ownerPos + pos0, new Vector3(1 - i / 40f, 0, 1), color));
                    Vector2 pos1 = Projectile.oldRot[i].ToRotationVector2() * Projectile.velocity.Length() * (1 - Projectile.localAI[0] + Projectile.localAI[0] * i / 40f);//Բ�ܰ뾶
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
    //�ֳֵ�����ͼ�񣬼�¼����ʱ�䲢���ж��Ƿ�ӳ���ս����
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
            //��ɫ�ֳֵ�Ļ��״̬��ai[0]���뽣�İѳֽǶȼ���
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
                Projectile.timeLeft = 2;//�ֳ�
            }
            Projectile.velocity *= 0;Projectile.Center = player.Center;//ά���ٶ�Ϊ0�����ĺ�����һ��

            //��������Իӳ��ˣ�
            if(Projectile.ai[1] < 30)
            {
                Vector2 P = Main.rand.NextVector2CircularEdge(50, 50);
                Dust.NewDust(player.Center + P, 0, 0, DustID.ShadowbeamStaff, -P.X / 5, -P.Y / 5, 0, default, 1.5f);
            }
            //30f���ڲ��ûӳ�����������
            if(Projectile.ai[1] == 30)
            {
                SoundEngine.PlaySound(SoundID.Item4);
                for(int i = 0; i < 30; i++)
                {
                    Vector2 P = Main.rand.NextVector2CircularEdge(50, 50);
                    Dust.NewDust(player.Center + P, 0, 0, DustID.ShadowbeamStaff,0,0, 0, default, 1.5f);
                }
            }
            //30fʱ����ԲȦ���Ӵ�����Իӽ���
            player.direction = Math.Sign(Main.MouseWorld.X - player.Center.X);
            base.AI();
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] >= 30)
            {
                Player player = Main.player[Projectile.owner];
                //�����������30f��ӽ�ʱ����Slash���������л��ƣ������������Ǹ������λ�þ����Ļӵ��Ƕȣ����ĸ�λ����Ԥ�����ʼ�Ƕ�
                HeroSlashMethod.Slash(true, Projectile.GetSource_FromAI(), (Main.MouseWorld - player.Center).ToRotation(), Projectile.ai[0], 250, 0.65f, 0.65f, 5,Projectile.damage, 5, player.whoAmI, 0, Color.Purple, TextureAssets.Projectile[Projectile.type].Value,0);
            }
            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            bool Reverse = Projectile.ai[0] > 0; //���ai[0]����ĳ�ʼ�Ƕȴ���0��ת
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
