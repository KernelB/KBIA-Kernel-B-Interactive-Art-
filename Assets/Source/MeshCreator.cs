using UnityEngine;

public static class MeshCreator
{
	public const int MAX_VERTS = 65535;

	public static Mesh Get(Mesh primitive, uint width, uint height, float scale, out Transform[] groups)
	{
		MeshFilter[] meshFilters = new MeshFilter[width * height];
		groups = new Transform[meshFilters.Length];

		Vector3 pivot = new Vector3(
			-width / 2 + primitive.bounds.extents.x, 
			0,
			-height / 2 + primitive.bounds.extents.z);

		Vector3 colSize = primitive.bounds.size * scale;

		int i = 0;
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				MeshFilter _filter = new GameObject(i.ToString()).AddComponent<MeshFilter>();
				_filter.mesh = primitive;
				_filter.transform.position = pivot + new Vector3(x, 0, y);
				_filter.transform.localScale *= scale;

				Factory.AddBoxCollider(_filter.gameObject, true, colSize);

				meshFilters[i] = _filter;
				groups[i] = _filter.transform;
				i++;
			}
		}

		return Combine(meshFilters);
	}

	static Mesh Combine( MeshFilter[] meshFilters )
	{
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		AppManager appManager = Camera.main.GetComponent<AppManager>();
		while (i < meshFilters.Length)
		{
			combine[i].mesh = meshFilters[i].mesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			appManager.Delete(meshFilters[i]);
			i++;
		}

		Mesh result = new Mesh();
		result.CombineMeshes(combine);
		return result;
	}

}