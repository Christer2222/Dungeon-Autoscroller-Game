using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ForwardMover : MonoBehaviour
{
	private GameObject segmentPrefab;
	private GameObject enemyPrefab;
	private Dictionary<string, Sprite> enemySpriteDictionary = new Dictionary<string, Sprite>();

	private const float SEGMENT_DISTANCE = 8;

	public const float ENEMY_SPAWN_DISTANCE = 5;

	private List<GameObject> segmentList = new List<GameObject>();

	public static float encounterTimer = 2;
	public static float buffTimer = 1;

	public static float speedBoost;

	private float aspectFloat;


	void SetApectUI(RectTransform _recTrans, int _index)
	{
		var _baseOffset = new Vector2(30, 30);
		var _buttonRect = new Vector2(_recTrans.rect.width, _recTrans.rect.height);

		_recTrans.anchorMax = Vector2.zero;
		_recTrans.anchorMin = Vector2.zero;
		_recTrans.offsetMin = new Vector2(_baseOffset.x * (_index + 1) + (_buttonRect.x)  * _index, _baseOffset.y);
		_recTrans.offsetMax = new Vector2(_baseOffset.x * (_index + 1) + (_buttonRect.x) * (_index + 1), _baseOffset.y + _buttonRect.y);
	}

    // Start is called before the first frame update
    void Start()
    {
		aspectFloat = Camera.main.aspect;
		if (aspectFloat >= 1)
		{
			RectTransform _abRec = GameObject.Find("$AbilityButton").GetComponent<RectTransform>();
			SetApectUI(_abRec, 0);

			RectTransform _itRec = GameObject.Find("$ItemsButton").GetComponent<RectTransform>();
			SetApectUI(_itRec, 1);

			RectTransform _flRec = GameObject.Find("$FleeButton").GetComponent<RectTransform>();
			SetApectUI(_flRec, 2);

			RectTransform _opRec = GameObject.Find("$OptionsButton").GetComponent<RectTransform>();
			SetApectUI(_opRec, 3);

			RectTransform _plRec = GameObject.Find("$PlayerPortrait").GetComponent<RectTransform>();
			_plRec.anchorMax = new Vector2(1, 0);
			_plRec.anchorMin = new Vector2(1, 0);
			_plRec.offsetMin = new Vector2(-256,0);
			_plRec.offsetMax = new Vector2(0, 256);

			RectTransform _hpRec = GameObject.Find("$HealthSlider").GetComponent<RectTransform>();
			_hpRec.anchorMin = new Vector2(1, 0);
			_hpRec.anchorMax = new Vector2(1, 0);
			_hpRec.offsetMin = new Vector2(_opRec.localPosition.x - _hpRec.localPosition.x, 128);
			_hpRec.offsetMax = new Vector2(-256, 256);

			RectTransform _mpRec = GameObject.Find("$ManaSlider").GetComponent<RectTransform>();
			_mpRec.anchorMin = _hpRec.anchorMin;
			_mpRec.anchorMax = _hpRec.anchorMax;
			_mpRec.offsetMin = new Vector2(_opRec.localPosition.x - _mpRec.localPosition.x, 0);
			_mpRec.offsetMax = new Vector2(-256, 128);
		}


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
			encounterTimer -= Time.deltaTime;
			transform.position += Vector3.forward * Time.deltaTime * (5 + speedBoost * 10);

			speedBoost = Mathf.Clamp(speedBoost - Time.deltaTime,0,float.MaxValue);
			buffTimer -= Time.deltaTime;
			if (buffTimer <= 0)
			{
				buffTimer = 1;
				CombatController.playerCombatController.TickBuffs();
			}
		}
		else
		{
			if (CombatController.turnOrder.Count == 0)
			{
				CombatController.turnOrder.Add(CombatController.playerCombatController);

				var _playerStats = CombatController.playerCombatController.myStats;

				var _possibles = EncounterData.encounterTable.Where(x => x.level == _playerStats.level + 1).ToArray(); //find encounter of equal level
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

				if (_playerStats.level == 1 && _playerStats.xp == 0) //if this is the first battle
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

		var _go = Instantiate(enemyPrefab, transform.position + Vector3.forward * ENEMY_SPAWN_DISTANCE + EncounterData.offsetTable[_pos], Quaternion.identity);
		var _cc = _go.GetComponent<CombatController>();

		_cc.myStats = _monstarStat;
		_go.name = _monstarStat.name + " " + _pos;
		var _sprite = _monstarStat.name.Replace(" ", "_").ToLower();
		_go.GetComponent<SpriteRenderer>().sprite = enemySpriteDictionary.TryGetValue(_sprite, out Sprite _out) ? _out: enemySpriteDictionary["unknown_sprite"];
		CombatController.turnOrder.Add(_cc);
	}


	public static void DoneWithCombat()
	{
		encounterTimer = Random.Range(5,10);
		CombatController.turnOrder.Clear();
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
