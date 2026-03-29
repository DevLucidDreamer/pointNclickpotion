using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PotionCraftPanelController : MonoBehaviour
{
    [Header("Panel Root")]
    [SerializeField] private GameObject craftPanelRoot;

    [Header("Main Craft Interaction")]
    [SerializeField] private CanvasGroup craftInteractionCanvasGroup;

    [Header("Recipe Book UI")]
    [SerializeField] private GameObject recipeBookModalRoot;
    [SerializeField] private GameObject recipeBookPanel;
    [SerializeField] private GameObject healPotionRecipeView;
    [SerializeField] private Button recipeBookButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button healPotionRecipeButton;

    [Header("Recipe Text")]
    [SerializeField] private TextMeshProUGUI recipeTitleText;
    [SerializeField] private TextMeshProUGUI recipeDescriptionText;

    [Header("Ingredient Buttons")]
    [SerializeField] private Button herbButton;
    [SerializeField] private Button waterButton;

    [Header("Cauldron Animator")]
    [SerializeField] private Animator cauldronAnimator;
    [SerializeField] private string idleStateName = "pot_idle";
    [SerializeField] private string addWaterTriggerName = "addWater";
    [SerializeField] private string brewTriggerName = "brew";

    [Header("Brew Timing")]
    [SerializeField] private float delayBeforeBrew = 0.6f;
    [SerializeField] private float brewDuration = 3.0f;
    [SerializeField] private float delayBeforeResult = 0.25f;
    [SerializeField] private float resultShowTime = 1.2f;

    [Header("Result")]
    [SerializeField] private GameObject healPotionResult;

    [Header("Outside UI")]
    [SerializeField] private GameObject potionCraftButtonObject;

    private bool hasWater = false;
    private bool hasHerb = false;
    private bool isBrewing = false;

    private void Awake()
    {
        if (recipeBookButton != null)
            recipeBookButton.onClick.AddListener(OpenRecipeBook);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);

        if (healPotionRecipeButton != null)
            healPotionRecipeButton.onClick.AddListener(ShowHealPotionRecipe);

        if (waterButton != null)
            waterButton.onClick.AddListener(AddWater);

        if (herbButton != null)
            herbButton.onClick.AddListener(AddHerb);
    }

    private void OnEnable()
    {
        ResetPanelState();
    }

    private void ResetPanelState()
    {
        hasWater = false;
        hasHerb = false;
        isBrewing = false;

        SetCraftInteractionEnabled(true);

        if (recipeBookModalRoot != null)
            recipeBookModalRoot.SetActive(false);

        if (recipeBookPanel != null)
            recipeBookPanel.SetActive(false);

        if (healPotionRecipeView != null)
            healPotionRecipeView.SetActive(false);

        if (backButton != null)
        {
            backButton.gameObject.SetActive(false);
            backButton.interactable = true;
        }

        if (recipeBookButton != null)
            recipeBookButton.interactable = true;

        if (healPotionRecipeButton != null)
        {
            healPotionRecipeButton.gameObject.SetActive(true);
            healPotionRecipeButton.interactable = true;
        }

        if (healPotionResult != null)
            healPotionResult.SetActive(false);

        if (waterButton != null)
        {
            waterButton.gameObject.SetActive(true);
            waterButton.interactable = true;
        }

        if (herbButton != null)
        {
            herbButton.gameObject.SetActive(true);
            herbButton.interactable = true;
        }

        if (recipeTitleText != null)
            recipeTitleText.text = "";

        if (recipeDescriptionText != null)
            recipeDescriptionText.text = "";

        ResetCauldronAnimator();
    }

    private void ResetCauldronAnimator()
    {
        if (cauldronAnimator == null)
            return;

        cauldronAnimator.ResetTrigger(addWaterTriggerName);
        cauldronAnimator.ResetTrigger(brewTriggerName);
        cauldronAnimator.Play(idleStateName, 0, 0f);
        cauldronAnimator.Update(0f);
    }

    private void SetCraftInteractionEnabled(bool enabled)
    {
        if (craftInteractionCanvasGroup != null)
        {
            craftInteractionCanvasGroup.interactable = enabled;
            craftInteractionCanvasGroup.blocksRaycasts = enabled;
            craftInteractionCanvasGroup.alpha = 1f;
        }
        else
        {
            if (waterButton != null)
                waterButton.interactable = enabled && !hasWater;

            if (herbButton != null)
                herbButton.interactable = enabled && !hasHerb;

            if (recipeBookButton != null)
                recipeBookButton.interactable = enabled;
        }
    }

    public void OpenRecipeBook()
    {
        if (isBrewing) return;

        SetCraftInteractionEnabled(false);

        if (recipeBookModalRoot != null)
        {
            recipeBookModalRoot.SetActive(true);
            recipeBookModalRoot.transform.SetAsLastSibling();
        }

        if (recipeBookPanel != null)
        {
            recipeBookPanel.SetActive(true);
            recipeBookPanel.transform.SetAsLastSibling();
        }

        if (healPotionRecipeView != null)
            healPotionRecipeView.SetActive(false);

        if (backButton != null)
        {
            backButton.gameObject.SetActive(true);
            backButton.transform.SetAsLastSibling();
        }
    }

    public void CloseRecipeBook()
    {
        CloseRecipeBookInternal(true);
    }

    private void CloseRecipeBookInternal(bool restoreCraftInteraction)
    {
        if (recipeBookModalRoot != null)
            recipeBookModalRoot.SetActive(false);

        if (recipeBookPanel != null)
            recipeBookPanel.SetActive(false);

        if (healPotionRecipeView != null)
            healPotionRecipeView.SetActive(false);

        if (backButton != null)
            backButton.gameObject.SetActive(false);

        if (restoreCraftInteraction)
            SetCraftInteractionEnabled(true);
    }

    private void OnBackButtonClicked()
    {
        CloseRecipeBook();
    }

    public void ShowHealPotionRecipe()
    {
        if (healPotionRecipeButton != null)
        {
            healPotionRecipeButton.gameObject.SetActive(false);
        }

        if (healPotionRecipeView != null)
        {
            healPotionRecipeView.SetActive(true);
            healPotionRecipeView.transform.SetAsLastSibling();
        }

        if (recipeTitleText != null)
            recipeTitleText.text = "힐 포션";

        if (recipeDescriptionText != null)
            recipeDescriptionText.text = "재료: 물 1개, 허브 1개\n효과: 체력을 회복합니다.";
    }

    public void AddWater()
    {
        if (isBrewing || hasWater) return;

        hasWater = true;

        if (waterButton != null)
        {
            waterButton.interactable = false;
            waterButton.gameObject.SetActive(false);
        }

        if (cauldronAnimator != null)
        {
            cauldronAnimator.ResetTrigger(addWaterTriggerName);
            cauldronAnimator.ResetTrigger(brewTriggerName);
            cauldronAnimator.SetTrigger(addWaterTriggerName);
        }
    }

    public void AddHerb()
    {
        if (isBrewing || hasHerb) return;
        if (!hasWater) return;

        hasHerb = true;

        if (herbButton != null)
        {
            herbButton.interactable = false;
            herbButton.gameObject.SetActive(false);
        }

        CheckBrewStart();
    }

    private void CheckBrewStart()
    {
        if (hasWater && hasHerb && !isBrewing)
        {
            StartCoroutine(BrewHealPotionRoutine());
        }
    }

    private IEnumerator BrewHealPotionRoutine()
    {
        isBrewing = true;

        SetCraftInteractionEnabled(false);

        if (recipeBookButton != null)
            recipeBookButton.interactable = false;

        if (backButton != null)
            backButton.interactable = false;

        if (healPotionRecipeButton != null)
            healPotionRecipeButton.interactable = false;

        CloseRecipeBookInternal(false);

        // 물이 차 있는 상태를 잠시 보여줌
        yield return new WaitForSeconds(delayBeforeBrew);

        if (cauldronAnimator != null)
        {
            cauldronAnimator.ResetTrigger(addWaterTriggerName);
            cauldronAnimator.ResetTrigger(brewTriggerName);
            cauldronAnimator.SetTrigger(brewTriggerName);
        }

        // 끓는 애니메이션을 충분히 보여줌
        yield return new WaitForSeconds(brewDuration);

        // 결과창이 너무 급하게 뜨지 않도록 약간의 텀
        yield return new WaitForSeconds(delayBeforeResult);

        if (healPotionResult != null)
        {
            healPotionResult.SetActive(true);
            healPotionResult.transform.SetAsLastSibling();
        }

        yield return new WaitForSeconds(resultShowTime);

        if (potionCraftButtonObject != null)
        {
            potionCraftButtonObject.SetActive(false);
        }

        CloseCraftPanel();
    }

    public void CloseCraftPanel()
    {
        ResetCauldronAnimator();

        if (craftPanelRoot != null)
            craftPanelRoot.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}