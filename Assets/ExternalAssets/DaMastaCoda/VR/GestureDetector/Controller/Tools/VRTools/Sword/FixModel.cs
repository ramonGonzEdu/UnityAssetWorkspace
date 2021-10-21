using UnityEngine;

public class FixModel : MonoBehaviour
{
	private void Start()
	{
		var mesh = GetComponent<MeshFilter>().mesh;
		mesh.subMeshCount = GetComponent<Renderer>().materials.Length;
	}
}