using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerControllingNS
{
	public class CameraControlling : OnPlayerInput
	{
		private class CameraState
		{
			public float CameraZoom;
			public Vector3 CameraPosition;

			public CameraState(float cameraZoom, Vector3 cameraPosition)
			{
				CameraZoom = cameraZoom;
				CameraPosition = cameraPosition;
			}
		}

		[SerializeField, Min(0)]
		private float zoomSensitivity = 1f;
		[SerializeField, Min(0)]
		private float zoomToCursorStep = 10f;
		[SerializeField, Min(0)]
		private float zoomButtonsControlAmount = 2f;

		private CameraState cameraInitialState;
		private Vector3 origin;
		private Vector3 difference;
		private bool isDragging;


		protected override void OnEnable()
		{
			inputManager.OnMMBAction += OnDrag;
			inputManager.OnZoom += ZoomInput;
		}
		protected override void OnDisable()
		{
			inputManager.OnMMBAction -= OnDrag;
			inputManager.OnZoom -= ZoomInput;
		}
		private void Start()
		{
			cameraInitialState = SaveCurrentCameraState();
		}
		public void CameraReset()
		{
			camera.orthographicSize = cameraInitialState.CameraZoom;
			camera.transform.position = cameraInitialState.CameraPosition;
			origin = cameraInitialState.CameraPosition;
		}
		public void CameraZoomUp()
		{
			camera.orthographicSize /= zoomButtonsControlAmount;
			CheckMinOrthographicSize();
		}
		public void CameraZoomDown()
		{
			camera.orthographicSize *= zoomButtonsControlAmount;
		}
		private void OnDrag(InputAction.CallbackContext ctx)
		{
			if (ctx.performed)
				origin = MousePosition;
			isDragging = ctx.performed;
		}
		private void LateUpdate()
		{
			if (!isDragging) return;
			difference = MousePosition - transform.position;
			transform.position = origin - difference;
		}
		private void ZoomInput(InputAction.CallbackContext context)
		{
			float zoomDirection = Mathf.Sign(context.ReadValue<float>());
			if (zoomDirection > 0)
			{
				camera.orthographicSize /= zoomSensitivity;
				camera.transform.position = Vector3.MoveTowards(camera.transform.position, MousePosition, zoomToCursorStep);
			}
			else
				camera.orthographicSize *= zoomSensitivity;
			CheckMinOrthographicSize();
		}
		private void CheckMinOrthographicSize() =>
			camera.orthographicSize = camera.orthographicSize <= 0 ? 0.00001f : camera.orthographicSize;
		private CameraState SaveCurrentCameraState() =>
			new CameraState(camera.orthographicSize, camera.transform.position);
	}
}