using BeastScam.Core;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
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
						if (Main.netMode == NetmodeID.Server)
						{
							ModPacket newPacket = ModContent.GetInstance<BeastExitScamMod>().GetPacket();
							newPacket.Write((byte)PacketHandler.PacketType.SyncPlayerChanges);
							newPacket.Write((byte)PlayerHandler.TransactionType.PlayerBuff);
							newPacket.Write((int)targetPlayerID);
							newPacket.Write((int)GetRandomBuffTime(1, 60 * 5));
							newPacket.Write((int)selectedRandomBuffID);
							newPacket.Send(-1, -1);
						}
						else if (Main.netMode == NetmodeID.SinglePlayer)
						{
							Main.player[targetPlayerID].AddBuff(selectedRandomBuffID, GetRandomBuffTime(1, 60 * 5));
						}
						// message here
						MessagingHandler.NetworkAnnouncement(MessagingHandler.MessageType.ReceivedGoodBuff,
							selectedRandomBuffID,
							chosenBuffIDPool == TierOneBuffs ? MessagingHandler.Tiers.TierOne :
							chosenBuffIDPool == TierTwoBuffs ? MessagingHandler.Tiers.TierTwo :
							chosenBuffIDPool == TierThreeBuffs ? MessagingHandler.Tiers.TierThree : MessagingHandler.Tiers.TierOne,
							targetPlayerID,
							true);
						//if (Main.netMode == NetmodeID.Server)
						//{
						//	NetMessage.SendData(MessageID.PlayerBuffs, -1, -1, null, targetPlayerID);
						//}
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
						//Main.player[targetPlayerID].AddBuff(randomBuffID, GetRandomBuffTime(1, 60 * 5));
						// message here
						MessagingHandler.NetworkAnnouncement(MessagingHandler.MessageType.ReceivedGoodBuff,
							randomBuffID,
							MessagingHandler.Tiers.TierOne,
							targetPlayerID,
							true);
						//if (Main.netMode == NetmodeID.Server)
						//{
						//	NetMessage.SendData(MessageID.PlayerBuffs, -1, -1, null, targetPlayerID);
						//}
						if (Main.netMode == NetmodeID.Server)
						{
							ModPacket newPacket = ModContent.GetInstance<BeastExitScamMod>().GetPacket();
							newPacket.Write((byte)PacketHandler.PacketType.SyncPlayerChanges);
							newPacket.Write((byte)PlayerHandler.TransactionType.PlayerBuff);
							newPacket.Write((int)targetPlayerID);
							newPacket.Write((int)GetRandomBuffTime(1, 60 * 5));
							newPacket.Write((int)randomBuffID);
							newPacket.Send(-1, -1);
						}
						else if (Main.netMode == NetmodeID.SinglePlayer)
						{
							Main.player[targetPlayerID].AddBuff(randomBuffID, GetRandomBuffTime(1, 60 * 5));
						}
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
				BuffID.Webbed
				];
			int[] TierThreeDebuff =
				[
				BuffID.Cursed,
				BuffID.Frozen,
				BuffID.Stoned,
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
						if (Main.netMode == NetmodeID.Server)
						{
							ModPacket newPacket = ModContent.GetInstance<BeastExitScamMod>().GetPacket();
							newPacket.Write((byte)PacketHandler.PacketType.SyncPlayerChanges);
							newPacket.Write((byte)PlayerHandler.TransactionType.PlayerBuff);
							newPacket.Write((int)targetPlayerID);
							newPacket.Write((int)GetRandomBuffTime(1, 30));
							newPacket.Write((int)selectedRandomBuffID);
							newPacket.Send(-1, -1);
						}
						else if (Main.netMode == NetmodeID.SinglePlayer)
						{
							Main.player[targetPlayerID].AddBuff(selectedRandomBuffID, GetRandomBuffTime(1, 30));
						}
						//Main.player[targetPlayerID].AddBuff(selectedRandomBuffID, GetRandomBuffTime(1, 30));
						// message here
						MessagingHandler.NetworkAnnouncement(MessagingHandler.MessageType.ReceivedBadBuff,
							selectedRandomBuffID,
							chosenBuffIDPool == TierOneDebuff ? MessagingHandler.Tiers.TierOne :
							chosenBuffIDPool == TierTwoDebuff ? MessagingHandler.Tiers.TierTwo :
							chosenBuffIDPool == TierThreeDebuff ? MessagingHandler.Tiers.TierThree : MessagingHandler.Tiers.TierOne,
							targetPlayerID,
							true);
						//if (Main.netMode == NetmodeID.Server)
						//{
						//	NetMessage.SendData(MessageID.PlayerBuffs, -1, -1, null, targetPlayerID);
						//}
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
						if (Main.netMode == NetmodeID.Server)
						{
							ModPacket newPacket = ModContent.GetInstance<BeastExitScamMod>().GetPacket();
							newPacket.Write((byte)PacketHandler.PacketType.SyncPlayerChanges);
							newPacket.Write((byte)PlayerHandler.TransactionType.PlayerBuff);
							newPacket.Write((int)targetPlayerID);
							newPacket.Write((int)GetRandomBuffTime(1, 30));
							newPacket.Write((int)randomBuffID);
							newPacket.Send(-1, -1);
						}
						else if (Main.netMode == NetmodeID.SinglePlayer)
						{
							Main.player[targetPlayerID].AddBuff(randomBuffID, GetRandomBuffTime(1, 30));
						}
						//Main.player[targetPlayerID].AddBuff(randomBuffID, GetRandomBuffTime(1, 30));
						// message here
						MessagingHandler.NetworkAnnouncement(MessagingHandler.MessageType.ReceivedBadBuff,
							randomBuffID,
							MessagingHandler.Tiers.TierOne,
							targetPlayerID,
							true);
						//if (Main.netMode == NetmodeID.Server)
						//{
						//	NetMessage.SendData(MessageID.PlayerBuffs, -1, -1, null, targetPlayerID);
						//}
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
						bool isStackable = ContentSamples.ItemsByType[selectedRandomItemID].maxStack > 1;
						int amountOfThisItem = isStackable == true ? Main.rand.Next(1, 64) : 1;
						player.QuickSpawnItem(player.GetSource_FromThis(), selectedRandomItemID, amountOfThisItem); // this spawns something in world, auto synced
						// message here
						MessagingHandler.NetworkAnnouncement(MessagingHandler.MessageType.WonAnItem,
							selectedRandomItemID,
							chosenItemIDPool == TierOneItems ? MessagingHandler.Tiers.TierOne :
							chosenItemIDPool == TierTwoItems ? MessagingHandler.Tiers.TierTwo :
							chosenItemIDPool == TierThreeItems ? MessagingHandler.Tiers.TierThree :
							chosenItemIDPool == TierFourItems ? MessagingHandler.Tiers.TierFour : MessagingHandler.Tiers.TierOne,
							targetPlayerID,
							false);
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
						bool isStackable = ContentSamples.ItemsByType[randomItemID].maxStack > 1;
						int amountOfThisItem = isStackable == true ? Main.rand.Next(1, 64) : 1;
						player.QuickSpawnItem(player.GetSource_FromThis(), randomItemID, amountOfThisItem); // this spawns something in world, already in sync
						// message here
						MessagingHandler.NetworkAnnouncement(MessagingHandler.MessageType.WonAnItem,
							randomItemID,
							MessagingHandler.Tiers.TierOne,
							targetPlayerID,
							false);
					}
				}
			}
		}
		private static bool CanAfford(Player player, int price)
		{
			int total = 0;
			for (int i = 0; i < 50; i++)
			{
				Item item = player.inventory[i];
				switch (item.type)
				{
					case ItemID.CopperCoin: total += item.stack; break;
					case ItemID.SilverCoin: total += item.stack * 100; break;
					case ItemID.GoldCoin: total += item.stack * 10000; break;
					case ItemID.PlatinumCoin: total += item.stack * 1000000; break;
				}
			}
			return total >= price;
		}
		public void InflictNegativeReward(int targetPlayerID)
		{
			foreach (var player in Main.ActivePlayers)
			{
				if (player == Main.player[targetPlayerID])
				{
					// Attempt grabbing coins, first grabbing a randomized price represented in copper coins
					int fakePurchaseAmount = Item.buyPrice(
						platinum: Main.rand.Next(0, 1),
						gold: Main.rand.Next(1, 50),
						silver: Main.rand.Next(1, 50),
						copper: Main.rand.Next(1, 99)
						);
					if (CanAfford(player, fakePurchaseAmount))
					{
						if (Main.netMode == NetmodeID.Server)
						{
							ModPacket newPacket = ModContent.GetInstance<BeastExitScamMod>().GetPacket();
							newPacket.Write((byte)PacketHandler.PacketType.SyncPlayerChanges);
							newPacket.Write((byte)PlayerHandler.TransactionType.PlayerCoinSubtraction);
							newPacket.Write((int)targetPlayerID);
							newPacket.Write((int)fakePurchaseAmount);
							newPacket.Write((int)ItemID.CopperCoin);
							newPacket.Send(-1, -1);
						}
						else if (Main.netMode == NetmodeID.SinglePlayer)
						{
							player.BuyItem(fakePurchaseAmount);
						}
						MessagingHandler.NetworkAnnouncement(MessagingHandler.MessageType.LostAnItem,
							ItemID.GoldCoin,
							MessagingHandler.Tiers.LostItem,
							targetPlayerID,
							false);
					}
					else
					{
						// If the player cannot afford the coins, try to get the most expensive item
						Item mostExpensiveItemOrItemStack = player.inventory
							.Where(item => !item.IsAir)
							.OrderByDescending(item => item.value * item.stack)
							.FirstOrDefault();
						int slotIndex = System.Array.IndexOf(player.inventory, mostExpensiveItemOrItemStack);
						if (mostExpensiveItemOrItemStack != null)
						{
							if (Main.netMode == NetmodeID.Server)
							{
								ModPacket newPacket = ModContent.GetInstance<BeastExitScamMod>().GetPacket();
								newPacket.Write((byte)PacketHandler.PacketType.SyncPlayerChanges);
								newPacket.Write((byte)PlayerHandler.TransactionType.PlayerItemSubtraction);
								newPacket.Write((int)targetPlayerID);
								newPacket.Write((int)slotIndex);
								newPacket.Write((int)mostExpensiveItemOrItemStack.type);
								newPacket.Send(-1, -1);
							}
							else if (Main.netMode == NetmodeID.SinglePlayer)
							{
								mostExpensiveItemOrItemStack.TurnToAir();
								player.inventory[slotIndex].TurnToAir();
							}
							MessagingHandler.NetworkAnnouncement(MessagingHandler.MessageType.LostAnItem,
								mostExpensiveItemOrItemStack.type,
								MessagingHandler.Tiers.LostItem,
								targetPlayerID,
								false);
						}
						else
						{
							//ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("InflictNegativeReward: inventory empty, falling back to debuff"), Color.White);
							AddRandomNegativeBuff(targetPlayerID);
						}
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
			float ratio = MathHelper.Min((float)closedAds / givenAds, 1f); //(float)closedAds / givenAds;
			float chance = ratio * 0.75f; //capped at 75% success
			bool result = Main.rand.NextFloat() < chance;
			return result;
		}
		public void HandleServerReward(bool rewardType, int targetPlayerID)
		{
			//DisplayMessage(rewardType, targetPlayerID);
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
