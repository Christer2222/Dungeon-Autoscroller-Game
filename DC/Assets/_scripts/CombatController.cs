using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using AbilityInfo;

public class CombatController : AbilityScript
{
	//Keeps track of whos turn it is, and switching
	public static List<CombatController> turnOrder = new List<CombatController>();
	private bool playerOwned;
	public bool startedTurn;
	private static Text turnorderText;
	public static int turnCounter;
	private bool processingAbility;

	//Shows the player their status
	private static GameObject UICanvas;
	private Slider healthSlider, manaSlider, xpSlider;
	private Slider healthSliderSlow, manaSliderSlow;
	private Text currentHealthText, currentManaText, maxManaText, maxHealthText;
	private Coroutine healthMove, manaMove;
	private const float SLOW_SLIDER_RATIO_TO_NORMAL = 100;
	private Transform buffContent;
	private ScrollRect buffScrollRect;
	private Image buffScrollImage;
	private static GameObject buffEntryPrefab;
	private Text levelUpText;

	//game stats that change often
	public StatBlock myStats;
	public int currentHealth;
	public int currentMana;

	//Gameover variables
	private GameObject gameOverHolder;
	private EnemyMover myEnemyMover;

	//Variables for targeting, and using abilities
	public Ability selectedAbility;
	private CombatController targetCombatController;
    public static CombatController playerCombatController;
	//private Vector3 hitPosition;
	private bool isCritted;
	private Color activeColor = new Color(0,0,0.35f), deactiveColor = Color.gray;


	private GameObject inventoryScreen;

	//private Button fleeButton;
	private Slider fleeSlider;
	/*
	private int FleeThreshold
	{
		get
		{
			return 5 + myStats.dexterity;
		}
	}
	*/
    public bool actedLastTick;
	private bool invokingAbility;

	//variables for player ability toggeling
	private GameObject abilityMenuScrollView, buttonMenuContent;
	private static GameObject entryPrefab;
	private Text abilityButtonText;

	private List<Ability> debugAbilityList = new List<Ability>()
	{
		poision, airSlash, bubble, crystalLance,
		eruption, thunderbolt, hardenSkin, magicShield, meteorShower, freezingStrike,
		chaosThesis, debulk, divineFists, bulkUp, manaDrain, divineLuck, regeneration,
		restoreSoul, clense, syncSoul, curse, bless, punch, doubleKick, wildPunch, forcePunch,
		spotWeakness, smiteUnlife, siphonSoul, heal, lifeTap, massHeal,fireball, focus, 
		timeWarp, keenSight,

		tiltSwing, massExplosion,
	};

	public static void ClearAllValues()
	{
		turnOrder.Clear();
		//abilityButton1 = abilityButton2 = abilityButton3 = abilityButton4 = null;
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
		if (UICanvas == null) UICanvas = GameObject.Find("$UICanvas");
			//damageText = transform.Find("$DamageTextHolder").Find("$DamageText").GetComponent<Text>();


		//Set stats for enemies and the player + ui for player
		if (gameObject.name == "$PlayerPortrait")
		{
            myStats = new StatBlock(
                StatBlock.Race.Human,
				"Player",
                10, 15, //hp, mp
                1, 0, //lv, xp
                1, 1, 1, 1, //str, dex. int, luck
                (DebugController.debugAbilities) ? debugAbilityList : new List<Ability> { punch });

			//playerOwned = true;

			entryPrefab = Resources.Load<GameObject>("Prefabs/$Entry");
			buffEntryPrefab = Resources.Load<GameObject>("Prefabs/$BuffEntry");

#region UI Assignment
			foreach(Transform _t in UICanvas.GetComponentsInChildren<Transform>(true))
			{
				switch(_t.name)
				{
					case "$FleeButton":
						//fleeButton = _t.GetComponent<Button>();
						ForwardMover.fleeButton = _t.GetComponent<Button>();
						ForwardMover.fleeButton.onClick.AddListener(delegate {

							if (LevelUpScreen.levelUpScreen.activeSelf) return;

							if (turnOrder.Count != 0)
								if (turnOrder[0] == playerCombatController && !processingAbility)
								{
									ResetAbilityPick();

									abilityMenuScrollView.SetActive(false);

									fleeSlider.gameObject.SetActive(!fleeSlider.gameObject.activeSelf);
									fleeSlider.GetComponent<FleeLogic>().enabled = true;
									//fleeSlider.value = 0;
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
						ForwardMover.abilityButton = _t.GetComponent<Button>();
						ForwardMover.abilityButton.onClick.AddListener(delegate {
							if (CheckIfHasBuff("Busy") || LevelUpScreen.levelUpScreen.activeSelf) return;

							abilityMenuScrollView.SetActive(!abilityMenuScrollView.activeSelf);
							fleeSlider.gameObject.SetActive(false);
							ResetAbilityPick();
						});
						abilityButtonText = _t.Find("$Text").GetComponent<Text>();
						break;
					case "$ItemsButton":
						ForwardMover.itemsButton = _t.GetComponent<Button>();
						ForwardMover.itemsButton.onClick.AddListener(delegate {
							print("items");
						});
						break;
					case "$InspectButton":
						ForwardMover.inspectButton = _t.GetComponent<Button>();
						ForwardMover.inspectButton.onClick.AddListener(delegate {
							print("inspect");
						});
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
						abilityMenuScrollView = _t.gameObject;
						abilityMenuScrollView.transform.SetParent(UICanvas.transform);
						break;
					case "$AbilityContent":
						buttonMenuContent = _t.gameObject;
						break;
					case "$BuffContent":
						buffContent = _t;
						buffScrollRect = _t.parent.parent.GetComponent<ScrollRect>();
						buffScrollImage = buffScrollRect.transform.Find("$BuffScrollbarVertical").GetComponent<Image>();
						break;
					case "$TurnorderText":
						turnorderText = _t.GetComponent<Text>();
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

			CheckIfBuffIconsAreCorrect();

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
			manaSlider.value = currentMana;
			currentManaText.text = currentMana.ToString();
			AdjustPlayerXP(0);
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

		if (myStats.idleAnimation != null)
		{
			StartCoroutine(Animate());
		}

	}

	/// <summary>
	/// Update available buttons
	/// </summary>
	public void RefreshAbilityList()
	{
		var _children = buttonMenuContent.GetComponentsInChildren<Transform>();

		for (int i = 1; i < _children.Length; i++) //exclude parent by starting at 1
		{
			Destroy(_children[i].gameObject);
		}

		for (int i = 0; i < myStats.abilities.Count; i++) //for all of my abilities
		{
			string _s = "";
			_s = myStats.abilities[i].name;

			buttonMenuContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0,(myStats.abilities.Count - 1) * 160 + 10); //set size to fit all entries

			GameObject _go = Instantiate(entryPrefab,buttonMenuContent.transform); //spawn a new entry for each ability
			_go.transform.localPosition = new Vector3(325,-10 + -(i + 0.5f) * 170,0); //place it
			_go.transform.Find("$Text").GetComponent<Text>().text = myStats.abilities[i].name; //write what ability the button selects

			int _index = i;
			_go.GetComponent<Button>().onClick.AddListener(delegate {
				abilityMenuScrollView.SetActive(false);
				abilityButtonText.text = _s;
				selectedAbility = myStats.abilities[_index]; //set ability to this ability
			});

		}

		CheckMana();
	}

	IEnumerator EndActedLastTick()
	{
		yield return new WaitForEndOfFrame();
		actedLastTick = false;
	}

	IEnumerator Animate()
	{
		SpriteRenderer _rend = GetComponent<SpriteRenderer>();
		int _index = 0;
		while (true)
		{
			_index++;
			_index %= myStats.idleAnimation.Length;
			_rend.sprite = myStats.idleAnimation[_index];
			yield return new WaitForSeconds(0.2f);
		}
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
				if (playerOwned)
				{
					if(Input.GetMouseButtonDown(0)) Click(); //wait for clicks
				}

				if (!startedTurn)
				{
					startedTurn = true;
					turnCounter++;

					//UpdateTurnOrder turn order
					UpdateTurnOrderDisplay();

					if (turnOrder[0] == playerOwned || turnCounter == 1)
					{
						var _playerTurnText = EffectTools.SpawnText(Vector3.zero, UICanvas.transform, (turnCounter == 1)? new Color(0.8f,0.4f,0) :Color.yellow + Color.red, (turnCounter == 1)? "Combat!": "Your Turn!", 150);
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


					//Invoke buffs
					TickBuffs();

					if (!playerOwned)
					{
						myEnemyMover.shouldMove = false;
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

	public static void UpdateTurnOrderDisplay()
	{
		turnorderText.text = string.Empty;
		if (turnOrder.Count > 1)
			for (int i = 0; i < turnOrder.Count; i++)
			{
				turnorderText.text += turnOrder[i].myStats.name + " | ";
			}
	}
	
	/// <summary>
	/// Call each buff, and decrement their turn counter
	/// </summary>
	public void TickBuffs()
	{
		targetCombatController = this;
		var _prevActiveAbility = selectedAbility;

		for(int i = 0; i < myStats.buffList.Count; i++)
		{
			var _buff = myStats.buffList[i];
			for (int j = 0; j < _buff.functions.Count; j++)
			{
				selectedAbility = _buff.functions[j];
				StartCoroutine(InvokeActiveAbility(false,_buff.constant));
			}
			_buff.turns--;
		}

		myStats.buffList.RemoveAll(x => x.turns <= 0);

		selectedAbility = _prevActiveAbility;//null;
		targetCombatController = null;

		if (playerOwned)
			CheckIfBuffIconsAreCorrect();
	}

	bool CheckIfHasBuff(string _buffName)
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
		List<Text> _buffHolderTurnList = buffContent.GetComponentsInChildren<Text>(true).Where(x => x.transform.name == "$BuffTurns").ToList();
		List<string> _uncheckedBuffs = buffContent.GetComponentsInChildren<Text>(true).Where(x => x.transform.name == "$BuffName").Select(y => y.text).ToList();
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
				var _go = Instantiate(buffEntryPrefab, buffContent);
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
			buffScrollRect.vertical = true;
			buffScrollImage.color = Color.white * 0.3f;
			//EffectTools.BlinkImage(buffScrollImage,Color.white,1,0.5f);
		}
		else
		{
			buffScrollRect.vertical = false;
			buffScrollImage.color = Color.clear;
		}
	}

	IEnumerator TakeEnemyTurn()
	{
		yield return new WaitForSeconds(1);
		//var a = myStats.abilities.FindAll(x => x.);
		List<Ability> _recoveries = myStats.abilities.FindAll(x => x.abilityType == AbilityType.recovery && -x.manaCost <= currentMana);// (manaCostDictionary.TryGetValue(x, out int y)? y <= currentMana: true)); //find all recoveries. If it has cost, check if has more or equal mana. If no cost, act as if has mana.
		List<Ability> _nonRecover = myStats.abilities.FindAll(x => x.abilityType != AbilityType.recovery && -x.manaCost <= currentMana);// (manaCostDictionary.TryGetValue(x, out int y) ? y <= currentMana : true));
		List<Ability> _offensive = myStats.abilities.FindAll(x => x.abilityType == AbilityType.offensive && -x.manaCost <= currentMana);// (manaCostDictionary.TryGetValue(x, out int y) ? y <= currentMana : true));
		List<Ability> _buffs = myStats.abilities.FindAll(x => x.abilityType == AbilityType.buff && -x.manaCost <= currentMana);// (manaCostDictionary.TryGetValue(x, out int y) ? y <= currentMana : true));

		AbilityType activeType = AbilityType.none;

		switch(myStats.aiType)
		{
			case StatBlock.AIType.None:
				selectedAbility = null;
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
				else if (myStats.buffList.Count == 0 && _buffs.Count > 0)
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

		var _startScale = transform.localScale;


		//_playerTurnText.transform.parent.localPosition = Vector3.zero;

		var _abilityUsedText = EffectTools.SpawnText(Vector3.zero, UICanvas.transform, new Color(0.7f,0,0), myStats.name + " used " + selectedAbility.name, 90);
		_abilityUsedText.transform.parent.localPosition = Vector3.zero + Vector3.up * 400;
		Destroy(_abilityUsedText.transform.parent.gameObject, 6);

		float _turnDelay = 1f / ((float)turnOrder.Count / 5);
		StartCoroutine(
		EffectTools.ActivateInOrder(_abilityUsedText, new List<EffectTools.FunctionAndDelay>()
		{
			new EffectTools.FunctionAndDelay(EffectTools.MoveDirection(_abilityUsedText.transform,Vector3.right,100,5),_turnDelay),
		}));

		yield return StartCoroutine(
		EffectTools.ActivateInOrder(this, new List<EffectTools.FunctionAndDelay>()
		{
			new EffectTools.FunctionAndDelay(EffectTools.StretchFromTo(transform, _startScale, _startScale * 1.5f, 0.2f), _turnDelay + 0.5f),
			new EffectTools.FunctionAndDelay(EffectTools.StretchFromTo(transform, transform.localScale, _startScale, 0.2f), 0.2f),
		}));

		yield return new WaitForSeconds(0.3f);

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

		yield return StartCoroutine(InvokeActiveAbility());
		myEnemyMover.shouldMove = true;
	}

	public void AdjustPlayerXP(int _amount)
	{
		myStats.xp += _amount;
		xpSlider.value = myStats.xp;
		if (levelUpText == null && myStats.xp >= xpSlider.maxValue)
		{
			levelUpText = EffectTools.SpawnText(transform.position, transform, Color.yellow, "LEVEL UP!");
			levelUpText.StartCoroutine(EffectTools.MoveDirection(levelUpText.transform, Vector3.up, 1, 2));
			levelUpText.StartCoroutine(EffectTools.BlinkText(levelUpText, Color.yellow + Color.red, 5));
			Destroy(levelUpText.gameObject,5);
		}

		while (myStats.xp >= xpSlider.maxValue)
		{
			LevelUpScreen.traitPointsToSpend += 1;

			if ((myStats.level + 1) % 2 == 0)
			{
				LevelUpScreen.instance.AddNextChoicesToQue();
			}

			myStats.level++;
			myStats.xp -= (int)xpSlider.maxValue;
			xpSlider.value = myStats.xp;
			xpSlider.maxValue = myStats.level * 10;
		}
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
			_amount += ((isCritted) ? -myStats.strength : 0); //allow critting

		var _damageCalc = Mathf.RoundToInt(_amount * _amountMultiplier); //round up any damage/healing
		if (_damageCalc < 0 && _extraData.HasFlag(ExtraData.blockable)) _damageCalc = Mathf.Min(_damageCalc + ((_extraData.HasFlag(ExtraData.magic))? myStats.defense: myStats.magicDefense), 0); //if value is blockable and is under 0, reduce damage by defense or magicDefense depending on if the move makes contact

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
					new EffectTools.FunctionAndDelay(EffectTools.StretchFromTo(transform, transform.localScale, _orgScale, 0.1f),Random.Range(0.1f,0.2f))
				}));
			}
		}

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

				gameOverHolder.transform.GetChild(0).gameObject.SetActive(true);
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
			currentManaText.text = currentMana.ToString();

			if (manaMove != null)
				StopCoroutine(manaMove);

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
	}

	void ToggleButtonOnMana(Button _button)
	{
		string _buttonText = _button.GetComponentInChildren<Text>().text;//.ToLower(); //text of the button, to get the ability

		bool _hasEnoughMana = playerCombatController.currentMana >= -myStats.abilities.Find(x => x.name == _buttonText).manaCost; //playerCombatController.selectedAbility.manaCost;

		_button.GetComponentInChildren<Image>().color = (_hasEnoughMana)? activeColor: deactiveColor;
		_button.GetComponent<Button>().enabled = _hasEnoughMana;
		_button.GetComponent<Collider2D>().enabled = _hasEnoughMana;
	}

	public void Click()
	{
		RaycastHit2D _hit = CheckIfHit(hitPosition); //get click info
		
		bool _hitSomething = (_hit.collider != null); //store if something was hit

		if (_hitSomething) //what was hit
		{
			//print("name: " + _hit.transform.name + " actedLastTick: " + actedLastTick + " selectedAb: " + selectedAbility);
			
			if(_hit.transform.CompareTag("AbilityButton"))
			{
				return;
			}
			else if(_hit.transform.CompareTag("UI"))
			{
				return;
			}
		}

		if (selectedAbility != null && !fleeSlider.gameObject.activeSelf) //if not fleeing, but has an ability selected
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


			actedLastTick = true;
			StartCoroutine(InvokeActiveAbility()); //activate the ability with the information gathered

			if (targetCombatController != null) targetCombatController.isCritted = false;
			targetCombatController = null;
		}
	}

	/// <summary>
	/// Calls the active ability
	/// </summary>
	IEnumerator InvokeActiveAbility(bool _byUser = true, float? _value = null)
	{
		if (invokingAbility) yield break;
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
			if (turnOrder.Count == 0)
			{
				AddBuff(new Buff("Busy", "busy", 1, BuffIcons.TryGetBuffIcon("busy"), Buff.StackType.Add_Duplicate, 1), this);
				ForwardMover.speedBoost = 0;
				ForwardMover.shouldMove = false;
				CheckIfBuffIconsAreCorrect();
			}
		}

		//yield return new WaitForEndOfFrame();
		//actedLastTick = false;

		var _targetData = new TargetData(_tempActiveAbility, this, targetCombatController, 0, _tempActiveAbility.element, lastClick, StatBlock.Race.Human);
		if (_tempActiveAbility.name.ToLower().Contains("undead")) _targetData.targetRace = StatBlock.Race.Undead;
		print(targetCombatController);
		print(selectedAbility);
		//var _targetData = new TargetData(this, targetCombatController, 0, selectedAbility.element, lastClick, StatBlock.Race.Human);
		//if (selectedAbility.name.ToLower().Contains("undead")) _targetData.targetRace = StatBlock.Race.Undead;

		if (_byUser)
		{
			//AdjustMana(selectedAbility.manaCost);
			AdjustMana(_tempActiveAbility.manaCost);
			if (playerOwned)
			{
				playerCombatController.CheckMana();
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
			EndTurn();
		}
		invokingAbility = false;
	}

	IEnumerator RemoveFromTurnOrder(float _sec, CombatController _target)
	{
		turnOrder.Remove(_target);
		yield return new WaitForSeconds(_sec);
		if(turnOrder.Count <= 1)
			ForwardMover.DoneWithCombat();

		Destroy(_target.gameObject);
	}

	public void EndTurn()
	{
		processingAbility = false;

		if(turnOrder.Count > 1 && !CheckIfHasBuff(timeWarp.name))//"extra turn"))
		{
			turnOrder.Remove(this);
			turnOrder.Add(this);
		}

		if (turnOrder.Count <= 1)
		{
			UpdateTurnOrderDisplay();
			//turnorderText.text = string.Empty;
			ForwardMover.DoneWithCombat();
		}

		startedTurn = false;
	}

	void ResetAbilityPick()
	{
		abilityButtonText.text = "Abilities";
		selectedAbility = null;// string.Empty;
	}

	public void CloseAllCombatUI()
	{
		ResetAbilityPick();
		fleeSlider.gameObject.SetActive(false);
		abilityMenuScrollView.SetActive(false);
		inventoryScreen.SetActive(false);
	}
}
