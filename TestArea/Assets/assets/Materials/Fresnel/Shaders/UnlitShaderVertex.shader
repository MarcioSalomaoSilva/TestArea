// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/NewUnlitShader"
{
	//makes things visible in inspector
	Properties
	{
		_Color ("Main Color", Color) = (0.1,0.1,0.1,0.5)
		_SpecColor ("Specular Color", Color) = (1.0,1.0,1.0,1.0)
		_Shininess ("Shininess",Float) = 10
		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
      	_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
	}
	SubShader
	{
		Tags {"LightMode"="ForwardBase"}
		
		ZWrite On // writing to the depth buffer
		
		//blend modes equationl
		//		SrcFactor * fragment_output + DistFactor * pixel_color_in_framebuffer;
		//blend {code for SrcFactir} {code for DistFactor}
		
		Blend SrcAlpha OneMinusSrcAlpha
		
		//BlendOP MIN
		//BlendOP MAX
		
		
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			
			//variables
			uniform fixed4 _Color;
			uniform fixed4 _SpecColor;
			uniform fixed4 _RimColor;
			uniform float _Shininess;
			uniform float _RimPower;
			
			//unity defined variables
			uniform float4 _LightColor0;
			
			//base classes
			struct vertexInput
			{
				float4 vertex : POSITION; // position (in object coordinates, i.e. local or model coordinates
				float4 tangent : TANGENT; // vector orthogonal to the surface model
				float3 normal : NORMAL; //surface normal vector (in object coordinates; usually normalized to unit length)
				//float4 texcoord : TEXCOORD0; //0th set of texture coordinates (a.k.a. "UV"; between 0 and 1)
				//float4 texcoord1 : TEXCOORD1; //0th set of texture coordinates (a.k.a. "UV"; between 0 and 1)
				fixed4 color : COLOR; // vertex color
				
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 posWorld: TEXCOORD0;
				float3 normalDir: TEXCOORD1;
				float4 color : COLOR;
				
			};
			

			//vertex function, once per vertex per frame and uses struct as base
			vertexOutput vert( vertexInput v)
			{
				vertexOutput o;
				
				//taking vertex pos multipliying it by unity matrix
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normalDir = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - o.posWorld.xyz);
				float3 normalDirection = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float atten = 1.0;
				
				//rim lighting
				float rim =  saturate(dot(normalize(viewDirection), normalDirection));
				float3 rimLighting = atten * _LightColor0.xyz * _RimColor * saturate(dot(normalDirection, lightDirection)) * pow(rim, _RimPower);
				
				//lighting
				float3 diffuseReflection = atten * _LightColor0.xyz * max(0.0, dot(normalDirection, lightDirection));
				float3 specularReflection = atten * _SpecColor.rgb * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)) * max(0.0, dot(normalDirection, lightDirection)), _Shininess);
				
				float3 lightFinal = specularReflection + rimLighting + diffuseReflection + UNITY_LIGHTMODEL_AMBIENT.xyz;
				
				//debug, uncomment to test
				//o.color = _Color;
				//o.color = v.color;
				//o.color = v.texcoord;
				//o.color = v.vertex;
				//o.color = v.vertex + float4(0.5,0.5,0.5,0.0); // add 0.5 offset if models verts go from -0.5 to 0.5
				//o.color = v.tangent;
				//o.color = float4(i.normal* 0.5 + 0.5, 1.0); // scale and bias the normal to get it in the range of 0 - 1
				//o.color = v.color; //vertex colors
				//o.color = float4(v.normal,1.0); // normals
				//o.color = float4(lightFinal * _Color,1.0); // for ambient light and picked color
				//o.color = float4(lightFinal * v.color; // for ambient light and vertex information from max
				o.color = float4(lightFinal * v.color * _Color,1.0); //vertex information multiplied by a color choosen in the inspector
				
				return o;
			}
			
			//fragment function uses vertex output, once per pixel per frame
			half4 frag (vertexOutput i) : COLOR
			{	
			
				
			
				return i.color;
			}
			
			ENDCG
		}
	}
	//fallback commented out during development incase of a bug
	//FallBack “diffuse”
}
