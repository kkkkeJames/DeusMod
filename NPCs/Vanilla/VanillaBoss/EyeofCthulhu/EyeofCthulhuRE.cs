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

namespace DeusMod.NPCs.Vanilla.VanillaBoss.EyeOfCthulhu
{
    public class EyeofCthulhuRE : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
				bool speedup2 = false;
				if (Main.expertMode && (double)npc.life < (double)npc.lifeMax * 0.12)
				{
					speedup2 = true;
				}
				bool speedup3 = false;
				if (Main.expertMode && (double)npc.life < (double)npc.lifeMax * 0.04)
				{
					speedup3 = true;
				}
				float num5 = 20f;
				if (speedup3)
				{
					num5 = 10f;
				}
				if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				{
					npc.TargetClosest(true);
				}
				bool dead = Main.player[npc.target].dead;
				float Xdistance = npc.position.X + (float)(npc.width / 2) - Main.player[npc.target].position.X - (float)(Main.player[npc.target].width / 2);
				float Ydistance = npc.position.Y + (float)npc.height - 59f - Main.player[npc.target].position.Y - (float)(Main.player[npc.target].height / 2);
				float targetangle = (float)Math.Atan2((double)Ydistance, (double)Xdistance) + 1.57f; //atan2求的是某向量的X轴偏角
				if (targetangle < 0f) //夹角数据调整
				{
					targetangle += 6.283f;
				}
				else if ((double)targetangle > 6.283)
				{
					targetangle -= 6.283f;
				}
				float rotnum = 0f; //微调数据
				if (npc.ai[0] == 0f && npc.ai[1] == 0f)
				{
					rotnum = 0.02f;
				}
				if (npc.ai[0] == 0f && npc.ai[1] == 2f && npc.ai[2] > 40f)
				{
					rotnum = 0.05f;
				}
				if (npc.ai[0] == 3f && npc.ai[1] == 0f)
				{
					rotnum = 0.05f;
				}
				if (npc.ai[0] == 3f && npc.ai[1] == 2f && npc.ai[2] > 40f)
				{
					rotnum = 0.08f;
				}
				if (npc.ai[0] == 3f && npc.ai[1] == 4f && npc.ai[2] > num5)
				{
					rotnum = 0.15f;
				}
				if (npc.ai[0] == 3f && npc.ai[1] == 5f)
				{
					rotnum = 0.05f;
				}
				if (Main.expertMode)
				{
					rotnum *= 1.5f;
				}
				if (speedup3 && Main.expertMode)
				{
					rotnum = 0f;
				}
				if (npc.rotation < targetangle) //角度调整
				{
					if ((double)(targetangle - npc.rotation) > 3.1415)
					{
						npc.rotation -= rotnum;
					}
					else
					{
						npc.rotation += rotnum;
					}
				}
				else if (npc.rotation > targetangle)
				{
					if ((double)(npc.rotation - targetangle) > 3.1415)
					{
						npc.rotation += rotnum;
					}
					else
					{
						npc.rotation -= rotnum;
					}
				}
				if (npc.rotation > targetangle - rotnum && npc.rotation < targetangle + rotnum)
				{
					npc.rotation = targetangle;
				}
				if (npc.rotation < 0f)
				{
					npc.rotation += 6.283f;
				}
				else if ((double)npc.rotation > 6.283)
				{
					npc.rotation -= 6.283f;
				}
				if (npc.rotation > targetangle - rotnum && npc.rotation < targetangle + rotnum)
				{
					npc.rotation = targetangle;
				}
				if (Main.rand.NextBool(5)) //出血粒子，毕竟是一个眼球
				{
					int dust = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default(Color), 1f);
					Main.dust[dust].velocity.X *= 0.5f;
					Main.dust[dust].velocity.Y *= 0.1f;
				}
				if (Main.dayTime || dead) //boss开润
				{
					npc.velocity.Y -= 0.04f;
					npc.EncourageDespawn(10);
					//return;
				}
				if (npc.ai[0] == 0f)
				{
					if (npc.ai[1] == 0f)
					{
						float disnum = 5f; //距离修正
						float chaseacc = 0.04f; //长距离速度修正
						if (Main.expertMode) //专家模式强化
						{
							disnum = 7f;
							chaseacc = 0.15f;
						}
						Vector2 position = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float targetdisX = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - position.X;
						float targetdisY = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 200f - position.Y;
						float targetdis = (float)Math.Sqrt((double)(targetdisX * targetdisX + targetdisY * targetdisY));
						float flagtargetdis = targetdis;
						targetdis = disnum / targetdis;
						targetdisX *= targetdis;
						targetdisY *= targetdis;
						if (npc.velocity.X < targetdisX)
						{
							npc.velocity.X += chaseacc; //如果自身速度小于目标的距离则加速
							if (npc.velocity.X < 0f && targetdisX > 0f)
							{
								npc.velocity.X += chaseacc; //如果移动方向还是错误的则再加速
							}
						}
						else if (npc.velocity.X > targetdisX)
						{
							npc.velocity.X -= chaseacc; //反之亦然
							if (npc.velocity.X > 0f && targetdisX < 0f)
							{
								npc.velocity.X -= chaseacc; //调头
							}
						}
						if (npc.velocity.Y < targetdisY) //Y轴和X轴同理
						{
							npc.velocity.Y += chaseacc;
							if (npc.velocity.Y < 0f && targetdisY > 0f)
							{
								npc.velocity.Y += chaseacc;
							}
						}
						else if (npc.velocity.Y > targetdisY)
						{
							npc.velocity.Y -= chaseacc;
							if (npc.velocity.Y > 0f && targetdisY < 0f)
							{
								npc.velocity.Y -= chaseacc;
							}
						}
						npc.ai[2] += 1f; //计时器
						float timer0 = 600f; //npc.ai[0]和npc.ai[1]都是0时的时间上限
						if (Main.expertMode)
						{
							timer0 *= 0.35f;
						}
						if (npc.ai[2] >= timer0) //时间到
						{
							npc.ai[1] = 1f; //ai[1]变为1
							npc.ai[2] = 0f;
							npc.ai[3] = 0f;
							npc.target = 255;
							npc.netUpdate = true;
						}
						else if ((npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y && flagtargetdis < 500f) || (Main.expertMode && flagtargetdis < 500f)) //时间不到的话只要在一定距离内（非专家下还有高度必须在上的限制）
						{
							if (!Main.player[npc.target].dead)
							{
								npc.ai[3] += 1f; //另一个计时器
							}
							float timer1 = 110f;
							if (Main.expertMode)
							{
								timer1 *= 0.4f;
							}
							if (npc.ai[3] >= timer1)
							{
								npc.ai[3] = 0f;
								npc.rotation = targetangle; //瞄准玩家的角度(盯着玩家)
								float speednum2 = 5f;
								if (Main.expertMode)
								{
									speednum2 = 6f;
								}
								float targetdis2X = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - position.X;
								float targetdis2Y = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - position.Y;
								float targetdis2 = (float)Math.Sqrt((double)(targetdis2X * targetdis2X + targetdis2Y * targetdis2Y));
								targetdis2 = speednum2 / targetdis2;
								Vector2 vector2 = position;
								Vector2 vector3;
								vector3.X = targetdis2X * targetdis2;
								vector3.Y = targetdis2Y * targetdis2;
								vector2.X += vector3.X * 10f;
								vector2.Y += vector3.Y * 10f;
								if (Main.netMode != 1) //生成克苏鲁之仆
								{
									int newnpc = NPC.NewNPC(npc.GetSource_FromAI(), (int)vector2.X, (int)vector2.Y, 5, 0, 0f, 0f, 0f, 0f, 255);
									Main.npc[newnpc].velocity.X = vector3.X;
									Main.npc[newnpc].velocity.Y = vector3.Y;
									if (Main.netMode == 2 && newnpc < 200)
									{
										NetMessage.SendData(23, -1, -1, null, newnpc, 0f, 0f, 0f, 0, 0, 0);
									}
								}
								//SoundEngine.PlaySound(3, (int)vector2.X, (int)vector2.Y, 1, 1f, 0f);
								SoundEngine.PlaySound(SoundID.Item43, vector2);
								int num;
								for (int m = 0; m < 10; m = num + 1) //血粒子
								{
									Dust.NewDust(vector2, 20, 20, 5, vector3.X * 0.4f, vector3.Y * 0.4f, 0, default(Color), 1f);
									num = m;
								}
							}
						}
					}
					else if (npc.ai[1] == 1f) //冲刺，即把速度固定
					{
						npc.rotation = targetangle; //继续盯着玩家
						float dashnum = 6f;
						if (Main.expertMode)
						{
							dashnum = 7f;
						}
						Vector2 position = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float targetdistanceX = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - position.X;
						float targetdistanceY = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - position.Y;
						float targetdistance = (float)Math.Sqrt((double)(targetdistanceX * targetdistanceX + targetdistanceY * targetdistanceY));
						targetdistance = dashnum / targetdistance;
						npc.velocity.X = targetdistanceX * targetdistance;
						npc.velocity.Y = targetdistanceY * targetdistance;
						npc.ai[1] = 2f; //直接换ai状态
						npc.netUpdate = true;
						if (npc.netSpam > 10)
						{
							npc.netSpam = 10;
						}
					}
					else if (npc.ai[1] == 2f)
					{
						npc.ai[2] += 1f; //计时器增加
						if (npc.ai[2] >= 40f) //计时器达到某个值
						{
							npc.velocity *= 0.98f; //速度开始降低
							if (Main.expertMode)
							{
								npc.velocity *= 0.985f;
							}
							if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1) //完全停下
							{
								npc.velocity.X = 0f;
							}
							if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
							{
								npc.velocity.Y = 0f;
							}
						}
						else
						{
							npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f; //角度等于移动方向向量所指
						}
						int timer = 150; //计时器
						if (Main.expertMode)
						{
							timer = 100;
						}
						if (npc.ai[2] >= (float)timer)
						{
							npc.ai[3] += 1f; //冲刺次数增加，这里的ai[3]代表冲刺次数
							npc.ai[2] = 0f; //npc.ai[2]清零
							npc.target = 255;
							npc.rotation = targetangle; //盯着玩家
							if (npc.ai[3] >= 3f) //冲刺超过三次清零数据
							{
								npc.ai[1] = 0f;
								npc.ai[3] = 0f;
							}
							else
							{
								npc.ai[1] = 1f; //继续冲刺
							}
						}
					}
					float phase2life = 0.5f;
					if (Main.expertMode)
					{
						phase2life = 0.65f;
					}
					if ((float)npc.life < (float)npc.lifeMax * phase2life) //切换形态
					{
						npc.ai[0] = 1f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
						if (npc.netSpam > 10)
						{
							npc.netSpam = 10;
							//return;
						}
					}
				}
				else if (npc.ai[0] == 1f || npc.ai[0] == 2f) //转转
				{
					if (npc.ai[0] == 1f) //旋转
					{
						npc.ai[2] += 0.005f; //npc.ai[2]始终在微调，大于0小于0.5，增大增小都是100f时间
						if ((double)npc.ai[2] > 0.5)
						{
							npc.ai[2] = 0.5f;
						}
					}
					else
					{
						npc.ai[2] -= 0.005f;
						if (npc.ai[2] < 0f)
						{
							npc.ai[2] = 0f;
						}
					}
					npc.rotation += npc.ai[2];
					npc.ai[1] += 1f;
					if (Main.expertMode && npc.ai[1] % 20f == 0f)
					{
						Vector2 position = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float sr1 = (float)Main.rand.Next(-200, 200);
						float sr2 = (float)Main.rand.Next(-200, 200);
						float srdis = (float)Math.Sqrt((double)(sr1 * sr1 + sr2 * sr2));
						srdis = 5f / srdis;
						Vector2 vector = position;
						Vector2 vector2;
						vector2.X = sr1 * srdis;
						vector2.Y = sr2 * srdis;
						vector.X += vector2.X * 10f;
						vector.Y += vector2.Y * 10f;
						if (Main.netMode != 1) //丢大量npc
						{
							int newnpc = NPC.NewNPC(npc.GetSource_FromAI(), (int)vector.X, (int)vector.Y, 5, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[newnpc].velocity.X = vector2.X;
							Main.npc[newnpc].velocity.Y = vector2.Y;
							if (Main.netMode == 2 && newnpc < 200)
							{
								NetMessage.SendData(23, -1, -1, null, newnpc, 0f, 0f, 0f, 0, 0, 0);
							}
						}
						int num;
						for (int n = 0; n < 10; n = num + 1)
						{
							Dust.NewDust(vector, 20, 20, 5, vector2.X * 0.4f, vector2.Y * 0.4f, 0, default(Color), 1f);
							num = n;
						}
					}
					if (npc.ai[1] == 100f) //时间到了
					{
						npc.ai[0] += 1f; //开始降低转速
						npc.ai[1] = 0f; //清空计时器
						if (npc.ai[0] == 3f) //转完了两次以后清空ai[2]
						{
							npc.ai[2] = 0f;
						}
						else
						{
							SoundEngine.PlaySound(SoundID.Item43, npc.position);
							for (int i = 0; i < 2; i++) //送克眼石块
							{
								Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 8, 1f);
								Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
								Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
							}
							for (int i = 0; i < 20; i++) //为啥不是黒色石油？
							{
								Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);
							}
							SoundEngine.PlaySound(SoundID.Roar, npc.position);
							//SoundEngine.PlaySound(15, npc.position);
						}
					}
					Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);
					npc.velocity.X *= 0.98f;
					npc.velocity.Y *= 0.98f;
					if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
					{
						npc.velocity.X = 0f;
					}
					if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
					{
						npc.velocity.Y = 0f;
						//return;
					}
				}
				else //二阶段
				{
					npc.defense = 0; //二阶段是没防御的，没想到吧（）
					int damlerp = 23;
					int damlerp2 = 18;
					if (Main.expertMode)
					{
						if (speedup2)
						{
							npc.defense = -15;
						}
						if (speedup3)
						{
							damlerp2 = 20;
							npc.defense = -30;
						}
					}
					npc.damage = npc.GetAttackDamage_LerpBetweenFinalValues((float)damlerp, (float)damlerp2);
					npc.damage = npc.GetAttackDamage_ScaledByStrength((float)npc.damage);
					if (npc.ai[1] == 0f && speedup2)
					{
						npc.ai[1] = 5f;
					}
					if (npc.ai[1] == 0f) //常规态，0次冲刺
					{
						float phase2disnum = 6f;
						float phase2acc = 0.07f;
						Vector2 position = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float targetdistanceX = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - position.X;
						float targetdistanceY = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 120f - position.Y;
						float targetdistance = (float)Math.Sqrt((double)(targetdistanceX * targetdistanceX + targetdistanceY * targetdistanceY));
						if (targetdistance > 400f && Main.expertMode)
						{
							phase2disnum += 1f;
							phase2acc += 0.05f;
							if (targetdistance > 600f)
							{
								phase2disnum += 1f;
								phase2acc += 0.05f;
								if (targetdistance > 800f)
								{
									phase2disnum += 1f;
									phase2acc += 0.05f;
								}
							}
						}
						targetdistance = phase2disnum / targetdistance;
						targetdistanceX *= targetdistance;
						targetdistanceY *= targetdistance;
						if (npc.velocity.X < targetdistanceX)
						{
							npc.velocity.X += phase2acc;
							if (npc.velocity.X < 0f && targetdistanceX > 0f)
							{
								npc.velocity.X += phase2acc;
							}
						}
						else if (npc.velocity.X > targetdistanceX)
						{
							npc.velocity.X -= phase2acc;
							if (npc.velocity.X > 0f && targetdistanceX < 0f)
							{
								npc.velocity.X -= phase2acc;
							}
						}
						if (npc.velocity.Y < targetdistanceY)
						{
							npc.velocity.Y += phase2acc;
							if (npc.velocity.Y < 0f && targetdistanceY > 0f)
							{
								npc.velocity.Y += phase2acc;
							}
						}
						else if (npc.velocity.Y > targetdistanceY)
						{
							npc.velocity.Y -= phase2acc;
							if (npc.velocity.Y > 0f && targetdistanceY < 0f)
							{
								npc.velocity.Y -= phase2acc;
							}
						}
						npc.ai[2] += 1f;
						if (npc.ai[2] >= 200f) //计时器到时间了
						{
							npc.ai[1] = 1f; //开始冲刺
							npc.ai[2] = 0f;
							npc.ai[3] = 0f;
							if (Main.expertMode && (double)npc.life < (double)npc.lifeMax * 0.35) //生命值低于一定值直接忽略前几次冲刺
							{
								npc.ai[1] = 3f;
							}
							npc.target = 255;
							npc.netUpdate = true;
						}
						if (Main.expertMode && speedup3)
						{
							npc.TargetClosest(true);
							npc.netUpdate = true;
							npc.ai[1] = 3f;
							npc.ai[2] = 0f;
							npc.ai[3] -= 1000f;
						}
					}
					else if (npc.ai[1] == 1f)
					{
						//SoundEngine.PlaySound(36, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
						npc.rotation = targetangle;
						float dashnum = 6.8f;
						if (Main.expertMode && npc.ai[3] == 1f)
						{
							dashnum *= 1.15f;
						}
						if (Main.expertMode && npc.ai[3] == 2f)
						{
							dashnum *= 1.3f;
						}
						Vector2 position = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float targetdistanceX = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - position.X;
						float targetdistanceY = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - position.Y;
						float targetdistance = (float)Math.Sqrt((double)(targetdistanceX * targetdistanceX + targetdistanceY * targetdistanceY));
						targetdistance = dashnum / targetdistance;
						npc.velocity.X = targetdistanceX * targetdistance;
						npc.velocity.Y = targetdistanceY * targetdistance;
						npc.ai[1] = 2f; //加速后立刻变成下一个阶段
						npc.netUpdate = true;
						if (npc.netSpam > 10)
						{
							npc.netSpam = 10;
						}
					}
					else if (npc.ai[1] == 2f)
					{
						float timer = 40f;
						npc.ai[2] += 1f;
						if (Main.expertMode)
						{
							timer = 50f;
						}
						if (npc.ai[2] >= timer) //计时器
						{
							npc.velocity *= 0.97f; //减速停车
							if (Main.expertMode)
							{
								npc.velocity *= 0.98f;
							}
							if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
							{
								npc.velocity.X = 0f;
							}
							if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
							{
								npc.velocity.Y = 0f;
							}
						}
						else //变换旋转角度
						{
							npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;
						}
						int dashtimer = 130;
						if (Main.expertMode)
						{
							dashtimer = 90;
						}
						if (npc.ai[2] >= (float)dashtimer) //超过冲刺时间
						{
							npc.ai[3] += 1f; //冲刺次数增加
							npc.ai[2] = 0f;
							npc.target = 255;
							npc.rotation = targetangle;
							if (npc.ai[3] >= 3f) //冲刺三次之后
							{
								npc.ai[1] = 0f; //正常飞行
								npc.ai[3] = 0f; //零次冲撞
								if (Main.expertMode && Main.netMode != 1 && (double)npc.life < (double)npc.lifeMax * 0.5) //生命值低于一定
								{
									npc.ai[1] = 3f; //疯狗冲撞
									npc.ai[3] += (float)Main.rand.Next(1, 4);
								}
								npc.netUpdate = true;
								if (npc.netSpam > 10)
								{
									npc.netSpam = 10;
								}
							}
							else
							{
								npc.ai[1] = 1f; //否则继续冲刺
							}
						}
					}
					else if (npc.ai[1] == 3f) //疯狗
					{
						if (npc.ai[3] == 4f && speedup2 && npc.Center.Y > Main.player[npc.target].Center.Y)
						{
							npc.TargetClosest(true);
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
							npc.ai[3] = 0f;
							npc.netUpdate = true;
							if (npc.netSpam > 10)
							{
								npc.netSpam = 10;
							}
						}
						else if (Main.netMode != 1)
						{
							npc.TargetClosest(true);
							float dashnum = 20f;
							Vector2 position = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float targetdistanceX = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - position.X;
							float targetdistanceY = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - position.Y;
							float fastdashnum = Math.Abs(Main.player[npc.target].velocity.X) + Math.Abs(Main.player[npc.target].velocity.Y) / 4f;
							fastdashnum += 10f - fastdashnum;
							if (fastdashnum < 5f)
							{
								fastdashnum = 5f;
							}
							if (fastdashnum > 15f)
							{
								fastdashnum = 15f;
							}
							if (npc.ai[2] == -1f && !speedup3)
							{
								fastdashnum *= 4f;
								dashnum *= 1.3f;
							}
							if (speedup3)
							{
								fastdashnum *= 2f;
							}
							targetdistanceX -= Main.player[npc.target].velocity.X * fastdashnum;
							targetdistanceY -= Main.player[npc.target].velocity.Y * fastdashnum / 4f;
							targetdistanceX *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
							targetdistanceY *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
							if (speedup3)
							{
								targetdistanceX *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
								targetdistanceY *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
							}
							float targetdistance = (float)Math.Sqrt((double)(targetdistanceX * targetdistanceX + targetdistanceY * targetdistanceY));
							float targetdistance2 = targetdistance;
							targetdistance = dashnum / targetdistance;
							npc.velocity.X = targetdistanceX * targetdistance;
							npc.velocity.Y = targetdistanceY * targetdistance;
							npc.velocity.X += (float)Main.rand.Next(-20, 21) * 0.1f;
							npc.velocity.Y += (float)Main.rand.Next(-20, 21) * 0.1f;
							if (speedup3)
							{
								npc.velocity.X += (float)Main.rand.Next(-50, 51) * 0.1f;
								npc.velocity.Y += (float)Main.rand.Next(-50, 51) * 0.1f;
								float velXabs = Math.Abs(npc.velocity.X);
								float velYabs = Math.Abs(npc.velocity.Y);
								if (npc.Center.X > Main.player[npc.target].Center.X)
								{
									velYabs *= -1f;
								}
								if (npc.Center.Y > Main.player[npc.target].Center.Y)
								{
									velXabs *= -1f;
								}
								npc.velocity.X = velYabs + npc.velocity.X;
								npc.velocity.Y = velXabs + npc.velocity.Y;
								npc.velocity.Normalize();
								npc.velocity *= dashnum;
								npc.velocity.X += (float)Main.rand.Next(-20, 21) * 0.1f;
								npc.velocity.Y += (float)Main.rand.Next(-20, 21) * 0.1f;
							}
							else if (targetdistance2 < 100f)
							{
								if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
								{
									float velXabs = Math.Abs(npc.velocity.X);
									float velYabs = Math.Abs(npc.velocity.Y);
									if (npc.Center.X > Main.player[npc.target].Center.X)
									{
										velYabs *= -1f;
									}
									if (npc.Center.Y > Main.player[npc.target].Center.Y)
									{
										velXabs *= -1f;
									}
									npc.velocity.X = velYabs;
									npc.velocity.Y = velXabs;
								}
							}
							else if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
							{
								float velYdet = (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) / 2f;
								float velXdet = velYdet;
								if (npc.Center.X > Main.player[npc.target].Center.X)
								{
									velXdet *= -1f;
								}
								if (npc.Center.Y > Main.player[npc.target].Center.Y)
								{
									velYdet *= -1f;
								}
								npc.velocity.X = velXdet;
								npc.velocity.Y = velYdet;
							}
							npc.ai[1] = 4f;
							npc.netUpdate = true;
							if (npc.netSpam > 10)
							{
								npc.netSpam = 10;
							}
						}
					}
					else if (npc.ai[1] == 4f)
					{
						if (npc.ai[2] == 0f)
						{
							//SoundEngine.PlaySound(36, (int)npc.position.X, (int)npc.position.Y, -1, 1f, 0f);
						}
						float num62 = num5;
						npc.ai[2] += 1f;
						if (npc.ai[2] == num62 && Vector2.Distance(npc.position, Main.player[npc.target].position) < 200f)
						{
							npc.ai[2] -= 1f;
						}
						if (npc.ai[2] >= num62)
						{
							npc.velocity *= 0.95f;
							if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
							{
								npc.velocity.X = 0f;
							}
							if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
							{
								npc.velocity.Y = 0f;
							}
						}
						else
						{
							npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;
						}
						float num63 = num62 + 13f;
						if (npc.ai[2] >= num63)
						{
							npc.netUpdate = true;
							if (npc.netSpam > 10)
							{
								npc.netSpam = 10;
							}
							npc.ai[3] += 1f;
							npc.ai[2] = 0f;
							if (npc.ai[3] >= 5f)
							{
								npc.ai[1] = 0f;
								npc.ai[3] = 0f;
							}
							else
							{
								npc.ai[1] = 3f;
							}
						}
					}
					else if (npc.ai[1] == 5f)
					{
						float num64 = 600f;
						float num65 = 9f;
						float num66 = 0.3f;
						Vector2 vector11 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num67 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector11.X;
						float num68 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) + num64 - vector11.Y;
						float num69 = (float)Math.Sqrt((double)(num67 * num67 + num68 * num68));
						num69 = num65 / num69;
						num67 *= num69;
						num68 *= num69;
						float ptr;
						if (npc.velocity.X < num67)
						{
							npc.velocity.X += num66;
							if (npc.velocity.X < 0f && num67 > 0f)
							{
								npc.velocity.X += num66;
							}
						}
						else if (npc.velocity.X > num67)
						{
							npc.velocity.X -= num66;
							if (npc.velocity.X > 0f && num67 < 0f)
							{
								npc.velocity.X -= num66;
							}
						}
						if (npc.velocity.Y < num68)
						{
							npc.velocity.Y += num66;
							if (npc.velocity.Y < 0f && num68 > 0f)
							{
								npc.velocity.Y += num66;
							}
						}
						else if (npc.velocity.Y > num68)
						{
							npc.velocity.Y -= num66;
							if (npc.velocity.Y > 0f && num68 < 0f)
							{
								npc.velocity.Y -= num66;
							}
						}
						npc.ai[2] += 1f;
						if (npc.ai[2] >= 70f)
						{
							npc.TargetClosest(true);
							npc.ai[1] = 3f;
							npc.ai[2] = -1f;
							npc.ai[3] = (float)Main.rand.Next(-3, 1);
							npc.netUpdate = true;
						}
					}
					if (speedup3 && npc.ai[1] == 5f)
					{
						npc.ai[1] = 3f;
						//return;
					}
				}
				return true;
            }
            return base.PreAI(npc);
        }
    }
}
