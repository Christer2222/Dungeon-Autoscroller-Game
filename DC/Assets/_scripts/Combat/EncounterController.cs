using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EncounterController : MonoBehaviour
{
	public static EncounterController instance;

	private static GameObject[] plainsSegments, forestSegments, steppesSegments, tundreaSegments, oceanSegments, waterSegments, dungeonSegments, desertSegments;

	private GameObject[] segmentPrefabs;
	private GameObject segmentPrefabEmpty, segmentPrefabTorch, segmentPrefabUrn, segmentPrefabStalagmite;
	private GameObject enemyPrefab;
	//private readonly Dictionary<string, Sprite> enemySpriteDictionary = new Dictionary<string, Sprite>();

	private readonly List<GameObject> segmentsInMap = new List<GameObject>();
	private const float SEGMENT_DISTANCE = 8;

	private float encounterTimer;
	public const float ENEMY_SPAWN_DISTANCE = 5;
	private const float APPEAR_SPEED = 1.4f;
	private const float ENCOUNTER_COOLDOWN = 10;

	private const float DEFAULT_BUFF_TIMER = 5;
	public float buffTimer = DEFAULT_BUFF_TIMER;

	public float speedBoost;
	//private bool finnishingCombat;

	public GameState currentGameState = GameState.Walking;
	public enum GameState
	{
		None = 0x00,
		Walking = 0x02,
		Starting_Battle = 0x04,
		Battling = 0x8,
		//Finishing_Combat = 0x10,
		Busy = 0x20,
		Confirming_Drops = 0x40,
		Waiting_For_Path = 0x80,
		Realtime = Walking | Busy,
		In_Battle = Starting_Battle | Battling,
	}


	public void SetGameState(GameState state)
	{
		currentGameState = state;
	}

    // Start is called before the first frame update
    void Start()
    {
		instance = this;

		encounterTimer = ENCOUNTER_COOLDOWN;//5;

		//segmentPrefabEmpty		= (GameObject)Resources.Load("$Environment/$DungeonSegments/$Segment_Hallway_Empty");
		//segmentPrefabTorch		= (GameObject)Resources.Load("$Environment/$DungeonSegments/$Segment_Hallway_Torch");
		//segmentPrefabUrn		= (GameObject)Resources.Load("$Environment/$DungeonSegments/$Segment_Hallway_Urn");
		//segmentPrefabStalagmite = (GameObject)Resources.Load("$Environment/$DungeonSegments/$Segment_Hallway_Stalagmite");
		//segmentPrefabs = new GameObject[] { segmentPrefabEmpty, segmentPrefabTorch, segmentPrefabUrn, segmentPrefabStalagmite };

		if (plainsSegments == null)		plainsSegments	= Resources.LoadAll<GameObject>("$Environment/$PlainsSegments");
		if (desertSegments == null)		desertSegments	= Resources.LoadAll<GameObject>("$Environment/$DesertSegments");
		if (steppesSegments == null)	steppesSegments = Resources.LoadAll<GameObject>("$Environment/$SteppesSegments");
		if (forestSegments == null)		forestSegments	= Resources.LoadAll<GameObject>("$Environment/$ForestSegments");
		if (oceanSegments == null)		oceanSegments	= Resources.LoadAll<GameObject>("$Environment/$OceanSegments");
		if (tundreaSegments == null)	tundreaSegments	= Resources.LoadAll<GameObject>("$Environment/$TundraSegments");
		if (waterSegments == null)		waterSegments	= Resources.LoadAll<GameObject>("$Environment/$WaterSegments");
		if (dungeonSegments == null)	dungeonSegments = Resources.LoadAll<GameObject>("$Environment/$DungeonSegments");


		//segments = Resources.LoadAll<GameObject>("Prefabs/Segments/");

		enemyPrefab = (GameObject)Resources.Load("Prefabs/Enemies/$EnemyCanvas");
		//var enemySprites = Resources.LoadAll<Sprite>("Sprites/Enemies");
		/*
		for (int i = 0; i < enemySprites.Length; i++)
		{
			enemySpriteDictionary.Add(enemySprites[i].name.Replace(" ", "_"), enemySprites[i]);
		}
		*/
		//playerCombatController = gameObject.GetComponent<CombatController>();
	}


	private void ResetEncounterTimer()
	{
		encounterTimer = ENCOUNTER_COOLDOWN;
	}

	// Update is called once per frame
	void Update()
    {
		//print("t: " + (encounterTimer/2).ToString("0") + currentGameState);
		//print("gs: " + currentGameState);
		if ((currentGameState & GameState.Realtime) != 0 && !UIController.IsFullscreenUI() && ((currentGameState & GameState.Waiting_For_Path) == 0)) //if it is realtime, not fullscreen, and not waiting for path
		{
			encounterTimer -= Time.deltaTime; //count down for the next encounter

			if (encounterTimer <= 0)
			{
				currentGameState = GameState.Starting_Battle;
				//CombatController.playerCombatController.RemoveAllBufsWithName("Busy");
				
			}
			else
			{
				buffTimer -= Time.deltaTime;
				if (buffTimer <= 0)
				{
					buffTimer = DEFAULT_BUFF_TIMER;
					StartCoroutine(CombatController.playerCombatController.TickBuffs());
				}
			}
		}


		switch (currentGameState)
		{
			case (GameState.Walking)://encounterTimer > 0)
			{
				if (!UIController.IsFullscreenUI())//(UIController.currentUIMode & (UIController.UIMode.FullScreen)) == 0) //if the current uimode doesn't have the inventory or levelup flag set
				{
					transform.position += Vector3.forward * Time.deltaTime * (5 + speedBoost * 10);

					speedBoost = Mathf.Max(speedBoost - Time.deltaTime, 0);
				}
			}
			break;
			case (GameState.Starting_Battle):
			{
				if (CombatController.turnOrder.Count == 0)// && !finnishingCombat)
				{
					ResetEncounterTimer();

					CombatController.turnOrder.Add(CombatController.playerCombatController);

					CombatController.playerCombatController.RemoveAllBufsWithName("busy");

					var _playerStats = CombatController.playerCombatController.MyStats;

					var _possibles = EncounterData.encounterTable.Where(x => x.level == _playerStats.level).ToArray(); //find encounter of equal level
					int _lower = _playerStats.level; //store level
					while (_possibles.Length == 0) //if no encounters
					{
						_lower--; //lower level check by 1
						_possibles = EncounterData.encounterTable.Where(x => x.level == _lower).ToArray(); //find encounters
						if (_lower <= 0) //if checking below 0, end search
						{
							//ResetEncounterTimer();
							//encounterTimer = 5;
							return;
						}
					}

					if (!Options.finishedTutorial) //if this is the first battle
						_possibles = EncounterData.encounterTable.Where(x => x.level == 0).ToArray(); //overwrite encounter check for the easiest

					EncounterData.Encounter _selectedEncounter = _possibles[Random.Range(0, _possibles.Length)];

					_selectedEncounter = EncounterData.RandomizeEncounter(_selectedEncounter);

					int divider = 0;
					int GetSpeed(StatBlock encounter)
					{
						if (encounter != null)
						{
							divider++;
							return encounter.level;
						}
						return 0;
					}

					int averageSpeed = (
						GetSpeed(_selectedEncounter.monsterBL) +
						GetSpeed(_selectedEncounter.monsterBM) +
						GetSpeed(_selectedEncounter.monsterBR) +
						GetSpeed(_selectedEncounter.monsterTL) +
						GetSpeed(_selectedEncounter.monsterTM) +
						GetSpeed(_selectedEncounter.monsterTR))/divider;
				

					SpawnEnemy(_selectedEncounter.monsterBL, 0, averageSpeed);
					SpawnEnemy(_selectedEncounter.monsterBM, 1, averageSpeed);
					SpawnEnemy(_selectedEncounter.monsterBR, 2, averageSpeed);
					SpawnEnemy(_selectedEncounter.monsterTL, 3, averageSpeed);
					SpawnEnemy(_selectedEncounter.monsterTM, 4, averageSpeed);
					SpawnEnemy(_selectedEncounter.monsterTR, 5, averageSpeed);

					CombatController.turnOrder.OrderBy(x => (x.MyStats.level * 2 + x.MyStats.Luck));

					currentGameState = GameState.Battling;
				}


				//DoneWithCombat();
			}
			break;
			case (GameState.Busy):
			{
				if (!CombatController.playerCombatController.CheckIfHasBuff("Busy"))// && currentGameState != GameState.ConfirmingPrompt)
				{
					SetGameState(GameState.Walking);
					//currentGameState = GameState.Walking;
				}
			}
			break;
		}
	}

	void SpawnEnemy(StatBlock _monstarStat, int _pos, float _speed)
	{
		if (_monstarStat == null) return;

		//var _go = Instantiate(enemyPrefab, transform.position + Vector3.forward * ENEMY_SPAWN_DISTANCE + EncounterData.offsetTable[_pos], Quaternion.identity);
		Vector3 _startPos = transform.position + Vector3.forward * ENEMY_SPAWN_DISTANCE;
		
		var _go = Instantiate(enemyPrefab, _startPos + EncounterData.offsetTable[_pos] * 0.25f, Quaternion.identity);

		var _cc = _go.GetComponent<CombatController>();
		var _em = _go.GetComponent<EnemyMover>();

		_cc.MyStats = _monstarStat.Clone();
		_go.name = _monstarStat.name + " " + _pos;

		_go.GetComponentInChildren<ToolTip>().SetToolTipText(_cc.MyStats.GetToolTipStats());
		
		//StartCoroutine(EffectTools.PingPongSize(_go.transform, Vector3.zero, Vector3.one * 0.5f, APPEAR_SPEED, 0.5f));
		//StartCoroutine(EffectTools.MoveToPoint(_go.transform, _startPos + EncounterData.offsetTable[_pos], APPEAR_SPEED));

		StartCoroutine(EffectTools.ActivateInOrder(_em, new List<EffectTools.FunctionGroup>() 
		{
			new EffectTools.FunctionGroup(new List<IEnumerator>() 
			{
				EffectTools.PingPongSize(_go.transform, Vector3.zero, Vector3.one * 0.5f, APPEAR_SPEED, 0.5f),
				EffectTools.MoveToPoint(_go.transform, _startPos + EncounterData.offsetTable[_pos], APPEAR_SPEED)
			},0),
			
			new EffectTools.FunctionGroup(StartMove(),APPEAR_SPEED) 
		}
			
			));


		IEnumerator StartMove()
		{
			yield return null;
			_em.Initialize(_speed);
			//_em.shouldMove = true;
		}

		//var _sprite = _monstarStat.name.Replace(" ", "_").ToLower();
		//_go.GetComponent<SpriteRenderer>().sprite = TryGetEnemySprite(_sprite);
		CombatController.turnOrder.Add(_cc);
	}

	/*
	Sprite TryGetEnemySprite(string _name)
	{
		return enemySpriteDictionary.TryGetValue(_name, out Sprite _out) ? _out : enemySpriteDictionary["Unknown_Sprite"];
	}
	*/

	public IEnumerator DoneWithCombat()
	{
		//currentGameState = GameState.Finishing_Combat;
		//print("done with combat");
		if (CombatController.turnOrder.Count >= 1)
			Options.finishedTutorial = true;

		CombatController.turnCounter = 0;
		CombatController.turnOrder.Clear();
		CombatController.playerCombatController.startedTurn = false;

		yield return new WaitForSeconds(3);

		CombatController.UpdateTurnOrderDisplay();

		//ResetEncounterTimer();

		if (currentGameState != GameState.Confirming_Drops)
			SetGameState(GameState.Walking);

		//SetGameState(GameState.Walking);
		//currentGameState = GameState.Walking;
	}

	public void RemoveFlagFromGameState(GameState gameState)
	{
		currentGameState &= ~gameState;

		if (currentGameState == GameState.None)
			SetGameState(GameState.Walking);
		//currentGameState &= ~GameState.Waiting_For_Path; //remove the waiting for path flag
	}

	public void AddFlagFromGameState(GameState gameState)
	{
		currentGameState |= gameState;
	}

	private void OnTriggerEnter(Collider _trig)
	{
		if (_trig.CompareTag("Segment"))
		{
			int paths = PathPicker.instance.GoToNextNode();
			if (paths != 1)
			{
				RemoveFlagFromGameState(GameState.Walking);
				UIController.PathChoiceButtonsHolder.gameObject.SetActive(true);//!UIController.PathChoiceButtonsHolder.gameObject.activeSelf);
			}

			/*
			bool canContinue = PathPicker.instance.GoToNextNode();
			if (!canContinue && ((currentGameState & GameState.Confirming_Drops) == 0))
				SetGameState(GameState.Waiting_For_Path);
			*/

			var _enviromentInteractibles = _trig.GetComponentsInChildren<TerrainInterractible>();
			foreach (var v in _enviromentInteractibles)
			{
				v.StartCoroutine(v.trapTriggered());//.Invoke();
			}
			

			_trig.enabled = false;

			GameObject _chosenPrefab = null;
			switch (PathPicker.instance.currentNode.connectionInfo.sceneryHere)
			{
				case PathNode.Scenery.Plains:
					_chosenPrefab = GetPrefabFromArray(plainsSegments);
					break;
				case PathNode.Scenery.Forest:
					_chosenPrefab = GetPrefabFromArray(forestSegments);
					break;
				case PathNode.Scenery.Steppes:
					_chosenPrefab = GetPrefabFromArray(steppesSegments);
					break;
				//case PathNode.Scenery.Coast:
					break;
				case PathNode.Scenery.Desert:
					_chosenPrefab = GetPrefabFromArray(desertSegments);
					break;
				case PathNode.Scenery.Sea:
					_chosenPrefab = GetPrefabFromArray(waterSegments);
					break;
				//case PathNode.Scenery.Tropical:
					break;
				case PathNode.Scenery.Water:
					_chosenPrefab = GetPrefabFromArray(waterSegments);
					break;
				case PathNode.Scenery.Tundra:
					_chosenPrefab = GetPrefabFromArray(tundreaSegments);
					break;
				//case PathNode.Scenery.Mountains:
					break;
				case PathNode.Scenery.Dungeon:
					_chosenPrefab = GetPrefabFromArray(dungeonSegments);
					break;
				default:
					_chosenPrefab = GetPrefabFromArray(plainsSegments);
					break;
			}
			
			GameObject GetPrefabFromArray(in GameObject[] array)
			{
				print("array: " + array.Length);
				return array[Random.Range(0, array.Length)];
			}

			//var _next = Instantiate(segmentPrefabEmpty, _trig.transform.position + Vector3.forward * _trig.transform.localScale.z * SEGMENT_DISTANCE, Quaternion.identity);
			//_chosenPrefab = segmentPrefabs[Random.Range(0, segmentPrefabs.Length)];
			var _next = Instantiate(_chosenPrefab, _trig.transform.position + Vector3.forward * _trig.transform.localScale.z * SEGMENT_DISTANCE, Quaternion.identity);
			segmentsInMap.Add(_next);

			
			var _newEnviromentInteractibles = _next.GetComponentsInChildren<TerrainInterractible>();
			foreach (var v in _newEnviromentInteractibles)
			{
				if (_chosenPrefab == segmentPrefabStalagmite)
				{
					v.SetStatBlock(EncounterData.stalagmiteEnvironmentBlock);
					v.trapTriggered = v.StalagmiteTrap;
				}
				else// (_next == segmentPrefabUrn)
				{
					v.SetStatBlock(EncounterData.urnEnvironmentBlock);
					v.trapTriggered = v.PrintName;
				}

				/*
				if (v.MyStats == EncounterData.stalagmiteEnvironmentBlock)
				{
					print("stalag create");
				}
				else
				{
					print("urn create");
				}
				*/
			}
			
			if (segmentsInMap.Count > 10)
			{
				Destroy(segmentsInMap[0]);
				segmentsInMap.Remove(segmentsInMap[0]);
			}
		}
	}
}
