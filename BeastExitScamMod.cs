using BeastExitScamMod.Core;
using System.IO;
using Terraria.ModLoader;

namespace BeastExitScamMod
{
	public class BeastExitScamMod : Mod
	{
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			PacketHandler.HandlePacket(reader, whoAmI);
		}
	}
}
