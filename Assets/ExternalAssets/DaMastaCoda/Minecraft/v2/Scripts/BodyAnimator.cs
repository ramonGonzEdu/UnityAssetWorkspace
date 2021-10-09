using DaMastaCoda.GenericAI;
using UnityEngine;

namespace DaMastaCoda.Minecraft.v2
{
	public class BodyAnimator : MonoBehaviour, InputHaving
	{
		[SerializeField] PlayerLook playerLook;
		[SerializeField] PlayerMovement playerMovement;
		[SerializeField] Transform orientation;
		[SerializeField] Animator animator;

		float turnLimit = 40;

		IInput Input = PlayerInput.Singleton;

		IInput InputHaving.GetInput()
		{
			return Input;
		}

		void InputHaving.SetInput(IInput p_Input)
		{
			Input = p_Input;
		}

		private void Update()
		{
			float headRot = (playerLook.YRotation + 360) % 360;
			float bodyRot = (transform.eulerAngles.y - headRot + 360) % 360;
			if (bodyRot > 180)
				bodyRot -= 360;

			animator.SetFloat("Crouch", Input.GetKey(KeyCode.LeftShift) ? 1 : 0, 0.2f, Time.deltaTime);

			float cMult = Input.GetKey(KeyCode.LeftShift) ? 0.5f : 1;

			animator.SetFloat("Forward", 0, 0.2f, Time.deltaTime);

			Vector3 movement = orientation.InverseTransformVector(playerMovement.MoveDirection);
			if (movement.z > 0.5)
			{
				// bodyRot *= (1 - 0.6f * Time.deltaTime);
				bodyRot = Mathf.Lerp(bodyRot, 0, 10 * Time.deltaTime);
				animator.SetFloat("Forward", Input.GetKey(KeyCode.LeftControl) ? cMult : (cMult * 0.5f), 0.2f, Time.deltaTime);
			}
			else if (movement.z < -0.5)
			{
				bodyRot += Mathf.Sign(bodyRot) * 60 * Time.deltaTime;
				animator.SetFloat("Forward", -0.5f * cMult, 0.2f, Time.deltaTime);
			}

			if (movement.x > 0.5)
			{
				bodyRot += 200 * Time.deltaTime;
				animator.SetFloat("Forward", 0.5f * cMult, 0.2f, Time.deltaTime);
			}
			else if (movement.x < -0.5)
			{
				bodyRot -= 200 * Time.deltaTime;
				animator.SetFloat("Forward", 0.5f * cMult, 0.2f, Time.deltaTime);
			}


			float diff = bodyRot;
			float newDif = ((Mathf.Abs(diff) > turnLimit) ? Mathf.Clamp(diff, -turnLimit, turnLimit) : diff) + headRot;
			transform.rotation = Quaternion.Euler(0, newDif, 0);
			playerLook.Cam.transform.parent.rotation = Quaternion.Euler(playerLook.XRotation, playerLook.YRotation, 0);

		}
	}
}