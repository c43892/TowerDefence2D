// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GalPanic/Sprite"
{
	Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MaskTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MaskColor ("RendererColor", Color) = (0,0,0,0)
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM

            #include "UnitySprites.cginc"

            sampler2D _MaskTex;
            float4 _MaskColor;

            fixed4 GalPanicSprite(v2f IN) : SV_Target
            {
                fixed4 ma = tex2D(_MaskTex, IN.texcoord).a;
                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;

                c.rgba = ma > 0 ? c.rgba : _MaskColor;
                return c;
            }

            #pragma vertex SpriteVert
            // #pragma fragment SpriteFrag
            #pragma fragment GalPanicSprite
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            
        ENDCG
        }
    }
}