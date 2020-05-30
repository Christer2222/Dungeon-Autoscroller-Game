using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
	private CombatController combatController;

	//variables for enemy movement
	private readonly List<Vector3> localEnemyMovePoints = new List<Vector3>();
	private Vector3 home;
	private int positionIndex;
	private float moveSpeed;
	public bool shouldMove = true;

	// Start is called before the first frame update
	void Start()
    {
		//home = transform.position;
		combatController = GetComponent<CombatController>();
		home = transform.position;

		localEnemyMovePoints.Add(new Vector3(0,1,0));
		localEnemyMovePoints.Add(new Vector3(0,0,0));
		localEnemyMovePoints.Add(new Vector3(0,-1,0));
		//localEnemyMovePoints.Add(new Vector3(-1,0,0));
		//localEnemyMovePoints.Add(new Vector3(0.2f,0,0));

		//int _randomIndex = Random.Range(0,localEnemyMovePoints.Count - 1);
		//transform.position += localEnemyMovePoints[_randomIndex];
		positionIndex = 1;// _randomIndex;
		moveSpeed = (float)combatController.MyStats.Dexterity / 10; // Random.Range(0.2f,2f);
	}

    // Update is called once per frame
    void Update()
    {
		/*
		if(!shouldMove)//!CombatController.turnOrder.Contains(combatController))
			return;
		
		if(Vector2.Distance(transform.position,localEnemyMovePoints[positionIndex] + home) < 0.1f)
		{
			positionIndex++;
			positionIndex %= localEnemyMovePoints.Count;
		}

		transform.position = Vector3.MoveTowards(transform.position,localEnemyMovePoints[positionIndex] + home,Time.deltaTime * moveSpeed);
		*/
	}
}
