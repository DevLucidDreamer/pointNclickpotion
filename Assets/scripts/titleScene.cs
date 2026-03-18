using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class titleScene : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string nextSceneName = "MainScene";

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI pressAnyKeyText;
    [SerializeField] private Image fadePanel;

    [Header("Blink Settings")]
    [SerializeField] private float blinkSpeed = 2f;
    [SerializeField] private float minTextAlpha = 0.2f;
    [SerializeField] private float maxTextAlpha = 1f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;

    private bool isTransitioning = false;
    //커밋실험용 주석 한번더 테스트
    void Start()
    {
        if (fadePanel != null)
        {
            Color color = fadePanel.color;
            color.a = 0f;
            fadePanel.color = color;
        }
    }

    void Update()
    {
        if (!isTransitioning)
        {
            BlinkText();

            if (IsAnyInputPressed())
            {
                isTransitioning = true;
                StartCoroutine(FadeAndLoadScene());
            }
        }
    }

    private void BlinkText()
    {
        if (pressAnyKeyText == null) return;

        Color color = pressAnyKeyText.color;

        float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        float alpha = Mathf.Lerp(minTextAlpha, maxTextAlpha, t);

        color.a = alpha;
        pressAnyKeyText.color = color;
    }

    private bool IsAnyInputPressed()
    {
        bool keyboardPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;

        bool mousePressed = Mouse.current != null &&
                            (Mouse.current.leftButton.wasPressedThisFrame ||
                             Mouse.current.rightButton.wasPressedThisFrame ||
                             Mouse.current.middleButton.wasPressedThisFrame);

        return keyboardPressed || mousePressed;
    }

    private IEnumerator FadeAndLoadScene()
    {
        if (pressAnyKeyText != null)
        {
            Color textColor = pressAnyKeyText.color;
            textColor.a = 1f;
            pressAnyKeyText.color = textColor;
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);

            if (fadePanel != null)
            {
                Color fadeColor = fadePanel.color;
                fadeColor.a = alpha;
                fadePanel.color = fadeColor;
            }

            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}