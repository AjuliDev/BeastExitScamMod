using BeastScam.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Audio;

namespace BeastExitScamMod.Core.DataClasses
{
	internal static class ConstantData
	{
		/// <summary>
		/// Time allocated to each player to close all ads given.
		/// </summary>
		public const int C_TIME_ALLOCATED_TO_CLOSE_ADS = 15 * 60;
		/// <summary>
		/// Window amount multiplier, easy difficulty
		/// </summary>
		public const float C_WINDOW_AMOUNT_MULTIPLIER_EASY = 1.0f;
		/// <summary>
		/// Window amount multiplier, medium difficulty
		/// </summary>
		public const float C_WINDOW_AMOUNT_MULTIPLIER_MEDIUM = 1.5f;
		/// <summary>
		/// Window amount multiplier, hard difficulty
		/// </summary>
		public const float C_WINDOW_AMOUNT_MUTLIPLIER_HARD = 2.0f;
		/// <summary>
		/// Minimum amount of windows to be given to the randomizer, before the difficulty multiplier
		/// </summary>
		public const int C_WINDOW_BASE_AMOUNT_MIN_RAND = 5;
		/// <summary>
		/// Maximum amount of windows to be given to the randomizer, before the difficulty multiplier
		/// </summary>
		public const int C_WINDOW_BASE_AMOUNT_MAX_RAND = 15;
		/// <summary>
		/// Fallback amount of windows to be given to the player if the randomizer bugs out
		/// </summary>
		public const int C_WINDOW_BASE_AMOUNT_FALLBACK = 2;
		/// <summary>
		/// Item use time for the Beast Phone
		/// </summary>
		public const int C_BEAST_PHONE_USE_TIME = 3 * 60;
		/// <summary>
		/// Item use cooldown for the Beast Phone
		/// </summary>
		public const int C_BEAST_PHONE_COOLDOWN = 30 * 60;
		/// <summary>
		/// Window frame textures, self explanatory, filled with textures from GeneratedSource
		/// </summary>
		public static readonly Texture2D[] WINDOW_FRAME_TEXTURES =
		[
			GeneratedSource.scam_alert_01?.Value,
			GeneratedSource.scam_alert_02?.Value,
			GeneratedSource.scam_alert_03?.Value
		];
		/// <summary>
		/// Window icon textures, self explanatory, filled with textures from GeneratedSource
		/// </summary>
		public static readonly Texture2D[] WINDOW_FRAME_ICON_TEXTURES =
			[
			GeneratedSource.icon_01?.Value,
			GeneratedSource.icon_02?.Value,
			GeneratedSource.icon_03?.Value
			];
		/// <summary>
		/// Window open sounds, self explanatory, filled with sounds from GeneratedSource
		/// </summary>
		public static readonly SoundStyle[] WINDOW_FRAME_OPEN_SOUNDS =
			[
			GeneratedSource.popup_01,
			GeneratedSource.popup_02,
			GeneratedSource.popup_03
			];
		/// <summary>
		/// Window close sounds, self explanatory, filled with sounds from GeneratedSource
		/// </summary>
		public static readonly SoundStyle[] WINDOW_FRAME_CLOSE_SOUNDS =
			[
			GeneratedSource.click_01,
			GeneratedSource.click_02
			];
	}
}
