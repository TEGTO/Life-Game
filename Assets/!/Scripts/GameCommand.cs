using PlayerControllingNS;
using UnityEngine;

namespace GameControlling
{
	public class GameCommand : MonoBehaviour
	{
		private const int SEED_LENGTH = 15;

		[SerializeField]
		private CameraControlling cameraControlling;
		[SerializeField]
		private GameBoard board;
		[SerializeField]
		private LifePattern intialPattern;
		[SerializeField]
		private TileControlling tileControlling;

		private float[] gameSpeed = new float[] { 0.1f, 0.25f, 0.5f, 1f, 1.5f, 2f, 4f, 8f, 16f };
		private int index = 3;

		public float GameSpeed { get => board.UpdateIntervalCoeff; }
		public string SeedName { get; private set; }
		public int CurrentPopulation { get => board.Population; }

		private void Awake()
		{
			GenerateNewSeed();
		}
		public void CameraReset()
		{
			cameraControlling.CameraReset();
		}
		public void RestartGame()
		{
			board.Clear();
			board.SetPattern(intialPattern);
		}
		public void PlayGame()
		{
			board.StartSimulate();
		}
		public void PauseGame()
		{
			board.StopSimulate();
		}
		public void GameSpeedUp()
		{
			index = index + 1 >= gameSpeed.Length ? index : index + 1;
			board.UpdateIntervalCoeff = gameSpeed[index];
		}
		public void GameSpeedDown()
		{
			index = index - 1 < 0 ? 0 : index - 1;
			board.UpdateIntervalCoeff = gameSpeed[index];
		}
		public void CameraZoomUp()
		{
			cameraControlling.CameraZoomUp();
		}
		public void CameraZoomDown()
		{
			cameraControlling.CameraZoomDown();
		}
		public void SetCanUseUIButtonEventStatus(bool canUse)
		{
			InputManager.CanCallUIButtonOnClickEvent = canUse;
			tileControlling.DisableInteraction();
		}
		public void EnterNewSeed(string text)
		{
			GenerateNewPatternAndResetGame(text);
		}
		public void GenerateNewSeed()
		{
			string randomSeed = RandomLifePatternMaker.GenerateRandomSeed(SEED_LENGTH);
			GenerateNewPatternAndResetGame(randomSeed);
		}
		public void ChangeCellSpawnAmount(float min, float max)
		{
			RandomLifePatternMaker.MinMaxAmount.x = min;
			RandomLifePatternMaker.MinMaxAmount.y = max;
		}

		public void ChangeCellSpawnBoundaries(float min, float max)
		{
			RandomLifePatternMaker.Boundaries.x = min;
			RandomLifePatternMaker.Boundaries.y = max;
		}
		private void GenerateNewPatternAndResetGame(string seed)
		{
			if (seed != SeedName)
			{
				SeedName = seed;
				intialPattern = new LifePattern(RandomLifePatternMaker.GenerateHashSetFromSeed(seed).GetAwaiter().GetResult());
			}
			RestartGame();
		}
	}
}