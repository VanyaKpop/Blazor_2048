using Blazor_2048.Enums;

namespace Blazor_2048.Services
{
	public interface IGameStateService
	{
		bool IsInitialized { get; }
		static int Score { get; }
		bool IsMoving { get; }
		Tile[][] GetRows();
		Tile[][] GetColumns();
		int GetScore();
		Task MoveAsync(Move move);
        Task ResetBoardAsync();
	}
}
