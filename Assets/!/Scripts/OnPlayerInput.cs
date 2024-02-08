using PlayerControllingNS;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class OnPlayerInput : MonoBehaviour
{
	[SerializeField]
	protected InputManager inputManager;
	[SerializeField]
	protected Camera camera;

	protected Vector3 MousePosition =>
		camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

	protected abstract void OnEnable();
	protected abstract void OnDisable();
}
