using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaMastaCoda.VR.GestureDetector.Controller
{


	public class ControllerWorld : MonoBehaviour
	{
		[SerializeField] private Transform rig;
		[SerializeField] private Transform rightHand;
		[SerializeField] private Transform leftHand;

		void Start()
		{
		}



		void Update()
		{
			OVRInput.Update();


			if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
			{
				CalculateCameraControls();
			}

			pLeft = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
			pRight = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
		}

		private void CalculateCameraControls()
		{
			if (!InitializeCameraControls())
			{
				CalculateCameraTranslation();
				CalculateCameraScale();
				CalculateCameraRotation();
			}

		}

		float sizeOffset;
		float rigSizeOffset;
		private void CalculateCameraScale()
		{
			var currentSizeOffset = (rightHand.position - leftHand.position).magnitude;
			var scale = sizeOffset / currentSizeOffset * rigSizeOffset;
			rig.localScale = new Vector3(scale, scale, scale);
			rigSizeOffset = scale;
		}



		Vector3 offset;
		private void CalculateCameraTranslation()
		{
			var currentOffset = GetControllerCenter();
			rig.position += -currentOffset + offset;
		}


		float initialAngle;
		private void CalculateCameraRotation()
		{
			var angle = GetAngleOfHands();
			var angleOffset = angle - initialAngle;
			rig.RotateAround(GetControllerCenter(), Vector3.up, -angleOffset);
		}

		private float GetAngleOfHands()
		{
			Vector3 rightH = new Vector3(rightHand.position.x, 0, rightHand.position.z);
			Vector3 leftH = new Vector3(leftHand.position.x, 0, leftHand.position.z);
			return Mathf.Atan2(rightH.x - leftH.x, rightH.z - leftH.z) * Mathf.Rad2Deg;
		}


		private Vector3 GetControllerCenter()
		{
			return (rightHand.position + leftHand.position) * 0.5f;
		}


		bool pLeft = false;
		bool pRight = false;
		private bool InitializeCameraControls()
		{
			if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && (!pLeft || !pRight))
			{
				//Start Movement Code
				offset = GetControllerCenter();

				sizeOffset = (rightHand.position - leftHand.position).magnitude;
				rigSizeOffset = rig.localScale.x;

				initialAngle = GetAngleOfHands();
				return true;
			}
			return false;
		}
	}
}