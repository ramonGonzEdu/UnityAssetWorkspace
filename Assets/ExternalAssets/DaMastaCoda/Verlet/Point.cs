namespace DaMastaCoda.Verlet
{
	using System;
	using UnityEngine;

	public class Point : MonoBehaviour
	{
		[NonSerialized] public Vector3 previousPosition;
		public bool locked;

		public float collision = 0.9f;
		public Vector3 gravity = 9.81f * Vector3.down;
		public LayerMask layerMask;

		private void Update()
		{
			// Physics Update
			if (!locked)
			{
				Vector3 vel = (position - previousPosition) * 0.99f;
				// float velSpeed = vel.magnitude;
				// vel = vel.normalized * Mathf.Min(velSpeed, 0.1f);

				position += vel;
				position += gravity * Time.deltaTime * Time.deltaTime;
			}
		}


		Vector3 tempPosition;
		private void LateUpdate()
		{

			previousPosition = tempPosition;
			if (collision > 0f)
			{
				float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z) * 0.5f * collision;
				// check if new position collides with anything, and revert to previous position if it does
				if (Physics.CheckSphere(position, maxScale, layerMask))
				{
					position = previousPosition;
				}
			}
			tempPosition = position;
		}

		public Vector3 position { get => transform.position; set => transform.position = value; }

		private void Start()
		{
			OnEnable();
		}

		private void OnEnable()
		{

			// previousPosition = position;
			// tempPosition = position;
		}

		private void OnDrawGizmosSelected()
		{
			float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z) * 0.5f;
			Gizmos.color = Color.red;
			// Gizmos.DrawSphere(position, maxScale);
			Gizmos.DrawWireSphere(position, maxScale);
		}
	}
}