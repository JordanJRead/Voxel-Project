#version 430 core

#define PI 3.1415926535897932384626433832795

#extension GL_ARB_bindless_texture : require

in vec3 cubeMapCoord;
in flat int instanceID;
out vec4 FragColor;
in vec3 fragNormal;

in vec3 sunNDCCoord;
in vec3 moonNDCCoord;

uniform float dayProgress;
uniform float dayStrength;
uniform bool isCursor;
uniform bool isCloud;

uniform sampler2D sunDepthTexture;
uniform sampler2D moonDepthTexture;

layout(std430, binding = 2) readonly buffer textureSSBO {
	samplerCube cubeMaps[];
};

void main() {
	if (isCloud) {
        float dayStrength = sin(dayProgress * 2 * PI) / 2.0 + 0.5;
		vec4 brightColor = vec4(0.8, 0.8, 0.8, 1);
		vec4 darkColor = vec4(brightColor.xyz * 0.3, 1);
		FragColor = darkColor + dayStrength * (brightColor - darkColor);
	}
	else {
		vec3 sunPosition = vec3(cos(2 * PI * dayProgress), sin(2 * PI * dayProgress), 0);
		sunPosition *= 10;
		sunPosition = normalize(sunPosition);

		vec3 moonPosition = -sunPosition;

		vec4 totalObjectColor = texture(cubeMaps[instanceID], cubeMapCoord);
		vec3 objectColor = totalObjectColor.xyz;

		if (totalObjectColor.w < 0.5) {
			discard;
		}

		float sunDiffuseFactor = dot(sunPosition, fragNormal);
		float moonDiffuseFactor = dot(moonPosition, fragNormal);

		sunDiffuseFactor = clamp(sunDiffuseFactor, 0.0, 1.0);
		moonDiffuseFactor = clamp(moonDiffuseFactor, 0.0, 1.0);

		vec3 colorFromSun = objectColor * sunDiffuseFactor;
		vec3 colorFromMoon = objectColor * moonDiffuseFactor * vec3(0.1, 0.1, 0.4);

		// Sun shadows
		vec2 sunDepthSamplingCoord = sunNDCCoord.xy / 2 + vec2(0.5, 0.5);
		float lowestSunDepth = texture(sunDepthTexture, sunDepthSamplingCoord).x;
		float sunFragDepth = sunNDCCoord.z * 0.5 + 0.5;
		
		float sunBias = 0.005*tan(acos(sunDiffuseFactor));
		sunBias = clamp(sunBias, 0.000001, 0.01);
		sunBias *= 0.1;

		if (abs(sunFragDepth - lowestSunDepth) > sunBias) {
			colorFromSun *= 0;
		}
		
		// Moon shadows
		vec2 moonDepthSamplingCoord = moonNDCCoord.xy / 2 + vec2(0.5, 0.5);
		float lowestMoonDepth = texture(moonDepthTexture, moonDepthSamplingCoord).x;
		float moonFragDepth = moonNDCCoord.z * 0.5 + 0.5;
		
		float moonBias = 0.005*tan(acos(moonDiffuseFactor));
		moonBias = clamp(moonBias, 0.0000001, 0.01);
		moonBias *= 0.1;
		if (abs(moonFragDepth - lowestMoonDepth) > moonBias) {
			colorFromMoon *= 0;
		}

		// Final color
		float alpha = isCursor ? 0.5f : 1;
		vec3 addition = isCursor ? vec3(0.5) : vec3(0);
		float emissionScale = 0.2;// + dayStrength * 0.3;

		FragColor = vec4(colorFromSun + colorFromMoon + objectColor * emissionScale + addition, alpha);
		//FragColor = vec4(lowestMoonDepth, lowestMoonDepth, lowestMoonDepth, 1);
	}
}