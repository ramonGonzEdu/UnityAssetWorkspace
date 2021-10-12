using System.Collections.Generic;
using UnityEngine;

namespace DaMastaCoda.GenericAI
{
	public class WayPointAI : GenericAI
	{
		public Queue<Vector3> waypointList = new Queue<Vector3>();
		public Vector3 current;

		public float locationThreshold = 1f;
		float angleThresholdWalk = 0.8f;
		float angleThreshold = 0.99f;

		private void Start()
		{
			current = playerBody.position;
			current.y += 0.25f;
			// input.SetAxis("Vertical", 1);
			// input.SetKey(KeyCode.LeftShift, true);
		}


		public bool skipQueues = true;
		public LayerMask skipQueuesLayerMask;
		private void Update()
		{

			input.SetKey(KeyCode.Space, false);

			if (NextQueueViable())
			{
				current = waypointList.Dequeue();
			}

			Vector3 left = current - orientation.position;

			if (left.sqrMagnitude > locationThreshold * locationThreshold)
			{
				if (Physics.Raycast(playerBody.position + new Vector3(0, playerBody.localScale.y * 0.5f, 0), left.normalized, left.magnitude, skipQueuesLayerMask))
				{
					input.SetKey(KeyCode.Space, true);
				}

				float angleSim = Vector3.Dot(left.normalized, orientation.forward);
				if (angleSim < angleThreshold)
				{
					float angleSimNorm = angleSim * 0.5f + 0.5f;
					{
						var turnLeft = Quaternion.Euler(0, -1, 0) * orientation.forward;
						var turnRight = Quaternion.Euler(0, 1, 0) * orientation.forward;
						var leftDot = Vector3.Dot(left.normalized, turnLeft) * 0.5f + 0.5f;
						var rightDot = Vector3.Dot(left.normalized, turnRight) * 0.5f + 0.5f;

						if (leftDot < rightDot)
						{
							input.SetAxis("Mouse X", 0.5f);
							// input.SetAxis("Mouse X", (1 - angleSimNorm) * 0.5f);
						}
						else if (leftDot > rightDot)
						{
							input.SetAxis("Mouse X", -0.5f);
							// input.SetAxis("Mouse X", (1 - angleSimNorm) * -0.5f);
						}
					}

					{

						var turnUp = Quaternion.AngleAxis(-1, orientation.right) * orientation.forward;
						var turnDown = Quaternion.AngleAxis(1, orientation.right) * orientation.forward;
						var upDot = Vector3.Dot(left.normalized, turnUp) * 0.5f + 0.5f;
						var downDot = Vector3.Dot(left.normalized, turnDown) * 0.5f + 0.5f;

						if (upDot < downDot)
						{
							// input.SetAxis("Mouse Y", (1 - angleSimNorm) * -0.1f); 
							input.SetAxis("Mouse Y", -0.3f);
						}
						else if (upDot > downDot)
						{
							// input.SetAxis("Mouse Y", (1 - angleSimNorm) * 0.1f); 
							input.SetAxis("Mouse Y", 0.3f);
						}
					}
				}
				else
				{
					input.SetAxis("Mouse X", 0f);
					input.SetAxis("Mouse Y", 0f);
				}



				input.SetAxis("Vertical", angleSim > angleThresholdWalk ? 1 : 0);
				input.SetKey(KeyCode.LeftControl, left.sqrMagnitude > 4 * locationThreshold * locationThreshold);

				// input.SetAxis("Mouse X", angleSim > angleThresholdWalk ? 1 : 0);
				// input.SetAxis("Mouse Y", angleSim > angleThresholdWalk ? 1 : 0);
			}
			else
			{
				if (waypointList.Count > 0)
				{
					current = waypointList.Dequeue();
				}

				Gander();
				Wander();
			}
		}


		float wanderCooldown = 0f;
		bool wander = true;
		private void Wander()
		{

			if (wanderCooldown <= 0.0f)
			{
				if (wander)
				{
					input.SetAxis("Vertical", (float)Random.Range(0, 1));

				}
				else
				{
					input.SetAxis("Vertical", 0f);
					input.SetAxis("Horizontal", 0f);
				}

				wanderCooldown = wander ?
					Random.Range(0.0f, 1.0f) :
					Random.Range(1.0f, 1.5f);

				wander = !wander;
			}

			wanderCooldown -= Time.deltaTime;
		}


		float ganderCooldown = 0f;
		bool gander = true;
		private void Gander()
		{
			if (ganderCooldown <= 0.0f)
			{
				if (gander)
				{
					input.SetAxis("Mouse X", Random.Range(-0.3f, 0.3f));

					float xorient = (orientation.localRotation.eulerAngles.x);
					if (xorient > 180) xorient -= 360;

					if (xorient > 10)
						input.SetAxis("Mouse Y", Random.Range(-0.01f, 0.1f));
					else if (xorient < -10)
						input.SetAxis("Mouse Y", Random.Range(0.01f, -0.1f));
					else
						input.SetAxis("Mouse Y", Random.Range(-0.1f, 0.1f));
				}
				else
				{
					input.SetAxis("Mouse X", 0f);
					input.SetAxis("Mouse Y", 0f);
				}

				ganderCooldown = gander ?
					Random.Range(0.0f, 1.0f) :
					Random.Range(1.0f, 1.5f);

				gander = !gander;
			}

			ganderCooldown -= Time.deltaTime;
		}

		private bool NextQueueViable()
		{
			if (!skipQueues) return false;
			if (waypointList.Count <= 0)
				return false;

			foreach (var point in waypointList)
			{
				var left = (point - playerBody.position);

				Debug.DrawLine(orientation.position, orientation.position + left, Color.green);
				Debug.DrawLine(playerBody.position, playerBody.position + left, Color.green);

				bool check = !(Physics.Raycast(orientation.position, left.normalized, left.magnitude, skipQueuesLayerMask)
				|| Physics.Raycast(playerBody.position, left.normalized, left.magnitude, skipQueuesLayerMask));
				if (check) return true;
			}
			return false;
		}
	}
}