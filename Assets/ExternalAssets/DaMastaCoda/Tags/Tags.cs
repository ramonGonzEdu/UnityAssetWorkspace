using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Events;

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

	public static class TagsExtensions
	{
		public static Tags GetTags(this GameObject obj)
		{
			return Tags.GetComponent(obj);
		}
	}

	public class Tags : MonoBehaviour
	{
		// True -> Tag is present and all child tags are present
		// False -> Tag is present but one or more child tags are missing
		// Null -> Tag is missing
		Dictionary<Tag, int> tags = new Dictionary<Tag, int>();


		[SerializeField] SerializableDictionary<string, string> tagsData = new SerializableDictionary<string, string>();
		static Dictionary<Tag, Dictionary<GameObject, int>> objectTags = new Dictionary<Tag, Dictionary<GameObject, int>>();
		[SerializeField] private string[] inspectorTags = new string[0];
		public float amount;


		static public bool g_existed = false;
		[NonSerialized] public bool existed = false;
		private void Start()
		{
			if (!g_existed)
			{
				g_existed = true;
				objectTags = new Dictionary<Tag, Dictionary<GameObject, int>>();
			}
			if (!existed)
			{
				existed = true;
				OnValidate();
				afterValidation.Invoke();
			}
		}

		public void Clone(Tags oldTags)
		{
			existed = true;
			inspectorTags = oldTags.inspectorTags;
			tagsData = new SerializableDictionary<string, string>(oldTags.tagsData);
			OnValidate();
			// tags = new Dictionary<Tag, int>(oldTags.tags);

			// if (objectTags != null)
			// 	foreach (var tag in tags)
			// 	{
			// 		if (!objectTags.ContainsKey(tag.Key)) objectTags[tag.Key] = new Dictionary<GameObject, int>();
			// 		objectTags[tag.Key][gameObject] = tag.Value;
			// 	}
		}

		public static Tags GetComponent(GameObject obj)
		{
			if (obj.GetComponent<Tags>() == null) obj.AddComponent<Tags>();
			return obj.GetComponent<Tags>();
		}

		public UnityEvent afterValidation = new UnityEvent();

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
		public string GetTagData(string tag)
		{
			return tagsData.TryGetValue(tag, out string value) ? value : "";
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
