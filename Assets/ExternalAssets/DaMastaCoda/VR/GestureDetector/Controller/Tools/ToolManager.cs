using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DaMastaCoda.VR.GestureDetector.Controller.Tools
{

	public class ToolManager : MonoBehaviour
	{

		[SerializeField] private Transform rig;
		[SerializeField] private Transform handObject;
		[SerializeField] private OVRInput.Controller handInput;
		[SerializeField] private Transform rbHand;
		[SerializeField] private GameObject[] disabledObjects;
		[SerializeField] private Renderer graphics;
		[SerializeField] private Material freeMaterial;
		[SerializeField] private Material toolMaterial;
		// Disable tool after holding PrimaryHandTrigger for 3s

		bool hasTool { get => (bool)tool; }
		GameObject tool = null;
		public bool EnableTool(GameObject p_tool, bool useRigidBody = false)
		{
			if (tool) return false;


			var toolInstance = Instantiate(p_tool, useRigidBody ? rbHand : transform);
			toolInstance.transform.localScale = p_tool.transform.localScale;
			if (useRigidBody)
				toolInstance.transform.localScale /= rbHand.transform.localScale.x;
			// toolInstance.transform.localPosition = p_tool.transform.localPosition;
			// toolInstance.transform.localRotation = p_tool.transform.localRotation;

			startScale = toolInstance.transform.localScale;
			tool = toolInstance;


			foreach (var behavior in disabledObjects)
			{
				behavior.SetActive(false);
			}
			tool.SetActive(true);
			toolInstance.GetComponent<Tool>().handInput = handInput;
			toolInstance.GetComponent<Tool>().rig = rig;
			toolInstance.GetComponent<Tool>().handObject = handObject;
			graphics.material = toolInstance.GetComponent<Tool>().activeMaterial ?? toolMaterial;
			toolInstance.GetComponent<Tool>().initialize.Invoke();
			return true;
		}
		public bool DisableTool()
		{
			if (tool != null)
			{
				graphics.material = freeMaterial;

				tool.SetActive(false);
				Destroy(tool);
				tool = null;
				foreach (var behavior in disabledObjects)
				{
					behavior.SetActive(true);
				}
				return true;
			}
			return false;
		}

		float timeHeld = 0;
		Vector3 startScale;
		private void Update()
		{
			if (tool)
			{
				OVRInput.Update();
				if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, handInput) && (!OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) || !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger)))
				{
					if (timeHeld == 0) startScale = tool.transform.localScale;

					timeHeld += Time.deltaTime;
					tool.transform.localScale = (2f - timeHeld) * 0.5f * startScale;
					if (timeHeld > 2f)
					{
						if (!DisableTool())
							Destroy(transform.parent.gameObject);
						timeHeld = 0;
					}
				}
				else
				{
					tool.transform.localScale = startScale;
					timeHeld = 0;
				}
			}
		}
	}
}
