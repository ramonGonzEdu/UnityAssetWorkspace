namespace DaMastaCoda.RigidBody
{
	using UnityEngine;

	public class DoubleJoint : MonoBehaviour
	{
		public Rigidbody joint1;
		Vector3 anchor1;
		Vector3 upanchor1;
		public Rigidbody joint2;
		Vector3 anchor2;
		Rigidbody rb;

		private void Start()
		{
			rb = GetComponent<Rigidbody>();
			if (joint1 && joint2)
				RecalculateAnchors();
		}


		private void OnEnable()
		{
			if (!joint1 || !joint2) return;
			var j1C = joint1.GetComponents<Collider>();
			var j2C = joint2.GetComponents<Collider>();
			var C = GetComponents<Collider>();

			foreach (var c in C)
			{
				foreach (var j1 in j1C)
				{
					Physics.IgnoreCollision(c, j1);
				}
				foreach (var j2 in j2C)
				{
					Physics.IgnoreCollision(c, j2);
				}
			}

		}

		private void OnDisable()
		{
			if (!joint1 || !joint2) return;
			var j1C = joint1.GetComponents<Collider>();
			var j2C = joint2.GetComponents<Collider>();
			var C = GetComponents<Collider>();

			foreach (var c in C)
			{
				foreach (var j1 in j1C)
				{
					Physics.IgnoreCollision(c, j1, false);
				}
				foreach (var j2 in j2C)
				{
					Physics.IgnoreCollision(c, j2, false);
				}
			}
		}

		public void RecalculateAnchors()
		{
			anchor1 = transform.InverseTransformPoint(joint1.transform.position);
			anchor2 = transform.InverseTransformPoint(joint2.transform.position);

			upanchor1 = transform.InverseTransformDirection(Vector3.Cross((-joint1.transform.right).normalized, (joint2.transform.position - joint1.transform.position).normalized));
		}

		private void FixedUpdate()
		{
			if (!joint1 || !joint2) return;

			{
				// var local1 = transform.InverseTransformPoint(joint1.transform.position);

				var delta1 = joint1.transform.position - transform.TransformPoint(anchor1);
				// rb.MovePosition(rb.position + delta1);
				rb.position += delta1;
			}

			{
				var localup = Vector3.Cross((-joint1.transform.right).normalized, (joint2.transform.position - joint1.transform.position).normalized);
				rb.rotation = Quaternion.FromToRotation(transform.TransformDirection(upanchor1).normalized, localup.normalized) * rb.rotation;

			}

			{

				var local2 = transform.InverseTransformPoint(joint2.transform.position);

				// get anchor2 and local2 relative to local1
				var anchor2l = transform.TransformPoint(anchor2) - transform.TransformPoint(anchor1); //Target
				var local2l = transform.TransformPoint(local2) - transform.TransformPoint(anchor1); //Current

				// rb.MoveRotation(Quaternion.FromToRotation(anchor2l.normalized, local2l.normalized) * rb.rotation);
				rb.rotation = Quaternion.FromToRotation(anchor2l.normalized, local2l.normalized) * rb.rotation;
			}

			// {
			// 	var delta1 = joint1.transform.position - transform.TransformPoint(anchor1);
			// 	// rb.MovePosition(rb.position + delta1);
			// 	rb.position += delta1;
			// }

		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawWireCube(transform.TransformPoint(anchor1), Vector3.one);
			Gizmos.DrawWireCube(transform.TransformPoint(anchor2), Vector3.one);
			Gizmos.DrawWireCube(transform.TransformPoint(anchor1 + upanchor1.normalized), Vector3.one);
		}
	}
}