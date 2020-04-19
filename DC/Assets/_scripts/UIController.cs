using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{
    public static Transform ItemDropListGameObject { get; private set; }
    public static RectTransform InventoryItemContent { get; private set; }
    public static RectTransform InventoryRootRectTransform { get; private set; }
    public static RectTransform InventoryContextMenu { get; private set; }
    public static Button InventoryUseButton { get; private set; }
    public static Button InventoryEquipButton { get; private set; }
    public static Button InventoryTossButton { get; private set; }
    public static Button InventoryTossUp1Button { get; private set; }
    public static Button InventoryTossUp10Button { get; private set; }
    public static Button InventoryTossDown1Button { get; private set; }
    public static Button InventoryTossDown10Button { get; private set; }
    public static InputField InventoryTossInputField { get; private set; }
    public static Button InventoryCloseButton { get; private set; }


    private static MinMax<Slider> healthSliders = new MinMax<Slider>(), manaSliders = new MinMax<Slider>();
    public static MinMax<Slider> HealthSliderPair { get { return healthSliders; } }
    public static MinMax<Slider> ManaSliderPair { get { return manaSliders; } }


    private static MinMax<Text> healthTexts = new MinMax<Text>(), manaTexts = new MinMax<Text>();
    public static MinMax<Text> HealthTextPair { get { return healthTexts; } } //Text min, max;
    public static MinMax<Text> ManaTextPair { get { return manaTexts; } } //Text min, max;

    public static Slider XPSlider { get; private set; }

    public static Text TurnOrderText { get; private set; }

    public static Button AbilityButton { get; private set; }
    public static Button FleeButton { get; private set; }
    public static Button InventoryButton { get; private set; }
    public static Button InspectButton { get; private set; }
    public static Button LevelUpButton { get; private set; }

    public static RectTransform AbilityMenuScrollView { get; private set; }
    public static RectTransform AbilityMenuContent { get; private set; }
    public static Slider FleeSlider { get; private set; }
    public static Text AbilityButtonText { get; private set; }


    public static GameObject LevelUpScreen { get; private set; }


    public class MinMax<T>
    {
        public T minObject;
        public T maxObject;
    }

    public UIMode currentUIMode;
    public enum UIMode
    {
        None,
        Abilities,
        Inventory,
        Flee,
        Inspect,
        LevelUp,
    }

    // Start is called before the first frame update
    void Start()
    {
        //LevelUpButton = CombatController.playerCombatController.GetComponent<Button>();

        var childrenTransforms = GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < childrenTransforms.Length; i++)
        {
            Transform child = childrenTransforms[i];
            switch (child.name)
            {
                case "$ItemDropList":
                    ItemDropListGameObject = child;
                    break;
                case "$ItemListContent":
                    InventoryItemContent = child.GetComponent<RectTransform>();
                    break;
                case "$ContextMenu":
                    InventoryContextMenu = child.GetComponent<RectTransform>();
                    break;
                case "$UseButton":
                    InventoryUseButton = child.GetComponent<Button>();
                    break;
                case "$EquipButton":
                    InventoryEquipButton = child.GetComponent<Button>();
                    break;
                case "$TossButton":
                    InventoryTossButton = child.GetComponent<Button>();
                    break;
                case "$TossDown10Button":
                    InventoryTossDown10Button = child.GetComponent<Button>();
                    break;
                case "$TossUp10Button":
                    InventoryTossUp10Button = child.GetComponent<Button>();
                    break;
                case "$TossDown1Button":
                    InventoryTossDown1Button = child.GetComponent<Button>();
                    break;
                case "$TossUp1Button":
                    InventoryTossUp1Button = child.GetComponent<Button>();
                    break;
                case "$TossInputField":
                    InventoryTossInputField = child.GetComponent<InputField>();
                    break;
                case "$InventoryRoot":
                    InventoryRootRectTransform = child.GetComponent<RectTransform>();
                    break;
                case "$InventoryCloseButton":
                    InventoryCloseButton = child.GetComponent<Button>();
                    break;
                case "$HealthSlider":
                    healthSliders.minObject = child.GetComponent<Slider>();
                    break;
                case "$HealthSliderSlow":
                    healthSliders.maxObject = child.GetComponent<Slider>();
                    break;
                case "$ManaSlider":
                    manaSliders.minObject = child.GetComponent<Slider>();
                    break;
                case "$ManaSliderSlow":
                    manaSliders.maxObject = child.GetComponent<Slider>();
                    break;
                case "$TurnOrderText":
                    TurnOrderText = child.GetComponent<Text>();
                    break;
                case "$XPSlider":
                    XPSlider = child.GetComponent<Slider>();
                    break;
                case "$CurrentHealthText":
                    healthTexts.minObject = child.GetComponent<Text>();
                    break;
                case "$MaxHealthText":
                    healthTexts.maxObject = child.GetComponent<Text>();
                    break;
                case "$CurrentManaText":
                    manaTexts.minObject = child.GetComponent<Text>();
                    break;
                case "$MaxManaText":
                    manaTexts.maxObject = child.GetComponent<Text>();
                    break;
                case "$AbilityButton":
                    AbilityButton = child.GetComponent<Button>();
                    AbilityButton.onClick.AddListener(delegate { SetUIMode(UIMode.Abilities); });
                    break;
                case "$InventoryButton":
                    InventoryButton = child.GetComponent<Button>();
                    InventoryButton.onClick.AddListener(delegate { SetUIMode(UIMode.Inventory); });

                    break;
                case "$ButtonMenuScrollView":
                    AbilityMenuScrollView = child.GetComponent<RectTransform>();
                    break;
                case "$InspectButton":
                    InspectButton = child.GetComponent<Button>();
                    InspectButton.onClick.AddListener(delegate { SetUIMode(UIMode.Inspect); });

                    if (InspectButton.onClick.GetPersistentEventCount() == 0)
                    {
                        child.GetComponentInChildren<Text>().text = "-";
                    }
                    else
                        print("REMOVE ME");

                    break;
                case "$FleeSlider":
                    FleeSlider = child.GetComponent<Slider>();
                    break;
                case "$FleeButton":
                    //fleeButton = _t.GetComponent<Button>();
                    FleeButton = child.GetComponent<Button>();
                    FleeButton.onClick.AddListener(delegate { SetUIMode(UIMode.Flee); });

                    break;
                case "$AbilityButtonText":
                    AbilityButtonText = child.GetComponent<Text>();
                    break;
                case "$AbilityContent":
                    AbilityMenuContent = child.GetComponent<RectTransform>();
                    break;
                case "$PlayerPortrait":
                    LevelUpButton = child.GetComponent<Button>();
                    break;
                case "$LevelUpHolder":
                    LevelUpScreen = child.gameObject;
                    break;
            }
        }
    }

    void SetUIMode(UIMode targetUIMode)
    {
        if (currentUIMode == targetUIMode)
        {
            currentUIMode = UIMode.None;
        }
        else
            currentUIMode = targetUIMode;

        UpdateUI();
    }

    void UpdateUI()
    {
        AbilityMenuScrollView.gameObject.SetActive(false);
        InventoryRootRectTransform.gameObject.SetActive(false);
        FleeSlider.gameObject.SetActive(false);
        //Inspect .setActive false
        LevelUpScreen.SetActive(false);

        switch(currentUIMode)
        {
            case UIMode.Abilities:
                AbilityMenuScrollView.gameObject.SetActive(true);
                break;
            case UIMode.Inventory:
                InventoryRootRectTransform.gameObject.SetActive(true);
                break;
            case UIMode.Flee:
                FleeSlider.gameObject.SetActive(true);
                break;
            case UIMode.LevelUp:
                LevelUpScreen.SetActive(true);
                break;
        }
    }
}
