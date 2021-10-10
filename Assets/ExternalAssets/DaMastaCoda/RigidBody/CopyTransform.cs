using UnityEngine;

namespace DaMastaCoda.RigidBody
{
	[RequireComponent(typeof(Rigidbody))]
	public class CopyTransform : MonoBehaviour
	{
		[SerializeField] private Transform target;
		[SerializeField] public bool copyPosition;
		[SerializeField] public bool copyRotation;
		[SerializeField] public bool copyScale;

		private Rigidbody rb;

		private void Start()
		{

			rb = GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			if (copyPosition)
			{
				rb.MovePosition(target.position);
			}
			if (copyRotation)
			{
				rb.MoveRotation(target.rotation);
			}
			if (copyScale)
			{
				rb.transform.localScale = target.localScale;
			}
		}
	}
}