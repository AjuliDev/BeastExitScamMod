using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace BeastExitScamMod.Core.Prefabs
{
	/// <summary>
	/// ProgressBar class, implements the following: <br/>
	/// Texture2D <see cref="BarFrame"/> as the main texture <br/>
	/// old: Vector2 <see cref="PositionOnScreen"/> as the screen position <br/>
	/// int <see cref="Percentage"/> as the Percentage of the total ScaledBarWidth to show <br/>
	/// old: int <see cref="ScaledBarWidth"/> as the new horizontal sprite scale
	/// old:  int <see cref="ScaledBarHeight"/> as the new vertical sprite scale, defaults to 1
	/// Rectangle <see cref="DestinationRectangle"/> as the position and scale of the sprite.
	/// </summary>
	internal class ProgressBar
	{
		public Texture2D BarFrame;
		//public Vector2 PositionOnScreen => new Vector2((Main.screenWidth / Main.UIScale) * 0.5f, ((Main.screenHeight / Main.UIScale) - BarFrame.Height) * 0.8f);
		public float Percentage;
		public float ScaledBarWidth;
		public float ScaledScreenWidth;
		public float ScaledScreenHeight;
		public Rectangle DestinationRectangle
		{
			get
			{
				float currentSpriteWidth = ScaledScreenWidth * Percentage;
				float spriteHeight = BarFrame?.Height ?? 1;
				float horizontalPosition = (ScaledScreenWidth - currentSpriteWidth) * 0.5f;
				float verticalPosition = (ScaledScreenHeight - spriteHeight) * 0.9f;
				return new Rectangle((int)horizontalPosition, (int)verticalPosition, (int)currentSpriteWidth, (int)spriteHeight);
			}
		}
		//public float ScaledBarHeight = 1;
		//public Vector2 SpritebatchOrigin => new Vector2(BarFrame.Width / 2f, BarFrame.Height / 2f);
		//public Vector2 SpritebatchScale => new Vector2((ScaledBarWidth / BarFrame.Width) * Percentage, ScaledBarHeight);
	}
}
