#version 430 core

#define PI 3.1415926535897932384626433832795

#extension GL_ARB_bindless_texture : require

in vec3 cubeMapCoord;
in flat int instanceID;
out vec4 FragColor;
in vec3 fragNormal;

uniform float dayProgress;
uniform bool isCursor;

layout(std430, binding = 2) readonly buffer textureSSBO {
	samplerCube cubeMaps[];
};

void main() {
	vec3 celestialPosition = vec3(cos(2 * PI * dayProgress), sin(2 * PI * dayProgress), 0);
	celestialPosition *= 10;
	if (dayProgress > 0.5) {
		celestialPosition = -celestialPosition;
	}
	celestialPosition = normalize(celestialPosition);

	vec3 objectColor = texture(cubeMaps[instanceID], cubeMapCoord).xyz;
	float diffuseFactor = (dot(celestialPosition, fragNormal) + 1) * 0.5;
	diffuseFactor = clamp(diffuseFactor, 0.3, 1.0);

	float alpha = isCursor ? 0.5f : 1;
	vec3 emission = isCursor ? vec3(0.2) : vec3(0);
	FragColor = vec4(objectColor * diffuseFactor + emission, alpha);
}