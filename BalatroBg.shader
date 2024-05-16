//创建于0326参考小丑牌背景background.fs
Shader "GrapeCore/BalatroBg" 
{
    Properties 
    {
        [PerRendererData]_MainTex("MainTex",2D)="white"{}
        [Header(ScreenRatio)]
        _screenRatio("Screen RatioXY",vector) = (1,0.5625,0,0)
        [Header(ColorControl)]
        _color1("Color1",Color) = (0.0,0.6159,1,1)
        _color2("Color2",Color) = (1,1,1,1)
        _color3("Black Color3",Color) = (0.3098,0.3882,0.4039,1)
        [Header(Transition)]
        _vortSpeed("Vort Speed",float) = 1
        _vortScale("Vort Scale",float) = 30
        _vortOffset("Vort Offset",float) = 0
        _midFlash("Mid Flash",float) = 0
        [Header(Pixel)]
        [Toggle]_PIXELIT("Pixel It",int) = 0
        _pixelSize("Pixel Size",float) = 1
        
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        Cull Off 
        // ZWrite Off
        Pass{
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag     

            #pragma shader_feature _ _PIXELIT_ON
            // ----Includes--------------------------------------
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"          
            // --------------------------------------------------

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionCS   : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _color1,_color2,_color3;
                float4 _screenRatio;
                float _midFlash,_vortOffset,_vortSpeed,_vortScale;
                float _pixelSize;
            CBUFFER_END

            TEXTURE2D(_MainTex);SAMPLER(sampler_MainTex);
            // #define PIXEL_SIZE_FAC 700.
            // #define SPIN_EASE 0.5
            #define T _Time.y
            

            half4 PaintEffect(half2 screen_coords )
            {
                //---Convert to UV coords (0-1) and floor for pixel effect
                // half loveScreen_Len = length(_loveScreenSize.xy);
                //越往中心，像素化大小越小，近大远小很合理
                // half pixel_size = loveScreen_Len/PIXEL_SIZE_FAC;
                // half2 uv = (floor(screen_coords.xy*(1./pixel_size))*pixel_size - 0.5*_loveScreenSize.xy)/loveScreen_Len;
                //去像素化
                // half2 uv = (screen_coords.xy- 0.5*_loveScreenSize.xy)/loveScreen_Len;
                half2 uv = (screen_coords-0.5)*_screenRatio;
                //是否像素化
                #if _PIXELIT_ON
                half2 pixelSize = max(0.001,_pixelSize)/_ScreenParams;
                uv = floor(uv/pixelSize)*pixelSize;
                #endif
                // return half4(uv,0,1);

                half uv_len = length(uv);

                //---Adding in a center swirl, changes with T
                half speed = T*_vortSpeed;
                half new_pixel_angle = atan2(uv.y, -uv.x)
                                        + (2.2 + 0.4*min(6.,speed))*uv_len //差异化的角度，旋转
                                        // - 1 
                                        // - speed*0.05 
                                        - min(6.,speed)*speed*0.02 + _vortOffset;
                // half2 mid = (_loveScreenSize.xy/loveScreen_Len)/2;
                // half2 sv = half2((uv_len * cos(new_pixel_angle) + mid.x), (uv_len * sin(new_pixel_angle) + mid.y)) - mid;
                half2 sv = half2((uv_len * cos(new_pixel_angle)), (uv_len * sin(new_pixel_angle)));
                // return half4(sv,0,1);
                // return sv.x;
                //---Now add the smoke effect to the swirled UV
                sv *= _vortScale;
                speed = T*6*_vortSpeed + _vortOffset ;
                half2 uv2 = sv.x+sv.y;
                //---扭曲吧
                for(int i=0; i < 5; i++) {
                    uv2 += sin(max(sv.x, sv.y)) + sv;
                    sv  += 0.5*half2(cos(2.1123314 + 0.353*uv2.y + speed*0.131121),sin(uv2.x - 0.113*speed));
                    sv  -= 1.0*cos(sv.x + sv.y) - 1.0*sin(sv.x*0.711 - sv.y);
                }
                // return half4(sv,0,1);
                // ---Make the smoke amount range from 0 - 2
                half smoke_res =
                min(2, 
                    max(-2, 
                        // 1.5 + length(sv)*0.12 - 0.17*(min(10,T*1.2 - 4))
                         1.5 + length(sv)*0.12 - 0.17*(min(10,T*3.2 - 4))
                        )
                );
                if (smoke_res < 0.2) {
                    smoke_res = (smoke_res - 0.2)*0.6 + 0.2;
                }
                half c1p = max(0.,1. - 2.*abs(1.-smoke_res));
                half c2p = max(0.,1. - 2.*(smoke_res));
                half cb = 1. - min(1., c1p + c2p);
                half4 ret_col = _color1*c1p + _color2*c2p + half4(cb*_color3.rgb*0.6, cb*_color1.a);
                // return ret_col;
                //用来计算转场时候的白光flash
                half mod_flash = max(_midFlash*0.8, max(c1p, c2p)*5. - 4.4) + _midFlash*max(c1p, c2p);
                return lerp(ret_col,1,mod_flash);
            }

            Varyings vert(Attributes v)
            {
                Varyings o ;
                o.uv = v.uv;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                return o;
            }
            half4 frag(Varyings i) : SV_TARGET 
            {
                half2 ScreenUv = i.positionCS.xy/_ScreenParams.xy;
                half4 finalCol = PaintEffect(ScreenUv);
                return finalCol;
            }

            ENDHLSL
        }
    }
}
