using BeastExitScamMod.Core.DataClasses;
using BeastExitScamMod.Core.Prefabs;
using BeastScam.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace BeastExitScamMod.Core
{
	internal class UIHandler : ModSystem
	{
		private float RegisteredUIScaling = Main.UIScale;
		private List<Window> ActiveWindows = new List<Window>();
		private ProgressBar TimeRemainingProgressBar = new ProgressBar();
		public PlayerData ClientData;
		private int Timer;
		/// <summary>
		/// Cleanup function, used in multiple spots to prevent issues.
		/// </summary>
		private void Cleanup()
		{
			Timer = 0;
			TimeRemainingProgressBar = new ProgressBar();
			ActiveWindows = new List<Window>();
			RegisteredUIScaling = Main.UIScale;
		}
		public override void PostSetupContent() => Cleanup();
		public override void OnWorldUnload() => Cleanup();
		public override void OnWorldLoad() => Cleanup();
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int referenceIndex = layers.FindIndex(targetLayer => targetLayer.Name.Equals("Vanilla: Mouse Text"));
			if (referenceIndex != -1)
			{
				layers.Insert(referenceIndex + 1, new LegacyGameInterfaceLayer("BeastScam: Window",
					delegate
					{
						DrawWindows();
						return true;
					}, InterfaceScaleType.UI));

				layers.Insert(referenceIndex + 2, new LegacyGameInterfaceLayer("BeastScam: ProgressBar",
					delegate
					{
						DrawProgressBar();
						return true;
					}, InterfaceScaleType.UI));
			}
		}
		/// <summary>
		/// This function starts the main function of the mod.
		/// The windows appear.
		/// The progress bar that tracks your timer until failure appears.
		/// </summary>
		public void StartExitScam()
		{
			SpawnWindows();
			SpawnProgressBar();
		}
		private void HandleScaling()
		{
			if (RegisteredUIScaling != Main.UIScale)
			{
				RegisteredUIScaling = Main.UIScale;
				if (TimeRemainingProgressBar != null)
				{
					TimeRemainingProgressBar.ScaledScreenWidth = Main.screenWidth / Main.UIScale;
					TimeRemainingProgressBar.ScaledScreenHeight = Main.screenHeight / Main.UIScale;
				}
				if (ClientData != null && ClientData.AdsGiven > 0 && ClientData.AdsGiven > ClientData.AdsClosed)
				{
					SpawnWindows(ClientData.AdsGiven - ClientData.AdsClosed);
				}
			}
		}
		public override void PostDrawInterface(SpriteBatch spriteBatch)
		{
			// Slight problem with the timer progress bar if someone changes the ui scaling mid-way but honestly why tf would someone do that instead of closing the windows, already handled the windows themselves.
			HandleCloseButton();
			HandleScaling();
			if (TimeRemainingProgressBar != null && TimeRemainingProgressBar.Percentage > 0f)
			{
				Timer++;
				TimeRemainingProgressBar.Percentage = 1f - (Timer / (float)ClientData.AllocatedTime);
				if (Timer >= ClientData.AllocatedTime)
				{
					TimeRemainingProgressBar.Percentage = 0f;
				}
			}
			if (TimeRemainingProgressBar != null && TimeRemainingProgressBar.Percentage <= 0f && ClientData != null && ClientData.AdsGiven > 0)
			{
				TimeRemainingProgressBar = null; // don't fuck with this or it goes to shit and I'm not rewriting allat
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					ModPacket newNetworkPacket = ModContent.GetInstance<BeastExitScamMod>().GetPacket();
					newNetworkPacket.Write((byte)PacketHandler.PacketType.TransmitClientData);
					newNetworkPacket.Write((int)ClientData.AdsGiven);
					newNetworkPacket.Write((int)ClientData.AdsClosed);
					newNetworkPacket.Send(-1, -1);
				}
				else if (Main.netMode == NetmodeID.SinglePlayer)
				{
					ScamOrBuffHandler SOBHinstance = ModContent.GetInstance<ScamOrBuffHandler>();
					bool calculatedRandomizedReward = SOBHinstance.DecideResult(ClientData.AdsGiven, ClientData.AdsClosed);
					SOBHinstance.HandleServerReward(rewardType: calculatedRandomizedReward, targetPlayerID: Main.myPlayer);
					SOBHinstance.HandleClientReward(rewardType: calculatedRandomizedReward);
				}
			}
		}
		/// <summary>
		/// This function spawns the ad windows and their respective icons, for the small variants, ending with a sound.
		/// </summary>
		private void SpawnWindows(int forceRespawnAmount = 0)
		{
			if (forceRespawnAmount > 0)
			{
				SpawnWindows_ForLoop(forceRespawnAmount);
				return;
			}
			ActiveWindows.Clear();
			if (ClientData.AdsGiven <= 0)
			{
				ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("BeastExitScamMod: Something went seriously wrong when trying to assign a random number of ads."), Color.Red);
			}
			SpawnWindows_ForLoop(ClientData.AdsGiven);
			Boilerplate.PlayWindowSound(true);
		}
		private void SpawnWindows_ForLoop(int Amount)
		{
			for (int i = 0; i < Amount; i++)
			{
				Texture2D[] windowTextures = Boilerplate.GetRandomWindowTextures();
				Vector2 screenLimits = new Vector2
					(
					Math.Max(0, (int)(Main.screenWidth / Main.UIScale) - windowTextures[0].Width),
					Math.Max(0, (int)(Main.screenHeight / Main.UIScale) - windowTextures[0].Height)
					);
				ActiveWindows.Add(new Window
				{
					WindowFrame = windowTextures[0],
					WindowIcon = windowTextures[1],
					Position = new Vector2(Main.rand.Next(0, (int)screenLimits.X), Main.rand.Next(0, (int)screenLimits.Y))
				});
			}
		}
		/// <summary>
		/// This function is called to draw each window frame and window icon from ActiveWindows
		/// </summary>
		private void DrawWindows()
		{
			for (int i = 0; i < ActiveWindows.Count; i++)
			{
				Window currentWindow = ActiveWindows[i];
				Main.spriteBatch.Draw(currentWindow.WindowFrame, currentWindow.Position, Color.White);
				Main.spriteBatch.Draw(currentWindow.WindowIcon, currentWindow.RelativeIconPosition, Color.White);
			}
		}
		/// <summary>
		/// This function checks if your mouse is in one of the windows X buttons and whether you clicked it or not, thus removing the window you were pointing at if you did click it.
		/// </summary>
		private void HandleCloseButton()
		{
			Point currentMousePosition = Main.MouseScreen.ToPoint();
			bool mouseClicked = Main.mouseLeft && Main.mouseLeftRelease;
			bool inputHandled = false;
			for (int i = ActiveWindows.Count - 1; i >= 0; i--)
			{
				Window currentWindow = ActiveWindows[i];
				if (!inputHandled && currentWindow.CloseButtonRectangle.Contains(currentMousePosition))
				{
					Main.LocalPlayer.mouseInterface = true;
					if (mouseClicked)
					{
						ActiveWindows.RemoveAt(i);
						inputHandled = true;
						Boilerplate.PlayWindowSound(false);
						ClientData.AdsClosed += 1;
					}
				}
			}
		}
		/// <summary>
		/// This function spawns the progress bar used to measure time remaining until failure
		/// </summary>
		private void SpawnProgressBar()
		{
			Timer = 0;
			TimeRemainingProgressBar = new ProgressBar()
			{
				BarFrame = GeneratedSource.cooldown_timer_sprite?.Value,
				Percentage = 1.0f,
				ScaledBarWidth = (Main.screenWidth / Main.UIScale) * 0.8f,
				ScaledScreenWidth = Main.screenWidth / Main.UIScale,
				ScaledScreenHeight = Main.screenHeight / Main.UIScale
			};
		}
		/// <summary>
		/// This function is called to draw the progress bar which measures time remaining until failure
		/// </summary>
		private void DrawProgressBar()
		{
			if (TimeRemainingProgressBar != null && TimeRemainingProgressBar.BarFrame != null && TimeRemainingProgressBar.Percentage > 0)
			{
				//Main.spriteBatch.Draw(
				//	TimeRemainingProgressBar.BarFrame,
				//	TimeRemainingProgressBar.PositionOnScreen,
				//	null,
				//	Color.White,
				//	0f,
				//	TimeRemainingProgressBar.SpritebatchOrigin,
				//	TimeRemainingProgressBar.SpritebatchScale,
				//	SpriteEffects.None,
				//	0f);
				Main.spriteBatch.Draw(
					TimeRemainingProgressBar.BarFrame,
					TimeRemainingProgressBar.DestinationRectangle,
					Color.White);
			}
		}
	}
}
