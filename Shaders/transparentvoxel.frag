#version 430 core

#extension GL_ARB_bindless_texture : require

in vec3 cubeMapCoord;
in vec3 fragNormal;
out vec4 FragColor;

uniform samplerCube textureHandle;

void main() {
	vec3 lightDir = normalize(vec3(0.4, -1, 0.4)); // Angled down
	vec3 objectColor = texture(textureHandle, cubeMapCoord).xyz;
	float diffuseFactor = (dot(-lightDir, fragNormal) + 1) * 0.5;

	FragColor = vec4(objectColor * diffuseFactor, 0.5);
}