using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;
[Title("Custom", "paralax")]
	public class ParalaxNode : CodeFunctionNode
{
	public ParalaxNode()
	{
		name = "paralax";
	}

	protected override MethodInfo GetFunctionToConvert()
	{
		return GetType().GetMethod("MyCustomFunction",
			BindingFlags.Static | BindingFlags.NonPublic);
	}
	static string MyCustomFunction(
		[Slot(0, Binding.None)] Vector1 heightScale,
		[Slot(1, Binding.TangentSpaceViewDirection)] Vector3 viewDir,
    [Slot(2, Binding.None)] Texture2D HeightTex,
		[Slot(3, Binding.MeshUV0)] Vector2 uv,
    [Slot(4, Binding.None)] SamplerState sampleState,
		[Slot(5, Binding.None)] out Vector2 Out)

	{
		Out = uv; 
		return
			@"
{
  // determine required number of layers
  const float minLayers = 30;
  const float maxLayers = 60;
  float numLayers = lerp(maxLayers, minLayers, abs(dot(float3(0, 0, 1), viewDir)));

  float numSteps = numLayers;//60.0f; // How many steps the UV ray tracing should take
  float height = 1.0;
  float step = 1.0 / numSteps;
  
  float2 offset = uv.xy;
  float4 HeightMap = HeightTex.Sample(sampleState, offset);
  
  float2 delta = viewDir.xy * heightScale / (viewDir.z * numSteps);
  
  // find UV offset
  for (float i = 0.0f; i < numSteps; i++) {
    if (HeightMap.r < height) {
      height -= step;
      offset += delta;
      HeightMap = HeightTex.Sample(sampleState, offset);
    } else {
      break;
    }
  }
  Out = offset;
}  
";
	}


}