using UnityEngine;

namespace DaMastaCoda.GenericAI
{
	[RequireComponent(typeof(WayPointAI))]
	public class FollowObject : MonoBehaviour
	{
		public Transform target;
		WayPointAI ai;
		public bool useQueue = true;

		private void Start()
		{
			ai = GetComponent<WayPointAI>();
		}

		float time = 0f;

		private void Update()
		{
			if (useQueue)
			{
				time += Time.deltaTime;

				if (time > 1)
				{
					ai.waypointList.Enqueue(target.position);

					time = 0;
				}
			}
			else ai.current = target.position;
		}
	}
}