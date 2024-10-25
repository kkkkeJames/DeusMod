using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using DeusMod.Dusts;
using DeusMod.Buffs;
using Terraria.DataStructures;
using DeusMod.Projs;
using DeusMod.Core.Systems;

namespace DeusMod.NPCs.Vanilla.VanillaBoss.KingSlime
{
    public class KingSlimeRE : GlobalNPC
    {
		public float KSnospellcard = 0;
		public float KSspellcard = 0;
		public int KSspellcardtype = 0;
		public static bool KSstopspell = false;
		public static Vector2 squarecenter;
		public static Player target;
		public override bool InstancePerEntity => true;
		public override bool PreAI(NPC npc)
        {
			if (npc.type == NPCID.KingSlime)
			{
				//ai[0]记录跳起来后落地时的时间间隔，从负数加到0的时候再次起跳并重设ai[0]
				//ai[1]代表boss行为，0到3代表不同的跳跃数，5代表瞬移消失，6代表瞬移出现
				//ai[2]代表滞空时间，滞空时间大于360固定瞬移
				//ai[3]代表最大生命值
				//localai[0]跳起来时增大，落地后减少，但不会小于0
				//localai[1]和localai[2]记录瞬移点
				//localai[3]代表数据初始化，0代表刚生成，1代表数据已初始化
				float bosssizefix = 1f;
				bool isdodge = false;
				bool isdodge2 = false;
				npc.aiAction = 0;
				if (npc.ai[3] == 0f && npc.life > 0)
				{
					npc.ai[3] = (float)npc.lifeMax;
				}
				if (npc.localAI[3] == 0f && Main.netMode != 1)
				{
					npc.ai[0] = -100f;
					npc.localAI[3] = 1f;
					npc.TargetClosest(true);
					npc.netUpdate = true;
				}
				int despawnrange;
				if (KSspellcard == 0)
					despawnrange = 500;
				else despawnrange = 5000;
				if (Main.player[npc.target].dead || Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) / 16f > (float)despawnrange)
				{
					npc.TargetClosest(true); //超过距离或玩家死亡重新找玩家
					if (Main.player[npc.target].dead || Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) / 16f > (float)despawnrange)
					{
						npc.EncourageDespawn(10); //没有目标则消失
						if (Main.player[npc.target].Center.X < npc.Center.X) //根据玩家调整面向方位
						{
							npc.direction = 1;
						}
						else
						{
							npc.direction = -1;
						}
					}
				}
				if (!Main.player[npc.target].dead && npc.timeLeft > 10 && npc.ai[2] >= 300f && npc.ai[1] < 5f && npc.velocity.Y == 0f) //在地面上，寻找合适的瞬移点
				{
					npc.ai[2] = 0f;
					npc.ai[0] = 0f;
					npc.ai[1] = 5f; //也就是可以瞬移的意思
					if (Main.netMode != 1)
					{
						npc.TargetClosest(false);
						Point npccoord = npc.Center.ToTileCoordinates(); //具体到坐标的史王位置
						Point playercoord = Main.player[npc.target].Center.ToTileCoordinates(); //具体到坐标的玩家位置
						Vector2 distance = Main.player[npc.target].Center - npc.Center; //玩家和史王中心的直线距离
						int dodgetimer = 0; //瞬移计时器
						bool dodgeflag = false;
						if (npc.localAI[0] >= 360f || distance.Length() > 2000f) //高度差过高超过6秒或者距离太远
						{
							if (npc.localAI[0] >= 360f)
							{
								npc.localAI[0] = 360f;
							}
							dodgeflag = true; //如果距离远了就true
							dodgetimer = 100;
						}
						while (!dodgeflag && dodgetimer < 100) //瞬移的两个条件都没满足，也就是没找到合适的瞬移点，要么找够100个点，要么提前触发flag
						{
							dodgetimer++; //计时器开始计时
							int playersrandcoordX = Main.rand.NextBool() ? Main.rand.Next(playercoord.X - 10, playercoord.X - 5) : Main.rand.Next(playercoord.X + 6, playercoord.X + 11);
							//int playersrandcoordX = Main.rand.Next(playercoord.X - 10, playercoord.X + 11); //玩家随机的X轴附近坐标
							int playersrandcoordY = Main.rand.Next(playercoord.Y - 10, playercoord.Y + 1); //玩家随机的Y轴附近坐标
							if ((playersrandcoordY < playercoord.Y - 7 || playersrandcoordY > playercoord.Y + 7 || playersrandcoordX < playercoord.X - 7 || playersrandcoordX > playercoord.X + 7) && (playersrandcoordY < npccoord.Y || playersrandcoordY > npccoord.Y || playersrandcoordX < npccoord.X || playersrandcoordX > npccoord.X) && !Main.tile[playersrandcoordX, playersrandcoordY].HasUnactuatedTile) //一些瞬移判定
							{
								int playersolidtimer = 0;
								bool playertilesolidflag = !Main.tile[playersrandcoordX, playersrandcoordY].HasTile && Main.tileSolid[(int)Main.tile[playersrandcoordX, playersrandcoordY].TileType] && !Main.tileSolidTop[(int)Main.tile[playersrandcoordX, playersrandcoordY].TileType]; //玩家的格子是否坚实
								if (playertilesolidflag)
								{
									playersolidtimer = 1;
								}
								else
								{
									while (playersolidtimer < 150 && playersrandcoordY + playersolidtimer < Main.maxTilesY)
									{
										int num249 = playersrandcoordY + playersolidtimer;
										bool plrtilesflag2 = !Main.tile[playersrandcoordX, num249].HasTile && Main.tileSolid[(int)Main.tile[playersrandcoordX, num249].TileType] && !Main.tileSolidTop[(int)Main.tile[playersrandcoordX, num249].TileType];
										if (plrtilesflag2)
										{
											playersolidtimer--;
											break;
										}
										int num = playersolidtimer;
										playersolidtimer = num + 1;
									}
								}
								playersrandcoordY += playersolidtimer;
								bool avoidlava = true;
								if (avoidlava && Main.tile[playersrandcoordX, playersrandcoordY].LiquidType == LiquidID.Lava)
								{
									avoidlava = false;
								}
								if (avoidlava && !Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
								{
									avoidlava = false;
								}
								if (avoidlava)
								{
									npc.localAI[1] = (float)(playersrandcoordX * 16 + 8);
									npc.localAI[2] = (float)(playersrandcoordY * 16 + 16);
									break;
								}
							}

						}
						if (dodgetimer >= 100) //瞬移点找到了，把史王的屁股落点标记出来
						{
							Vector2 bottom = Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].Bottom;
							npc.localAI[1] = bottom.X;
							npc.localAI[2] = bottom.Y;
						}
					}
				}
				if (!Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0) || Math.Abs(npc.Top.Y - Main.player[npc.target].Bottom.Y) > 160f) //Y轴距离过远的判定，防止玩家高空遛史
				{
					npc.ai[2] += 1f; //保证瞬移间隔不会太长
					if (Main.netMode != 1)
					{
						npc.localAI[0] += 1f; //满足条件则计时器增加
					}
				}
				else if (Main.netMode != 1)
				{
					npc.localAI[0] -= 1f; //不满足则计时器减
					if (npc.localAI[0] < 0f)
					{
						npc.localAI[0] = 0f;
					}
				}
				if (npc.timeLeft < 10 && (npc.ai[0] != 0f || npc.ai[1] != 0f)) //消失判定
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.netUpdate = true;
					isdodge = false;
				}
				Dust dust;
				if (KSspellcard == 0)
				{
					if (npc.ai[1] == 5f) //瞬移时史王大小缩小，消失
					{
						isdodge = true;
						npc.aiAction = 1;
						npc.ai[0]++;
						bosssizefix = MathHelper.Clamp((60f - npc.ai[0]) / 60f, 0f, 1f);
						bosssizefix = 0.5f + bosssizefix * 0.5f;
						if (npc.ai[0] >= 60f)
						{
							isdodge2 = true;
						}
						if (npc.ai[0] == 60f)
						{
							Gore.NewGore(npc.GetSource_FromAI(), npc.Center + new Vector2(-40f, (float)(-(float)npc.height / 2)), npc.velocity, 734, 1f); //王冠掉落
						}
						if (npc.ai[0] >= 60f && Main.netMode != 1) //一秒钟后出现在目标地点
						{
							npc.Bottom = new Vector2(npc.localAI[1], npc.localAI[2]); //屁股先到位
							npc.ai[1] = 6f; //本体的出现AI
							npc.ai[0] = 0f;
							npc.netUpdate = true;
						}
						if (Main.netMode == 1 && npc.ai[0] >= 120f) //联机虽然不知道为什么，但是是两秒钟
						{
							npc.ai[1] = 6f;
							npc.ai[0] = 0f;
						}
						if (!isdodge2) //史王消失时的粒子
						{
							for (int i = 0; i < 10; i++)
							{
								int d = Dust.NewDust(npc.position + Vector2.UnitX * -20f, npc.width + 40, npc.height, 4, npc.velocity.X, npc.velocity.Y, 150, new Color(78, 136, 255, 80), 2f);
								Main.dust[d].noGravity = true;
								dust = Main.dust[d];
								dust.velocity *= 0.5f;
							}
						}
					}
					else if (npc.ai[1] == 6f) //史王变大并出现
					{
						isdodge = true;
						npc.aiAction = 0;
						npc.ai[0]++;
						bosssizefix = MathHelper.Clamp(npc.ai[0] / 30f, 0f, 1f);
						bosssizefix = 0.5f + bosssizefix * 0.5f;
						if (npc.ai[0] >= 30f && Main.netMode != 1) //恢复正常行动
						{
							npc.ai[1] = 0f;
							npc.ai[0] = 0f;
							npc.netUpdate = true;
							npc.TargetClosest(true);
						}
						if (Main.netMode == 1 && npc.ai[0] >= 60f)
						{
							npc.ai[1] = 0f;
							npc.ai[0] = 0f;
							npc.TargetClosest(true);
						}
						for (int i = 0; i < 10; i++) //出现时的粒子
						{
							int d = Dust.NewDust(npc.position + Vector2.UnitX * -20f, npc.width + 40, npc.height, 4, npc.velocity.X, npc.velocity.Y, 150, new Color(78, 136, 255, 80), 2f);
							Main.dust[d].noGravity = true;
							dust = Main.dust[d];
							dust.velocity *= 2f;
						}
					}
					npc.dontTakeDamage = (npc.hide = isdodge2);
					if (npc.velocity.Y == 0f) //史王跳跃，ai[0]决定横向移动速度，ai[1]决定跳跃的段数
					{
						npc.velocity.X *= 0.8f;
						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1) //把史王的X轴速度在极低时直接降到零
						{
							npc.velocity.X = 0f;
						}
						if (!isdodge) //可以跳跃的判定
						{
							npc.ai[0] += 2f; //这个ai[0]是控制每一跳的间隔的
							if ((double)npc.life <= (double)npc.lifeMax * 0.8) //不同生命值下的X轴速度决定值
							{
								npc.ai[0] += 1f;
							}
							if ((double)npc.life <= (double)npc.lifeMax * 0.6)
							{
								npc.ai[0] -= 1f;
							}
							if ((double)npc.life <= (double)npc.lifeMax * 0.3)
							{
								npc.ai[0] -= 0f;
							}
							if (npc.ai[0] >= 0f)
							{
								npc.netUpdate = true;
								npc.TargetClosest(true);
								if (npc.ai[1] == 3f) //不同ai[1]下的速度
								{
									if ((double)npc.life > (double)npc.lifeMax * 0.6)
									{
										npc.velocity.Y = -13f;
										npc.velocity.X += 3.5f * (float)npc.direction;
										npc.ai[0] = -200f;
										npc.ai[1] = 0f;
									}
									else
									{
										npc.velocity.Y = -17f;
										npc.velocity.X += 12f * (float)npc.direction;
										npc.ai[0] = -200f;
										npc.ai[1] = 0f;
									}
								}
								else if (npc.ai[1] == 2f)
								{
									if ((double)npc.life > (double)npc.lifeMax * 0.6)
									{
										npc.velocity.Y = -6f;
										npc.velocity.X += 4.5f * (float)npc.direction;
										npc.ai[0] = -120f;
										npc.ai[1] += 1f;
									}
									else
									{
										npc.velocity.Y = -12f;
										npc.velocity.X += 9f * (float)npc.direction;
										npc.ai[0] = -120f;
										npc.ai[1] += 1f;
									}
								}
								else
								{
									if ((double)npc.life > (double)npc.lifeMax * 0.6)
									{
										npc.velocity.Y = -8f;
										npc.velocity.X += 4f * (float)npc.direction;
										npc.ai[0] = -120f;
										npc.ai[1] += 1f;
									}
									else
									{
										npc.velocity.Y = -16f;
										npc.velocity.X += 10f * (float)npc.direction;
										npc.ai[0] = -120f;
										npc.ai[1] += 1f;
									}
								}
							}
							else if (npc.ai[0] >= -30f)
							{
								npc.aiAction = 1;
							}
						}
					}
					else if (npc.target < 255 && ((npc.direction == 1 && npc.velocity.X < 3f) || (npc.direction == -1 && npc.velocity.X > -3f))) //横向速度的加减值
					{
						if ((npc.direction == -1 && (double)npc.velocity.X < 0.1) || (npc.direction == 1 && (double)npc.velocity.X > -0.1))
						{
							npc.velocity.X += 0.2f * (float)npc.direction;
						}
						else
						{
							npc.velocity.X *= 0.93f;
						}
					}
					else
					{
						//当主动滞空时投下史莱姆弹幕轰炸
						//根据原理，当ai[2]或者localai[0]为20的整数倍时投下弹幕
						if ((double)npc.life <= (double)npc.lifeMax * 0.6 && npc.ai[2] % 20 == 0 && npc.ai[2] != 0 && Math.Abs(npc.velocity.X) >= 8 && (!Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0) || Math.Abs(npc.Top.Y - Main.player[npc.target].Bottom.Y) > 160f))
						{
							if (Main.rand.NextBool(3))
								Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<VolatileGel>(), npc.damage / 4, 0);
							else
								Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BurstGel>(), npc.damage / 4, 0);
						}
					}
				}

				if ((double)npc.life <= (double)npc.lifeMax * 0.3 && KSnospellcard == 0 && npc.velocity.Y == 0f && !isdodge) //低于百分之30就开始甩符卡
				{
					KSspellcard++;
					npc.aiAction = 1;
					npc.velocity = Vector2.Zero;
					npc.ai[1] = 0f;
					npc.dontTakeDamage = true;
					if (KSspellcard == 1) //史莱姆限制框
					{
						KSspellcardtype = Main.rand.Next(0, 3);
						squarecenter = Main.player[npc.target].Center;
						Projectile.NewProjectileDirect(npc.GetSource_FromThis(), Main.player[npc.target].Center, Vector2.Zero, ModContent.ProjectileType<KSRestriction>(), 10000, 0, 255);
					}
					if (KSspellcardtype == 0)
					{
						if (KSspellcard == 120)
						{
							npc.ai[0] = Main.rand.Next(-20, 20);
							Vector2[] pos =
							{new Vector2(-1, -4),
							new Vector2(0, -4),
							new Vector2(1, -4),
							new Vector2(-1, -2),
							new Vector2(0, -2),
							new Vector2(1, -2),
							new Vector2(-4, 0),
							new Vector2(-3, 0),
							new Vector2(-2, 0),
							new Vector2(-1, 0),
							new Vector2(0, 0),
							new Vector2(1, 0),
							new Vector2(2, 0),
							new Vector2(3, 0),
							new Vector2(4, 0),
							new Vector2(-1, 2),
							new Vector2(0, 2),
							new Vector2(1, 2),
							new Vector2(-1, 4),
							new Vector2(0, 4),
							new Vector2(1, 4),
							new Vector2(-1, 6),
							new Vector2(0, 6),
							new Vector2(1, 6),
							new Vector2(0, 8)
							};
							Vector2[] rotnspeed = 
							{new Vector2(0.1f, 0), 
							new Vector2(0, 0), 
							new Vector2(-0.1f, 0), 
							new Vector2(0.1f, 0.2f), 
							new Vector2(0, 0.2f), 
							new Vector2(-0.1f, 0.2f), 
							new Vector2(0.4f, 0.4f), 
							new Vector2(0.3f, 0.4f), 
							new Vector2(0.2f, 0.4f), 
							new Vector2(0.1f, 0.4f),
							new Vector2(0, 0.4f),
							new Vector2(-0.1f, 0.4f),
							new Vector2(-0.2f, 0.4f),
							new Vector2(-0.3f, 0.4f),
							new Vector2(-0.4f, 0.4f),
							new Vector2(0.1f, 0.6f),
							new Vector2(0, 0.6f),
							new Vector2(-0.1f, 0.6f),
							new Vector2(0.1f, 0.8f),
							new Vector2(0, 0.8f),
							new Vector2(-0.1f, 0.8f),
							new Vector2(0.1f, 1),
							new Vector2(0, 1),
							new Vector2(-0.1f, 1),
							new Vector2(0, 1.2f)};
							for (int i = 0; i <= 24; i++)
							{
								Projectile.NewProjectileDirect(npc.GetSource_FromThis(), squarecenter + pos[i].RotatedBy(-Math.PI / 4) * 30 + new Vector2(-360 + npc.ai[0], -360), Vector2.Zero, ModContent.ProjectileType<BossSlimyThrowingKnife>(), npc.damage / 8, 0, Main.player[npc.target].whoAmI, rotnspeed[i].Y, (float)Math.PI * 3 / 4 + rotnspeed[i].X);
							}
						}
						if (KSspellcard == 180)
						{
							Projectile.NewProjectileDirect(npc.GetSource_FromThis(), squarecenter, Vector2.Zero, ModContent.ProjectileType<TargetedLineWarn>(), 0, 0, Main.player[npc.target].whoAmI, (float)Math.PI / 4, npc.ai[0]);
							TargetedLineWarn.color = Color.Blue;
							TargetedLineWarn.target = null;
						}
						if (KSspellcard == 390)
						{
							npc.ai[0] = Main.rand.Next(-20, 20);
							Vector2[] pos =
							{new Vector2(-1, -4),
							new Vector2(0, -4),
							new Vector2(1, -4),
							new Vector2(-1, -2),
							new Vector2(0, -2),
							new Vector2(1, -2),
							new Vector2(-4, 0),
							new Vector2(-3, 0),
							new Vector2(-2, 0),
							new Vector2(-1, 0),
							new Vector2(0, 0),
							new Vector2(1, 0),
							new Vector2(2, 0),
							new Vector2(3, 0),
							new Vector2(4, 0),
							new Vector2(-1, 2),
							new Vector2(0, 2),
							new Vector2(1, 2),
							new Vector2(-1, 4),
							new Vector2(0, 4),
							new Vector2(1, 4),
							new Vector2(-1, 6),
							new Vector2(0, 6),
							new Vector2(1, 6),
							new Vector2(0, 8)
							};
							Vector2[] rotnspeed =
							{new Vector2(0.1f, 0),
							new Vector2(0, 0),
							new Vector2(-0.1f, 0),
							new Vector2(0.1f, 0.2f),
							new Vector2(0, 0.2f),
							new Vector2(-0.1f, 0.2f),
							new Vector2(0.4f, 0.4f),
							new Vector2(0.3f, 0.4f),
							new Vector2(0.2f, 0.4f),
							new Vector2(0.1f, 0.4f),
							new Vector2(0, 0.4f),
							new Vector2(-0.1f, 0.4f),
							new Vector2(-0.2f, 0.4f),
							new Vector2(-0.3f, 0.4f),
							new Vector2(-0.4f, 0.4f),
							new Vector2(0.1f, 0.6f),
							new Vector2(0, 0.6f),
							new Vector2(-0.1f, 0.6f),
							new Vector2(0.1f, 0.8f),
							new Vector2(0, 0.8f),
							new Vector2(-0.1f, 0.8f),
							new Vector2(0.1f, 1),
							new Vector2(0, 1),
							new Vector2(-0.1f, 1),
							new Vector2(0, 1.2f)};
							for (int i = 0; i <= 24; i++)
							{
								Projectile.NewProjectileDirect(npc.GetSource_FromThis(), squarecenter + pos[i].RotatedBy(Math.PI / 4) * 30 + new Vector2(360 + npc.ai[0], -360), Vector2.Zero, ModContent.ProjectileType<BossSlimyThrowingKnife>(), npc.damage / 8, 0, Main.player[npc.target].whoAmI, rotnspeed[i].Y, -(float)Math.PI * 3 / 4 + rotnspeed[i].X);
							}
						}
						if (KSspellcard == 450)
						{
							Projectile.NewProjectileDirect(npc.GetSource_FromThis(), squarecenter, Vector2.Zero, ModContent.ProjectileType<TargetedLineWarn>(), 0, 0, Main.player[npc.target].whoAmI, (float)Math.PI * 3 / 4, npc.ai[0]);
							TargetedLineWarn.color = Color.Blue;
							TargetedLineWarn.target = null;
						}
						if (KSspellcard == 660)
						{
							npc.ai[0] = Main.rand.Next(-20, 20);
							Vector2[] pos =
							{new Vector2(-1, -4),
							new Vector2(0, -4),
							new Vector2(1, -4),
							new Vector2(-1, -2),
							new Vector2(0, -2),
							new Vector2(1, -2),
							new Vector2(-4, 0),
							new Vector2(-3, 0),
							new Vector2(-2, 0),
							new Vector2(-1, 0),
							new Vector2(0, 0),
							new Vector2(1, 0),
							new Vector2(2, 0),
							new Vector2(3, 0),
							new Vector2(4, 0),
							new Vector2(-1, 2),
							new Vector2(0, 2),
							new Vector2(1, 2),
							new Vector2(-1, 4),
							new Vector2(0, 4),
							new Vector2(1, 4),
							new Vector2(-1, 6),
							new Vector2(0, 6),
							new Vector2(1, 6),
							new Vector2(0, 8)
							};
							Vector2[] rotnspeed =
							{new Vector2(0.1f, 0),
							new Vector2(0, 0),
							new Vector2(-0.1f, 0),
							new Vector2(0.1f, 0.2f),
							new Vector2(0, 0.2f),
							new Vector2(-0.1f, 0.2f),
							new Vector2(0.4f, 0.4f),
							new Vector2(0.3f, 0.4f),
							new Vector2(0.2f, 0.4f),
							new Vector2(0.1f, 0.4f),
							new Vector2(0, 0.4f),
							new Vector2(-0.1f, 0.4f),
							new Vector2(-0.2f, 0.4f),
							new Vector2(-0.3f, 0.4f),
							new Vector2(-0.4f, 0.4f),
							new Vector2(0.1f, 0.6f),
							new Vector2(0, 0.6f),
							new Vector2(-0.1f, 0.6f),
							new Vector2(0.1f, 0.8f),
							new Vector2(0, 0.8f),
							new Vector2(-0.1f, 0.8f),
							new Vector2(0.1f, 1),
							new Vector2(0, 1),
							new Vector2(-0.1f, 1),
							new Vector2(0, 1.2f)};
							for (int i = 0; i <= 24; i++)
							{
								Projectile.NewProjectileDirect(npc.GetSource_FromThis(), squarecenter + pos[i].RotatedBy(Math.PI * 3 / 4) * 30 + new Vector2(360 + npc.ai[0], 360), Vector2.Zero, ModContent.ProjectileType<BossSlimyThrowingKnife>(), npc.damage / 8, 0, Main.player[npc.target].whoAmI, rotnspeed[i].Y, -(float)Math.PI / 4 + rotnspeed[i].X);
							}
						}
						if (KSspellcard == 720)
						{
							Projectile.NewProjectileDirect(npc.GetSource_FromThis(), squarecenter, Vector2.Zero, ModContent.ProjectileType<TargetedLineWarn>(), 0, 0, Main.player[npc.target].whoAmI, -(float)Math.PI * 3 / 4, npc.ai[0]);
							TargetedLineWarn.color = Color.Blue;
							TargetedLineWarn.target = null;
						}
						if (KSspellcard == 930)
						{
							npc.ai[0] = Main.rand.Next(-20, 20);
							Vector2[] pos =
							{new Vector2(-1, -4),
							new Vector2(0, -4),
							new Vector2(1, -4),
							new Vector2(-1, -2),
							new Vector2(0, -2),
							new Vector2(1, -2),
							new Vector2(-4, 0),
							new Vector2(-3, 0),
							new Vector2(-2, 0),
							new Vector2(-1, 0),
							new Vector2(0, 0),
							new Vector2(1, 0),
							new Vector2(2, 0),
							new Vector2(3, 0),
							new Vector2(4, 0),
							new Vector2(-1, 2),
							new Vector2(0, 2),
							new Vector2(1, 2),
							new Vector2(-1, 4),
							new Vector2(0, 4),
							new Vector2(1, 4),
							new Vector2(-1, 6),
							new Vector2(0, 6),
							new Vector2(1, 6),
							new Vector2(0, 8)
							};
							Vector2[] rotnspeed =
							{new Vector2(0.1f, 0),
							new Vector2(0, 0),
							new Vector2(-0.1f, 0),
							new Vector2(0.1f, 0.2f),
							new Vector2(0, 0.2f),
							new Vector2(-0.1f, 0.2f),
							new Vector2(0.4f, 0.4f),
							new Vector2(0.3f, 0.4f),
							new Vector2(0.2f, 0.4f),
							new Vector2(0.1f, 0.4f),
							new Vector2(0, 0.4f),
							new Vector2(-0.1f, 0.4f),
							new Vector2(-0.2f, 0.4f),
							new Vector2(-0.3f, 0.4f),
							new Vector2(-0.4f, 0.4f),
							new Vector2(0.1f, 0.6f),
							new Vector2(0, 0.6f),
							new Vector2(-0.1f, 0.6f),
							new Vector2(0.1f, 0.8f),
							new Vector2(0, 0.8f),
							new Vector2(-0.1f, 0.8f),
							new Vector2(0.1f, 1),
							new Vector2(0, 1),
							new Vector2(-0.1f, 1),
							new Vector2(0, 1.2f)};
							for (int i = 0; i <= 24; i++)
							{
								Projectile.NewProjectileDirect(npc.GetSource_FromThis(), squarecenter + pos[i].RotatedBy(-Math.PI * 3 / 4) * 30 + new Vector2(-360 + npc.ai[0], 360), Vector2.Zero, ModContent.ProjectileType<BossSlimyThrowingKnife>(), npc.damage / 8, 0, Main.player[npc.target].whoAmI, rotnspeed[i].Y, (float)Math.PI / 4 + rotnspeed[i].X);
							}
						}
						if (KSspellcard == 990)
						{
							Projectile.NewProjectileDirect(npc.GetSource_FromThis(), squarecenter, Vector2.Zero, ModContent.ProjectileType<TargetedLineWarn>(), 0, 0, Main.player[npc.target].whoAmI, -(float)Math.PI / 4, npc.ai[0]);
							TargetedLineWarn.color = Color.Blue;
							TargetedLineWarn.target = null;
						}
					}
					if (KSspellcardtype == 1)
					{
						if (KSspellcard == 60)
						{
							Vector2[] pos =
							{new Vector2(0, -4),
							new Vector2(-1, -3),
							new Vector2(0, -3),
							new Vector2(-2, -2),
							new Vector2(-1, -2),
							new Vector2(0, -2),
							new Vector2(1, -2),
							new Vector2(2, -2),
							new Vector2(-2, -1),
							new Vector2(-1, -1),
							new Vector2(0, -1),
							new Vector2(1, -1),
							new Vector2(2, -1),
							new Vector2(3, -1),
							new Vector2(-4, 0),
							new Vector2(-3, 0),
							new Vector2(-2, 0),
							new Vector2(-1, 0),
							new Vector2(0, 0),
							new Vector2(1, 0),
							new Vector2(2, 0),
							new Vector2(3, 0),
							new Vector2(4, 0),
							new Vector2(-3, 1),
							new Vector2(-2, 1),
							new Vector2(-1, 1),
							new Vector2(0, 1),
							new Vector2(1, 1),
							new Vector2(2, 1),
							new Vector2(-2, 2),
							new Vector2(-1, 2),
							new Vector2(0, 2),
							new Vector2(1, 2),
							new Vector2(2, 2),
							new Vector2(0, 3),
							new Vector2(1, 3),
							new Vector2(0, 4)
							};
							for (int i = 0; i <= 36; i++)
							{
								Projectile.NewProjectileDirect(npc.GetSource_FromThis(), squarecenter + pos[i] * 120, Vector2.Zero, ModContent.ProjectileType<CircleWarn>(), 0, 0, Main.player[npc.target].whoAmI, 0.5f);
								CircleWarn.color = Color.Blue;
							}
						}
						if (KSspellcard == 120)
						{
							Vector2[] pos =
							{new Vector2(0, -4),
							new Vector2(-1, -3),
							new Vector2(0, -3),
							new Vector2(-2, -2),
							new Vector2(-1, -2),
							new Vector2(0, -2),
							new Vector2(1, -2),
							new Vector2(2, -2),
							new Vector2(-2, -1),
							new Vector2(-1, -1),
							new Vector2(0, -1),
							new Vector2(1, -1),
							new Vector2(2, -1),
							new Vector2(3, -1),
							new Vector2(-4, 0),
							new Vector2(-3, 0),
							new Vector2(-2, 0),
							new Vector2(-1, 0),
							new Vector2(0, 0),
							new Vector2(1, 0),
							new Vector2(2, 0),
							new Vector2(3, 0),
							new Vector2(4, 0),
							new Vector2(-3, 1),
							new Vector2(-2, 1),
							new Vector2(-1, 1),
							new Vector2(0, 1),
							new Vector2(1, 1),
							new Vector2(2, 1),
							new Vector2(-2, 2),
							new Vector2(-1, 2),
							new Vector2(0, 2),
							new Vector2(1, 2),
							new Vector2(2, 2),
							new Vector2(0, 3),
							new Vector2(1, 3),
							new Vector2(0, 4)
							};
							for (int i = 0; i <= 36; i++)
							{
								Projectile.NewProjectileDirect(npc.GetSource_FromThis(), squarecenter + pos[i] * 120, Vector2.Zero, ModContent.ProjectileType<BossSlimyShuriken>(), npc.damage / 8, 0, Main.player[npc.target].whoAmI);
							}
						}
					}
					if (KSspellcardtype == 2)
					{
						if(KSspellcard == 60)
                        {
							Projectile.NewProjectileDirect(npc.GetSource_FromThis(), squarecenter, Vector2.Zero, ModContent.ProjectileType<TargetedLineWarn>(), 0, 0, Main.player[npc.target].whoAmI, (float)Math.PI / 2, -240);
							Projectile.NewProjectileDirect(npc.GetSource_FromThis(), squarecenter, Vector2.Zero, ModContent.ProjectileType<TargetedLineWarn>(), 0, 0, Main.player[npc.target].whoAmI, (float)Math.PI / 2, 240);
							TargetedLineWarn.color = Color.Blue;
							TargetedLineWarn.target = Main.player[npc.target];
						}
						if (KSspellcard == 120)
						{
							Vector2[] pos =
							{new Vector2(-1, -10),
						new Vector2(1, -10),
						new Vector2(-2, -8),
						new Vector2(2, -8),
						new Vector2(-1, -6),
						new Vector2(1, -6),
						new Vector2(-1, -4),
						new Vector2(1, -4),
						new Vector2(-1, -2),
						new Vector2(1, -2),
						new Vector2(-2, 0),
						new Vector2(0, 0),
						new Vector2(2, 0),
						new Vector2(-3, 2),
						new Vector2(-1, 2),
						new Vector2(1, 2),
						new Vector2(3, 2),
						new Vector2(-4, 4),
						new Vector2(-2, 4),
						new Vector2(0, 4),
						new Vector2(2, 4),
						new Vector2(4, 4),
						new Vector2(-3, 6),
						new Vector2(-1, 6),
						new Vector2(1, 6),
						new Vector2(3, 6),
						new Vector2(-2, 8),
						new Vector2(0, 8),
						new Vector2(2, 8),
						new Vector2(-1, 10),
						new Vector2(1, 10),
						new Vector2(0, 12)
						};
							for (int i = 0; i <= 31; i++)
							{
								Projectile.NewProjectileDirect(npc.GetSource_FromThis(), Main.player[npc.target].Center + pos[i] * 20 + new Vector2(-240, -600), Vector2.Zero, ModContent.ProjectileType<BossSlimyKunai>(), npc.damage / 8, 0, Main.player[npc.target].whoAmI, 0, i);
								Projectile.NewProjectileDirect(npc.GetSource_FromThis(), Main.player[npc.target].Center + pos[i] * 20 + new Vector2(240, -600), Vector2.Zero, ModContent.ProjectileType<BossSlimyKunai>(), npc.damage / 8, 0, Main.player[npc.target].whoAmI, 0, i);
							}
						}
					}

					if (KSspellcard >= 1200 || Main.player[npc.target].dead)
					{
						npc.aiAction = 0;
						KSstopspell = true;
						npc.ai[1] = 0f;
						npc.ai[0] = 0f;
						npc.ai[2] = 360f;
						npc.TargetClosest(true);
						KSspellcard = 0;
						KSnospellcard = 6000;
					}
				}
				else
				{
					if (KSnospellcard > 0)
						KSnospellcard--;
				}

				int d2 = Dust.NewDust(npc.position, npc.width, npc.height, 4, npc.velocity.X, npc.velocity.Y, 255, new Color(0, 80, 255, 80), npc.scale * 1.2f);
				Main.dust[d2].noGravity = true;
				dust = Main.dust[d2];
				dust.velocity *= 0.5f; //神秘粒子
				if (npc.life > 0) //boss大小随着血量变化
				{
					float lifeper = (float)npc.life / (float)npc.lifeMax;
					lifeper = lifeper * 0.5f + 0.75f;
					lifeper *= bosssizefix;
					if (lifeper != npc.scale)
					{
						npc.position.X += (float)(npc.width / 2);
						npc.position.Y += (float)npc.height;
						npc.scale = lifeper;
						npc.width = (int)(98f * npc.scale);
						npc.height = (int)(92f * npc.scale);
						npc.position.X -= (float)(npc.width / 2);
						npc.position.Y -= (float)npc.height;
					}
					/*if (Main.netMode != 1)
					{
						int num257 = (int)((double)npc.lifeMax * 0.05);
						if ((float)(npc.life + num257) < npc.ai[3])
						{
							npc.ai[3] = (float)npc.life;
							int num258 = Main.rand.Next(1, 4);
							int num;
							for (int num259 = 0; num259 < num258; num259 = num + 1)
							{
								int x = (int)(npc.position.X + (float)Main.rand.Next(npc.width - 32));
								int y = (int)(npc.position.Y + (float)Main.rand.Next(npc.height - 32));
								int num260 = 1;
								if (Main.expertMode && Main.rand.Next(4) == 0)
								{
									num260 = 535;
								}
								int num261 = NPC.NewNPC(npc.GetSource_FromAI(), x, y, num260, 0, 0f, 0f, 0f, 0f, 255);
								Main.npc[num261].SetDefaults(num260, default(NPCSpawnParams));
								Main.npc[num261].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
								Main.npc[num261].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
								Main.npc[num261].ai[0] = (float)(-1000 * Main.rand.Next(3));
								Main.npc[num261].ai[1] = 0f;
								if (Main.netMode == 2 && num261 < 200)
								{
									NetMessage.SendData(23, -1, -1, null, num261, 0f, 0f, 0f, 0, 0, 0);
								}
								num = num259;
							}
						}
					}*/
				}
				target = Main.player[npc.target];
				return true;
			}
			return base.PreAI(npc);
		}
		public override void OnSpawn(NPC npc, IEntitySource source)
		{
			switch (npc.type)
			{
				case NPCID.KingSlime:
					target = null;
					KSspellcard = 0;
					KSspellcardtype = 0;
					KSstopspell = false;
					squarecenter = Vector2.Zero;
					break;
			}
		}
	}

	public class VolatileGel : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			/*DisplayName.SetDefault("Volatile Gel");
			DisplayName.AddTranslation(7, "挥发凝胶");*/
		}
		public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 34;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.alpha = 30;
			base.SetDefaults();
		}
        public override void AI()
        {
			Projectile.rotation += 0.2f;
			if(Projectile.velocity.Y < 10)
				Projectile.velocity.Y += 0.5f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			for (int i = 0; i < 20; i++) 
			{
				Dust dust;
				int d = Dust.NewDust(Projectile.position, 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
				Main.dust[d].noGravity = true;
				dust = Main.dust[d];
				dust.velocity *= 2f;
			}
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StickyGel>(), 0, 0);
			return true;
        }
		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
		}
	}
	public class BurstGel : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			/*DisplayName.SetDefault("Burst Gel");
			DisplayName.AddTranslation(7, "爆裂凝胶");*/
		}
		public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 34;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.alpha = 30;
			base.SetDefaults();
		}
		public override void AI()
		{
			Projectile.rotation += 0.3f;
			if (Projectile.velocity.Y < 10)
				Projectile.velocity.Y += 1f;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			for (int i = 0; i < 20; i++)
			{
				Dust dust;
				int d = Dust.NewDust(Projectile.position, 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
				Main.dust[d].noGravity = true;
				dust = Main.dust[d];
				dust.velocity *= 2f;
			}
			for (int i = 0; i < 4; i++)
            {
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitX.RotatedBy(0.1 + 0.2 * (i - 2) + Math.PI / 2) * 20, ModContent.ProjectileType<SmallBurstGel>(), Projectile.damage / 2, 0);
			}
			return true;
		}
		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
		}
	}
	public class SmallBurstGel : ModProjectile
	{
		public override string Texture => "DeusMod/NPCs/Vanilla/VanillaBoss/KingSlime/BurstGel";
		public override void SetStaticDefaults()
		{
			/*DisplayName.SetDefault("Burst Gel");
			DisplayName.AddTranslation(7, "爆裂凝胶");*/
		}
		public override void SetDefaults()
		{
			Projectile.width = 17;
			Projectile.height = 17;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.alpha = 30;
			Projectile.scale = 0.5f;
			base.SetDefaults();
		}
		public override void AI()
		{
			if (Projectile.velocity.Y < 10)
				Projectile.velocity.Y += 0.5f;
		}
	}
	public class KSRestriction : ModProjectile
	{
		public override string Texture => "DeusMod/NPCs/Vanilla/VanillaBoss/KingSlime/VolatileGel";
		public override void SetStaticDefaults()
		{
			/*DisplayName.SetDefault("King Slime");
			DisplayName.AddTranslation(7, "史莱姆王");*/
		}
		public override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.timeLeft = 1200;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.alpha = 30;
			base.SetDefaults();
		}
		public override void OnSpawn(IEntitySource source)
		{
			for (int i = 1; i <= 32; i++)
			{
				if (i <= 8)
				{
					for (int j = 0; j < 10; j++)
					{
						Dust dust;
						int d = Dust.NewDust(Projectile.Center + new Vector2(360, -360 + i * 90), 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
						Main.dust[d].noGravity = true;
						dust = Main.dust[d];
						dust.velocity *= 2f;
					}
				}
				else if (i <= 16)
				{
					for (int j = 0; j < 10; j++)
					{
						Dust dust;
						int d = Dust.NewDust(Projectile.Center + new Vector2(360 - (i - 8) * 90, 360), 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
						Main.dust[d].noGravity = true;
						dust = Main.dust[d];
						dust.velocity *= 2f;
					}
				}
				else if (i <= 24)
				{
					for (int j = 0; j < 10; j++)
					{
						Dust dust;
						int d = Dust.NewDust(Projectile.Center + new Vector2(-360, 360 - (i - 16) * 90), 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
						Main.dust[d].noGravity = true;
						dust = Main.dust[d];
						dust.velocity *= 2f;
					}
				}
				else
				{
					for (int j = 0; j < 10; j++)
					{
						Dust dust;
						int d = Dust.NewDust(Projectile.Center + new Vector2(-360 + (i - 24) * 90, -360), 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
						Main.dust[d].noGravity = true;
						dust = Main.dust[d];
						dust.velocity *= 2f;
					}
				}
			}
		}
		public override void AI()
        {
			if (KingSlimeRE.KSstopspell)
				Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
			for (int i = 1; i <= 32; i++)
			{
				if (i <= 8)
					Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center + new Vector2(360, -360 + i * 90) - Main.screenPosition, new Rectangle(0, 0, 34, 34), Color.White, 0, new Vector2(17, 17), 1, SpriteEffects.None, 0);
				else if (i <= 16)
					Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center + new Vector2(360 - (i - 8) * 90, 360) - Main.screenPosition, new Rectangle(0, 0, 34, 34), Color.White, 0, new Vector2(17, 17), 1, SpriteEffects.None, 0);
				else if (i <= 24)
					Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center + new Vector2(-360, 360 - (i - 16) * 90) - Main.screenPosition, new Rectangle(0, 0, 34, 34), Color.White, 0, new Vector2(17, 17), 1, SpriteEffects.None, 0);
				else
					Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center + new Vector2(-360 + (i - 24) * 90, -360) - Main.screenPosition, new Rectangle(0, 0, 34, 34), Color.White, 0, new Vector2(17, 17), 1, SpriteEffects.None, 0);
			}
			return false;
        }
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			return (Math.Abs(targetHitbox.Center.X - projHitbox.Center.X) >= 360 || Math.Abs(targetHitbox.Center.Y - projHitbox.Center.Y) >= 360);
		}
        public override void Kill(int timeLeft)
        {
			for (int i = 1; i <= 32; i++)
			{
				if (i <= 8)
				{
					for (int j = 0; j < 10; j++)
					{
						Dust dust;
						int d = Dust.NewDust(Projectile.Center + new Vector2(360, -360 + i * 90), 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
						Main.dust[d].noGravity = true;
						dust = Main.dust[d];
						dust.velocity *= 2f;
					}
				}
				else if (i <= 16)
				{
					for (int j = 0; j < 10; j++)
					{
						Dust dust;
						int d = Dust.NewDust(Projectile.Center + new Vector2(360 - (i - 8) * 90, 360), 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
						Main.dust[d].noGravity = true;
						dust = Main.dust[d];
						dust.velocity *= 2f;
					}
				}
				else if (i <= 24)
				{
					for (int j = 0; j < 10; j++)
					{
						Dust dust;
						int d = Dust.NewDust(Projectile.Center + new Vector2(-360, 360 - (i - 16) * 90), 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
						Main.dust[d].noGravity = true;
						dust = Main.dust[d];
						dust.velocity *= 2f;
					}
				}
				else
				{
					for (int j = 0; j < 10; j++)
					{
						Dust dust;
						int d = Dust.NewDust(Projectile.Center + new Vector2(-360 + (i - 24) * 90, -360), 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
						Main.dust[d].noGravity = true;
						dust = Main.dust[d];
						dust.velocity *= 2f;
					}
				}
			}
		}
    }
	public class StickyGel : ModProjectile
	{
		public override string Texture => "DeusMod/NPCs/Vanilla/VanillaBoss/KingSlime/StickyGel";
		public override void SetDefaults()
		{
			Projectile.width = 52;
			Projectile.height = 56;
			Projectile.timeLeft = 1800;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.alpha = 30;
			base.SetDefaults();
		}
        public override void AI()
        {
			Rectangle projHitbox = Projectile.Hitbox;
			Rectangle playerHitbox = KingSlimeRE.target.Hitbox;
			Rectangle overlap = Rectangle.Intersect(projHitbox, playerHitbox);
			if (overlap.Size().X != 0 && overlap.Size().Y != 0)
			{
				KingSlimeRE.target.AddBuff(ModContent.BuffType<SlimedTier2>(), 30);
			}
		}
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 52, 56), Color.White, 0, new Vector2(0, 28), 1, SpriteEffects.None, 0);
			return false;
        }
    }
	public class BossSlimyThrowingKnife : ModProjectile
	{
		public override string Texture => "DeusMod/Assets/Items/Weapon/Ranged/SlimyThrowingKnife";
        public override void SetStaticDefaults()
        {
			/*DisplayName.SetDefault("Slimy Throwing Knife");
			DisplayName.AddTranslation(7, "史莱姆投刀");*/
		}
        public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 24;
			Projectile.timeLeft = 540;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.alpha = 30;
			base.SetDefaults();
		}
		public override void OnSpawn(IEntitySource source)
		{
			for (int i = 0; i < 10; i++)
			{
				Dust dust;
				int d = Dust.NewDust(Projectile.Center, 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
				Main.dust[d].noGravity = true;
				dust = Main.dust[d];
				dust.velocity *= 2f;
			}
		}
		public override void AI()
        {
			if (KingSlimeRE.KSstopspell)
				Projectile.Kill();
			Projectile.rotation = Projectile.ai[1];
			Projectile.localAI[0]++;
			if (Projectile.localAI[0] > 90 && Projectile.localAI[0] <= 120)
				Projectile.velocity = Vector2.UnitY.RotatedBy(Projectile.rotation) * (1 + Projectile.ai[0]) * 2 * ((120 - Projectile.localAI[0]) / 30);
			else if (Projectile.localAI[0] > 120 && Projectile.localAI[0] <= 150)
				Projectile.velocity = -Vector2.UnitY.RotatedBy(Projectile.rotation) * (1 + Projectile.ai[0]) * 8;
			else if (Projectile.localAI[0] > 150 && Projectile.localAI[0] <= 165)
				Projectile.velocity = -Vector2.UnitY.RotatedBy(Projectile.rotation) * (1 + Projectile.ai[0]) * 8 * ((165 - Projectile.localAI[0]) / 15);
			else Projectile.velocity = Vector2.Zero;
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			target.AddBuff(BuffID.Slimed, Main.rand.Next(5, 11) * 60);
		}
		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				Dust dust;
				int d = Dust.NewDust(Projectile.Center, 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
				Main.dust[d].noGravity = true;
				dust = Main.dust[d];
				dust.velocity *= 2f;
			}
		}
	}
	public class BossSlimyShuriken : ModProjectile
	{
		public override string Texture => "DeusMod/Assets/Items/Weapon/Ranged/SlimyShuriken";
		public override void SetStaticDefaults()
		{
			/*DisplayName.SetDefault("Slimy Shuriken");
			DisplayName.AddTranslation(7, "史莱姆手里剑");*/
		}
		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.timeLeft = 1080;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.alpha = 30;
			base.SetDefaults();
		}
        public override void OnSpawn(IEntitySource source)
        {
			for (int i = 0; i < 10; i++)
			{
				Dust dust;
				int d = Dust.NewDust(Projectile.Center, 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
				Main.dust[d].noGravity = true;
				dust = Main.dust[d];
				dust.velocity *= 2f;
			}
		}
        public override void AI()
		{
			if (KingSlimeRE.KSstopspell)
				Projectile.Kill();
			Projectile.rotation += 0.3f;
			Projectile.ai[0]++;
			float dis = 1;
			if (Projectile.ai[0] > 120)
			{
				dis = 1 + (float)Math.Sin((Projectile.ai[0] - 120) / 90) / 2;
				if (Projectile.ai[0] == 121)
				{
					Projectile.localAI[0] = (Projectile.Center - KingSlimeRE.squarecenter).X;
					Projectile.localAI[1] = (Projectile.Center - KingSlimeRE.squarecenter).Y;
				}
				Projectile.Center = KingSlimeRE.squarecenter + (new Vector2(Projectile.localAI[0], Projectile.localAI[1]) * dis).RotatedBy((Projectile.ai[0] - 120) * 0.01f);
			}
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.Slimed, Main.rand.Next(5, 11) * 60);
		}
		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				Dust dust;
				int d = Dust.NewDust(Projectile.Center, 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
				Main.dust[d].noGravity = true;
				dust = Main.dust[d];
				dust.velocity *= 2f;
			}
		}
	}
	public class BossSlimyKunai : ModProjectile
	{
		public override string Texture => "DeusMod/Assets/Items/Weapon/Melee/SlimyKunai";
		public override void SetStaticDefaults()
		{
			/*DisplayName.SetDefault("Slimy Kunai");
			DisplayName.AddTranslation(7, "史莱姆苦无");*/
		}
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 42;
			Projectile.timeLeft = 1080;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.alpha = 30;
			Projectile.rotation = (float)Math.PI;
			base.SetDefaults();
		}
		public override void AI()
		{
			Projectile.ai[0]++;
			if (Projectile.ai[0] <= 15)
				Projectile.velocity.Y = 40;
			else
				Projectile.velocity.Y = 0;

			if (Projectile.ai[0] == 15)
			{
				Main.LocalPlayer.GetModPlayer<ScreenShake>().ScreenShakeShort(24, (float)Math.PI / 2);
				for (int i = 0; i < 10; i++)
				{
					Dust dust;
					int d = Dust.NewDust(Projectile.Center, 40, 40, 4, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-2, 2), 150, new Color(78, 136, 255, 80), 2f);
					Main.dust[d].noGravity = true;
					dust = Main.dust[d];
					dust.velocity *= 2f;
				}
			}

			if(Projectile.ai[0] > 85 && Projectile.ai[0] <= 90)
            {
				if (KingSlimeRE.target.Center.X > Projectile.Center.X)
					Projectile.rotation = Projectile.rotation + (((float)Math.Atan((KingSlimeRE.target.Center - Projectile.Center).Y / (KingSlimeRE.target.Center - Projectile.Center).X) + (float)Math.PI / 2) - Projectile.rotation) / (91 - Projectile.ai[0]);
				else
					Projectile.rotation = Projectile.rotation + (((float)Math.Atan((KingSlimeRE.target.Center - Projectile.Center).Y / (KingSlimeRE.target.Center - Projectile.Center).X) - (float)Math.PI / 2) - Projectile.rotation) / (91 - Projectile.ai[0]);
			}

			if (Projectile.ai[0] > 90 && Projectile.ai[0] <= 120 + Projectile.ai[1] * 27)
            {
				if(KingSlimeRE.target.Center.X > Projectile.Center.X)
					Projectile.rotation = (float)Math.Atan((KingSlimeRE.target.Center - Projectile.Center).Y / (KingSlimeRE.target.Center - Projectile.Center).X) + (float)Math.PI / 2;
				else
					Projectile.rotation = (float)Math.Atan((KingSlimeRE.target.Center - Projectile.Center).Y / (KingSlimeRE.target.Center - Projectile.Center).X) - (float)Math.PI / 2;
			}

			if (Projectile.ai[0] > 120 + Projectile.ai[1] * 27)
				Projectile.velocity = -Vector2.UnitY.RotatedBy(Projectile.rotation) * 8;

			if (KingSlimeRE.KSstopspell)
				Projectile.Kill();
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.Slimed, Main.rand.Next(5, 11) * 60);
		}
	}
}
