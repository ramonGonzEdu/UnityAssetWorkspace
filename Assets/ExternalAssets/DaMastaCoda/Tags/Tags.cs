using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

namespace DaMastaCoda.Tags
{
	[System.Serializable]
	public struct Tag
	{
		public string name;

		public Tag(string p_name) : this()
		{
			this.name = p_name;
		}

		public override bool Equals(object obj)
		{
			return obj is Tag tag &&
						 name == tag.name;
		}

		public static Tag[] getTags(string name)
		{
			var tagParts = name.Split('.');
			var tagAmount = tagParts.Length;
			var outVar = new Tag[tagAmount];
			outVar[0] = new Tag(tagParts[0]);
			for (int i = 1; i < tagAmount; i++)
			{
				outVar[i] = new Tag(outVar[i - 1].name + "." + tagParts[i]);
			}

			return outVar;
		}

		public Tag[] getTags()
		{
			return Tag.getTags(name);
		}
		public override int GetHashCode()
		{
			return 363513814 + EqualityComparer<string>.Default.GetHashCode(name);
		}
	}

	public class Tags : MonoBehaviour
	{
		// True -> Tag is present and all child tags are present
		// False -> Tag is present but one or more child tags are missing
		// Null -> Tag is missing
		Dictionary<Tag, int> tags = new Dictionary<Tag, int>();
		static Dictionary<Tag, Dictionary<GameObject, int>> objectTags = null;
		[SerializeField] private string[] inspectorTags;
		public float amount;

		private void Start()
		{
			if (objectTags == null)
				objectTags = new Dictionary<Tag, Dictionary<GameObject, int>>();

			OnValidate();
		}

		public static Tags GetComponent(GameObject obj)
		{
			if (obj.GetComponent<Tags>() == null) obj.AddComponent<Tags>();
			return obj.GetComponent<Tags>();
		}

		private void OnValidate()
		{
			if (objectTags != null)
				foreach (var tag in tags)
				{
					if (objectTags.ContainsKey(tag.Key))
						objectTags[tag.Key].Remove(gameObject);
				}
			tags.Clear();
			foreach (var item in inspectorTags)
			{
				var childKeys = Tag.getTags(item);
				foreach (var childKey in childKeys)
				{
					tags[childKey] = (tags.ContainsKey(childKey) ? tags[childKey] : 0) + 1;
				}
			}
			amount = tags.Count;
			if (objectTags != null)
				foreach (var tag in tags)
				{
					if (!objectTags.ContainsKey(tag.Key)) objectTags[tag.Key] = new Dictionary<GameObject, int>();
					objectTags[tag.Key][gameObject] = tag.Value;
				}
		}

		public bool HasTag(string tag)
		{
			return tags.ContainsKey(new Tag(tag));
		}

		public bool HasTagAncestry(string tag)
		{
			if (HasTag(tag))
				return true;
			var eligibleParents = gameObject.GetComponentsInParent<Tags>();
			foreach (var parent in eligibleParents)
			{
				if (parent.HasTag(tag))
					return true;
			}
			return false;
		}
		public GameObject GetTagAncestry(string tag)
		{
			if (HasTag(tag))
				return gameObject;
			var eligibleParents = gameObject.GetComponentsInParent<Tags>();
			foreach (var parent in eligibleParents)
			{
				if (parent.HasTag(tag))
					return parent.gameObject;
			}
			return null;
		}

		public void AddTag(string tagname)
		{
			// return tags.ContainsKey(new Tag(tag));
			var childKeys = Tag.getTags(tagname);
			foreach (var childKey in childKeys)
			{
				tags[childKey] = (tags.ContainsKey(childKey) ? tags[childKey] : 0) + 1;
				if (objectTags != null)
				{
					if (!objectTags.ContainsKey(childKey)) objectTags[childKey] = new Dictionary<GameObject, int>();
					objectTags[childKey][gameObject] = tags[childKey];
				}
			}
		}

		public void RemoveTag(string tagname)
		{
			// return tags.ContainsKey(new Tag(tag));
			if (!tags.ContainsKey(new Tag(tagname))) return;
			var childKeys = Tag.getTags(tagname);
			foreach (var childKey in childKeys)
			{
				tags[childKey] = tags[childKey] - 1;
				if (tags[childKey] <= 0)
				{
					tags.Remove(childKey);
					if (objectTags != null)
						objectTags[childKey].Remove(gameObject);
				}
				else
				{
					if (objectTags != null)
						objectTags[childKey][gameObject] = tags[childKey];
				}
			}
		}

		public static GameObject[] FindTaggedObjects(Tag tag)
		{
			// var outlist = GameObject.FindObjectsOfType<Tags>();
			// var outVar = new List<GameObject>();
			// foreach (var item in outlist)
			// {
			// 	if (item.tags.ContainsKey(tag))
			// 	{
			// 		outVar.Add(item.gameObject);
			// 	}
			// }

			// return outVar.ToArray();



			if (objectTags != null && objectTags.ContainsKey(tag))
				return objectTags[tag].Keys.ToArray();
			return new GameObject[0];
		}

	}
}