using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DaMastaCoda.RigidBody;
using UnityEngine;

namespace DaMastaCoda.VR.GestureDetector.Controller
{


	public class ControllerDoubleGrab : MonoBehaviour
	{

		[SerializeField] private Transform rig;
		[SerializeField] private Transform handObject1;
		[SerializeField] private Transform handObject2;
		[SerializeField] private OVRInput.Controller handInput1;
		[SerializeField] private OVRInput.Controller handInput2;
		[SerializeField] private ControllerGrab hand1GrabMB;
		[SerializeField] private ControllerGrab hand2GrabMB;

		private void Update()
		{
			OVRInput.Update();
			if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, handInput1) && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, handInput2))
			{
				TryGrab();
			}
			else
			{
				if (TryReleaseGrab())
				{
					hand1GrabMB.enabled = true;
					hand2GrabMB.enabled = true;
				}
			}
		}

		private bool TryReleaseGrab()
		{
			if (grabbedDJ != null)
			{
				Destroy(grabbedDJ);
				grabbed = null;
				return true;
			}

			return false;
		}

		GameObject grabbed;
		DoubleJoint grabbedDJ;

		private bool TryGrab()
		{
			if (grabbed != null) return false;

			var obj1 = hand1GrabMB.GetGrabbedObject();
			var obj2 = hand2GrabMB.GetGrabbedObject();

			if (obj1 == null || obj1 != obj2) return false;

			grabbed = obj1;

			hand1GrabMB.enabled = false;
			hand2GrabMB.enabled = false;

			grabbedDJ = grabbed.AddComponent<DoubleJoint>();
			grabbedDJ.joint1 = handObject1.GetComponent<Rigidbody>();
			grabbedDJ.joint2 = handObject2.GetComponent<Rigidbody>();
			grabbedDJ.RecalculateAnchors();


			Tags.Tags.GetComponent(grabbed).AddTag("VR.Grabbable.Grabbed");
			return true;

		}
	}
}