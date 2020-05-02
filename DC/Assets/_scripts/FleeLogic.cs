using Boo.Lang;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class FleeLogic : MonoBehaviour
{
	private Image thresholdIndicatorImage;
	private Slider fleeSlider;
	private float prevVal;
	private float timer;
	private bool waiting;

	private int FleeThreshold
	{
		get
		{
			return 5 + CombatController.playerCombatController.myStats.Dexterity;
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		fleeSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(Options.acceptKey))
		{
			prevVal = fleeSlider.value;
			var _hit = AbilityScript.CheckIfHit(AbilityScript.HitPosition);
			if (_hit.transform != null)
			{
				print(_hit.transform.name + " tag: " + _hit.transform.tag);

				if (!_hit.transform.CompareTag("UI") && _hit.transform.name != CombatController.playerCombatController.gameObject.name)
					CombatController.playerCombatController.StartCoroutine(Flee());

			}
			else
			{
				CombatController.playerCombatController.StartCoroutine(Flee());
			}

			return;
		}

		if (!waiting)
		{
			fleeSlider.value = Mathf.PingPong(timer * fleeSlider.maxValue, fleeSlider.maxValue);
			timer += Time.deltaTime;
		}
	}

	IEnumerator Flee()
	{
		fleeSlider.value = prevVal;
		waiting = true;

		if (Mathf.Abs((fleeSlider.maxValue / 2) - (fleeSlider.value)) <= FleeThreshold) //if the pointer is withing the flee zone, succeed
		{
			//var _tempTurnorder = new CombatController[CombatController.turnOrder.Count];
			//CombatController.turnOrder.CopyTo(_tempTurnorder);
			List<GameObject> _slatedForRemoval = new List<GameObject>();
			foreach (CombatController _cc in CombatController.turnOrder)//_tempTurnorder) //for all entries in the temp turn order
			{
				if (_cc != CombatController.playerCombatController)
				{
					_slatedForRemoval.Add(_cc.gameObject);
					//Destroy(_cc.gameObject, 2);
				}
			}
			yield return EncounterController.DoneWithCombat();
			
			foreach(var v in _slatedForRemoval)
			{
				Destroy(v,2);
			}


			CombatController.turnOrder.Clear();

			EncounterController.speedBoost = 3; //run after fleeing

			if (UIController.IsCurrentUIMode(UIController.UIMode.Flee))
				UIController.SetUIMode(UIController.UIMode.None);
			//gameObject.SetActive(false);
		}
		else
		{
			yield return CombatController.playerCombatController.EndTurn(); //if failed fleeing, end the turn as an action
		}

		CombatController.playerCombatController.StartCoroutine(EffectTools.DeactivateGameObject(gameObject, 0.2f));
		enabled = false;
		waiting = false;
	}
}
