using BeastExitScamMod.Core;
using BeastExitScamMod.Core.DataClasses;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static BeastExitScamMod.Core.PacketHandler;

namespace BeastExitScamMod.Content
{
	internal class BeastPhone : ModItem
	{
		private int useCooldown = 0;
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = 1;
			Item.value = 0;
			Item.rare = ItemRarityID.Master;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = ConstantData.C_BEAST_PHONE_USE_TIME;
		}
		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
			{
				return true;
			}
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				EventHandler.ClientRequestEventStart();
				ModPacket packet = ModContent.GetInstance<BeastExitScamMod>().GetPacket();
				packet.Write((byte)PacketType.IHaveGambled); // this is to reset the timings in ScamScheduler btw, if u forget (you will)
				packet.Send(toClient: Main.myPlayer, -1);
			}
			else if (Main.netMode == NetmodeID.SinglePlayer)
			{
				ModContent.GetInstance<UIHandler>().ClientData = new PlayerData
				{
					AllocatedTime = ConstantData.C_TIME_ALLOCATED_TO_CLOSE_ADS,
					AdsGiven = Boilerplate.GetRandomAmountOfAds(),
					AdsClosed = 0
				};
				ModContent.GetInstance<UIHandler>().StartExitScam();
				ModContent.GetInstance<ScamScheduler>().UpdateEntry(Main.myPlayer);
			}
			useCooldown = ConstantData.C_BEAST_PHONE_COOLDOWN;
			return true;
		}
		public override bool CanUseItem(Player player)
		{
			//if (useCooldown > 0)
			//{
			//	ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"BeastExitScamMod: On cooldown: {useCooldown} ticks left"), Color.Red);
			//}
			return useCooldown <= 0;
		}
		public override void UpdateInventory(Player player)
		{
			if (useCooldown > 0)
			{
				useCooldown--;
			}
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			bool holdingShift = Main.keyState.PressingShift();
			if (holdingShift)
			{
				tooltips.Add(new TooltipLine(Mod, "BeastPhoneShiftLines",
					"I'm excited to announce the launch of my very own crypto casino! To celebrate, I'm giving away $2,500 to everyone who registers - and you can withdraw the bonus instantly.\n" +
					"\n" +
					"How to claim your reward:\n" +
					"1. Go to the TModLoader discord server.\n" +
					"2. Start spamming this\n" +
					"3. Get banned\n" +
					"4. ???\n" +
					"5. Receive your $2,500 bonus\n" +
					"\n" +
					"This post will be deleted one hour after publication. The promotion will run for a few days, so don't miss your chance!")
				{
					OverrideColor = new Color(158, 209, 229)
				});
				tooltips.Add(new TooltipLine(Mod, "BeastPhoneShiftExtraDisclaimer",
					"Hold [Shift] to view promotional ad!")
				{
					OverrideColor = new Color(252, 57, 3)
				});
			}
			else
			{
				tooltips.Add(new TooltipLine(Mod, "BeastPhonePreShift",
					"Hold [Shift] to view promotional ad!")
				{
					OverrideColor = new Color(252, 57, 3)
				});
				if (useCooldown > 0)
				{
					tooltips.Add(new TooltipLine(Mod, "BeastPhoneCooldownPreShift",
						$"You'll be able to use this item again in {System.MathF.Ceiling(useCooldown / 60)}!")
					{
						OverrideColor = new Color(255, 253, 109)
					});
				}
			}
		}
	}
}
