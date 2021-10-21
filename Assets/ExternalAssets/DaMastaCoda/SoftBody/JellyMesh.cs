namespace DaMastaCoda.SoftBody
{
	using UnityEngine;

	public class JellyMesh : MonoBehaviour
	{
		public float intensity = 1f;
		public float mass = 1f;
		public float stiffness = 1f;
		public float damping = 0.75f;
		private Mesh originalMesh, meshClone;
		private MeshRenderer thisRenderer;
		private JellyVertex[] jv;
		private Vector3[] vertexArray;

		private void Start()
		{
			originalMesh = GetComponent<MeshFilter>().sharedMesh;
			meshClone = Instantiate(originalMesh);
			GetComponent<MeshFilter>().sharedMesh = meshClone;
			thisRenderer = GetComponent<MeshRenderer>();
			jv = new JellyVertex[originalMesh.vertices.Length];
			for (int i = 0; i < jv.Length; i++)
			{
				jv[i] = new JellyVertex(i, transform.TransformPoint(originalMesh.vertices[i]));

			}
		}

		private void FixedUpdate()
		{
			vertexArray = originalMesh.vertices;
			for (int i = 0; i < jv.Length; i++)
			{
				Vector3 target = transform.TransformPoint(vertexArray[jv[i].id]);
				float l_intensity = (1 - (thisRenderer.bounds.max.y - target.y) / thisRenderer.bounds.size.y) * intensity;
				jv[i].Shake(target, mass, stiffness, damping);
				target = transform.InverseTransformPoint(jv[i].position);
				vertexArray[jv[i].id] = Vector3.Lerp(vertexArray[jv[i].id], target, l_intensity);
			}
			meshClone.vertices = vertexArray;
		}


	}

	class JellyVertex
	{
		public int id;
		public Vector3 position;
		public Vector3 velocity, force;
		public JellyVertex(int _id, Vector3 _position)
		{
			id = _id;
			position = _position;
		}

		public void Shake(Vector3 target, float m, float s, float d)
		{
			force = (target - position) * s;
			velocity = (velocity + force / m) * d;
			position += velocity;
			if ((velocity + force + force / m).magnitude < 0.001f)
				position = target;
		}
	}
}