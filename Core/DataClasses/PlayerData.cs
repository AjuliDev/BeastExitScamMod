namespace BeastExitScamMod.Core.DataClasses
{
	/// <summary>
	/// Player data class which implements the following: <br/>
	/// int <see cref="AllocatedTime"/> as the time given to the player to complete the minigame.<br/>
	/// int <see cref="AdsGiven"/> as the amount of ads the server decided to give the player to close, in accordance to the server chosen difficulty.<br/>
	/// int <see cref="AdsClosed"/> as the amount of ads the player has managed to close.
	/// </summary>
	internal class PlayerData
	{
		public int AllocatedTime = 0;
		public int AdsGiven = 0;
		public int AdsClosed = 0;
	}
}
