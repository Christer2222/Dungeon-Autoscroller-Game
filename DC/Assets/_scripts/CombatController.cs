using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CombatController : AbilityScript
{
	//Keeps track of whos turn it is, and switching
	public static List<CombatController> turnOrder = new List<CombatController>();
	private bool playerOwned;
	private bool startedTurn;

	//Shows the player their status
	private GameObject UICanvas;
	private Slider healthSlider, manaSlider, xpSlider;
	private Slider healthSliderSlow, manaSliderSlow;
	private Text currentHealthText, currentManaText, maxManaText, maxHealthText;
	private Coroutine healthMove, manaMove;
	private const float SLOW_SLIDER_RATIO_TO_NORMAL = 100;
	private Transform buffContent;
	private static GameObject buffEntryPrefab;

	//private Text damageText;
	//private static Button abilityButton1, abilityButton2, abilityButton3, abilityButton4;

	//game stats that change often
	public StatBlock myStats;
	public int currentHealth;
	public int currentMana;


	public static StatBlock ghostBlock				= new StatBlock(StatBlock.Race.Undead, "Ghost"				,2,2,1,0,1,3,1,1,new List<string> { SPOOK },_weaknesses: Elementals.Light,_absorbs: Elementals.Unlife, _immunities: Elementals.Physical, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock nosemanBlock			= new StatBlock(StatBlock.Race.Demon, "Noseman"				,2,0,1,0,1,1,1,1,new List<string> { PUNCH }, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock eyeballBlock			= new StatBlock(StatBlock.Race.Demon, "Eyeball"				,7,7,2,1,1,2,1,2,new List<string> { PUNCH, MANA_DRAIN },_aiType: StatBlock.AIType.Dumb);
	public static StatBlock lightElementalBlock		= new StatBlock(StatBlock.Race.Elemental, "Light Elemental"	,10,2,2,0,1,2,1,2,new List<string> { PUNCH, HEAL},_absorbs: Elementals.Light, _weaknesses: Elementals.Void,_aiType: StatBlock.AIType.Coward);
	public static StatBlock airElementalBlock		= new StatBlock(StatBlock.Race.Elemental, "Air Elemental"	,7,5,2,1,1,2,1,2,new List<string> { PUNCH },_absorbs: Elementals.Air, _weaknesses:  Elementals.Earth, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock earthElementalBlock		= new StatBlock(StatBlock.Race.Elemental, "Earth Elemental"	,15,5,2,1,1,2,1,2,new List<string> { PUNCH },_absorbs: Elementals.Earth, _weaknesses: Elementals.Air, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock fireElementalBlock		= new StatBlock(StatBlock.Race.Elemental, "Fire Elemental"	,5,5,2,1,1,2,1,2,new List<string> { PUNCH, FIREBALL },_absorbs: Elementals.Fire, _weaknesses: Elementals.Water,_aiType: StatBlock.AIType.Dumb);
	public static StatBlock waterElementalBlock		= new StatBlock(StatBlock.Race.Elemental, "Water Elemental"	,12,5,2,0,1,2,1,2,new List<string> { PUNCH, REGENERATION },_absorbs: Elementals.Water, _weaknesses: Elementals.Fire, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock harpyBlock				= new StatBlock(StatBlock.Race.Demon, "Harpy"				,10,5,2,0,1,2,1,2,new List<string> { PUNCH, BULK_UP },_aiType: StatBlock.AIType.Dumb);
	public static StatBlock druidBlock				= new StatBlock(StatBlock.Race.Elf, "Druid"					,5,10,2,0,1,2,1,2,new List<string> { PUNCH, HEAL, REGENERATION },_resistances: Elementals.Earth,_aiType: StatBlock.AIType.Coward);

	//Gameover variables
	private GameObject gameOverHolder;

	//Variables for targeting, and using abilities
	public string selectedAbility;
	private CombatController targetCombatController;
    public static CombatController playerCombatController;
	private Vector3 hitPosition;
	private bool isCritted;
	private Color activeColor = new Color(0,0,0.35f), deactiveColor = Color.gray;


	private GameObject inventoryScreen;

	private Button fleeButton;
	private Slider fleeSlider;
	private int fleeThreshold = 5;

    public bool actedLastTick;

	private static Camera mainCamera;

	//variables for player ability toggeling
	private GameObject buttonMenuScrollView, buttonMenuContent;
	private static GameObject entryPrefab;
	private Text abilityButtonText;

    private bool debugAbilities = true;
    private List<string> debugAbilityList = new List<string>() {TIME_WARP, BULK_UP, MANA_DRAIN, DIVINE_LUCK, REGENERATION, SPOT_WEAKNESS, SMITE_UNLIFE, SIPHON_SOUL, HEAL,
					LIFE_TAP, MASS_HEAL,FIREBALL, FOCUS, MASS_EXPLOSION, PUNCH, KEEN_SIGHT, SPOOK};

	public static void ClearAllValues()
	{
		turnOrder.Clear();
		//abilityButton1 = abilityButton2 = abilityButton3 = abilityButton4 = null;
		playerCombatController = null;
		mainCamera = null;
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
		if(gameObject.name == "$PlayerPortrait")
		{
            myStats = new StatBlock(
                StatBlock.Race.Human,
				"Player",
                10, 15, //hp, mp
                1, 0, //lv, xp
                1, 1, 1, 1, //str, dex. int, luck
                (debugAbilities) ? debugAbilityList : new List<string> { PUNCH });

			//playerOwned = true;

			entryPrefab = Resources.Load<GameObject>("Prefabs/$Entry");
			buffEntryPrefab = Resources.Load<GameObject>("Prefabs/$BuffEntry");

			#region UI Assignment
			UICanvas = GameObject.Find("$UICanvas");
			foreach(Transform _t in UICanvas.GetComponentsInChildren<Transform>(true))
			{
				switch(_t.name)
				{
					case "$FleeButton":
						fleeButton = _t.GetComponent<Button>();
						fleeButton.onClick.AddListener(delegate {
							if (turnOrder.Count != 0)
								if (turnOrder[0] == this)
								{
									ResetAbilityPick();

									buttonMenuScrollView.SetActive(false);

									fleeSlider.gameObject.SetActive(!fleeSlider.gameObject.activeSelf);
									fleeSlider.value = 0;
								}
						});
						break;
					case "$FleeSlider":
						fleeSlider = _t.GetComponent<Slider>();
						break;
					case "$CurrentHealthText":
						currentHealthText = _t.GetComponent<Text>();
						break;
					case "$MaxHealthText":
						maxHealthText = _t.GetComponent<Text>();
						break;
					case "$CurrentManaText":
						currentManaText = _t.GetComponent<Text>();
						break;
					case "$MaxManaText":
						maxManaText = _t.GetComponent<Text>();
						break;
					case "$AbilityButton":
						_t.GetComponent<Button>().onClick.AddListener(delegate {
							buttonMenuScrollView.SetActive(!buttonMenuScrollView.activeSelf);
							fleeSlider.gameObject.SetActive(false);
							ResetAbilityPick();
						});
						abilityButtonText = _t.Find("$Text").GetComponent<Text>();
						break;
					case "$HealthSlider":
						healthSlider = _t.GetComponent<Slider>();
						break;
					case "$HealthSliderSlow":
						healthSliderSlow = _t.GetComponent<Slider>();
						break;
					case "$ManaSlider":
						manaSlider = _t.GetComponent<Slider>();
						break;
					case "$ManaSliderSlow":
						manaSliderSlow = _t.GetComponent<Slider>();
						break;
					case "$XPSlider":
						xpSlider = _t.GetComponent<Slider>();
						break;
					case "$ButtonMenuScrollView":
						buttonMenuScrollView = _t.gameObject;
						buttonMenuScrollView.transform.SetParent(UICanvas.transform);
						break;
					case "$AbilityContent":
						buttonMenuContent = _t.gameObject;
						break;
					case "$BuffContent":
						buffContent = _t;
						break;
					case "$InventoryScreen":
						inventoryScreen = _t.gameObject;
						break;
					case "$GameOverHolder":
						gameOverHolder = _t.gameObject;
						foreach (Transform _childGameOver in _t.GetComponentsInChildren<Transform>(true))
						{
							if (_childGameOver.name == "$RestartButton")
							{
								_childGameOver.GetComponent<Button>().onClick.AddListener(delegate { var a = new GameObject(); a.AddComponent<ResetStaticVariablesManager>();});
							}
						}
						break;
				}
			}
			#endregion

			//foreach(KeyValuePair<string,AbilityType> i in abilityTypeDictionary) print(i.Key);

			//print(BulkUp(null).ToString());
			//print(myStats.buffs.Count);

			#region Stat Reset
			currentHealth = myStats.maxHealth;
			currentMana = myStats.maxMana;
			maxHealthText.text = myStats.maxHealth.ToString();
			maxManaText.text = myStats.maxMana.ToString();
			RefreshAbilityList();

			xpSlider.maxValue = myStats.level * 10;

			healthSlider.maxValue = myStats.maxHealth;
			manaSlider.maxValue = myStats.maxMana;
			healthSliderSlow.maxValue = healthSlider.maxValue * SLOW_SLIDER_RATIO_TO_NORMAL;
			healthSliderSlow.value = healthSliderSlow.maxValue;
			manaSliderSlow.maxValue = manaSlider.maxValue * SLOW_SLIDER_RATIO_TO_NORMAL;
			manaSliderSlow.value = manaSliderSlow.maxValue;


			healthSlider.value = currentHealth;
			currentHealthText.text = currentHealth.ToString();
			//AdjustHealth(0, Elementals.None, false);
			AdjustMana(0);
			AdjustPlayerXP(0);
			#endregion

			if (buffIconDictionary == null)
			{
				buffIconDictionary = new Dictionary<string, Sprite>();
				Sprite[] _buffIcons = Resources.LoadAll<Sprite>("Sprites/UI/StatusSpriteSheet");
				for (int i = 0; i < _buffIcons.Length; i++)
				{
					buffIconDictionary.Add(_buffIcons[i].name, _buffIcons[i]);
				}
			}
		}
		else
		{
			currentHealth = myStats.maxHealth;
			currentMana = myStats.maxMana;
		}

	}

	void RefreshAbilityList()
	{
		var _children = buttonMenuContent.GetComponentsInChildren<Transform>();

		for (int i = 1; i < _children.Length; i++) //exclude parent by starting at 1
		{
			Destroy(_children[i].gameObject);
		}

		for (int i = 0; i < myStats.abilities.Count; i++)
		{
			string _s = "";
			_s = myStats.abilities[i];

			buttonMenuContent.GetComponent<RectTransform>().sizeDelta = new Vector2(650,myStats.abilities.Count * 170 + 10);

			GameObject _go = Instantiate(entryPrefab,buttonMenuContent.transform);
			_go.transform.localPosition = new Vector3(325,-10 + -(i + 0.5f) * 170,0);
			_go.transform.Find("$Text").GetComponent<Text>().text = myStats.abilities[i];
			_go.GetComponent<Button>().onClick.AddListener(delegate {
				buttonMenuScrollView.SetActive(false);
				abilityButtonText.text = _s;
				selectedAbility = _s;
			});

		}

		CheckMana();
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

			//StartCoroutine(EndActedLastTick());
           // if (actedLastTick) print("update: " + Time.timeSinceLevelLoad);
        }

		if (turnOrder.Count != 0) //if there are combatants
		{
			if (turnOrder[0] == this) //if it is this actors turn
			{
				if (playerOwned)
				{
					if(fleeSlider.gameObject.activeSelf == true)
						fleeSlider.value = Mathf.PingPong(Time.timeSinceLevelLoad * fleeSlider.maxValue,fleeSlider.maxValue);

					if(Input.GetMouseButtonDown(0)) Click(); //wait for clicks
				}

				if (!startedTurn)
				{
					TickBuffs();

					//Invoke buffs
					startedTurn = true;
					if (!playerOwned)
					{
						StartCoroutine(TakeEnemyTurn());
					}
				}
			}
		}
		else
		{
			if(Input.GetMouseButtonDown(0)) Click(); //wait for clicks outside of the players turn
		}
	}
	

	public void TickBuffs()
	{
		targetCombatController = this;
		var _prevActiveAbility = selectedAbility;

		for(int i = 0; i < myStats.buffs.Count; i++)
		{
			var _buff = myStats.buffs[i];
			selectedAbility = _buff.function;
			StartCoroutine(InvokeActiveAbility(false,_buff.constant));
			_buff.turns--;
		}

		myStats.buffs.RemoveAll(x => x.turns <= 0);

		selectedAbility = _prevActiveAbility;//null;
		targetCombatController = null;
	}

	void CheckIfBuffIconsAreCorrect()
	{
		//var _tempBuffList = myStats.buffs.Count;
		List<string> _needsToBeAdded = new List<string>();
		List<string> _tempIconListNames = new List<string>();
		_tempIconListNames.AddRange(buffContent.GetComponentsInChildren<Text>(true).Select(x => x.text).ToList());

		print("til: " + _tempIconListNames.Count);
		foreach(var v in _tempIconListNames)
		{ print(v); }

		for (int i = 0; i < myStats.buffs.Count; i++)
		{
			//if (_tempIconListNames.Count >= i)
			//	print(_tempIconListNames[i]);

			string _current = myStats.buffs[i].buffIcon.name;
			if (_tempIconListNames.Contains(_current))
			{
				_tempIconListNames.Remove(_current);
				if (myStats.buffs[i].turns <= 0)
				{
					//destroy icon
				}
			}
			else if (myStats.buffs[i].turns > 0)
			{
				_needsToBeAdded.Add(_current);
				var _go = Instantiate(buffEntryPrefab, buffContent);
				var _children = _go.GetComponentsInChildren<Text>(true);
				var _info = _go.transform.GetChild(0).GetChild(0).gameObject;
				_go.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { _info.SetActive(!_info.activeSelf); });
				for (int j = 0; j < _children.Length; j++)
				{
					switch (_children[j].transform.name)
					{
						case "$BuffName":
							_children[j].text = myStats.buffs[i].name;
							break;
						case "$BuffTurns":
							_children[j].text = "Turns: " + myStats.buffs[i].turns;
							break;
						case "$BuffDescription":
							_children[j].text = myStats.buffs[i].function;
							break;
					}

				}
				
				//spawn icon
			}
		}
	}

	/// <summary>
	/// Toggles all buttons for which the player don't have enough mana for
	/// </summary>
	void CheckMana()
	{
		var _children = buttonMenuContent.GetComponentsInChildren<Button>();
		for (int i = 0; i < _children.Length; i++)
		{
			ToggleButtonOnMana(_children[i]);
		}

		/*
		ToggleButtonOnMana(abilityButton1);
		ToggleButtonOnMana(abilityButton2);
		ToggleButtonOnMana(abilityButton3);
		ToggleButtonOnMana(abilityButton4);
		*/
	}

	void ToggleButtonOnMana(Button _button)
	{
		string _buttonText = _button.GetComponentInChildren<Text>().text;//.ToLower(); //text of the button, to get the ability

		bool _hasManaCost = manaCostDictionary.ContainsKey(_buttonText);

		//if() //check dictionary if ti contains button text
		{
			bool _hasEnoughMana = (_hasManaCost)? (playerCombatController.currentMana >= -manaCostDictionary[_buttonText])  : true;

			//if (_hasManaCost)
			//	_hasEnoughMana = (playerCombatController.currentMana >= manaCostDictionary[_buttonText]);
			
			_button.GetComponentInChildren<Image>().color = (_hasEnoughMana)? activeColor: deactiveColor;
			_button.GetComponent<Button>().enabled = _hasEnoughMana;
			_button.GetComponent<Collider2D>().enabled = _hasEnoughMana;
		}
	}

	IEnumerator TakeEnemyTurn()
	{
		yield return new WaitForSeconds(1);
		var _recoveries = myStats.abilities.FindAll(x => abilityTypeDictionary[x] == AbilityType.recovery && (manaCostDictionary.TryGetValue(x, out int y)? y <= currentMana: true)); //find all recoveries. If it has cost, check if has more or equal mana. If no cost, act as if has mana.
		var _nonRecover = myStats.abilities.FindAll(x => abilityTypeDictionary[x] != AbilityType.recovery && (manaCostDictionary.TryGetValue(x, out int y) ? y <= currentMana : true));
		var _offensive = myStats.abilities.FindAll(x => abilityTypeDictionary[x] == AbilityType.offensive && (manaCostDictionary.TryGetValue(x, out int y) ? y <= currentMana : true));
		var _buffs = myStats.abilities.FindAll(x => abilityTypeDictionary[x] == AbilityType.buff && (manaCostDictionary.TryGetValue(x, out int y) ? y <= currentMana : true));

		AbilityType activeType = AbilityType.none;

		switch(myStats.aiType)
		{
			case StatBlock.AIType.None:
				selectedAbility = "";
				break;
			case StatBlock.AIType.Dumb:
				selectedAbility = myStats.abilities[Random.Range(0,myStats.abilities.Count)];
				activeType = AbilityType.offensive;
				break;
			case StatBlock.AIType.Smart:
				if(currentHealth <= myStats.maxHealth / 3 && _recoveries.Count > 0)
				{
					selectedAbility = _recoveries[Random.Range(0,_recoveries.Count)];
					activeType = AbilityType.recovery;
				}
				else if (myStats.buffs.Count == 0 && _buffs.Count > 0)
				{
					selectedAbility = _buffs[Random.Range(0,_offensive.Count)];
					activeType = AbilityType.buff;
				}
				else
				{
					selectedAbility = _offensive[Random.Range(0,myStats.abilities.Count)];
					activeType = AbilityType.offensive;
				}

				break;
			case StatBlock.AIType.Coward:

				if (currentHealth <= myStats.maxHealth / 2 && _recoveries.Count > 0)
				{
					selectedAbility = _recoveries[Random.Range(0,_recoveries.Count)];
					activeType = AbilityType.recovery;
				}
				else
				{
					selectedAbility = _nonRecover[Random.Range(0,_nonRecover.Count)];
					activeType = AbilityType.offensive;
				}
				break;
			case StatBlock.AIType.Sprinter:
				break;
		}

		transform.position += Vector3.up * (UnityEngine.Random.Range(0f,2f) - 1);
		//activeAbility = myStats.abilities[UnityEngine.Random.Range(0,myStats.abilities.Count)];

		if(activeType == AbilityType.offensive)
		{
			targetCombatController = playerCombatController;
			lastClick = playerCombatController.transform.position;
		}
		else
		{
			targetCombatController = this;
			lastClick = transform.position;
		}



		print(transform.name + "(+" + myStats.aiType + "+)" + " is doing " + selectedAbility);
		yield return StartCoroutine(InvokeActiveAbility());		
	}

	public void AdjustPlayerXP(int _amount)
	{
		myStats.xp += _amount;
		xpSlider.value = myStats.xp;
		if (myStats.xp >= xpSlider.maxValue)
		{
			var _lvText = EffectTools.SpawnText(transform.position + Vector3.left, transform, Color.yellow, "LEVEL UP!");
			StartCoroutine(EffectTools.MoveDirection(_lvText.transform, Vector3.up, 1, 2));
			StartCoroutine(EffectTools.BlinkText(_lvText, Color.green, 5));
			Destroy(_lvText.gameObject,5);
		}

		while (myStats.xp >= xpSlider.maxValue)
		{
			LevelUpScreen.traitPointsToSpend += 1;
			LevelUpScreen.abilityPointsToSpend += (myStats.level + 1) % 2;

			myStats.level++;
			myStats.xp -= (int)xpSlider.maxValue;
			print(myStats.xp);
			xpSlider.value = myStats.xp;
			xpSlider.maxValue = myStats.level * 10;
		}
	}

	public int AdjustHealth(int _amount, Elementals _elementals)
	{
		
		//print("adjust hp: " + transform.name + " amount: " + _amount);
		//if (!playerOwned) print(playerCombatController.activeAbility + " has Light: " + elementals.HasFlag(Elementals.Light));
		float _amountMultiplier = 1;
		if (myStats.weaknesses.HasFlag(_elementals)) //if weakness
		{
			_amountMultiplier *= (_amount > 0)? -1.5f: 1.5f; //any positive adjustment is turned negative with a multiplier
		}

		if (myStats.resistances.HasFlag(_elementals)) //if resist
		{
			 _amountMultiplier *= (_amount < 0)? 0.5f : 1; //any negative adjustment is halfed if resist
		}

		if (myStats.immunities.HasFlag(_elementals)) //if immune
		{
			_amountMultiplier *= (_amount < 0) ? 0 : 1; //any negative adjustment is set to 0 if immune
		}

		if (myStats.absorbs.HasFlag(_elementals))
		{
			if(_amountMultiplier == 0) _amountMultiplier = 1;
			else
				_amountMultiplier *= (_amount < 0) ? -1 : 1; //any negative adjustment is turned positive
		}

		//if (playerOwned)
			//print(transform.name + " adjusted: " + _amount + " from: " + currentHealth + " to: " + (currentHealth+_amount));
		//print("amount multiplier: " + _amountMultiplier);
		int _totalDamage = currentHealth;

		var _damageCalc = Mathf.CeilToInt(_amount * _amountMultiplier) * ((isCritted) ? 2 : 1);

		string _textToWrite = ((_damageCalc > 0)? "+":"") + _damageCalc.ToString();
		GameObject _spawnedText = EffectTools.SpawnText(transform.position + Vector3.up, transform, (_damageCalc > 0)? Color.green: Color.red , _textToWrite).transform.parent.gameObject;
		StartCoroutine(EffectTools.CurveMove(_spawnedText.transform,4));
		_spawnedText.transform.SetParent(null);
		_spawnedText.transform.position = transform.position + Vector3.up;
		Destroy(_spawnedText,5);

		currentHealth = Mathf.Clamp(currentHealth + _damageCalc, 0, myStats.maxHealth);
		_totalDamage -= currentHealth;


		if(playerOwned)
		{
			currentHealthText.text = currentHealth.ToString();

			if (healthMove != null)
			{
				StopCoroutine(healthMove);
			}

			if (_damageCalc < 0)
			{
				healthSlider.value = currentHealth;
				healthMove = StartCoroutine(EffectTools.ApproachSlider(healthSliderSlow, healthSlider, 3, SLOW_SLIDER_RATIO_TO_NORMAL));
			}
			else
			{
				healthSliderSlow.value = currentHealth * SLOW_SLIDER_RATIO_TO_NORMAL;
				healthMove = StartCoroutine(EffectTools.ApproachSlider(healthSlider, healthSliderSlow, 1, 1/SLOW_SLIDER_RATIO_TO_NORMAL));
			}
		}

		if (currentHealth <= 0)
		{
			if (!playerOwned)
			{
				playerCombatController.AdjustPlayerXP(myStats.level * 3);

				StartCoroutine(RemoveFromTurnOrder(1.0f,this));
			}
			else
			{
				
				foreach(CombatController _cc in turnOrder)
				{
					RemoveFromTurnOrder(0,_cc);//Destroy(_cc);
					Destroy(_cc.gameObject);
				}

				gameOverHolder.transform.GetChild(0).gameObject.SetActive(true);
			}

		}

		return _totalDamage;
	}


	IEnumerator RemoveFromTurnOrder(float _sec, CombatController _target)
	{
		turnOrder.Remove(_target);
		yield return new WaitForSeconds(_sec);
		if(turnOrder.Count <= 1)
			ForwardMover.DoneWithCombat();

		Destroy(_target.gameObject);
	}

	public void AdjustMana(string _manaCost = "")
	{
		if(manaCostDictionary.ContainsKey(_manaCost))
		{
			AdjustMana(manaCostDictionary[_manaCost]);
		}
		else
			AdjustMana(0);
	}

	public int AdjustMana(int _amount)
	{
		var _manaLost = currentMana;
		currentMana = Mathf.Clamp(currentMana + _amount,0,myStats.maxMana);
		_manaLost -= currentMana;

		if(playerOwned)
		{
			currentManaText.text = currentMana.ToString();

			if (_manaLost > 0)
			{
				manaSlider.value = currentMana;
				manaMove = StartCoroutine(EffectTools.ApproachSlider(manaSliderSlow, manaSlider, 1, SLOW_SLIDER_RATIO_TO_NORMAL));
			}
			else
			{
				manaSliderSlow.value = currentMana * SLOW_SLIDER_RATIO_TO_NORMAL;
				manaMove = StartCoroutine(EffectTools.ApproachSlider(manaSlider, manaSliderSlow, 0.3f, 1 / SLOW_SLIDER_RATIO_TO_NORMAL));
			}

		}

		return _manaLost;
	}

	public void Click()
	{
		hitPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, ForwardMover.ENEMY_SPAWN_DISTANCE)); //store where to click
		RaycastHit2D _hit = CheckIfHit(hitPosition); //get click info

		bool _hitSomething = (_hit.collider != null); //store if something was hit

		if(_hitSomething) //what was hit
		{
			//print("name: " + _hit.transform.name + " actedLastTick: " + actedLastTick + " selectedAb: " + selectedAbility);
			
			if(_hit.transform.CompareTag("AbilityButton"))
			{
				selectedAbility = _hit.transform.Find("$Text").GetComponent<Text>().text;
				return;
			}
			else if(_hit.transform.CompareTag("UI"))
			{

				//activeAbility = string.Empty;
				return;
			}
		}

		#region Flee Logic
		bool _fleeing = fleeSlider != null; //is the fleeslider there
		if(_fleeing) _fleeing = fleeSlider.gameObject.activeSelf; //is the player fleeing

		if (_fleeing) //if fleeing
		{
			if(Mathf.Abs((fleeSlider.maxValue / 2) - (fleeSlider.value)) <= fleeThreshold + myStats.dexterity) //if the pointer is withing the flee zone, succeed
			{
				var _tempTurnorder = new CombatController[turnOrder.Count];
				turnOrder.CopyTo(_tempTurnorder);
				foreach (CombatController _cc in _tempTurnorder) //for all entries in the temp turn order
				{
					if (_cc != this)
						StartCoroutine(RemoveFromTurnOrder(0,_cc)); //destroy and remove items from the turn order
				}

				ForwardMover.speedBoost = 3; //run after fleeing
			}
			else
			{
				EndTurn(); //if failed fleeing, end the turn as an action
				
			}
			StartCoroutine(DeactivateGameObject(fleeSlider.gameObject,0.2f));
		}
		#endregion
		else if (!string.IsNullOrEmpty(selectedAbility)) //if not fleeing, but has an ability selected
		{
			if(_hit.transform != null) //if something was hit
			{
				if(_hit.transform.CompareTag("CritArea")) //if it was a crit area
				{
					targetCombatController = _hit.transform.GetComponentInParent<CombatController>();
					targetCombatController.isCritted = true;
				}
				else if (_hit.transform.name == "$PlayerPortrait") //if it was the player itself
				{
					print(selectedAbility + " self!");
					targetCombatController = playerCombatController;
				}
				else //if just the normal enemy body was hit
					targetCombatController = _hit.transform.GetComponent<CombatController>();
			}
			else //if an ability is selected, but the player missed
			{
				targetCombatController = null;
			}

			

			StartCoroutine(InvokeActiveAbility()); //activate the ability with the information gathered

			if (targetCombatController != null) targetCombatController.isCritted = false;
			targetCombatController = null;
		}
		/*
		else if (_hit.transform.name == "$PlayerPortrait")
		{
			LevelUpScreen.levelUpScreenInstance.GetComponent<LevelUpScreen>().ToggleLevelUpScreen();
		}
		*/
	}

	/// <summary>
	/// Calls the active ability
	/// </summary>
	IEnumerator InvokeActiveAbility(bool _byUser = true, float? _value = null)
	{
		var _tempActiveAbility = selectedAbility;
		if (!_byUser)
		{
			lastClick = targetCombatController.transform.position;
		}
		else if (playerOwned)
			ResetAbilityPick();

        actedLastTick = true;
		//yield return new WaitForEndOfFrame();
		//actedLastTick = false;

        switch (_tempActiveAbility)
		{
			case TIME_WARP:
				yield return StartCoroutine(TimeWarp(this));
				break;
			case BULK_UP:
				yield return StartCoroutine(BulkUp(this));
				break;
			case MANA_DRAIN:
				yield return StartCoroutine(ManaDrain(targetCombatController,this));
				break;
			case DIVINE_LUCK:
				yield return StartCoroutine(DivineLuck(this));
				break;
			case REGENERATION:
				yield return StartCoroutine(Regeneration(targetCombatController, myStats.luck + 1));
				break;
			case PUNCH:
				yield return StartCoroutine(Punch(targetCombatController,this));
				break;
			case FIREBALL:
				yield return StartCoroutine(Fireball(hitPosition,this));
				break;
			case MASS_EXPLOSION:
				yield return StartCoroutine(MassExplosion(hitPosition,this));
				break;
			case FOCUS:
				yield return StartCoroutine(Focus(this));
				break;
			case KEEN_SIGHT:
				yield return StartCoroutine(DisplayCritAreas(this));
				break;
			case SPOT_WEAKNESS:
				yield return StartCoroutine(SpotWeakness(targetCombatController, this));
				break;
			case SPOOK:
				yield return StartCoroutine(Spook(targetCombatController,this));
				break;
			case HEAL:
				yield return StartCoroutine(Heal(targetCombatController,(_value == null)? myStats.luck + 2: (int)_value));
				break;
			case MASS_HEAL:
				yield return StartCoroutine(MassHeal(this));
				break;
			case SMITE_UNLIFE:
				yield return StartCoroutine(Smite(targetCombatController, StatBlock.Race.Undead,myStats.strength));
				break;
			case SIPHON_SOUL:
				yield return StartCoroutine(SiphonSoul(targetCombatController,this));
				break;
			case LIFE_TAP:
				yield return StartCoroutine(LifeTap(this));
				break;
			default:
				Debug.LogError("No move set for \"" + _tempActiveAbility + "\"");
				break;
		}

		//if (actedLastTick) print("end ability:" + Time.timeSinceLevelLoad);
		if (_byUser)
		{
			print(transform.name + " called check buff");
			playerCombatController.CheckIfBuffIconsAreCorrect();
		}

        if (_byUser)
		{
			AdjustMana(_tempActiveAbility);

			EndTurn();
		}
	}

	IEnumerator DeactivateGameObject(GameObject _go, float _time)
	{
		yield return new WaitForSeconds(_time);
		_go.SetActive(false);
	}

	void EndTurn()
	{
		if(playerOwned)
		{
			ResetAbilityPick();
			//CheckMana();
		}

		playerCombatController.CheckMana();

		if(turnOrder.Count > 1 && (myStats.buffs.Find(x => x.function == "extra turn") == null))
		{
			turnOrder.Remove(this);
			turnOrder.Add(this);
		}

		if (turnOrder.Count <= 1)
		{
			ForwardMover.DoneWithCombat();

		}

		startedTurn = false;
	}

	void ResetAbilityPick()
	{
		abilityButtonText.text = "Abilities";
		selectedAbility = string.Empty;
	}

	public void CloseAllCombatUI()
	{
		ResetAbilityPick();
		fleeSlider.gameObject.SetActive(false);
		buttonMenuScrollView.SetActive(false);
		inventoryScreen.SetActive(false);
	}
}
