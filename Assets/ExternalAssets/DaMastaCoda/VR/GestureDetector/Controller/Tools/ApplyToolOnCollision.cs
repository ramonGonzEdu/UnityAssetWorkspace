using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DaMastaCoda.VR.GestureDetector.Controller.Tools
{
	[RequireComponent(typeof(Collider))]
	public class ApplyToolOnCollision : MonoBehaviour
	{
		[SerializeField] private GameObject toolPrefab;
		[SerializeField] private bool rbTool = false;
		[SerializeField] private bool destroyOnCollision = false;

		private void OnCollisionEnter(Collision collision)
		{
			OnTriggerEnter(collision.collider);
		}

		private void OnTriggerEnter(Collider collision)
		{
			if (collision.gameObject.tag == "Controller")
			{
				if (toolPrefab != null)
				{
					var success = collision.transform.parent.GetComponentInChildren<ToolManager>().EnableTool(toolPrefab, rbTool);
					if (destroyOnCollision && success)
						Destroy(gameObject);
				}

			}
		}
	}
}