using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIShake : MonoBehaviour
{
    [Header("Target to shake (usually the same object)")]
    [SerializeField] private RectTransform target;

    [Header("Shake Params")]
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private float strength = 12f;   // px
    [SerializeField] private int vibrato = 12;       // 진동 횟수

    [Header("Optional Feedback")]
    [SerializeField] private Image flashImage;       // 붉게 점멸할 이미지(예: 배경/외곽)
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.25f);
    [SerializeField] private float flashTime = 0.1f;
    [SerializeField] private bool punchScale = true;
    [SerializeField] private float punchScaleAmount = 0.05f;

    Vector2 originalPos;
    Vector3 originalScale;
    Color originalFlashColor;
    bool isShaking;

    void Reset()
    {
        target = GetComponent<RectTransform>();
        flashImage = GetComponent<Image>();
    }

    void Awake()
    {
        if (!target) target = GetComponent<RectTransform>();
        originalPos = target.anchoredPosition;
        originalScale = target.localScale;
        if (flashImage) originalFlashColor = flashImage.color;
    }

    public void Play()
    {
        if (!gameObject.activeInHierarchy) return;
        if (isShaking) return;
        StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        isShaking = true;

        // UI 살짝 키우기
        if (punchScale)
        {
            target.localScale = originalScale * (1f + punchScaleAmount);
        }

        // 빨간색 효과 넣기
        if (flashImage)
        {
            flashImage.color = flashColor;
            yield return new WaitForSeconds(flashTime);
            flashImage.color = originalFlashColor;
        }

        // 흔들림
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // UI라면 TimeScale 무시 권장
            float t = Mathf.Clamp01(elapsed / duration);

            // 강도가 자연스럽게 감소하도록 보간함수 사용 
            float currentStrength = Mathf.Lerp(strength, 0f, t);

            // 간단한 프랙탈 랜덤 오프셋
            float angle = Random.value * Mathf.PI * 2f; // (0 ~ 1.0) * 2파이
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * currentStrength;

            // vibrato가 높을수록 더 자주 바뀌도록 보간
            float lerp = Mathf.PingPong(t * vibrato, 1f);
            Vector2 shakePos = Vector2.Lerp(Vector2.zero, offset, lerp);

            target.anchoredPosition = originalPos + shakePos;
            yield return null;
        }

        // 원복
        target.anchoredPosition = originalPos;
        target.localScale = originalScale;

        isShaking = false;
    }
}
