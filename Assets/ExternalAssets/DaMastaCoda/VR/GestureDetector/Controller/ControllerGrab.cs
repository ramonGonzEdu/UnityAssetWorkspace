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
					TryGrab();
			}
			else
			{
				if (grabbed != null)
				{
					Destroy(grabbed.GetComponent<FixedJoint>());
					grabbed = null;
				}
			}
		}

		GameObject grabbed;
		private void TryGrab()
		{
			Collider[] colliders = Physics.OverlapSphere(handObject.position, 0.04f * rig.localScale.x, layerMask);
			if (colliders.Length > 0)
			{
				grabbed = colliders[0].gameObject;
				if (grabbed.gameObject.GetComponent<Rigidbody>() == null)
					grabbed = grabbed.GetComponentInParent<Rigidbody>().gameObject;


				FixedJoint fj = grabbed.AddComponent<FixedJoint>();
				fj.connectedBody = handObject.GetComponent<Rigidbody>();
			}
		}

	}
}