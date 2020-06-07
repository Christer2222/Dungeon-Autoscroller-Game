using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EncounterController : MonoBehaviour
{
	public static EncounterController instance;

	private GameObject[] segments;
	private GameObject segmentPrefabEmpty;
	private GameObject enemyPrefab;
	//private readonly Dictionary<string, Sprite> enemySpriteDictionary = new Dictionary<string, Sprite>();

	private readonly List<GameObject> segmentList = new List<GameObject>();
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
		None = 1,
		Walking = 2,
		Starting_Battle = 4,
		Battling = 8,
		//Finishing_Combat = 16,
		Busy = 16,
		Realtime = Walking | Busy,
		In_Battle = Starting_Battle | Battling,
	}

    // Start is called before the first frame update
    void Start()
    {
		instance = this;

		encounterTimer = ENCOUNTER_COOLDOWN;//5;

		segmentPrefabEmpty = (GameObject)Resources.Load("Prefabs/Segments/$Segment_Hallway_Empty");
		segments = Resources.LoadAll<GameObject>("Prefabs/Segments/");

		enemyPrefab = (GameObject)Resources.Load("Prefabs/Enemies/Enemy");
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


					SpawnEnemy(_selectedEncounter.monsterBL, 0);
					SpawnEnemy(_selectedEncounter.monsterBM, 1);
					SpawnEnemy(_selectedEncounter.monsterBR, 2);
					SpawnEnemy(_selectedEncounter.monsterTL, 3);
					SpawnEnemy(_selectedEncounter.monsterTM, 4);
					SpawnEnemy(_selectedEncounter.monsterTR, 5);

					CombatController.turnOrder.OrderBy(x => (x.MyStats.level * 2 + x.MyStats.Luck));

					currentGameState = GameState.Battling;
				}


				//DoneWithCombat();
			}
			break;
			case (GameState.Busy):
			{
				if (!CombatController.playerCombatController.CheckIfHasBuff("Busy"))
				{
					currentGameState = GameState.Walking;
				}
			}
			break;
		}
	}

	void SpawnEnemy(StatBlock _monstarStat, int _pos)
	{
		if (_monstarStat == null) return;

		//var _go = Instantiate(enemyPrefab, transform.position + Vector3.forward * ENEMY_SPAWN_DISTANCE + EncounterData.offsetTable[_pos], Quaternion.identity);
		Vector3 _startPos = transform.position + Vector3.forward * ENEMY_SPAWN_DISTANCE;
		
		var _go = Instantiate(enemyPrefab, _startPos + EncounterData.offsetTable[_pos] * 0.25f, Quaternion.identity);

		var _cc = _go.GetComponent<CombatController>();

		_cc.MyStats = _monstarStat.Clone();
		_go.name = _monstarStat.name + " " + _pos;

		_go.GetComponentInChildren<ToolTip>().ChangeToolTipText(_cc.MyStats.GetToolTipStats());
		
		StartCoroutine(EffectTools.PingPongSize(_go.transform, Vector3.zero, Vector3.one * 0.5f, APPEAR_SPEED, 0.5f));
		StartCoroutine(EffectTools.MoveToPoint(_go.transform, _startPos + EncounterData.offsetTable[_pos], APPEAR_SPEED));
		
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

		currentGameState = GameState.Walking;
	}

	private void OnTriggerEnter(Collider _trig)
	{
		if (_trig.CompareTag("Segment"))
		{
			_trig.enabled = false;

			//var _next = Instantiate(segmentPrefabEmpty, _trig.transform.position + Vector3.forward * _trig.transform.localScale.z * SEGMENT_DISTANCE, Quaternion.identity);
			var _next = Instantiate(segments[Random.Range(0,segments.Length)], _trig.transform.position + Vector3.forward * _trig.transform.localScale.z * SEGMENT_DISTANCE, Quaternion.identity);
			segmentList.Add(_next);

			var _enviromentInteractibles = _next.GetComponentsInChildren<TerrainInterractible>();
			foreach (var v in _enviromentInteractibles)
			{
				v.SetStatBlock(EncounterData.urnEnvironmentBlock);
			}

			if (segmentList.Count > 10)
			{
				Destroy(segmentList[0]);
				segmentList.Remove(segmentList[0]);
			}
		}
	}
}
