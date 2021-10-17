using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace DaMastaCoda.VR.GestureDetector.Controller.Tools
{

	public class Tool : MonoBehaviour
	{

		public Transform rig;
		public Transform handObject;
		public OVRInput.Controller handInput;
		public Material activeMaterial;
		public UnityEvent initialize;
		// Disable tool after holding PrimaryHandTrigger for 3s

		public void SetHand(OVRInput.Controller hand)
		{
			handInput = hand;
		}

	}
}
