namespace DaMastaCoda.VR.GestureDetector.Controller.Tools.VRTools.FixedJointTool
{
	using System;
	using UnityEngine;

	public class FixedJointTool : Tool
	{

		void Start()
		{
		}


		private void OnEnable()
		{
			Application.onBeforeRender += OnPreRender;
		}

		private void OnDisable()
		{
			Application.onBeforeRender -= OnPreRender;
		}
		private void OnPreRender()
		{

			if (line != null)
			{
				line.GetComponent<LineRenderer>().SetPosition(1, line.transform.InverseTransformPoint(handObject.position));
			}
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
					CreateLink();

				}
				else if (line != null)
				{
					DestroyLink();
				}
			}
		}

		private void DestroyLink()
		{
			Destroy(line);
		}

		private void CreateLink()
		{
			Collider[] colliders = Physics.OverlapSphere(handObject.position, 0.04f * rig.localScale.x);
			foreach (var collider in colliders)
			{
				if (Tags.Tags.GetComponent(collider.gameObject).HasTagAncestry("VR.Grabbable"))
				{
					var newsel = collider.gameObject;
					if (newsel.gameObject.GetComponent<Rigidbody>() == null)
						newsel = newsel.GetComponentInParent<Rigidbody>().gameObject;

					if (grabbed == newsel)
						return;


					var fj = grabbed.AddComponent<FixedJoint>();
					fj.connectedBody = newsel.GetComponent<Rigidbody>();

					line.GetComponent<LineRenderer>().SetPosition(1, line.transform.InverseTransformPoint(newsel.transform.position));
					line = null;
					grabbed = null;

					return;
				}
			}

		}

		GameObject line;
		GameObject grabbed;
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

					line = new GameObject();
					var linecomp = line.AddComponent<LineRenderer>();
					line.transform.SetParent(grabbed.transform);
					line.transform.localPosition = Vector3.zero;
					line.transform.localRotation = Quaternion.identity;
					var fjdef = line.AddComponent<FixedJointDef>();

					linecomp.material = activeMaterial;
					linecomp.widthCurve.MoveKey(0, new Keyframe(0, 0.025f));
					linecomp.SetPosition(0, Vector3.zero);
					linecomp.SetPosition(1, line.transform.InverseTransformPoint(handObject.position));
					linecomp.useWorldSpace = false;
					// fj = grabbed.AddComponent<FixedJoint>();
					// fj.connectedBody = handObject.GetComponent<Rigidbody>();

					// Tags.Tags.GetComponent(grabbed).AddTag("VR.Grabbable.Grabbed");
					return;
				}
			}
		}

	}
}