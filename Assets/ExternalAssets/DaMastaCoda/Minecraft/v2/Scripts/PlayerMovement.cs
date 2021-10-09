using DaMastaCoda.GenericAI;
using UnityEngine;

namespace DaMastaCoda.Minecraft.v2
{
	public class PlayerMovement : MonoBehaviour, DaMastaCoda.GenericAI.InputHaving
	{
		[Header("References")]
		[SerializeField] private Transform orientation;
		[SerializeField] private Transform playerBody;


		[Header("Movement")]
		[SerializeField] private float moveSpeed = 6f;
		[SerializeField] private float movementMultiplier = 10f;
		[SerializeField] private float airMultiplier = 0.4f;
		[SerializeField] private float runMultiplier = 1.5f;
		[SerializeField] private float crouchMultiplier = 0.5f;

		[Header("Jumping")]
		[SerializeField] private float jumpForce = 5f;

		[Header("Keybinds")]
		[SerializeField] private KeyCode jumpKey = KeyCode.Space;

		[Header("Drag")]
		[SerializeField] private float groundDrag = 6f;
		[SerializeField] private float airDrag = 2f;

		[Header("Ground Detection")]
		[SerializeField] private LayerMask groundMask;
		bool isGrounded;
		float groundDistance = 0.1f;

		float horizontalMovement;
		float verticalMovement;
		Vector3 moveDirection;
		public Vector3 MoveDirection { get => moveDirection; }
		Vector3 slopeMoveDirection;
		Rigidbody rb;

		RaycastHit slopeHit;

		private bool OnSlope()
		{
			if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out slopeHit, 1.5f))
			{
				if (slopeHit.normal != Vector3.up)
					return true;
				else return false;
			}
			return false;
		}

		private void Start()
		{
			rb = GetComponent<Rigidbody>();
			rb.freezeRotation = true;
		}

		private void Update()
		{
			// isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);
			isGrounded = Physics.CheckBox(transform.position + new Vector3(0, 0.5f, 0), new Vector3(0.25f, 0.6f, 0.125f), playerBody.rotation, groundMask);

			ProcessInput();
			ControlDrag();

			if (Input.GetKeyDown(jumpKey) && isGrounded)
			{
				Jump();
			}

			slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
		}

		private void Jump()
		{
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}

		private void ProcessInput()
		{
			horizontalMovement = Input.GetAxisRaw("Horizontal");
			verticalMovement = Input.GetAxisRaw("Vertical");
			moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
		}

		void ControlDrag()
		{
			if (isGrounded)
				rb.drag = groundDrag;
			else
				rb.drag = airDrag;
		}

		private void FixedUpdate()
		{
			MovePlayer();
		}

		private void MovePlayer()
		{

			float moveMult = 1.0f;
			if (Input.GetKey(KeyCode.LeftShift)) moveMult *= crouchMultiplier;
			if (Input.GetKey(KeyCode.LeftControl)) moveMult *= runMultiplier;

			rb.AddForce(
				(OnSlope() ? slopeMoveDirection : moveDirection).normalized *
					moveSpeed *
					movementMultiplier *
					(isGrounded ? 1f : airMultiplier) *
					moveMult,
				ForceMode.Acceleration
			);
		}

		public DaMastaCoda.GenericAI.IInput Input = DaMastaCoda.GenericAI.PlayerInput.Singleton;
		void InputHaving.SetInput(IInput p_Input)
		{
			Input = p_Input;
		}

		IInput InputHaving.GetInput()
		{
			return Input;
		}
	}
}