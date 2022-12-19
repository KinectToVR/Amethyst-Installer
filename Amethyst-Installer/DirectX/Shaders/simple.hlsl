// These would be in some common functions hlsl file but we don't have a good compiler
// Thus, everything is inlined into the same file
static const float PI           = 3.14159265358979323846;
static const float TWO_PI       = PI * 2.f;

Texture2D UpgradeColorRamp : register(t0);
SamplerState UpgradeColorRamp_Sampler : register(s0);

#define glsl_mod(x,y) (((x)-(y)*floor((x)/(y))))

// Vertex input, must match vertex struct
struct vinput
{
    float4 pos              : POSITION;
    float3 posInst          : TEXCOORD0;
    float3 colorInst        : TEXCOORD1;
    float4 animDataInst     : TEXCOORD2;
    float4 animDataInst2    : TEXCOORD3;
    // uint instanceID     : SV_InstanceID; // Doesn't seem necessary?
};

struct v2f
{
    float4 pos              : SV_POSITION;
    float2 uv               : TEXCOORD0;
    float2 screenSpace      : TEXCOORD1;
    float3 color            : TEXCOORD2;
    float4 animData         : TEXCOORD3;
    float4 animData2        : TEXCOORD4;
};

cbuffer CommonDataCBuffer {
    float2 Time; // X => Elapsed Time ; Y => DeltaTime
    float2 ScreenResolution; // Screen resolution in pixels
};

// Noise functions from https://www.shadertoy.com/view/4dS3Wd
static const int NUM_NOISE_OCTAVES = 5;

float hash(float p) {
    p = frac(p * 0.011);
    p *= p + 7.5;
    p *= p + p;
    return frac(p);
}
float hash(float2 p) {
    float3 p3 = frac(float3(p.xyx) * 0.13);
    p3 += dot(p3, p3.yzx + 3.333);
    return frac((p3.x + p3.y) * p3.z);
}
float noise3D(float3 x) {
    const float3 step = float3(110, 241, 171);

    float3 i = floor(x);
    float3 f = frac(x);

    // For performance, compute the base input to a 1D hash from the integer part of the argument and the 
    // incremental change to the 1D based on the 3D -> 1D wrapping
    float n = dot(i, step);

    float3 u = f * f * (3.0 - 2.0 * f);
    return lerp(lerp(lerp(hash(n + dot(step, float3(0, 0, 0))), hash(n + dot(step, float3(1, 0, 0))), u.x),
        lerp(hash(n + dot(step, float3(0, 1, 0))), hash(n + dot(step, float3(1, 1, 0))), u.x), u.y),
        lerp(lerp(hash(n + dot(step, float3(0, 0, 1))), hash(n + dot(step, float3(1, 0, 1))), u.x),
            lerp(hash(n + dot(step, float3(0, 1, 1))), hash(n + dot(step, float3(1, 1, 1))), u.x), u.y), u.z);
}

float fbm(float3 x) {
    float v = 0.0;
    float a = 0.5;
    float3 shift = 100;
    for (int i = 0; i < NUM_NOISE_OCTAVES; ++i) {
        v += a * noise3D(x);
        x = x * 2.0 + shift;
        a *= 0.5;
    }
    return v;
}


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
    const float lifeSpan            = input.animDataInst.w;
    const float scale               = input.animDataInst.y;
    const float speed               = input.animDataInst2.x;

    const float aspectRatio = ScreenResolution.y / ScreenResolution.x;

    const float2 animDir = toCartesian(float2(animDirectionPolar, 1));
    const float timeElapsed = glsl_mod((Time.x * speed * 0.28f - animTimingOffset), lifeSpan);
    const float currentTimeInAnimation = sin(timeElapsed);

    // Transform the vertex position into projected space.
    output.pos = float4(input.pos.xyz * aspectRatio * scale + input.posInst + float3(animDir * currentTimeInAnimation, 0), 1.f);
    output.pos.x *= aspectRatio;

    // Pass attributes to the pixel shader
    output.color        = input.colorInst;
    output.uv           = input.pos.xy;
    output.screenSpace  = output.pos.xy;
    output.animData     = input.animDataInst;
    output.animData2    = float4(input.animDataInst2.x, currentTimeInAnimation, timeElapsed / lifeSpan, 0);
    
    return output;
}
float4 frag(v2f input) : SV_TARGET
{
    const float elapsedTime = saturate(Time.x / 1.f);

    const float animDirectionPolar  = input.animData.x;
    const float animTimingOffset    = input.animData.z;
    const float lifeSpan            = input.animData.w;
    const float scale               = input.animData.y;
    const float lifetime            = saturate(input.animData2.y);
    const float timeElapsed         = input.animData2.y;

    const float sampleOffset = (animTimingOffset * 0.4f + lifeSpan * 0.34f + scale * 0.025f) * 0.25f;
    const float4 gradientTex = UpgradeColorRamp.Sample(UpgradeColorRamp_Sampler, float2(input.screenSpace.x * 0.5f + 0.5f + sampleOffset, 0));

    const float2 animDir = toCartesian(float2(animDirectionPolar, 1));
    
    const float distanceToScreenEdge = saturate(length(float2(input.screenSpace.x, input.screenSpace.y)));
    const float distanceToScreenEdge2 = distanceToScreenEdge * distanceToScreenEdge;

    float3 fakeWorldSpacePos = float3(input.screenSpace * scale, dot(input.screenSpace, animDir)) * 12.4f + 20.f;
    float noise = fbm(fakeWorldSpacePos);
    const float lifeTimeFade = -4.f * lifetime * lifetime + 4.f * lifetime;
    const float lifeTimeFade2 = 1.f - lifeTimeFade * lifeTimeFade;

    const float alpha =
          (1.f - saturate(length(input.uv)))    // Edge fade per particle
        * (1.f - distanceToScreenEdge2)         // Distance to edge of framebuffer
        * elapsedTime                           // Fade in
        * saturate(noise)                       // Noise for more detail
        * gradientTex.a                         // Gradient alpha fade
        * lifeTimeFade2;                        // Particle life time fade

    float3 diffuse = gradientTex.rgb;

    // return float4(input.animData2.xyz, 1.f);
    // return float4(1, 1, 1, timeElapsed);

    return float4(alpha * diffuse, alpha);
}