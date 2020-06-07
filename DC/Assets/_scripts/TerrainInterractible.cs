using AbilityInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInterractible : MonoBehaviour, IAbilityInterractible
{
	public MonoBehaviour MyMono { get => this; }
	public StatBlock MyStats { get; set; }

	public delegate IEnumerator OnTrapTrigger();
	public OnTrapTrigger trapTriggered;

	public void SetStatBlock(StatBlock statBlock)
	{
		MyStats = statBlock;
	}

	public int AdjustHealth(int _amount, Elementals _elementals, ExtraData _extraData)
	{
		print(transform.name + " was hit by: " + _amount + " damage");
		MyStats.currentHealth = Mathf.Min(MyStats.currentHealth + _amount, MyStats.maxHealth);

		if (MyStats.currentHealth <= 0)
		{
			PlayerInventory.instance.ProcessDrops(MyStats.drops);

			Destroy(gameObject);
		}

		return 0;
		//throw new System.NotImplementedException();
	}

	public int AdjustMana(int _amount)
	{
		throw new System.NotImplementedException();
	}

	public IEnumerator PrintName()
	{
		print(MyStats.name);
		yield return null;
	}

	public IEnumerator StalagmiteTrap()
	{
		int _timeToMoveToPlayer = 1;

		print(MyStats.name);


		//yield return StartCoroutine(EffectTools.MoveToPoint(transform, CombatController.playerCombatController.transform.position, _timeToMoveToPlayer));
		yield return StartCoroutine(EffectTools.MoveToPoint(transform, transform.position + Vector3.down  * 20, _timeToMoveToPlayer));


		//should be doing a circlecast2d check or something to figure out closest target, while moving downwards. That way tactics can be made to break environment for damage.
		CombatController.playerCombatController.AdjustHealth(-2, Elementals.Earth, ExtraData.nonPiercing | ExtraData.makes_contact_with_user);

		print(2 + " " + MyStats.name);

		/*
		StartCoroutine(
			EffectTools.ActivateInOrder(this, new List<EffectTools.FunctionGroup>()
			{
				new EffectTools.FunctionGroup(EffectTools.MoveToPoint(transform, CombatController.playerCombatController.transform.position, _timeToMoveToPlayer),0),
				new EffectTools.FunctionGroup(EffectTools.Delay(0),1),
				CombatController.playerCombatController.AdjustHealth(-2,Elementals.Earth,ExtraData.nonPiercing | ExtraData.makes_contact_with_user),
			}
			));

		*/
		yield return null;
	}

	/*
	[System.Serializable]
	public class A
	{
		public StatBlock sb;
	}
	*/
}
