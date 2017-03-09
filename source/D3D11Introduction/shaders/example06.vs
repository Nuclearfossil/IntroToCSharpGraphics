struct VS_IN
{
    float4 pos  : POSITION;
    float4 norm : NORMAL;
    float4 col  : COLOR;
    float2 uv   : TEXCOORD;
};

struct PS_IN
{
    float4 pos  : SV_POSITION;
    float4 norm : NORMAL;
    float4 col  : COLOR;
    float2 uv   : TEXCOORD;
};

cbuffer MatrixBuffer : register(b1)
{
    matrix world;
    matrix view;
    matrix proj;
}

PS_IN VS( VS_IN input )
{
    PS_IN output = (PS_IN)0;

    matrix worldview = mul(view, proj);
    matrix viewproj = mul(proj, view);
    matrix wvp = transpose(mul(viewproj, world));

    float3x3 rotation = transpose((float3x3)worldview);

    output.col = input.col;

    output.pos = mul(input.pos, wvp);
    float3 normal = input.norm.xyz;
    normal = mul(normal, rotation);

    output.norm.xyz = normal.xyz;
    output.norm.w = 1.0f;
    output.norm = normalize(output.norm);

    output.uv = input.uv;

    return output;
}
