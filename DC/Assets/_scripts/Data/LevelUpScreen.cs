using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AbilityInfo;

public class LevelUpScreen : AbilityCollection
{
	public static LevelUpScreen instance;

	private readonly List<AbilityChoices> levelUpQueue = new List<AbilityChoices>();
	public LevelGroup currentGroup = adventurerGroup;
	private static readonly LevelGroup adventurerGroup = new LevelGroup("Adventurer", 2, 2, 
		new List<AbilityChoices>() {
			new AbilityChoices(spotWeakness                         ),
			new AbilityChoices(wildPunch,       tiltSwing	        ),
			new AbilityChoices(lifeTap,         doubleKick,      heal),
			new AbilityChoices(siphonSoul,      fireball            ),
			new AbilityChoices(forcePunch,      regeneration        ),
			new AbilityChoices(chaosThesis,     divineLuck          ),
			new AbilityChoices(clense,          hardenSkin          ),
			new AbilityChoices(bulkUp,          magicShield         ),
			new AbilityChoices(restoreSoul,     debulk              ),
		});

	/*
	private int addedAdventurerLevels, addedWizardLevels;
	private List<AbilityChoices> adventurerLevelLine = new List<AbilityChoices>()
	{
		new AbilityChoices(spotWeakness							),
		new AbilityChoices(wildPunch,		doubleKick			),
		new AbilityChoices(lifeTap,			tiltSwing,		heal),
		new AbilityChoices(siphonSoul,		fireball			),
		new AbilityChoices(forcePunch,		regeneration		),
		new AbilityChoices(chaosThesis,		divineLuck			),
		new AbilityChoices(clense,			hardenSkin			),
		new AbilityChoices(bulkUp,			magicShield			),
		new AbilityChoices(restoreSoul,		debulk				),
	};

	private List<AbilityChoices> wizardLevelLine = new List<AbilityChoices>()
	{
		new AbilityChoices(focus					            ),
		new AbilityChoices(fireball,	    doubleKick          ),
		new AbilityChoices(manaDrain,       tiltSwing,      heal),
		new AbilityChoices(massExplosion,   fireball            ),
		new AbilityChoices(forcePunch,      regeneration        ),
		new AbilityChoices(chaosThesis,     divineLuck          ),
		new AbilityChoices(meteorShower,    hardenSkin          ),
		new AbilityChoices(bulkUp,          magicShield         ),
		new AbilityChoices(timeWarp,		debulk              ),

	};
	*/

	private int strengthChange, dexterityChange, intelligenceChange, luckChange;
	public int traitPointsToSpend = 0;// DebugController.bonusAbilityPoints;// abilityPointsToSpend;

    private const string TRAIT_POINTS_DEFAULT_STRING = "Trait points: ", ABILITY_POINTS_DEFAULT_STRING = "Ability points: ";
	//private Text classText;

	//private Button changeClassButton;
	//private Button abilityPickButton1, abilityPickButton2, abilityPickButton3;
	//private Text abilityPickText1, abilityPickText2, abilityPickText3;
	private ToolTip abilityButton1ToolTip, abilityButton2ToolTip, abilityButton3ToolTip;
	private Button cancelButton, confirmButton;

	public struct AbilityChoices
	{
		public AbilityChoices(Ability _option1) : this(_option1, null, null) { }

		public AbilityChoices(Ability _option1, Ability _option2) : this(_option1, _option2, null) { }

		public AbilityChoices(Ability _option1, Ability _option2, Ability _option3)
		{
			option1 = _option1;
			option2 = _option2;
			option3 = _option3;
			used = false;
		}

		public Ability option1, option2, option3;
		public bool used;
	}

	public class LevelGroup
	{
		public string className;
		public int levels = 0;
		public int healthPerLevel, manaPerLevel;
		public List<Buff> passiveBuffs;
		public List<AbilityChoices> abilityUnlocks;

		public LevelGroup(string _name, int _health, int _mana, List<AbilityChoices> _abilityChoices, List<Buff> _passiveBuffs = null)
		{
			className = _name;
			healthPerLevel = _health;
			manaPerLevel = _mana;
			passiveBuffs = (_passiveBuffs == null) ? new List<Buff>() : _passiveBuffs;
			abilityUnlocks = _abilityChoices;
		}
	}

	/// <summary>
	/// Needs to be initialized as this is not a monobehaviour.
	/// </summary>
	public void Initialize()
	{
		abilityButton1ToolTip = UIController.LevelUpPickAbilityButton1.GetComponent<ToolTip>();
		abilityButton2ToolTip = UIController.LevelUpPickAbilityButton2.GetComponent<ToolTip>();
		abilityButton3ToolTip = UIController.LevelUpPickAbilityButton3.GetComponent<ToolTip>();

		UIController.LevelUpButton.onClick.AddListener(delegate { UIController.SetUIMode(UIController.UIMode.LevelUp); ToggleLevelUpScreen(); });

		instance = this;

		//playerCombatController = CombatController.playerCombatController;//GameObject.Find("$Player").GetComponent<CombatController>();

		var _UICanvas = GameObject.Find("$UICanvas");
		foreach(Transform _t in _UICanvas.GetComponentsInChildren<Transform>(true))
		{
			switch(_t.name)
			{

				case "$CancelButton":
					cancelButton = _t.GetComponent<Button>();
					cancelButton.onClick.AddListener(delegate
					{
						traitPointsToSpend = strengthChange + dexterityChange + intelligenceChange + luckChange + traitPointsToSpend;
						strengthChange = dexterityChange = intelligenceChange = luckChange = 0;

						UIController.SetUIMode(UIController.UIMode.None);

						//ToggleLevelUpScreen();
					});

					break;
				case "$ConfirmButton":
					confirmButton = _t.GetComponent<Button>();
					confirmButton.onClick.AddListener(delegate {
                        CombatController.playerCombatController.MyStats.baseStrength += strengthChange;
                        CombatController.playerCombatController.MyStats.baseDexterity += dexterityChange;
                        CombatController.playerCombatController.MyStats.baseIntelligence += intelligenceChange;
                        CombatController.playerCombatController.MyStats.baseLuck += luckChange;

						strengthChange = dexterityChange = intelligenceChange = luckChange = 0;

						UIController.LevelUpStrengthCurrentTraitText.text = CombatController.playerCombatController.MyStats.baseStrength.ToString();
						UIController.LevelUpDexterityCurrentTraitText.text = CombatController.playerCombatController.MyStats.baseDexterity.ToString();
						UIController.LevelUpIntelligenceCurrentTraitText.text = CombatController.playerCombatController.MyStats.baseIntelligence.ToString();
						UIController.LevelUpLuckCurrentTraitText.text = CombatController.playerCombatController.MyStats.baseLuck.ToString();

                        confirmButton.gameObject.SetActive(false);

						ToggleArrowButtons();
					});
					break;
			}
		}

		UIController.LevelUpPickAbilityButton1.onClick.AddListener(delegate {
			Debug.Log("queue count: " + levelUpQueue.Count + " option 1: " + levelUpQueue[0].option1.name);

			AddAbility(levelUpQueue[0].option1);
		});

		UIController.LevelUpPickAbilityButton2.onClick.AddListener(delegate {
			Debug.Log("queue count: " + levelUpQueue.Count + " option 2: " + levelUpQueue[0].option2.name);

			AddAbility(levelUpQueue[0].option2);
		});

		UIController.LevelUpPickAbilityButton3.onClick.AddListener(delegate {
			Debug.Log("queue count: " + levelUpQueue.Count + " option 3: " + levelUpQueue[0].option3.name);
			AddAbility(levelUpQueue[0].option3);
		});

		UIController.LevelUpStrengthChangeButtons.maxObject.onClick.AddListener(delegate {
			UpdateTraitText(ref strengthChange, 1, CombatController.playerCombatController.MyStats.baseStrength, UIController.LevelUpStrengthCurrentTraitText, UIController.LevelUpStrengthChangeButtons.minObject);
		});
		UIController.LevelUpStrengthChangeButtons.minObject.onClick.AddListener(delegate {
			UpdateTraitText(ref strengthChange, -1, CombatController.playerCombatController.MyStats.baseStrength, UIController.LevelUpStrengthCurrentTraitText, UIController.LevelUpStrengthChangeButtons.minObject);
		});
		UIController.LevelUpDexterityChangeButtons.maxObject.onClick.AddListener(delegate {
			UpdateTraitText(ref dexterityChange, 1, CombatController.playerCombatController.MyStats.baseDexterity, UIController.LevelUpDexterityCurrentTraitText, UIController.LevelUpDexterityChangeButtons.minObject);
		});
		UIController.LevelUpDexterityChangeButtons.minObject.onClick.AddListener(delegate
		{
			UpdateTraitText(ref dexterityChange, -1, CombatController.playerCombatController.MyStats.baseDexterity, UIController.LevelUpDexterityCurrentTraitText, UIController.LevelUpDexterityChangeButtons.minObject);
		});
		UIController.LevelUpIntellectChangeButtons.maxObject.onClick.AddListener(delegate {
			UpdateTraitText(ref intelligenceChange, 1, CombatController.playerCombatController.MyStats.baseIntelligence, UIController.LevelUpIntelligenceCurrentTraitText, UIController.LevelUpIntellectChangeButtons.minObject);
		});
		UIController.LevelUpIntellectChangeButtons.minObject.onClick.AddListener(delegate
		{
			UpdateTraitText(ref intelligenceChange, -1, CombatController.playerCombatController.MyStats.baseIntelligence, UIController.LevelUpIntelligenceCurrentTraitText, UIController.LevelUpIntellectChangeButtons.minObject);
		});
		UIController.LevelUpLuckChangeButtons.maxObject.onClick.AddListener(delegate {
			UpdateTraitText(ref luckChange, 1, CombatController.playerCombatController.MyStats.baseLuck, UIController.LevelUpLuckCurrentTraitText, UIController.LevelUpLuckChangeButtons.minObject);
		});
		UIController.LevelUpLuckChangeButtons.minObject.onClick.AddListener(delegate
		{
			UpdateTraitText(ref luckChange, -1, CombatController.playerCombatController.MyStats.baseLuck, UIController.LevelUpLuckCurrentTraitText, UIController.LevelUpLuckChangeButtons.minObject);
		});
	}

	public void AddNextChoicesToQue()
	{
		if (currentGroup.abilityUnlocks.Count > currentGroup.levels) //if the current unlocks are higher than the level
		{
			levelUpQueue.Add(currentGroup.abilityUnlocks[currentGroup.levels]); //add the next abilities to the choice pool
			currentGroup.levels++; //and increase the level
		}

		/*
		if (adventurerLevelLine.Count > addedAdventurerLevels)
		{
			levelUpQue.Add(adventurerLevelLine[addedAdventurerLevels]);
			addedAdventurerLevels++;
		}
		*/
	}

	void AddAbility(Ability _abilityToAdd)
	{
		if (levelUpQueue.Count <= 0) return;

		CombatController.playerCombatController.MyStats.abilities.Add(_abilityToAdd);
		CombatController.playerCombatController.RefreshAbilityList();
		levelUpQueue.Remove(levelUpQueue[0]);
		RefreshAbilities();
	}

	void RefreshAbilities()
	{
		bool _hasAbilityQueue = (levelUpQueue.Count > 0);
		bool _hasOption1 = (_hasAbilityQueue) ? (levelUpQueue[0].option1 != null): false;
		bool _hasOption2 = (_hasAbilityQueue) ? (levelUpQueue[0].option2 != null): false;
		bool _hasOption3 = (_hasAbilityQueue) ? (levelUpQueue[0].option3 != null): false;

		UIController.LevelUpPickAbilityButton1.gameObject.SetActive(_hasOption1);
		UIController.LevelUpPickAbilityButton2.gameObject.SetActive(_hasOption2);
		UIController.LevelUpPickAbilityButton3.gameObject.SetActive(_hasOption3);

		if (_hasAbilityQueue)
		{
			if (_hasOption1)
			{
		 		//UIController.LevelUpPickAbilityButtonText1.text = (_hasOption1)? levelUpQueue[0].option1.name : string.Empty;
		 		UIController.LevelUpPickAbilityButtonText1.text = levelUpQueue[0].option1.name;
				abilityButton1ToolTip.ChangeToolTipText(levelUpQueue[0].option1.description);
			}
			if (_hasOption2)
			{
				UIController.LevelUpPickAbilityButtonText2.text = levelUpQueue[0].option2.name;
				abilityButton2ToolTip.ChangeToolTipText(levelUpQueue[0].option2.description);
			}
			if (_hasOption3)
			{
				UIController.LevelUpPickAbilityButtonText3.text = levelUpQueue[0].option3.name;
				abilityButton3ToolTip.ChangeToolTipText(levelUpQueue[0].option3.description);
			}
		}
		else
		{
			abilityButton1ToolTip.OnPointerExit(null);
			abilityButton2ToolTip.OnPointerExit(null);
			abilityButton3ToolTip.OnPointerExit(null);
		}
	}



	public void UpdateTraitText(ref int _change, int _difference, int _traitScore, Text _textToChange, Button _minusButton)
	{
		_change += _difference;
		traitPointsToSpend -= _difference;
		_minusButton.gameObject.SetActive(_change > 0);
		//_plusButton.gameObject.SetActive(traitPointsToSpend > 0);

		UIController.LevelUpStrengthChangeButtons.maxObject.gameObject.SetActive(traitPointsToSpend > 0);
		UIController.LevelUpDexterityChangeButtons.maxObject.gameObject.SetActive(traitPointsToSpend > 0);
		UIController.LevelUpIntellectChangeButtons.maxObject.gameObject.SetActive(traitPointsToSpend > 0);
		UIController.LevelUpLuckChangeButtons.maxObject.gameObject.SetActive(traitPointsToSpend > 0);

        UIController.LevelUpTraitPointsToSpendText.text = TRAIT_POINTS_DEFAULT_STRING + traitPointsToSpend;
        confirmButton.gameObject.SetActive(strengthChange + dexterityChange + intelligenceChange + luckChange > 0);
		//strengthMinusButton.gameObject.SetActive(_change > 0);
		//dexterityMinusButton.gameObject.SetActive(_change > 0);
		//intelligenceMinusButton.gameObject.SetActive(_change > 0);
		//luckMinusButton.gameObject.SetActive(_change > 0);


		//_textToChange.text = (_traitScore + _change) + ((_change > 0)? "(+" + _change + ")": "");
		string _changeString = (_change > 0) ? $"(<color=#00AA00>+{_change}</color>)" : string.Empty;
		_textToChange.text = $"{_changeString} {_traitScore + _change}";// (_traitScore + _change) + ((_change > 0) ? "(+" + _change + ")" : "");

	}


	void ToggleLevelUpScreen()
	{
		if (CombatController.playerCombatController.actedLastTick)
		{
			UIController.SetUIMode(UIController.UIMode.None);
			return;
		}
		//if (CombatController.playerCombatController.selectedAbility != string.Empty) return;

		//UIController.LevelUpScreen.SetActive(!UIController.LevelUpScreen.activeSelf);

		if (UIController.IsCurrentUIMode(UIController.UIMode.LevelUp))//UIController.LevelUpScreen.activeSelf)
		{
			RefreshAbilities();

			UIController.LevelUpStrengthCurrentTraitText.text = CombatController.playerCombatController.MyStats.baseStrength.ToString();
			UIController.LevelUpDexterityCurrentTraitText.text = CombatController.playerCombatController.MyStats.baseDexterity.ToString();
			UIController.LevelUpIntelligenceCurrentTraitText.text = CombatController.playerCombatController.MyStats.baseIntelligence.ToString();
			UIController.LevelUpLuckCurrentTraitText.text = CombatController.playerCombatController.MyStats.baseLuck.ToString();

            confirmButton.gameObject.SetActive(false);

            UIController.LevelUpTraitPointsToSpendText.text = TRAIT_POINTS_DEFAULT_STRING + traitPointsToSpend;
			UIController.LevelUpAbilityPointsToSpendText.text = ABILITY_POINTS_DEFAULT_STRING + levelUpQueue.Count;// + abilityPointsToSpend;

			ToggleArrowButtons();
			//CombatController.playerCombatController.CloseAllCombatUI();
		}
	}

	void ToggleArrowButtons()
	{
		bool _hasTraitPoints = (traitPointsToSpend > 0);

		UIController.LevelUpStrengthChangeButtons.maxObject.gameObject.SetActive(_hasTraitPoints);
		UIController.LevelUpDexterityChangeButtons.maxObject.gameObject.SetActive(_hasTraitPoints);
		UIController.LevelUpIntellectChangeButtons.maxObject.gameObject.SetActive(_hasTraitPoints);
		UIController.LevelUpLuckChangeButtons.maxObject.gameObject.SetActive(_hasTraitPoints);

		UIController.LevelUpStrengthChangeButtons.minObject.gameObject.SetActive(strengthChange > 0);
		UIController.LevelUpDexterityChangeButtons.minObject.gameObject.SetActive(dexterityChange > 0);
		UIController.LevelUpIntellectChangeButtons.minObject.gameObject.SetActive(intelligenceChange > 0);
		UIController.LevelUpLuckChangeButtons.minObject.gameObject.SetActive(luckChange > 0);
    }
}
