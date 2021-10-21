namespace DaMastaCoda.VR.GestureDetector.Controller.Tools.VRTools.SwordTool
{
	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine.Events;

	class CutData
	{
		public Vector3 _triggerEnterTipPosition;
		public Vector3 _triggerEnterBasePosition;
		public Vector3 _triggerExitTipPosition;


	}

	public class SwordTool : Tool
	{
		[SerializeField] private GameObject tipPos;
		[SerializeField] private GameObject basePos;
		public float forceAppliedToCut;

		public Material bladeNormal;
		public Material bladeZaWarudo;


		public int materialPos;
		Renderer rnd;
		bool existed = false;

		private void Start()
		{
			rnd = GetComponentInChildren<Renderer>();

		}

		void Update()
		{
			OVRInput.Update();


			if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, handInput))
			{
				Time.timeScale = 0.2f;
				Time.fixedDeltaTime = Time.timeScale * 0.02f;
				rnd.materials[materialPos] = bladeZaWarudo;
			}
			else
			{
				Time.timeScale = 1.0f;
				Time.fixedDeltaTime = Time.timeScale * 0.02f;
				rnd.materials[materialPos] = bladeNormal;
			}
		}


		private Dictionary<GameObject, CutData> objectsBeingCut = new Dictionary<GameObject, CutData>();

		private void OnTriggerEnter(Collider other)
		{
			if (Tags.Tags.GetComponent(other.gameObject).HasTag("Physics.Cuttable"))
			{
				CutData data = new CutData();
				data._triggerEnterTipPosition = tipPos.transform.position;
				data._triggerEnterBasePosition = basePos.transform.position;
				objectsBeingCut.Add(other.gameObject, data);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (objectsBeingCut.ContainsKey(other.gameObject))
			{
				objectsBeingCut[other.gameObject]._triggerExitTipPosition = tipPos.transform.position;

				CutData data = objectsBeingCut[other.gameObject];

				Vector3 side1 = data._triggerExitTipPosition - data._triggerEnterTipPosition;
				Vector3 side2 = data._triggerExitTipPosition - data._triggerEnterBasePosition;

				Vector3 normal = Vector3.Cross(side1, side2).normalized;

				Vector3 transformedNormal = other.gameObject.transform.InverseTransformVector(normal);

				Vector3 transformedStartingPoint = other.gameObject.transform.InverseTransformPoint(data._triggerEnterTipPosition);

				Plane plane = new Plane(transformedNormal, transformedStartingPoint);

				if (transformedNormal.x < 0 || transformedNormal.y < 0 || transformedNormal.z < 0)
				{
					plane.Flip();
				}

				GameObject[] slices = Slicer.Slice(plane, other.gameObject);

				objectsBeingCut.Remove(other.gameObject);
				Destroy(other.gameObject);
				// {
				// 	Rigidbody rb = slices[0].GetComponent<Rigidbody>();
				// 	Vector3 rawNormal = transformedNormal + Vector3.down * forceAppliedToCut;
				// 	rb.AddForce(rawNormal, ForceMode.Impulse);
				// }
				// {

				// 	Rigidbody rb = slices[1].GetComponent<Rigidbody>();
				// 	Vector3 rawNormal = transformedNormal + Vector3.up * forceAppliedToCut;
				// 	rb.AddForce(rawNormal, ForceMode.Impulse);
				// }
			}
		}


	}

	class Slicer
	{
		/// <summary>
		/// Slice the object by the plane 
		/// </summary>
		/// <param name="plane"></param>
		/// <param name="objectToCut"></param>
		/// <returns></returns>
		public static GameObject[] Slice(Plane plane, GameObject objectToCut)
		{
			//Get the current mesh and its verts and tris
			Mesh mesh = objectToCut.GetComponent<MeshFilter>().mesh;
			var a = mesh.GetSubMesh(0);


			//Create left and right slice of hollow object
			// SlicesMetadata slicesMeta = new SlicesMetadata(plane, mesh, true, false, false, Tags.Tags.GetComponent(objectToCut).HasTag("Physics.Cuttable.Smooth"));
			SlicesMetadata slicesMeta = new SlicesMetadata(objectToCut, plane, mesh, true, false, false, false);

			GameObject positiveObject = CreateMeshGameObject(objectToCut);
			positiveObject.name = string.Format("{0}_positive", objectToCut.name);

			GameObject negativeObject = CreateMeshGameObject(objectToCut);
			negativeObject.name = string.Format("{0}_negative", objectToCut.name);

			var positiveSideMeshData = slicesMeta.PositiveSideMesh;
			var negativeSideMeshData = slicesMeta.NegativeSideMesh;

			positiveObject.GetComponent<MeshFilter>().mesh = positiveSideMeshData;
			negativeObject.GetComponent<MeshFilter>().mesh = negativeSideMeshData;

			SetupCollidersAndRigidBodys(ref positiveObject, positiveSideMeshData, (objectToCut.GetComponentInParent<Rigidbody>()?.useGravity ?? false));
			SetupCollidersAndRigidBodys(ref negativeObject, negativeSideMeshData, (objectToCut.GetComponentInParent<Rigidbody>()?.useGravity ?? false));

			//copy over velocity and angularVelocity
			positiveObject.GetComponent<Rigidbody>().velocity = objectToCut.GetComponent<Rigidbody>().velocity;
			positiveObject.GetComponent<Rigidbody>().angularVelocity = objectToCut.GetComponent<Rigidbody>().angularVelocity;
			negativeObject.GetComponent<Rigidbody>().velocity = objectToCut.GetComponent<Rigidbody>().velocity;
			negativeObject.GetComponent<Rigidbody>().angularVelocity = objectToCut.GetComponent<Rigidbody>().angularVelocity;

			return new GameObject[] { positiveObject, negativeObject };
		}

		/// <summary>
		/// Creates the default mesh game object.
		/// </summary>
		/// <param name="originalObject">The original object.</param>
		/// <returns></returns>
		private static GameObject CreateMeshGameObject(GameObject originalObject)
		{
			var originalMaterial = originalObject.GetComponent<MeshRenderer>().materials;

			GameObject meshGameObject = new GameObject();

			meshGameObject.AddComponent<MeshFilter>();
			meshGameObject.AddComponent<MeshRenderer>();

			{
				var tags = meshGameObject.AddComponent<Tags.Tags>();
				tags.Clone(Tags.Tags.GetComponent(originalObject));
			}

			meshGameObject.GetComponent<MeshRenderer>().materials = originalMaterial;

			meshGameObject.transform.SetParent(originalObject.transform.parent);
			meshGameObject.transform.localScale = originalObject.transform.localScale;
			meshGameObject.transform.rotation = originalObject.transform.rotation;
			meshGameObject.transform.position = originalObject.transform.position;

			meshGameObject.tag = originalObject.tag;

			return meshGameObject;
		}

		/// <summary>
		/// Add mesh collider and rigid body to game object
		/// </summary>
		/// <param name="gameObject"></param>
		/// <param name="mesh"></param>
		private static void SetupCollidersAndRigidBodys(ref GameObject gameObject, Mesh mesh, bool useGravity)
		{
			MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = mesh;
			meshCollider.convex = true;

			var rb = gameObject.AddComponent<Rigidbody>();
			rb.useGravity = useGravity;
		}
	}

	/// <summary>
	/// The side of the mesh
	/// </summary>
	public enum MeshSide
	{
		Positive = 0,
		Negative = 1
	}

	public static class EM
	{
		public static int[] FindAllIndexof<T>(this IEnumerable<T> values, T val)
		{
			return values.Select((b, i) => object.Equals(b, val) ? i : -1).Where(i => i != -1).ToArray();
		}
	}

	/// <summary>
	/// An object used to manage the positive and negative side mesh data for a sliced object
	/// </summary>
	class SlicesMetadata
	{
		private Mesh _positiveSideMesh;
		private List<Vector3> _positiveSideVertices;
		private GameObject _original;
		private List<List<int>> _positiveSideTriangles;
		private List<Vector2> _positiveSideUvs;
		private List<Vector3> _positiveSideNormals;

		private Mesh _negativeSideMesh;
		private List<Vector3> _negativeSideVertices;
		private List<List<int>> _negativeSideTriangles;
		private List<Vector2> _negativeSideUvs;
		private List<Vector3> _negativeSideNormals;

		private readonly List<Vector3> _pointsAlongPlane;
		private Plane _plane;
		private Mesh _mesh;
		private bool _isSolid;
		private bool _useSharedVertices = false;
		private bool _smoothVertices = false;
		private int _cutMaterialID;
		private bool _createReverseTriangleWindings = false;
		private int _submesh;

		public bool IsSolid
		{
			get
			{
				return _isSolid;
			}
			set
			{
				_isSolid = value;
			}
		}

		public Mesh PositiveSideMesh
		{
			get
			{
				if (_positiveSideMesh == null)
				{
					_positiveSideMesh = new Mesh();
				}

				SetMeshData(MeshSide.Positive);
				return _positiveSideMesh;
			}
		}

		public Mesh NegativeSideMesh
		{
			get
			{
				if (_negativeSideMesh == null)
				{
					_negativeSideMesh = new Mesh();
				}

				SetMeshData(MeshSide.Negative);

				return _negativeSideMesh;
			}
		}

		public SlicesMetadata(GameObject original, Plane plane, Mesh mesh, bool isSolid, bool createReverseTriangleWindings, bool shareVertices, bool smoothVertices)
		{
			_original = original;
			_positiveSideTriangles = new List<List<int>>();
			_positiveSideVertices = new List<Vector3>();
			_negativeSideTriangles = new List<List<int>>();
			_negativeSideVertices = new List<Vector3>();
			_positiveSideUvs = new List<Vector2>();
			_negativeSideUvs = new List<Vector2>();
			_positiveSideNormals = new List<Vector3>();
			_negativeSideNormals = new List<Vector3>();
			_pointsAlongPlane = new List<Vector3>();
			_plane = plane;
			_mesh = mesh;
			_isSolid = isSolid;
			_createReverseTriangleWindings = createReverseTriangleWindings;
			_useSharedVertices = shareVertices;
			_smoothVertices = smoothVertices;

			_cutMaterialID = int.TryParse(Tags.Tags.GetComponent(_original).GetTagData("Physics.Cuttable.InnerMaterialID"), out int cutMaterialIDTemp) ? cutMaterialIDTemp : 0;

			for (var i = 0; (i < Math.Max(_mesh.subMeshCount, _cutMaterialID + 1)); i++)
			{

				_negativeSideTriangles.Add(new List<int>());
				_positiveSideTriangles.Add(new List<int>());
			}
			for (var i = 0; (i < _mesh.subMeshCount); i++)
			{

				_submesh = i;
				ComputeNewMeshes(i);
			}
			_submesh = _cutMaterialID;
			if (_isSolid) JoinPointsAlongPlane();
		}

		/// <summary>
		/// Add the mesh data to the correct side and calulate normals
		/// </summary>
		/// <param name="side"></param>
		/// <param name="vertex1"></param>
		/// <param name="vertex1Uv"></param>
		/// <param name="vertex2"></param>
		/// <param name="vertex2Uv"></param>
		/// <param name="vertex3"></param>
		/// <param name="vertex3Uv"></param>
		/// <param name="shareVertices"></param>
		private void AddTrianglesNormalAndUvs(MeshSide side, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
		{
			if (side == MeshSide.Positive)
			{
				var tris = _positiveSideTriangles[_submesh];
				AddTrianglesNormalsAndUvs(ref _positiveSideVertices, tris, ref _positiveSideNormals, ref _positiveSideUvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
			}
			else
			{
				var tris = _negativeSideTriangles[_submesh];
				AddTrianglesNormalsAndUvs(ref _negativeSideVertices, tris, ref _negativeSideNormals, ref _negativeSideUvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
			}
		}


		/// <summary>
		/// Adds the vertices to the mesh sets the triangles in the order that the vertices are provided.
		/// If shared vertices is false vertices will be added to the list even if a matching vertex already exists
		/// Does not compute normals
		/// </summary>
		/// <param name="vertices"></param>
		/// <param name="triangles"></param>
		/// <param name="uvs"></param>
		/// <param name="normals"></param>
		/// <param name="vertex1"></param>
		/// <param name="vertex1Uv"></param>
		/// <param name="normal1"></param>
		/// <param name="vertex2"></param>
		/// <param name="vertex2Uv"></param>
		/// <param name="normal2"></param>
		/// <param name="vertex3"></param>
		/// <param name="vertex3Uv"></param>
		/// <param name="normal3"></param>
		/// <param name="shareVertices"></param>
		private void AddTrianglesNormalsAndUvs(ref List<Vector3> vertices, List<int> triangles, ref List<Vector3> normals, ref List<Vector2> uvs, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
		{
			int tri1Index = vertices.IndexOf(vertex1);

			if (addFirst)
			{
				ShiftTriangleIndeces(ref triangles);
			}

			//If a the vertex already exists we just add a triangle reference to it, if not add the vert to the list and then add the tri index
			if (tri1Index > -1 && shareVertices)
			{
				triangles.Add(tri1Index);
			}
			else
			{
				if (normal1 == null)
				{
					normal1 = ComputeNormal(vertex1, vertex2, vertex3);
				}

				int? i = null;
				if (addFirst)
				{
					i = 0;
				}

				AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex1, (Vector3)normal1, uv1, i);
			}

			int tri2Index = vertices.IndexOf(vertex2);

			if (tri2Index > -1 && shareVertices)
			{
				triangles.Add(tri2Index);
			}
			else
			{
				if (normal2 == null)
				{
					normal2 = ComputeNormal(vertex2, vertex3, vertex1);
				}

				int? i = null;

				if (addFirst)
				{
					i = 1;
				}

				AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex2, (Vector3)normal2, uv2, i);
			}

			int tri3Index = vertices.IndexOf(vertex3);

			if (tri3Index > -1 && shareVertices)
			{
				triangles.Add(tri3Index);
			}
			else
			{
				if (normal3 == null)
				{
					normal3 = ComputeNormal(vertex3, vertex1, vertex2);
				}

				int? i = null;
				if (addFirst)
				{
					i = 2;
				}

				AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex3, (Vector3)normal3, uv3, i);
			}
		}

		private void AddVertNormalUv(ref List<Vector3> vertices, ref List<Vector3> normals, ref List<Vector2> uvs, ref List<int> triangles, Vector3 vertex, Vector3 normal, Vector2 uv, int? index)
		{
			if (index != null)
			{
				int i = (int)index;
				vertices.Insert(i, vertex);
				uvs.Insert(i, uv);
				normals.Insert(i, normal);
				triangles.Insert(i, i);
			}
			else
			{
				int vIndex = vertices.Count;
				vertices.Add(vertex);
				normals.Add(normal);
				uvs.Add(uv);
				triangles.Add(vIndex);
			}
		}

		private void ShiftTriangleIndeces(ref List<int> triangles)
		{
			for (int j = 0; j < triangles.Count; j += 3)
			{
				triangles[j] += +3;
				triangles[j + 1] += 3;
				triangles[j + 2] += 3;
			}
		}

		/// <summary>
		/// Will render the inside of an object
		/// This is heavy as it duplicates all the vertices and creates opposite winding direction
		/// </summary>
		private void AddReverseTriangleWinding()
		{

			int positiveVertsStartIndex = _positiveSideVertices.Count;
			//Duplicate the original vertices
			_positiveSideVertices.AddRange(_positiveSideVertices);
			_positiveSideUvs.AddRange(_positiveSideUvs);
			_positiveSideNormals.AddRange(FlipNormals(_positiveSideNormals));

			int numPositiveTriangles = _positiveSideTriangles[_submesh].Count;

			//Add reverse windings
			for (int i = 0; i < numPositiveTriangles; i += 3)
			{
				_positiveSideTriangles[_submesh].Add(positiveVertsStartIndex + _positiveSideTriangles[_submesh][i]);
				_positiveSideTriangles[_submesh].Add(positiveVertsStartIndex + _positiveSideTriangles[_submesh][i + 2]);
				_positiveSideTriangles[_submesh].Add(positiveVertsStartIndex + _positiveSideTriangles[_submesh][i + 1]);
			}

			int negativeVertextStartIndex = _negativeSideVertices.Count;
			//Duplicate the original vertices
			_negativeSideVertices.AddRange(_negativeSideVertices);
			_negativeSideUvs.AddRange(_negativeSideUvs);
			_negativeSideNormals.AddRange(FlipNormals(_negativeSideNormals));

			int numNegativeTriangles = _negativeSideTriangles[_submesh].Count;

			//Add reverse windings
			for (int i = 0; i < numNegativeTriangles; i += 3)
			{
				_negativeSideTriangles[_submesh].Add(negativeVertextStartIndex + _negativeSideTriangles[_submesh][i]);
				_negativeSideTriangles[_submesh].Add(negativeVertextStartIndex + _negativeSideTriangles[_submesh][i + 2]);
				_negativeSideTriangles[_submesh].Add(negativeVertextStartIndex + _negativeSideTriangles[_submesh][i + 1]);
			}
		}

		/// <summary>
		/// Join the points along the plane to the halfway point
		/// </summary>
		private void JoinPointsAlongPlane()
		{
			Vector3 halfway = GetHalfwayPoint(out float distance);

			for (int i = 0; i < _pointsAlongPlane.Count; i += 2)
			{
				Vector3 firstVertex;
				Vector3 secondVertex;

				firstVertex = _pointsAlongPlane[i];
				secondVertex = _pointsAlongPlane[i + 1];

				Vector3 normal3 = ComputeNormal(halfway, secondVertex, firstVertex);
				normal3.Normalize();

				var direction = Vector3.Dot(normal3, _plane.normal);

				if (direction > 0)
				{
					AddTrianglesNormalAndUvs(MeshSide.Positive, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, false);
					AddTrianglesNormalAndUvs(MeshSide.Negative, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, false);
				}
				else
				{
					AddTrianglesNormalAndUvs(MeshSide.Positive, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, false);
					AddTrianglesNormalAndUvs(MeshSide.Negative, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, false);
				}
			}
		}

		/// <summary>
		/// For all the points added along the plane cut, get the half way between the first and furthest point
		/// </summary>
		/// <returns></returns>
		private Vector3 GetHalfwayPoint(out float distance)
		{
			if (_pointsAlongPlane.Count > 0)
			{
				Vector3 firstPoint = _pointsAlongPlane[0];
				Vector3 furthestPoint = Vector3.zero;
				distance = 0f;

				foreach (Vector3 point in _pointsAlongPlane)
				{
					float currentDistance = 0f;
					currentDistance = Vector3.Distance(firstPoint, point);

					if (currentDistance > distance)
					{
						distance = currentDistance;
						furthestPoint = point;
					}
				}

				return Vector3.Lerp(firstPoint, furthestPoint, 0.5f);
			}
			else
			{
				distance = 0;
				return Vector3.zero;
			}
		}

		/// <summary>
		/// Setup the mesh object for the specified side
		/// </summary>
		/// <param name="side"></param>
		private void SetMeshData(MeshSide side)
		{
			if (side == MeshSide.Positive)
			{
				_positiveSideMesh.subMeshCount = Math.Max(_cutMaterialID + 1, _mesh.subMeshCount);
				_positiveSideMesh.vertices = _positiveSideVertices.ToArray();
				_positiveSideMesh.normals = _positiveSideNormals.ToArray();
				_positiveSideMesh.uv = _positiveSideUvs.ToArray();
				for (var i = 0; i < _positiveSideMesh.subMeshCount; i++)
					_positiveSideMesh.SetTriangles(_positiveSideTriangles[i].ToArray(), i);
			}
			else
			{
				_negativeSideMesh.subMeshCount = Math.Max(_cutMaterialID + 1, _mesh.subMeshCount);
				_negativeSideMesh.vertices = _negativeSideVertices.ToArray();
				_negativeSideMesh.normals = _negativeSideNormals.ToArray();
				_negativeSideMesh.uv = _negativeSideUvs.ToArray();
				for (var i = 0; i < _negativeSideMesh.subMeshCount; i++)
					_negativeSideMesh.SetTriangles(_negativeSideTriangles[i].ToArray(), i);
			}
		}

		/// <summary>
		/// Compute the positive and negative meshes based on the plane and mesh
		/// </summary>
		private void ComputeNewMeshes(int submesh)
		{
			_submesh = submesh;
			int[] meshTriangles = _mesh.GetTriangles(submesh);
			Vector3[] meshVerts = _mesh.vertices;
			Vector3[] meshNormals = _mesh.normals;
			Vector2[] meshUvs = _mesh.uv;

			for (int i = 0; i < meshTriangles.Length; i += 3)
			{
				//We need the verts in order so that we know which way to wind our new mesh triangles.
				int vert1Index = meshTriangles[i];
				Vector3 vert1 = meshVerts[vert1Index];
				Vector2 uv1 = meshUvs[vert1Index];
				Vector3 normal1 = meshNormals[vert1Index];
				bool vert1Side = _plane.GetSide(vert1);

				int vert2Index = meshTriangles[i + 1];
				Vector3 vert2 = meshVerts[vert2Index];
				Vector2 uv2 = meshUvs[vert2Index];
				Vector3 normal2 = meshNormals[vert2Index];
				bool vert2Side = _plane.GetSide(vert2);

				int vert3Index = meshTriangles[i + 2];
				Vector3 vert3 = meshVerts[vert3Index];
				Vector2 uv3 = meshUvs[vert3Index];
				Vector3 normal3 = meshNormals[vert3Index];
				bool vert3Side = _plane.GetSide(vert3);

				//All verts are on the same side
				if (vert1Side == vert2Side && vert2Side == vert3Side)
				{
					//Add the relevant triangle
					MeshSide side = (vert1Side) ? MeshSide.Positive : MeshSide.Negative;
					AddTrianglesNormalAndUvs(side, vert1, normal1, uv1, vert2, normal2, uv2, vert3, normal3, uv3, _useSharedVertices, false);
				}
				else
				{
					//we need the two points where the plane intersects the triangle.
					Vector3 intersection1;
					Vector3 intersection2;

					Vector2 intersection1Uv;
					Vector2 intersection2Uv;

					MeshSide side1 = (vert1Side) ? MeshSide.Positive : MeshSide.Negative;
					MeshSide side2 = (vert1Side) ? MeshSide.Negative : MeshSide.Positive;

					//vert 1 and 2 are on the same side
					if (vert1Side == vert2Side)
					{
						//Cast a ray from v2 to v3 and from v3 to v1 to get the intersections                       
						intersection1 = GetRayPlaneIntersectionPointAndUv(vert2, uv2, vert3, uv3, out intersection1Uv);
						intersection2 = GetRayPlaneIntersectionPointAndUv(vert3, uv3, vert1, uv1, out intersection2Uv);

						//Add the positive or negative triangles
						AddTrianglesNormalAndUvs(side1, vert1, null, uv1, vert2, null, uv2, intersection1, null, intersection1Uv, _useSharedVertices, false);
						AddTrianglesNormalAndUvs(side1, vert1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, _useSharedVertices, false);

						AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert3, null, uv3, intersection2, null, intersection2Uv, _useSharedVertices, false);

					}
					//vert 1 and 3 are on the same side
					else if (vert1Side == vert3Side)
					{
						//Cast a ray from v1 to v2 and from v2 to v3 to get the intersections                       
						intersection1 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert2, uv2, out intersection1Uv);
						intersection2 = GetRayPlaneIntersectionPointAndUv(vert2, uv2, vert3, uv3, out intersection2Uv);

						//Add the positive triangles
						AddTrianglesNormalAndUvs(side1, vert1, null, uv1, intersection1, null, intersection1Uv, vert3, null, uv3, _useSharedVertices, false);
						AddTrianglesNormalAndUvs(side1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, vert3, null, uv3, _useSharedVertices, false);

						AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert2, null, uv2, intersection2, null, intersection2Uv, _useSharedVertices, false);
					}
					//Vert1 is alone
					else
					{
						//Cast a ray from v1 to v2 and from v1 to v3 to get the intersections                       
						intersection1 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert2, uv2, out intersection1Uv);
						intersection2 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert3, uv3, out intersection2Uv);

						AddTrianglesNormalAndUvs(side1, vert1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, _useSharedVertices, false);

						AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert2, null, uv2, vert3, null, uv3, _useSharedVertices, false);
						AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert3, null, uv3, intersection2, null, intersection2Uv, _useSharedVertices, false);
					}

					//Add the newly created points on the plane.
					_pointsAlongPlane.Add(intersection1);
					_pointsAlongPlane.Add(intersection2);
				}
			}

			//If the object is solid, join the new points along the plane otherwise do the reverse winding

			if (_createReverseTriangleWindings)
			{
				AddReverseTriangleWinding();
			}

			if (_smoothVertices)
			{
				SmoothVertices();
			}

		}

		void calculateSolid()
		{
			if (_isSolid)
			{
				JoinPointsAlongPlane();
			}
		}

		/// <summary>
		/// Casts a reay from vertex1 to vertex2 and gets the point of intersection with the plan, calculates the new uv as well.
		/// </summary>
		/// <param name="plane">The plane.</param>
		/// <param name="vertex1">The vertex1.</param>
		/// <param name="vertex1Uv">The vertex1 uv.</param>
		/// <param name="vertex2">The vertex2.</param>
		/// <param name="vertex2Uv">The vertex2 uv.</param>
		/// <param name="uv">The uv.</param>
		/// <returns>Point of intersection</returns>
		private Vector3 GetRayPlaneIntersectionPointAndUv(Vector3 vertex1, Vector2 vertex1Uv, Vector3 vertex2, Vector2 vertex2Uv, out Vector2 uv)
		{
			float distance = GetDistanceRelativeToPlane(vertex1, vertex2, out Vector3 pointOfIntersection) / (vertex1 - vertex2).magnitude;
			uv = InterpolateUvs(vertex1Uv, vertex2Uv, distance);
			return pointOfIntersection;
		}

		/// <summary>
		/// Computes the distance based on the plane.
		/// </summary>
		/// <param name="vertex1">The vertex1.</param>
		/// <param name="vertex2">The vertex2.</param>
		/// <param name="pointOfintersection">The point ofintersection.</param>
		/// <returns></returns>
		private float GetDistanceRelativeToPlane(Vector3 vertex1, Vector3 vertex2, out Vector3 pointOfintersection)
		{
			Ray ray = new Ray(vertex1, (vertex2 - vertex1));
			_plane.Raycast(ray, out float distance);
			pointOfintersection = ray.GetPoint(distance);
			return distance;
		}

		/// <summary>
		/// Get a uv between the two provided uvs by the distance.
		/// </summary>
		/// <param name="uv1">The uv1.</param>
		/// <param name="uv2">The uv2.</param>
		/// <param name="distance">The distance.</param>
		/// <returns></returns>
		private Vector2 InterpolateUvs(Vector2 uv1, Vector2 uv2, float distance)
		{
			Vector2 uv = Vector2.Lerp(uv1, uv2, distance);
			return uv;
		}

		/// <summary>
		/// Gets the point perpendicular to the face defined by the provided vertices        
		//https://docs.unity3d.com/Manual/ComputingNormalPerpendicularVector.html
		/// </summary>
		/// <param name="vertex1"></param>
		/// <param name="vertex2"></param>
		/// <param name="vertex3"></param>
		/// <returns></returns>
		private Vector3 ComputeNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
		{
			Vector3 side1 = vertex2 - vertex1;
			Vector3 side2 = vertex3 - vertex1;

			Vector3 normal = Vector3.Cross(side1, side2).normalized;

			return normal;
		}

		/// <summary>
		/// Reverese the normals in a given list
		/// </summary>
		/// <param name="currentNormals"></param>
		/// <returns></returns>
		private List<Vector3> FlipNormals(List<Vector3> currentNormals)
		{
			List<Vector3> flippedNormals = new List<Vector3>();

			foreach (Vector3 normal in currentNormals)
			{
				flippedNormals.Add(-normal);
			}

			return flippedNormals;
		}

		//
		private void SmoothVertices()
		{
			var posTris = _positiveSideTriangles[_submesh];
			DoSmoothing(ref _positiveSideVertices, ref _positiveSideNormals, ref posTris);
			var negTris = _negativeSideTriangles[_submesh];
			DoSmoothing(ref _negativeSideVertices, ref _negativeSideNormals, ref negTris);
		}

		private void DoSmoothing(ref List<Vector3> vertices, ref List<Vector3> normals, ref List<int> triangles)
		{
			Vector3[] normalsRead = normals.ToArray();

			float _smoothingAngle = 70f;
			float smoothingLimit = Mathf.Cos(Mathf.Deg2Rad * _smoothingAngle);

			for (int i = 0; i < triangles.Count; i += 3)
			{
				// Find the vertices which have duplicates in the mesh
				int index1 = triangles[i];
				int index2 = triangles[i + 1];
				int index3 = triangles[i + 2];

				Vector3 vertex1 = vertices[index1];
				Vector3 vertex2 = vertices[index2];
				Vector3 vertex3 = vertices[index3];

				int[] v1 = vertices.FindAllIndexof(vertex1).Where(v => Vector3.Dot(normalsRead[v], normalsRead[index1]) > 0.5f).ToArray();
				int[] v2 = vertices.FindAllIndexof(vertex2).Where(v => Vector3.Dot(normalsRead[v], normalsRead[index2]) > 0.5f).ToArray();
				int[] v3 = vertices.FindAllIndexof(vertex3).Where(v => Vector3.Dot(normalsRead[v], normalsRead[index3]) > 0.5f).ToArray();

				// Set the vertecies for this triangle to the last element of the v arrays, otherwise keep the original vertex
				triangles[i] = v1.Length > 0 ? v1[v1.Length - 1] : index1;
				triangles[i + 1] = v2.Length > 0 ? v2[v2.Length - 1] : index2;
				triangles[i + 2] = v3.Length > 0 ? v3[v3.Length - 1] : index3;

			}

			normals.ForEach(x =>
			{
				x = Vector3.zero;
			});

			for (int i = 0; i < triangles.Count; i += 3)
			{
				int vertIndex1 = triangles[i];
				int vertIndex2 = triangles[i + 1];
				int vertIndex3 = triangles[i + 2];

				Vector3 triangleNormal = ComputeNormal(vertices[vertIndex1], vertices[vertIndex2], vertices[vertIndex3]);

				normals[vertIndex1] += triangleNormal;
				normals[vertIndex2] += triangleNormal;
				normals[vertIndex3] += triangleNormal;
			}

			normals.ForEach(x =>
			{
				x.Normalize();
			});


		}
	}

}
