using UnityEngine;
using UnityEngine.Events;

namespace DaMastaCoda.RigidBody.Interact
{
	public class Button : MonoBehaviour
	{

		public Rigidbody rb;

		public UnityEvent onPress;
		public UnityEvent onRelease;
		public float threshold = 0.35f;

		private void Start()
		{
			if (!rb) rb = GetComponent<Rigidbody>();
			lastY = rb.transform.localPosition.y;
		}

		float lastY = 0.0f;

		private void FixedUpdate()
		{
			var newY = rb.transform.localPosition.y;
			if (lastY > threshold && newY < threshold)
				onPress.Invoke();
			if (lastY < threshold && newY > threshold)
				onRelease.Invoke();

			lastY = newY;
		}
	}
}