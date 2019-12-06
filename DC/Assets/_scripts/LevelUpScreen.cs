using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpScreen : AbilityData
{
	public GameObject levelUpScreen;

	public static LevelUpScreen instance;

	private struct AbilityChoices
	{
		public AbilityChoices(string _option1, string _option2, string _option3)
		{
			option1 = _option1;
			option2 = _option2;
			option3 = _option3;
			used = false;
		}
		public string option1, option2, option3;
		public bool used;
	}

	private List<AbilityChoices> levelUpQue = new List<AbilityChoices>();
	private int addedAdventurerLevels;
	private List<AbilityChoices> adventurerLevelLine = new List<AbilityChoices>()
	{
		new AbilityChoices(SPOT_WEAKNESS,"",""),
		new AbilityChoices(WILD_PUNCH,DOUBLE_KICK,""),
		new AbilityChoices(LIFE_TAP,TILT_SWING,HEAL),
		new AbilityChoices(SIPHON_SOUL,FIREBALL,""),
		new AbilityChoices(FORCE_PUNCH,REGENERATION,""),
		new AbilityChoices(CHAOS_THESIS,DIVINE_LUCK,""),
	};

	private int strengthChange, dexterityChange, intelligenceChange, luckChange;
	public static int traitPointsToSpend = DebugController.bonusAbilityPoints, abilityPointsToSpend;

	private Text strengthText, dexterityText, intelligenceText, luckText;
	private Text traitPointToSpendText, abilityPointsToSpendText;
    private const string TRAIT_POINTS_DEFAULT_STRING = "Trait points: ", ABILITY_POINTS_DEFAULT_STRING = "Ability points: ";
	private Text classText;

	private Button strengthPlusButton, dexterityPlusButton, intelligencePlusButton, luckPlusButton;
	private Button strengthMinusButton, dexterityMinusButton, intelligenceMinusButton, luckMinusButton;
	private Button changeClassButton;
	private Button abilityPickButton1, abilityPickButton2, abilityPickButton3;
	private Text abilityPickText1, abilityPickText2, abilityPickText3;
	private Button cancelButton, confirmButton;

	void Start()
	{
		GetComponent<Button>().onClick.AddListener(delegate { ToggleLevelUpScreen(); });

		instance = this;

		//playerCombatController = CombatController.playerCombatController;//GameObject.Find("$Player").GetComponent<CombatController>();

		var _UICanvas = GameObject.Find("$UICanvas");
		foreach(Transform _t in _UICanvas.GetComponentsInChildren<Transform>(true))
		{
			switch(_t.name)
			{
				case "$LevelUpHolder":
					levelUpScreen = _t.gameObject;
					break;
                case "$TraitPointsToSpendText":
                    traitPointToSpendText = _t.GetComponent<Text>();
                    break;
                case "$StrengthCurrentTraitText":
					strengthText = _t.GetComponent<Text>();
					break;
				case "$DexterityCurrentTraitText":
					dexterityText = _t.GetComponent<Text>();
					break;
				case "$IntelligenceCurrentTraitText":
					intelligenceText = _t.GetComponent<Text>();
					break;
				case "$LuckCurrentTraitText":
					luckText = _t.GetComponent<Text>();
					break;
				case "$StrengthPlusButton":
					strengthPlusButton = _t.GetComponent<Button>();
					strengthPlusButton.onClick.AddListener(delegate {
						UpdateTraitText(ref strengthChange,1, CombatController.playerCombatController.myStats.baseStrength,ref strengthText,ref strengthMinusButton);
					});
					break;
				case "$DexterityPlusButton":
					dexterityPlusButton = _t.GetComponent<Button>();
					dexterityPlusButton.onClick.AddListener(delegate {
						UpdateTraitText(ref dexterityChange,1, CombatController.playerCombatController.myStats.baseDexterity,ref dexterityText, ref dexterityMinusButton);
					});
					break;
				case "$IntelligencePlusButton":
					intelligencePlusButton = _t.GetComponent<Button>();
					intelligencePlusButton.onClick.AddListener(delegate {
						UpdateTraitText(ref intelligenceChange,1, CombatController.playerCombatController.myStats.baseIntelligence,ref intelligenceText, ref intelligenceMinusButton);
					});
					break;
				case "$LuckPlusButton":
					luckPlusButton = _t.GetComponent<Button>();
					luckPlusButton.onClick.AddListener(delegate {
						UpdateTraitText(ref luckChange,1, CombatController.playerCombatController.myStats.baseLuck,ref luckText, ref luckMinusButton);
					});
					break;
				case "$StrengthMinusButton":
					strengthMinusButton = _t.GetComponent<Button>();
					strengthMinusButton.onClick.AddListener(delegate {
						UpdateTraitText(ref strengthChange,-1, CombatController.playerCombatController.myStats.baseStrength,ref strengthText, ref strengthMinusButton);
					});
					break;
				case "$DexterityMinusButton":
					dexterityMinusButton = _t.GetComponent<Button>();
					dexterityMinusButton.onClick.AddListener(delegate
					{
						UpdateTraitText(ref dexterityChange,-1, CombatController.playerCombatController.myStats.baseDexterity,ref dexterityText, ref dexterityMinusButton);
					});
					break;
				case "$IntelligenceMinusButton":
					intelligenceMinusButton = _t.GetComponent<Button>();
					intelligenceMinusButton.onClick.AddListener(delegate
					{
						UpdateTraitText(ref intelligenceChange,-1, CombatController.playerCombatController.myStats.baseIntelligence,ref intelligenceText, ref intelligenceMinusButton);
					});
					break;
				case "$LuckMinusButton":
					luckMinusButton = _t.GetComponent<Button>();
					luckMinusButton.onClick.AddListener(delegate
					{
						UpdateTraitText(ref luckChange,-1, CombatController.playerCombatController.myStats.baseLuck,ref luckText, ref luckMinusButton);
					});
					break;
				case "$ClassChangePossibleButton":
					changeClassButton = _t.GetComponent<Button>();
					break;
				case "$ClassText":
					classText = _t.GetComponent<Text>();
					break;
				case "$AbilityPointsToSpendText":
					abilityPointsToSpendText = _t.GetComponent<Text>();
					break;
				case "$AbilityButton1":
					abilityPickButton1 = _t.GetComponent<Button>();
					abilityPickText1 = abilityPickButton1.GetComponentInChildren<Text>();

					abilityPickButton1.onClick.AddListener(delegate {
						CombatController.playerCombatController.myStats.abilities.Add(abilityPickText1.text);
						CombatController.playerCombatController.RefreshAbilityList();
						levelUpQue.Remove(levelUpQue[0]);
						RefreshAbilities();
					});

					break;
				case "$AbilityButton2":
					abilityPickButton2 = _t.GetComponent<Button>();
					abilityPickText2 = abilityPickButton2.GetComponentInChildren<Text>();
					break;
				case "$AbilityButton3":
					abilityPickButton3 = _t.GetComponent<Button>();
					abilityPickText3 = abilityPickButton3.GetComponentInChildren<Text>();
					break;
				case "$CancelButton":
					cancelButton = _t.GetComponent<Button>();
					cancelButton.onClick.AddListener(delegate
					{
						traitPointsToSpend = strengthChange + dexterityChange + intelligenceChange + luckChange + traitPointsToSpend;
						strengthChange = dexterityChange = intelligenceChange = luckChange = 0;

						ToggleLevelUpScreen();
					});

					break;
				case "$ConfirmButton":
					confirmButton = _t.GetComponent<Button>();
					confirmButton.onClick.AddListener(delegate {
                        CombatController.playerCombatController.myStats.baseStrength += strengthChange;
                        CombatController.playerCombatController.myStats.baseDexterity += dexterityChange;
                        CombatController.playerCombatController.myStats.baseIntelligence += intelligenceChange;
                        CombatController.playerCombatController.myStats.baseLuck += luckChange;

						strengthChange = dexterityChange = intelligenceChange = luckChange = 0;

						strengthText.text = CombatController.playerCombatController.myStats.baseStrength.ToString();
						dexterityText.text = CombatController.playerCombatController.myStats.baseDexterity.ToString();
						intelligenceText.text = CombatController.playerCombatController.myStats.baseIntelligence.ToString();
						luckText.text = CombatController.playerCombatController.myStats.baseLuck.ToString();

                        confirmButton.gameObject.SetActive(false);

						ToggleArrowButtons();
					});
					break;
			}
		}
	}

	public void AddNextChoicesToQue()
	{
		if (adventurerLevelLine.Count > addedAdventurerLevels)
		{
			levelUpQue.Add(adventurerLevelLine[addedAdventurerLevels]);
			addedAdventurerLevels++;
		}
	}

	void RefreshAbilities()
	{
		bool _lc = (levelUpQue.Count > 0);
		bool _hasOption1 = (_lc) ? (!string.IsNullOrEmpty(levelUpQue[0].option1)): false;
		bool _hasOption2 = (_lc) ? (!string.IsNullOrEmpty(levelUpQue[0].option2)): false;
		bool _hasOption3 = (_lc) ? (!string.IsNullOrEmpty(levelUpQue[0].option3)): false;

		abilityPickButton1.gameObject.SetActive(_hasOption1);
		abilityPickButton2.gameObject.SetActive(_hasOption2);
		abilityPickButton3.gameObject.SetActive(_hasOption3);

		if (_lc)
		{
			abilityPickText1.text = levelUpQue[0].option1;
			abilityPickText2.text = levelUpQue[0].option2;
			abilityPickText3.text = levelUpQue[0].option3;
		}
	}



	public void UpdateTraitText(ref int _change, int _difference, int _traitScore, ref Text _textToChange, ref Button _minusButton)
	{
		_change += _difference;
		traitPointsToSpend -= _difference;
		_minusButton.gameObject.SetActive(_change > 0);
		//_plusButton.gameObject.SetActive(traitPointsToSpend > 0);

		strengthPlusButton.gameObject.SetActive(traitPointsToSpend > 0);
		dexterityPlusButton.gameObject.SetActive(traitPointsToSpend > 0);
		intelligencePlusButton.gameObject.SetActive(traitPointsToSpend > 0);
		luckPlusButton.gameObject.SetActive(traitPointsToSpend > 0);

        traitPointToSpendText.text = TRAIT_POINTS_DEFAULT_STRING + traitPointsToSpend;
        confirmButton.gameObject.SetActive(strengthChange + dexterityChange + intelligenceChange + luckChange > 0);
        //strengthMinusButton.gameObject.SetActive(_change > 0);
        ///dexterityMinusButton.gameObject.SetActive(_change > 0);
        //intelligenceMinusButton.gameObject.SetActive(_change > 0);
        //luckMinusButton.gameObject.SetActive(_change > 0);


        _textToChange.text = (_traitScore + _change) + ((_change > 0)? "(+" + _change + ")": "");
	}


	void ToggleLevelUpScreen()
	{
        if (CombatController.playerCombatController.actedLastTick) return;
		//if (CombatController.playerCombatController.selectedAbility != string.Empty) return;

		levelUpScreen.SetActive(!levelUpScreen.activeSelf);

		if (levelUpScreen.activeSelf)
		{
			RefreshAbilities();

			strengthText.text = CombatController.playerCombatController.myStats.baseStrength.ToString();
			dexterityText.text = CombatController.playerCombatController.myStats.baseDexterity.ToString();
			intelligenceText.text = CombatController.playerCombatController.myStats.baseIntelligence.ToString();
			luckText.text = CombatController.playerCombatController.myStats.baseLuck.ToString();

            confirmButton.gameObject.SetActive(false);

            traitPointToSpendText.text = TRAIT_POINTS_DEFAULT_STRING + traitPointsToSpend;
            abilityPointsToSpendText.text = ABILITY_POINTS_DEFAULT_STRING + abilityPointsToSpend;

			ToggleArrowButtons();
			CombatController.playerCombatController.CloseAllCombatUI();
		}
	}

	void ToggleArrowButtons()
	{
		bool _hasTraitPoints = (traitPointsToSpend > 0);
		strengthPlusButton.gameObject.SetActive(_hasTraitPoints);
		dexterityPlusButton.gameObject.SetActive(_hasTraitPoints);
		intelligencePlusButton.gameObject.SetActive(_hasTraitPoints);
		luckPlusButton.gameObject.SetActive(_hasTraitPoints);

		strengthMinusButton.gameObject.SetActive(strengthChange > 0);
		dexterityMinusButton.gameObject.SetActive(dexterityChange > 0);
		intelligenceMinusButton.gameObject.SetActive(intelligenceChange > 0);
		luckMinusButton.gameObject.SetActive(luckChange > 0);
    }
}
