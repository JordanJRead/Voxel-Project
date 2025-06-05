#version 430 core
layout(location = 0) in vec3 vPos;

out flat int instanceID;

layout(std430, binding = 0) readonly buffer positionSSBO {
	float positions[];
};

layout(std430, binding = 1) readonly buffer scaleSSBO {
	float scales[];
};

uniform mat4 view;
uniform mat4 projection;

out vec3 cubeMapCoord;

void main() {
	instanceID = gl_InstanceID;

	vec3 position = vec3(positions[gl_InstanceID * 3 + 0], positions[gl_InstanceID * 3 + 1], positions[gl_InstanceID * 3 + 2]);
	vec3 scale = vec3(scales[gl_InstanceID * 3 + 0], scales[gl_InstanceID * 3 + 1], scales[gl_InstanceID * 3 + 2]);
	gl_Position = projection * view * vec4(vPos * scale + position, 1);
	instanceID = gl_InstanceID;
	cubeMapCoord = vPos;
}