using UnityEngine;

namespace DaMastaCoda.GenericAI
{
	public class GenericAI : MonoBehaviour
	{
		public Transform orientation;
		public Transform playerBody;
		[SerializeField] public MonoBehaviour[] behaviors;
		public ControllableInput input = new ControllableInput();

		protected void OnEnable()
		{
			foreach (var behavior in behaviors)
			{
				if (behavior is InputHaving)
				{
					InputHaving b = behavior as InputHaving;
					if (b != null) b.SetInput(input);
				}
			}

		}
	}
}