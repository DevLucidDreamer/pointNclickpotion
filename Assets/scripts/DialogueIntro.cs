using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueIntro : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private CanvasGroup textBoxCanvasGroup;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerText;

    [Header("Background")]
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Sprite closedDoorBackground;
    [SerializeField] private Sprite openedDoorBackground;

    [Header("Warrior")]
    [SerializeField] private GameObject warriorObject;
    [SerializeField] private Animator warriorAnimator;
    [SerializeField] private Transform warriorSpawnPoint;
    [SerializeField] private string idleStateName = "idle";
    [SerializeField] private string fallStateName = "fall";

    [Header("Door SFX")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip doorOpenSfx;

    [Header("Potion Tutorial UI")]
    [SerializeField] private Button potionCraftButton;
    [SerializeField] private CanvasGroup tutorialDimCanvasGroup;
    [SerializeField] private TextMeshProUGUI tutorialGuideText;
    [SerializeField] private string tutorialGuideMessage = "포션 제작 버튼을 누르세요";
    [SerializeField] private GameObject potionCraftPanel;

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

    [Header("Timing")]
    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float textBoxFadeDuration = 0.4f;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float afterOpeningLineWait = 0.6f;
    [SerializeField] private float afterDoorOpenWait = 0.15f;
    [SerializeField] private float idleDurationBeforeFall = 1.1f;
    [SerializeField] private float afterFallWait = 0.35f;
    [SerializeField] private float afterReactionLineWait = 0.45f;
    [SerializeField] private float afterAdventurerLineWait = 0.75f;
    [SerializeField] private float afterResponseLineWait = 0.6f;
    [SerializeField] private float tutorialFadeDuration = 0.25f;

    [Header("Tutorial Pulse")]
    [SerializeField] private float buttonPulseSpeed = 3.2f;
    [SerializeField] private float buttonPulseScale = 0.06f;

    private bool tutorialActive = false;
    private Vector3 potionButtonBaseScale = Vector3.one;

    private void Start()
    {
        if (textBoxCanvasGroup != null)
        {
            textBoxCanvasGroup.gameObject.SetActive(true);
            textBoxCanvasGroup.alpha = 0f;
            textBoxCanvasGroup.blocksRaycasts = false;
            textBoxCanvasGroup.interactable = false;
        }

        ClearDialogueUI();
        ApplyClosedBackground();

        if (warriorObject != null)
        {
            warriorObject.SetActive(false);
        }

        if (potionCraftButton != null)
        {
            potionCraftButton.gameObject.SetActive(false);
            potionCraftButton.onClick.RemoveListener(OnPotionCraftButtonClicked);
            potionCraftButton.onClick.AddListener(OnPotionCraftButtonClicked);
            potionButtonBaseScale = potionCraftButton.transform.localScale;
        }

        if (tutorialGuideText != null)
        {
            tutorialGuideText.text = "";
            tutorialGuideText.raycastTarget = false;
        }

        HideTutorialImmediate();

        if (potionCraftPanel != null)
        {
            potionCraftPanel.SetActive(false);
        }

        StartCoroutine(ShowDialogueRoutine());
    }

    private void Update()
    {
        if (!tutorialActive || potionCraftButton == null)
            return;

        float t = (Mathf.Sin(Time.unscaledTime * buttonPulseSpeed) + 1f) * 0.5f;
        float scale = 1f + (t * buttonPulseScale);
        potionCraftButton.transform.localScale = potionButtonBaseScale * scale;
    }

    private IEnumerator ShowDialogueRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        yield return StartCoroutine(FadeCanvasGroup(textBoxCanvasGroup, 0f, 1f, textBoxFadeDuration));

        // 1) 주인공 첫 대사
        yield return StartCoroutine(ShowLine(protagonistName, protagonistOpeningLine));
        yield return new WaitForSeconds(afterOpeningLineWait);

        // 2) 문 열림 배경 교체 + 사운드 + 모험가 등장
        OpenDoorBackground();
        PlayDoorOpenSfx();

        if (warriorObject != null)
        {
            if (warriorSpawnPoint != null)
            {
                warriorObject.transform.position = warriorSpawnPoint.position;
            }

            warriorObject.SetActive(true);
        }

        yield return new WaitForSeconds(afterDoorOpenWait);

        if (warriorAnimator != null)
        {
            warriorAnimator.Play(idleStateName, 0, 0f);
        }

        yield return new WaitForSeconds(idleDurationBeforeFall);

        if (warriorAnimator != null)
        {
            warriorAnimator.Play(fallStateName, 0, 0f);
        }

        yield return new WaitForSeconds(afterFallWait);

        // 3) 이어지는 대사
        yield return StartCoroutine(ShowLine(protagonistName, protagonistReactionLine));
        yield return new WaitForSeconds(afterReactionLineWait);

        yield return StartCoroutine(ShowLine(adventurerName, adventurerLine));
        yield return new WaitForSeconds(afterAdventurerLineWait);

        yield return StartCoroutine(ShowLine(protagonistName, protagonistResponseLine));
        yield return new WaitForSeconds(afterResponseLineWait);

        // 4) 대화 종료 후 튜토리얼 진입
        yield return StartCoroutine(FadeCanvasGroup(textBoxCanvasGroup, 1f, 0f, textBoxFadeDuration));

        if (textBoxCanvasGroup != null)
        {
            textBoxCanvasGroup.gameObject.SetActive(false);
        }

        ShowPotionTutorial();
    }

    private IEnumerator ShowLine(string speaker, string line)
    {
        if (speakerText != null)
        {
            speakerText.text = speaker;
        }

        yield return StartCoroutine(TypeText(line));
    }

    private IEnumerator TypeText(string text)
    {
        if (dialogueText == null)
            yield break;

        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        if (group == null)
            yield break;

        group.alpha = from;
        group.blocksRaycasts = to > 0.95f;
        group.interactable = to > 0.95f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        group.alpha = to;
        group.blocksRaycasts = to > 0.95f;
        group.interactable = to > 0.95f;
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

    private void ApplyClosedBackground()
    {
        if (backgroundRenderer == null || closedDoorBackground == null)
            return;

        backgroundRenderer.sprite = closedDoorBackground;
    }

    private void OpenDoorBackground()
    {
        if (backgroundRenderer == null || openedDoorBackground == null)
            return;

        backgroundRenderer.sprite = openedDoorBackground;
    }

    private void PlayDoorOpenSfx()
    {
        if (sfxAudioSource != null && doorOpenSfx != null)
        {
            sfxAudioSource.PlayOneShot(doorOpenSfx);
        }
    }

    private void ShowPotionTutorial()
    {
        tutorialActive = true;

        if (potionCraftButton != null)
        {
            potionCraftButton.gameObject.SetActive(true);
            potionCraftButton.transform.localScale = potionButtonBaseScale;
        }

        if (tutorialDimCanvasGroup != null)
        {
            tutorialDimCanvasGroup.gameObject.SetActive(true);
            tutorialDimCanvasGroup.alpha = 0f;
            tutorialDimCanvasGroup.blocksRaycasts = true;
            tutorialDimCanvasGroup.interactable = true;
            StartCoroutine(FadeCanvasGroup(tutorialDimCanvasGroup, 0f, 1f, tutorialFadeDuration));
        }

        if (tutorialGuideText != null)
        {
            tutorialGuideText.text = tutorialGuideMessage;
        }
    }

    private void HideTutorialImmediate()
    {
        tutorialActive = false;

        if (tutorialDimCanvasGroup != null)
        {
            tutorialDimCanvasGroup.alpha = 0f;
            tutorialDimCanvasGroup.blocksRaycasts = false;
            tutorialDimCanvasGroup.interactable = false;
            tutorialDimCanvasGroup.gameObject.SetActive(false);
        }
    }

    private void OnPotionCraftButtonClicked()
    {
        StartCoroutine(OpenPotionCraftPanelRoutine());
    }

    private IEnumerator OpenPotionCraftPanelRoutine()
    {
        tutorialActive = false;

        if (potionCraftButton != null)
        {
            potionCraftButton.transform.localScale = potionButtonBaseScale;
        }

        if (tutorialDimCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(
                tutorialDimCanvasGroup,
                tutorialDimCanvasGroup.alpha,
                0f,
                tutorialFadeDuration
            ));

            tutorialDimCanvasGroup.gameObject.SetActive(false);
        }

        if (tutorialGuideText != null)
        {
            tutorialGuideText.text = "";
        }

        if (potionCraftPanel != null)
        {
            potionCraftPanel.SetActive(true);
        }
    }

    // 닫기 버튼에 연결할 용도
    public void ClosePotionCraftPanel()
    {
        if (potionCraftPanel != null)
        {
            potionCraftPanel.SetActive(false);
        }
    }
}