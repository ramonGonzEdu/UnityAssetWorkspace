namespace DaMastaCoda.Verlet
{
	using System;
	using UnityEngine;

	[RequireComponent(typeof(Point))]
	public class Stick : MonoBehaviour
	{

		public bool primary = true;
		public Point otherPoint;
		[NonSerialized] public Point selfPoint;
		[SerializeField] private float length;
		public bool autoCalculateLength = true;

		private void Start()
		{
			selfPoint = GetComponent<Point>();
			if (autoCalculateLength)
				length = Vector3.Distance(selfPoint.position, otherPoint.position);
			autoCalculateLength = false;
		}

		Stick linkedStick;

		void OnValidate()
		{


			selfPoint = GetComponent<Point>();


			if (otherPoint != null && autoCalculateLength)
				length = (
					otherPoint.transform.position
					- selfPoint.transform.position
					).magnitude;


			if (otherPoint != null && linkedStick == null)
			{
				Array.ForEach(otherPoint.GetComponents<Stick>(), (s) =>
				{
					if (s.otherPoint == selfPoint)
						linkedStick = s;
				}
				);

				if (linkedStick == null || linkedStick.gameObject != otherPoint.gameObject || linkedStick.otherPoint != selfPoint)
				{
					linkedStick = otherPoint.gameObject.AddComponent<Stick>();
					linkedStick.primary = false;
					linkedStick.otherPoint = selfPoint;

				}
			}
			if (linkedStick != null)
			{
				if (linkedStick.length != length) linkedStick.length = length;
				if (linkedStick.autoCalculateLength != autoCalculateLength) linkedStick.autoCalculateLength = autoCalculateLength;
				if (linkedStick.primary == primary) linkedStick.primary = !primary;
			}
		}



		public float Length { get => length; set => length = autoCalculateLength ? length : value; }

		// private void OnEnable()
		// {
		// 	if (otherPoint != null && autoCalculateLength) Length = (otherPoint.transform.position - selfPoint.transform.position).magnitude;

		// 	if (!linkedStick.enabled)
		// 		linkedStick.enabled = true;
		// }

		// private void OnDisable()
		// {
		// 	if (linkedStick.enabled)
		// 		linkedStick.enabled = false;
		// }

		private void OnDrawGizmosSelected()
		{
			// if (transform.hasChanged && otherPoint != null && autoCalculateLength) length = (otherPoint.transform.position - selfPoint.transform.position).magnitude;
			// OnValidate();

			if (otherPoint != null)
			{
				Gizmos.color = Color.red;
				Vector3 facing = (otherPoint.transform.position - selfPoint.transform.position).normalized * length;
				Gizmos.DrawLine(selfPoint.transform.position, selfPoint.transform.position + facing);
			}
		}
	}
}