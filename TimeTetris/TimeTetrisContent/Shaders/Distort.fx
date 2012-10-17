extern float4x4 Projection;

uniform extern texture ScreenTexture;
uniform extern float time;

sampler ScreenSampler = sampler_state
{
	Texture = <ScreenTexture>;
	MipFilter = None;
};

// Input: It uses texture coords as the random number seed.
// Output: Random number: [0,1), that is between 0.0 and 0.999999... inclusive.
// Author: Michael Pohoreski
// Copyright: Copyleft 2012 :-)

float random( float2 p )
{
  // We need irrationals for pseudo randomness.
  // Most (all?) known transcendental numbers will (generally) work.
  const float2 r = float2(
    23.1406926327792690,  // e^pi (Gelfond's constant)
     2.6651441426902251); // 2^sqrt(2) (Gelfond–Schneider constant)
  return frac( cos( fmod( 123456789., 1e-7 + 256. * dot(p,r) ) ) );  
}

void SpriteVertexShader(inout float4 color    : COLOR0,
						inout float2 texCoord : TEXCOORD0,
						inout float4 position : POSITION0)
{
	position = mul(position, Projection);
}

float4 PixelDraw(float4 color : COLOR0, float2 texCoord: TEXCOORD0) : COLOR
{
		// Monitor distort
	float2 pos = texCoord - 0.5;
	pos.x *= (1 + pow(abs(texCoord.y - 0.5),3.6));
	pos.y *= (1 + pow(abs(texCoord.x - 0.5),3.6));
	pos += 0.5;

	// Original color
	float4 ret = tex2D(ScreenSampler, pos);
	
	// Glass reflection
	float offset = 0.004;
	float4 up = tex2D(ScreenSampler, pos + float2(offset,0));
	float4 down = tex2D(ScreenSampler, pos - float2(offset,0));
	float4 left = tex2D(ScreenSampler, pos + float2(0,offset));
	float4 right = tex2D(ScreenSampler, pos - float2(0,offset));
	float4 avg = (up + down + left + right) / 4;
	ret += avg / 2;

	// Light line
	float4 light = 0;
	for(int i = 0; i < 30; i++)
		light += tex2D(ScreenSampler, pos - i * float2(offset / 2, 0));
	float total = (light.x + light.y + light.z) / 90;

	// Clip if outside
	if(pos.x < 0)
		discard;
	if(pos.x > 1)
		discard;
	if(pos.y < 0)
		discard;
	if(pos.y > 1)
		discard;

	// Plus add random noise (time based)
	return ret + 0.05 * random(pos * (10 + 3.1425 * sin(frac(time / 100.0)))) + total / 10;

}

technique Draw
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 PixelDraw();
	}
}