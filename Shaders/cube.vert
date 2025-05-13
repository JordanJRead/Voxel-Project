#version 430 core
layout(location = 0) in vec3 vPos;
layout(location = 1) in vec3 vNormal;

out vec3 cubeMapCoord;
out flat int instanceID;
out vec3 fragNormal;

layout(std430, binding = 0) readonly buffer positionSSBO {
	float cubePositions[];
};

uniform mat4 view;
uniform mat4 projection;

void main() {
	cubeMapCoord = vPos;
	instanceID = gl_InstanceID;

	vec3 cubePosition = vec3(cubePositions[gl_InstanceID * 3 + 0], cubePositions[gl_InstanceID * 3 + 1], cubePositions[gl_InstanceID * 3 + 2]);
	gl_Position = projection * view * vec4(vPos + cubePosition, 1);
	fragNormal = normalize(vNormal);
}