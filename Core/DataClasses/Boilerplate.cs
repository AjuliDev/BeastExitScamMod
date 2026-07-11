using BeastScam.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;

namespace BeastExitScamMod.Core.DataClasses
{
	internal class Boilerplate
	{
		/// <summary>
		/// Gets a random amount of ads in the form of an integer. Only to be called on singleplayer or server. Takes config into account.
		/// </summary>
		/// <returns></returns>
		public static int GetRandomAmountOfAds()
		{
			Config config = Config.Instance;
			int randomAmount = Main.rand.Next(ConstantData.C_WINDOW_BASE_AMOUNT_MIN_RAND, ConstantData.C_WINDOW_BASE_AMOUNT_MAX_RAND);
			if (config == null || config.Difficulty == Config.EventDifficulty.Easy)
			{
				return (int)Math.Ceiling(randomAmount * ConstantData.C_WINDOW_AMOUNT_MULTIPLIER_EASY);
			}
			else if (config.Difficulty == Config.EventDifficulty.Medium)
			{
				return (int)Math.Ceiling(randomAmount * ConstantData.C_WINDOW_AMOUNT_MULTIPLIER_MEDIUM);
			}
			else if (config.Difficulty == Config.EventDifficulty.Hard)
			{
				return (int)Math.Ceiling(randomAmount * ConstantData.C_WINDOW_AMOUNT_MUTLIPLIER_HARD);
			}
			else
			{
				return ConstantData.C_WINDOW_BASE_AMOUNT_FALLBACK;
			}
		}
		/// <summary>
		/// This function returns a random window frame texture and window icon texture.
		/// </summary>
		/// <returns></returns>
		public static Texture2D[] GetRandomWindowTextures()
		{
			Texture2D WindowFrameTexture = ConstantData.WINDOW_FRAME_TEXTURES[Main.rand.Next(ConstantData.WINDOW_FRAME_TEXTURES.Length)];
			Texture2D WindowIconTexture;
			if (WindowFrameTexture == GeneratedSource.scam_alert_01?.Value)
			{
				WindowIconTexture = ConstantData.WINDOW_FRAME_ICON_TEXTURES[Main.rand.Next(ConstantData.WINDOW_FRAME_ICON_TEXTURES.Length)];
			}
			else
			{
				WindowIconTexture = GeneratedSource.empty_icon?.Value;
			}
			return
				[
				WindowFrameTexture,
				WindowIconTexture
				];
		}
		/// <summary>
		/// This functions plays a window opening sound when given a true bool and a window closing sound when given a false bool.
		/// </summary>
		/// <param name="openWindow"></param>
		public static void PlayWindowSound(bool openWindow = true)
		{
			if (openWindow)
			{
				SoundEngine.PlaySound(ConstantData.WINDOW_FRAME_OPEN_SOUNDS[Main.rand.Next(ConstantData.WINDOW_FRAME_OPEN_SOUNDS.Length)]);
			}
			else
			{
				SoundEngine.PlaySound(ConstantData.WINDOW_FRAME_CLOSE_SOUNDS[Main.rand.Next(ConstantData.WINDOW_FRAME_CLOSE_SOUNDS.Length)]);
			}
		}
	}
}
