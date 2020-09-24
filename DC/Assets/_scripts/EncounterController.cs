using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EncounterController : MonoBehaviour
{
	public static EncounterController instance;

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
		None = 0x01,
		Walking = 0x02,
		Starting_Battle = 0x04,
		Battling = 0x8,
		//Finishing_Combat = 0x10,
		Busy = 0x20,
		ConfirmingDrops = 0x40,
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

		segmentPrefabEmpty		= (GameObject)Resources.Load("Prefabs/Segments/$Segment_Hallway_Empty");
		segmentPrefabTorch		= (GameObject)Resources.Load("Prefabs/Segments/$Segment_Hallway_Torch");
		segmentPrefabUrn		= (GameObject)Resources.Load("Prefabs/Segments/$Segment_Hallway_Urn");
		segmentPrefabStalagmite = (GameObject)Resources.Load("Prefabs/Segments/$Segment_Hallway_Stalagmite");

		segmentPrefabs = new GameObject[] { segmentPrefabEmpty, segmentPrefabTorch, segmentPrefabUrn, segmentPrefabStalagmite };

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
		if ((currentGameState & GameState.Realtime) != 0 && !UIController.IsFullscreenUI())
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
				if ((UIController.currentUIMode & (UIController.UIMode.FullScreen)) == 0) //if the current uimode doesn't have the inventory or levelup flag set
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
							ResetEncounterTimer();
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
				if (!CombatController.playerCombatController.CheckIfHasBuff("Busy") && currentGameState != GameState.ConfirmingDrops)
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

		ResetEncounterTimer();

		if (currentGameState != GameState.ConfirmingDrops)
			SetGameState(GameState.Walking);

		//SetGameState(GameState.Walking);
		//currentGameState = GameState.Walking;
	}

	private void OnTriggerEnter(Collider _trig)
	{
		if (_trig.CompareTag("Segment"))
		{
			
			var _enviromentInteractibles = _trig.GetComponentsInChildren<TerrainInterractible>();
			foreach (var v in _enviromentInteractibles)
			{
				v.StartCoroutine(v.trapTriggered());//.Invoke();
			}
			

			_trig.enabled = false;

			//var _next = Instantiate(segmentPrefabEmpty, _trig.transform.position + Vector3.forward * _trig.transform.localScale.z * SEGMENT_DISTANCE, Quaternion.identity);
			GameObject _chosenPrefab = segmentPrefabs[Random.Range(0, segmentPrefabs.Length)];
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
