using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BeastExitScamMod.Core
{
	internal class MessagingHandler : ModSystem // Localized messages, because Gabe told me
	{
		public enum MessageType
		{
			WonAnItem = 1,
			LostAnItem = 2,
			ReceivedGoodBuff = 3,
			ReceivedBadBuff = 4
		}
		public enum Tiers
		{
			TierOne = 1,
			TierTwo = 2,
			TierThree = 3,
			TierFour = 4, // Item Exclusive
			LostItem = 5
		}

		public static void NetworkAnnouncement(MessageType type, int referenceID, Tiers rewardTier, int targetPlayerID, bool isBuff = false)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return;
			}
			NetworkText displayMessage;
			Color messageColors = (type, rewardTier) switch
			{
				(MessageType.WonAnItem, Tiers.TierOne) => new Color(192, 192, 192),
				(MessageType.WonAnItem, Tiers.TierTwo) => new Color(0, 164, 224),
				(MessageType.WonAnItem, Tiers.TierThree) => new Color(114, 71, 255),
				(MessageType.WonAnItem, Tiers.TierFour) => new Color(255, 61, 251),
				(MessageType.LostAnItem, Tiers.LostItem) => new Color(255, 35, 35),
				(MessageType.ReceivedGoodBuff, Tiers.TierOne) => new Color(255, 211, 102),
				(MessageType.ReceivedGoodBuff, Tiers.TierTwo) => new Color(255, 106, 43),
				(MessageType.ReceivedGoodBuff, Tiers.TierThree) => new Color(138, 71, 255),
				(MessageType.ReceivedBadBuff, Tiers.TierOne) => new Color(255, 102, 99),
				(MessageType.ReceivedBadBuff, Tiers.TierTwo) => new Color(255, 30, 38),
				(MessageType.ReceivedBadBuff, Tiers.TierThree) => new Color(63, 35, 42),
				_ => new Color(255, 255, 255)
			};
			int maxOptions = type == default ? 1 : 5;
			string localizationKey = $"Mods.BeastExitScamMod.Messages.Rewards.{type}.{rewardTier}.{Main.rand.Next(maxOptions)}";
			string rewardName = isBuff == false ? Lang.GetItemNameValue(referenceID) : Lang.GetBuffName(referenceID);
			displayMessage = NetworkText.FromKey(
				localizationKey,
				Main.player[targetPlayerID].name,
				rewardName
				);
			ChatHelper.BroadcastChatMessage(displayMessage, messageColors);
		}
	}
}
