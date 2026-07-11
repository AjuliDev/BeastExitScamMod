using BeastScam.Core;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace BeastExitScamMod.Core
{
	internal class ScamOrBuffHandler : ModSystem
	{
		public void DisplayMessage(bool result, int playerID)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return;
			}
			if (result)
			{
				ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"<Mr. Beast> {Main.player[playerID].name} has passed the ultimate challenge!"), Color.LimeGreen);
			}
			else
			{
				ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"<{Main.player[playerID].name}> I fell for the scam..."), Color.Red);
			}
		}
		public void PlayResultSound(bool result)
		{
			if (Main.netMode == NetmodeID.Server) return;
			if (result == true)
			{
				SoundEngine.PlaySound(GeneratedSource.chaching_win);
			}
			else
			{
				SoundEngine.PlaySound(GeneratedSource.w95_fail);
			}
		}
		public int GetRandomBuffTime(int minTimeSeconds, int maxTimeSeconds)
		{
			return Main.rand.Next(60 * minTimeSeconds, 60 * maxTimeSeconds);
		}
		public void AddRandomPositiveBuff(int targetPlayerID)
		{
			int[] TierOneBuffs =
				[
				BuffID.Shine,
				BuffID.NightOwl,
				BuffID.Dangersense,
				BuffID.Hunter,
				BuffID.Featherfall,
				BuffID.Gravitation,
				BuffID.Invisibility,
				BuffID.WaterWalking,
				BuffID.Gills,
				BuffID.Flipper,
				BuffID.Mining,
				BuffID.Builder,
				BuffID.Sonar,
				BuffID.Crate,
				BuffID.AmmoBox,
				BuffID.Campfire,
				BuffID.HeartLamp,
				BuffID.WeaponImbueConfetti
				];
			int[] TierTwoBuffs =
				[
				BuffID.Regeneration,
				BuffID.Swiftness,
				BuffID.Ironskin,
				BuffID.ManaRegeneration,
				BuffID.MagicPower,
				BuffID.Archery,
				BuffID.Wrath,
				BuffID.Rage,
				BuffID.Summoning,
				BuffID.Titan,
				BuffID.Battle,
				BuffID.Calm,
				BuffID.Lifeforce,
				BuffID.Heartreach,
				BuffID.Endurance,
				BuffID.Inferno,
				BuffID.Warmth,
				BuffID.WellFed,
				BuffID.Tipsy,
				BuffID.ObsidianSkin,
				BuffID.WellFed2
				];
			int[] TierThreeBuffs =
				[
				BuffID.WellFed3,
				BuffID.Clairvoyance,
				BuffID.AmmoReservation,
				BuffID.Sharpened,
				BuffID.AmmoBox,
				BuffID.Bewitched,
				BuffID.WarTable,
				BuffID.SugarRush,
				BuffID.Thorns,
				BuffID.WeaponImbueCursedFlames,
				BuffID.WeaponImbueFire,
				BuffID.WeaponImbueGold,
				BuffID.WeaponImbueIchor,
				BuffID.WeaponImbueNanites,
				BuffID.WeaponImbuePoison,
				BuffID.WeaponImbueVenom,
				BuffID.DryadsWard,
				BuffID.Honey,
				BuffID.PeaceCandle,
				BuffID.Lucky
				];
			Config config = Config.Instance;
			if (config != null)
			{
				// Easy difficulty: 50% tier 1, 30% tier 2, 20% tier 3
				// Medium difficulty: 30% tier 1, 50% tier 2, 20% tier 3
				// Hard difficulty: 30% tier 1, 30% tier 2, 40% tier 3
				WeightedRandom<int[]> weightedBuffIDPools = new WeightedRandom<int[]>();
				weightedBuffIDPools.Add(TierOneBuffs, config.Difficulty == Config.EventDifficulty.Easy ? 0.5 : config.Difficulty == Config.EventDifficulty.Medium ? 0.3 : 0.3);
				weightedBuffIDPools.Add(TierTwoBuffs, config.Difficulty == Config.EventDifficulty.Easy ? 0.4 : config.Difficulty == Config.EventDifficulty.Medium ? 0.5 : 0.3);
				weightedBuffIDPools.Add(TierThreeBuffs, config.Difficulty == Config.EventDifficulty.Easy ? 0.1 : config.Difficulty == Config.EventDifficulty.Medium ? 0.2 : 0.4);
				int[] chosenBuffIDPool = weightedBuffIDPools.Get();
				int selectedRandomBuffID = chosenBuffIDPool[Main.rand.Next(chosenBuffIDPool.Length)];
				foreach (var player in Main.ActivePlayers)
				{
					if (player == Main.player[targetPlayerID])
					{
						player.AddBuff(selectedRandomBuffID, GetRandomBuffTime(1, 60 * 5));
						// message here
					}
				}
			}
			else
			{
				int[] combinedPool = TierOneBuffs.Concat(TierTwoBuffs).Concat(TierThreeBuffs).ToArray();
				int randomBuffID = combinedPool[Main.rand.Next(combinedPool.Length)];
				foreach (var player in Main.ActivePlayers)
				{
					if (player == Main.player[targetPlayerID])
					{
						player.AddBuff(randomBuffID, GetRandomBuffTime(1, 60 * 5));
						// message here
					}
				}
			}
		}
		public void AddRandomNegativeBuff(int targetPlayerID)
		{
			int[] TierOneDebuff =
				[
				BuffID.Darkness,
				BuffID.Blackout,
				BuffID.Obstructed,
				BuffID.Bleeding,
				BuffID.Slow,
				BuffID.Weak,
				BuffID.Chilled,
				BuffID.Wet,
				BuffID.WindPushed,
				BuffID.PotionSickness,
				BuffID.ManaSickness
				];
			int[] TierTwoDebuff =
				[
				BuffID.Poisoned,
				BuffID.Venom,
				BuffID.OnFire,
				BuffID.Frostburn,
				BuffID.CursedInferno,
				BuffID.ShadowFlame,
				BuffID.Confused,
				BuffID.Silenced,
				BuffID.BrokenArmor,
				BuffID.Ichor,
				BuffID.Suffocation,
				BuffID.Burning,
				BuffID.Webbed,
				BuffID.DryadsWardDebuff
				];
			int[] TierThreeDebuff =
				[
				BuffID.Cursed,
				BuffID.Frozen,
				BuffID.Stoned,
				BuffID.Horrified,
				BuffID.TheTongue,
				BuffID.Electrified,
				BuffID.VortexDebuff,
				BuffID.WitheredArmor,
				BuffID.WitheredWeapon
				];
			Config config = Config.Instance;
			if (config != null)
			{
				// Easy difficulty: 50% tier 1, 30% tier 2, 20% tier 3
				// Medium difficulty: 30% tier 1, 50% tier 2, 20% tier 3
				// Hard difficulty: 30% tier 1, 30% tier 2, 40% tier 3
				WeightedRandom<int[]> weightedBuffIDPools = new WeightedRandom<int[]>();
				weightedBuffIDPools.Add(TierOneDebuff, config.Difficulty == Config.EventDifficulty.Easy ? 0.6 : config.Difficulty == Config.EventDifficulty.Medium ? 0.5 : 0.3);
				weightedBuffIDPools.Add(TierTwoDebuff, config.Difficulty == Config.EventDifficulty.Easy ? 0.3 : config.Difficulty == Config.EventDifficulty.Medium ? 0.3 : 0.3);
				weightedBuffIDPools.Add(TierThreeDebuff, config.Difficulty == Config.EventDifficulty.Easy ? 0.1 : config.Difficulty == Config.EventDifficulty.Medium ? 0.2 : 0.4);
				int[] chosenBuffIDPool = weightedBuffIDPools.Get();
				int selectedRandomBuffID = chosenBuffIDPool[Main.rand.Next(chosenBuffIDPool.Length)];
				foreach (var player in Main.ActivePlayers)
				{
					if (player == Main.player[targetPlayerID])
					{
						player.AddBuff(selectedRandomBuffID, GetRandomBuffTime(1, 30));
						// message here
					}
				}
			}
			else
			{
				int[] combinedPool = TierOneDebuff.Concat(TierTwoDebuff).Concat(TierThreeDebuff).ToArray();
				int randomBuffID = combinedPool[Main.rand.Next(combinedPool.Length)];
				foreach (var player in Main.ActivePlayers)
				{
					if (player == Main.player[targetPlayerID])
					{
						player.AddBuff(randomBuffID, GetRandomBuffTime(1, 30));
						// message here
					}
				}
			}
		}
		public void GiveRandomPositiveReward(int targetPlayerID)
		{
			int[] TierOneItems =
				[
				ItemID.Wood,
				ItemID.Gel,
				ItemID.CopperCoin,
				ItemID.ReflectiveCopperDye,
				ItemID.BorealWood,
				ItemID.PalmWood,
				ItemID.BorealWood,
				ItemID.IronBar,
				ItemID.LeadBar,
				ItemID.CopperBar,
				ItemID.CopperOre,
				ItemID.LeadOre,
				ItemID.IronOre,
				ItemID.IronAnvil,
				ItemID.LeadAnvil,
				ItemID.WoodenCrate,
				ItemID.OldShoe,
				ItemID.Boomstick,
				ItemID.Vilethorn,
				ItemID.AmethystStaff,
				ItemID.TopazStaff,
				ItemID.EmeraldStaff,
				ItemID.Aglet,
				ItemID.Shackle,
				ItemID.GrapplingHook,
				ItemID.SquirrelHook,
				ItemID.CloudinaBottle,
				ItemID.ShinyRedBalloon,
				ItemID.Obsidian,
				ItemID.StoneBlock,
				ItemID.Amethyst,
				ItemID.Topaz,
				ItemID.Emerald,
				ItemID.Spear,
				ItemID.Trident,
				ItemID.WoodYoyo,
				ItemID.CactusSword,
				ItemID.BatBat,
				ItemID.Gladius,
				ItemID.TragicUmbrella,
				ItemID.BladedGlove,
				ItemID.AntlionMandible,
				ItemID.AntlionClaw,
				ItemID.Mace,
				ItemID.LeadBow,
				ItemID.Musket,
				ItemID.MusketBall,
				ItemID.Sandgun,
				ItemID.Bomb,
				ItemID.Grenade
				];
			int[] TierTwoItems =
				[
				ItemID.GoldBar,
				ItemID.GoldOre,
				ItemID.PlatinumBar,
				ItemID.PlatinumOre,
				ItemID.SilverBar,
				ItemID.SilverOre,
				ItemID.TungstenOre,
				ItemID.TungstenBar,
				ItemID.Ebonwood,
				ItemID.Shadewood,
				ItemID.GoldenCrate,
				ItemID.IronCrate,
				ItemID.OceanCrate,
				ItemID.OasisCrate,
				ItemID.LavaCrate,
				ItemID.DiamondStaff,
				ItemID.Diamond,
				ItemID.Ruby,
				ItemID.RubyStaff,
				ItemID.Minishark,
				ItemID.CrimtaneBar,
				ItemID.CrimtaneOre,
				ItemID.DemoniteBar,
				ItemID.DemoniteOre,
				ItemID.CrimsonHeart,
				ItemID.DemonHeart,
				ItemID.LifeCrystal,
				ItemID.ManaCrystal,
				ItemID.FieryGreatsword,
				ItemID.Muramasa,
				ItemID.Katana,
				ItemID.BeeKeeper,
				ItemID.BladeofGrass,
				ItemID.NightsEdge,
				ItemID.PurpleClubberfish,
				ItemID.LightsBane,
				ItemID.BoneSword,
				ItemID.Rally,
				ItemID.CorruptYoyo,
				ItemID.CrimsonYoyo,
				ItemID.JungleYoyo,
				ItemID.Code1,
				ItemID.HiveFive,
				ItemID.Valor,
				ItemID.Cascade,
				ItemID.CandyCaneSword,
				ItemID.IceBlade,
				ItemID.EnchantedSword,
				ItemID.GreenPhaseblade,
				ItemID.DyeTradersScimitar,
				ItemID.ThunderSpear,
				ItemID.TheRottedFork,
				ItemID.Swordfish,
				ItemID.DarkLance,
				ItemID.StarCannon,
				ItemID.MagicMirror,
				ItemID.MagicMissile,
				ItemID.AquaScepter,
				ItemID.FlowerofFire,
				ItemID.Flamelash,
				ItemID.SpaceGun,
				ItemID.ZapinatorGray,
				ItemID.BeeGun,
				ItemID.WaterBolt,
				ItemID.BookofSkulls,
				ItemID.DemonScythe,
				ItemID.CrimsonRod,
				ItemID.SlimeStaff,
				ItemID.FlinxStaff,
				ItemID.AbigailsFlower,
				ItemID.VampireFrogStaff,
				ItemID.ImpStaff,
				ItemID.HoundiusShootius,
				ItemID.Beenade,
				ItemID.PhoenixBlaster,
				ItemID.HellwingBow,
				ItemID.MoltenFury,
				ItemID.GoldCoin,
				ItemID.BlueMoon,
				ItemID.Sunfury,
				ItemID.TinkerersWorkshop,
				ItemID.RocketBoots,
				ItemID.HermesBoots
				];
			int[] TierThreeItems =
				[
				ItemID.PlatinumCoin,
				ItemID.CobaltBar,
				ItemID.CobaltOre,
				ItemID.PalladiumBar,
				ItemID.PalladiumOre,
				ItemID.MythrilBar,
				ItemID.MythrilOre,
				ItemID.OrichalcumBar,
				ItemID.OrichalcumOre,
				ItemID.TitaniumBar,
				ItemID.TitaniumOre,
				ItemID.GoldenCrateHard,
				ItemID.WoodenCrateHard,
				ItemID.OceanCrateHard,
				ItemID.OasisCrateHard,
				ItemID.LavaCrateHard,
				ItemID.JungleFishingCrateHard,
				ItemID.HallowedFishingCrateHard,
				ItemID.FrozenCrateHard,
				ItemID.CrimsonFishingCrateHard,
				ItemID.CorruptFishingCrateHard,
				ItemID.SoulofLight,
				ItemID.SoulofNight,
				ItemID.HallowedBar,
				ItemID.CrystalStorm,
				ItemID.DaedalusStormbow,
				ItemID.ClockworkAssaultRifle,
				ItemID.FormatC,
				ItemID.Gradient,
				ItemID.Amarok,
				ItemID.BreakerBlade,
				ItemID.FetidBaghnakhs,
				ItemID.DaoofPow,
				ItemID.Marrow,
				ItemID.CoinGun,
				ItemID.SkyFracture,
				ItemID.CrystalSerpent,
				ItemID.FrostStaff,
				ItemID.CrystalVileShard,
				ItemID.MeteorStaff,
				ItemID.VenomStaff,
				ItemID.Uzi,
				ItemID.GoldenShower,
				ItemID.CursedFlames,
				ItemID.NimbusRod,
				ItemID.MagicDagger,
				ItemID.OpticStaff,
				ItemID.SpiderStaff,
				ItemID.Cannon,
				ItemID.Cannonball
				];
			int[] TierFourItems =
				[
				ItemID.LunarBar,
				ItemID.LunarOre,
				ItemID.Meowmere,
				ItemID.TrueExcalibur,
				ItemID.TrueNightsEdge,
				ItemID.TerraBlade,
				ItemID.FirstFractal, // Odd item
				ItemID.EmpressBlade,
				ItemID.NebulaArcanum,
				ItemID.NebulaBlaze,
				ItemID.RazorbladeTyphoon,
				ItemID.RainbowGun,
				ItemID.PortalGun,
				ItemID.SpectreStaff,
				ItemID.SuperStarCannon,
				ItemID.Celeb2,
				ItemID.EmpressFlightBooster,
				ItemID.Phantasm,
				ItemID.Zenith,
				ItemID.SolarEruption,
				ItemID.NorthPole,
				ItemID.TheEyeOfCthulhu,
				ItemID.BoringBow // Odd item
				];
			Config config = Config.Instance;
			if (config != null)
			{
				WeightedRandom<int[]> weightedItemIDPools = new WeightedRandom<int[]>();
				weightedItemIDPools.Add(TierOneItems, config.Difficulty == Config.EventDifficulty.Easy ? 0.39 : config.Difficulty == Config.EventDifficulty.Medium ? 0.28 : 0.2);
				weightedItemIDPools.Add(TierTwoItems, config.Difficulty == Config.EventDifficulty.Easy ? 0.3 : config.Difficulty == Config.EventDifficulty.Medium ? 0.4 : 0.5);
				weightedItemIDPools.Add(TierThreeItems, config.Difficulty == Config.EventDifficulty.Easy ? 0.3 : config.Difficulty == Config.EventDifficulty.Medium ? 0.3 : 0.25);
				weightedItemIDPools.Add(TierFourItems, config.Difficulty == Config.EventDifficulty.Easy ? 0.01 : config.Difficulty == Config.EventDifficulty.Medium ? 0.02 : 0.05);
				int[] chosenItemIDPool = weightedItemIDPools.Get();
				int selectedRandomItemID = chosenItemIDPool[Main.rand.Next(chosenItemIDPool.Length)];
				foreach (var player in Main.ActivePlayers)
				{
					if (player == Main.player[targetPlayerID])
					{
						player.QuickSpawnItem(player.GetSource_FromThis(), selectedRandomItemID, Main.rand.Next(1, 64));
						// message here
					}
				}
			}
			else
			{
				int[] combinedPool = TierOneItems.Concat(TierTwoItems).Concat(TierThreeItems).ToArray();
				int randomItemID = combinedPool[Main.rand.Next(combinedPool.Length)];
				foreach (var player in Main.ActivePlayers)
				{
					if (player == Main.player[targetPlayerID])
					{
						player.QuickSpawnItem(player.GetSource_FromThis(), randomItemID, Main.rand.Next(1, 64));
						// message here
					}
				}
			}
		}
		public void InflictNegativeReward(int targetPlayerID)
		{
			foreach (var player in Main.ActivePlayers)
			{
				if (player == Main.player[targetPlayerID])
				{
					int amountToScam = Item.buyPrice(platinum: Main.rand.Next(0, 1), gold: Main.rand.Next(0, 10), silver: Main.rand.Next(0, 50), copper: Main.rand.Next(0, 25));
					bool canPlayerAfford = player.BuyItem(amountToScam);
					if (canPlayerAfford)
					{
						// message here
					}
					else
					{
						// brokie with no bugatti
						Item mostExpensiveItemOrItemStack = player.inventory
							.Where(item => !item.IsAir)
							.OrderByDescending(item => item.value * item.stack)
							.FirstOrDefault();
						if (mostExpensiveItemOrItemStack != null)
						{
							mostExpensiveItemOrItemStack.TurnToAir();
						}
						// message here
					}
				}
			}
		}
		/// <summary>
		/// This function returns a bool with weighted probability. (gambling lol)
		/// </summary>
		/// <param name="givenAds"></param>
		/// <param name="closedAds"></param>
		/// <returns></returns>
		public bool DecideResult(int givenAds, int closedAds)
		{
			if (givenAds <= 0)
			{
				return false;
			}
			float ratio = (float)closedAds / givenAds;
			float chance = ratio * 0.75f; //capped at 75% success
			bool result = Main.rand.NextFloat() < chance;
			return result;
		}
		public void HandleServerReward(bool rewardType, int targetPlayerID)
		{
			DisplayMessage(rewardType, targetPlayerID);
			if (rewardType)
			{
				if (Main.rand.NextBool())
				{
					AddRandomPositiveBuff(targetPlayerID);
				}
				else
				{
					GiveRandomPositiveReward(targetPlayerID);
				}
			}
			else
			{
				if (Main.rand.NextBool())
				{
					AddRandomNegativeBuff(targetPlayerID);
				}
				else
				{
					InflictNegativeReward(targetPlayerID);
				}
			}
		}
		public void HandleClientReward(bool rewardType)
		{
			PlayResultSound(rewardType);
		}
	}
}
