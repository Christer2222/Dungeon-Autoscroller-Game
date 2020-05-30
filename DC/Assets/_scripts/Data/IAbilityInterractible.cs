using AbilityInfo;
using UnityEngine;

public interface IAbilityInterractible
{
	MonoBehaviour MyMono { get; }
	StatBlock MyStats { get; }

	int AdjustHealth(int _amount, Elementals _elementals, ExtraData _extraData);
	int AdjustMana(int _amount);
}
