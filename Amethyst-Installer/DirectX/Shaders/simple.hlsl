static const float PI           = 3.14159265358979323846;
static const float TWO_PI       = PI * 2.f;

// Vertex input, must match vertex struct
struct vinput
{
    // float4 pos              : Position;
    float3 posInst          : INST_POSITION;
    float4 colorInst        : INST_COLOR0;
    float4 animDataInst     : INST_ANIMDATA;
    // uint instanceID     : SV_InstanceID;
};

struct v2f
{
    float4 pos : INST_POS;
    float4 color : INST_COLOR;
};

float2 toCartesian(float2 polar){
    float2 cartesian;
    sincos(polar.x * TWO_PI, cartesian.y, cartesian.x);
    return cartesian * polar.y;
}

v2f vert(vinput input)
{
    v2f output = (v2f)0;

    const float animDirectionPolar  = input.animDataInst.x;
    const float animTimingOffset    = input.animDataInst.z;
    const float animTimingPeriod    = input.animDataInst.w;
    const float animUnused          = input.animDataInst.y;

    const float2 animDir = toCartesian(float2(animDirectionPolar, 1));
    
    // Transform the vertex position into projected space.
    output.pos = float4(input.posInst, 1.f);
    // output.pos.xy *= 0.f;

    // Pass through the color without modification.
    output.color = input.colorInst;
    
    return output;
}

float4 frag(v2f input) : SV_TARGET
{
    // TODO: Read UV from v2f
    float2 uv = 0;

    return input.color;
    // return float4(input.color, 1.0f);
}