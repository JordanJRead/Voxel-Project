#version 430 core
layout(location = 0) in vec3 vPos;
layout(location = 1) in vec3 vNormal;

out vec3 cubeMapCoord;
out vec3 fragNormal;

uniform mat4 view;
uniform mat4 projection;
uniform vec3 position;

void main() {
	cubeMapCoord = vPos;
	gl_Position = projection * view * vec4(vPos + position, 1);
	fragNormal = normalize(vNormal);
}