using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ForwardMover : MonoBehaviour
{
	private GameObject segmentPrefab;
	private GameObject enemyPrefab;
	private Sprite[] enemySprites;

	private const float SEGMENT_DISTANCE = 8;

	public const float ENEMY_SPAWN_DISTANCE = 5;

	private List<GameObject> segmentList = new List<GameObject>();

	private static float encounterTimer = 2;
	private float buffTimer = 1;

	public static float speedBoost;

	private float aspectFloat;

	private Dictionary<string,int> monsterDifficulty = new Dictionary<string,int>
	{
		{"ghost", 1},
		{"noseman", 1},
		{"light elemental", 1},
		{"eyeball", 1},
		{"air elemental", 2},
		{"fire elemental", 2},
		{"earth elemental", 2},
		{"water elemental", 2},
		{"harpy", 2},
		{"druid", 2},


	};

	//private CombatController playerCombatController;

	private Vector3[][] offsetTable = new Vector3[][]
	{
		new Vector3[] { new Vector3(0,0,0)/2 },
		new Vector3[] { new Vector3(-0.66f,0,0)/2,new Vector3(0.66f,0,0)/2},
		new Vector3[] { new Vector3(-0.66f,0,0)/2,new Vector3(0,1,0)/2,new Vector3(0.66f,0,0)/2 },
		new Vector3[] { new Vector3(-1,-1,0)/2,new Vector3(1,-1,0)/2,new Vector3(-1,1,0)/2,new Vector3(1,1,0)/2},
		new Vector3[] { new Vector3(-1,-1,0)/2,new Vector3(0,-1,0)/2,new Vector3(1,-1,0)/2,new Vector3(-0.66f,1,0)/2,new Vector3(0.66f,1,0)/2}
	};

	void SetApectUI(RectTransform _recTrans, int _index)
	{
		var _baseOffset = new Vector2(30, 30);
		var _buttonRect = new Vector2(_recTrans.rect.width, _recTrans.rect.height);

		_recTrans.anchorMax = Vector2.zero;
		_recTrans.anchorMin = Vector2.zero;
		_recTrans.offsetMin = new Vector2(_baseOffset.x * (_index + 1) + (_buttonRect.x)  * _index, _baseOffset.y);//(_buttonRect) * ((_index)); // new Vector2(-(120 + _recTrans.rect.width / 2) * (_index + 1), 130);
		_recTrans.offsetMax = new Vector2(_baseOffset.x * (_index + 1) + (_buttonRect.x) * (_index + 1), _baseOffset.y + _buttonRect.y);//new Vector2(230, 230);
	}

    // Start is called before the first frame update
    void Start()
    {
		aspectFloat = Camera.main.GetComponent<Camera>().aspect;//(float)( Screen.currentResolution.width * 10/ Screen.currentResolution.height)/10;
		print(aspectFloat);
		if (aspectFloat >= 1)// && aspectFloat <= 2f)
		{
			//print("aspect float: " + aspectFloat);
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
		enemySprites = Resources.LoadAll<Sprite>("Sprites/Enemies");
		print(enemySprites.Length);
		//playerCombatController = gameObject.GetComponent<CombatController>();

	}

	// Update is called once per frame
	void Update()
    {
		if (encounterTimer > 0)
		{
			encounterTimer -= Time.deltaTime;
			transform.position += Vector3.forward * Time.deltaTime * (5 + speedBoost * 10);
			print(speedBoost);

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

				int _difficulty = Mathf.Clamp(CombatController.playerCombatController.myStats.level * 2 + Random.Range(-2,2),1, CombatController.playerCombatController.myStats.level * 3);

				int _ghosts = 3;//Random.Range(2,4);
				for(int i = 0; i < _ghosts; i++)
				{
					/*
					float _rightOffset = (((float)(i + 1) / _ghosts) - ((float)(_ghosts + 1) / 2) / (float)_ghosts) * _ghosts * 2;
					Vector3 _spawnPos = transform.position + Vector3.forward * 10 + Vector3.right * _rightOffset;
					*/

					var _go = Instantiate(enemyPrefab, transform.position + Vector3.forward * ENEMY_SPAWN_DISTANCE + offsetTable[_ghosts - 1][i] * 2f,Quaternion.identity);
					_go.name = (Random.Range(0,2) == 0) ? "ghost " + i : "ghost " + i;// "light elemental " + i;
					CombatController.turnOrder.Add(_go.GetComponent<CombatController>());
				}

				CombatController.turnOrder.OrderBy(x => (x.myStats.level * 2 + x.myStats.luck));

			}


			//DoneWithCombat();
		}
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
