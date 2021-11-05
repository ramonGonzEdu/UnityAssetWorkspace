namespace DaMastaCoda.Verlet
{
	using System;
	using UnityEngine;

	public class StickCalculator : MonoBehaviour
	{

		public int iterations = 4;

		private void Start()
		{
			enabled = false;
			enabled = true;
		}

		private void Update()
		{
			var sticks = UnityEngine.Object.FindObjectsOfType<Stick>();

			for (int i = 0; i < iterations; i++)
			{

				foreach (var stick in sticks)
				{
					if (stick.primary)
					{
						Vector3 stickCenter = (stick.selfPoint.position + stick.otherPoint.position) * 0.5f;
						Vector3 stickDir = (stick.selfPoint.position - stick.otherPoint.position).normalized;
						if (!stick.selfPoint.locked)
							stick.selfPoint.position = stickCenter + stickDir * stick.Length * 0.5f;
						if (!stick.otherPoint.locked)
							stick.otherPoint.position = stickCenter - stickDir * stick.Length * 0.5f;
					}
				}
			}
		}

	}
}