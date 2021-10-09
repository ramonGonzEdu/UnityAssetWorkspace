using DaMastaCoda.GenericAI;
using UnityEngine;

namespace DaMastaCoda.Minecraft.v2
{
	public class PlayerLook : MonoBehaviour, DaMastaCoda.GenericAI.InputHaving
	{
		[SerializeField] private float sensX = 250f;
		[SerializeField] private float sensY = 250f;

		[SerializeField] private Transform cam;
		[SerializeField] private Transform orientation;

		float mouseX;
		float mouseY;

		float multiplier = 0.01f;

		float xRotation;
		float yRotation;

		public float XRotation { get => xRotation; }
		public float YRotation { get => yRotation; }
		public Transform Cam { get => cam; }




		private void Start()
		{

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		private void LateUpdate()
		{
			HandleInput();

			cam.transform.parent.rotation = Quaternion.Euler(xRotation, yRotation, 0);
			// cam.transform.parent.localRotation = Quaternion.Euler(xRotation, 0, 0);
			orientation.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
		}

		private void HandleInput()
		{
			mouseX = Input.GetAxisRaw("Mouse X");
			mouseY = Input.GetAxisRaw("Mouse Y");

			yRotation += mouseX * sensX * multiplier;
			xRotation -= mouseY * sensY * multiplier;

			xRotation = Mathf.Clamp(xRotation, -80f, 80f);
		}

		public DaMastaCoda.GenericAI.IInput Input = DaMastaCoda.GenericAI.PlayerInput.Singleton;
		void InputHaving.SetInput(IInput p_Input)
		{
			Input = p_Input;
		}

		IInput InputHaving.GetInput()
		{
			return Input;
		}
	}
}