using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static BeastExitScamMod.Core.PacketHandler;

namespace BeastExitScamMod.Core
{
	internal class ScamScheduler : ModSystem // Scam people automatically if they don't gamble.
	{
		public Dictionary<int, int> PlayerTimings = new();
		public override void PreUpdatePlayers()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return;
			}
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				if (Main.player[i].active)
				{
					if (!PlayerTimings.ContainsKey(i))
					{
						Config config = Config.Instance;
						if (config != null)
						{
							PlayerTimings.Add(i, config.MaxTimeWithoutUseUntilScammed * 60 * 60);
						}
						else
						{
							PlayerTimings.Add(i, 7 * 60 * 60);
						}
					}
				}
				else
				{
					if (PlayerTimings.ContainsKey(i))
					{
						PlayerTimings.Remove(i);
					}
				}
			}
			foreach (int key in PlayerTimings.Keys.ToList())
			{
				int value = PlayerTimings[key];
				if (value <= 0)
				{
					Config config = Config.Instance;
					if (config != null)
					{
						PlayerTimings[key] = config.MaxTimeWithoutUseUntilScammed * 60 * 60;
					}
					else
					{
						PlayerTimings[key] = 7 * 60 * 60;
					}
					ScamOrBuffHandler SOBHinstance = ModContent.GetInstance<ScamOrBuffHandler>();
					SOBHinstance.HandleServerReward(rewardType: false, targetPlayerID: key);
					if (Main.netMode == NetmodeID.Server)
					{
						ModPacket packet = ModContent.GetInstance<BeastExitScamMod>().GetPacket();
						packet.Write((byte)PacketType.ServerBroadcastResult);
						packet.Write((bool)false);
						packet.Send(toClient: key);
					}
					else if (Main.netMode == NetmodeID.SinglePlayer)
					{
						ModContent.GetInstance<ScamOrBuffHandler>().HandleClientReward(rewardType: false);
					}
				}
				else
				{
					PlayerTimings[key] -= 1;
				}
			}
		}
		public override void OnWorldUnload()
		{
			PlayerTimings = new();
		}
		public override void OnWorldLoad()
		{
			PlayerTimings = new();
		}
		public void UpdateEntry(int playerID)
		{
			Config config = Config.Instance;
			if (config != null)
			{
				PlayerTimings[playerID] = config.MaxTimeWithoutUseUntilScammed * 60 * 60;
			}
			else
			{
				PlayerTimings[playerID] = 7 * 60 * 60;
			}
			//ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"UpdateEntry called for player {playerID}"), Color.Yellow);
		}
	}
}
