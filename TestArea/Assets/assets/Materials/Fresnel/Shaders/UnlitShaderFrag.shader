// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/UnlitShaderFrag"
{
	//makes things visible in inspector
	Properties
	{
		//_MainTex ("Gradient Texture", 2D)= "white"{}
		_Color ("Color Multiplier", Color) = (0.1,0.1,0.1,0.5)
		_TopColor("Top Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (0,0,0,1)
		_HeightBlur("Height blurring value", Range(0.0, 1.0)) = 1.0
		_BaseHeight("height of gradient", Range(0.0, 1.0)) = 1.0
		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_TopRimBlur ("Top rim blurring", Range(0.0, 1.0)) = 1.0
		_TopRimOffset("Top Rim Offset ", Range(0.0, 1.0)) = 1.0
      	//_FresnelColor ("Fresnal Color", Color) = (1,1,1,1)
      	//_FresnalScale("Fresnel Scale", Range(0.0, 1.0)) = 1.0
      	//_FresnelHeight("Fresneal Height",  Range(0.0, 1.0)) = 1.0
		//_SpecColor ("Specular Color", Color) = (1.0,1.0,1.0,1.0) 
		//_Shininess ("Shininess",Range(0.0, 1.0)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		//LOD 200
		
		Pass {
		
			Tags {"LightMode"="ForwardBase"
			}
		
			ZWrite On // writing to the depth buffer
		
			//blend modes equationl
			//		SrcFactor * fragment_output + DistFactor * pixel_color_in_framebuffer;
			//blend {code for SrcFactir} {code for DistFactor}
		
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
			uniform fixed _HeightBlur;
			uniform fixed _BaseHeight;
			uniform fixed4 _TopColor;
			uniform fixed4 _BottomColor;
			uniform fixed4 _SpecColor;
			uniform fixed4 _RimColor;
			uniform half _Shininess;
			uniform half _RimPower;
			uniform fixed _TopRimOffset;
			uniform fixed _TopRimBlur;
			uniform fixed _TopRimHeight;
			uniform fixed _FresnelColor;
			uniform fixed _FresnelScale;
			uniform fixed _FresnelHeight;
			//uniform sampler2D _MainTex;
			
			
			//unity defined variables
			uniform half4 _LightColor0;
			
			//base classes
			struct vertexInput
			{
				
				half4 vertex : POSITION; // position (in object coordinates, i.e. local or model coordinates
				half4 tangent : TANGENT; // vector orthogonal to the surface model
				half3 normal : NORMAL; //surface normal vector (in object coordinates; usually normalized to unit length)
				fixed4 color : COLOR; // vertex color
				fixed4 texcoord : TEXCOORD0;
				//fixed4 lightDirection: TEXCOORD0;
				//fixed4 viewDirection: TEXCOORD0;
				//fixed4 normalDirection: TEXCOORD0;
				//fixed4 refFactor : TEXCOORD0;
			};

			struct vertexOutput
			{
				half4 pos : SV_POSITION;
				fixed4 lightDirection: TEXCOORD0;
				fixed3 viewDirection: TEXCOORD1;
				fixed3 normalDirection: TEXCOORD2;
				fixed4 tex : TEXCOORD3;
				fixed4 refFactor : TEXCOORD4;
				half4 color : COLOR;
				
				//shadows
				LIGHTING_COORDS(5,6)
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
				
				o.refFactor = _FresnelScale * pow(1.0 + dot(normalize(I),normWorld), 1.4);
				
				o.lightDirection = half4(
								normalize(lerp(_WorldSpaceLightPos0.xyz,fragmentToLightSource,_WorldSpaceLightPos0.w)),
								lerp(1.0 ,1.0/length (fragmentToLightSource),_WorldSpaceLightPos0.w)
								);
				
				//lerps
				o.tex = v.texcoord; //TRANSFORM_TEX(v.tex, _MainTex);
				
				fixed4 Multiply3 = fixed4( v.texcoord.y, v.texcoord.y, v.texcoord.y, v.texcoord.y) * _HeightBlur.xxxx;
				fixed4 Add1 = Multiply3 +  _BaseHeight.xxxx;
				fixed4 Clamp0 = clamp(Add1,fixed4( 0.0, 0.0, 0.0, 0.0 ),fixed4( 1.0, 1.0, 1.0, 1.0 ));
				fixed4 Lerp0 = lerp(_BottomColor,_TopColor,Clamp0);
				fixed4 Multiply0 = fixed4( v.texcoord.y, v.texcoord.y, v.texcoord.y, v.texcoord.y) * _TopRimBlur.xxxx;
				fixed4 Add2=Multiply0 + _TopRimOffset.xxxx;
//				fixed4 Fresnel0_1_NoInput = fixed4(0,0,1,1);
//				fixed4 Fresnel0 = (1.0 - dot( normalize( fixed4( o.viewDirection.x, o.viewDirection.y,o.viewDirection.z,1.0 ).xyz), normalize( Fresnel0_1_NoInput.xyz ) )).xxxx;
//				fixed4 Multiply1 = Add2 * Fresnel0;
				fixed4 Clamp1 = clamp(Multiply0,fixed4( 0.0, 0.0, 0.0, 0.0 ),fixed4( 1.0, 1.0, 1.0, 1.0 ));
				fixed4 Lerp1 = lerp(Lerp0,_RimColor,Clamp1);
				fixed4 Multiply2 = Lerp1 ; // or just color tint or just use above  _Color * v.color
				
				o.color = Multiply2 * v.color*_Color*3; //multiply + Lerp0
				
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
				
				//rim lighting
				fixed rim = 1- nDotL;
				fixed3 rimLighting = nDotL * _RimColor.xyz *_LightColor0.xyz * pow(rim, (_RimPower));
				
				//lighting ambient
				fixed3 diffuseReflection = i.lightDirection.w * _LightColor0.xyz * nDotL;
				
				//lighting specular
				fixed3 specularReflection = diffuseReflection * _LightColor0.xyz * _SpecColor.rgb * pow(max(0.0,dot(reflect(-i.lightDirection, i.normalDirection), i.viewDirection)), (_Shininess));
				
				//final solution comment out unneeded
				fixed3 lightFinal = rimLighting +specularReflection + diffuseReflection + UNITY_LIGHTMODEL_AMBIENT.xyz;
			
				
				return fixed4 (i.color  ) ;
			
				//previous complete
				//return fixed3 (i.color * attenuation * (diffuseReflection+ UNITY_LIGHTMODEL_AMBIENT.xyz)*2) ; // ( i.color * (diffuseReflection+ UNITY_LIGHTMODEL_AMBIENT.xyz)*0.9) + (specularReflection)  + rimLighting
				//return tex2D (_MainTex, i.tex);
				//return fixed4 (i.color * attenuation * (diffuseReflection + UNITY_LIGHTMODEL_AMBIENT.xyz),1.0);//lerp(i.color,_FresnelColor,i.refFactor); 
				
			}
			ENDCG
		}
		} //SubShader
	FallBack "Diffuse" //note: required for passes: ForwardBase, ShadowCaster, ShadowCollector
}