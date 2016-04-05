using UnityEngine;

public static class Factory
{

	public static void AddBoxCollider(GameObject _container, bool isTrigger, Vector3 size)
	{
		BoxCollider col = _container.AddComponent<BoxCollider>();
		col.isTrigger = isTrigger;
		col.size = size;
	}

	public static BoxCollider GetBoxCollider(GameObject _container, bool isTrigger, Vector3 size)
	{
		BoxCollider col = _container.AddComponent<BoxCollider>();
		col.isTrigger = isTrigger;
		col.size = size;
		return col;
	}

	public static void AddSphereCollider(GameObject _container, bool isTrigger, float radius)
	{
		SphereCollider col = _container.AddComponent<SphereCollider>();
		col.isTrigger = isTrigger;
		col.radius = radius;
	}

	public static void AddMeshCollider(GameObject _container, bool isTrigger, Mesh mesh)
	{
		MeshCollider col = _container.AddComponent<MeshCollider>();
		col.convex = true;
		col.isTrigger = isTrigger;
		col.sharedMesh = mesh;
	}

	public static void AddRigidbodyForCollisionVisability(GameObject _container)
	{
		Rigidbody rigidBody = _container.AddComponent<Rigidbody>();
		rigidBody.mass = rigidBody.drag = rigidBody.angularDrag = 0;
		rigidBody.useGravity = rigidBody.isKinematic = false;
		rigidBody.constraints = RigidbodyConstraints.FreezeAll;
	}

	public static void Add3DGraphics(GameObject _container, Mesh _mesh, Material _material)
	{
		_container.AddComponent<MeshFilter>().mesh = _mesh;
		MeshRenderer meshRender = _container.AddComponent<MeshRenderer>();
		SetUpMeshRender(meshRender, _material);
	}

	public static void SetUpMeshRender(MeshRenderer _meshRender, Material _material)
	{
		_meshRender.material = _material;
		_meshRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		_meshRender.receiveShadows = _meshRender.useLightProbes = false;
		_meshRender.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
	}

	public static void SetUpSkinnedMeshRender(SkinnedMeshRenderer _skinnedMeshRender, Material[] _materials)
	{
		_skinnedMeshRender.materials = _materials;
		_skinnedMeshRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		_skinnedMeshRender.useLightProbes = false;
		_skinnedMeshRender.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
	}

	public static void FillLocal(Transform from, Transform to)
	{
		to.localPosition = from.localPosition;
		to.localEulerAngles = from.localEulerAngles;
		to.localScale = from.localScale;
	}

	public static void Reset(Transform t)
	{
		t.localPosition = Vector3.zero;
		t.localEulerAngles = Vector3.zero;
		t.localScale = Vector3.one;
	}

	public static Vector3 WorldEulerAngles(Transform t)
	{
		Vector3 worldEulerAngles = t.transform.eulerAngles;
		int axe = 0;
		foreach (Transform next in Sequence(t))
		{
			worldEulerAngles += next.transform.localEulerAngles;
			for (axe = 0; axe < 3; axe++)
			{
				if (worldEulerAngles[axe] >= 360) worldEulerAngles[axe] -= 360;
			}
			Debug.Log(worldEulerAngles);
		}
		Debug.Log("Result: " + worldEulerAngles);
		return worldEulerAngles;
	}

	static System.Collections.IEnumerable Sequence(Transform t)
	{
		while (t != null)
		{
			if (t.parent != null) yield return t.parent;
			t = t.parent;
		}
	}

}