using UnityEngine.Rendering.Universal;
using UnityEngine;

public class VisionManager : MonoBehaviour
{
    [Header("Light Sources")]
    public Light2D closeVisionLight;    // Маленький круг вокруг игрока
    public Light2D coneVisionLight;      // Конус света перед игроком

    [Header("Settings")]
    public bool debugMode = true;

    private Transform playerTransform;

    void Start()
    {
        if (closeVisionLight == null || coneVisionLight == null)
        {
            Debug.LogError("VisionManager: Please assign both lights!");
        }

        playerTransform = transform;
    }

    void Update()
    {
        if (closeVisionLight == null || coneVisionLight == null) return;

        UpdateShaderParameters();

        if (debugMode)
        {
            Debug.Log($"Close Light - Pos: {closeVisionLight.transform.position}, Radius: {closeVisionLight.pointLightOuterRadius}, Intensity: {closeVisionLight.intensity}");
            Debug.Log($"Cone Light - Pos: {coneVisionLight.transform.position}, Angle: {coneVisionLight.pointLightOuterAngle}, Length: {coneVisionLight.pointLightOuterRadius}, Intensity: {coneVisionLight.intensity}");
        }
    }

    void UpdateShaderParameters()
    {
        // Передаем позицию игрока
        Shader.SetGlobalVector("_PlayerPosition", playerTransform.position);

        // Параметры ближнего света (круг)
        Shader.SetGlobalVector("_CloseLightPosition", closeVisionLight.transform.position);
        Shader.SetGlobalFloat("_CloseLightRadius", closeVisionLight.pointLightOuterRadius);
        Shader.SetGlobalFloat("_CloseLightIntensity", closeVisionLight.intensity);

        // Параметры конуса света
        Shader.SetGlobalVector("_ConeLightPosition", coneVisionLight.transform.position);

        // Направление зависит от того, как повернут свет
        Vector3 forwardDirection = coneVisionLight.transform.up;
        Shader.SetGlobalVector("_ConeLightDirection", forwardDirection);

        Shader.SetGlobalFloat("_ConeLightLength", coneVisionLight.pointLightOuterRadius);
        Shader.SetGlobalFloat("_ConeLightAngle", coneVisionLight.pointLightOuterAngle);
        Shader.SetGlobalFloat("_ConeLightIntensity", coneVisionLight.intensity);
    }

    // Визуализация зон в редакторе
    void OnDrawGizmos()
    {
        if (!debugMode) return;

        if (closeVisionLight != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawWireSphere(closeVisionLight.transform.position, closeVisionLight.pointLightOuterRadius);
        }

        if (coneVisionLight != null)
        {
            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Vector3 forward = coneVisionLight.transform.up;
            float angle = coneVisionLight.pointLightOuterAngle;
            float length = coneVisionLight.pointLightOuterRadius;
            float halfAngle = angle / 2;

            Vector3 leftBoundary = Quaternion.Euler(0, 0, halfAngle) * forward;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, -halfAngle) * forward;

            Gizmos.DrawLine(coneVisionLight.transform.position, coneVisionLight.transform.position + leftBoundary * length);
            Gizmos.DrawLine(coneVisionLight.transform.position, coneVisionLight.transform.position + rightBoundary * length);

            // Рисуем дугу
            DrawArc(coneVisionLight.transform.position, length, -halfAngle, halfAngle);
        }
    }

    void DrawArc(Vector3 center, float radius, float startAngle, float endAngle)
    {
        int segments = 30;
        Vector3 prevPoint = center + Quaternion.Euler(0, 0, startAngle) * Vector3.up * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, (float)i / segments);
            Vector3 point = center + Quaternion.Euler(0, 0, angle) * Vector3.up * radius;
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
}