#version 430 core

#define PI 3.1415926535897932384626433832795

layout(location = 0) in vec3 vPos;

uniform float dayProgress;
uniform bool isSun;
uniform mat4 view;
uniform mat4 projection;

void main() {
	vec3 scale;

	if (isSun) {
		scale = vec3(1000, 1000, 1000);
	}
	else {
		scale = vec3(500, 500, 500);
	}

	vec3 objectPosition = vec3(cos(2 * PI * dayProgress), sin(2 * PI * dayProgress), 0);
	objectPosition *= 10000;
	if (!isSun) {
		objectPosition = -objectPosition;
	}
	vec4 worldPos = vec4(objectPosition + vPos * scale, 1);
	gl_Position = projection * view * worldPos;
	//gl_Position = projection * view * vec4(objectPosition + vPos + vec3(0, 5, 0), 1);
}