// These would be in some common functions hlsl file but we don't have a good compiler
// Thus, everything is inlined into the same file
static const float PI           = 3.14159265358979323846;
static const float TWO_PI       = PI * 2.f;

// Vertex input, must match vertex struct
struct vinput
{
    float4 pos              : POSITION;
    float3 posInst          : TEXCOORD0;
    float4 colorInst        : TEXCOORD1;
    float4 animDataInst     : TEXCOORD2;
    // uint instanceID     : SV_InstanceID; // Doesn't seem necessary?
};

struct v2f
{
    float4 pos              : SV_POSITION;
    float4 color            : TEXCOORD0;
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
    const float scale               = input.animDataInst.y;

    const float2 animDir = toCartesian(float2(animDirectionPolar, 1));

    // Transform the vertex position into projected space.
    output.pos = float4(input.pos.xyz * scale + input.posInst, 1.f);

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