
namespace Blazor_2048
{
	public class Tile
	{
		public int AnimationFactor { get; set; } = 0;
		public bool NewTile { get; set; } = false;
		public bool Merged { get; set; } = false;
		public int Value { get; set; } = 0;

		public Tile(int value)
		{
			Value = value;
		}
		public Tile(int value, int animationFactor) : this(value)
		{
			AnimationFactor = animationFactor;
		}

		public Tile(int value, bool merged) : this(value)
		{
			Merged = merged;
		}

	}
}
