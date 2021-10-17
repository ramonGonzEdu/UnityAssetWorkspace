namespace DaMastaCoda.RigidBody.Interact.CloningVat
{
	using UnityEngine;
	using System.Collections.Generic;
	using System.Linq;

	public class CloningVat : MonoBehaviour
	{
		[SerializeField] private Transform inputVat;
		[SerializeField] private Transform outputVat;


		bool m_Started = false;

		void Start()
		{
			//Use this to ensure that the Gizmos are being drawn when in Play Mode.
			m_Started = true;
		}

		public void DoClone()
		{
			var colliders = Physics.OverlapBox(inputVat.position, inputVat.lossyScale, inputVat.rotation, ~0, QueryTriggerInteraction.Collide);

			var alreadyCloned = new HashSet<GameObject>();

			foreach (var collider in colliders)
			{
				if (Tags.Tags.GetComponent(collider.gameObject).HasTagAncestry("Special.Clonable"))
				{
					var cloneBase = collider.GetComponent<Tags.Tags>().GetTagAncestry("Special.Clonable");
					if (alreadyCloned.Contains(cloneBase)) continue;


					var oldParent = cloneBase.transform.parent;
					cloneBase.transform.SetParent(inputVat);
					var clone = Instantiate(cloneBase.gameObject, outputVat);
					clone.transform.localPosition = cloneBase.transform.localPosition;
					clone.transform.localRotation = cloneBase.transform.localRotation;
					clone.transform.localScale = cloneBase.transform.localScale;
					cloneBase.transform.SetParent(oldParent);
					clone.transform.SetParent(cloneBase.transform.parent);

					clone.GetComponent<Tags.Tags>().AddTag("Special.Clone");

					alreadyCloned.Add(cloneBase);
				}
			}

		}

		void OnDrawGizmos()
		{
			// Gizmos.color = Color.red;
			// //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
			// if (m_Started)
			// 	//Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
			// 	Gizmos.DrawWireCube(inputVat.position, inputVat.lossyScale);
		}
	}
}