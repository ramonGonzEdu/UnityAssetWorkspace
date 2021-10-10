using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaMastaCoda.VR.GestureDetector.Controller
{


	public class ControllerJoystickWalk : MonoBehaviour
	{
		[SerializeField] private Transform rig;
		[SerializeField] private Transform orientation;
		[SerializeField] private OVRInput.Controller hand;

		Vector3 clearY(Vector3 position)
		{
			return new Vector3(position.x, 0, position.z).normalized;
		}

		private void Update()
		{
			Vector2 joystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, hand);
			Vector3 movement = clearY(orientation.forward) * joystick.y + clearY(orientation.right) * joystick.x;
			rig.position += movement * Time.deltaTime * rig.localScale.x;
		}
	}
}