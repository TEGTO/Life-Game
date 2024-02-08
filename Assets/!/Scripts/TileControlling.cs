using UnityEngine;
using UnityEngine.InputSystem;

namespace GameControlling
{
	public class TileControlling : OnPlayerInput
	{
		[SerializeField]
		private GameBoard gameBoard;

		private bool isLMBPressing = false;
		private bool isRMBPressing = false;

		protected override void OnEnable()
		{
			inputManager.OnLMBClick += OnLMBClick;
			inputManager.OnRMBClick += OnRMBClick;
		}
		protected override void OnDisable()
		{
			inputManager.OnLMBClick -= OnLMBClick;
			inputManager.OnRMBClick -= OnRMBClick;
		}
		public void DisableInteraction()
		{
			isLMBPressing = false;
		}
		private void OnLMBClick(InputAction.CallbackContext ctx)
		{
			isLMBPressing = ctx.performed;
		}
		private void OnRMBClick(InputAction.CallbackContext ctx)
		{
			isRMBPressing = ctx.performed;
		}
		private void LateUpdate()
		{
			if (isLMBPressing)
				AddNewTileByMousePosition();
			if (isRMBPressing)
				RemoveTileByMousePosition();
		}
		private void AddNewTileByMousePosition()
		{
			Vector3Int tilePosition = GetMouseTilePosition();
			gameBoard.AddNewLiveTile(tilePosition);
			gameBoard.UpdateTilemap();
		}
		private void RemoveTileByMousePosition()
		{
			Vector3Int tilePosition = GetMouseTilePosition();
			gameBoard.RemoveTile(tilePosition);
			gameBoard.UpdateTilemap();
		}
		private Vector3Int GetMouseTilePosition()
		{
			Vector3 mousePosition = MousePosition;
			Vector3Int tilePosition = gameBoard.CurrentTilemapState.WorldToCell(mousePosition);
			return tilePosition;
		}
	}
}