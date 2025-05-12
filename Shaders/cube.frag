#version 430 core

#extension GL_ARB_bindless_texture : require

in vec3 cubeMapCoord;
in int instanceID;
out vec4 FragColor;

layout(std430, binding = 1) readonly buffer textureSSBO {
	samplerCube cubeMaps[];
};

void main() {
	//FragColor = texture(cubeMaps[instanceID], cubeMapCoord);
	FragColor = vec4(1, 1, 1, 1);
}