using Blazor_2048.Extensions;
using Blazor_2048.Enums;

namespace Blazor_2048.Services
{
	public class GameStateService : IGameStateService
	{
		private readonly Random r = new Random();

		private Tile[] Tiles { get; set; } = new Tile[Board.Board_Height * Board.Board_Width];
        public bool HaHaYouLost { get; set; } = false;
		public bool IsMoving { get; private set; } = false;
		public bool IsInitialized { get; private set; } = false;

        private static int Score { get; set; } = 0;

        public Tile[][] GetRows() => Tiles.GetRows();
        public Tile[][] GetColumns() => Tiles.GetColumns();
        public int GetScore() => Score;

        private async Task<bool> HasLost()
           => await Task.FromResult(Tiles.ToList().All(t => t.Value > 0)
               && !(Tiles.GetRows().Any(t => t.HasConsecutiveDuplicates()) || Tiles.GetColumns().Any(t => t.HasConsecutiveDuplicates())));

        public async Task ResetBoardAsync()
        {
            Score = 0;

            for (var i = 0; i < Tiles.Length; i++)
                Tiles[i] = new Tile(0);

			HaHaYouLost = false;

			for (var i = 0; i < Board.Board_Starting_Tiles; i++)
                await GenerateNewTileAsync();

        }

        private async Task GenerateNewTileAsync()
        {
            Tile[] emptyTiles = Tiles.Where(t => t.Value == 0).ToArray();
            int index = r.Next(emptyTiles.Length);

            Tile tile = emptyTiles[index];
			tile.Value = await GenerateNewTileValueAsync();
            tile.NewTile = true;

		}

        private async Task<int> GenerateNewTileValueAsync()
        {
            return await Task.FromResult(r.Next(0, 100) <= 89 ? 2 : 4);
        }

        public async Task MoveAsync(Move move)
        {
            if (HaHaYouLost || IsMoving)
                return;

			IsMoving = true;

			Tile[] newTiles = Tiles.Select(t => new Tile(t.Value)).ToArray();

            bool HasMoved = move switch
            {
                Move.UP => newTiles.GetColumns().Aggregate(false, (acc, column) => acc | MoveLine(column)),
                Move.RIGHT => newTiles.GetRows().Aggregate(false, (acc, row) => acc | MoveLine(row.Reversed())),
                Move.DOWN => newTiles.GetColumns().Aggregate(false, (acc, column) => acc | MoveLine(column.Reversed())),
                Move.LEFT => newTiles.GetRows().Aggregate(false, (acc, row) => acc | MoveLine(row)),
                _ => throw new NotSupportedException("Huh?")
            };
            if (HasMoved)
            {
                Tiles = Tiles.Zip(newTiles, (a, b) => new Tile(a.Value, b.AnimationFactor)).ToArray();

                await Task.Delay(120);

                Tiles = newTiles.Select(t => new Tile(t.Value, t.Merged)).ToArray();

                await GenerateNewTileAsync();

            }
            IsMoving = false;
        }

        private static bool MoveLine(Tile[] tiles)
        {
            bool Moved = false;
            for (int i = 0; i < tiles.Length - 1; i++)
            {
                for (int j = i + 1; j < tiles.Length; j++)
                {
                    // Can't move.
                    if (tiles[i].Value != 0 && tiles[j].Value != 0 && tiles[i].Value != tiles[j].Value)
                        break;

                    // Can move but not merge.
                    if (tiles[i].Value == 0 && tiles[j].Value != 0)
                    {
                        var value = tiles[j].Value;
                        tiles[j].Value = 0;
                        tiles[i].Value = value;
						tiles[j].AnimationFactor = j - i;
						Moved = true;
                        continue;
                    }

                    // Can move and merge.
                    if (tiles[i].Value == tiles[j].Value && tiles[i].Value != 0)
                    {
                        var value = tiles[i].Value + tiles[j].Value;
                        tiles[j].Value = 0;
                        tiles[i].Value = value;
						tiles[j].AnimationFactor = j - i;
                        Score += value; 
						Moved = true;
						tiles[i].Merged = true;
						break;
                    }
                }
            }

            return Moved;
        }
    }
}
