Shader "Custom/ShadowOnly"
{
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        ZWrite Off
        ColorMask 0
        Lighting Off

        // ����Pass�ł͉����`�悵�Ȃ��i�����j
        Pass {}

        // �e�������󂯎�邽�߂̃p�X
        Pass
        {
            Tags { "LightMode" = "ShadowCaster" }
        }
    }
    Fallback Off
}
