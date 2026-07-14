using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static XPT.Core.Audio.MP3Sharp.Decoding.Decoder;

namespace BeastExitScamMod.Core
{
	internal class PlayerHandler : ModPlayer // Cut loose endz
	{
		public static Dictionary<int, List<(TransactionType, int, int)>> Scheduler = new();
		public enum TransactionType
		{
			PlayerItemSubtraction = 1,
			PlayerBuff = 2,
			PlayerCoinSubtraction = 3
		}
		public override void PlayerDisconnect()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				return;
			}
			if (EventHandler.Data != null)
			{
				EventHandler.Data[Player.whoAmI] = null;
			}
			Scheduler.Remove(Player.whoAmI);
		}
		public override void PostUpdate()
		{
			int playerIndex = Player.whoAmI;
			if (!Scheduler.TryGetValue(playerIndex, out List<(TransactionType, int, int)> pending) || pending.Count == 0)
			{
				return;
			}
			foreach (var (type, a, b) in pending)
			{
				ApplyOperation(type, Player, a, b);
			}
			pending.Clear();
		}

		private void ApplyOperation(TransactionType type, Player player, int a, int b)
		{
			switch (type)
			{
				case TransactionType.PlayerItemSubtraction:
					int slotIndx = a;
					int expectedItemType = b;
					//ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"ItemSub: slot={slotIndx}, expectedType={expectedItemType}, invLen={player.inventory.Length}"), Color.White);
					if (slotIndx < 0 || slotIndx >= player.inventory.Length)
					{
						//ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Bailed: slot out of range"), Color.White);
						break;
					}

					Item item = player.inventory[slotIndx];
					if (item.IsAir || item.type != expectedItemType)
					{
						//ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"Bailed: item.IsAir={item.IsAir}, item.type={item.type} vs expected={expectedItemType}"), Color.White);
						break;
					}

					item.TurnToAir();
					break;
				case TransactionType.PlayerBuff:
					int buffTime = a;
					int buffID = b;
					player.AddBuff(buffID, buffTime);
					break;
				case TransactionType.PlayerCoinSubtraction:
					int price = a;
					player.BuyItem(price);
					break;
			}
		}
		public static void ScheduleOperation(TransactionType type, int playerIndex, int param1, int param2)
		{
			if (playerIndex < 0 || playerIndex >= Main.maxPlayers)
			{
				return;
			}
			if (!Scheduler.TryGetValue(playerIndex, out List<(TransactionType, int, int)> list))
			{
				list = new List<(TransactionType, int, int)>();
				Scheduler[playerIndex] = list;
			}
			list.Add((type, param1, param2));
		}
	}
}
