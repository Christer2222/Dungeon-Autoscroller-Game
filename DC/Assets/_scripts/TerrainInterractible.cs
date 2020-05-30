using AbilityInfo;
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
		throw new System.NotImplementedException();
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
