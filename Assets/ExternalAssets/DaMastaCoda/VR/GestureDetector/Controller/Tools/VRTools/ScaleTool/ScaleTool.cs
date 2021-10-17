
namespace DaMastaCoda.VR.GestureDetector.Controller.Tools.VRTools.ScaleTool
{

	using UnityEngine;
	class ScaleTool : Tool
	{

		GameObject grabbed;
		float initialHandDistanceReciprocal;
		Vector3 initialObjOrigin;
		Vector3 initialScale;
		void Update()
		{
			OVRInput.Update();


			if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, handInput))
			{
				if (grabbed == null)
					TryGrab();
				else
				{
					//Do Scaling
					Vector3 handPos = handObject.transform.position;
					var scaleFactor = (handPos - initialObjOrigin).magnitude * initialHandDistanceReciprocal;
					grabbed.transform.localScale = initialScale * (scaleFactor);
				}
			}
			else
			{
				if (grabbed != null)
					grabbed = null;
			}
		}

		private void TryGrab()
		{
			if (grabbed != null) return;
			Collider[] colliders = Physics.OverlapSphere(handObject.position, 0.04f * rig.localScale.x);
			foreach (var collider in colliders)
			{
				if (Tags.Tags.GetComponent(collider.gameObject).HasTagAncestry("VR.Grabbable"))
				{
					grabbed = collider.gameObject;
					initialObjOrigin = grabbed.transform.position;
					initialHandDistanceReciprocal = 1 / (initialObjOrigin - handObject.transform.position).magnitude;
					initialScale = grabbed.transform.localScale;
					return;
				}
			}
		}
	}
}