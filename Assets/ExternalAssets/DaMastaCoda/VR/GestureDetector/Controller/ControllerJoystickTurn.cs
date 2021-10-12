using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaMastaCoda.VR.GestureDetector.Controller
{


	public class ControllerJoystickTurn : MonoBehaviour
	{
		[SerializeField] private Transform rig;
		[SerializeField] private Transform rotateAround;
		[SerializeField] private OVRInput.Controller hand;


		private void Update()
		{
			Vector2 joystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, hand);
			float turning = joystick.x;

			rig.RotateAround(rotateAround.position, Vector3.up, turning * Time.deltaTime * 80f);
		}
	}
}