using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueIntro : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup textBoxCanvasGroup;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue Settings")]
    [TextArea(2, 5)]
    [SerializeField] private string message = "어서 오십시오. 이곳은 숲속의 작은 선술집입니다.";

    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float typingSpeed = 0.05f;

    private void Start()
    {
        textBoxCanvasGroup.alpha = 0f;
        dialogueText.text = "";

        StartCoroutine(ShowDialogueRoutine());
    }

    private IEnumerator ShowDialogueRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        yield return StartCoroutine(FadeInTextBox());

        yield return StartCoroutine(TypeText(message));
    }

    private IEnumerator FadeInTextBox()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            textBoxCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        textBoxCanvasGroup.alpha = 1f;
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}