using UnityEngine;
using System.Collections;

public class AppManager : MonoBehaviour 
{
	bool menu = true;

	//References
	//-----------------------------------
	public CPUManager cpuManager;
	public GUISkin skin;
	//-----------------------------------

	//Layouts
	//-----------------------------------
	Rect buttonRect;
	Rect fpsRect;
	Rect wavesAttenuationNameRect;
	Rect wavesAttenuationRect;
	Rect lengthOfPlumeNameRect;
	Rect lengthOfPlumeRect;
	Rect animationAppearanceNameRect;
	Rect animationAppearanceRect;
	Rect objectsNameRect;
	Rect objectsXRect;
	Rect objectsYRect;
	Rect meshesRect;
	Rect animateCameraRect;
	//-----------------------------------

	//Variables
	//-----------------------------------
	//button
	string buttonStr = "GO!";
	//tougle
	string[] meshNames = 
	{
		"Cube",
		"Tube",
		"Cola"
	};
	//fps
	int frameCount = 0;
	float dt = 0.0f;
	float fps = 0.0f;
	float updateRate = 4.0f;  // 4 updates per sec.
	//data
	string width = "";
	string heigh = "";
	int previousMeshID = 0;
	//-----------------------------------

	//Once
	//---------------------------------------------------------------

	void Awake()
	{
		Application.targetFrameRate = 60;
		Screen.fullScreen = true;

		CashLayouts();
		Reset();
	}

	void CashLayouts()
	{
		Vector2 buttonSize = new Vector3(Screen.width * 0.2f, Screen.height * 0.075f);
		int buttonsCount = 4;
		float spaceX = (Screen.width - buttonSize.x * buttonsCount) / (buttonsCount +1);
		Vector2 buttonPos = new Vector2(Screen.width - buttonSize.x - spaceX, Screen.height - buttonSize.y * 1.5f);
		
		buttonRect = new Rect(buttonPos, buttonSize);

		fpsRect = new Rect(new Vector2(spaceX, buttonPos.y), buttonSize);
		
		Vector2 nameOffsetY = new Vector2(0, 25);

		wavesAttenuationNameRect = new Rect(new Vector2(fpsRect.position.x, buttonSize.y * 0.5f), buttonSize);
		wavesAttenuationRect = wavesAttenuationNameRect;
		wavesAttenuationRect.position += nameOffsetY;

		Vector2 nameOffsetX = new Vector2(buttonSize.x + spaceX, 0);

		lengthOfPlumeNameRect = wavesAttenuationNameRect;
		lengthOfPlumeNameRect.position += nameOffsetX;
		lengthOfPlumeRect = wavesAttenuationRect;
		lengthOfPlumeRect.position += nameOffsetX;

		animationAppearanceNameRect = lengthOfPlumeNameRect;
		animationAppearanceNameRect.position += nameOffsetX;
		animationAppearanceRect = lengthOfPlumeRect;
		animationAppearanceRect.position += nameOffsetX;

		objectsNameRect = animationAppearanceNameRect;
		objectsNameRect.position += nameOffsetX;
		objectsXRect = animationAppearanceRect;
		objectsXRect.size = new Vector2(objectsXRect.size.x * 0.5f, objectsXRect.size.y);
		objectsXRect.position += nameOffsetX;
		objectsYRect = objectsXRect;
		objectsYRect.position += new Vector2(objectsXRect.size.x, 0);

		meshesRect = new Rect(
			new Vector2(spaceX, buttonRect.position.y),
			new Vector2(buttonRect.size.x * 3.0f, buttonRect.size.y) );

		animateCameraRect = wavesAttenuationRect;
		animateCameraRect.position += new Vector2(spaceX, 0);
	}

	//---------------------------------------------------------------

	void Reset()
	{
		cpuManager.Reset();
		width = cpuManager.width.ToString();
		heigh = cpuManager.height.ToString();
		Camera.main.backgroundColor = Color.blue;
	}

	void Update()
	{
		frameCount++;
		dt += Time.deltaTime;
		if (dt > 1.0f / updateRate)
		{
			fps = frameCount / dt;
			frameCount = 0;
			dt -= 1.0f / updateRate;
		}
	}

	void OnGUI()
	{
		//BUTTON
		if (GUI.Button(buttonRect, buttonStr))
		{
			menu = !menu;
			buttonStr = menu ? "GO!" : "MENU";
			SetUpScene(!menu);
		}
		if (menu)
		{
			//WavesAttenuation
			GUI.Label(wavesAttenuationNameRect, "Waves Attenuation", skin.label);
			cpuManager.wavesAttenuation = 1.0f - GUI.HorizontalSlider(
				wavesAttenuationRect, 1.0f - cpuManager.wavesAttenuation, 0.0f, 1.0f);

			//LengthOfPlumeRect
			GUI.Label(lengthOfPlumeNameRect, "Length of Plume", skin.label);
			cpuManager.lenghtOfPlume = GUI.HorizontalSlider(
				lengthOfPlumeRect, cpuManager.lenghtOfPlume, 0.0f, 100.0f);

			//AnimationAppearance
			GUI.Label(animationAppearanceNameRect, "Animation Appearance", skin.label);
			cpuManager.moveSpeed = GUI.HorizontalSlider(
				animationAppearanceRect, cpuManager.moveSpeed, 0.0001f, 0.01f);

			//Objects Rect
			width = GUI.TextField(objectsXRect, width);
			heigh = GUI.TextField(objectsYRect, heigh);
			GUI.Label(objectsNameRect, "Objects: " + width + " X " + heigh, skin.label);

			//MeshID
			cpuManager.meshID = GUI.Toolbar(meshesRect, cpuManager.meshID, meshNames);
			if (cpuManager.meshID != previousMeshID)
			{
				uint w;
				uint h;
				w = cpuManager.DeafultValues(cpuManager.meshID, out h);
				width = w.ToString();
				heigh = h.ToString();
				previousMeshID = cpuManager.meshID;
			}
		}
		else
		{
			//Animate Camera
			GUI.Box(wavesAttenuationRect, "");
			cpuManager.animateCamera = GUI.Toggle(
				animateCameraRect, cpuManager.animateCamera, "Animate Camera", skin.toggle);

			//FPS
			GUI.Box(fpsRect, "fps: " + fps, skin.box);
		}
	}

	void SetUpScene( bool demo )
	{
		cpuManager.enabled = demo;

		if (demo)
		{
			cpuManager.SetUp(width, heigh);
			cpuManager.Init();
		}
		else //if(menu)
		{
			Reset();
		}
	}

	public void Delete(Object obj)
	{
		Destroy(obj);
	}
}
