using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DaMastaCoda.VR.GestureDetector.Controller.Tools
{

	public class ToolManager : MonoBehaviour
	{
		[SerializeField] private OVRInput.Controller handInput;
		[SerializeField] private MonoBehaviour[] disableBehaviors;
		// Disable tool after holding PrimaryHandTrigger for 3s

		GameObject tool;
		public void EnableTool(GameObject tool)
		{
			if (tool != null) DisableTool();

			var toolInstance = Instantiate(tool, transform.position, transform.rotation);
			toolInstance.GetComponent<Tool>().SetHand(handInput);
			toolInstance.transform.SetParent(transform);
			tool = toolInstance;

			foreach (var behavior in disableBehaviors)
			{
				behavior.enabled = false;
			}
			tool.SetActive(true);
		}
		public void DisableTool()
		{
			if (tool != null)
			{
				tool.SetActive(false);
				Destroy(tool);
				tool = null;
			}
			foreach (var behavior in disableBehaviors)
			{
				behavior.enabled = true;
			}
		}

		float timeHeld = 0;
		private void Update()
		{
			OVRInput.Update();
			if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, handInput) && tool != null)
			{
				Debug.DrawRay(transform.position, transform.forward * 10, Color.green);
				timeHeld += Time.deltaTime;
				if (timeHeld > 3)
				{
					DisableTool();
					timeHeld = 0;
				}
			}
			else
			{
				Debug.DrawRay(transform.position, transform.forward * 10, Color.red);
				timeHeld = 0;
			}
		}
	}
}
