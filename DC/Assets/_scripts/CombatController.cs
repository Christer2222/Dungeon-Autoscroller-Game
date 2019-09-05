using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class CombatController : AbilityScript
{
	//Keeps track of whos turn it is, and switching
	public static List<CombatController> turnOrder = new List<CombatController>();
	private bool playerOwned;
	private bool startedTurn;

	//Shows the player their status
	private GameObject UICanvas;
	private Slider healthSlider, manaSlider, xpSlider;
	private Text currentHealthText, currentManaText, maxManaText, maxHealthText;
	//private static Button abilityButton1, abilityButton2, abilityButton3, abilityButton4;

	//game stats that change often
	public StatBlock myStats;
	public int currentHealth;
	public int currentMana;


	private static StatBlock ghostBlock				= new StatBlock(StatBlock.Race.Undead	,2,2,1,0,1,3,1,1,new List<string> { "spook" },_weaknesses: Elementals.Light,_absorbs: Elementals.Unlife,_aiType: StatBlock.AIType.Dumb);
	private static StatBlock nosemanBlock			= new StatBlock(StatBlock.Race.Demon	,1,0,1,0,3,1,1,1,new List<string> { "punch" }, _aiType: StatBlock.AIType.Dumb);
	private static StatBlock lightElementalBlock	= new StatBlock(StatBlock.Race.Elemental,10,2,1,0,1,2,1,2,new List<string> { "punch", "heal" },_absorbs: Elementals.Light, _weaknesses: Elementals.Void,_aiType: StatBlock.AIType.Coward);
	private static StatBlock eyeballBlock			= new StatBlock(StatBlock.Race.Demon	,7,7,2,0,1,2,1,2,new List<string> { "punch", "mana drain" },_aiType: StatBlock.AIType.Dumb);
	private static StatBlock airElementalBlock		= new StatBlock(StatBlock.Race.Elemental,7,5,2,0,1,2,1,2,new List<string> { "punch" },_absorbs: Elementals.Air, _weaknesses:  Elementals.Earth, _aiType: StatBlock.AIType.Dumb);
	private static StatBlock earthElementalBlock	= new StatBlock(StatBlock.Race.Elemental,15,5,2,0,1,2,1,2,new List<string> { "punch" },_absorbs: Elementals.Earth, _weaknesses: Elementals.Air, _aiType: StatBlock.AIType.Dumb);
	private static StatBlock fireElementalBlock		= new StatBlock(StatBlock.Race.Elemental,5,5,2,0,1,2,1,2,new List<string> { "punch", "fireball" },_absorbs: Elementals.Fire, _weaknesses: Elementals.Water,_aiType: StatBlock.AIType.Dumb);
	private static StatBlock waterElementalBlock	= new StatBlock(StatBlock.Race.Elemental,12,5,2,0,1,2,1,2,new List<string> { "punch", "regeneration" },_absorbs: Elementals.Water, _weaknesses: Elementals.Fire, _aiType: StatBlock.AIType.Dumb);
	private static StatBlock harpyBlock				= new StatBlock(StatBlock.Race.Demon	,10,5,2,0,1,2,1,2,new List<string> { "punch", "bulk up" },_aiType: StatBlock.AIType.Dumb);
	private static StatBlock druidBlock				= new StatBlock(StatBlock.Race.Elf		,5,10,2,0,1,2,1,2,new List<string> { "punch", "heal", "regeneration" },_resistances: Elementals.Earth,_aiType: StatBlock.AIType.Coward);

	//Gameover variables
	private GameObject gameOverHolder;

	//Variables for targeting, and using abilities
	private string activeAbility;
	private CombatController targetCombatController;
	private static CombatController playerCombatController;
	private Vector3 hitPosition;
	private bool isCritted;
	private Color activeColor = new Color(0,0,0.35f), deactiveColor = Color.gray;

	private Button fleeButton;
	private Slider fleeSlider;
	private int fleeThreshold = 5;

	private static Camera mainCamera;

	//variables for player ability toggeling
	private GameObject buttonMenuScrollView, buttonMenuContent;
	private static GameObject entryPrefab;
	private Text abilityButtonText;

	public static void ClearAllValues()
	{
		turnOrder.Clear();
		//abilityButton1 = abilityButton2 = abilityButton3 = abilityButton4 = null;
		playerCombatController = null;
		mainCamera = null;
	}

	private void Start()
	{
		if(playerCombatController == null) playerCombatController = GameObject.Find("$Player").GetComponent<CombatController>();

		if(mainCamera == null) mainCamera = Camera.main;

		//Set stats for enemies and the player + ui for player
		if(gameObject.name == "$Player")
		{
			myStats = new StatBlock(
				StatBlock.Race.Human, 
				10,15, //hp, mp
				1,0, //lv, xp
				1,1,1,1, //str, dex. int, luck
				new List<string>() {"time warp", "bulk up", "mana drain", "divine luck", "regeneration", "spot weakness", "smite unlife", "siphon soul", "heal",
					"life tap", "mass heal","fireball","focus","mass explosion" , "punch", "keen sight", "spook"});

			playerOwned = true;

			entryPrefab = Resources.Load<GameObject>("Prefabs/$Entry");
			
			#region UI Assignment
			UICanvas = GameObject.Find("$UICanvas");
			foreach(Transform _t in UICanvas.GetComponentsInChildren<Transform>(true))
			{
				switch(_t.name)
				{/*
					case "$AbilityButton1":
						abilityButton1 = _t.GetComponent<Button>();

						break;
					case "$AbilityButton2":
						abilityButton2 = _t.GetComponent<Button>();

						break;
					case "$AbilityButton3":
						abilityButton3 = _t.GetComponent<Button>();

						break;
					case "$AbilityButton4":
						abilityButton4 = _t.GetComponent<Button>();
						break;
						*/
					case "$FleeButton":
						fleeButton = _t.GetComponent<Button>();
						fleeButton.onClick.AddListener(delegate {
							if (turnOrder.Count != 0)
								if (turnOrder[0] == this)
								{
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
						});
						abilityButtonText = _t.Find("$Text").GetComponent<Text>();
						break;
					case "$HealthSlider":
						healthSlider = _t.GetComponent<Slider>();
						break;
					case "$ManaSlider":
						manaSlider = _t.GetComponent<Slider>();
						break;
					case "$XPSlider":
						xpSlider = _t.GetComponent<Slider>();
						break;
					case "$ButtonMenuScrollView":
						buttonMenuScrollView = _t.gameObject;
						break;
					case "$Content":
						buttonMenuContent = _t.gameObject;
						break;
					case "$GameOverHolder":
						gameOverHolder = _t.gameObject;
						foreach (Transform _childGameOver in _t.GetComponentsInChildren<Transform>(true))
						{
							if (_childGameOver.name == "$RestartButton")
							{
								_childGameOver.GetComponent<Button>().onClick.AddListener(delegate { var a = new GameObject(); a.AddComponent<RestStaticVariables>(); UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);});
							}
						}
						break;
				}
			}
			#endregion

			//foreach(KeyValuePair<string,AbilityType> i in abilityTypeDictionary) print(i.Key);

			print(BulkUp(null).ToString());
			print(myStats.buffs.Count);

			#region Stat Reset
			currentHealth = myStats.maxHealth;
			currentMana = myStats.maxMana;
			maxHealthText.text = myStats.maxHealth.ToString();
			maxManaText.text = myStats.maxMana.ToString();
			RefreshAbilityList();

			healthSlider.maxValue = myStats.maxHealth;
			manaSlider.maxValue = myStats.maxMana;
			xpSlider.maxValue = myStats.level * 10;
			AdjustHealth(0, Elementals.None);
			AdjustMana(0);
			AdjustPlayerXP(0);
			#endregion
		}
		else
		{
			#region enemy stat set
			var _regName = Regex.Match(gameObject.name.ToLower(),".+(?=[ ])").Value;
			switch(_regName)
			{
				case "ghost":
					myStats = ghostBlock.Clone();
					break;
				case "noseman":
					myStats = nosemanBlock.Clone();
					break;
				case "light elemental":
					myStats = lightElementalBlock.Clone();
					break;
				case "eyeball":
					myStats = eyeballBlock.Clone();
					break;
				case "air elemental":
					myStats = airElementalBlock.Clone();
					break;
				case "fire elemental":
					myStats = fireElementalBlock.Clone();
					break;
				case "earth elemental":
					myStats = earthElementalBlock.Clone();
					break;
				case "water elemental":
					myStats = waterElementalBlock.Clone();
					break;
				case "harpy":
					myStats = harpyBlock.Clone();
					break;
				case "druid":
					myStats = druidBlock.Clone();
					break;
				default:
					Debug.LogError("No enemy with name: " + _regName);
					break;
			}


			currentHealth = myStats.maxHealth;
			currentMana = myStats.maxMana;
			#endregion
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
				activeAbility = _s;
			});

		}

		CheckMana();
	}

	void Update()
	{
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
		var _prevActiveAbility = activeAbility;

		print(transform.name + ": " + myStats.buffs.Count);

		for(int i = 0; i < myStats.buffs.Count; i++)
		{
			var _buff = myStats.buffs[i];
			activeAbility = _buff.function;
			StartCoroutine(InvokeActiveAbility(false,_buff.constant));
			_buff.turns--;
		}

		myStats.buffs.RemoveAll(x => x.turns <= 0);

		activeAbility = _prevActiveAbility;//null;
		targetCombatController = null;
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
		string _buttonText = _button.GetComponentInChildren<Text>().text.ToLower(); //text of the button, to get the ability

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
		var _recoveries = myStats.abilities.FindAll(x => abilityTypeDictionary[x] == AbilityType.recovery);
		var _nonRecover = myStats.abilities.FindAll(x => abilityTypeDictionary[x] != AbilityType.recovery);
		var _offensive = myStats.abilities.FindAll(x => abilityTypeDictionary[x] == AbilityType.offensive);
		var _buffs = myStats.abilities.FindAll(x => abilityTypeDictionary[x] == AbilityType.buff);

		AbilityType activeType = AbilityType.none;

		switch(myStats.aiType)
		{
			case StatBlock.AIType.None:
				activeAbility = "";
				break;
			case StatBlock.AIType.Dumb:
				activeAbility = myStats.abilities[Random.Range(0,myStats.abilities.Count)];
				activeType = AbilityType.offensive;
				break;
			case StatBlock.AIType.Smart:
				if(currentHealth <= myStats.maxHealth / 3)
				{
					activeAbility = _recoveries[Random.Range(0,_recoveries.Count)];
					activeType = AbilityType.recovery;
				}
				else if (myStats.buffs.Count == 0)
				{
					activeAbility = _buffs[Random.Range(0,_offensive.Count)];
					activeType = AbilityType.buff;
				}
				else
				{
					activeAbility = _offensive[Random.Range(0,myStats.abilities.Count)];
					activeType = AbilityType.offensive;
				}

				break;
			case StatBlock.AIType.Coward:

				if (currentHealth <= myStats.maxHealth / 2)
				{
					activeAbility = _recoveries[Random.Range(0,_recoveries.Count)];
					activeType = AbilityType.recovery;
				}
				else
				{
					activeAbility = _nonRecover[Random.Range(0,_nonRecover.Count)];
					activeType = AbilityType.offensive;
				}
				break;
			case StatBlock.AIType.Sprinter:
				break;
		}

		transform.position += Vector3.up * (UnityEngine.Random.Range(0f,2f) - 1);
		//activeAbility = myStats.abilities[UnityEngine.Random.Range(0,myStats.abilities.Count)];

		if(activeType == AbilityType.offensive)
			targetCombatController = playerCombatController;
		else
			targetCombatController = this;



		print(transform.name + "(+" + myStats.aiType + "+)" + " is doing " + activeAbility);
		yield return StartCoroutine(InvokeActiveAbility());		
	}

	public void AdjustPlayerXP(int _amount)
	{
		myStats.xp += _amount;
		xpSlider.value = myStats.xp;
		while (myStats.xp >= xpSlider.maxValue)
		{
			LevelUpScreen.traitPointsToSpend += 2;
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

		currentHealth = Mathf.Clamp(currentHealth + Mathf.CeilToInt(_amount * _amountMultiplier) * ((isCritted)? 2 : 1), 0, myStats.maxHealth);
		_totalDamage -= currentHealth;

		if(playerOwned)
		{
			healthSlider.value = currentHealth;
			currentHealthText.text = currentHealth.ToString();
		}

		if(currentHealth <= 0)
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
					Destroy(_cc);
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
		var _prevMan = currentMana;
		currentMana = Mathf.Clamp(currentMana + _amount,0,myStats.maxMana);
		_prevMan -= currentMana;

		if(playerOwned)
		{
			manaSlider.value = currentMana;
			currentManaText.text = currentMana.ToString();
		}

		return _prevMan;
	}

	public void Click()
	{
		hitPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, ForwardMover.ENEMY_SPAWN_DISTANCE));
		RaycastHit2D _hit = CheckIfHit(hitPosition);

		bool _hitSomething = (_hit.collider != null);

		if(_hitSomething)
		{

			if(_hit.transform.CompareTag("AbilityButton"))
			{
				activeAbility = _hit.transform.Find("$Text").GetComponent<Text>().text;
				return;
			}
			else if(_hit.transform.CompareTag("UI"))
			{
				//activeAbility = string.Empty;
				return;
			}
		}

		bool _fleeing = fleeSlider != null;
		if(_fleeing) _fleeing = fleeSlider.gameObject.activeSelf;

		if (_fleeing)
		{
			if(Mathf.Abs((fleeSlider.maxValue / 2) - (fleeSlider.value)) <= fleeThreshold + myStats.dexterity)
			{
				var _dt = new CombatController[turnOrder.Count];
				turnOrder.CopyTo(_dt);
				foreach (CombatController _cc in _dt)
				{
					if (_cc != this)
						StartCoroutine(RemoveFromTurnOrder(0,_cc));
				}

				//turnOrder.ForEach(x => x);// StartCoroutine(RemoveFromTurnOrder(1,x)));
				ForwardMover.speedBoost = 3;
			}
			else
			{
				EndTurn();
			}
			StartCoroutine(DeactivateGameObject(fleeSlider.gameObject,0.2f));
		}
		else if(!string.IsNullOrEmpty(activeAbility))
		{
			if(_hit.transform != null)
			{
				if(_hit.transform.CompareTag("CritArea"))
				{
					targetCombatController = _hit.transform.GetComponentInParent<CombatController>();
					targetCombatController.isCritted = true;
				}
				else if (_hit.transform.name == "$PlayerPortrait")
				{
					print(activeAbility + " self!");
					targetCombatController = playerCombatController;
				}
				else
					targetCombatController = _hit.transform.GetComponent<CombatController>();
			}
			else
			{
				targetCombatController = null;
			}

			StartCoroutine(InvokeActiveAbility());

			if (targetCombatController != null) targetCombatController.isCritted = false;
			targetCombatController = null;
		}

	}

	/// <summary>
	/// Calls the active ability
	/// </summary>
	IEnumerator InvokeActiveAbility(bool _byUser = true, float? _value = null)
	{
		print("invoke: " + activeAbility);
		if(!_byUser)
			lastClick = targetCombatController.transform.position;

		switch(activeAbility)
		{
			case "time warp":
				yield return StartCoroutine(TimeWarp(this));
				break;
			case "bulk up":
				yield return StartCoroutine(BulkUp(this));
				break;
			case "mana drain":
				yield return StartCoroutine(ManaDrain(targetCombatController,this));
				break;
			case "divine luck":
				yield return StartCoroutine(DivineLuck(this));
				break;
			case "regeneration":
				yield return StartCoroutine(Regeneration(targetCombatController, myStats.luck + 1));
				break;
			case "punch":
				yield return StartCoroutine(Punch(targetCombatController,this));
				break;
			case "fireball":
				yield return StartCoroutine(Fireball(hitPosition,this));
				break;
			case "mass explosion":
				yield return StartCoroutine(MassExplosion(hitPosition,this));
				break;
			case "focus":
				yield return StartCoroutine(Focus(this));
				break;
			case "keen sight":
				yield return StartCoroutine(DisplayCritAreas(this));
				break;
			case "spot weakness":
				yield return StartCoroutine(SpotWeakness(targetCombatController, this));
				break;
			case "spook":
				yield return StartCoroutine(Spook(targetCombatController,this));
				break;
			case "heal":
				yield return StartCoroutine(Heal(targetCombatController,(_value == null)? myStats.luck + 2: (int)_value));
				break;
			case "mass heal":
				yield return StartCoroutine(MassHeal(this));
				break;
			case "smite unlife":
				yield return StartCoroutine(Smite(targetCombatController, StatBlock.Race.Undead,myStats.strength));
				break;
			case "siphon soul":
				yield return StartCoroutine(SiphonSoul(targetCombatController,this));
				break;
			case "life tap":
				yield return StartCoroutine(LifeTap(this));
				break;
			default:
				Debug.LogError("No move set for \"" + activeAbility + "\"");
				break;
		}

		if (_byUser)
		{
			AdjustMana(activeAbility);

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
			abilityButtonText.text = "Abilities";
			CheckMana();
			activeAbility = string.Empty;
		}

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
}
