using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace DeusMod.Helpers
{
    public static class DeusModSoundHelper
    {
        #region 玩家音效
        //玩家受伤，共3种
        public static SoundStyle PlayerHit => SoundID.PlayerHit;
        //女性玩家受伤的音效，共3种
        public static SoundStyle FemalePlayerHit => SoundID.FemaleHit;
        //玩家死亡 
        public static SoundStyle PlayerKilled => SoundID.PlayerKilled;
        //捡起物品的声音（也可用于点击按钮） 
        public static SoundStyle Grab => SoundID.Grab;
        //魔力值回复至满时的音效，防反饰品冷却结束，部分武器射弹幕冷却时间结束时的音效（如冰雪剑）  
        public static SoundStyle MaxManaBlip => SoundID.MaxMana;
        //玩家溺水/淹死/缺少氧气的音效  
        public static SoundStyle Drown => SoundID.Drown;
        //打开NPC聊天面板时的音效 
        public static SoundStyle ChatSound => SoundID.Chat;
        //二段跳的音效，例如云瓶，独角兽坐骑等 
        public static SoundStyle DoubleJump => SoundID.DoubleJump;
        //疾跑的音效，比如赫尔墨斯鞋，泰拉闪耀鞋等 
        public static SoundStyle RunSlideSound => SoundID.Run;
        //微光BUFF消失时的音效 
        public static SoundStyle ShimmeringExpires => SoundID.Shimmer2;
        #endregion

        #region 物品音效
        #region 剑
        //原版大多数近战武器和工具的使用的音效
        public static SoundStyle Sword_Wield_Vanilla => SoundID.Item1;
        //mod挥剑的音效，清脆
        public static SoundStyle Sword_Wield => new SoundStyle($"{nameof(DeusMod)}/Sounds/Swords/SwordWield");
        //mod挥剑的二号音效，更清脆
        public static SoundStyle Sword_Wield2 => new SoundStyle($"{nameof(DeusMod)}/Sounds/Swords/SwordWield2");
        //mod挥剑的三号音效，更慢
        public static SoundStyle Sword_Wield3 => new SoundStyle($"{nameof(DeusMod)}/Sounds/Swords/SwordWield3");
        //mod非完美拼刀音效，偏弱
        public static SoundStyle Sword_Clash_Normal => new SoundStyle($"{nameof(DeusMod)}/Sounds/Swords/SwordClashNormal");
        //mod完美拼刀音效，清脆且持续更长
        public static SoundStyle Sword_Clash_Perfect => new SoundStyle($"{nameof(DeusMod)}/Sounds/Swords/SwordClashPerfect");
        #endregion
        #region 枪

        //原版大部分枪的音效，鉴于较为脆弱，较少使用
        public static SoundStyle Gun_Vanilla => SoundID.Item11;
        //mod转轮手枪的音效，特点是粗犷
        public static SoundStyle Gun_Revolver => new SoundStyle($"{nameof(DeusMod)}/Sounds/Ranged/Guns/Undertaker/UndertakerShoot");
        //原版手枪使用的音效，同样太脆
        public static SoundStyle Gun_Handgun_Vanilla => SoundID.Item41;
        //原版霰弹枪的音效（带抛壳）
        public static SoundStyle Gun_Shotgun_Vanilla_Egect => SoundID.Item36;
        //原版霰弹枪的音效（无抛壳）
        public static SoundStyle Gun_Shotgun_Vanilla_NoEgect => SoundID.Item38;
        //原版狙击步枪的音效（无抛壳）
        public static SoundStyle Gun_Rifle_Vanilla => SoundID.Item40;
        //使用外星霰弹枪的音效
        public static SoundStyle Xenopopper => SoundID.Item95;
        //外星霰弹枪泡泡的音效
        public static SoundStyle Xenopopper_Bubble => SoundID.Item96;
        //飞镖手枪射击的音效
        public static SoundStyle DartPistol => SoundID.Item98;
        //飞镖步枪射击的音效
        public static SoundStyle DartRifle => SoundID.Item99;
        //钉枪射击的音效
        public static SoundStyle NailGun => SoundID.Item108;


        #endregion
        #region 各种激光

        //各种激光射线类武器的声音，Biu~
        public static SoundStyle Laser_Item12 => SoundID.Item12;

        //棱镜，美杜莎头，各种陨石光剑的声音，嗡~~
        public static SoundStyle LaserSwing_Item15 => SoundID.Item15;

        //射激光的声音，比方说激光眼的射击声音，啾！啾！啾！
        public static SoundStyle LaserShoot_Item33 => SoundID.Item33;

        //脉冲弓的声音，Jiu~</summary>
        public static SoundStyle LaserShoot2_Item75 => SoundID.Item75;

        //激光加特林的使用声音</summary>
        public static SoundStyle LaserMachinegun_Item91 => SoundID.Item91;

        //太空枪的声音
        public static SoundStyle SpaceGun_Item157 => SoundID.Item157;

        /// 灰光枪，橙光枪的声音
        public static SoundStyle LaserGun_Item158 => SoundID.Item158;



        #endregion
        #region 乐器

        //竖琴的声音（你可以自己调整音高来实现原版那种根据鼠标与人物距离不同音高不同的效果）
        public static SoundStyle Harp_Item26 => SoundID.Item26;

        //The Axe ：电吉他斧的声音
        public static SoundStyle ElectricGuitar_Item47 => SoundID.Item47;

        //lvy，雨歌，星星吉他弹奏的声音</summary>
        public static SoundStyle GuitarC_Item133 => SoundID.Item133;

        //lvy，雨歌，星星吉他弹奏的声音
        public static SoundStyle GuitarD_Item134 => SoundID.Item134;

        //lvy，雨歌，星星吉他弹奏的声音
        public static SoundStyle GuitarEm_Item135 => SoundID.Item135;

        //lvy，雨歌，星星吉他弹奏的声音
        public static SoundStyle GuitarG_Item136 => SoundID.Item136;

        //lvy，雨歌，星星吉他弹奏的声音</summary>
        public static SoundStyle GuitarBm_Item137 => SoundID.Item137;

        //lvy，雨歌，星星吉他弹奏的声音
        public static SoundStyle GuitarAm_Item138 => SoundID.Item138;

        //架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumHiHat_Item139 => SoundID.Item139;

        //架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumTomHigh_Item140 => SoundID.Item140;

        //架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumTomLow_Item141 => SoundID.Item141;

        //架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumTomMid_Item142 => SoundID.Item142;

        //架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）
        public static SoundStyle DrumClosedHiHat_Item143 => SoundID.Item143;

        /// 架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumCymbal1_Item144 => SoundID.Item144;

        //架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumCymbal2_Item145 => SoundID.Item145;

        //架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumKick_Item146 => SoundID.Item146;

        /// 架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumTamaSnare_Item147 => SoundID.Item147;

        //架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）
        public static SoundStyle DrumFloorTom_Item148 => SoundID.Item148;

        #endregion

        //----------其他-----------



        /// 吃东西，召唤宠物的声音</summary>
        public static SoundStyle Eat_Item2 => SoundID.Item2;

        //喝药水的声音
        public static SoundStyle Drink_Item3 => SoundID.Item3;

        //生命水晶等的Ding~
        public static SoundStyle Ding_Item4 => SoundID.Item4;

        //弓射箭的声音，大概是Jiu~
        public static SoundStyle Bow_Item5 => SoundID.Item5;

        //各种传送的声音，比如魔镜</summary>
        public static SoundStyle Teleport_Item6 => SoundID.Item6;

        //各种回旋镖，圣骑士锤，吹叶机（世花掉的那个魔法武器）的声音</summary>
        public static SoundStyle Swing2_Item7 => SoundID.Item7;

        /// 前期各种矿物法杖法杖，一些NPC的声音，类似weng~
        public static SoundStyle MagicStaff_Item8 => SoundID.Item8;

        /// 魔晶风暴，裂天剑等的声音，类似Xiu~</summary>
        public static SoundStyle MagicShoot_Item9 => SoundID.Item9;

        /// 石巨人拳头之类的，还有子弹落地的声音，Da!
        public static SoundStyle Hit_Item10 => SoundID.Item10;

        /// 喷水的声音，例如海蓝法杖，黄金雨，滋~~~
        public static SoundStyle WaterShoot_Item13 => SoundID.Item13;

        /// 小炸弹的声音，例如各种手雷
        public static SoundStyle Boom_Item14 => SoundID.Item14;

        //放屁声音</summary>
        public static SoundStyle Fart_Item16 => SoundID.Item16;

        //各种怪射毒刺的声音，黄蜂，毒刺史莱姆之类的
        public static SoundStyle Stinger_Item17 => SoundID.Item17;

        //原版中没用到
        public static SoundStyle NoUse_Item18 => SoundID.Item18;

        //原版中没用到
        public static SoundStyle NoUse_Item19 => SoundID.Item19;

        //类似火焰的声音，另外星云拳套，恶魔三叉戟等也用到了
        public static SoundStyle Flame_Item20 => SoundID.Item20;

        //水箭魔法书的声音
        public static SoundStyle WaterBolt_Item21 => SoundID.Item21;

        //电锯，钻头之类的使用声音，类似拉动引擎
        public static SoundStyle Drill_Item22 => SoundID.Item22;

        /// 电锯，钻头之类的的使用声音</summary>
        public static SoundStyle Drill2_Item23 => SoundID.Item23;

        //各类悬浮推进器悬浮时的声音，嗡嗡嗡
        public static SoundStyle Floating_Item24 => SoundID.Item24;

        //召唤坐骑以及各种小妖精宠物的声音
        public static SoundStyle MountSummon_Item25 => SoundID.Item25;

        //碎冰和各种冰弹幕碎裂的声音（还有穿着寒霜套的玩家受伤的声音）
        public static SoundStyle CrushedIce_Item27 => SoundID.Item27;

        //各种冰魔法杖的声音，还有符文法师的攻击声音
        public static SoundStyle IceMagic_Item28 => SoundID.Item28;

        //使用魔力星的声音
        public static SoundStyle ManaCrystal_Item29 => SoundID.Item29;

        //冰魔杖放置冰块的声音</summary>
        public static SoundStyle IcePlaced_Item30 => SoundID.Item30;

        //翅膀飞行的声音
        public static SoundStyle Wing_Item32 => SoundID.Item32;

        //喷火器的声音
        public static SoundStyle Flamethrower_Item34 => SoundID.Item34;

        //铃铛的声音，叮~（听起来可能更接近于噔~）
        public static SoundStyle Bell_Item35 => SoundID.Item35;

        //锤子敲铁砧的声音，也是哥布林重铸的声音
        public static SoundStyle Knock_Item37 => SoundID.Item37;

        //扔飞刀的声音，吸血鬼刀，腐化灾兵，剃刀松使用的音效。
        public static SoundStyle ThrowKnives_Item39 => SoundID.Item39;

        //高级些的法杖的使用音效，比如雷电法杖，毒液法杖等
        public static SoundStyle MagicStaff2_Item43 => SoundID.Item42;

        //一些召唤杖的声音，比如史莱姆法杖
        public static SoundStyle SummonStaff_Item44 => SoundID.Item44;

        //巨鹿掉的眼球塔召唤杖的攻击音效
        public static SoundStyle FireBall_Item45 => SoundID.Item45;

        //蜘蛛女王召唤杖和冰霜九头蛇召唤杖的声音
        public static SoundStyle HorribleSummon_Item46 => SoundID.Item46;

        /// 雪球碎裂的声音
        public static SoundStyle SnowBall_Item51 => SoundID.Item51;

        //矿车铁轨放置时的声音，Ding!
        public static SoundStyle MinecartTrack_Item52 => SoundID.Item52;

        //矿车上铁轨时的声音，Ding-
        public static SoundStyle MinecartHit_Item53 => SoundID.Item53;

        /// 气泡破了时候的声音，Pa-
        public static SoundStyle Bubble_Item54 => SoundID.Item54;

        //矿车减速的声音，chi-
        public static SoundStyle MinecartSlowDown_Item55 => SoundID.Item55;

        //矿车反弹的声音（我不知道这个在哪里用到了），duang~~~
        public static SoundStyle MinecartBounces_Item56 => SoundID.Item56;

        //彩虹猫剑的音效，共2种
        public static SoundStyle Meowmere => SoundID.Meowmere;

        //彩虹猫剑的声音，喵呜~
        public static SoundStyle Mio_Mewo_Miao_Item57 => SoundID.Item57;

        //彩虹猫剑的声音，喵呜~
        public static SoundStyle Mio_Mewo_Miao_Item58 => SoundID.Item58;

        //猪猪存钱罐召唤时的声音，哼~
        public static SoundStyle MoneyPig_Item59 => SoundID.Item59;

        //泰拉之刃，沙漠精的某个招式的声音，类似能量波
        public static SoundStyle TerraBlade_Item60 => SoundID.Item60;

        //榴弹发射器的声音（世纪之花掉落），巨鹿掉落的气喇叭的声音
        public static SoundStyle GrenadeLauncher_Item61 => SoundID.Item61;

        //榴弹爆炸的声音
        public static SoundStyle BigBOOM_Item62 => SoundID.Item62;

        //吹管的声音
        public static SoundStyle Blowpipe_Item63 => SoundID.Item63;

        //强化吹管，工程蓝图的声音（这玩意也有声音？）/
        public static SoundStyle Blowgun_Item64 => SoundID.Item64;

        /// 原版未使用，感觉像是强化再强化的吹管的声音</summary>
        public static SoundStyle NoUse_BlowgunPlus_Item65 => SoundID.Item65;

        //雷云法杖，巨鹿掉的天气魔棒的声音
        public static SoundStyle StrongWinds_Item66 => SoundID.Item66;

        //彩虹枪的声音
        public static SoundStyle RainbowGun_Item67 => SoundID.Item67;

        //原版未使用，听上去像那种非常炫酷的魔法射击的声音
        public static SoundStyle NoUse_SuperMagicShoot_Item68 => SoundID.Item68;

        //大地魔杖的声音（石巨人掉的那个放滚石的)</summary>
        public static SoundStyle StaffOfEarth_Item69 => SoundID.Item69;

        //石头碎裂的声音</summary>
        public static SoundStyle StoneBurst_Item70 => SoundID.Item70;

        //斩击的声音，死神镰刀使用了
        public static SoundStyle Slash_Item71 => SoundID.Item71;

        //暗影束法杖的声音，Diu--
        public static SoundStyle ShadowBeam_Item72 => SoundID.Item72;

        /// 狱火叉的使用声音</summary>
        public static SoundStyle FireFork_Item73 => SoundID.Item73;

        //狱火叉的火球爆炸的声音
        public static SoundStyle FireBallExplosion_Item74 => SoundID.Item74;

        //黄蜂法杖的召唤声音</summary>
        public static SoundStyle HoneyStaffSummon_Item76 => SoundID.Item76;

        /// 小鬼法杖的召唤声音</summary>
        public static SoundStyle FireStaffSummon_Item77 => SoundID.Item77;

        /// 月亮门法杖，蜘蛛女王法杖，七彩水晶法杖的召唤声音</summary>
        public static SoundStyle SpecialSummon_Item78 => SoundID.Item78;

        /// 白色的胡萝卜的召唤声音，召唤兔子坐骑
        public static SoundStyle RabbitMount_Item79 => SoundID.Item79;

        //猪鱼坐骑的召唤声音（是猪龙稀有掉落物的那个）</summary>
        public static SoundStyle PigronMount_Item80 => SoundID.Item80;

        //史莱姆坐骑的召唤声音
        public static SoundStyle SlimeMount_Item81 => SoundID.Item81;

        //泰拉棱镜，双子眼召唤杖的召唤声音，星光灯笼的使用声音</summary>
        public static SoundStyle TerraprismaSummon_Item82 => SoundID.Item82;

        /// 蜘蛛法杖的声音</summary>
        public static SoundStyle SpiderStaff_Item83 => SoundID.Item83;

        /// 水龙卷刃的使用声音</summary>
        public static SoundStyle WaterTyphoon_Item84 => SoundID.Item84;

        //泡泡枪的使用声音</summary>
        public static SoundStyle BubbleGun_Item85 => SoundID.Item85;

        /// 原版未使用，水滴落入水中的声音</summary>
        public static SoundStyle NoUse_WaterDrop_Item86 => SoundID.Item86;

        //原版未使用，水滴落入水中的声音
        public static SoundStyle NoUse_WaterDrop2_Item87 => SoundID.Item87;

        /// 月曜，陨石法杖的使用声音
        public static SoundStyle LunarFlare_Item88 => SoundID.Item88;

        /// 陨石法杖的爆炸声音</summary>
        public static SoundStyle MeteorImpact_Item89 => SoundID.Item89;

        /// 外星蛞蝓的召唤声音，感觉可以用于魔法弹幕射击的声音</summary>
        public static SoundStyle ScutlixMount_Item90 => SoundID.Item90;

        /// 战斗指南1，2的使用声音，3种动物通行卷的使用声音</summary>
        public static SoundStyle NPCSummon_NPCStrengthen_Item92 => SoundID.Item92;

        /// 原版未使用，类似电流的声音
        public static SoundStyle NoUse_Electric_Item93 => SoundID.Item93;

        //电圈发射器的声音</summary>
        public static SoundStyle ElectricExplosion_Item94 => SoundID.Item94;

        //蜂膝弓射击的声音
        public static SoundStyle TheBeesKnees_Item97 => SoundID.Item97;

        //诅咒焰法杖的声音（放诅咒焰火墙的那个）
        public static SoundStyle ClingerStaff_Item100 => SoundID.Item100;

        //水晶魔棒的声音（神圣大宝箱怪的那个）
        public static SoundStyle Crystal_Item101 => SoundID.Item101;

        //暗影焰弓，空中灾祸的声音</summary>
        public static SoundStyle Bow2_Item102 => SoundID.Item102;

        //暗影焰妖娃的声音
        public static SoundStyle DeathCalling_Item103 => SoundID.Item103;

        //原版未使用，和暗影焰妖娃的声音类似
        public static SoundStyle NoUse_DeathCalling2_Item104 => SoundID.Item104;

        //狂星之怒的声音
        public static SoundStyle StarFalling_Item105 => SoundID.Item105;

        //扔瓶子的的声音，各种环境球也是这个声音（改变背景的那个东西）</summary>
        public static SoundStyle ThrowBottle_Item106 => SoundID.Item106;

        //瓶子碎裂的的声音，各种环境球也是这个声音（改变背景的那个东西）
        public static SoundStyle BottleExplosion_Item107 => SoundID.Item107;

        /// 水晶蛇的声音
        public static SoundStyle CrystalSerpent_Item109 => SoundID.Item109;

        //水晶蛇弹幕的声音，但是听上去像是烟花的声音</summary>
        public static SoundStyle Firework_Item110 => SoundID.Item110;

        /// 叫什么忘记了，总之是肉后腐化之地钓鱼获得的那个武器，射出毒泡泡的声音
        public static SoundStyle ToxicBubble_Item111 => SoundID.Item111;

        //泡泡的声音，和上面的类似
        public static SoundStyle NoUse_ToxicBubble2_Item112 => SoundID.Item112;

        //血荆棘的声音，致命球法杖召唤的声音
        public static SoundStyle BloodThron_Item113 => SoundID.Item113;

        //传送枪的声音</summary>
        public static SoundStyle PortalGun_Item114 => SoundID.Item114;

        //传送枪的声音</summary>
        public static SoundStyle PortalGun2_Item115 => SoundID.Item115;

        /// 太阳能喷发的声音</summary>
        public static SoundStyle SolarEruption_Item116 => SoundID.Item116;

        /// 神灯烈焰，星云奥秘的声音</summary>
        public static SoundStyle SpiritFlame_Item117 => SoundID.Item117;

        //还是水晶蛇弹幕的声音（一把武器3个音效可还行）
        public static SoundStyle LiteBoom_Item118 => SoundID.Item118;

        //高尔夫挥杆的声音
        public static SoundStyle Golf_Item126 => SoundID.Item126;

        //高尔夫球哨的声音</summary>
        public static SoundStyle GolfWhistle_Item128 => SoundID.Item128;

        //高尔夫进球得分的声音
        public static SoundStyle GetScores_Item129 => SoundID.Item129;

        /// 虚空袋的声音
        public static SoundStyle VoidBag_Item130 => SoundID.Item130;

        /// 原版未使用，听不出是什么，类似什么东西摩擦滑行的声音</summary>
        public static SoundStyle NoUse_Item131 => SoundID.Item131;

        /// 激光钻头的声音（这玩意是这个声音的？）
        public static SoundStyle LaserDrill_Item132 => SoundID.Item132;

        //弹幕被反弹时的声音，比如ftw克眼旋转时，还有微光史莱姆，大宝箱怪等
        public static SoundStyle ProjectileReflected_Item150 => SoundID.Item150;

        //耍蛇者长笛的声音（召唤蛇蛇绳索的那个笛子）</summary>
        public static SoundStyle Snake_Item151 => SoundID.Item151;

        /// 鞭子甩出的声音
        public static SoundStyle WhipSwing_Item152 => SoundID.Item152;

        /// 鞭子命中的声音，Pia!</summary>
        public static SoundStyle WhipHit_Item153 => SoundID.Item153;

        //喜庆弹射器的声音，放烟花
        public static SoundStyle Firework2_Item156 => SoundID.Item156;

        /// 音乐盒记录的声音
        public static SoundStyle MusicBoxRecorded_Item166 => SoundID.Item166;

        //蹦蹦跷召唤的声音
        public static SoundStyle PogoStickMount_Item168 => SoundID.Item168;

        /// 天顶剑的声音，其实是复制的Item1，只不过调整了音高，听起来更沉重
        public static SoundStyle Zenith_Item169 => SoundID.Item169;

        //1.4.4蜂后召唤物，幼虫的声音
        public static SoundStyle BugsScream_Item173 => SoundID.Item173;

        /// 1.4.4KO大炮的声音</summary>
        public static SoundStyle KOCannon_Item174 => SoundID.Item174;

        //1.4.4拍拍手的声音（超高击退的那个玩具武器）
        public static SoundStyle SlapHand_Item175 => SoundID.Item175;

        //1.4.4便便的声音
        public static SoundStyle Poo_Item177 => SoundID.Item177;

        /// 1.4.4华夫铁的声音
        public static SoundStyle WafflesIron_Item178 => SoundID.Item178;

        //钱币放置，堆叠，从NPC处买卖物品的音效
        public static SoundStyle Coins => SoundID.Coins;

        //钱币捡起的声音，共5种</summary>
        public static SoundStyle CoinPickup => SoundID.CoinPickup;

        #endregion

        #region NPC相关音效

        #region NPC受击音效

        //大多数肉质生物的受击的声音，举例：史莱姆，僵尸，克眼
        public static SoundStyle Fleshy_NPCHit1 => SoundID.NPCHit1;

        //大多数骨头生物的受击的声音，举例：骷髅
        public static SoundStyle Bone_NPCHit2 => SoundID.NPCHit2;

        //火球命中的声音
        public static SoundStyle FireBallHit_NPCHit3 => SoundID.NPCHit3;

        //大部分金属制敌人的受击声音，举例：圣骑士，附魔圣剑，双子魔眼2阶段
        public static SoundStyle Metal_NPCHit4 => SoundID.NPCHit4;

        //小精灵，冰精灵，冰霜巨人的受击音效
        public static SoundStyle Fairy_NPCHit5 => SoundID.NPCHit5;

        /// 类似狼的叫声，各种狼，狼人还有一些“野兽”的受击音效
        public static SoundStyle Wolf_NPCHit6 => SoundID.NPCHit6;

        /// 也是类似肉体的受击音效，例如小白龙，冰霜女皇等</summary>
        public static SoundStyle Fleshy2_NPCHit7 => SoundID.NPCHit7;

        //血肉之墙 a.k.a 肉山的受击音效
        public static SoundStyle WallOfFlesh_NPCHit8 => SoundID.NPCHit8;

        //类似血肉的受击音效，克苏鲁之脑，飞眼怪（Creeper?），肉山及肉山召唤的蠕虫的受击音效</summary>
        public static SoundStyle Bloody_NPCHit9 => SoundID.NPCHit9;

        //原版未使用，听上去像什么野兽的受击音效，和上面的Wolf挺像</summary>
        public static SoundStyle NoUse_Beast_NPCHit10 => SoundID.NPCHit10;

        /// 雪人军团的怪，风滚草，日耀蜈蚣的受击音效（很难形容是个什么......）
        public static SoundStyle SnowMan_NPCHit11 => SoundID.NPCHit11;

        //无头骑士，独角兽的受击音效
        public static SoundStyle Unicorn_NPCHit12 => SoundID.NPCHit12;

        //脓血乌贼，血腥水母，沙漠蝎子，血腥食人鱼的受击音效
        public static SoundStyle Bloody2_NPCHit13 => SoundID.NPCHit13;

        //猪鲨的受击音效
        public static SoundStyle DukeFishron_NPCHit14 => SoundID.NPCHit14;

        /// 傀儡 a.k.a 训练假人的受击音效
        public static SoundStyle TargetDummy_NPCHit15 => SoundID.NPCHit15;

        /// 傀儡 a.k.a 训练假人的受击音效</summary>
        public static SoundStyle TargetDummy2_NPCHit16 => SoundID.NPCHit16;

        //傀儡 a.k.a 训练假人的受击音效</summary>
        public static SoundStyle TargetDummy3_NPCHit17 => SoundID.NPCHit17;

        /// 鲜血僵尸，僵尸鱼人的受击音效（都是血月的怪）</summary>
        public static SoundStyle Bloddy3_NPCHit18 => SoundID.NPCHit18;

        /// 滴滴怪的受击音效，还有荷兰人飞艇（这东西是这个音效的？感觉可能是因为平常打的都是它的炮）</summary>
        public static SoundStyle Drippler_NPCHit19 => SoundID.NPCHit19;

        /// 血蜘蛛的受击音效
        public static SoundStyle BloodCrawler_NPCHit20 => SoundID.NPCHit20;

        /// 恶魔，红恶魔，巫毒恶魔的受击音效
        public static SoundStyle Demon_NPCHit21 => SoundID.NPCHit21;

        /// 跳跳兽的受击音效（肉后丛林的那个蓝色的虫子）
        public static SoundStyle Derpling_NPCHit22 => SoundID.NPCHit22;

        //飞蛇，沙尘精的受击音效</summary>
        public static SoundStyle FlyingSnake_NPCHit23 => SoundID.NPCHit23;

        //巨型陆龟 a.k.a 丛林王八 a.k.a 丛林核弹 a.k.a 萌新杀手......（编不下去了），冰雪陆龟的受击音效
        public static SoundStyle GiantTortoise_NPCHit24 => SoundID.NPCHit24;

        /// 3种颜色的水母的受击音效
        public static SoundStyle Jellyfish_NPCHit25 => SoundID.NPCHit25;

        //丛林蜥蜴的受击音效</summary>
        public static SoundStyle Lihzahrd_NPCHit26 => SoundID.NPCHit26;

        //猪龙的受击音效
        public static SoundStyle Pigron_NPCHit27 => SoundID.NPCHit27;

        //秃鹫/秃鹰的受击音效
        public static SoundStyle Vulture_NPCHit28 => SoundID.NPCHit28;

        //黑隐士，爬藤怪的受击音效（蜘蛛洞的2种蜘蛛）</summary>
        public static SoundStyle Spider_NPCHit29 => SoundID.NPCHit29;

        //愤怒雨云的受击音效
        public static SoundStyle AngryNimbus_NPCHit30 => SoundID.NPCHit30;

        /// 各种蚁狮的受击音效（除了蚁狮蜂）</summary>
        public static SoundStyle JuicyBug_NPCHit31 => SoundID.NPCHit31;

        //蚁狮蜂的受击音效</summary>
        public static SoundStyle FlyingBug_NPCHit32 => SoundID.NPCHit32;

        //龙虾的受击音效
        public static SoundStyle Crawdad_NPCHit33 => SoundID.NPCHit33;

        //地牢幽魂/灵魂/鬼魂的受击音效/
        public static SoundStyle DungeonSpirit_NPCHit36 => SoundID.NPCHit36;

        /// 各种食尸鬼的受击音效（肉后地底沙漠的那个）
        public static SoundStyle Ghouls_NPCHit37 => SoundID.NPCHit37;

        //巨型卷壳怪的受击音效
        public static SoundStyle GiantShelly_NPCHit38 => SoundID.NPCHit38;

        //哥布林巫师/召唤师的受击音效（因为没有召唤的掉落物而惨遭改名的那个）
        public static SoundStyle GoblinWarlock_NPCHit40 => SoundID.NPCHit40;

        /// 岩石巨人，花岗岩傀儡/巨人的受击音效</summary>
        public static SoundStyle RockGolem_NPCHit41 => SoundID.NPCHit41;

        //蘑菇瓢虫的受击音效
        public static SoundStyle MushiLadybug_NPCHit45 => SoundID.NPCHit45;

        /// 鹦鹉的受击音效</summary>
        public static SoundStyle Parrot_NPCHit46 => SoundID.NPCHit46;

        //蝾螈 a.k.a 鸭鸭怪的受击音效</summary>
        public static SoundStyle Salamander_NPCHit50 => SoundID.NPCHit50;

        /// 暗影焰幻鬼的受击音效（哥布林巫师/召唤师召唤出来的）</summary>
        public static SoundStyle ShadowflameApparition_NPCHit52 => SoundID.NPCHit52;

        /// 幻灵/幽灵的受击音效（砸恶魔祭坛出一堆的那个）</summary>
        public static SoundStyle Wraith_NPCHit54 => SoundID.NPCHit54;

        #endregion
        #region NPC死亡

        //各种肉体生物的死亡音效，听上去像是肉被碾碎了的声音，举例：各种小动物，克眼，史莱姆
        public static SoundStyle Fleshy_NPCDeath1 => SoundID.NPCDeath1;

        /// 各种类人生物的死亡音效，举例：僵尸，骷髅</summary>
        public static SoundStyle HumanoidsDeath_NPCDeath2 => SoundID.NPCDeath2;

        //火球命中的声音</summary>
        public static SoundStyle FireBallDrath_NPCDeath3 => SoundID.NPCDeath3;

        //类似野兽一样的死亡声音，比如冰霜女皇，哀木，骨蛇等</summary>
        public static SoundStyle BeastDeath_NPCDeath5 => SoundID.NPCDeath5;

        //听上去像灵魂消散的声音的死亡声音，比如渔夫，鬼魂，荧光蝙蝠，荧光史莱姆，沙尘精等
        public static SoundStyle SpiritDeath_NPCDeath6 => SoundID.NPCDeath6;

        //小精灵的死亡声音，比如小精灵，冰精灵，冰霜巨人等</summary>
        public static SoundStyle FairyDeath_NPCDeath7 => SoundID.NPCDeath7;

        //小白龙，不感恩的火鸡的死亡声音
        public static SoundStyle Wyvern_NPCDeath8 => SoundID.NPCDeath8;

        //魔唾液的死亡声音（腐化者和世界吞噬者吐出的东西，这东西也有死亡音效啊...）
        public static SoundStyle VileSpit_NPCDeath9 => SoundID.NPCDeath9;

        //血肉之墙 a.k.a 肉山的死亡声音
        public static SoundStyle WallOfFlesh_NPCDeath10 => SoundID.NPCDeath10;

        /// 克苏鲁之脑，飞眼怪（Creeper?），肉山，蚁狮卵的死亡声音</summary>
        public static SoundStyle BloodyDeath_NPCDeath11 => SoundID.NPCDeath11;

        //饿鬼，肉山召唤的蠕虫的死亡声音</summary>
        public static SoundStyle BloodyDeath2_NPCDeath12 => SoundID.NPCDeath12;

        //肉山召唤的蠕虫时声音，听上去像呕吐的声音</summary>
        public static SoundStyle LeechSummoned_NPCDeath13 => SoundID.NPCDeath13;

        //和Item14基本一样，各种机器生物死亡音效
        public static SoundStyle Boom_NPCDeath14 => SoundID.NPCDeath14;

        //雪人军团的怪死亡音效，其他还有愤怒雨云，永恒水晶（旧日军团召唤物）</summary>
        public static SoundStyle SnowManDeath_NPCDeath15 => SoundID.NPCDeath15;

        /// 各种染料甲虫的死亡音效
        public static SoundStyle Beetles_BugDeath_NPCDeath16 => SoundID.NPCDeath16;

        /// 无头骑士，独角兽的死亡音效</summary>
        public static SoundStyle Unicorn_NPCDeath18 => SoundID.NPCDeath18;

        /// 脓血乌贼，血腥水母，沙漠蝎子，血腥食人鱼，猪鲨泡泡的死亡音效</summary>
        public static SoundStyle BloodyDeath2_NPCDeath19 => SoundID.NPCDeath19;

        //猪鲨的死亡音效</summary>
        public static SoundStyle DukeFishron_NPCDeath20 => SoundID.NPCDeath20;

        //鲜血僵尸，僵尸鱼人的死亡音效
        public static SoundStyle BloodyDeath3_NPCDeath21 => SoundID.NPCDeath21;

        /// 滴滴怪，日耀蜈蚣，荷兰人飞艇的死亡音效
        public static SoundStyle Drippler_NPCDeath22 => SoundID.NPCDeath22;

        /// 血蜘蛛的死亡音效
        public static SoundStyle BloodCrawler_NPCDeath23 => SoundID.NPCDeath23;

        //恶魔，红恶魔，巫毒恶魔的死亡音效
        public static SoundStyle Demon_NPCDeath24 => SoundID.NPCDeath24;

        //跳跳兽的死亡音效（肉后丛林的那个蓝色的虫子）
        public static SoundStyle Derpling_NPCDeath25 => SoundID.NPCDeath25;

        //飞蛇的死亡音效
        public static SoundStyle FlyingSnake_NPCDeath26 => SoundID.NPCDeath26;

        //巨型陆龟 a.k.a 丛林王八 a.k.a 丛林核弹 a.k.a 萌新杀手......（编不下去了），冰雪陆龟的死亡音效</summary>
        public static SoundStyle GiantTortoise_NPCDeath27 => SoundID.NPCDeath27;

        //3种颜色的水母的死亡音效
        public static SoundStyle Jellyfish_NPCDeath28 => SoundID.NPCDeath28;

        /// 丛林蜥蜴的死亡音效
        public static SoundStyle Lihzahrd_NPCDeath29 => SoundID.NPCDeath29;

        /// 猪龙的死亡音效
        public static SoundStyle Pigron_NPCDeath30 => SoundID.NPCDeath30;

        /// 秃鹫/秃鹰的死亡音效
        public static SoundStyle Vulture_NPCDeath31 => SoundID.NPCDeath31;

        //黑隐士，爬藤怪的死亡音效（蜘蛛洞的2种蜘蛛）
        public static SoundStyle Spider_NPCDeath32 => SoundID.NPCDeath32;

        //愤怒雨云的死亡音效</summary>
        public static SoundStyle AngryNimbus_NPCDeath33 => SoundID.NPCDeath33;

        /// 各种蚁狮的死亡音效（除了蚁狮蜂）</summary>
        public static SoundStyle JuicyBugDeath_NPCDeath34 => SoundID.NPCDeath34;

        //蚁狮蜂的死亡音效</summary>
        public static SoundStyle FlyingBugDeath_NPCDeath35 => SoundID.NPCDeath35;

        //龙虾的死亡音效</summary>
        public static SoundStyle Crawdad_NPCDeath36 => SoundID.NPCDeath36;

        /// 地牢幽魂/灵魂/鬼魂，沙尘精的死亡音效</summary>
        public static SoundStyle DungeonSpirit_NPCDeath39 => SoundID.NPCDeath39;

        //各种食尸鬼的死亡音效（肉后地底沙漠的那个）
        public static SoundStyle Ghouls_NPCDeath40 => SoundID.NPCDeath40;

        //巨型卷壳怪的死亡音效
        public static SoundStyle GiantShelly_NPCDeath41 => SoundID.NPCDeath41;

        /// 哥布林巫师/召唤师的死亡音效（因为没有召唤的掉落物而惨遭改名的那个）
        public static SoundStyle GoblinWarlock_NPCDeath42 => SoundID.NPCDeath42;

        //岩石巨人，花岗岩傀儡/巨人的死亡音效
        public static SoundStyle RockGolem_NPCDeath43 => SoundID.NPCDeath43;

        /// 蘑菇瓢虫的死亡音效</summary>
        public static SoundStyle MushiLadybug_NPCDeath47 => SoundID.NPCDeath47;

        //鹦鹉的死亡音效
        public static SoundStyle Parrot_NPCDeath48 => SoundID.NPCDeath48;

        //幻灵/幽灵的死亡音效（砸恶魔祭坛出一堆的那个）</summary>
        public static SoundStyle Wraith_NPCDeath52 => SoundID.NPCDeath52;

        //蝾螈 a.k.a 鸭鸭怪的死亡音效</summary>
        public static SoundStyle Salamander_NPCDeath53 => SoundID.NPCDeath53;

        //暗影焰幻鬼的死亡音效（哥布林巫师/召唤师召唤出来的）
        public static SoundStyle ShadowflameApparition_NPCDeath55 => SoundID.NPCDeath55;

        /// 四柱护盾碎裂的音效
        public static SoundStyle ShieldDestroyed_NPCDeath58 => SoundID.NPCDeath58;

        /// 气球破了的声音
        public static SoundStyle WindyBalloon_NPCDeath63 => SoundID.NPCDeath63;

        //1.4.4蜂后的死亡音效
        //public static SoundStyle QueenBee_NPCDeath66 => SoundID.NPCDeath66;

        #endregion
        #region NPC其他音效

        /// 僵尸音效，共3种</summary>
        public static SoundStyle Zombie => SoundID.ZombieMoan;

        /// 僵尸音效2</summary>
        public static SoundStyle Zombie_Zombie1 => SoundID.Zombie1;

        /// 僵尸音效3</summary>
        public static SoundStyle Zombie_Zombie2 => SoundID.Zombie2;


        //木乃伊的音效
        public static SoundStyle Mummy => SoundID.Mummy;

        /// 木乃伊音效1</summary>
        public static SoundStyle Mummy1_Zombie3 => SoundID.Zombie3;

        /// 木乃伊音效2</summary>
        public static SoundStyle Mummy2_Zombie4 => SoundID.Zombie4;


        /// 原版未使用，哼叫声，有回音所以听上去像是在洞穴里</summary>
        public static SoundStyle NoUse_CaveRoar_Zombie5 => SoundID.Zombie5;

        /// 大脸怪/脸怪的音效</summary>
        public static SoundStyle FaceMonster_Zombie8 => SoundID.Zombie8;

        /// 猪鲨的音效</summary>
        public static SoundStyle DukeFishron_Zombie20 => SoundID.Zombie20;


        /// 血僵尸，僵尸鱼人的音效，共3种</summary>
        public static SoundStyle BloodZombie => SoundID.BloodZombie;

        /// 血僵尸，僵尸鱼人的音效</summary>
        public static SoundStyle BloodZombie1_Zombie21 => SoundID.Zombie21;

        /// 血僵尸，僵尸鱼人的音效</summary>
        public static SoundStyle BloodZombie2_Zombie22 => SoundID.Zombie22;

        /// 血僵尸，僵尸鱼人的音效</summary>
        public static SoundStyle BloodZombie3_Zombie23 => SoundID.Zombie23;


        /// 血蜘蛛的音效</summary>
        public static SoundStyle BloodCrawler1_Zombie24 => SoundID.Zombie24;

        /// 血蜘蛛的音效</summary>
        public static SoundStyle BloodCrawler2_Zombie25 => SoundID.Zombie25;


        /// 恶魔，红恶魔的音效</summary>
        public static SoundStyle Demon1_Zombie26 => SoundID.Zombie26;

        /// 恶魔，红恶魔的音效</summary>
        public static SoundStyle Demon2_Zombie27 => SoundID.Zombie27;

        /// 恶魔，红恶魔的音效</summary>
        public static SoundStyle Demon3_Zombie28 => SoundID.Zombie28;

        /// 恶魔，红恶魔的音效</summary>
        public static SoundStyle Demon4_Zombie29 => SoundID.Zombie29;


        /// 跳跳兽的音效（肉后丛林的那个蓝色的虫子）</summary>
        public static SoundStyle Derpling1_Zombie30 => SoundID.Zombie30;

        /// 跳跳兽的音效（肉后丛林的那个蓝色的虫子）</summary>
        public static SoundStyle Derpling2_Zombie31 => SoundID.Zombie31;


        /// 飞蛇的音效</summary>
        public static SoundStyle FlyingSnake_Zombie32 => SoundID.Zombie32;

        /// 巨型陆龟 a.k.a 丛林王八 a.k.a 丛林核弹 a.k.a 萌新杀手......（编不下去了）的音效</summary>
        public static SoundStyle GiantTortoise_Zombie33 => SoundID.Zombie33;


        /// 3种颜色的水母在液体中的音效</summary>
        public static SoundStyle Jellyfish1_Zombie34 => SoundID.Zombie34;

        /// 3种颜色的水母在液体中的音效</summary>
        public static SoundStyle Jellyfish2_Zombie35 => SoundID.Zombie35;


        /// 丛林蜥蜴的音效</summary>
        public static SoundStyle Lihzahrd1_Zombie36 => SoundID.Zombie36;

        /// 丛林蜥蜴的音效</summary>
        public static SoundStyle Lihzahrd2_Zombie37 => SoundID.Zombie37;


        /// 猪龙的音效</summary>
        public static SoundStyle Pigron1_Zombie9 => SoundID.Zombie9;

        /// 猪龙的音效</summary>
        public static SoundStyle Pigron2_Zombie38 => SoundID.Zombie38;

        /// 猪龙的音效</summary>
        public static SoundStyle Pigron3_Zombie39 => SoundID.Zombie39;

        /// 猪龙的音效</summary>
        public static SoundStyle Pigron4_Zombie40 => SoundID.Zombie40;


        //愤怒雨云的音效
        public static SoundStyle AngryNimbus1_Zombie41 => SoundID.Zombie41;

        //愤怒雨云的音效
        public static SoundStyle AngryNimbus2_Zombie42 => SoundID.Zombie42;

        //愤怒雨云的音效
        public static SoundStyle AngryNimbus3_Zombie43 => SoundID.Zombie43;


        //大部分蚁狮的音效
        public static SoundStyle BugRore_Zombie44 => SoundID.Zombie44;

        //蚁狮蜂，巨型蚁狮蜂的音效
        public static SoundStyle BugWingsShaking1_Zombie45 => SoundID.Zombie45;

        //蚁狮蜂，巨型蚁狮蜂的音效
        public static SoundStyle BugWingsShaking2_Zombie46 => SoundID.Zombie46;


        //龙虾的音效
        public static SoundStyle Crawdad_Zombie47 => SoundID.Zombie47;


        //地牢幽魂/灵魂/鬼魂的音效
        public static SoundStyle DungeonSpirit1_Zombie53 => SoundID.Zombie53;

        //地牢幽魂/灵魂/鬼魂的音效
        public static SoundStyle DungeonSpirit2_Zombie54 => SoundID.Zombie54;


        //各种食尸鬼的音效（肉后地底沙漠的那个）的音效
        public static SoundStyle Ghoul1_Zombie55 => SoundID.Zombie55;

        //各种食尸鬼的音效（肉后地底沙漠的那个）的音效
        public static SoundStyle Ghoul2_Zombie56 => SoundID.Zombie56;


        //巨型卷壳怪的音效
        public static SoundStyle GiantShelly1_Zombie57 => SoundID.Zombie57;

        //巨型卷壳怪的音效
        public static SoundStyle GiantShelly2_Zombie58 => SoundID.Zombie58;


        /// 哥布林巫师/召唤师的音效
        public static SoundStyle GoblinWarlock1_Zombie61 => SoundID.Zombie61;

        /// 哥布林巫师/召唤师的音效
        public static SoundStyle GoblinWarlock2_Zombie62 => SoundID.Zombie62;


        /// 花岗岩巨人/傀儡的音效
        public static SoundStyle GraniteGolem1_Zombie63 => SoundID.Zombie63;

        /// 花岗岩巨人/傀儡的音效
        public static SoundStyle GraniteGolem2_Zombie64 => SoundID.Zombie64;

        /// 花岗岩巨人/傀儡的音效
        public static SoundStyle GraniteGolem3_Zombie65 => SoundID.Zombie65;


        /// 蘑菇瓢虫的音效
        public static SoundStyle MushiLadybug1_Zombie74 => SoundID.Zombie74;

        /// 蘑菇瓢虫的音效
        public static SoundStyle MushiLadybug2_Zombie75 => SoundID.Zombie75;

        /// 蘑菇瓢虫的音效
        public static SoundStyle MushiLadybug3_Zombie76 => SoundID.Zombie76;

        /// 蘑菇瓢虫的音效
        public static SoundStyle MushiLadybug4_Zombie77 => SoundID.Zombie77;


        /// 鹦鹉的音效
        public static SoundStyle Parrot_Zombie78 => SoundID.Zombie78;


        /// 幻灵/幽灵，死神的音效
        public static SoundStyle Wraith_Reaper1_Zombie81 => SoundID.Zombie81;

        /// 幻灵/幽灵，死神的音效
        public static SoundStyle Wraith_Reaper2_Zombie82 => SoundID.Zombie82;

        /// 幻灵/幽灵，死神的音效
        public static SoundStyle Wraith_Reaper3_Zombie83 => SoundID.Zombie83;


        /// 蝾螈/鸭鸭怪的音效
        public static SoundStyle Salamander1_Zombie84 => SoundID.Zombie84;

        /// 蝾螈/鸭鸭怪的音效
        public static SoundStyle Salamander2_Zombie85 => SoundID.Zombie85;


        /// 原版未使用，奇怪的叫声
        public static SoundStyle NoUse_StrangeRore_Zombie87 => SoundID.Zombie87;

        /// 小丑的声音
        //public static SoundStyle Clown => SoundID.Clown;

        /// 沙鲨的音效</summary>
        public static SoundStyle SandShark => SoundID.SandShark;

        #endregion
        #region 小动物音效

        /// 鸭鸭的音效，共3种</summary>
        public static SoundStyle Duck => SoundID.Duck;

        /// 鸭鸭的音效</summary>
        public static SoundStyle Duck1_Zombie10 => SoundID.Zombie10;

        /// 鸭鸭的音效</summary>
        public static SoundStyle Duck2_Zombie11 => SoundID.Zombie11;

        /// 鸭鸭的音效</summary>
        public static SoundStyle Duck3_Zombie12 => SoundID.Zombie12;


        /// 青蛙/呱呱/蛙蛙的音效</summary>
        public static SoundStyle Frog => SoundID.Frog;

        /// 青蛙/呱呱/蛙蛙的音效</summary>
        public static SoundStyle Frog_Zombie13 => SoundID.Zombie13;


        /// 鼠鼠/老鼠的音效（别打我兄弟TAT）</summary>
        public static SoundStyle Critter => SoundID.Critter;

        /// 鼠鼠/老鼠的音效（别打我兄弟TAT）</summary>
        public static SoundStyle Mouse_Rat_Critter_Zombie15 => SoundID.Zombie15;

        /// 各种啮齿类生物死亡声音，比如老鼠，蝙蝠等（别鲨我鼠鼠兄弟TAT）
        public static SoundStyle Mouse_Rat_Critter_Bat_NPCDeath4 => SoundID.NPCDeath4;


        /// 鸟的音效，共5种</summary>
        public static SoundStyle Bird => SoundID.Bird;

        /// 鸟的音效</summary>
        public static SoundStyle Bird1_Zombie14 => SoundID.Zombie14;

        /// 鸟的音效</summary>
        public static SoundStyle Bird2_BlueJay_Zombie16 => SoundID.Zombie16;

        /// 鸟的音效</summary>
        public static SoundStyle Bird3_Cardinal_Zombie17 => SoundID.Zombie17;

        /// 鸟的音效</summary>
        public static SoundStyle Bird4_Zombie18 => SoundID.Zombie18;

        /// 鸟的音效</summary>
        public static SoundStyle Bird5_Cardinal_Zombie19 => SoundID.Zombie19;


        /// 海鸥的音效，共3种</summary>
        public static SoundStyle Seagull => SoundID.Seagull;

        /// 海鸥的音效
        public static SoundStyle Seagull1_Zombie106 => SoundID.Zombie106;

        /// 海鸥的音效
        public static SoundStyle Seagull2_Zombie107 => SoundID.Zombie107;

        /// 海鸥的音效
        public static SoundStyle Seagull3_Zombie108 => SoundID.Zombie108;


        /// 海豚的音效
        public static SoundStyle Dolphin => SoundID.Dolphin;

        /// 海豚的音效
        public static SoundStyle Dolphin_Zombie109 => SoundID.Zombie109;


        /// 猫头鹰的音效，共5种
        public static SoundStyle Owl => SoundID.Owl;

        /// 猫头鹰的音效
        public static SoundStyle Owl1_Zombie110 => SoundID.Zombie110;

        /// 猫头鹰的音效
        public static SoundStyle Owl2_Zombie111 => SoundID.Zombie111;

        /// 猫头鹰的音效
        public static SoundStyle Owl3_Zombie112 => SoundID.Zombie112;

        /// 猫头鹰的音效
        public static SoundStyle Owl4_Zombie113 => SoundID.Zombie113;

        /// 猫头鹰的音效
        public static SoundStyle Owl5_Zombie114 => SoundID.Zombie114;

        /// 玄凤鹦鹉的声音
        //public static SoundStyle Cockatiel => SoundID.Cockatiel;

        /// 金刚鹦鹉的声音
        //public static SoundStyle Macaw => SoundID.Macaw;

        /// 巨嘴鸟的声音
        //public static SoundStyle Toucan => SoundID.Toucan;

        #endregion

        #region BOSS-邪教徒

        /// 邪教徒召唤时的音效
        public static SoundStyle LunaticCultistSummoned_Zombie105 => SoundID.Zombie105;

        //教徒放冰刺时的声音
        public static SoundStyle IceMist_Item120 => SoundID.Item120;

        /// 教徒放闪电球时的声音</summary>
        public static SoundStyle LightningOrb_Item121 => SoundID.Item121;

        /// 原版未使用，类似电击魔法的声音
        public static SoundStyle NoUse_ElectricMagic_Item122 => SoundID.Item122;

        //教徒的某个动作的声音（不知道在哪用上了）</summary>
        public static SoundStyle LightningRitual_Item123 => SoundID.Item123;

        //直译：幻影闪电，也是不知道哪里用上了，听上去像Jiu ! Jiu ! 嚎呜~~
        public static SoundStyle PhantasmalBolt_Item124 => SoundID.Item124;

        //直译：幻影闪电，也是不知道哪里用上了，听上去像Jiu ! Jiu ! Jiu !
        public static SoundStyle PhantasmalBolt2_Item125 => SoundID.Item125;

        //邪教徒的受击音效
        public static SoundStyle LunaticCultist_NPCHit55 => SoundID.NPCHit55;

        /// 邪教徒的音效
        public static SoundStyle LunaticCultist1_Zombie88 => SoundID.Zombie88;

        /// 邪教徒的音效
        public static SoundStyle LunaticCultist2_Zombie89 => SoundID.Zombie89;

        /// 邪教徒的音效
        public static SoundStyle LunaticCultist3_Zombie90 => SoundID.Zombie90;

        /// 邪教徒的音效
        public static SoundStyle LunaticCultist4_Zombie91 => SoundID.Zombie91;

        //邪教徒的死亡音效，同伴方块的音效（致敬传送门的那个宠物）</summary>
        public static SoundStyle LunaticCultist_NPCDeath59 => SoundID.NPCDeath59;

        //幻影龙/幻影弓龙召唤时的声音
        public static SoundStyle DragonRoar_Item119 => SoundID.Item119;

        /// 幻影龙/幻影弓龙的受击音效
        public static SoundStyle PhantasmDragon_NPCHit56 => SoundID.NPCHit56;

        //幻影龙/幻影弓龙的死亡音效
        public static SoundStyle PhantasmDragon_NPCDeath60 => SoundID.NPCDeath60;

        #endregion
        #region BOSS - 月球领主/月总

        //月亮领主 a.k.a 月总，月蛭凝块（月总舌头）的受击音效</summary>
        public static SoundStyle MoonLord_NPCHit57 => SoundID.NPCHit57;

        /// 月球领主/月总召唤时的音效
        public static SoundStyle MoonLordSummoned_Zombie92 => SoundID.Zombie92;

        /// 月球领主/月总的音效
        public static SoundStyle MoonLord1_Zombie93 => SoundID.Zombie93;

        /// 月球领主/月总的音效
        public static SoundStyle MoonLord2_Zombie94 => SoundID.Zombie94;

        /// 月球领主/月总的音效
        public static SoundStyle MoonLord3_Zombie95 => SoundID.Zombie95;

        /// 月球领主/月总的音效
        public static SoundStyle MoonLord4_Zombie96 => SoundID.Zombie96;

        /// 月球领主/月总的音效
        public static SoundStyle MoonLord5_Zombie97 => SoundID.Zombie97;

        /// 月球领主/月总的音效
        public static SoundStyle MoonLord6_Zombie98 => SoundID.Zombie98;

        /// 月球领主/月总的音效
        public static SoundStyle MoonLord7_Zombie99 => SoundID.Zombie99;

        /// 月球领主/月总真眼/克苏鲁真眼的音效
        public static SoundStyle TrueEyeOfCthulhu1_Zombie100 => SoundID.Zombie100;

        /// 月球领主/月总真眼/克苏鲁真眼的音效
        public static SoundStyle TrueEyeOfCthulhu2_Zombie101 => SoundID.Zombie101;

        /// 月球领主/月总真眼/克苏鲁真眼的幻影球音效
        public static SoundStyle PhantasmalSphere_Zombie102 => SoundID.Zombie102;

        /// 月球领主/月总的幻影眼音效
        public static SoundStyle PhantasmalEyeImpact_Zombie103 => SoundID.Zombie103;

        /// 月球领主/月总的幻影死亡射线音效
        public static SoundStyle PhantasmalDeathray_Zombie104 => SoundID.Zombie104;

        //月亮领主 a.k.a 月总的死亡音效，同伴方块的音效（致敬传送门的那个宠物）</summary>
        public static SoundStyle MoonLord_NPCDeath61 => SoundID.NPCDeath61;

        /// 月亮领主 a.k.a 月总的手，头，月蛭凝块（月总舌头）的死亡音效</summary>
        public static SoundStyle MoonLord2_NPCDeath62 => SoundID.NPCDeath62;

        /// 不知道什么音效，但名字叫做月总</summary>
        public static SoundStyle MoonLord => SoundID.MoonLord;

        #endregion
        #region BOSS-史莱姆皇后

        /// 史莱姆皇后的声音，类似果冻弹弹的声音</summary>
        public static SoundStyle QueenSlime_Item154 => SoundID.Item154;

        //史莱姆皇后的音效，共3种
        public static SoundStyle QueenSlime => SoundID.QueenSlime;

        /// 史莱姆皇后的音效
        public static SoundStyle QueenSlime1_Zombie115 => SoundID.Zombie115;

        /// 史莱姆皇后的音效
        public static SoundStyle QueenSlime2_Zombie116 => SoundID.Zombie116;

        /// 史莱姆皇后的音效
        public static SoundStyle QueenSlime3_Zombie117 => SoundID.Zombie117;

        /// 史莱姆皇后的声音，类似一堆泡泡的声音</summary>
        public static SoundStyle QueenSlime2_Bubble_Item155 => SoundID.Item155;

        //史莱姆皇后，没听错的话应该是落地的声音
        public static SoundStyle QueneSlimeFalling_Item167 => SoundID.Item167;

        //史莱姆皇后的死亡音效
        public static SoundStyle QueenSlime_NPCDeath64 => SoundID.NPCDeath64;

        #endregion
        #region BOSS-光之女皇
        /// 光之女皇的太阳舞的声音
        public static SoundStyle EmpressOfLight_SunDance_Item159 => SoundID.Item159;

        /// 光之女皇的猛冲的声音
        public static SoundStyle EmpressOfLight_Dash_Item160 => SoundID.Item160;

        //光之女皇召唤时的声音
        public static SoundStyle EmpressOfLight_Summoned_Item161 => SoundID.Item161;

        //光之女皇的空灵长枪的声音
        public static SoundStyle EmpressOfLight_EtherealLance_Item162 => SoundID.Item162;

        //光之女皇的永恒彩虹的声音
        public static SoundStyle EmpressOfLight_EverlastingRainbow_Item163 => SoundID.Item163;

        /// 光之女皇的七彩矢的声音
        public static SoundStyle EmpressOfLight_PrismaticBolts_Item164 => SoundID.Item164;

        /// 光之女皇的七彩矢2的声音（Wiki上就这么写的）</summary>
        public static SoundStyle EmpressOfLight_PrismaticBolts2_Item165 => SoundID.Item165;

        //光之女皇的死亡音效
        public static SoundStyle EmpressOfLight_NPCDeath65 => SoundID.NPCDeath65;

        #endregion
        #region BOSS-恐惧鹦鹉螺

        /// 恐惧鹦鹉螺蓄力的声音
        public static SoundStyle Dreadnautilus_ChargeUp_Item170 => SoundID.Item170;

        /// 恐惧鹦鹉螺发射血弹的声音</summary>
        public static SoundStyle Dreadnautilus_FireProjectiles_Item171 => SoundID.Item171;

        //恐惧鹦鹉螺冲刺的声音
        public static SoundStyle Dreadnautilus_Dash_Item172 => SoundID.Item172;

        #endregion

        #region 日食NPC

        /// 科学怪人/弗兰肯斯坦的音效</summary>
        public static SoundStyle Frankenstein_Zombie6 => SoundID.Zombie6;

        /// 吸血鬼，沙鲨的音效</summary>
        public static SoundStyle Vampire_SandShark_Zombie7 => SoundID.Zombie7;

        //屠夫（日食的），美杜莎的死亡音效</summary>
        public static SoundStyle Medusa_NPCDeath17 => SoundID.NPCDeath17;

        //致命球的受击音效
        public static SoundStyle DeadlySphere_NPCHit34 => SoundID.NPCHit34;

        //致命球的音效
        public static SoundStyle DeadlySphere1_Zombie48 => SoundID.Zombie48;

        //致命球的音效
        public static SoundStyle DeadlySphere2_Zombie49 => SoundID.Zombie49;

        //致命球的死亡音效
        public static SoundStyle DeadlySphere_NPCDeath37 => SoundID.NPCDeath37;

        /// 苍蝇人博士的受击音效</summary>
        public static SoundStyle DrManFly_NPCHit35 => SoundID.NPCHit35;

        //苍蝇人博士的音效
        public static SoundStyle DrManFly1_Zombie50 => SoundID.Zombie50;

        //苍蝇人博士的音效
        public static SoundStyle DrManFly2_Zombie51 => SoundID.Zombie51;

        //苍蝇人博士的音效
        public static SoundStyle DrManFly3_Zombie52 => SoundID.Zombie52;

        //苍蝇人博士的死亡音效
        public static SoundStyle DrManFly_NPCDeath38 => SoundID.NPCDeath38;

        /// 蛾怪 a.k.a 魔斯拉的受击音效</summary>
        public static SoundStyle Mothron_NPCHit44 => SoundID.NPCHit44;

        /// 蛾怪 a.k.a 魔斯拉的音效
        public static SoundStyle Mothron_Zombie73 => SoundID.Zombie73;

        /// 蛾怪 a.k.a 魔斯拉的死亡音效</summary>
        public static SoundStyle Mothron_NPCDeath46 => SoundID.NPCDeath46;

        //攀爬魔的受击音效</summary>
        public static SoundStyle ThePossessed_NPCHit47 => SoundID.NPCHit47;

        /// 攀爬魔的音效
        public static SoundStyle ThePossessed1_Zombie79 => SoundID.Zombie79;

        /// 攀爬魔的音效
        public static SoundStyle ThePossessed2_Zombie80 => SoundID.Zombie80;

        /// 攀爬魔的死亡音效
        public static SoundStyle ThePossessed_NPCDeath49 => SoundID.NPCDeath49;

        //变态人的死亡音效</summary>
        public static SoundStyle Psycho_NPCDeath50 => SoundID.NPCDeath50;

        //变态人的受击音效</summary>
        public static SoundStyle Psycho_NPCHit48 => SoundID.NPCHit48;

        //死神的受击音效
        public static SoundStyle Reaper_NPCHit49 => SoundID.NPCHit49;

        //死神的死亡音效</summary>
        public static SoundStyle Reaper_NPCDeath51 => SoundID.NPCDeath51;

        #endregion
        #region 火星入侵NPC

        //电击怪的音效（某个火星敌怪）
        public static SoundStyle Gigazapper1_Zombie59 => SoundID.Zombie59;

        //电击怪的音效（某个火星敌怪）
        public static SoundStyle Gigazapper2_Zombie60 => SoundID.Zombie60;

        /// 火星走妖的音效
        public static SoundStyle MartianWalker1_Zombie69 => SoundID.Zombie69;

        /// 火星走妖的音效
        public static SoundStyle MartianWalker2_Zombie70 => SoundID.Zombie70;

        /// 火星走妖的音效
        public static SoundStyle MartianWalker3_Zombie71 => SoundID.Zombie71;

        /// 火星走妖的音效
        public static SoundStyle MartianWalker4_Zombie72 => SoundID.Zombie72;

        /// 火星自爆飞船的音效
        public static SoundStyle MartianDrone1_Zombie66 => SoundID.Zombie66;

        /// 火星自爆飞船的音效
        public static SoundStyle MartianDrone2_Zombie67 => SoundID.Zombie67;

        /// 火星自爆飞船的音效
        public static SoundStyle MartianDrone3_Zombie68 => SoundID.Zombie68;

        //火星自爆飞船的受击音效</summary>
        public static SoundStyle MartianDrone_HitMetal_NPCHit42 => SoundID.NPCHit42;

        //火星自爆飞船的死亡音效，听上去像护盾碎了的声音
        public static SoundStyle MartianDrone_ShieldBroken_NPCDeath44 => SoundID.NPCDeath44;

        /// 火星人的泡泡盾的受击音效
        public static SoundStyle BubbleShield_Electric_NPCHit43 => SoundID.NPCHit43;

        //火星人的泡泡盾的死亡音效</summary>
        public static SoundStyle BubbleShield_NPCDeath45 => SoundID.NPCDeath45;

        //鳞甲怪 a.k.a 火星蛞蝓的受击音效</summary>
        public static SoundStyle Scutlix_NPCHit51 => SoundID.NPCHit51;

        /// 鳞甲怪 a.k.a 火星蛞蝓的音效
        public static SoundStyle Scutlix_Zombie86 => SoundID.Zombie86;

        //鳞甲怪 a.k.a 火星蛞蝓的死亡音效</summary>
        public static SoundStyle Scutlix_NPCDeath54 => SoundID.NPCDeath54;

        //特斯拉炮塔的受击音效</summary>
        public static SoundStyle TeslaTurret_Electric_NPCHit53 => SoundID.NPCHit53;

        //特斯拉炮塔的死亡音效</summary>
        public static SoundStyle TeslaTurret_NPCDeath56 => SoundID.NPCDeath56;

        //各种火星人的受击音效
        public static SoundStyle Martian_NPCHit39 => SoundID.NPCHit39;

        /// 各种火星人的死亡音效</summary>
        public static SoundStyle Martian_NPCDeath57 => SoundID.NPCDeath57;

        #endregion


        //----------其他-----------


        /// 敌怪咆哮的音效，大部分召唤BOSS的物品的音效</summary>
        public static SoundStyle Roar => SoundID.Roar;

        /// 蠕虫挖地的声音</summary>
        public static SoundStyle WormDig => SoundID.WormDig;

        /// 原版未使用，是尖叫声，带点机械的感觉</summary>
        public static SoundStyle NoUse_ScaryScream => SoundID.ScaryScream;

        //小精灵自身的音效</summary>
        public static SoundStyle Pixie => SoundID.Pixie;

        //哀木等的扔火球的声音
        public static SoundStyle FireThrow_Item42 => SoundID.Item42;

        //专家模式克苏鲁之眼/克眼的叫声
        public static SoundStyle ForceRoar => SoundID.ForceRoar;

        //专家模式克苏鲁之眼/克眼的叫声，调整音高之后的版本，疯狗冲刺时的叫声
        public static SoundStyle ForceRoarPitched => SoundID.ForceRoarPitched;

        #endregion

        #region 物块音效

        /// 植物物块/墙壁破坏的声音，部分弹幕的声音，如吹叶机
        public static SoundStyle Grass => SoundID.Grass;

        //挖掘时的声音，共3种
        public static SoundStyle Dig => SoundID.Dig;

        //开门的声音</summary>
        public static SoundStyle DoorOpen => SoundID.DoorOpen;

        /// 关门的声音</summary>
        public static SoundStyle DoorClosed => SoundID.DoorClosed;

        /// 听上去像玻璃破碎的声音，一些墙壁的声音，例如玻璃墙，落雪墙等</summary>
        public static SoundStyle GlassBroken_Shatter => SoundID.Shatter;

        /// 大部分电路开关的音效</summary>
        public static SoundStyle Trigger_Mech => SoundID.Mech;

        /// 石制物块和墙壁的挖掘音效
        public static SoundStyle DigStone_Tink => SoundID.Tink;

        /// 挖冰雪块的声音，从3个中随机一个
        public static SoundStyle DigIce
        {
            get => Main.rand.Next(3) switch
            {
                0 => SoundID.Item48,
                1 => SoundID.Item49,
                _ => SoundID.Item50,
            };
        }

        /// 挖冰雪块的声音
        public static SoundStyle DigIce1_Item48 => SoundID.Item48;

        //挖冰雪块的声音</summary>
        public static SoundStyle DigIce2_Item49 => SoundID.Item49;

        /// 挖冰雪块的声音
        public static SoundStyle DigIce3_Item50 => SoundID.Item50;

        /// 破碎地牢砖的声音
        public static SoundStyle CrackedDungeonBricks_Item127 => SoundID.Item127;

        //子弹盒的声音（放地上的物块，可以右键加BUFF的那个）
        public static SoundStyle AmmoBox_Item149 => SoundID.Item149;

        //箱子解锁时的音效
        public static SoundStyle Chest_Unlock => SoundID.Unlock;

        //以太块/镒块产生时的音效
        public static SoundStyle AetheriumBlock => SoundID.ShimmerWeak1;

        #endregion

        #region 液体音效

        //接触水的音效
        public static SoundStyle Water_Splash => SoundID.Splash;

        //向下流动的水的声音
        public static SoundStyle Waterfall => SoundID.Waterfall;

        //向下流动的岩浆的声音
        public static SoundStyle Lavafall => SoundID.Lavafall;

        //接触岩浆，蜂蜜的音效，用桶装起液体的音效
        public static SoundStyle Lava_Honey_SplashWeak => SoundID.SplashWeak;

        //滴落的液体的声音，共3种
        public static SoundStyle Drip => SoundID.Drip;

        //接触微光或离开微光的音效
        public static SoundStyle ShimmerContract => SoundID.Shimmer1;

        /// 1.4.4微光的声音
        public static SoundStyle Shlimmer_Item176 => SoundID.Item176;

        //不知道在哪用了，反正也是液体流动的音效
        public static SoundStyle IDontKnow_ShimmerWeak2 => SoundID.ShimmerWeak2;

        #endregion

        #region UI音效

        /// 打开UI的声音，例如打开背包</summary>
        public static SoundStyle MenuOpen => SoundID.MenuOpen;

        /// 关闭UI的声音，例如关闭背包</summary>
        public static SoundStyle MenuClose => SoundID.MenuClose;

        /// 鼠标悬浮在UI上时发出的声音</summary>
        public static SoundStyle MenuTick => SoundID.MenuTick;

        /// 照相模式照相的声音</summary>
        public static SoundStyle Camera => SoundID.Camera;

        /// 搜索物品的声音，共3种</summary>
        public static SoundStyle Research => SoundID.Research;

        /// 搜索物品完成时的声音</summary>
        public static SoundStyle ResearchComplete => SoundID.ResearchComplete;

        #endregion

        //打雷/闪电天气的声音，共7种
        public static SoundStyle Thunder => SoundID.Thunder;
    }
}