using UnityEngine;

public class CPUManager : MonoBehaviour 
{
	//References
	//polymorph
	public Light light;
	public Color gold;
	public Texture colaTex;
	public Texture colaBump;
	public Texture cubeBump;
	//geometry
	public MeshFilter filter;
	public Material mat;
	// 0 - Cube
	// 1 - Tube
	// 2 - Cola
	public Mesh[] primitives;
	public Color[] colors;
	//Data	
	public int meshID;
	public uint width;	//numbers of objects.
	public uint height;	//numbers of objects.

	int groupVertsCount;
	//Variables
	Transform[] groups;
	Vector3[] vertices;
	Vector2[] uv2s;
	Vector2[] uv3s;
	int vertID;
	int groupID;
	//touch
	//---------------------
	RaycastHit hit;
	Ray ray;
	Vector3 pressPoint;
	Vector3 touchPoint;
	const int MAX_TOUCHES = 8;
	int touchID;
	int touchNum;
	bool activeMode;
	//animation
	//---------------------
	float[] groupLevel;
	float[] groupDitancesLast;
	float progress;
	internal float moveSpeed;			//animation appearance
	
	//wave
	//---------------------
	public float speedWaveSpread;
	internal float wavesAttenuation;	//waves attenuation
	internal float lenghtOfPlume;		//length of plume
	int waveNumber;
	float distanceX, distanceZ;
	float[] waveAmplitude;
	Vector2[] impactPos;
	float[] distance;
	//passive
	//---------------------
	float[] to;
	float[] initDistances;
	Vector3 passiveCamPos;
	Vector3 passiveEuler = new Vector3(90, 0);
	public bool animateCamera = true;
	//---------------------

	public void Reset()
	{
		meshID = 0;
		width = DeafultValues(meshID, out height);
		wavesAttenuation = 0.98f;
		lenghtOfPlume = 20.0f;
		moveSpeed = 0.01f;

		Destroy(filter.mesh);
	}

	public void SetUp(string widthStr, string heighStr)
	{
		uint widthCandidate;
		uint heightCandidate;

		//----------------------------------------------

		if (uint.TryParse(widthStr, out widthCandidate) &&
			uint.TryParse(heighStr, out heightCandidate))
		{
			int step = 0;
			while (widthCandidate * heightCandidate > AccepatbleObjectsCount())
			{
				if (step % 2 == 0) widthCandidate--;
				else heightCandidate--;
				step++;
			}
		}
		else
		{
			widthCandidate = DeafultValues(meshID, out heightCandidate);
		}

		//----------------------------------------------

		width = widthCandidate;
		height = heightCandidate;
	}

	public void Init()
	{
		//GENERATE
		filter.mesh = MeshCreator.Get(primitives[meshID], width, height, 1.0f, out groups);

		//DATA
		groupVertsCount = primitives[meshID].vertexCount;

		//BUFFERS
		vertices = new Vector3[filter.mesh.vertexCount];
		uv2s = new Vector2[vertices.Length];
		uv3s = new Vector2[vertices.Length];
		groupDitancesLast = new float[groups.Length];
		groupLevel = new float[groups.Length];

		waveAmplitude = new float[MAX_TOUCHES];
		impactPos = new Vector2[MAX_TOUCHES];
		distance = new float[MAX_TOUCHES];

		to = new float[groups.Length];
		initDistances = new float[groups.Length];

		for (groupID = 0; groupID < groups.Length; groupID++)
		{
			initDistances[groupID] = Vector3.Distance(groups[groupID].position, Vector3.zero);

			to[groupID] = Random.Range(0.5f, 2.0f);
		}

		//Group initial positions
		for (vertID = 0; vertID < vertices.Length; vertID++)	//HUGE !!!
		{
			groupID = GroupID();

			//Center Distance
			uv2s[vertID][1] = initDistances[groupID];

			uv3s[vertID][0] = groups[groupID].position.x;
			uv3s[vertID][1] = groups[groupID].position.z;
		}
		filter.mesh.uv3 = uv3s;

		//SURFACE
		if (meshID == 0)
		{
			//Mapping
			DefaultSurface();
			//Mesh colors
			filter.mesh.colors = Colors();
			//Lighting
			DefaultLight();
			//Background
			Camera.main.backgroundColor = Color.white;
		}
		else if (meshID == 1)
		{
			//Mapping
			DefaultSurface();
			//Mesh colors
			filter.mesh.colors = null;
			//Lighting
			light.color = gold;
			light.intensity = 1.5f;
			//Background
			Camera.main.backgroundColor = gold;
		}
		else if (meshID == 2)
		{
			//Mapping
			mat.mainTexture = colaTex;
			mat.SetTexture("_BumpMap", colaBump);
			//Mesh colors
			filter.mesh.colors = null;
			//Lighting
			DefaultLight();
			//Background
			Camera.main.backgroundColor = Color.white;
		}

		//CAMERA POS
		passiveCamPos = new Vector3(0, CamHeight());
		Camera.main.transform.position = passiveCamPos;
	}

	float CamHeight()
	{
		float b = filter.mesh.bounds.max.z;
		float bc = 90.0f - Camera.main.fieldOfView / 2;
		float c = b / Mathf.Cos(bc);
		float a = c * Mathf.Sin(bc);
		return a / 10;	// 10 - ??
	}

	void DefaultSurface()
	{
		mat.mainTexture = null;
		mat.SetTexture("_BumpMap", cubeBump);
	}

	void DefaultLight()
	{
		light.color = Color.white;
		light.intensity = 1.0f;
	}

	Color[] Colors()
	{
		Color[] _colors = new Color[vertices.Length];
		Color colorVar = Color.white;
		int previousGroupID = -1;
		for (vertID = 0; vertID < _colors.Length; vertID++)
		{
			groupID = GroupID();
			if (groupID != previousGroupID)
			{
				colorVar = colors[Random.Range(0, colors.Length)];
				previousGroupID = groupID;
			}
			_colors[vertID] = colorVar;
		}
		return _colors;
	}

	public static void SmoothLookAt(Transform origin, Vector3 direction, float _rotateSpeed = 1.0f)
	{
		origin.rotation = SmoothLookRotation(origin, direction, _rotateSpeed);
	}

	public static Quaternion SmoothLookRotation(Transform origin, Vector3 direction, float _rotateSpeed = 1.0f)
	{
		return Quaternion.Lerp(
			origin.rotation,
			Quaternion.LookRotation(direction),
			Time.deltaTime * _rotateSpeed);
	}

	void Update() 
	{

		//Input
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit))
		{
			//General custom logic
			if (Input.GetMouseButtonDown(0))
			{
				activeMode = true;
				OnMove(hit.transform.position, 10.0f);
				touchPoint = hit.transform.position;
			}
			if (Input.GetMouseButton(0))
			{
				if (hit.transform.position != pressPoint)
				{
					progress = 0;
					groupDitancesLast = groupLevel;
					pressPoint = hit.transform.position;
				}
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			activeMode = false;

			progress = 0;
			groupDitancesLast = groupLevel;

			Camera.main.transform.eulerAngles = passiveEuler;
			Camera.main.transform.position = passiveCamPos;
		}

		//Wave
		//---------------------------------------------
		for (touchID = 0; touchID < MAX_TOUCHES; touchID++)
		{
			touchNum = touchID + 1;
			waveAmplitude[touchID] = mat.GetFloat("_WaveAmplitude" + touchNum);
			if (waveAmplitude[touchID] > 0)
			{
				if (distance[touchID] < lenghtOfPlume) distance[touchID] += speedWaveSpread;
				mat.SetFloat("_Distance" + touchNum, distance[touchID]);
				mat.SetFloat("_WaveAmplitude" + touchNum, waveAmplitude[touchID] * wavesAttenuation);
			}
			if (waveAmplitude[touchID] < 0.01)
			{
				mat.SetFloat("_WaveAmplitude" + touchNum, 0);
				distance[touchID] = 0;
			}
		}
		//---------------------------------------------

		
		if (activeMode)
		{
			//Groups
			for (groupID = 0; groupID < groups.Length; groupID++)
			{
				groupLevel[groupID] =
					Mathf.Lerp(
						groupDitancesLast[groupID],
						Vector3.Distance(groups[groupID].position, pressPoint),
						progress += Time.deltaTime * moveSpeed);	//Distance of Group ID from Touch (INTERPOLATED FROM PREVIOUS)
			}

			//CAMERA animation
			if (animateCamera)
			{
				SmoothLookAt(Camera.main.transform, touchPoint - Camera.main.transform.position, 3.0f);
				Camera.main.transform.position = new Vector3(-touchPoint.x * 2, passiveCamPos.y, -touchPoint.z * 2);
			}
		}
		else //if (! activeMode)
		{
			//Groups
			if (meshID == 1)
			{
				for (groupID = 0; groupID < groups.Length; groupID++)
				{
					groupLevel[groupID] =
						Mathf.Lerp(
							groupDitancesLast[groupID],
							1.5f * Mathf.Sin(1 * initDistances[groupID] - 2.0f * Time.time),
							progress += Time.deltaTime * moveSpeed);
				}
			}
			else //if (meshID == 0 || meshID ==2  )
			{
				for (groupID = 0; groupID < groups.Length; groupID++)
				{
					groupLevel[groupID] =
						Mathf.Lerp(
							groupDitancesLast[groupID],
							to[groupID] + Mathf.PingPong(Time.time, to[groupID]),
							progress += Time.deltaTime * moveSpeed);
				}
			}
		}

		//Shader DATA
		//---------------------------------------------
		for (vertID = 0; vertID < vertices.Length; vertID++)	//HUGE !!!
		{
			//Fill Touch Info
			uv2s[vertID][0] = groupLevel[GroupID()];	//Target Y
		}
		filter.mesh.uv2 = uv2s;
		//---------------------------------------------
	}

	int GroupID()
	{
		return vertID / groupVertsCount;
	}

	//Is Top of The Cube
	//bool TopVertex(int _index)
	//{
	//	return _index == 2 ||
	//			_index == 3 ||
	//			_index == 4 ||
	//			_index == 5 ||
	//			_index == 8 ||
	//			_index == 9 ||
	//			_index == 10 ||
	//			_index == 11 ||
	//			_index == 17 ||
	//			_index == 18 ||
	//			_index == 21 ||
	//			_index == 22;
	//}


	void OnMove( Vector3 point, float touchForce)
	{
		waveNumber++;
		if (waveNumber == 9)
		{
			waveNumber = 1;
		}

		//waveAmplitude[waveNumber - 1] = 0;
		//distance[waveNumber - 1] = 0;

		distanceX = this.transform.position.x - point.x;
		distanceZ = this.transform.position.z - point.z;
		impactPos[waveNumber - 1].x = point.x;
		impactPos[waveNumber - 1].y = point.z;

		mat.SetFloat("_xImpact" + waveNumber, point.x);
		mat.SetFloat("_zImpact" + waveNumber, point.z);

		mat.SetFloat("_OffsetX" + waveNumber, distanceX / filter.mesh.bounds.size.x * lenghtOfPlume);
		mat.SetFloat("_OffsetZ" + waveNumber, distanceZ / filter.mesh.bounds.size.z * lenghtOfPlume);

		mat.SetFloat("_WaveAmplitude" + waveNumber, touchForce);
	}

	public int AccepatbleObjectsCount()
	{
		return MeshCreator.MAX_VERTS / primitives[meshID].vertexCount;
	}

	//public int PotentialVertices(int w, int h)
	//{
	//	return primitives[meshID].vertexCount * w * h;
	//}

	//public bool AcceptableMesh(int w, int h)
	//{
	//	return PotentialVertices(w, h) < MeshCreator.MAX_VERTS;
	//}

	public uint DeafultValues(int _meshID, out uint _height)
	{
		switch (_meshID)
		{
			case 0:
				{
					_height = 20;
					return 36;
				}
			case 1:
				{
					_height = 60;
					return 108;
				}
			case 2:
				{
					_height = 16;
					return 26;
				}
			default: throw new System.Exception("Inncorrect meshID");
		}
	}

	void OnDestroy()
	{
		DefaultSurface();
	}
}
