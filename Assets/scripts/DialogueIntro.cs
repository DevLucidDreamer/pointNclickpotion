using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueIntro : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup textBoxCanvasGroup;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerText;

    [Header("Warrior")]
    [SerializeField] private GameObject warriorObject;
    [SerializeField] private Animator warriorAnimator;

    [Header("Animation State Names")]
    [SerializeField] private string idleStateName = "idle";
    [SerializeField] private string fallStateName = "fall";

    [Header("Dialogue Lines")]
    [TextArea(2, 5)]
    [SerializeField] private string protagonistOpeningLine = "........이제 곧 마감이네";

    [TextArea(2, 5)]
    [SerializeField] private string protagonistReactionLine = "...!";

    [TextArea(2, 5)]
    [SerializeField] private string adventurerLine = "....힐 포션 하나";

    [TextArea(2, 5)]
    [SerializeField] private string protagonistResponseLine = "알겠습니다. 잠시만 기다려주세요.";

    [Header("Speaker Names")]
    [SerializeField] private string protagonistName = "주인공";
    [SerializeField] private string adventurerName = "모험가";

    [Header("Dialogue Settings")]
    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Sequence Timing")]
    [SerializeField] private float afterOpeningLineWait = 0.8f;
    [SerializeField] private float warriorAppearDelay = 0.2f;
    [SerializeField] private float idleDurationBeforeFall = 1.2f;
    [SerializeField] private float afterFallWait = 0.5f;
    [SerializeField] private float afterReactionLineWait = 0.6f;
    [SerializeField] private float afterAdventurerLineWait = 1.2f;
    [SerializeField] private float afterProtagonistResponseWait = 1.0f;

    private void Start()
    {
        if (textBoxCanvasGroup != null)
        {
            textBoxCanvasGroup.alpha = 0f;
            textBoxCanvasGroup.gameObject.SetActive(true);
        }

        ClearDialogueUI();

        if (warriorObject != null)
        {
            warriorObject.SetActive(false);
        }

        StartCoroutine(ShowDialogueRoutine());
    }

    private IEnumerator ShowDialogueRoutine()
    {
        // 1. 시작 대기
        yield return new WaitForSeconds(startDelay);

        // 2. 대화창 등장
        ClearDialogueUI();
        yield return StartCoroutine(FadeCanvasGroup(0f, 1f));

        // 3. 주인공 첫 대사
        yield return StartCoroutine(ShowLine(protagonistName, protagonistOpeningLine));

        yield return new WaitForSeconds(afterOpeningLineWait);

        // 4. 대화창 사라짐
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f));

        if (textBoxCanvasGroup != null)
        {
            textBoxCanvasGroup.gameObject.SetActive(false);
        }

        // 5. 모험가 등장
        yield return new WaitForSeconds(warriorAppearDelay);

        if (warriorObject != null)
        {
            warriorObject.SetActive(true);
        }

        // 6. idle 재생
        yield return null;

        if (warriorAnimator != null)
        {
            warriorAnimator.Play(idleStateName, 0, 0f);
        }

        // 7. 잠깐 뒤 fall 재생
        yield return new WaitForSeconds(idleDurationBeforeFall);

        if (warriorAnimator != null)
        {
            warriorAnimator.Play(fallStateName, 0, 0f);
        }

        // 8. fall 후 잠시 대기
        yield return new WaitForSeconds(afterFallWait);

        // 9. 대화창 다시 등장
        ClearDialogueUI();

        if (textBoxCanvasGroup != null)
        {
            textBoxCanvasGroup.gameObject.SetActive(true);
            textBoxCanvasGroup.alpha = 0f;
        }

        yield return StartCoroutine(FadeCanvasGroup(0f, 1f));

        // 10. 주인공 반응
        yield return StartCoroutine(ShowLine(protagonistName, protagonistReactionLine));

        yield return new WaitForSeconds(afterReactionLineWait);

        // 11. 모험가 대사
        yield return StartCoroutine(ShowLine(adventurerName, adventurerLine));

        yield return new WaitForSeconds(afterAdventurerLineWait);

        // 12. 주인공 응답
        yield return StartCoroutine(ShowLine(protagonistName, protagonistResponseLine));

        yield return new WaitForSeconds(afterProtagonistResponseWait);
    }

    private void ClearDialogueUI()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        if (speakerText != null)
        {
            speakerText.text = "";
        }
    }

    private IEnumerator ShowLine(string speaker, string line)
    {
        if (speakerText != null)
        {
            speakerText.text = speaker;
        }

        yield return StartCoroutine(TypeText(line));
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