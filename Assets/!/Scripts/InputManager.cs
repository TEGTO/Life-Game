using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerControllingNS
{
	public class InputManager : MonoBehaviour
	{
		public static bool CanCallUIButtonOnClickEvent = true;
		private static PlayerInput playerInput;

		private PlayerInput.MainActions mainActions;

		public delegate void InputBehaviour(InputAction.CallbackContext context);
		public event InputBehaviour OnLMBClick;
		public event InputBehaviour OnRMBClick;
		public event InputBehaviour OnMMBAction;
		public event InputBehaviour OnZoom;

		private void OnEnable()
		{
			if (playerInput == null)
				playerInput = new PlayerInput();
			mainActions = playerInput.Main;

			mainActions.LMB.performed += PerformLMBClick;
			mainActions.LMB.canceled += PerformLMBClick;

			mainActions.RMB.performed += PerformRMBClick;
			mainActions.RMB.canceled += PerformRMBClick;

			mainActions.MMB.performed += MMBEvent;
			mainActions.MMB.canceled += MMBEvent;

			mainActions.Zoom.performed += PerformZoom;
			playerInput.Enable();
		}
		private void OnDisable()
		{
			mainActions.LMB.performed -= PerformLMBClick;
			mainActions.LMB.canceled -= PerformLMBClick;

			mainActions.RMB.performed -= PerformRMBClick;
			mainActions.RMB.canceled -= PerformRMBClick;

			mainActions.MMB.performed -= MMBEvent;
			mainActions.MMB.canceled -= MMBEvent;

			mainActions.Zoom.performed += PerformZoom;
			playerInput.Disable();
		}
		private void PerformLMBClick(InputAction.CallbackContext context)
		{
			if (CanCallUIButtonOnClickEvent)
				OnLMBClick?.Invoke(context);
		}
		private void PerformRMBClick(InputAction.CallbackContext context) =>
			OnRMBClick?.Invoke(context);
		private void MMBEvent(InputAction.CallbackContext context) =>
			OnMMBAction?.Invoke(context);
		private void PerformZoom(InputAction.CallbackContext context) =>
			OnZoom?.Invoke(context);
	}
}
