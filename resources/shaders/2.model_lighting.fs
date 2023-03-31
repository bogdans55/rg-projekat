#version 330 core
out vec4 FragColor;

struct PointLight {
    vec3 position;

    vec3 specular;
    vec3 diffuse;
    vec3 ambient;

    float constant;
    float linear;
    float quadratic;
};
struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
struct SpotLight {
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct Material {
    sampler2D diffuse;
    sampler2D specular;

    float shininess;
};
in vec2 TexCoords;
in vec3 Normal;
in vec3 FragPos;


uniform DirLight dirLight;
uniform SpotLight spotLight1;
uniform SpotLight spotLight2;
uniform PointLight pointLight1;
uniform PointLight pointLight2;
uniform PointLight pointLight3;
uniform PointLight pointLight4;
uniform Material material;

uniform vec3 viewPosition;
// calculates the color when using a point light.
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    // combine results
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords).xxx);
    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // combine results
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    return (ambient + diffuse + specular);
}

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
 {
     vec3 lightDir = normalize(light.position - fragPos);
     // diffuse shading
     float diff = max(dot(normal, lightDir), 0.0);
     // specular shading
     vec3 reflectDir = reflect(-lightDir, normal);
     float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
     // attenuation
     float distance = length(light.position - fragPos);
     float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
     // spotlight intensity
     float theta = dot(lightDir, normalize(-light.direction));
     float epsilon = light.cutOff - light.outerCutOff;
     float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
     // combine results
     vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
     vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
     vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
     ambient *= attenuation * intensity;
     diffuse *= attenuation * intensity;
     specular *= attenuation * intensity;
     return (ambient + diffuse + specular);
 }

void main()
{
    vec3 normal = normalize(Normal);
    vec3 viewDir = normalize(viewPosition - FragPos);
    vec3 result =  CalcDirLight(dirLight, normal, viewDir)
                +  CalcSpotLight(spotLight1, normal, FragPos, viewDir)
                +  CalcSpotLight(spotLight2, normal, FragPos, viewDir);
//                 +  CalcPointLight(pointLight1, normal, FragPos, viewDir)
//                 +  CalcPointLight(pointLight2, normal, FragPos, viewDir)
//                 +  CalcPointLight(pointLight3, normal, FragPos, viewDir)
//                 +  CalcPointLight(pointLight4, normal, FragPos, viewDir);
    FragColor = vec4(result, 1.0);
}