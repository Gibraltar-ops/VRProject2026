Shader "Custom/Raymarch"
{
    // base of video - https://www.youtube.com/watch?v=8P338C9vYEE

    // for cloud rendering extend with concepts from 
    // extend with (How big budget AA games render clouds)- https://www.youtube.com/watch?v=Qj_tK_mdRcA&t=3s
    // or (Sebastian Lague - coding adventure clouds)- https://www.youtube.com/watch?v=4QOcCGI6xOU

    // Looks very promising as well 
    // ( FMX2017 Technical Directing Special: Real-time Volumetric Cloud Rendering)- https://www.youtube.com/watch?v=8OrvIQUFptA

    // - making 3D Textures - https://www.youtube.com/watch?v=hXYOlXVRRL8
    //  check sebastian launge as well

    // https://overcloud.me/ - is a cool project where he got everything working for VR as well
    // https://www.youtube.com/watch?v=8Aeyafaz7Jo
    // https://www.youtube.com/watch?v=5tHzP00qZMQ



    // where we define tweekable properties
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _MaxDistance("Max Distance",float) = 100
        _StepSize("Step size", Range(0.1, 20)) = 1
        _DensityMultiplier("Density Multipler", Range(0, 10)) = 1
        _NoiseOffset("Noise Offset", float) = 0

        _FogNoise("Fog Noise", 3D) = "white"{}
        _NoiseTiling("Noise tiling", float) = 1
        _DensityThreshold("Density threshold", Range(0, 1)) = 0.1
        
        [HDR]_LightContribution("Light Contribution", Color) = (1,1,1,1)
        _LightScattering("Light scattering", Range(0, 1)) = 0.2
    }

    // Not done to do - usally rendered at lower res and upscaled with a depth aware thechniqe like bilatteral blurring 

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment frag
            // pragma statement taken from a urp light material - to get shadow information
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            float4 _Color; // property
            float _MaxDistance; // is a property tweekable from the material
            float _DensityMultiplier; // property
            float _StepSize; // property
            float _NoiseOffset; // used for introducing noise to the start step creating less banding on high step lenghts
            TEXTURE3D(_FogNoise); // 3d nosie texture
            float _DensityThreshold;
            float _NoiseTiling;
            float4 _LightContribution; // fog tint
            float _LightScattering;

            float henyey_greenstein(float angle, float scattering)
            {
                return (1.0 - angle * angle) / (4.0 * PI * pow(1.0 + scattering * scattering - (2.0 * scattering) * angle, 1.5f));
                
            }

            // function for modifying the density in case of non linear 
            float get_density(float3 worldPos)
            {
                // this is a simple example - You can use only one channel or use a wirght vector to get more control, or look something other up
                float4 noise = _FogNoise.SampleLevel(sampler_TrilinearRepeat, worldPos * 0.01 * _NoiseTiling, 0);
                // float3 noise = _FogNoise.SampleLevel(sampler_TrilinearRepeat, worldPos * 0.01 * _NoiseTiling, 0);
                float density = dot(noise, noise); // dot product of itself
                // float density = noise.r;

                // float noise = _FogNoise.SampleLevel();
                // float density = noise;

                density = saturate(density - _DensityThreshold) * _DensityMultiplier;

                return density;

                // return _DensityMultiplier; // simple return the multiplier 
                
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord); // the original image
                float depth = SampleSceneDepth(IN.texcoord); // depth texture from renderer
                float3 worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP); // camera inverted view port projection

                float3 entryPoint = _WorldSpaceCameraPos; // current camera position
                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                float viewLength = length(viewDir);
                float3 rayDir = normalize(viewDir);


                float2 pixelCoords = IN.texcoord * _BlitTexture_TexelSize.zw; // get texture coord of the pixel
                float distLimit = min(viewLength, _MaxDistance);

                // float distTravelled = 0; // var for keeping track of how long we traveled along the ray already
                // var for keeping track of how long we traveled along the ray already - using a noise sample offset - HALF_EPS is epsilon a very small float number
                float distTravelled = InterleavedGradientNoise(pixelCoords, (int)(_Time.y / max(HALF_EPS, unity_DeltaTime.x))) * _NoiseOffset; // the noise algorithm need a time in int , we get it by currentTime/ unity_DeltaTime - gives the current Framenumber
                
                // float transmittance = 0; // Linear - acculminated transmittance += density * _StepSize 
                float transmittance = 1; // Exponential - transmittance by removing *= exp(-density * _StepSize)

                float4 fogColor = _Color;


                while (distTravelled< distLimit)
                {
                    float3 rayPos = entryPoint + rayDir * distTravelled; // current ray position in world space

                    float density = get_density(rayPos);

                    if (density > 0)
                    {
                        Light mainLight = GetMainLight(TransformWorldToShadowCoord(rayPos));
                        // fogColor.rgb += mainLight.color.rgb * _LightContribution.rgb * density * mainLight.shadowAttenuation * _StepSize; // simpler light - with basic shadow
                        fogColor.rgb += mainLight.color.rgb * _LightContribution.rgb * henyey_greenstein(dot(rayDir, mainLight.direction), _LightScattering) * density * mainLight.shadowAttenuation * _StepSize;
                        // Linear 
                        // transmittance += density * _StepSize; // Add density to transmittance, using stepsize to its consistant over multiple stepsizes
                        
                        // Exponensial 
                        transmittance *= exp(-density * _StepSize);
                    }

                    distTravelled += _StepSize;
                    
                }


                return lerp(col, fogColor, 1.0 - saturate(transmittance)); // if using exponensial fog
                // return lerp(col, _Color, 1.0 - saturate(transmittance)); // exponential fog no lightning
                // return lerp(col, _Color, saturate(transmittance)); // if using linear fog




                // return col;

                // return float4(frac(worldPos), 1);
                // return depth; // simple way just output the depth
                // return frac(LinearEyeDepth(depth, _ZBufferParams)); // zebra like output

            }
            ENDHLSL
        }
    }
}


// Simple invert fullscreen shader
// Shader "Custom/Raymarch"
// {
//     Properties
//     {

//     }

//     SubShader
//     {
//         Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

//         Pass
//         {
//             HLSLPROGRAM

//             #pragma vertex Vert
//             #pragma fragment frag

//             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//             #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"


//             half4 frag(Varyings IN) : SV_Target
//             {
//                 return 1.0 - SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);
//             }
//             ENDHLSL
//         }
//     }
// }