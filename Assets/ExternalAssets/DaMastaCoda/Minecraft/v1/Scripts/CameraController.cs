using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private float mouseSensitivity;
	[SerializeField] private Transform playerBody;
	[SerializeField] private Transform playerModel;
	[SerializeField] private Transform playerHead;

	[SerializeField] private float headRotationLimit;

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		Rotate();
	}

	private void Rotate()
	{

		float moveZ = Input.GetAxis("Vertical");

		if (moveZ < 0)
		{
			float dxm = playerModel.localRotation.eulerAngles.y;
			if (dxm > 180)
				dxm = dxm - 360;
			dxm = dxm * .01f;
			playerModel.Rotate(Vector3.up, dxm);
		}
		if (moveZ > 0)
		{
			float dxm = playerModel.localRotation.eulerAngles.y;
			if (dxm > 180)
				dxm = dxm - 360;
			dxm = dxm * -.008f;
			playerModel.Rotate(Vector3.up, dxm);
		}



		float hrot = playerHead.localRotation.eulerAngles.x;
		if (hrot > 180) hrot -= 360;

		if (hrot > 60 || hrot < -60)
		{
			playerHead.Rotate(Vector3.right, (60 - Math.Abs(hrot)) * Math.Sign(hrot));
		}

		float xRotation = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float yRotation = -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


		playerBody.Rotate(Vector3.up, xRotation);
		playerModel.Rotate(Vector3.up, -xRotation);
		playerHead.Rotate(Vector3.right, yRotation);

		float m = playerModel.localRotation.eulerAngles.y;
		if (m > 180) m -= 360;

		if (Mathf.Abs(m) > headRotationLimit)
		{
			float y = m;
			float ys = Mathf.Sign(y);
			float dy = (headRotationLimit - y * ys) * ys;
			playerModel.Rotate(Vector3.up, dy);

		}

		playerHead.Rotate(Vector3.forward, -playerHead.localRotation.eulerAngles.z);
		playerHead.Rotate(Vector3.up, -playerModel.localRotation.eulerAngles.y - playerHead.localRotation.eulerAngles.y);
	}
}