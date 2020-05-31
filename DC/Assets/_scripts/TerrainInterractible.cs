using AbilityInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInterractible : MonoBehaviour, IAbilityInterractible
{
	public MonoBehaviour MyMono { get => this; }
	public StatBlock MyStats { get; set; }

	//public A aefv = new A();

	void Start()
	{
		MyStats = EncounterData.urnBlock.Clone();
		//aefv.sb = MyStats;
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

	/*
	[System.Serializable]
	public class A
	{
		public StatBlock sb;
	}
	*/
}
