#version 430 core

#define PI 3.1415926535897932384626433832795

#extension GL_ARB_bindless_texture : require

in vec2 fragTexCoord;
in flat int instanceID;
in vec3 sunNDCCoord;
in vec3 moonNDCCoord;

in vec3 fragNormal;
in vec3 plantNormal;
out vec4 FragColor;

layout(std430, binding = 5) readonly buffer textureSSBO {
	sampler2D textures[];
};

uniform sampler2D sunDepthTexture;
uniform sampler2D moonDepthTexture;

uniform float dayProgress;

void main() {
	vec3 normal = fragNormal;
	if (gl_FrontFacing) {
		normal *= -1;
	}
	
	vec3 sunPosition = vec3(cos(2 * PI * dayProgress), sin(2 * PI * dayProgress), 0);
	sunPosition *= 10;

	sunPosition = normalize(sunPosition);
	vec3 moonPosition = -sunPosition;

	vec4 objectColor = texture(textures[instanceID], fragTexCoord);
	if (objectColor.w < 0.5) {
		discard;
	}

	float sunDiffuseFactor = dot(sunPosition, plantNormal);
	float moonDiffuseFactor = dot(moonPosition, plantNormal);

	sunDiffuseFactor = clamp(sunDiffuseFactor, 0.0, 1.0);
	moonDiffuseFactor = clamp(moonDiffuseFactor, 0.0, 1.0);

	float harshSunDiffuseFactor = dot(sunPosition, fragNormal);
	float harshMoonDiffuseFactor = dot(moonPosition, fragNormal);

	harshSunDiffuseFactor = clamp(harshSunDiffuseFactor, 0.0, 1.0);
	harshMoonDiffuseFactor = clamp(harshMoonDiffuseFactor, 0.0, 1.0);

	vec3 colorFromSun = objectColor.xyz * sunDiffuseFactor;
	vec3 colorFromMoon = objectColor.xyz * moonDiffuseFactor * vec3(0.1, 0.1, 0.4);

	// Sun shadows
	vec2 sunDepthSamplingCoord = sunNDCCoord.xy / 2 + vec2(0.5, 0.5);
	float sunLowestDepth = texture(sunDepthTexture, sunDepthSamplingCoord).x;
	float sunFragDepth = sunNDCCoord.z * 0.5 + 0.5;
		
	float sunBias = 0.005*tan(acos(harshSunDiffuseFactor));
	sunBias = clamp(sunBias, 0.0, 0.01);
	sunBias *= 10;
	if (abs(sunFragDepth - sunLowestDepth) > sunBias || dayProgress > 0.5) {
		// In shadow
		colorFromSun *= 0;
	}

	// Moon shadows
	vec2 moonDepthSamplingCoord = moonNDCCoord.xy / 2 + vec2(0.5, 0.5);
	float moonLowestDepth = texture(moonDepthTexture, moonDepthSamplingCoord).x;
	float moonFragDepth = moonNDCCoord.z * 0.5 + 0.5;
		
	float moonBias = 0.005*tan(acos(harshSunDiffuseFactor));
	moonBias = clamp(sunBias, 0.0, 0.01);
	moonBias *= 10;
	if (abs(moonFragDepth - moonLowestDepth) > moonBias || dayProgress < 0.5) {
		// In shadow
		colorFromMoon *= 0;
	}

	FragColor = vec4(colorFromSun + colorFromMoon + objectColor.xyz * 0.2, 1);
}