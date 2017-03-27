// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/colouroutlineshader"
{
	Properties
	{
		_MainTex ("Texture", 2D)= "white"{}
		_DetailTex ("DetailTexture", 2D)= "white"{}
		_TopColor ("TopColor", Color) = (0.1,0.1,0.1,0.5)
		_BottomColor ("BottomColor", Color) = (0.1,0.1,0.1,0.5)
		_HeightBlur("Height blurring value", Range(0.0, 1.0)) = 1.0
		_BaseHeight("height of gradient", Range(0.0, 1.0)) = 1.0
		_DiffuseThreshold ("Lighting Threshold", Range(0,1)) = 0.1
		
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Zwrite off
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
		
			//Tags {"LightMode"="ForwardBase"}
		
			ZWrite On // writing to the depth buffer
		
			//Blend SrcAlpha OneMinusSrcAlpha
		
			//BlendOP MIN
			//BlendOP MAX
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase 
			#include "AutoLight.cginc"
			
			#include "UnityCG.cginc"
			
			//variables
			uniform fixed4 _TopColor;
			uniform fixed4 _BottomColor;
			uniform sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;
			uniform sampler2D _DetailTex;
			uniform fixed4 _DetailTex_ST;

			//unity defined variables
			uniform half4 _LightColor0;

			uniform fixed _HeightBlur;
			uniform fixed _BaseHeight;
			uniform float _DiffuseThreshold;

			
			//base classes
			struct vertexInput
			{
				
				half4 vertex : POSITION; // position (in object coordinates, i.e. local or model coordinates
				half4 tangent : TANGENT; // vector orthogonal to the surface model
				half3 normal : NORMAL; //surface normal vector (in object coordinates; usually normalized to unit length)
				fixed4 color : COLOR; // vertex color
				fixed4 texcoord : TEXCOORD0;

			};

			struct vertexOutput
			{
				half4 pos : SV_POSITION;
				fixed4 lightDirection: TEXCOORD0;
				fixed3 viewDirection: TEXCOORD1;
				fixed3 normalDirection: TEXCOORD2;
				half2 uv : TEXCOORD3;
				half4 color : COLOR;
				
				//shadows
				LIGHTING_COORDS(4,5)
			};
			

			//vertex function, once per vertex per frame and uses struct as base
			vertexOutput vert( vertexInput v)
			{
				vertexOutput o;

				//definitions												
				half4 posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.normalDirection = normalize(mul(fixed4(v.normal, 0.0), unity_WorldToObject).xyz);
				
				o.viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
				
				float3 I = posWorld - _WorldSpaceCameraPos.xyz;
				float3 normWorld = normalize(mul(unity_ObjectToWorld, float4(v.normal,0.0)).xyz);
	
				float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
				
				o.lightDirection = half4(
								normalize(lerp(_WorldSpaceLightPos0.xyz,fragmentToLightSource,_WorldSpaceLightPos0.w)),
								lerp(1.0 ,1.0/length (fragmentToLightSource),_WorldSpaceLightPos0.w)
								);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				
				fixed4 Multiply3 = fixed4( v.texcoord.y, v.texcoord.y, v.texcoord.y, v.texcoord.y) * _HeightBlur.xxxx;
				fixed4 Add1 = Multiply3 +  _BaseHeight.xxxx;
				fixed4 Clamp0 = clamp(Add1,fixed4( 0.0, 0.0, 0.0, 0.0 ),fixed4( 1.0, 1.0, 1.0, 1.0 ));
				fixed4 Lerp0 = lerp(_BottomColor,_TopColor,Clamp0);
				
				o.color = Lerp0;
				
				//shadows			
				TRANSFER_VERTEX_TO_FRAGMENT(o);	
				
				return o;
			}
			
			//fragment function uses vertex output, once per pixel per frame
			fixed4 frag (vertexOutput i) : COLOR
			{	
				
				//shadows
				float attenuation = LIGHT_ATTENUATION(i) ;
				
				//dot product
				fixed3 nDotL = saturate(dot(i.normalDirection,i.lightDirection.xyz));
				
				//lighting ambient
				fixed3 diffuseReflection = i.lightDirection.w * _LightColor0.xyz * nDotL;
				
				//toon diffuse
				float diffuseCutoff = (saturate( ( max(_DiffuseThreshold, nDotL) - _DiffuseThreshold ) *1000 ));
				
				//final solution comment out unneeded
				fixed3 lightFinal = diffuseCutoff + UNITY_LIGHTMODEL_AMBIENT.xyz;
				
				//texture
				float4 texcol = tex2D (_MainTex, i.uv );
				
				return i.color * float4(lightFinal,1.0);
				
			}
			ENDCG
		}
		
		Pass {
		
			Tags {"LightMode"="ForwardBase"}
			
			ZWrite On // writing to the depth buffer
		
			//Blend SrcAlpha OneMinusSrcAlpha
		
			//BlendOP MIN
			//BlendOP MAX
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase 
			#include "AutoLight.cginc"
			
			#include "UnityCG.cginc"
			
			//variables
			uniform fixed4 _Color;
			uniform fixed4 _Color2;
			uniform sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;

			//unity defined variables
			uniform half4 _LightColor0;
			
			uniform float _DiffuseThreshold;

			
			//base classes
			struct vertexInput
			{
				
				half4 vertex : POSITION; // position (in object coordinates, i.e. local or model coordinates
				half4 tangent : TANGENT; // vector orthogonal to the surface model
				half3 normal : NORMAL; //surface normal vector (in object coordinates; usually normalized to unit length)
				fixed4 color : COLOR; // vertex color
				fixed4 texcoord : TEXCOORD0;

			};

			struct vertexOutput
			{
				half4 pos : SV_POSITION;
				fixed4 lightDirection: TEXCOORD0;
				fixed3 viewDirection: TEXCOORD1;
				fixed3 normalDirection: TEXCOORD2;
				half2 uv : TEXCOORD3;
				half4 color : COLOR;
				
				//shadows
				LIGHTING_COORDS(4,5)
			};
			

			//vertex function, once per vertex per frame and uses struct as base
			vertexOutput vert( vertexInput v)
			{
				vertexOutput o;

				//definitions												
				half4 posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.normalDirection = normalize(mul(fixed4(v.normal, 0.0), unity_WorldToObject).xyz);
				
				o.viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
				
				float3 I = posWorld - _WorldSpaceCameraPos.xyz;
				float3 normWorld = normalize(mul(unity_ObjectToWorld, float4(v.normal,0.0)).xyz);
	
				float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
				
				o.lightDirection = half4(
								normalize(lerp(_WorldSpaceLightPos0.xyz,fragmentToLightSource,_WorldSpaceLightPos0.w)),
								lerp(1.0 ,1.0/length (fragmentToLightSource),_WorldSpaceLightPos0.w)
								);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				
				o.color = _Color;
				
				//shadows			
				TRANSFER_VERTEX_TO_FRAGMENT(o);	
				
				return o;
			}
			
			//fragment function uses vertex output, once per pixel per frame
			fixed4 frag (vertexOutput i) : COLOR
			{	
				//shadows
				float attenuation = LIGHT_ATTENUATION(i) ;
				
				//dot product
				fixed3 nDotL = saturate(dot(i.normalDirection,i.lightDirection.xyz));
				
				//toon diffuse
				float diffuseCutoff = (saturate( ( max(_DiffuseThreshold, nDotL) - _DiffuseThreshold ) *1000 ));
				
				//final solution comment out unneeded
				fixed3 lightFinal = diffuseCutoff + UNITY_LIGHTMODEL_AMBIENT.xyz;
				
				//texture
				float4 texcol = tex2D (_MainTex, i.uv );
				
				return texcol * float4(lightFinal,1.0);
				
			}
			ENDCG
		}

		Pass {
		
			Tags {"LightMode"="ForwardBase"}
			
			ZWrite On // writing to the depth buffer
		
			//Blend SrcAlpha OneMinusSrcAlpha
		
			//BlendOP MIN
			//BlendOP MAX
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase 
			#include "AutoLight.cginc"
			
			#include "UnityCG.cginc"
			
			//variables
			uniform fixed4 _Color;
			uniform fixed4 _Color2;
			uniform sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;
			uniform sampler2D _DetailTex;
			uniform fixed4 _DetailTex_ST;

			//unity defined variables
			uniform half4 _LightColor0;
			
			uniform float _DiffuseThreshold;

			
			//base classes
			struct vertexInput
			{
				
				half4 vertex : POSITION; // position (in object coordinates, i.e. local or model coordinates
				half4 tangent : TANGENT; // vector orthogonal to the surface model
				half3 normal : NORMAL; //surface normal vector (in object coordinates; usually normalized to unit length)
				fixed4 color : COLOR; // vertex color
				fixed4 texcoord : TEXCOORD0;

			};

			struct vertexOutput
			{
				half4 pos : SV_POSITION;
				fixed4 lightDirection: TEXCOORD0;
				fixed3 viewDirection: TEXCOORD1;
				fixed3 normalDirection: TEXCOORD2;
				half2 uv : TEXCOORD3;
				half4 color : COLOR;
				
				//shadows
				LIGHTING_COORDS(4,5)
			};
			

			//vertex function, once per vertex per frame and uses struct as base
			vertexOutput vert( vertexInput v)
			{
				vertexOutput o;

				//definitions												
				half4 posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.normalDirection = normalize(mul(fixed4(v.normal, 0.0), unity_WorldToObject).xyz);
				
				o.viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
				
				float3 I = posWorld - _WorldSpaceCameraPos.xyz;
				float3 normWorld = normalize(mul(unity_ObjectToWorld, float4(v.normal,0.0)).xyz);
	
				float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
				
				o.lightDirection = half4(
								normalize(lerp(_WorldSpaceLightPos0.xyz,fragmentToLightSource,_WorldSpaceLightPos0.w)),
								lerp(1.0 ,1.0/length (fragmentToLightSource),_WorldSpaceLightPos0.w)
								);
				o.uv = TRANSFORM_TEX(v.texcoord, _DetailTex);
				
				o.color = _Color;
				
				//shadows			
				TRANSFER_VERTEX_TO_FRAGMENT(o);	
				
				return o;
			}
			
			//fragment function uses vertex output, once per pixel per frame
			fixed4 frag (vertexOutput i) : COLOR
			{	
				//shadows
				float attenuation = LIGHT_ATTENUATION(i) ;
				
				//dot product
				fixed3 nDotL = saturate(dot(i.normalDirection,i.lightDirection.xyz));
				
				//toon diffuse
				float diffuseCutoff = (saturate( ( max(_DiffuseThreshold, nDotL) - _DiffuseThreshold ) *1000 ));
				
				//final solution comment out unneeded
				fixed3 lightFinal = diffuseCutoff + UNITY_LIGHTMODEL_AMBIENT.xyz;
				
				//texture
				float4 texcol2 = tex2D (_DetailTex, i.uv );
				
				return texcol2 * float4(lightFinal,1.0);
				
			}
			ENDCG
		}
		} //SubShader
	FallBack "Diffuse" //note: required for passes: ForwardBase, ShadowCaster, ShadowCollector
}