Shader "Custom/Greenify" {
  Properties {
    _MainTex("Texture", 2D) = "white" {}
  }

  SubShader {
    Pass {
      CGPROGRAM
      #pragma vertex vert_img
      #pragma fragment frag
      #include "UnityCG.cginc" // required for v2f_img

      // Properties
      sampler2D _MainTex;

      float4 frag(v2f_img input) : COLOR {
        float4 base = tex2D(_MainTex, input.uv);
        if (input.uv.x > 0.5) {
            return float4(0, base.g, 0, 0);
        }
        return base;
      }
      ENDCG
}}}