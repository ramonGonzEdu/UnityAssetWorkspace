using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaMastaCoda.VR.GestureDetector.Hands
{


	public class GestureDetector : MonoBehaviour
	{
		[SerializeField] private Transform rig;
		[SerializeField] private Transform rightFinger1;
		[SerializeField] private Transform rightFinger2;
		[SerializeField] private Transform leftFinger1;
		[SerializeField] private Transform leftFinger2;
		[SerializeField] private SkinnedMeshRenderer rightMesh;
		[SerializeField] private SkinnedMeshRenderer leftMesh;
		[SerializeField] private Material touch;
		[SerializeField] private Material notTouch;
		[SerializeField] private LayerMask layerMask;


		void Start()
		{
		}

		bool right = false;
		bool left = false;
		bool pRight;
		bool pLeft;

		Vector3 offset;
		float sizeOffset;
		Vector3 rigOffset;
		float rigSizeOffset;
		void Update()
		{
			{
				var d = rightFinger1.position - rightFinger2.position;
				right = d.magnitude < (right ? 0.08f : 0.02f) * rig.localScale.x;
				if (right)
					rightMesh.material = touch;
				else
					rightMesh.material = notTouch;
			}

			{
				var d = leftFinger1.position - leftFinger2.position;
				left = d.magnitude < (left ? 0.08f : 0.02f) * rig.localScale.x;
				if (left)
					leftMesh.material = touch;
				else
					leftMesh.material = notTouch;
			}

			if (left && right && leftGrab == null && rightGrab == null)
			{
				CalculateCameraControls();
			}
			else
			{
				if (left)
				{
					if (leftGrab == null)
						TryGrabLeft();
				}
				else
				{
					if (leftGrab != null)
					{
						Destroy(leftGrab.gameObject.GetComponent<FixedJoint>());
						leftGrab = null;
					}
				}
				if (right)
				{
					if (rightGrab == null)
						TryGrabRight();
				}
				else
				{
					if (rightGrab != null)
					{
						Destroy(rightGrab.gameObject.GetComponent<FixedJoint>());
						rightGrab = null;
					}
				}
			}




			pRight = right;
			pLeft = left;
		}

		Collider leftGrab;
		private void TryGrabLeft()
		{
			Collider[] colliders = Physics.OverlapSphere(leftFinger1.position, 0.04f * rig.localScale.x, layerMask);
			if (colliders.Length > 0)
			{
				leftGrab = colliders[0];
				FixedJoint fj = leftGrab.gameObject.AddComponent<FixedJoint>();
				fj.connectedBody = leftFinger1.GetComponent<Rigidbody>();
			}
		}

		Collider rightGrab;
		private void TryGrabRight()
		{
			Collider[] colliders = Physics.OverlapSphere(rightFinger1.position, 0.04f * rig.localScale.x, layerMask);
			if (colliders.Length > 0)
			{
				rightGrab = colliders[0];
				FixedJoint fj = rightGrab.gameObject.AddComponent<FixedJoint>();
				fj.connectedBody = rightFinger1.GetComponent<Rigidbody>();
			}
		}

		private void CalculateCameraControls()
		{
			if (!pLeft || !pRight)
			{
				//Start Movement Code
				offset = (rightFinger1.position + leftFinger1.position) * 0.5f;
				sizeOffset = (rightFinger1.position - leftFinger1.position).magnitude;
				rigOffset = rig.position;
				rigSizeOffset = rig.localScale.x;

			}

			var currentOffset = (rightFinger1.position + leftFinger1.position) * 0.5f;
			rig.position = rigOffset - currentOffset + offset;
			rigOffset = rig.position;

			var currentSizeOffset = (rightFinger1.position - leftFinger1.position).magnitude;
			var scale = sizeOffset / currentSizeOffset * rigSizeOffset;
			//rig.localScale = new Vector3(scale, scale, scale);
			rigSizeOffset = scale;
		}
	}
}