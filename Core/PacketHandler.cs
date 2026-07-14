using BeastExitScamMod.Core.DataClasses;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.Decoding;

namespace BeastExitScamMod.Core
{
	internal class PacketHandler
	{
		internal enum PacketType : byte
		{
			RequestExitScam,
			LaunchExitScam,
			TransmitClientData,
			ServerBroadcastResult,
			IHaveGambled,
			SyncPlayerChanges
		}
		/// <summary>
		/// This class handles packets for the entire mod, redirected here from the main mod class.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="whoAmI"></param>
		public static void HandlePacket(BinaryReader reader, int whoAmI)
		{
			PacketType networkPacketType = (PacketType)reader.ReadByte();
			switch (networkPacketType)
			{
				case PacketType.RequestExitScam: // Server Packet
					if (Main.netMode != NetmodeID.Server)
					{
						break;
					}
					EventHandler.Data[whoAmI] = new PlayerData
					{
						AllocatedTime = ConstantData.C_TIME_ALLOCATED_TO_CLOSE_ADS,
						AdsGiven = Boilerplate.GetRandomAmountOfAds(),
						AdsClosed = 0
					};
					ModPacket responsePacket = ModContent.GetInstance<BeastExitScamMod>().GetPacket();
					responsePacket.Write((byte)PacketType.LaunchExitScam);
					responsePacket.Write((int)EventHandler.Data[whoAmI].AllocatedTime);
					responsePacket.Write((int)EventHandler.Data[whoAmI].AdsGiven);
					responsePacket.Write((int)EventHandler.Data[whoAmI].AdsClosed);
					responsePacket.Send(toClient: whoAmI);
					break;
				case PacketType.LaunchExitScam: // Client Packet
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						break;
					}
					UIHandler uiHandler = ModContent.GetInstance<UIHandler>();
					uiHandler.ClientData = new PlayerData
					{
						AllocatedTime = reader.ReadInt32(),
						AdsGiven = reader.ReadInt32(),
						AdsClosed = reader.ReadInt32()
					};
					EventHandler.ClientReceiveEventStart();
					break;
				case PacketType.TransmitClientData: // Server Packet
					if (Main.netMode != NetmodeID.Server)
					{
						break;
					}
					// consume these just in case ig
					int adsGiven = reader.ReadInt32();
					int adsClosed = reader.ReadInt32();
					if (EventHandler.Data[whoAmI] == null)
					{
						break;
						// message here
					}
					EventHandler.Data[whoAmI].AdsGiven = adsGiven;
					EventHandler.Data[whoAmI].AdsClosed = adsClosed;
					ScamOrBuffHandler SOBHinstance = ModContent.GetInstance<ScamOrBuffHandler>();
					bool calculatedRandomizedReward = SOBHinstance.DecideResult(EventHandler.Data[whoAmI].AdsGiven, EventHandler.Data[whoAmI].AdsClosed);
					ModPacket secondResponsePacket = ModContent.GetInstance<BeastExitScamMod>().GetPacket();
					secondResponsePacket.Write((byte)PacketType.ServerBroadcastResult);
					secondResponsePacket.Write((bool)calculatedRandomizedReward);
					secondResponsePacket.Send(toClient: whoAmI);
					SOBHinstance.HandleServerReward(rewardType: calculatedRandomizedReward, targetPlayerID: whoAmI);
					break;
				case PacketType.ServerBroadcastResult: // Client Packet
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						break;
					}
					ModContent.GetInstance<ScamOrBuffHandler>().HandleClientReward(rewardType: reader.ReadBoolean());
					break;
				case PacketType.IHaveGambled:
					if (Main.netMode == NetmodeID.MultiplayerClient)
					{
						break;
					}
					ModContent.GetInstance<ScamScheduler>().UpdateEntry(whoAmI);
					break;
				case PacketType.SyncPlayerChanges:
					//PlayerHandler.DoPlayerOperations((PlayerHandler.TransactionType)reader.ReadByte(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
					PlayerHandler.ScheduleOperation(
						(PlayerHandler.TransactionType)reader.ReadByte(),
						reader.ReadInt32(),
						reader.ReadInt32(),
						reader.ReadInt32());
					break;
				default:
					ModContent.GetInstance<BeastExitScamMod>().Logger.WarnFormat("BeastExitScamMod: Unknown packet type: {0}", networkPacketType);
					break;
			}
		}
	}
}
