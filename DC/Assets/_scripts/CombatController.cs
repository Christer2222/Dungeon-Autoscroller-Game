using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using AbilityInfo;
using System;

public class CombatController : AbilityScript
{
	//Keeps track of whos turn it is, and switching
	public static List<CombatController> turnOrder = new List<CombatController>();
	private bool playerOwned;
	public bool startedTurn;
	public static int turnCounter;
	private bool processingAbility;

	//Shows the player their status
	private Coroutine healthMove, manaMove, xpMove;
	private const float SLOW_SLIDER_RATIO_TO_NORMAL = 100;
	private static GameObject buffEntryPrefab;
	private Text levelUpText;
	private static int xpPoolToaddAfterCombat;

	//game stats that change often
	public StatBlock myStats;
	public int currentHealth;
	public int currentMana;

	//Gameover variables
	private EnemyMover myEnemyMover;

	//Variables for targeting, and using abilities
	public Ability selectedAbility;
	private CombatController targetCombatController;
	public static CombatController playerCombatController;
	private bool isCritted;
	private Color abilityActiveColor = new Color(0, 0, 0.35f), abilityInactive = Color.gray;
	private const float MAX_ABILITIES_ON_SCREEN = 6.5f;

	public bool actedLastTick;
	private bool invokingAbility;

	//variables for player ability toggeling
	private static GameObject entryPrefab;

	public static void ClearAllValues()
	{
		turnOrder.Clear();
		playerCombatController = null;
		mainCamera = null;
		turnCounter = 0;
	}

	private void Start()
	{
        if (playerCombatController == null)
        {
            playerCombatController = GameObject.Find("$PlayerPortrait").GetComponent<CombatController>();
            playerCombatController.playerOwned = true;
        }

		if(mainCamera == null) mainCamera = Camera.main;
			//damageText = transform.Find("$DamageTextHolder").Find("$DamageText").GetComponent<Text>();


		//Set stats for enemies and the player + ui for player
		if (playerOwned)
		{
			myStats = new StatBlock(
				StatBlock.Race.Human,
				"Player",
				10, 15, //hp, mp
				1, 0, //lv, xp
				1, 1, 1, 1, //str, dex. int, luck
				//new List<Ability> { AbilityClass.punch});
				new List<Ability> { AbilityClass.punch },
				_drops: DropTable.none
				);

			entryPrefab = Resources.Load<GameObject>("Prefabs/$Entry");
			buffEntryPrefab = Resources.Load<GameObject>("Prefabs/$BuffEntry");

#region UI Assignment


			UIController.AbilityButton.onClick.AddListener(delegate {
				//if (/*playerCombatController.CheckIfHasBuff("Busy") ||*/ UIController.LevelUpScreen.activeSelf) return;

				//UIController.AbilityMenuScrollView.SetActive(!UIController.AbilityMenuScrollView.activeSelf);
				//UIController.FleeSlider.gameObject.SetActive(false);
				playerCombatController.ResetAbilityPick();
			});

			UIController.FleeButton.onClick.AddListener(delegate {

				//if (LevelUpScreen.levelUpScreen.activeSelf) return;
				ResetAbilityPick();

				if (turnOrder.Count != 0)
					if (turnOrder[0] == playerCombatController && !processingAbility)
						UIController.FleeSlider.GetComponent<FleeLogic>().enabled = true;
					else
						UIController.FleeSlider.gameObject.SetActive(false);

			});

			UIController.InventoryButton.onClick.AddListener(delegate { ResetAbilityPick(); });

			

			#endregion

			CheckIfBuffIconsAreCorrect();

#region Stat Reset
			//get current stats
			currentHealth = myStats.maxHealth;
			currentMana = myStats.maxMana;

			RefreshAbilityList();

			UIController.XPSlider.maxValue = myStats.level * 10;

			//set the normal bars to their max value
			UIController.HealthSliderPair.minObject.maxValue = myStats.maxHealth;
			UIController.ManaSliderPair.minObject.maxValue = myStats.maxMana;

			//show values are at current status
			UIController.HealthSliderPair.minObject.value = currentHealth;
			UIController.ManaSliderPair.minObject.value = currentMana;

			//update texts to min/max status
			UIController.HealthTextPair.minObject.text = currentHealth.ToString();
			UIController.HealthTextPair.maxObject.text = myStats.maxHealth.ToString();
			UIController.ManaTextPair.minObject.text = currentMana.ToString();
			UIController.ManaTextPair.maxObject.text = myStats.maxMana.ToString();

			//update slow sliders
			UIController.HealthSliderPair.maxObject.maxValue = UIController.HealthSliderPair.minObject.maxValue * SLOW_SLIDER_RATIO_TO_NORMAL;
			UIController.HealthSliderPair.maxObject.value = UIController.HealthSliderPair.maxObject.maxValue;
			UIController.ManaSliderPair.maxObject.maxValue = UIController.ManaSliderPair.minObject.maxValue * SLOW_SLIDER_RATIO_TO_NORMAL;
			UIController.ManaSliderPair.maxObject.value = UIController.ManaSliderPair.maxObject.maxValue;


			UIController.XPSlider.value = myStats.xp;
#endregion

			if (BuffIcons.buffIconDictionary == null)
			{
				BuffIcons.buffIconDictionary = new Dictionary<string, Sprite>();
				Sprite[] _buffIcons = Resources.LoadAll<Sprite>("Sprites/UI/StatusSpriteSheet");
				for (int i = 0; i < _buffIcons.Length; i++)
				{
					BuffIcons.buffIconDictionary.Add(_buffIcons[i].name, _buffIcons[i]);
				}
			}
		}
		else
		{
			myEnemyMover = GetComponent<EnemyMover>();

			currentHealth = myStats.maxHealth;
			currentMana = myStats.maxMana;
		}

		if (myStats.idleAnimation != null && !playerOwned)
		{
			gameObject.AddComponent<AnimationHandler>();
		}
	}

	/// <summary>
	/// Update available buttons
	/// </summary>
	public void RefreshAbilityList()
	{
		var _children = UIController.AbilityMenuContent.GetComponentsInChildren<Transform>(); //get all children already here

		for (int i = 1; i < _children.Length; i++) //exclude parent by starting at 1
		{
			Destroy(_children[i].gameObject); //and clear all children
		}

		int _abilityCount = myStats.abilities.Count; //for each ability the player has
		for (int i = 0; i < _abilityCount; i++) //go through them
		{
			string _s = "";
			_s = myStats.abilities[i].name;


			GameObject _go = Instantiate(entryPrefab, UIController.AbilityMenuContent.transform); //spawn a new entry for each ability
			//_go.transform.localPosition = Vector3.zero; //new Vector3(350,-10 + -(i + 0.5f) * 170,0); //place it
			_go.transform.Find("$Text").GetComponent<Text>().text = _s; //write what ability the button selects

			int _index = i; //store the current index
			_go.GetComponent<Button>().onClick.AddListener(delegate {
				UIController.AbilityMenuScrollView.gameObject.SetActive(false); //deactivate the menu
				UIController.AbilityButtonText.text = _s; //set the name of the button to the ability
				selectedAbility = myStats.abilities[_index]; //set ability to this ability
			});

		}

		//UIController.AbilityMenuContent.sizeDelta = new Vector2(0,(count - 1) * 113 + 10); //set size to fit all entries
		UIController.AbilityMenuContent.sizeDelta = new Vector2(0, (_abilityCount) * 110 + 10); //set size to fit all entries

		var oMax = UIController.AbilityMenuScrollView.offsetMax; //get the height
		oMax.y = UIController.AbilityMenuScrollView.offsetMin.y + (Mathf.Min(_abilityCount, MAX_ABILITIES_ON_SCREEN)) * 110 + 10; //set height to bottom + maxShowCount * height + offset
		UIController.AbilityMenuScrollView.offsetMax = oMax; //apply

		UpdateAbilitiesToManaAvailability(); //set colors
	}

	IEnumerator EndActedLastTick()
	{
		yield return new WaitForEndOfFrame();
		actedLastTick = false;
	}

	void Update()
	{
        if (Input.GetMouseButtonUp(0))
        {
			actedLastTick = false;
        }


		if (turnOrder.Count != 0) //if there are combatants
		{
			if (turnOrder[0] == this) //if it is this actors turn
			{
				StartCoroutine(StartOfTurnActions());
			}
		}
		
		if (playerOwned)
		{
			if(Input.GetMouseButtonDown(0)) Click(); //wait for clicks outside of the players turn
		}
	}

	IEnumerator StartOfTurnActions()
	{
		if (!startedTurn)
		{
			startedTurn = true;
			turnCounter++;

			//Invoke buffs
			yield return TickBuffs();

			//UpdateTurnOrder turn order
			UpdateTurnOrderDisplay();

			if (turnOrder[0] == playerOwned || turnCounter == 1)
			{
				var _playerTurnText = EffectTools.SpawnText(Vector3.zero, UIController.UICanvas, (turnCounter == 1) ? new Color(0.8f, 0.4f, 0) : Color.yellow + Color.red, (turnCounter == 1) ? "Combat!" : "Your Turn!", 150);
				_playerTurnText.transform.parent.localPosition = Vector3.zero;
				_playerTurnText.StartCoroutine(EffectTools.ActivateInOrder(_playerTurnText,
					new List<EffectTools.FunctionAndDelay>()
					{
									new EffectTools.FunctionAndDelay(EffectTools.StretchFromTo(_playerTurnText.transform, new Vector3(2, 0, 0), Vector3.one, 1f),   0),
									new EffectTools.FunctionAndDelay(new List<IEnumerator>()
										{
											EffectTools.MoveDirection(_playerTurnText.transform,Vector3.right,100,5),
											EffectTools.StretchFromTo(_playerTurnText.transform, _playerTurnText.transform.localScale, new Vector3(2, 1, 0), 2f),
										},2f)
					}
					));

				Destroy(_playerTurnText.transform.parent.gameObject, 3);
			}

			if (!playerOwned)
			{
				yield return new WaitForEndOfFrame();
				myEnemyMover.shouldMove = false;
				if (currentHealth > 0)
					StartCoroutine(TakeEnemyTurn());
			}
		}

		
		//if (playerOwned)
		//{
		//	if (Input.GetMouseButtonDown(0)) Click(); //wait for clicks
		//}
		
	}

	public static void UpdateTurnOrderDisplay()
	{
		UIController.TurnOrderText.text = string.Empty;
		if (turnOrder.Count > 1)
			for (int i = 0; i < turnOrder.Count; i++)
			{
				UIController.TurnOrderText.text += turnOrder[i].myStats.name + " | ";
			}
	}
	
	/// <summary>
	/// Call each buff, and decrement their turn counter
	/// </summary>
	public IEnumerator TickBuffs()
	{
		targetCombatController = this;
		var _prevActiveAbility = selectedAbility;

		for(int i = 0; i < myStats.buffList.Count; i++)
		{
			var _buff = myStats.buffList[i];
			for (int j = 0; j < _buff.functions.Count; j++)
			{
				selectedAbility = _buff.functions[j];
				yield return StartCoroutine(InvokeActiveAbility(false,_buff.constant));
			}
			_buff.turns--;
		}

		myStats.buffList.RemoveAll(x => x.turns <= 0);

		selectedAbility = _prevActiveAbility;//null;
		targetCombatController = null;

		if (playerOwned)
			CheckIfBuffIconsAreCorrect();
	}

	public bool CheckIfHasBuff(string _buffName)
	{
		return (myStats.buffList.Exists(x => x.name == _buffName));
	}

	public void RemoveAllBufsWithName(string _buffName)
	{
		myStats.buffList.RemoveAll(x => x.name.Contains(_buffName));
	}

	/// <summary>
	/// Refreshes the buff sidebar.
	/// </summary>
	void CheckIfBuffIconsAreCorrect()
	{
		List<Text> _buffHolderTurnList = UIController.BuffContent.GetComponentsInChildren<Text>(true).Where(x => x.transform.name == "$BuffTurns").ToList();
		List<string> _uncheckedBuffs = UIController.BuffContent.GetComponentsInChildren<Text>(true).Where(x => x.transform.name == "$BuffName").Select(y => y.text).ToList();
		int _amount = _buffHolderTurnList.Count;

		for (int i = 0; i < myStats.buffList.Count; i++)
		{
			int _currentIndex = _uncheckedBuffs.IndexOf(myStats.buffList[i].name);
			if (_currentIndex != -1)
			{
				//update
				_buffHolderTurnList[_currentIndex].text = "Turns: " + myStats.buffList[i].turns;
				_buffHolderTurnList.Remove(_buffHolderTurnList[_currentIndex]);
				_uncheckedBuffs.Remove(_uncheckedBuffs[_currentIndex]);
			}
			else
			{
				//create
				_amount++;
				var _go = Instantiate(buffEntryPrefab, UIController.BuffContent);
				var _children = _go.GetComponentsInChildren<Text>(true);
				var _info = _go.transform.GetChild(0).GetChild(0).gameObject;
				var _parent = _go.transform.GetChild(0);
				_parent.GetComponent<Button>().onClick.AddListener(delegate { _info.SetActive(!_info.activeSelf); });
				_parent.GetComponent<Image>().sprite = myStats.buffList[i].buffIcon;
				for (int j = 0; j < _children.Length; j++)
				{
					switch (_children[j].transform.name)
					{
						case "$BuffName":
							_children[j].text = myStats.buffList[i].name;
							break;
						case "$BuffTurns":
							_children[j].text = "Turns: " + myStats.buffList[i].turns;
							break;
						case "$BuffDescription":
							_children[j].text = string.Empty;
							for (int k = 0; k < myStats.buffList[i].functions.Count; k++)
							{
								_children[j].text += myStats.buffList[i].functions[k]; //TODO: Add description to all buffs
								if (k < myStats.buffList[i].functions.Count) _children[j].text += "\n";
							}

							for (int k = 0; k < myStats.buffList[i].traits.Count; k++)
							{
								_children[j].text += myStats.buffList[i].traits[k]; //TODO: Add description to all buffs
								if (k < myStats.buffList[i].traits.Count) _children[j].text += "\n";
							}
							break;
					}

				}
			}
		}

		for (int i = 0; i < _uncheckedBuffs.Count; i++)
		{
			//destroy rest
			_amount--;
			Destroy(_buffHolderTurnList[i].transform.parent.parent.parent.gameObject);
		}

		if (_amount > 8)
		{
			UIController.BuffScrollRect.vertical = true;
			UIController.BuffScrollImage.color = Color.white * 0.3f;
			//EffectTools.BlinkImage(buffScrollImage,Color.white,1,0.5f);
		}
		else
		{
			UIController.BuffScrollRect.vertical = false;
			UIController.BuffScrollImage.color = Color.clear;
		}
	}

	IEnumerator TakeEnemyTurn()
	{
		yield return new WaitForSeconds(1);

		selectedAbility = EnemyAI.SelectAbility(myStats, currentHealth, currentMana);

		//_playerTurnText.transform.parent.localPosition = Vector3.zero;

		yield return StartCoroutine(EnemyAI.SpawnAbilityTextUsed(transform, UIController.UICanvas, myStats, selectedAbility, this));

		yield return new WaitForSeconds(0.3f);

		//if the seleced ability has any bit set to any of the accepted types
		if (((AbilityType.buff | AbilityType.defensive | AbilityType.recovery | AbilityType.misc) & (selectedAbility.abilityType)) != 0)
		{
			targetCombatController = this;
			lastClick = transform.position;
		}
		else
		{
			targetCombatController = playerCombatController;
			lastClick = playerCombatController.transform.position;
		}


		yield return StartCoroutine(InvokeActiveAbility());
		myEnemyMover.shouldMove = true;


	}

	public void AdjustPlayerXP(int _amount)
	{
		myStats.xp += _amount;

		MoveXPSlider();

		//xpSlider.value = myStats.xp;
		if (levelUpText == null && myStats.xp >= UIController.XPSlider.maxValue)
		{
			levelUpText = EffectTools.SpawnText(transform.position, transform, Color.yellow, "LEVEL UP!");
			levelUpText.StartCoroutine(EffectTools.MoveDirection(levelUpText.transform, Vector3.up, 1, 2));
			levelUpText.StartCoroutine(EffectTools.BlinkText(levelUpText, Color.red + Color.green * 0.75f, 5));
			Destroy(levelUpText.gameObject,4);
		}

		while (myStats.xp >= UIController.XPSlider.maxValue)
		{
			LevelUpScreen.traitPointsToSpend += 1;

			if ((myStats.level + 1) % 2 == 0)
			{
				LevelUpScreen.instance.AddNextChoicesToQue();
			}

			myStats.level++;
			myStats.xp -= (int)UIController.XPSlider.maxValue; //subtract this levels xp requirement
			UIController.XPSlider.value = myStats.xp; //then set the xp bar to show the current xp (which might be over the requirement for a levelup)
			UIController.XPSlider.maxValue = myStats.level * 10; //set the next levelup to be at 10 times the level

			MoveXPSlider();
		}
	}

	void MoveXPSlider()
	{
		if (xpMove != null)
		{
			//xpSlider.value = myStats.xp;
			StopCoroutine(xpMove);
		}

		xpMove = StartCoroutine(EffectTools.AnimateSlider(UIController.XPSlider, myStats.xp, 1, 1));
	}

	public int AdjustHealth(int _amount, Elementals _elementals, ExtraData _extraData)
	{
		if (currentHealth <= 0) return 0;

		float _amountMultiplier = 1;

		if ((myStats.weaknesses & _elementals) != 0) _amountMultiplier += 0.5f; //multiplier is booseted if weakness
		if ((myStats.resistances & _elementals) != 0) _amountMultiplier -= 0.5f; //multiplier is halfed if resist
		if ((myStats.immunities & _elementals) != 0) _amountMultiplier = 0; //multiplier is set to 0 if immune
		if ((myStats.absorbs & _elementals) != 0) _amount = Mathf.Abs(_amount); //always heal if absorb
		

		int _totalDamage = currentHealth; //store previous health for display text and return value

		if (_amount < 0) //if negative damage
			_amount += ((isCritted) ? -myStats.Strength : 0); //allow critting

		var _damageCalc = Mathf.RoundToInt(_amount * _amountMultiplier); //round up any damage/healing
		if (_damageCalc < 0 && _extraData.HasFlag(ExtraData.nonPiercing)) _damageCalc = Mathf.Min(_damageCalc + ((_extraData.HasFlag(ExtraData.magic))? myStats.Defense: myStats.MagicDefense), 0); //if value is blockable and is under 0, reduce damage by defense or magicDefense depending on if the move makes contact

		//if ((_amount > 0 && _damageCalc < 0) || (_amount < 0 && _damageCalc > 0)) _damageCalc = 0; //if the damage shifts sign somehow, set it to 0
		if (Mathf.Sign(_amount) != Mathf.Sign(_damageCalc)) _damageCalc = 0; //if the damage shifts sign somehow, set it to 0

		string _textToWrite = ((_damageCalc > 0)? "+" + _damageCalc.ToString(): (_damageCalc == 0)? "Block": _damageCalc.ToString()); //if over 0 +amount if 0 block if under 0 -amount

		Text _spawnedText = EffectTools.SpawnText(transform.position + Vector3.up, transform, //spawn a text with color
			(_damageCalc > 0)? Color.green: (_damageCalc == 0)? Color.gray: Color.red, _textToWrite); //if over 0 green if 0 gray if under red

		GameObject _spawnedTextParent = _spawnedText.transform.parent.gameObject;

		_spawnedText.StartCoroutine(EffectTools.CurveDropMove(_spawnedText.transform,4)); //curve drop the damage text
		_spawnedTextParent.transform.SetParent(null); //free the text
		_spawnedTextParent.transform.position = transform.position + Vector3.up;
		Destroy(_spawnedText.transform.parent.gameObject,5);

		currentHealth = Mathf.Clamp(currentHealth + _damageCalc, 0, myStats.maxHealth);
		_totalDamage -= currentHealth;

		if (_damageCalc < 0)
		{
			if (playerOwned)
			{
				float _magnitude = Mathf.Min(Mathf.Ceil(Mathf.Abs(_damageCalc) * 0.2f),3);
				StartCoroutine(EffectTools.Shake(mainCamera.transform,_magnitude,3));
			}
			else
			{
				Vector3 _orgScale = transform.localScale;
				StartCoroutine(EffectTools.ActivateInOrder(this, new List<EffectTools.FunctionAndDelay>()
				{
					new EffectTools.FunctionAndDelay(EffectTools.StretchFromTo(transform,_orgScale,_orgScale * 0.9f, 0.1f),0),
					new EffectTools.FunctionAndDelay(EffectTools.StretchFromTo(transform, transform.localScale, _orgScale, 0.1f), UnityEngine.Random.Range(0.1f,0.2f))
				}));
			}
		}

		if(playerOwned)
		{
			UIController.HealthTextPair.minObject.text = currentHealth.ToString();

			if (healthMove != null)
			{
				StopCoroutine(healthMove);
			}

			if (_damageCalc < 0)
			{
				UIController.HealthSliderPair.minObject.value = currentHealth;
				healthMove = StartCoroutine(EffectTools.AnimateSlider(UIController.HealthSliderPair.maxObject, UIController.HealthSliderPair.minObject, 3, SLOW_SLIDER_RATIO_TO_NORMAL));
			}
			else
			{
				UIController.HealthSliderPair.maxObject.value = currentHealth * SLOW_SLIDER_RATIO_TO_NORMAL;
				healthMove = StartCoroutine(EffectTools.AnimateSlider(UIController.HealthSliderPair.minObject, UIController.HealthSliderPair.maxObject, 1, 1/SLOW_SLIDER_RATIO_TO_NORMAL));
			}
		}

		if (currentHealth <= 0)
		{
			if (!playerOwned)
			{
				xpPoolToaddAfterCombat += myStats.level * 3;
				

				if (turnOrder.Count <= 2) //if this was the last enemy, and the player is left
				{
					playerCombatController.AdjustPlayerXP(xpPoolToaddAfterCombat);
					xpPoolToaddAfterCombat = 0;


					PlayerInventory.instance.ProcessDrops(myStats.drops);
				}

				myEnemyMover.shouldMove = false;

				StartCoroutine(RemoveFromTurnOrder(1.0f,this));
			}
			else
			{
				
				foreach(CombatController _cc in turnOrder)
				{
					RemoveFromTurnOrder(0,_cc);
					Destroy(_cc.gameObject);
				}

				UIController.GameOverHolder.transform.GetChild(0).gameObject.SetActive(true);
			}

		}

		return _totalDamage;
	}

	public int AdjustMana(int _amount)
	{
		var _manaLost = currentMana;
		currentMana = Mathf.Clamp(currentMana + _amount,0,myStats.maxMana);
		_manaLost -= currentMana;

		if(playerOwned)
		{
			UIController.ManaTextPair.minObject.text = currentMana.ToString();

			if (manaMove != null)
				StopCoroutine(manaMove);

			if (_manaLost > 0)
			{
				UIController.ManaSliderPair.minObject.value = currentMana;
				manaMove = StartCoroutine(EffectTools.AnimateSlider(UIController.ManaSliderPair.maxObject, UIController.ManaSliderPair.minObject, 1, SLOW_SLIDER_RATIO_TO_NORMAL));
			}
			else
			{
				UIController.ManaSliderPair.maxObject.value = currentMana * SLOW_SLIDER_RATIO_TO_NORMAL;
				manaMove = StartCoroutine(EffectTools.AnimateSlider(UIController.ManaSliderPair.minObject, UIController.ManaSliderPair.maxObject, 0.3f, 1 / SLOW_SLIDER_RATIO_TO_NORMAL));
			}

		}

		return _manaLost;
	}

	/// <summary>
	/// Toggles all buttons for which the player don't have enough mana for
	/// </summary>
	void UpdateAbilitiesToManaAvailability()
	{
		var _children = UIController.AbilityMenuContent.GetComponentsInChildren<Button>(); //Get all ability buttons
		for (int i = 0; i < _children.Length; i++) //go through them
		{
			ToggleButtonOnMana(_children[i]); //Set state based on players mana
		}
	}

	void ToggleButtonOnMana(Button _button)
	{
		string _buttonText = _button.GetComponentInChildren<Text>().text;//.ToLower(); //text of the button, to get the ability

		bool _hasEnoughMana = playerCombatController.currentMana >= -myStats.abilities.Find(x => x.name == _buttonText).manaCost; //playerCombatController.selectedAbility.manaCost;

		_button.GetComponentInChildren<Image>().color = (_hasEnoughMana)? abilityActiveColor: abilityInactive;
		_button.GetComponent<Button>().enabled = _hasEnoughMana;
		_button.GetComponent<Collider2D>().enabled = _hasEnoughMana;
	}

	public void Click()
	{
		RaycastHit2D _hit = CheckIfHit(HitPosition); //get click info
		
		bool _hitSomething = (_hit.collider != null); //store if something was hit

		if (_hitSomething) //what was hit
		{
			//print("name: " + _hit.transform.name + " actedLastTick: " + actedLastTick + " selectedAb: " + selectedAbility);
			
			if(/*_hit.transform.CompareTag("AbilityButton") ||*/ _hit.transform.CompareTag("UI")) //cancel if ui has been hit
			{
				return;
			}
		}

		if (selectedAbility != null)// && !UIController.FleeSlider.gameObject.activeSelf) //if not fleeing, but has an ability selected
		{
			if (_hit.transform != null) //if something was hit
			{
				if(_hit.transform.CompareTag("CritArea")) //if it was a crit area
				{
					targetCombatController = _hit.transform.GetComponentInParent<CombatController>();
					targetCombatController.isCritted = true;
				}
				else if (_hit.transform.name == "$PlayerPortrait") //if it was the player itself
				{
					print(selectedAbility.name + " self!");
					targetCombatController = playerCombatController;
				}
				else //if just the normal enemy body was hit
					targetCombatController = _hit.transform.GetComponent<CombatController>();
			}
			else //if an ability is selected, but the player missed
			{
				targetCombatController = null;
			}

			UIController.currentUIMode = UIController.UIMode.None;

			actedLastTick = true;
			StartCoroutine(InvokeActiveAbility()); //activate the ability with the information gathered

			if (targetCombatController != null) targetCombatController.isCritted = false;
			targetCombatController = null;

		}
	}

	public IEnumerator SimpleInvokeAbility(TargetData targetData, Ability ability, bool drainMana, bool endsTurn, Action onComplete = null)
	{
		if (invokingAbility) yield break;

		if (drainMana) AdjustMana(ability.manaCost);
		AddBusyIfNotInCombat();

		yield return StartCoroutine(ability.function(targetData));

		if (onComplete != null) onComplete.Invoke();

		if (endsTurn) StartCoroutine(EndTurn());
	}

	void AddBusyIfNotInCombat()
	{
		if (turnOrder.Count == 0)
		{
			AddBuff(new Buff("Busy", "busy", 1, BuffIcons.TryGetBuffIcon(13), Buff.StackType.Add_Duplicate, 1), this);

			ForwardMover.speedBoost = 0;
			ForwardMover.shouldMove = false;
			CheckIfBuffIconsAreCorrect();
		}
	}
	/// <summary>
	/// Calls the active ability
	/// </summary>
	public IEnumerator InvokeActiveAbility(bool _byUser = true, float? _value = null)
	{
		if (invokingAbility || (turnOrder.Count != 0 && turnOrder[0] != this || CheckIfHasBuff("Busy"))) yield break;
		invokingAbility = true;

		var _orgTime = Time.time;

		var _tempActiveAbility = selectedAbility;
		if (!_byUser)
		{
			lastClick = targetCombatController.transform.position;
			//actedLastTick = true;
		}
		else if (playerOwned)
		{

			processingAbility = true;

			ResetAbilityPick();
			AddBusyIfNotInCombat();
		}

		//yield return new WaitForEndOfFrame();
		//actedLastTick = false;

		var _targetData = new TargetData(_tempActiveAbility, this, targetCombatController, 0, _tempActiveAbility.element, lastClick, StatBlock.Race.Human);
		if (_tempActiveAbility.name.ToLower().Contains("undead")) _targetData.targetRace = StatBlock.Race.Undead;
		//var _targetData = new TargetData(this, targetCombatController, 0, selectedAbility.element, lastClick, StatBlock.Race.Human);
		//if (selectedAbility.name.ToLower().Contains("undead")) _targetData.targetRace = StatBlock.Race.Undead;

		if (_byUser)
		{
			//AdjustMana(selectedAbility.manaCost);
			AdjustMana(_tempActiveAbility.manaCost);
			if (playerOwned)
			{
				playerCombatController.UpdateAbilitiesToManaAvailability();
				ResetAbilityPick();
			}
		}

		yield return StartCoroutine(_tempActiveAbility.function(_targetData));//.Invoke(this, Punch(targetCombatController, myStats.strength, Elementals.Physical));
		//yield return StartCoroutine(selectedAbility.function(_targetData));//.Invoke(this, Punch(targetCombatController, myStats.strength, Elementals.Physical));

		if (_byUser)
		{
			while (Time.time - _orgTime < 1) //if it has been less than 1 sec since ability was used
				yield return new WaitForSeconds(0.1f); //wait

			playerCombatController.CheckIfBuffIconsAreCorrect();
			ForwardMover.shouldMove = true;

			yield return new WaitForSeconds( playerOwned? 1:2);
			StartCoroutine( EndTurn());
		}
		invokingAbility = false;
	}

	IEnumerator RemoveFromTurnOrder(float _sec, CombatController _target)
	{
		turnOrder.Remove(_target);
		yield return new WaitForSeconds(_sec);
		if(turnOrder.Count <= 1)
		{
			yield return ForwardMover.DoneWithCombat();

		}

		Destroy(_target.gameObject);
	}

	public IEnumerator EndTurn()
	{
		processingAbility = false;

		if(turnOrder.Count > 1)// && !CheckIfHasBuff(TEST.timeWarp.name))//"extra turn"))
		{
			turnOrder.Remove(this);
			turnOrder.Add(this);
		}

		if (turnOrder.Count <= 1)
		{
			UpdateTurnOrderDisplay();
			//turnorderText.text = string.Empty;
			yield return ForwardMover.DoneWithCombat();
		}

		startedTurn = false;
	}

	void ResetAbilityPick()
	{
		UIController.AbilityButtonText.text = "Abilities";
		selectedAbility = null;// string.Empty;
	}

	public void CloseAllCombatUI()
	{
		ResetAbilityPick();
		UIController.FleeSlider.gameObject.SetActive(false);
		UIController.AbilityMenuScrollView.gameObject.SetActive(false);
	}
}
