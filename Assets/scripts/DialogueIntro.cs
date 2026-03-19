using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueIntro : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup textBoxCanvasGroup;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Warrior")]
    [SerializeField] private GameObject warriorObject;
    [SerializeField] private Animator warriorAnimator;

    [Header("Animation State Names")]
    [SerializeField] private string idleStateName = "idle";
    [SerializeField] private string fallStateName = "fall";

    [Header("Dialogue Settings")]
    [TextArea(2, 5)]
    [SerializeField] private string message = "........이제 곧 마감이네";

    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Sequence Timing")]
    [SerializeField] private float afterTypingWait = 0.8f;
    [SerializeField] private float warriorAppearDelay = 0.2f;
    [SerializeField] private float idleDurationBeforeFall = 1.2f;

    private void Start()
    {
        if (textBoxCanvasGroup != null)
        {
            textBoxCanvasGroup.alpha = 0f;
        }

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        if (warriorObject != null)
        {
            warriorObject.SetActive(false);
        }

        StartCoroutine(ShowDialogueRoutine());
    }

    private IEnumerator ShowDialogueRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        yield return StartCoroutine(FadeCanvasGroup(0f, 1f));

        yield return StartCoroutine(TypeText(message));

        yield return new WaitForSeconds(afterTypingWait);

        yield return StartCoroutine(FadeCanvasGroup(1f, 0f));

        if (textBoxCanvasGroup != null)
        {
            textBoxCanvasGroup.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(warriorAppearDelay);

        if (warriorObject != null)
        {
            warriorObject.SetActive(true);
        }

        yield return null;

        if (warriorAnimator != null)
        {
            warriorAnimator.Play(idleStateName, 0, 0f);
        }

        yield return new WaitForSeconds(idleDurationBeforeFall);

        if (warriorAnimator != null)
        {
            warriorAnimator.Play(fallStateName, 0, 0f);
        }
    }

    private IEnumerator FadeCanvasGroup(float from, float to)
    {
        if (textBoxCanvasGroup == null)
        {
            yield break;
        }

        float elapsed = 0f;
        textBoxCanvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            textBoxCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        textBoxCanvasGroup.alpha = to;
    }

    private IEnumerator TypeText(string text)
    {
        if (dialogueText == null)
        {
            yield break;
        }

        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}