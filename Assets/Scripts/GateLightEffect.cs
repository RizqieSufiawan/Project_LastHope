using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class GateLightEffect : MonoBehaviour
{
    public GateController gate;
    [Tooltip("Light2D on the Gate that starts at intensity 0 and fades in once the gate breaks.")]
    public Light2D gateLight;

    public float targetIntensity = 1.5f;
    public float fadeInDuration = 2f;

    private void OnEnable()
    {
        if (gate != null) gate.OnGateDestroyed += HandleGateDestroyed;
    }

    private void OnDisable()
    {
        if (gate != null) gate.OnGateDestroyed -= HandleGateDestroyed;
    }

    private void HandleGateDestroyed()
    {
        if (gateLight != null) StartCoroutine(FadeInLight());
    }

    private IEnumerator FadeInLight()
    {
        float timer = 0f;
        float startIntensity = gateLight.intensity;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            gateLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, timer / fadeInDuration);
            yield return null;
        }

        gateLight.intensity = targetIntensity;
    }
}