struct vinput
{
    float4 pos : POSITION;
    float4 color : COLOR0;
};

struct v2f
{
    float4 pos : SV_POSITION;
    float4 color : COLOR0;
};

v2f vert(vinput input)
{
    v2f output = (v2f)0;
    
    // Transform the vertex position into projected space.
    output.pos = input.pos;

    // Pass through the color without modification.
    output.color = input.color;
    
    return output;
}

float4 frag(v2f input) : SV_TARGET
{
    // TODO: Read UV from v2f
    float2 uv = 0;

    return input.color;
    // return float4(input.color, 1.0f);
}