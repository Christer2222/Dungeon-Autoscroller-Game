using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{
    public static Transform ItemDropListGameObject { get; private set; }
    public static RectTransform InventoryItemContent { get; private set; }
    public static RectTransform InventoryRootRectTransform { get; private set; }
    public static RectTransform InventoryContextMenu { get; private set; }
    public static RectTransform AccessoryContextMenu { get; private set; }
    public static RectTransform WeaponSlotContextMenu { get; private set; }
    public static Button InventoryUseButton { get; private set; }
    public static Text InventoryUseButtonText { get; private set; }
    public static Button InventoryEquipButton { get; private set; }
    public static Button InventoryEquipInMainHandButton { get; private set; }
    public static Button InventoryEquipInOffHandButton { get; private set; }
    public static Text InventoryEquipText { get; private set; }
    public static Button InventoryTossButton { get; private set; }
    public static Button InventoryTossUp1Button { get; private set; }
    public static Button InventoryTossUp10Button { get; private set; }
    public static Button InventoryTossDown1Button { get; private set; }
    public static Button InventoryTossDown10Button { get; private set; }
    public static InputField InventoryTossInputField { get; private set; }
    public static Button InventoryCloseButton { get; private set; }

    public static Image CurrentEquippedHelmetImage { get; private set;}
    public static Image CurrentEquippedChestplateImage { get; private set; }
    public static Image CurrentEquippedLeggingsImage { get; private set; }
    public static Image CurrentEquippedBootsImage { get; private set; }
    public static Image CurrentEquippedMainHandImage { get; private set; }
    public static Image CurrentEquippedOffHandImage { get; private set; }
    public static Image CurrentEquippedAccessory1Image { get; private set; }
    public static Image CurrentEquippedAccessory2Image { get; private set; }
    public static Image CurrentEquippedAccessory3Image { get; private set; }



    public static MinMax<Slider> HealthSliderPair { get; } = new MinMax<Slider>();
    public static MinMax<Slider> ManaSliderPair { get; } = new MinMax<Slider>();

    public static MinMax<Text> HealthTextPair { get; } = new MinMax<Text>();
    public static MinMax<Text> ManaTextPair { get; } = new MinMax<Text>();

    public static Slider XPSlider { get; private set; }

    
    public static Text TurnOrderText { get; private set; }

    public static GameObject GameOverHolder { get; private set; }

    public static Transform BuffContent { get; private set; }
    public static ScrollRect BuffScrollRect { get; private set; }
    public static Image BuffScrollImage { get; private set; }


    public static Button AbilityButton { get; private set; }
    public static Button FleeButton { get; private set; }
    public static Button InventoryButton { get; private set; }
    public static Button InspectButton { get; private set; }
    public static Button LevelUpButton { get; private set; }

    
    public static RectTransform AbilityMenuScrollView { get; private set; }
    public static RectTransform AbilityMenuContent { get; private set; }
    public static Text AbilityButtonText { get; private set; }

    
    public static Slider FleeSlider { get; private set; }


    public static GameObject LevelUpScreen { get; private set; }

    public static Transform UICanvas { get; private set; }

    public class MinMax<T>
    {
        public T minObject;
        public T maxObject;
    }

    public static UIMode currentUIMode;
    public enum UIMode
    {
        None = 0,
        Abilities = 1,
        Inventory = 2,
        Flee = 4,
        Inspect = 8,
        LevelUp = 16,

        FullScreen = LevelUp | Inventory,
    }

    // Start is called before the first frame update
    void Start()
    {
        UICanvas = transform.GetChild(0);

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
                case "$AccessoryContextMenu":
                    AccessoryContextMenu = child.GetComponent<RectTransform>();
                    break;
                case "$WeaponSlotContextMenu":
                    WeaponSlotContextMenu = child.GetComponent<RectTransform>();
                    break;
                case "$UseButton":
                    InventoryUseButton = child.GetComponent<Button>();
                    break;
                case "$UseButtonText":
                    InventoryUseButtonText = child.GetComponent<Text>();
                    break;
                case "$EquipButton":
                    InventoryEquipButton = child.GetComponent<Button>();
                    break;
                case "$EquipInMainHandButton":
                    InventoryEquipInMainHandButton = child.GetComponent<Button>();
                    break;
                case "$InventoryEquipInOffHandButton":
                    InventoryEquipInOffHandButton = child.GetComponent<Button>();
                    break;
                case "$EquipButtonText":
                    InventoryEquipText = child.GetComponent<Text>();
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
                    HealthSliderPair.minObject = child.GetComponent<Slider>();
                    break;
                case "$HealthSliderSlow":
                    HealthSliderPair.maxObject = child.GetComponent<Slider>();
                    break;
                case "$ManaSlider":
                    ManaSliderPair.minObject = child.GetComponent<Slider>();
                    break;
                case "$ManaSliderSlow":
                    ManaSliderPair.maxObject = child.GetComponent<Slider>();
                    break;
                case "$TurnOrderText":
                    TurnOrderText = child.GetComponent<Text>();
                    break;
                case "$XPSlider":
                    XPSlider = child.GetComponent<Slider>();
                    break;
                case "$CurrentHealthText":
                    HealthTextPair.minObject = child.GetComponent<Text>();
                    break;
                case "$MaxHealthText":
                    HealthTextPair.maxObject = child.GetComponent<Text>();
                    break;
                case "$CurrentManaText":
                    ManaTextPair.minObject = child.GetComponent<Text>();
                    break;
                case "$MaxManaText":
                    ManaTextPair.maxObject = child.GetComponent<Text>();
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
                case "$BuffContent":
                    BuffContent = child;
                    BuffScrollRect = child.parent.parent.GetComponent<ScrollRect>();
                    BuffScrollImage = BuffScrollRect.transform.Find("$BuffScrollbarVertical").GetComponent<Image>();
                    break;
                case "$GameOverHolder":
                    GameOverHolder = child.gameObject;
                    foreach (Transform _childGameOver in child.GetComponentsInChildren<Transform>(true))
                    {
                        if (_childGameOver.name == "$RestartButton")
                        {
                            _childGameOver.GetComponent<Button>().onClick.AddListener(delegate { var a = new GameObject(); a.AddComponent<ResetStaticVariablesManager>(); });
                        }
                    }
                    break;
                case "$CurrentEquippedHelmet":
                    CurrentEquippedHelmetImage = child.GetComponent<Image>();
                    break;
                case "$CurrentEquippedChestplate":
                    CurrentEquippedChestplateImage = child.GetComponent<Image>();
                    break;
                case "$CurrentEquippedLeggings":
                    CurrentEquippedLeggingsImage = child.GetComponent<Image>();
                    break;
                case "$CurrentEquippedBoots":
                    CurrentEquippedBootsImage = child.GetComponent<Image>();
                    break;
                case "$CurrentEquippedMainHand":
                    CurrentEquippedMainHandImage = child.GetComponent<Image>();
                    break;
                case "$CurrentEquippedOffHand":
                    CurrentEquippedOffHandImage = child.GetComponent<Image>();
                    break;
                case "$CurrentEquippedAccessory1":
                    CurrentEquippedAccessory1Image = child.GetComponent<Image>();
                    break;
                case "$CurrentEquippedAccessory2":
                    CurrentEquippedAccessory2Image = child.GetComponent<Image>();
                    break;
                case "$CurrentEquippedAccessory3":
                    CurrentEquippedAccessory3Image = child.GetComponent<Image>();
                    break;
            }
        }
    }

    public static void SetUIMode(UIMode targetUIMode)
    {
        if (currentUIMode == targetUIMode)
        {
            currentUIMode = UIMode.None;
        }
        else
            currentUIMode = targetUIMode;

        UpdateUI();
    }

    static void UpdateUI()
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

    public static bool IsCurrentUIMode(UIMode _toCheck)
    {
        return _toCheck == currentUIMode;
    }

    public static bool IsFullscreenUI()
    {
        return (currentUIMode & UIMode.FullScreen) == 0;
    }
}
