Shader "Botviniev/AbstractArt" {
	Properties {
	
		_MainTex ("Main Tex", 2D) = "white" {}
		_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
		_Cube ("Reflection Cubemap", Cube) = "_Skybox" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}

		_MaxDist ("Max Dist", Float) = 7.071068
		_MaxYCoef ("Max Y Coef", Float) = 7.071068	
		
		_Scale ("Scale", Float) = 1.0
		_Speed ("Speed", Float) = 1.0
		_Frequency ("Frequency", Float) = 1.0
		
		[HideInInspector]_WaveAmplitude1 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude2 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude3 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude4 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude5 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude6 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude7 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude8 ("WaveAmplitude1", float) = 0
		[HideInInspector]_xImpact1 ("x Impact 1", float) = 0
		[HideInInspector]_zImpact1 ("z Impact 1", float) = 0
		[HideInInspector]_xImpact2 ("x Impact 2", float) = 0
		[HideInInspector]_zImpact2 ("z Impact 2", float) = 0
		[HideInInspector]_xImpact3 ("x Impact 3", float) = 0
		[HideInInspector]_zImpact3 ("z Impact 3", float) = 0
		[HideInInspector]_xImpact4 ("x Impact 4", float) = 0
		[HideInInspector]_zImpact4 ("z Impact 4", float) = 0
		[HideInInspector]_xImpact5 ("x Impact 5", float) = 0
		[HideInInspector]_zImpact5 ("z Impact 5", float) = 0
		[HideInInspector]_xImpact6 ("x Impact 6", float) = 0
		[HideInInspector]_zImpact6 ("z Impact 6", float) = 0
		[HideInInspector]_xImpact7 ("x Impact 7", float) = 0
		[HideInInspector]_zImpact7 ("z Impact 7", float) = 0	
		[HideInInspector]_xImpact8 ("x Impact 8", float) = 0
		[HideInInspector]_zImpact8 ("z Impact 8", float) = 0

		[HideInInspector]_Distance1 ("Distance1", float) = 0
		[HideInInspector]_Distance2 ("Distance2", float) = 0
		[HideInInspector]_Distance3 ("Distance3", float) = 0
		[HideInInspector]_Distance4 ("Distance4", float) = 0
		[HideInInspector]_Distance5 ("Distance5", float) = 0
		[HideInInspector]_Distance6 ("Distance6", float) = 0
		[HideInInspector]_Distance7 ("Distance7", float) = 0
		[HideInInspector]_Distance8 ("Distance8", float) = 0
	}
	
	SubShader {
	
		Tags { "RenderType"="Opaque" }
		Cull Off
	
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma exclude_renderers d3d11_9x

		uniform sampler2D _MainTex;
 		uniform fixed _MaxYCoef;
 		fixed _MaxDist;
		
		fixed _Scale;
		fixed _Speed;
		fixed _Frequency;
		
		fixed _WaveAmplitude1, _WaveAmplitude2, _WaveAmplitude3, _WaveAmplitude4, _WaveAmplitude5, _WaveAmplitude6, _WaveAmplitude7, _WaveAmplitude8;
		fixed _OffsetX1, _OffsetZ1, _OffsetX2, _OffsetZ2, _OffsetX3, _OffsetZ3,_OffsetX4, _OffsetZ4,_OffsetX5, _OffsetZ5,_OffsetX6, _OffsetZ6,_OffsetX7, _OffsetZ7,_OffsetX8, _OffsetZ8;
		fixed _Distance1, _Distance2 , _Distance3, _Distance4, _Distance5, _Distance6, _Distance7, _Distance8;
		fixed _xImpact1, _zImpact1, _xImpact2, _zImpact2,_xImpact3, _zImpact3,_xImpact4, _zImpact4,_xImpact5, _zImpact5,_xImpact6, _zImpact6,_xImpact7, _zImpact7,_xImpact8, _zImpact8;
		
		sampler2D _BumpMap;
		samplerCUBE _Cube;
		fixed4 _ReflectColor;
		
		struct appdata_full_t {
		
			fixed4 vertex    : POSITION;  // The vertex position in model space.
     		fixed3 normal    : NORMAL;    // The vertex normal in model space.
     		fixed4 texcoord  : TEXCOORD0; // The first UV coordinate.
     		fixed4 texcoord1 : TEXCOORD1; // The second UV coordinate.
     		fixed4 texcoord2 : TEXCOORD2; // The third UV coordinate.
     		fixed4 tangent   : TANGENT;   // The tangent vector in Model Space (used for normal mapping).
     		fixed4 color     : COLOR;     // Per-vertex color
		};

		struct Input {
			fixed2 uv_MainTex;
			fixed2 uv_BumpMap;
			fixed3 worldRefl;
			INTERNAL_DATA
			fixed4 color;
		};
		
	
		void vert (inout appdata_full_t v, out Input o) {
			
			fixed2 groupInitialPos = v.texcoord2;	//X, Z
			
			//Wave
			//--------------------------------------------------------------
			fixed offsetvert = ((groupInitialPos.x * groupInitialPos.x) + (groupInitialPos.y * groupInitialPos.y));
				
			fixed value1 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (groupInitialPos.x * _OffsetX1) + (groupInitialPos.y * _OffsetZ1)  );
			fixed value2 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (groupInitialPos.x * _OffsetX2) + (groupInitialPos.y * _OffsetZ2)  );
			fixed value3 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (groupInitialPos.x * _OffsetX3) + (groupInitialPos.y * _OffsetZ3)  );
			fixed value4 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (groupInitialPos.x * _OffsetX4) + (groupInitialPos.y * _OffsetZ4)  );
			fixed value5 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (groupInitialPos.x * _OffsetX5) + (groupInitialPos.y * _OffsetZ5)  );
			fixed value6 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (groupInitialPos.x * _OffsetX6) + (groupInitialPos.y * _OffsetZ6)  );
			fixed value7 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (groupInitialPos.x * _OffsetX7) + (groupInitialPos.y * _OffsetZ7)  );
			fixed value8 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (groupInitialPos.x * _OffsetX8) + (groupInitialPos.y * _OffsetZ8)  );
			
			float3 worldPos = mul(_Object2World, fixed3(groupInitialPos.x,v.vertex.y,groupInitialPos.y) ).xyz;
			
			if (sqrt(pow(worldPos.x - _xImpact1, 2) + pow(worldPos.z - _zImpact1, 2)) < _Distance1)
				v.vertex.y += value1 * _WaveAmplitude1;
			if (sqrt(pow(worldPos.x - _xImpact2, 2) + pow(worldPos.z - _zImpact2, 2)) < _Distance2)
				v.vertex.y += value2 * _WaveAmplitude2;
			if (sqrt(pow(worldPos.x - _xImpact3, 2) + pow(worldPos.z - _zImpact3, 2)) < _Distance3)
				v.vertex.y += value3 * _WaveAmplitude3;
			if (sqrt(pow(worldPos.x - _xImpact4, 2) + pow(worldPos.z - _zImpact4, 2)) < _Distance4)
				v.vertex.y += value4 * _WaveAmplitude4;
			if (sqrt(pow(worldPos.x - _xImpact5, 2) + pow(worldPos.z - _zImpact5, 2)) < _Distance5)
				v.vertex.y += value5 * _WaveAmplitude5;
			if (sqrt(pow(worldPos.x - _xImpact6, 2) + pow(worldPos.z - _zImpact6, 2)) < _Distance6)
				v.vertex.y += value6 * _WaveAmplitude6;
			if (sqrt(pow(worldPos.x - _xImpact7, 2) + pow(worldPos.z - _zImpact7, 2)) < _Distance7)
				v.vertex.y += value7 * _WaveAmplitude7;
			if (sqrt(pow(worldPos.x - _xImpact8, 2) + pow(worldPos.z - _zImpact8, 2)) < _Distance8)
				v.vertex.y += value8 * _WaveAmplitude8;
			
            //Active Mode
            //--------------------------------------------------------------
			fixed distCoef = clamp(v.texcoord1[0] - _MaxDist, -_MaxDist, 0);
			fixed groupY = distCoef*distCoef * _MaxYCoef;
			v.vertex.y += groupY;
            
            //Color
            //--------------------------------------------------------------
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.color = v.color;
		}

		void surf (Input IN, inout SurfaceOutput o) {

			o.Albedo = tex2D (_MainTex, IN.uv_MainTex) * IN.color;
			
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			
			float3 worldRefl = WorldReflectionVector (IN, o.Normal);
			fixed4 reflcol = texCUBE (_Cube, worldRefl);

			o.Emission = reflcol.rgb * _ReflectColor.rgb;
			o.Alpha = reflcol.a * _ReflectColor.a;
		}
		ENDCG
	}		
}
