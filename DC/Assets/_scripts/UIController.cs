using System;
using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{
    public const string ABILITIES_BUTTON_STRING = "Abilities", ITEMS_BUTTON_STRING = "Items";// ABILITIES_BUTTON_STRING = "";

    #region General
    public static MonoBehaviour instance { get; private set; }
    public static Camera MainCamera { get; private set; }
    public static RectTransform UICanvas { get; private set; }
    public static GameObject GameOverHolder { get; private set; }
    public static Text FpsText { get; private set; }
    #endregion

    //-----------------------Inventory------------------------
    #region inventory
    #region general
    public static Text InventoryButtonText { get; private set; }
    public static Transform ItemDropListGameObject { get; private set; }
    public static Button ItemDropListConfirmButton { get; private set; }
    public static Image ItemDropListConfirmButtonImage { get; private set; }
    public static Text DropSlotSizeText { get; private set; }
    public static Text InventorySlotText { get; private set; }
    public static RectTransform InventoryItemContent { get; private set; }
    public static RectTransform InventoryRootRectTransform { get; private set; }
    public static RectTransform InventoryGeneralContextMenu { get; private set; }
    public static RectTransform InventoryAccessoryContextMenu { get; private set; }
    public static RectTransform InventoryWeaponSlotContextMenu { get; private set; }
    public static Button InventoryUseButton { get; private set; }
    public static Text InventoryUseButtonText { get; private set; }
    public static Button InventoryEquipButton { get; private set; }
    public static Button InventoryEquipInMainHandButton { get; private set; }
    public static Button InventoryEquipInOffHandButton { get; private set; }
    public static Button InventoryEquipInAccessory1Button { get; private set; }
    public static Button InventoryEquipInAccessory2Button { get; private set; }
    public static Button InventoryEquipInAccessory3Button { get; private set; }
    public static Text InventoryEquipText { get; private set; }
    public static Button InventoryTossButton { get; private set; }
    public static Button InventoryTossUp1Button { get; private set; }
    public static Button InventoryTossUp10Button { get; private set; }
    public static Button InventoryTossDown1Button { get; private set; }
    public static Button InventoryTossDown10Button { get; private set; }
    public static InputField InventoryTossInputField { get; private set; }
    public static Button InventoryCloseButton { get; private set; }
    #endregion

    //-------------------Inventory Equipment-----------------------------------
    #region equipment
    public static Image CurrentEquippedHelmetImage { get; private set; }
    public static Image CurrentEquippedChestplateImage { get; private set; }
    public static Image CurrentEquippedLeggingsImage { get; private set; }
    public static Image CurrentEquippedBootsImage { get; private set; }
    public static Image CurrentEquippedMainHandImage { get; private set; }
    public static Image CurrentEquippedOffHandImage { get; private set; }
    public static Image CurrentEquippedAccessory1Image { get; private set; }
    public static Image CurrentEquippedAccessory2Image { get; private set; }
    public static Image CurrentEquippedAccessory3Image { get; private set; }
    #endregion
    #region unequip
    public static Button UnequipHelmetButton { get; private set; }
    public static Button UnequipChestplateButton { get; private set; }
    public static Button UnequipLeggingsButton { get; private set; }
    public static Button UnequipBootsButton { get; private set; }
    public static Button UnequipMainHandButton { get; private set; }
    public static Button UnequipOffHandButton { get; private set; }
    public static Button UnequipAccessory1Button { get; private set; }
    public static Button UnequipAccessory2Button { get; private set; }
    public static Button UnequipAccessory3Button { get; private set; }
    #endregion
    #endregion

    //-----------------------Level Up---------------------------------------------
    #region LevelUp

    public static GameObject LevelUpScreenGameObject { get; private set; }
    public static Text LevelClassText { get; private set; }
    public static Text LevelUpLeftoverPointsText { get; private set; }
    public static Button ClassChangePossibleButton { get; private set; }
    public static RectTransform SpawnedAbilityToggleContent {get; private set;}
    public static Text AbilitySlotCountText { get; private set; }
    public static Button LevelUpPickAbilityButton1 { get; private set; }
    public static Button LevelUpPickAbilityButton2 { get; private set; }
    public static Button LevelUpPickAbilityButton3 { get; private set; }
    public static Image[] LevelUpPickAbilityInfoImages1 { get; } = new Image[9];
    public static Image[] LevelUpPickAbilityInfoImages2 { get; } = new Image[9];
    public static Image[] LevelUpPickAbilityInfoImages3 { get; } = new Image[9];
    public static Text LevelUpPickAbilityButtonText1 { get; private set; }
    public static Text LevelUpPickAbilityButtonText2 { get; private set; }
    public static Text LevelUpPickAbilityButtonText3 { get; private set; }
    public static Text LevelUpAbilityPointsToSpendText { get; private set; }
    public static Text LevelUpTraitPointsToSpendText { get; private set; }
    public static Text LevelUpStrengthCurrentTraitText { get; private set; }
    public static Text LevelUpDexterityCurrentTraitText { get; private set; }
    public static Text LevelUpIntelligenceCurrentTraitText { get; private set; }
    public static Text LevelUpLuckCurrentTraitText { get; private set; }
    public static MinMax<Button> LevelUpStrengthChangeButtons { get; private set; } = new MinMax<Button>();
    public static MinMax<Button> LevelUpDexterityChangeButtons { get; private set; } = new MinMax<Button>();
    public static MinMax<Button> LevelUpIntellectChangeButtons { get; private set; } = new MinMax<Button>();
    public static MinMax<Button> LevelUpLuckChangeButtons { get; private set; } = new MinMax<Button>();
    #endregion

    #region GameView
    public static MinMax<Slider> HealthSliderPair { get; } = new MinMax<Slider>();
    public static MinMax<Slider> ManaSliderPair { get; } = new MinMax<Slider>();

    public static MinMax<Text> HealthTextPair { get; } = new MinMax<Text>();
    public static MinMax<Text> ManaTextPair { get; } = new MinMax<Text>();

    public static Slider XPSlider { get; private set; }


    public static Text TurnOrderText { get; private set; }

	public static Slider FleeSlider { get; private set; }

	#region Buffs
	public static Transform BuffContent { get; private set; }
    public static ScrollRect BuffScrollRect { get; private set; }
    public static Image BuffScrollImage { get; private set; }
	#endregion
	#endregion

	#region GameButtons
	public static Button AbilityButton { get; private set; }
    public static Button FleeButton { get; private set; }
    public static Button InventoryButton { get; private set; }
    public static Button InspectButton { get; private set; }
    public static Button LevelUpButton { get; private set; }
    #endregion

    #region AbilitySelection
    public static RectTransform AbilityMenuScrollView { get; private set; }
    public static RectTransform AbilityMenuContent { get; private set; }
    public static Text AbilityButtonText { get; private set; }
#endregion

    #region ToolTips
        public static RectTransform ToolTipBackground {get; private set;}
        public static Image ToolTipBackgroundImage { get; private set; }
        public static Text ToolTipText {get; private set;}
    #endregion

	public class MinMax<T>
    {
        public T minObject;
        public T maxObject;
    }

    public static UIMode currentUIMode;
    public enum UIMode
    {
        None = 1,
        Abilities = 2,
        Inventory = 4,
        Flee = 8,
        Inspect = 16,
        LevelUp = 32,

        FullScreen = LevelUp | Inventory,
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        UICanvas = transform.GetChild(0).GetComponent<RectTransform>();
        MainCamera = Camera.main;

        //float progress = 0;

        var childrenTransforms = GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < childrenTransforms.Length; i++)
        {
            //progress = i / (float)childrenTransforms.Length;

            Transform child = childrenTransforms[i];
            switch (child.name)
            {
				#region inventory
				case "$InventoryButtonText":
                    InventoryButtonText = child.GetComponent<Text>();
                    break;
                case "$ItemDropList":
                    ItemDropListGameObject = child;
                    break;
                case "$ItemDropListConfirmButton":
                    ItemDropListConfirmButton = child.GetComponent<Button>();
                    ItemDropListConfirmButtonImage = child.GetComponent<Image>();
                    break;
                case "$DropSlotSizeText":
                    DropSlotSizeText = child.GetComponent<Text>();
                    break;
                case "$InventorySlotText":
                    InventorySlotText = child.GetComponent<Text>();
                    break;
                case "$ItemListContent":
                    InventoryItemContent = child.GetComponent<RectTransform>();
                    break;
                case "$ContextMenu":
                    InventoryGeneralContextMenu = child.GetComponent<RectTransform>();
                    break;
                case "$AccessoryContextMenu":
                    InventoryAccessoryContextMenu = child.GetComponent<RectTransform>();
                    break;
                case "$WeaponSlotContextMenu":
                    InventoryWeaponSlotContextMenu = child.GetComponent<RectTransform>();
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
                case "$InventoryEquipInAccessory1Button":
                    InventoryEquipInAccessory1Button = child.GetComponent<Button>();
                    break;
                case "$InventoryEquipInAccessory2Button":
                    InventoryEquipInAccessory2Button = child.GetComponent<Button>();
                    break;
                case "$InventoryEquipInAccessory3Button":
                    InventoryEquipInAccessory3Button = child.GetComponent<Button>();
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
                #region equipment
                case "$CurrentEquippedHelmet":
                    CurrentEquippedHelmetImage = child.GetComponent<Image>();
                    UnequipHelmetButton = child.GetComponent<Button>();
                    break;
                case "$CurrentEquippedChestplate":
                    CurrentEquippedChestplateImage = child.GetComponent<Image>();
                    UnequipChestplateButton = child.GetComponent<Button>();
                    break;
                case "$CurrentEquippedLeggings":
                    CurrentEquippedLeggingsImage = child.GetComponent<Image>();
                    UnequipLeggingsButton = child.GetComponent<Button>();
                    break;
                case "$CurrentEquippedBoots":
                    CurrentEquippedBootsImage = child.GetComponent<Image>();
                    UnequipBootsButton = child.GetComponent<Button>();
                    break;
                case "$CurrentEquippedMainHand":
                    CurrentEquippedMainHandImage = child.GetComponent<Image>();
                    UnequipMainHandButton = child.GetComponent<Button>();
                    break;
                case "$CurrentEquippedOffHand":
                    CurrentEquippedOffHandImage = child.GetComponent<Image>();
                    UnequipOffHandButton = child.GetComponent<Button>();
                    break;
                case "$CurrentEquippedAccessory1":
                    CurrentEquippedAccessory1Image = child.GetComponent<Image>();
                    UnequipAccessory1Button = child.GetComponent<Button>();
                    break;
                case "$CurrentEquippedAccessory2":
                    CurrentEquippedAccessory2Image = child.GetComponent<Image>();
                    UnequipAccessory2Button = child.GetComponent<Button>();
                    break;
                case "$CurrentEquippedAccessory3":
                    CurrentEquippedAccessory3Image = child.GetComponent<Image>();
                    UnequipAccessory3Button = child.GetComponent<Button>();
                    break;
				#endregion
				#endregion

				#region GameView
				case "$HealthSlider":
                    HealthSliderPair.minObject = child.GetComponent<Slider>();
                    HealthSliderPair.minObject.GetComponent<ToolTip>().SetToolTipText("Your Health Points (HP). Keep this above 0.");
                    break;
                case "$HealthSliderSlow":
                    HealthSliderPair.maxObject = child.GetComponent<Slider>();
                    break;
                case "$ManaSlider":
                    ManaSliderPair.minObject = child.GetComponent<Slider>();
                    ManaSliderPair.minObject.GetComponent<ToolTip>().SetToolTipText("Your Mana Points (MP). Required for some abilities.");
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
                case "$BuffContent":
                    BuffContent = child;
                    BuffScrollRect = child.parent.parent.GetComponent<ScrollRect>();
                    BuffScrollImage = BuffScrollRect.transform.Find("$BuffScrollbarVertical").GetComponent<Image>();
                    break;
                #endregion

                #region GameButtons
                case "$AbilityButton":
                    AbilityButton = child.GetComponent<Button>();
                    AbilityButton.onClick.AddListener(delegate { SetUIMode(UIMode.Abilities); });
                    break;
                case "$InventoryButton":
                    InventoryButton = child.GetComponent<Button>();
                    InventoryButton.onClick.AddListener(delegate { SetUIMode(UIMode.Inventory); });
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
                case "$FleeButton":
                    //fleeButton = _t.GetComponent<Button>();
                    FleeButton = child.GetComponent<Button>();
                    FleeButton.onClick.AddListener(delegate { SetUIMode(UIMode.Flee); });
                    break;
                case "$PlayerPortrait":
                    LevelUpButton = child.GetComponent<Button>();
                    LevelUpButton.GetComponent<ToolTip>().SetToolTipText("You. Another target. Click with no ability to open the status screen.");
                    break;
                #endregion

                case "$FleeSlider":
                    FleeSlider = child.GetComponent<Slider>();
                    //FleeSlider.transform.parent.SetParent(UICanvas.parent);
                    break;

				#region AbilityPicking
				case "$ButtonMenuScrollView":
                    AbilityMenuScrollView = child.GetComponent<RectTransform>();
                    break;
                case "$AbilityButtonText":
                    AbilityButtonText = child.GetComponent<Text>();
                    break;
                case "$AbilityContent":
                    AbilityMenuContent = child.GetComponent<RectTransform>();
                    break;
				#endregion

                #region LevelUp
                case "$LevelUpHolder":
                    LevelUpScreenGameObject = child.gameObject;
                    break;
                case "$LeftoverPointsText":
                    LevelUpLeftoverPointsText = child.GetComponent<Text>();
                    break;
                case "$LevelClassText":
                    LevelClassText = child.GetComponent<Text>();
                    LevelClassText.GetComponent<ToolTip>().SetToolTipText("Learns many different abilities.\n\nLevel up bonus:\n+2HP\n+2MP");
                    break;
                case "$ClassChangePossibleButton":
                    ClassChangePossibleButton = child.GetComponent<Button>();
                    break;
                case "$SpawnedAbilityToggleContent":
                    SpawnedAbilityToggleContent = child as RectTransform;
                    break;
                case "$AbilitySlotCountText":
                    AbilitySlotCountText = child.GetComponent<Text>();
                    break;
                case "$TraitPointsToSpendText":
                    LevelUpTraitPointsToSpendText = child.GetComponent<Text>();
                    break;
                case "$StrengthCurrentTraitText":
                    LevelUpStrengthCurrentTraitText = child.GetComponent<Text>();
                    break;
                case "$DexterityCurrentTraitText":
                    LevelUpDexterityCurrentTraitText = child.GetComponent<Text>();
                    break;
                case "$IntelligenceCurrentTraitText":
                    LevelUpIntelligenceCurrentTraitText = child.GetComponent<Text>();
                    break;
                case "$LuckCurrentTraitText":
                    LevelUpLuckCurrentTraitText = child.GetComponent<Text>();
                    break;
                case "$AbilityPointsToSpendText":
                    LevelUpAbilityPointsToSpendText = child.GetComponent<Text>();
                    break;
                case "$StrengthPlusButton":
                    LevelUpStrengthChangeButtons.maxObject = child.GetComponent<Button>();
                    break;
                case "$StrengthMinusButton":
                    LevelUpStrengthChangeButtons.minObject = child.GetComponent<Button>();
                    break;
                case "$DexterityPlusButton":
                    LevelUpDexterityChangeButtons.maxObject = child.GetComponent<Button>();
                    break;
                case "$DexterityMinusButton":
                    LevelUpDexterityChangeButtons.minObject = child.GetComponent<Button>();
                    break;
                case "$IntelligencePlusButton":
                    LevelUpIntellectChangeButtons.maxObject = child.GetComponent<Button>();
                    break;
                case "$IntelligenceMinusButton":
                    LevelUpIntellectChangeButtons.minObject = child.GetComponent<Button>();
                    break;
                case "$LuckPlusButton":
                    LevelUpLuckChangeButtons.maxObject = child.GetComponent<Button>();
                    break;
                case "$LuckMinusButton":
                    LevelUpLuckChangeButtons.minObject = child.GetComponent<Button>();
                    break;
                case "$LevelUpPickAbilityButton1":
                    LevelUpPickAbilityButton1 = child.GetComponent<Button>();

                    var _images1 = child.GetComponentsInChildren<Image>(true); //get all images
                    int _index1 = 0; //track how many is set (that meet conditions)
                    for (int j = 0; j < _images1.Length; j++)
					{
                        var _curImg = _images1[j]; //shortcut
                        if (_curImg.name.StartsWith("$PortraitType")) //if the image starts with this
						{
                            LevelUpPickAbilityInfoImages1[_index1] = _curImg; //store the current image in the array of images
                            _index1++; //increment the index
						}
					}

                    break;
                case "$LevelUpPickAbilityButton2":
                    LevelUpPickAbilityButton2 = child.GetComponent<Button>();

                    var _images2 = child.GetComponentsInChildren<Image>(true); //get all images
                    int _index2 = 0; //track how many is set (that meet conditions)
                    for (int j = 0; j < _images2.Length; j++)
                    {
                        var _curImg = _images2[j];
                        if (_curImg.name.StartsWith("$PortraitType"))
						{
                            LevelUpPickAbilityInfoImages2[_index2] = _curImg; //store the current image in the array of images
                            _index2++;
						}
                    }
                    break;
                case "$LevelUpPickAbilityButton3":
                    LevelUpPickAbilityButton3 = child.GetComponent<Button>();

                    var _images3 = child.GetComponentsInChildren<Image>(true); //get all images
                    int _index3 = 0; //track how many is set (that meet conditions)
                    for (int j = 0; j < _images3.Length; j++)
                    {
                        var _curImg = _images3[j];
                        if (_curImg.name.StartsWith("$PortraitType"))
						{
                            LevelUpPickAbilityInfoImages3[_index3] = _curImg; //store the current image in the array of images
                            _index3++;
						}
                    }
                    break;
                case "$LevelUpPickAbilityText1":
                    LevelUpPickAbilityButtonText1 = child.GetComponent<Text>();
                    break;
                case "$LevelUpPickAbilityText2":
                    LevelUpPickAbilityButtonText2 = child.GetComponent<Text>();
                    break;
                case "$LevelUpPickAbilityText3":
                    LevelUpPickAbilityButtonText3 = child.GetComponent<Text>();
                    break;
#endregion

				#region ToolTip
				case "$DescriptionBackground":
                    ToolTipBackground = child.GetComponent<RectTransform>();
                    ToolTipBackgroundImage = child.GetComponent<Image>();
                    break;
                case "$DescriptionText":
                    ToolTipText = child.GetComponent<Text>();
                    break;
                #endregion
                case "$FpsText":
                    FpsText = child.GetComponent<Text>();
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
            }
		}

        LevelUpScreen.instance = new LevelUpScreen(); //CombatController.playerCombatController.GetComponent<LevelUpScreen>();
        LevelUpScreen.instance.Initialize();
    }


    private void Update()
    {
        UpdateFpsCounter();

        if (Input.GetKeyDown(Options.abilitiesHotkey)) AbilityButton.onClick.Invoke();
        if (Input.GetKeyDown(Options.fleeHotkey)) FleeButton.onClick.Invoke();
        if (Input.GetKeyDown(Options.itemsHotkey)) InventoryButton.onClick.Invoke();
        if (Input.GetKeyDown(Options.inspectHotkey)) InspectButton.onClick.Invoke();
        if (Input.GetKeyDown(Options.levelUpHotkey)) LevelUpButton.onClick.Invoke();
    }

    private int frames = 0;
    private int totalFps = 0;
    private int min = int.MaxValue;
    private int max = int.MinValue;

    void UpdateFpsCounter()
	{
        if (frames == 100) //reset every 100 frames
        {
            frames = 0;
            totalFps = 0;
            min = int.MaxValue;
            max = int.MinValue;
        }


        int fps = (int)(1f / Time.deltaTime);
        totalFps += fps;
        frames++;
        min = Math.Min(fps, min);
        max = Math.Max(fps, max);


        FpsText.text = "low: " + min.ToString("000") + "\nhi: " + max.ToString("000") + "\navg: " + (totalFps / frames).ToString("000");
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
        LevelUpScreenGameObject.SetActive(false);

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
                LevelUpScreenGameObject.SetActive(true);
                break;
        }
    }

    public static bool IsCurrentUIMode(UIMode _toCheck)
    {
        return _toCheck == currentUIMode;
    }

    public static bool IsFullscreenUI()
    {
        return (currentUIMode & UIMode.FullScreen) != 0;
    }
}
