using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DaMastaCoda.VR.GestureDetector.Controller.Tools
{

	public class Tool : MonoBehaviour
	{
		[SerializeField] private OVRInput.Controller handInput;
		// Disable tool after holding PrimaryHandTrigger for 3s

		public void SetHand(OVRInput.Controller hand)
		{
			handInput = hand;
		}

	}
}
