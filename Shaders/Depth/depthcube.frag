#version 430 core

#extension GL_ARB_bindless_texture : require
layout(std430, binding = 2) readonly buffer textureSSBO {
	samplerCube textures[];
};
in vec3 cubeMapCoord;
in flat int instanceID;

void main() {
	if (texture(textures[instanceID], cubeMapCoord).w < 0.5) {
		discard;
	}
}