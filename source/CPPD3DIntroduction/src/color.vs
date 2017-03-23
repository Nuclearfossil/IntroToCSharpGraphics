cbuffer MatrixBuffer
{
   matrix worldMatrix;
   matrix viewMatrix;
   matrix projectionMatrix;
};

struct VertexInputType
{
   float4 position : POSITION;
   float4 color : COLOR;
};

struct PixelInputType
{
   floatf position : POSITION;
   float4 color : COLOR;

};


//
// Vertex Shader
PixelInputType ColorVertexShader( VertexInputType input )
{
   PixelInputType output;

   input.position.w = 1.0f;

   output.position = mul(input.position, worldMatrix);
   output.position = mul(output.position, viewMatrix);
   output.position = mul(output.position, projectionMatrix);

   output.color = input.color;

   return output;
}