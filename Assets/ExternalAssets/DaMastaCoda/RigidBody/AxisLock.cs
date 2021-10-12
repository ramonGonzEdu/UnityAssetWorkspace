using UnityEngine;

namespace DaMastaCoda.RigidBody
{
	[RequireComponent(typeof(Rigidbody))]
	public class AxisLock : MonoBehaviour
	{
		public bool lockX;
		public bool lockY;
		public bool lockZ;
		private Rigidbody rb;

		// float initialX;
		// float initialY;
		// float initialZ;

		private void Start()
		{
			rb = GetComponent<Rigidbody>();
			// Vector3 localPosition = transform.InverseTransformDirection(rb.position);
			// initialX = rb.position.x;
			// initialY = rb.position.y;
			// initialZ = rb.position.z;
		}


		private void FixedUpdate()
		{
			Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
			if (lockX) localVelocity.x = 0;
			if (lockY) localVelocity.y = 0;
			if (lockZ) localVelocity.z = 0;
			rb.velocity = transform.TransformDirection(localVelocity);
		}
	}
}