#version 430 core

uniform bool isSun;

out vec4 FragColor;

void main() {
	if (isSun) {
		FragColor = vec4(1, 1, 0.8, 1);
	}
	else {
		FragColor = vec4(0.5, 0.5, 0.8, 1);
	}
}