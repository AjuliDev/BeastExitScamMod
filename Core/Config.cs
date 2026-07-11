using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BeastExitScamMod.Core
{
	internal class Config : ModConfig
	{
		public static Config Instance;
		public override ConfigScope Mode => ConfigScope.ServerSide;
		public enum EventDifficulty
		{
			Easy = 1,
			Medium = 2,
			Hard = 3
		}
		[BackgroundColor(50, 250, 50, 255)]
		[DefaultValue(EventDifficulty.Easy)]
		public EventDifficulty Difficulty { get; set; }
	}
}
