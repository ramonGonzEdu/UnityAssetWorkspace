using UnityEngine;

namespace DaMastaCoda.Minecraft.v2
{
	public class MoveCamera : MonoBehaviour
	{
		[SerializeField] Transform cameraPosition;
		[SerializeField] Transform orientation;

		private void Update()
		{
			transform.position = cameraPosition.position;
			transform.rotation = cameraPosition.rotation;
		}
	}
}