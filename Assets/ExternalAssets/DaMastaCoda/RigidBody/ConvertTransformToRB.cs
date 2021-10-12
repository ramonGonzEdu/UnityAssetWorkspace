using UnityEngine;

namespace DaMastaCoda.RigidBody
{
	[RequireComponent(typeof(Rigidbody))]
	public class ConvertTransformToRB : MonoBehaviour
	{
		private Rigidbody rb;

		private void Start()
		{

			rb = GetComponent<Rigidbody>();
			oldRot = transform.rotation;
			oldPos = transform.position;
		}

		Quaternion oldRot;
		Vector3 oldPos;

		private void FixedUpdate()
		{
			var newPos = transform.position;
			var newRot = transform.rotation;

			transform.position = oldPos;
			transform.rotation = oldRot;

			rb.MovePosition(newPos);
			rb.MoveRotation(newRot);

			oldPos = newPos;
			oldRot = newRot;
		}
	}
}