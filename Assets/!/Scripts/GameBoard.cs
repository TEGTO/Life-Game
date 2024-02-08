using ConcurrentCollections;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameControlling
{
	public class GameBoard : MonoBehaviour
	{
		[SerializeField]
		private Tilemap currentTilemapState;
		[SerializeField]
		private Tile aliveTile;
		[SerializeField]
		private Tile deadTile;
		[SerializeField]
		private float updateInterval = 0.05f;

		private CancellationTokenSource tokenSource;
		private ConcurrentHashSet<Vector3Int> aliveCells = new ConcurrentHashSet<Vector3Int>();
		//true - add a new tile, false - delete an old tile
		private ConcurrentDictionary<Vector3Int, bool> operations = new ConcurrentDictionary<Vector3Int, bool>();

		public int Population { get; private set; }
		public int Iterations { get; private set; }
		public float Time { get; private set; }
		public float UpdateIntervalCoeff { get; set; } = 1f;
		private float UpdateInterval { get => updateInterval / UpdateIntervalCoeff; }
		public Tilemap CurrentTilemapState { get => currentTilemapState; }

		private void OnDisable()
		{
			StopSimulate();
		}
		public async void StartSimulate()
		{
			tokenSource = new CancellationTokenSource();
			await Simulate(tokenSource.Token);
		}
		public void StopSimulate()
		{
			if (tokenSource != null && !tokenSource.IsCancellationRequested)
			{
				tokenSource.Cancel();
				tokenSource.Dispose();
			}
		}
		public async void SetPattern(LifePattern pattern)
		{
			Clear();
			Vector2Int tilesCenter = pattern.GetCenter();
			await Awaitable.BackgroundThreadAsync();
			Parallel.ForEach(pattern.cells, cell =>
			{
				Vector3Int centeredCell = (Vector3Int)(cell - tilesCenter);
				AddNewLiveTile(centeredCell);
			});
			await Awaitable.MainThreadAsync();
			UpdateTilemap();
			Population = aliveCells.Count;
		}
		public void Clear()
		{
			aliveCells.Clear();
			operations.Clear();
			currentTilemapState.ClearAllTiles();
			Population = 0;
			Iterations = 0;
			Time = 0f;
		}
		public void AddNewLiveTile(Vector3Int cell) =>
			operations[cell] = true;
		public void RemoveTile(Vector3Int cell) =>
			operations[cell] = false;
		public void UpdateTilemap()
		{
			foreach (var operation in operations)
				UpdateCellTile(operation.Value, operation.Key);
			operations.Clear();
		}
		private void UpdateCellTile(bool operation, Vector3Int cell)
		{
			if (operation)
			{
				if (!aliveCells.Contains(cell))
				{
					aliveCells.Add(cell);
					currentTilemapState.SetTile(cell, aliveTile);
				}
			}
			else
			{
				if (aliveCells.Contains(cell))
				{
					aliveCells.TryRemove(cell);
					currentTilemapState.SetTile(cell, deadTile);
				}
			}
		}
		private async Task Simulate(CancellationToken token)
		{
			await Awaitable.WaitForSecondsAsync(UpdateInterval);
			while (!token.IsCancellationRequested)
			{
				UpdateState();
				Population = aliveCells.Count;
				Iterations++;
				Time += UpdateInterval;
				await Awaitable.WaitForSecondsAsync(UpdateInterval);
			}
		}
		private void UpdateState()
		{
			UpdateAliveCells();
			UpdateTilemap();
		}
		private void UpdateAliveCells()
		{
			Parallel.ForEach(aliveCells, cell =>
			{
				for (int x = -1; x <= 1; x++)
				{
					for (int y = -1; y <= 1; y++)
						UpdateCellState(cell + new Vector3Int(x, y));
				}
			});
		}
		private void UpdateCellState(Vector3Int cell)
		{
			int neighbors = CountNeighbors(cell);
			bool alive = IsAlive(cell);
			if (!alive && neighbors == 3)
				AddNewLiveTile(cell);
			else if (alive && (neighbors < 2 || neighbors > 3))
				RemoveTile(cell);
		}
		private int CountNeighbors(Vector3Int cell)
		{
			int count = 0;
			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					Vector3Int neighbor = cell + new Vector3Int(x, y);
					if (x == 0 && y == 0)
						continue;
					else if (IsAlive(neighbor))
						count++;
				}
			}
			return count;
		}
		private bool IsAlive(Vector3Int cell) =>
			aliveCells.Contains(cell);
	}
}