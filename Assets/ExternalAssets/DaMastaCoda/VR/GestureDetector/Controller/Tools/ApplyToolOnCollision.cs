using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DaMastaCoda.VR.GestureDetector.Controller.Tools
{
	[RequireComponent(typeof(Collider))]
	public class ApplyToolOnCollision : MonoBehaviour
	{
		[SerializeField] private GameObject toolPrefab;
		[SerializeField] private bool destroyOnCollision;

		private void OnCollisionEnter(Collision collision)
		{
			OnTriggerEnter(collision.collider);
		}

		private void OnTriggerEnter(Collider collision)
		{
			print(collision.gameObject.tag);
			if (collision.gameObject.tag == "Controller")
			{
				if (toolPrefab != null)
				{
					collision.gameObject.GetComponentInChildren<ToolManager>().EnableTool(toolPrefab);
				}

				if (destroyOnCollision)
					Destroy(gameObject);
			}
		}
	}
}