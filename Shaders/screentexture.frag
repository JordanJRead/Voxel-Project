#version 430 core

in vec2 fragTexCoord;
out vec4 FragColor;

uniform sampler2D image;

void main() {
	vec4 color = vec4(texture(image, fragTexCoord).xyz, 1);
	FragColor = color;
}