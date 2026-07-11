using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeastExitScamMod.Core.Prefabs
{
	/// <summary>
	/// Window class, implements the following: <br/>
	/// Texture2D <see cref="WindowFrame"/> as the window texture. <br/>
	/// Texture2D <see cref="WindowIcon"/> as the icon at the top left of the window texture, for small windows. <br/>
	/// Vector2 <see cref="Position"/> as the position of the window <br/>
	/// int <see cref="CloseButtonSizeSquaredInPixels"/> as the size of the close button on windows (the detection area, not the sprite), defaulting to 20.
	/// </summary>
	internal class Window
	{
		public Texture2D WindowFrame;
		public Texture2D WindowIcon;
		public Vector2 Position;
		public int CloseButtonSizeSquaredInPixels = 20;
		public Rectangle CloseButtonRectangle => new Rectangle((int)(Position.X + WindowFrame.Width - CloseButtonSizeSquaredInPixels), (int)Position.Y, CloseButtonSizeSquaredInPixels, CloseButtonSizeSquaredInPixels);
		public Vector2 RelativeIconPosition => new Vector2((int)(Position.X + 7), (int)(Position.Y + 4));
	}
}
