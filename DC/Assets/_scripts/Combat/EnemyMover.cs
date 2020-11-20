using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
	private CombatController combatController;

	//variables for enemy movement
	private readonly List<Vector3> localEnemyMovePoints = new List<Vector3>();
	private int positionIndex;
	private float moveSpeed;

	private Vector3 home;
	[HideInInspector]public bool shouldMove = false;

	private Vector3 nextPos = Vector3.zero;
	// Start is called before the first frame update
	public void Initialize(float _moveSpeed)
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
		moveSpeed = _moveSpeed/10 + 0.1f;//combatController.MyStats.level; //(float)combatController.MyStats.Dexterity / 10; // Random.Range(0.2f,2f);

		nextPos = localEnemyMovePoints[positionIndex] + home;
		shouldMove = true;
	}

    // Update is called once per frame
    void Update()
    {
		if(!shouldMove)//!CombatController.turnOrder.Contains(combatController))
			return;
		
		if(Vector2.Distance(transform.position,nextPos) < 0.1f)
		{
			positionIndex++;
			positionIndex %= localEnemyMovePoints.Count;
			nextPos = localEnemyMovePoints[positionIndex] + home;
		}
		//print(positionIndex);
		transform.position = Vector3.MoveTowards(transform.position,localEnemyMovePoints[positionIndex] + home,Time.deltaTime * moveSpeed);
	}
}
