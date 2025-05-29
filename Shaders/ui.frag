#version 430 core

out vec4 FragColor;

in vec2 fragTexCoord;

uniform sampler2D image;

void main() {
	FragColor = vec4(texture(image, fragTexCoord).xyz, 1);
}