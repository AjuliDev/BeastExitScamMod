using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BeastExitScamMod.Core
{
	internal class PlayerHandler : ModPlayer // Cut loose endz
	{
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
		}
	}
}
