Shader "Custom/FogShader"
{
    SubShader
    {
        // The rest of the code that defines the SubShader goes here.

       Pass
       {
            // Disable culling for this Pass.
            // You would typically do this for special effects, such as transparent objects or double-sided walls.
            Cull Off
            ColorMask RGBA

            // The rest of the code that defines the Pass goes here.
      }
    }
}
