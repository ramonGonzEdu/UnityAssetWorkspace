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
					Destroy(fj);
					Tags.Tags.GetComponent(grabbed).RemoveTag("VR.Grabbable.Grabbed");
					grabbed = null;
					fj = null;
				}
			}
		}


		private void OnDisable()
		{
			if (grabbed != null)
			{
				Destroy(fj);
			}
		}

		private void OnEnable()
		{
			if (grabbed != null)
			{
				fj = grabbed.AddComponent<FixedJoint>();
				fj.connectedBody = handObject.GetComponent<Rigidbody>();
			}
		}

		GameObject grabbed;


		public GameObject GetGrabbedObject()
		{
			return grabbed;
		}

		FixedJoint fj;
		private void TryGrab()
		{
			if (grabbed != null) return;
			Collider[] colliders = Physics.OverlapSphere(handObject.position, 0.04f * rig.localScale.x);
			foreach (var collider in colliders)
			{
				if (Tags.Tags.GetComponent(collider.gameObject).HasTagAncestry("VR.Grabbable"))
				{
					grabbed = collider.gameObject;
					if (grabbed.gameObject.GetComponent<Rigidbody>() == null)
						grabbed = grabbed.GetComponentInParent<Rigidbody>().gameObject;


					fj = grabbed.AddComponent<FixedJoint>();
					fj.connectedBody = handObject.GetComponent<Rigidbody>();

					Tags.Tags.GetComponent(grabbed).AddTag("VR.Grabbable.Grabbed");
					return;
				}
			}
		}

	}
}