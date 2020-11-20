using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using AbilityInfo;
using System;
using UnityEditor;

public class CombatController : AbilityScript, IAbilityInterractible
{
	//Keeps track of whos turn it is, and switching
	public static List<CombatController> turnOrder = new List<CombatController>();
	private bool playerOwned;
	[HideInInspector]
	public bool startedTurn;
	public static int turnCounter;
	private bool processingAbility;

	//Shows the player their status
	private Coroutine healthMove, manaMove, xpMove;
	private const float SLOW_SLIDER_RATIO_TO_NORMAL = 100;
	private static GameObject buffEntryPrefab;
	private Text levelUpText;
	private int xpPoolToaddAfterCombat;

	//game stats that change often
	public StatBlock MyStats { get; set; }
	public MonoBehaviour MyMono { get; set; }

	//Enemy Stuff
	private EnemyMover myEnemyMover;
	private ToolTip myToolTip; 


	//Variables for targeting, and using abilities
	public Ability selectedAbility;
	private IAbilityInterractible targetCombatController;
	public static CombatController playerCombatController;
	private bool isCritted;
	//private Color abilityActiveColor = new Color(0, 0, 0.35f), abilityInactive = Color.gray;
	private const float MAX_ABILITIES_ON_SCREEN = 6.5f;

	private readonly List<AbilitInField> listedAbilityObjects = new List<AbilitInField>();

	[HideInInspector]
	public bool actedLastTick;
	private bool invokingAbility;

	//variables for player ability toggeling
	private static GameObject abilityEntryPrefab;

	private readonly List<BuffUI> buffUIs = new List<BuffUI>();

	class BuffUI
	{
		public ToolTip tip;
		public Buff buff;
	}

	public static void ClearAllValues()
	{
		turnOrder.Clear();
		playerCombatController = null;
		mainCamera = null;
		turnCounter = 0;
		turnCounter = 0;
	}

	private void Start()
	{
		MyMono = this;
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
			
			MyStats = new StatBlock(
				StatBlock.Race.Human,
				"Player",
#if UNITY_EDITOR
				10, 1, //hp, mp
#else
				10, 1, //hp, mp
#endif
				1, 0, //lv, xp
				1, 1, 1, 1, //str, dex. int, luck
				//new List<Ability> { AbilityClass.punch});
				new List<Ability> { AbilityCollection.punch },
				_drops: DropTable.none
				);

			abilityEntryPrefab = Resources.Load<GameObject>("Prefabs/$AbilityEntry");
			buffEntryPrefab = Resources.Load<GameObject>("Prefabs/$BuffEntry");

#region UI Assignment


			UIController.AbilityButton.onClick.AddListener(delegate {
				//if (/*playerCombatController.CheckIfHasBuff("Busy") ||*/ UIController.LevelUpScreen.activeSelf) return;
				if ((EncounterController.instance.currentGameState & EncounterController.GameState.ConfirmingDrops) != 0)
					UIController.SetUIMode(UIController.UIMode.None);

				//UIController.AbilityMenuScrollView.SetActive(!UIController.AbilityMenuScrollView.activeSelf);
				//UIController.FleeSlider.gameObject.SetActive(false);
				playerCombatController.ResetAbilityPick();
				PlayerInventory.instance.ResetItemButton();
			});

			UIController.FleeButton.onClick.AddListener(delegate {

				//if (LevelUpScreen.levelUpScreen.activeSelf) return;
				ResetAbilityPick();
				PlayerInventory.instance.ResetItemButton();

				if (turnOrder.Count != 0)
					if (turnOrder[0] == playerCombatController && !processingAbility)
					{
						UIController.FleeSlider.GetComponent<FleeLogic>().enabled = true;
					}
					else
						UIController.SetUIMode(UIController.UIMode.None);
				//UIController.FleeSlider.gameObject.SetActive(false);
				else
					UIController.SetUIMode(UIController.UIMode.None);

			});

			UIController.InventoryButton.onClick.AddListener(delegate { 
				ResetAbilityPick(); 
				PlayerInventory.instance.ResetItemButton();
			});

			

#endregion

			UpdateBuffIcons();

#region Stat Reset
			//get current stats
			//currentHealth = myStats.maxHealth;
			//currentMana = myStats.maxMana;

			RefreshAbilityList();

			UIController.XPSlider.maxValue = MyStats.level * 10;

			//set the normal bars to their max value
			UIController.HealthSliderPair.minObject.maxValue = MyStats.maxHealth;
			UIController.ManaSliderPair.minObject.maxValue = MyStats.maxMana;

			//show values are at current status
			UIController.HealthSliderPair.minObject.value = MyStats.currentHealth;
			UIController.ManaSliderPair.minObject.value = MyStats.currentMana;

			//update texts to min/max status
			UIController.HealthTextPair.minObject.text = MyStats.currentHealth.ToString();
			UIController.HealthTextPair.maxObject.text = MyStats.maxHealth.ToString();
			UIController.ManaTextPair.minObject.text = MyStats.currentMana.ToString();
			UIController.ManaTextPair.maxObject.text = MyStats.maxMana.ToString();

			//update slow sliders
			UIController.HealthSliderPair.maxObject.maxValue = UIController.HealthSliderPair.minObject.maxValue * SLOW_SLIDER_RATIO_TO_NORMAL;
			UIController.HealthSliderPair.maxObject.value = UIController.HealthSliderPair.maxObject.maxValue;
			UIController.ManaSliderPair.maxObject.maxValue = UIController.ManaSliderPair.minObject.maxValue * SLOW_SLIDER_RATIO_TO_NORMAL;
			UIController.ManaSliderPair.maxObject.value = UIController.ManaSliderPair.maxObject.maxValue;


			UIController.XPSlider.value = MyStats.xp;
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
			myToolTip = GetComponentInChildren<ToolTip>();
			//TODO: health/mana would need to be loaded in from save

			//currentHealth = myStats.maxHealth;
			//currentMana = myStats.maxMana;
		}

		if (MyStats.idleAnimation != null && !playerOwned)
		{
			gameObject.AddComponent<AnimationHandler>();
		}
	}

	private class AbilitInField
	{
		public Ability ability;
		public GameObject gameObject;
	}

	/// <summary>
	/// Turns on or off ability buttons, as well as spawns new ones.
	/// </summary>
	public void RefreshAbilityList()
	{
		//Spawn abilities if they aren't already
		for (int i = 0; i < MyStats.abilities.Count; i++) //go through all abilities the player has
		{
			var _currentAbility = MyStats.abilities[i]; //shortcut

			var _foundAbilityObject = listedAbilityObjects.Find(x => x.ability == _currentAbility); //look for an instantiated gameobject with the checked ability
			if (_foundAbilityObject == null) //if none was found
			{
				GameObject _go = Instantiate(abilityEntryPrefab, UIController.AbilityMenuContent.transform); //spawn a new entry of it
				var _newAbilityObject = new AbilitInField { ability = MyStats.abilities[i], gameObject = _go };
				listedAbilityObjects.Add(_newAbilityObject); //add it to the list of abilities in the scene

				_go.transform.Find("$Text").GetComponent<Text>().text = _currentAbility.name; //name the button the name of the ability
				var rectTrans = _go.transform as RectTransform;
				rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 600); //set it to the right size

				int _index = i; //store the current index
				_go.GetComponent<Button>().onClick.AddListener(delegate {
					UIController.SetUIMode(UIController.UIMode.None); //when an ability is selected, hide all UI
					//UIController.AbilityButtonText.text = _currentAbility.name; //set the name of the button to the ability
					selectedAbility = MyStats.abilities[_index]; //set ability to this ability
					UIController.CurrentlySelectedImage.gameObject.SetActive(true);

					UIController.CurrentlySelectedText.text = selectedAbility.name;

					var _toolTip = _go.GetComponentInChildren<ToolTip>();
					_toolTip.OnPointerExit(null); //hide the current tooltip if it was hovering this
				});

				var _images = _go.GetComponentsInChildren<Image>(true).Where(x => x.name.StartsWith("$PortraitType")).ToArray();
				/*
				var _icons = new Image[_images.Length];

				for (int j = 0; j < _images.Length; j++)
				{
					_icons[j] = _images[j];
				}
				*/
				LevelUpScreen.UpdateIconsOnAbilitySelectionButton(MyStats.abilities[i], _images);

				_go.GetComponentInChildren<ToolTip>().SetToolTipText(MyStats.abilities[i].description); //give the spawned game object the correct tooltip
			}
		}

		//set active if they are in known abilities
		for (int i = 0; i < listedAbilityObjects.Count; i++)
		{
			var _currentCheck = listedAbilityObjects[i];
			//bool _inKnown = MyStats.abilities.Find(x => x == _currentCheck.ability) != null;
			//_currentCheck.gameObject.SetActive(_inKnown); //if the ability isn't null, leave it active. If it is null, deactivate it.

			var _listedObject = LevelUpScreen.instance.spawnedAbilities.Find(x => x.ability == _currentCheck.ability);
			if (_listedObject != null)
				_currentCheck.gameObject.SetActive(_listedObject.enabled); //if the ability is active in the levelup screen

			/*
			if (MyStats.abilities.Find(x => x == _currentCheck.ability) == null)
			{
				_currentCheck.gameObject.SetActive(false);
			}
			*/
		}

		//UIController.AbilityMenuContent.sizeDelta = new Vector2(0, (listedAbilityObjects.Count) * 110 + 10); //set size of content to fit all entries
		int _activeCount = (listedAbilityObjects.FindAll(x => x.gameObject.activeSelf).Count);
		print(_activeCount);
		UIController.AbilityMenuContent.sizeDelta = new Vector2(0, (_activeCount) * 110 + 10); //set size of content to fit all entries
		
		var oMax = UIController.AbilityMenuScrollView.offsetMax; //shortcut for height
		//oMax.y = UIController.AbilityMenuScrollView.offsetMin.y + (Mathf.Min(listedAbilityObjects.Count, MAX_ABILITIES_ON_SCREEN)) * 110 + 10; //set height to bottom + maxShowCount * height + offset
		oMax.y = UIController.AbilityMenuScrollView.offsetMin.y + (Mathf.Min(_activeCount, MAX_ABILITIES_ON_SCREEN)) * 110 + 10; //set height to bottom + maxShowCount * height + offset
		UIController.AbilityMenuScrollView.offsetMax = oMax; //apply

		UpdateAbilitiesToManaAvailability(); //set colors
	}

	/*
	/// <summary>
	/// Update available buttons
	/// </summary>
	public void RefreshAbilityListOld()
	{
		var _children = UIController.AbilityMenuContent.GetComponentsInChildren<Transform>(); //get all children already here

		ClearAbilityList();

		listedAbilityObjects.Clear();

		int _abilityCount = MyStats.abilities.Count; //for each ability the player has
		for (int i = 0; i < _abilityCount; i++) //go through them
		{
			string _s = "";
			_s = MyStats.abilities[i].name;


			GameObject _go = Instantiate(abilityEntryPrefab, UIController.AbilityMenuContent.transform); //spawn a new entry for each ability
			//listedAbilityObjects.Add(_go);

			//_go.transform.localPosition = Vector3.zero; //new Vector3(350,-10 + -(i + 0.5f) * 170,0); //place it
			_go.transform.Find("$Text").GetComponent<Text>().text = _s; //write what ability the button selects
			var rectTrans = _go.transform as RectTransform;
			rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 600);

			int _index = i; //store the current index
			_go.GetComponent<Button>().onClick.AddListener(delegate {
				UIController.SetUIMode(UIController.UIMode.None);
				//UIController.AbilityMenuScrollView.gameObject.SetActive(false); //deactivate the menu
				UIController.AbilityButtonText.text = _s; //set the name of the button to the ability
				selectedAbility = MyStats.abilities[_index]; //set ability to this ability
				_go.GetComponent<ToolTip>().OnPointerExit(null);
			});


			_go.GetComponent<ToolTip>().SetToolTipText(MyStats.abilities[i].description);
		}

		//UIController.AbilityMenuContent.sizeDelta = new Vector2(0,(count - 1) * 113 + 10); //set size to fit all entries
		UIController.AbilityMenuContent.sizeDelta = new Vector2(0, (_abilityCount) * 110 + 10); //set size to fit all entries

		var oMax = UIController.AbilityMenuScrollView.offsetMax; //get the height
		oMax.y = UIController.AbilityMenuScrollView.offsetMin.y + (Mathf.Min(_abilityCount, MAX_ABILITIES_ON_SCREEN)) * 110 + 10; //set height to bottom + maxShowCount * height + offset
		UIController.AbilityMenuScrollView.offsetMax = oMax; //apply

		UpdateAbilitiesToManaAvailability(); //set colors
	}
	*/

	void ClearAbilityList()
	{
		for (int i = 0; i < listedAbilityObjects.Count; i++)
		{
			Destroy(listedAbilityObjects[i].gameObject);
		}
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
			if (turnOrder[0] == this && !UIController.IsFullscreenUI() && !startedTurn) //if it is this actors turn, and fullscreen is not active
			{
				StartCoroutine(StartOfTurnActions());
			}
		}
		
		if (playerOwned)
		{
			if(Input.GetMouseButtonDown(0)) StartCoroutine( Click()); //wait for clicks outside of the players turn
		}
	}

	IEnumerator StartOfTurnActions()
	{
		if (!startedTurn)
		{
			isCritted = false;

			startedTurn = true;
			turnCounter++;

			//Invoke buffs
			yield return TickBuffs();

			//UpdateTurnOrder turn order
			UpdateTurnOrderDisplay();

			if (turnOrder[0] == playerOwned || turnCounter == 1)
			{
				var _playerTurnText = EffectTools.SpawnText(Vector3.zero, UIController.UICanvas, (turnCounter == 1) ? ColorScheme.combatStartTextColor: ColorScheme.playerTurnTextColor, (turnCounter == 1) ? "Combat!" : "Your Turn!", 150);
				_playerTurnText.transform.parent.localPosition = Vector3.zero;
				_playerTurnText.StartCoroutine(EffectTools.ActivateInOrder(_playerTurnText,
					new List<EffectTools.FunctionGroup>()
					{
									new EffectTools.FunctionGroup(EffectTools.StretchFromTo(_playerTurnText.transform, new Vector3(2, 0, 0), Vector3.one, 1f),   0),
									new EffectTools.FunctionGroup(new List<IEnumerator>()
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
				if (MyStats.currentHealth > 0)
				{
					yield return StartCoroutine(TakeEnemyTurn());
					myEnemyMover.shouldMove = true; //if can still move allow doing so
				}
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
				UIController.TurnOrderText.text += turnOrder[i].MyStats.name + " | ";
			}
	}
	
	/// <summary>
	/// Call each buff, and decrement their turn counter
	/// </summary>
	public IEnumerator TickBuffs()
	{
		targetCombatController = this;
		//var _prevActiveAbility = selectedAbility;

		for (int i = 0; i < MyStats.buffList.Count; i++)
		{
			var _buff = MyStats.buffList[i];

			for (int j = 0; j < _buff.functions.Count; j++)
			{
				var td = new TargetData(MyStats.buffList[i].functions[j], this, this, (int)MyStats.buffList[i].constant, _centerPos: transform.position, _useOwnStats: false);

				//selectedAbility = _buff.functions[j];
				//yield return StartCoroutine(InvokeActiveAbility(false,_buff.constant));
				yield return StartCoroutine(SimpleInvokeAbility(td, false, false, 0, false));
			}

			if (_buff.turns < 1000)
				_buff.turns--;
		}

		MyStats.buffList.RemoveAll(x => x.turns <= 0);

		//selectedAbility = _prevActiveAbility;//null;
		targetCombatController = null;

		if (playerOwned)
			UpdateBuffIcons();
	}

	/// <summary>
	/// Checks wether this combatcontroller has a (ToLowered) buff with same name as (ToLowered) input.
	/// </summary>
	public bool CheckIfHasBuff(string _buffName)
	{
		return (MyStats.buffList.Exists(x => x.name.ToLower() == _buffName.ToLower()));
	}

	/// <summary>
	/// REmoves all (ToLowered) buffs with same name as (ToLowered) input from this combatcontroller.
	/// </summary>
	public void RemoveAllBufsWithName(string _buffName)
	{
		MyStats.buffList.RemoveAll(x => x.name.ToLower() == _buffName.ToLower());// x.name.Contains(_buffName));
		UpdateBuffIcons();
	}


	/// <summary>
	/// Refreshes the buff sidebar.
	/// </summary>
	public void UpdateBuffIcons()
	{
		if (!playerOwned)
			return;

		for (int i = 0; i < MyStats.buffList.Count; i++) //find all buffs active, and make ui if none exist
		{
			var _current = MyStats.buffList[i]; //shortcut for i
			var _inList = buffUIs.Where(x => x.buff == _current); //find a buff that matches the one in the statblock
			//var _first = (_inList.ToList().Count != 0) ? _inList.First() : null;


			if ((_inList.ToList().Count == 0)) //if none were found
			{
				var _go = Instantiate(buffEntryPrefab, UIController.BuffContent); //make a new one and set the parent to the sidebar
				var _parent = _go.transform.GetChild(0); //get the icon holder
				_parent.GetComponent<Image>().sprite = MyStats.buffList[i].buffIcon; //set the icon to the propper one

				buffUIs.Add(new BuffUI {tip = _go.GetComponentInChildren<ToolTip>(), buff = _current }); //add it to the uis
			}
		}

		buffUIs.RemoveAll(x => x.tip == null); //remove all ui with no gameobject

		for (int i = 0; i < buffUIs.Count; i++) //find all ui that is used up, and destroy it
		{
			var _current = buffUIs[i]; //shortcut for i

			if (_current.buff.turns <= 0 || !MyStats.buffList.Contains(_current.buff)) //if it has no time left
			{
				_current.tip.OnPointerExit(null);
				Destroy(_current.tip.transform.parent.gameObject); //destroy game object
			}

		}

		//buffUIs.RemoveAll(x => x.tip == null); //remove all ui with no gameobject

		for (int i = 0; i < buffUIs.Count; i++) //update the text for all ui
		{
			var _current = buffUIs[i]; //shortcut for i

			string _actionString = string.Empty; //description for all a buff does
			for (int j = 0; j < _current.buff.functions.Count; j++) //for all functions add them to the description
			{
				_actionString += _current.buff.functions[j] + " " + _current.buff.constant;
				if (j != _current.buff.functions.Count) _actionString += ", ";
			}

			if (_current.buff.functions.Count != 0 && _current.buff.traits.Count != 0) _actionString += " and ";
			for (int j = 0; j < _current.buff.traits.Count; j++) //for all traits add them to the description
			{
				_actionString += _current.buff.traits[j].ToString().Replace("_", " ") + _current.buff.constant;
				if (j != _current.buff.traits.Count) _actionString += ", ";
			}

			//finally update text
			_current.tip.SetToolTipText(
				$"{_current.buff.name}" +
				$"\n" +
				$"\nActivates {_actionString}" +
				$"\n" +
				$"Lasts for: {_current.buff.turns} turns."
				);
		}

		/*
		ToolTip[] _buffToolTips = UIController.BuffContent.GetComponentsInChildren<ToolTip>(true);//.Where( x => x.transform.name == "$BuffEntry").ToList();

		for (int i = 0; i < myStats.buffList.Count; i++)
		{

		}


		for (int i = 0; i < _buffToolTips.Count; i++)
		{
			_buffToolTips[i].ChangeToolTipText();
		}
		*/



		/*
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
		*/
	}

	IEnumerator TakeEnemyTurn()
	{
		yield return new WaitForSeconds(1);

		selectedAbility = EnemyAI.SelectAbility(MyStats, MyStats.currentHealth, MyStats.currentMana);

		//_playerTurnText.transform.parent.localPosition = Vector3.zero;

		yield return playerCombatController.StartCoroutine(EnemyAI.SpawnAbilityTextUsed(transform, UIController.UICanvas, MyStats, selectedAbility, this));

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

		//print(MyStats.name + " used ability: " + selectedAbility.name + " on " + targetCombatController.MyStats.name + " Ability is of type: " + selectedAbility.abilityType + " (type & selfuse): " + ((AbilityType.buff | AbilityType.defensive | AbilityType.recovery | AbilityType.misc) & (selectedAbility.abilityType)));
		var _targetData = new TargetData(selectedAbility, this, targetCombatController, _centerPos: lastClick);

		//yield return StartCoroutine(InvokeActiveAbility());
		yield return StartCoroutine(SimpleInvokeAbility(_targetData,  true, true, 1));// InvokeActiveAbility());

		myToolTip.SetToolTipText(MyStats.GetToolTipStats());
	}

	public void AdjustPlayerXP(int _amount)
	{
		MyStats.xp += _amount;

		MoveXPSlider();

		//xpSlider.value = myStats.xp;
		if (levelUpText == null && MyStats.xp >= UIController.XPSlider.maxValue)
		{
			levelUpText = EffectTools.SpawnText(transform.position, transform, Color.yellow, "LEVEL UP!");
			levelUpText.StartCoroutine(EffectTools.MoveDirection(levelUpText.transform, Vector3.up, 1, 2));
			levelUpText.StartCoroutine(EffectTools.BlinkText(levelUpText, Color.red + Color.green * 0.75f, 5));
			Destroy(levelUpText.gameObject,4);
		}

		bool _leveledUpAtLeastOnce = MyStats.xp >= UIController.XPSlider.maxValue;

		bool _atMaxHealth = MyStats.currentHealth == MyStats.maxHealth;
		bool _atMaxMana = MyStats.currentMana == MyStats.maxMana;

		while (MyStats.xp >= UIController.XPSlider.maxValue)
		{
			MyStats.maxHealth += LevelUpScreen.instance.currentGroup.healthPerLevel;
			MyStats.maxMana += LevelUpScreen.instance.currentGroup.manaPerLevel;

			LevelUpScreen.instance.traitPointsToSpend += 1;

			if ((MyStats.level + 1) % 2 == 0)
			{
				LevelUpScreen.instance.AddNextChoicesToQue();
			}

			MyStats.level++;
			MyStats.xp -= (int)UIController.XPSlider.maxValue; //subtract this levels xp requirement
			UIController.XPSlider.value = MyStats.xp; //then set the xp bar to show the current xp (which might be over the requirement for a levelup)
			UIController.XPSlider.maxValue = MyStats.level * 10; //set the next levelup to be at 10 times the level
			
			MoveXPSlider();
		}

		if (_leveledUpAtLeastOnce)
		{
			LevelUpScreen.instance.SetLeftoverPointsText();

			MyStats.currentHealth = MyStats.maxHealth;
			MyStats.currentMana = MyStats.maxMana;
			UIController.HealthTextPair.maxObject.text = UIController.HealthTextPair.minObject.text = MyStats.maxHealth.ToString();
			UIController.ManaTextPair.maxObject.text = UIController.ManaTextPair.minObject.text = MyStats.maxMana.ToString();

			UIController.HealthSliderPair.minObject.maxValue = MyStats.maxHealth;
			UIController.ManaSliderPair.minObject.maxValue = MyStats.maxMana;
			//
			//

			UIController.HealthSliderPair.maxObject.maxValue = UIController.HealthSliderPair.minObject.maxValue * SLOW_SLIDER_RATIO_TO_NORMAL;
			UIController.HealthSliderPair.maxObject.value = UIController.HealthSliderPair.maxObject.maxValue;
			UIController.ManaSliderPair.maxObject.maxValue = UIController.ManaSliderPair.minObject.maxValue * SLOW_SLIDER_RATIO_TO_NORMAL;
			UIController.ManaSliderPair.maxObject.value = UIController.ManaSliderPair.maxObject.maxValue;

			if (healthMove != null) StopCoroutine(healthMove);
			if (manaMove != null) StopCoroutine(manaMove);

			if (_atMaxHealth)
				UIController.HealthSliderPair.minObject.value = MyStats.maxHealth;
			else
				healthMove = StartCoroutine(EffectTools.AnimateSlider(UIController.HealthSliderPair.minObject,MyStats.maxHealth,1,1));

			if (_atMaxMana)
				UIController.ManaSliderPair.minObject.value = MyStats.maxMana;
			else
				manaMove = StartCoroutine(EffectTools.AnimateSlider(UIController.ManaSliderPair.minObject, MyStats.maxMana, 1, 1));
		}
	}

	void MoveXPSlider()
	{
		if (xpMove != null)
		{
			//xpSlider.value = myStats.xp;
			StopCoroutine(xpMove);
		}

		xpMove = StartCoroutine(EffectTools.AnimateSlider(UIController.XPSlider, MyStats.xp, 1, 1));
	}

	public int AdjustHealth(int _amount, Elementals _elementals, ExtraData _extraData)
	{
		if (MyStats.currentHealth <= 0) return 0;

		float _amountMultiplier = 1;

		if ((MyStats.weaknesses & _elementals) != 0) _amountMultiplier += 0.5f; //multiplier is booseted if weakness
		if ((MyStats.resistances & _elementals) != 0) _amountMultiplier -= 0.5f; //multiplier is halfed if resist
		if ((MyStats.immunities & _elementals) != 0) _amountMultiplier = 0; //multiplier is set to 0 if immune
		if ((MyStats.absorbs & _elementals) != 0) _amount = Mathf.Abs(_amount); //always heal if absorb
		

		int _totalDamage = MyStats.currentHealth; //store previous health for display text and return value

		if (_amount < 0) //if negative damage
			_amount += ((isCritted) ? (int)Math.Round(_amount/2f,MidpointRounding.AwayFromZero) : 0); //allow critting

		var _damageCalc = Mathf.RoundToInt(_amount * _amountMultiplier); //round up any damage/healing
		if (_damageCalc < 0 && _extraData.HasFlag(ExtraData.nonPiercing)) _damageCalc = Mathf.Min(_damageCalc + ((_extraData.HasFlag(ExtraData.magic))? MyStats.MagicDefense: MyStats.PhysicalDefense), 0); //if value is blockable and is under 0, reduce damage by defense or magicDefense depending on if the move makes contact

		//print("_damage: " + _damageCalc + " has nonP: " + _extraData.HasFlag(ExtraData.nonPiercing) + " typeBlock: " + ((_extraData.HasFlag(ExtraData.magic)) ? MyStats.MagicDefense : MyStats.PhysicalDefense) + " total damage: " + Mathf.Min(_damageCalc + ((_extraData.HasFlag(ExtraData.magic)) ? MyStats.MagicDefense : MyStats.PhysicalDefense), 0));

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

		MyStats.currentHealth = Mathf.Clamp(MyStats.currentHealth + _damageCalc, 0, MyStats.maxHealth);
		_totalDamage -= MyStats.currentHealth;

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
				StartCoroutine(EffectTools.ActivateInOrder(this, new List<EffectTools.FunctionGroup>()
				{
					new EffectTools.FunctionGroup(EffectTools.StretchFromTo(transform,_orgScale,_orgScale * 0.9f, 0.1f),0),
					new EffectTools.FunctionGroup(EffectTools.StretchFromTo(transform, transform.localScale, _orgScale, 0.1f), UnityEngine.Random.Range(0.1f,0.2f))
				}));
			}
		}

		if(playerOwned)
		{
			UIController.HealthTextPair.minObject.text = MyStats.currentHealth.ToString();

			if (healthMove != null)
			{
				StopCoroutine(healthMove);
			}

			if (_damageCalc < 0)
			{
				UIController.HealthSliderPair.minObject.value = MyStats.currentHealth;
				healthMove = StartCoroutine(EffectTools.AnimateSlider(UIController.HealthSliderPair.maxObject, UIController.HealthSliderPair.minObject, 3, SLOW_SLIDER_RATIO_TO_NORMAL));
			}
			else
			{
				UIController.HealthSliderPair.maxObject.value = MyStats.currentHealth * SLOW_SLIDER_RATIO_TO_NORMAL;
				healthMove = StartCoroutine(EffectTools.AnimateSlider(UIController.HealthSliderPair.minObject, UIController.HealthSliderPair.maxObject, 1, 1/SLOW_SLIDER_RATIO_TO_NORMAL));
			}
		}
		else
		{
			myToolTip.SetToolTipText(MyStats.GetToolTipStats());
		}

		if (MyStats.currentHealth <= 0)
		{
			if (!playerOwned)
			{
				playerCombatController.xpPoolToaddAfterCombat += MyStats.level * 3;
				myToolTip.OnPointerExit(null);

				if (turnOrder.Count <= 2) //if this was the last enemy, and the player is left
				{
					playerCombatController.AdjustPlayerXP(playerCombatController.xpPoolToaddAfterCombat);
					playerCombatController.xpPoolToaddAfterCombat = 0;


					PlayerInventory.instance.ProcessDrops(MyStats.drops);
				}

				myEnemyMover.shouldMove = false; //if just took lethal damage, stop moving

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
		var _manaLost = MyStats.currentMana;
		MyStats.currentMana = Mathf.Clamp(MyStats.currentMana + _amount,0,MyStats.maxMana);
		_manaLost -= MyStats.currentMana;

		if(playerOwned)
		{
			UIController.ManaTextPair.minObject.text = MyStats.currentMana.ToString();

			if (manaMove != null)
				StopCoroutine(manaMove);

			if (_manaLost > 0)
			{
				UIController.ManaSliderPair.minObject.value = MyStats.currentMana;
				manaMove = StartCoroutine(EffectTools.AnimateSlider(UIController.ManaSliderPair.maxObject, UIController.ManaSliderPair.minObject, 1, SLOW_SLIDER_RATIO_TO_NORMAL));
			}
			else
			{
				UIController.ManaSliderPair.maxObject.value = MyStats.currentMana * SLOW_SLIDER_RATIO_TO_NORMAL;
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

		bool _hasEnoughMana = playerCombatController.MyStats.currentMana >= -MyStats.abilities.Find(x => x.name == _buttonText).manaCost; //playerCombatController.selectedAbility.manaCost;

		_button.GetComponentInChildren<Image>().color = (_hasEnoughMana)? ColorScheme.activeAbilityColor: ColorScheme.inactiveAbilityColor;
		_button.GetComponent<Button>().enabled = _hasEnoughMana;
		//_button.GetComponent<Collider2D>().enabled = _hasEnoughMana;
	}

	public IEnumerator Click()
	{
		/*
		Debug.DrawRay(HitPosition,Vector3.forward * 10, Color.cyan,5,false);
		RaycastHit _h;
		if (Physics.Raycast(HitPosition,Vector3.forward, out _h, 10))
		{
			print(_h.transform.name);
		}
		else
		{
			print("no hit");
		}
		*/

		/*
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		var _h = Physics2D.GetRayIntersection(ray, 150);
		if (_h.transform != null)
		{
			print("hit: " + _h.transform.name);//Instantiate(particle, transform.position, transform.rotation);
		}

		Debug.DrawRay(ray.origin,ray.direction * 200,Color.cyan,5,false);
	
		*/
		//if (Physics.Raycast(ray))
	





		if (CheckIfHasBuff("busy"))
			yield break;

		RaycastHit2D _hit = CheckIfHit(HitPosition); //get click info
		
		bool _hitSomething = (_hit.collider != null); //store if something was hit

		if (_hitSomething) //what was hit
		{
			//print("name: " + _hit.transform.name + " actedLastTick: " + actedLastTick + " selectedAb: " + selectedAbility);
			
			if(/*_hit.transform.CompareTag("AbilityButton") ||*/ _hit.transform.CompareTag("UI")) //cancel if ui has been hit
			{
				yield break;
			}
		}

		if (UIController.IsCurrentUIMode(UIController.UIMode.None))
		{
			if (selectedAbility != null || PlayerInventory.instance.selectedItem != null)// && !UIController.FleeSlider.gameObject.activeSelf) //if not fleeing, but has an ability selected
			{
				if (_hit.transform != null) //if something was hit
				{
					if (_hit.transform.CompareTag("CritArea")) //if it was a crit area
					{
						targetCombatController = _hit.transform.GetComponentInParent<IAbilityInterractible>();
						//targetCombatController.isCritted = true;
						_hit.transform.GetComponentInParent<CombatController>().isCritted = true;
					}
					else if (_hit.transform.name == "$PlayerPortrait") //if it was the player itself
					{
						//print(selectedAbility.name + " self!");
						targetCombatController = playerCombatController;
					}
					else //if just the normal enemy body was hit
						targetCombatController = _hit.transform.GetComponent<IAbilityInterractible>();
				}
				else //if an ability is selected, but the player missed
				{
					targetCombatController = null;
				}

				UIController.SetUIMode(UIController.UIMode.None);

				actedLastTick = true;
				//StartCoroutine(InvokeActiveAbility()); //activate the ability with the information gathered

				if ((EncounterController.instance.currentGameState & EncounterController.GameState.Realtime) != 0) { AddBusyIfNotInCombat(); }

				if (selectedAbility != null)
				{
					var _targetData = new TargetData(selectedAbility, this, targetCombatController, _centerPos: lastClick);
					
					ResetAbilityPick();
					processingAbility = true;

					yield return StartCoroutine(SimpleInvokeAbility(_targetData, true, true, 1));
					processingAbility = false;

				}
				else if (PlayerInventory.instance.selectedItem != null)
				{
					var _item = PlayerInventory.instance.selectedItem;
					PlayerInventory.instance.ChangeItemQuantity(_item, -1);
					PlayerInventory.instance.ResetItemButton();



					processingAbility = true;

					for (int i = 0; i < _item.item.activeAbilities.Count; i++)
					{
						print("i: " +i);
						var _targetData = new TargetData(_item.item.activeAbilities[i], this, targetCombatController, _item.item.activeConstants[i], _centerPos: lastClick, _useOwnStats: false);
						yield return StartCoroutine(SimpleInvokeAbility(_targetData, false, false, 1, false));
					}

					StartCoroutine(EndTurn());

					processingAbility = false;
				}



				//if (targetCombatController != null) targetCombatController.isCritted = false;
				targetCombatController = null;

			}
		}
	}

	public IEnumerator SimpleInvokeAbility(TargetData _targetData, bool _drainMana, bool _endsTurn, float _minimumUseTime = 0, bool _becomesBusy = false, Action _onComplete = null)
	{
		bool _usedInCombat = (EncounterController.instance.currentGameState & EncounterController.GameState.In_Battle) != 0;

		if (invokingAbility || (turnOrder.Count != 0 && turnOrder[0] != this)) yield break; //if already using an ability or its not this characters turn
		invokingAbility = true; //signal using an ability

		if (_drainMana) AdjustMana(_targetData.ability.manaCost); //if set to remove mana on use; i.e when using an ability, but not when a buff triggers
		if (_becomesBusy) AddBusyIfNotInCombat(); //if not in combat, give the player the busy status

		float orgTime = Time.time; //record when ability started

		yield return StartCoroutine(_targetData.ability.function(_targetData)); //play animation or whatever

		if (_onComplete != null) _onComplete.Invoke(); //if special action is set, invoke it

		if (_minimumUseTime != 0) //if has a minimum time
		{
			var wof = new WaitForEndOfFrame();

			while (orgTime + _minimumUseTime > Time.time) //wait until time expires
				yield return wof;
		}

		invokingAbility = false; //signal done with ability
		if (_endsTurn && _usedInCombat)//(EncounterController.currentGameState & (EncounterController.GameState.Realtime | EncounterController.GameState.Busy)) != 0) //if set to end turn
		{
			//print("ended in: " + EncounterController.instance.currentGameState);
			//EncounterController.shouldMove = true;
			StartCoroutine(EndTurn());
		}

		UpdateBuffIcons();
	}

	void AddBusyIfNotInCombat()
	{
		if (turnOrder.Count == 0)
		{
			AddBuff(new Buff("Busy", Buff.TraitType.Busy, 1, BuffIcons.TryGetBuffIcon(13), Buff.StackType.Add_Duplicate, 1), MyStats);

			EncounterController.instance.speedBoost = 0;
			//EncounterController.instance.currentGameState = EncounterController.GameState.Busy;
			EncounterController.instance.SetGameState(EncounterController.GameState.Busy);
			//EncounterController.shouldMove = false;
			UpdateBuffIcons();
		}
	}

	/*
	/// <summary>
	/// Calls the active ability
	/// </summary>
	public IEnumerator InvokeActiveAbility(bool _byUser = true, float? _value = null)
	{
		print("invoke ability old");
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
			//EncounterController.shouldMove = true;

			yield return new WaitForSeconds( playerOwned? 1:2);
			if (EncounterController.currentGameState == EncounterController.GameState.Battling)
				StartCoroutine( EndTurn());
		}
		invokingAbility = false;
	}
	*/

	IEnumerator RemoveFromTurnOrder(float _sec, CombatController _target)
	{
		turnOrder.Remove(_target);
		yield return new WaitForSeconds(_sec);
		if(turnOrder.Count <= 1)
		{
			yield return EncounterController.instance.DoneWithCombat();

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
			yield return EncounterController.instance.DoneWithCombat();
		}

		startedTurn = false;
	}

	void ResetAbilityPick()
	{
		UIController.CurrentlySelectedImage.gameObject.SetActive(false);

		//UIController.AbilityButtonText.text = UIController.ABILITIES_BUTTON_STRING;
		selectedAbility = null;// string.Empty;
	}

	/*
	public void CloseAllCombatUI()
	{
		ResetAbilityPick();
		UIController.FleeSlider.gameObject.SetActive(false);
		UIController.AbilityMenuScrollView.gameObject.SetActive(false);
	}
	*/
}
