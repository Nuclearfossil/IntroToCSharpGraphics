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
    matrix viewproj;
}

PS_IN VS( VS_IN input )
{
    PS_IN output = (PS_IN)0;

    matrix wvp = transpose(mul(viewproj, world));

    output.col = input.col;

    output.pos = mul(input.pos, wvp);

    output.norm = mul(input.norm, world);
    output.norm = normalize(output.norm);
    output.uv = input.uv;

    return output;
}
