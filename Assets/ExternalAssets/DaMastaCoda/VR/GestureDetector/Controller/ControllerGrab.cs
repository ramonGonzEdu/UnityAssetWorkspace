using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaMastaCoda.VR.GestureDetector.Controller
{


	public class ControllerGrab : MonoBehaviour
	{
		[SerializeField] private Transform rig;
		[SerializeField] private Transform rightHand;
		[SerializeField] private Transform leftHand;
		[SerializeField] private LayerMask layerMask;

		void Start()
		{
		}

		void Update()
		{
			OVRInput.Update();

			if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
			{
				if (leftGrab == null)
					TryGrabLeft();
			}
			else
			{
				if (leftGrab != null)
				{
					Destroy(leftGrab.gameObject.GetComponent<FixedJoint>());
					leftGrab = null;
				}
			}
			if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
			{
				if (rightGrab == null)
					TryGrabRight();
			}
			else
			{
				if (rightGrab != null)
				{
					Destroy(rightGrab.gameObject.GetComponent<FixedJoint>());
					rightGrab = null;
				}
			}




		}

		Collider leftGrab;
		private void TryGrabLeft()
		{
			Collider[] colliders = Physics.OverlapSphere(leftHand.position, 0.04f * rig.localScale.x, layerMask);
			if (colliders.Length > 0)
			{
				leftGrab = colliders[0];
				FixedJoint fj = leftGrab.gameObject.AddComponent<FixedJoint>();
				fj.connectedBody = leftHand.GetComponent<Rigidbody>();
			}
		}

		Collider rightGrab;
		private void TryGrabRight()
		{
			Collider[] colliders = Physics.OverlapSphere(rightHand.position, 0.04f * rig.localScale.x, layerMask);
			if (colliders.Length > 0)
			{
				rightGrab = colliders[0];
				FixedJoint fj = rightGrab.gameObject.AddComponent<FixedJoint>();
				fj.connectedBody = rightHand.GetComponent<Rigidbody>();
			}
		}

	}
}