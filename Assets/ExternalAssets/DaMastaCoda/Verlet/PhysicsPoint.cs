namespace DaMastaCoda.Verlet
{
	using System;
	using UnityEngine;

	[RequireComponent(typeof(Rigidbody))]
	public class PhysicsPoint : Point
	{
		Rigidbody rb;

		public void Awake()
		{
			rb = GetComponent<Rigidbody>();
		}

		public new Vector3 position { get => rb.position; set => rb.position = value; }

	}
}