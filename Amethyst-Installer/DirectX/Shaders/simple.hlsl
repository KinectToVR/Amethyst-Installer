cbuffer ModelViewProjectionConstantBuffer : register(b0)
{
    matrix model;
    matrix view;
    matrix projection;
};

struct vinput
{
    float3 pos : POSITION;
    float3 color : COLOR0;
};

struct v2f
{
    float3 color : COLOR0;
    float4 pos : SV_POSITION;
};

v2f vert(vinput input)
{
    v2f output;
    float4 pos = float4(input.pos, 1.0f);

    // Transform the vertex position into projected space.
    pos = mul(pos, model);
    pos = mul(pos, view);
    pos = mul(pos, projection);
    output.pos = pos;

    // Pass through the color without modification.
    output.color = input.color;

    return output;
}

float4 frag(v2f input) : SV_TARGET
{
    // TODO: Read UV from v2f
    float2 uv = 0;

    return float4(input.color,1.0f);
}