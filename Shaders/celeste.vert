#version 430 core

#define PI 3.1415926535897932384626433832795

layout(location = 0) in vec3 vPos;

uniform float dayProgress;
uniform bool isSun;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 normSunPosition;

void main() {
	vec3 scale;

	if (isSun) {
		scale = vec3(100, 100, 100);
	}
	else {
		scale = vec3(50, 50, 50);
	}

	vec3 objectPosition = normSunPosition;
	objectPosition *= 1000;
	if (!isSun) {
		objectPosition = -objectPosition;
	}
	vec4 worldPos = vec4(objectPosition + vPos * scale, 1);
	gl_Position = projection * view * worldPos;
	//gl_Position = projection * view * vec4(objectPosition + vPos + vec3(0, 5, 0), 1);
}