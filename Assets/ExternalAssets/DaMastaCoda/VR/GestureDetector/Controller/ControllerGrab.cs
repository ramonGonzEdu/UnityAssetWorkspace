using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaMastaCoda.VR.GestureDetector.Controller
{


	public class ControllerGrab : MonoBehaviour
	{
		[SerializeField] private Transform rig;
		[SerializeField] private Transform handObject;
		[SerializeField] private OVRInput.Controller handInput;
		[SerializeField] private LayerMask layerMask;

		void Start()
		{
		}

		void Update()
		{
			OVRInput.Update();

			if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, handInput))
			{
				if (grabbed == null)
					TryGrabLeft();
			}
			else
			{
				if (grabbed != null)
				{
					Destroy(grabbed.gameObject.GetComponent<FixedJoint>());
					grabbed = null;
				}
			}
		}

		Collider grabbed;
		private void TryGrabLeft()
		{
			Collider[] colliders = Physics.OverlapSphere(handObject.position, 0.04f * rig.localScale.x, layerMask);
			if (colliders.Length > 0)
			{
				grabbed = colliders[0];
				FixedJoint fj = grabbed.gameObject.AddComponent<FixedJoint>();
				fj.connectedBody = handObject.GetComponent<Rigidbody>();
			}
		}

	}
}