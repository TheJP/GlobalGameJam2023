Shader "Custom/Blackout" {
  Properties {
    _MainTex("Texture", 2D) = "white" {}
    _BlackRadius("Vignette Radius", Range(0.0, 1.0)) = 1.0
    _BlackSoft("Vignette Softness", Range(0.0, 1.0)) = 0.5
  }

  SubShader {
    Pass {
      CGPROGRAM
      #pragma vertex vert_img
      #pragma fragment frag
      #include "UnityCG.cginc" // required for v2f_img

      // Properties
      sampler2D _MainTex;
      float _BlackRadius;
      float _BlackSoft;

      float4 frag(v2f_img input) : COLOR {
        float4 base = tex2D(_MainTex, input.uv);
        float distanceCentre = distance(input.uv.xy, float2(0.5, 0.5));
        float vignette = smoothstep(_BlackRadius, _BlackRadius - _BlackSoft, distanceCentre);
        return saturate(vignette * base);
      }
      ENDCG
}}}