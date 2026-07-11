using BeastExitScamMod.Core.DataClasses;
using Terraria;
using Terraria.ModLoader;

namespace BeastExitScamMod.Core
{
	internal class EventHandler : ModSystem
	{
		public override void OnWorldLoad() => Data = new PlayerData[Main.maxPlayers];
		public override void Load() => Data = new PlayerData[Main.maxPlayers];
		public override void OnWorldUnload() => Data = null;
		// Server first, client does rendering and human interaction
		/// <summary>
		/// Server-only player data class
		/// </summary>
		public static PlayerData[] Data;
		/// <summary>
		/// Client to server request function to request a new scam
		/// </summary>
		public static void ClientRequestEventStart()
		{
			ModPacket newNetworkPacket = ModContent.GetInstance<BeastExitScamMod>().GetPacket();
			newNetworkPacket.Write((byte)PacketHandler.PacketType.RequestExitScam);
			newNetworkPacket.Send(-1, -1);
		}
		/// <summary>
		/// Server to client approval function to enable the new scam that was requested
		/// </summary>
		public static void ClientReceiveEventStart()
		{
			ModContent.GetInstance<UIHandler>().StartExitScam();
		}
	}
}
