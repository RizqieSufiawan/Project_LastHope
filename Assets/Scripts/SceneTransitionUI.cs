using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class SceneTransitionUI : MonoBehaviour
{
    public float fadeDuration = 0.6f;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        SetAlpha(1f);
        image.raycastTarget = true;
    }

    private void Start()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Lerp(1f, 0f, timer / fadeDuration));
            yield return null;
        }
        SetAlpha(0f);
        image.raycastTarget = false;
    }

    private void SetAlpha(float alpha)
    {
        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }
}