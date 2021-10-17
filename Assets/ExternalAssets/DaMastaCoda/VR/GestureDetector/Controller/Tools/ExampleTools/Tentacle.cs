using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace DaMastaCoda.VR.GestureDetector.Controller.Tools.ExampleTools
{

	public class Tentacle : Tool
	{

		public Joint topLevelRB;

		public void SetRigidBody()
		{
			topLevelRB.connectedBody = transform.parent.GetComponent<Rigidbody>();

		}

	}
}
