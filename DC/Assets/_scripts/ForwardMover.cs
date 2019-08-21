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

	private CombatController playerCombatController;

	private Vector3[][] offsetTable = new Vector3[][]
	{
		new Vector3[] { new Vector3(0,0,0)/2 },
		new Vector3[] { new Vector3(-0.66f,0,0)/2,new Vector3(0.66f,0,0)/2},
		new Vector3[] { new Vector3(-0.66f,0,0)/2,new Vector3(0,1,0)/2,new Vector3(0.66f,0,0)/2 },
		new Vector3[] { new Vector3(-1,-1,0)/2,new Vector3(1,-1,0)/2,new Vector3(-1,1,0)/2,new Vector3(1,1,0)/2},
		new Vector3[] { new Vector3(-1,-1,0)/2,new Vector3(0,-1,0)/2,new Vector3(1,-1,0)/2,new Vector3(-0.66f,1,0)/2,new Vector3(0.66f,1,0)/2}
	};

    // Start is called before the first frame update
    void Start()
    {
		segmentPrefab = (GameObject)Resources.Load("Prefabs/Segment");
		enemyPrefab = (GameObject)Resources.Load("Prefabs/Enemies/Enemy");
		enemySprites = Resources.LoadAll<Sprite>("Sprites/Enemies");
		print(enemySprites.Length);
		playerCombatController = gameObject.GetComponent<CombatController>();
		
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
				playerCombatController.TickBuffs();
			}
		}
		else
		{
			if (CombatController.turnOrder.Count == 0)
			{
				CombatController.turnOrder.Add(playerCombatController);

				int _difficulty = Mathf.Clamp(playerCombatController.myStats.level * 2 + Random.Range(-2,2),1,playerCombatController.myStats.level * 3);

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
