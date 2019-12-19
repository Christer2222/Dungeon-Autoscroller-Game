using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ForwardMover : MonoBehaviour
{
	private GameObject segmentPrefab;
	private GameObject enemyPrefab;
	private Dictionary<string, Sprite> enemySpriteDictionary = new Dictionary<string, Sprite>();

	private List<GameObject> segmentList = new List<GameObject>();
	private const float SEGMENT_DISTANCE = 8;

	public static float encounterTimer = 1;//5;
	public const float ENEMY_SPAWN_DISTANCE = 5;

	private const float DEFAULT_BUFF_TIMER = 5;
	public static float buffTimer = DEFAULT_BUFF_TIMER;

	public static float speedBoost;
	public static bool shouldMove = true;

    // Start is called before the first frame update
    void Start()
    {
		segmentPrefab = (GameObject)Resources.Load("Prefabs/Segment");
		enemyPrefab = (GameObject)Resources.Load("Prefabs/Enemies/Enemy");
		var enemySprites = Resources.LoadAll<Sprite>("Sprites/Enemies");
		for (int i = 0; i < enemySprites.Length; i++)
		{
			enemySpriteDictionary.Add(enemySprites[i].name.Replace(" ", "_"), enemySprites[i]);
		}
		//playerCombatController = gameObject.GetComponent<CombatController>();

	}

	// Update is called once per frame
	void Update()
    {
		if (encounterTimer > 0)
		{
			if (shouldMove)
			{
				encounterTimer -= Time.deltaTime;
				transform.position += Vector3.forward * Time.deltaTime * (5 + speedBoost * 10);

				speedBoost = Mathf.Max(speedBoost - Time.deltaTime,0);
				buffTimer -= Time.deltaTime;
				if (buffTimer <= 0)
				{
					buffTimer = DEFAULT_BUFF_TIMER;
					CombatController.playerCombatController.TickBuffs();
				}
			}
		}
		else
		{
			if (CombatController.turnOrder.Count == 0)
			{
				CombatController.turnOrder.Add(CombatController.playerCombatController);

				CombatController.playerCombatController.RemoveAllBufsWithName("busy");

				var _playerStats = CombatController.playerCombatController.myStats;

				var _possibles = EncounterData.encounterTable.Where(x => x.level == _playerStats.level).ToArray(); //find encounter of equal level
				int _lower = _playerStats.level; //store level
				while (_possibles.Length == 0) //if no encounters
				{
					_lower--; //lower level check by 1
					_possibles = EncounterData.encounterTable.Where(x => x.level == _lower).ToArray(); //find encounters
					if (_lower <= 0) //if checking below 0, end search
					{
						encounterTimer = 5;
						return;
					}
				}

				if (!Options.finishedTutorial) //if this is the first battle
					_possibles = EncounterData.encounterTable.Where(x => x.level == 0).ToArray(); //overwrite encounter check for the easiest

				EncounterData.Encounter _selectedEncounter = _possibles[Random.Range(0,_possibles.Length)];

				SpawnEnemy(_selectedEncounter.monsterBL, 0);
				SpawnEnemy(_selectedEncounter.monsterBM, 1);
				SpawnEnemy(_selectedEncounter.monsterBR, 2);
				SpawnEnemy(_selectedEncounter.monsterTL, 3);
				SpawnEnemy(_selectedEncounter.monsterTM, 4);
				SpawnEnemy(_selectedEncounter.monsterTR, 5);

				CombatController.turnOrder.OrderBy(x => (x.myStats.level * 2 + x.myStats.luck));

			}


			//DoneWithCombat();
		}
    }

	void SpawnEnemy(StatBlock _monstarStat, int _pos)
	{
		if (_monstarStat == null) return;

		print(EncounterData.offsetTable[_pos]);
		var _go = Instantiate(enemyPrefab, transform.position + Vector3.forward * ENEMY_SPAWN_DISTANCE + EncounterData.offsetTable[_pos], Quaternion.identity);
		var _cc = _go.GetComponent<CombatController>();

		_cc.myStats = _monstarStat.Clone();
		_go.name = _monstarStat.name + " " + _pos;
		var _sprite = _monstarStat.name.Replace(" ", "_").ToLower();
		_go.GetComponent<SpriteRenderer>().sprite = TryGetEnemySprite(_sprite);
		CombatController.turnOrder.Add(_cc);
	}

	Sprite TryGetEnemySprite(string _name)
	{
		return enemySpriteDictionary.TryGetValue(_name, out Sprite _out) ? _out : enemySpriteDictionary["Unknown_Sprite"];
	}


	public static void DoneWithCombat()
	{
		if (CombatController.turnOrder.Count >= 1)
			Options.finishedTutorial = true;

		encounterTimer = Random.Range(5,10);

		CombatController.turnCounter = 0;
		CombatController.turnOrder.Clear();
		CombatController.playerCombatController.startedTurn = false;
		CombatController.UpdateTurnOrderDisplay();
	}

	private void OnTriggerEnter(Collider _trig)
	{
		if (_trig.CompareTag("Segment"))
		{
			_trig.enabled = false;
			segmentList.Add(Instantiate(segmentPrefab,_trig.transform.position + Vector3.forward * _trig.transform.localScale.z * SEGMENT_DISTANCE,Quaternion.identity));

			if (segmentList.Count > 10)
			{
				Destroy(segmentList[0]);
				segmentList.Remove(segmentList[0]);
			}
		}
	}
}
