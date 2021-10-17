namespace DaMastaCoda.Tags
{
	using UnityEngine;

	public class TagInspector : MonoBehaviour
	{
		public string viewTag;
		public GameObject[] tagItems;

		private void OnValidate()
		{
			tagItems = Tags.FindTaggedObjects(new Tag(viewTag));
		}
	}
}