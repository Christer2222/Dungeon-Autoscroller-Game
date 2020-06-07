using AbilityInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI
{
	public static Ability SelectAbility(StatBlock stats, float currentHealth, float currentMana)
	{
		List<Ability> _recoveries = stats.abilities.FindAll(x => (x.abilityType & AbilityType.recovery) != 0 && -x.manaCost <= currentMana);// (manaCostDictionary.TryGetValue(x, out int y)? y <= currentMana: true)); //find all recoveries. If it has cost, check if has more or equal mana. If no cost, act as if has mana.
		List<Ability> _nonRecover = stats.abilities.FindAll(x => x.abilityType != AbilityType.recovery && -x.manaCost <= currentMana);// (manaCostDictionary.TryGetValue(x, out int y) ? y <= currentMana : true));
		List<Ability> _offensive = stats.abilities.FindAll(x => (x.abilityType & AbilityType.offensive) != 0 && -x.manaCost <= currentMana);// (manaCostDictionary.TryGetValue(x, out int y) ? y <= currentMana : true));
		List<Ability> _buffs = stats.abilities.FindAll(x => (x.abilityType & AbilityType.buff) != 0 && -x.manaCost <= currentMana);// (manaCostDictionary.TryGetValue(x, out int y) ? y <= currentMana : true));

		Ability pickedAbility = null;
		//AbilityType activeType = AbilityType.none;

		switch (stats.aiType)
		{
			case StatBlock.AIType.Dumb:
				pickedAbility = stats.abilities[Random.Range(0, stats.abilities.Count)];
				//activeType = AbilityType.offensive;
				break;
			case StatBlock.AIType.Smart:
				if (currentHealth <= stats.maxHealth / 3 && _recoveries.Count > 0)
				{
					pickedAbility = _recoveries[Random.Range(0, _recoveries.Count)];
					//activeType = AbilityType.recovery;
				}
				else if (stats.buffList.Count == 0 && _buffs.Count > 0)
				{
					//pickedAbility = _buffs[Random.Range(0, _offensive.Count)];
					pickedAbility = _buffs[Random.Range(0, _buffs.Count)];

					//activeType = AbilityType.buff;
				}
				else if (_offensive.Count > 0)
				{
					//pickedAbility = _offensive[Random.Range(0, stats.abilities.Count)];
					pickedAbility = _offensive[Random.Range(0, _offensive.Count)];

					//activeType = AbilityType.offensive;
				}
				break;
			case StatBlock.AIType.Coward:

				if (currentHealth <= stats.maxHealth / 2 && _recoveries.Count > 0)
				{
					pickedAbility = _recoveries[Random.Range(0, _recoveries.Count)];
					//activeType = AbilityType.recovery;
				}
				else
				{
					pickedAbility = _nonRecover[Random.Range(0, _nonRecover.Count)];
					//activeType = AbilityType.offensive;
				}
				break;
			case StatBlock.AIType.Sprinter:
				break;
		}

		if (pickedAbility == null)
		{
			pickedAbility = stats.abilities[Random.Range(0, stats.abilities.Count)];
			//Debug.LogWarning($"{stats.name} did not have any offensive abilities.");
		}

		return pickedAbility;
	}

	public static IEnumerator SpawnAbilityTextUsed(Transform transform, Transform uiCanvasTransform, StatBlock myStats, Ability selectedAbility, MonoBehaviour holder)
	{
		var _startScale = transform.localScale;
		var _abilityUsedText = EffectTools.SpawnText(Vector3.zero, uiCanvasTransform, new Color(0.7f, 0, 0), myStats.name + " used " + selectedAbility.name, 90);

		_abilityUsedText.transform.parent.localPosition = Vector3.zero + Vector3.up * 400;
		Object.Destroy(_abilityUsedText.transform.parent.gameObject, 6);

		float _turnDelay = 1f / ((float)CombatController.turnOrder.Count / 5);
		holder.StartCoroutine(
		EffectTools.ActivateInOrder(_abilityUsedText, new List<EffectTools.FunctionGroup>()
		{
			new EffectTools.FunctionGroup(EffectTools.MoveDirection(_abilityUsedText.transform,Vector3.right,100,5),_turnDelay),
		}));

		yield return holder.StartCoroutine(
		EffectTools.ActivateInOrder(holder, new List<EffectTools.FunctionGroup>()
		{
			new EffectTools.FunctionGroup(EffectTools.StretchFromTo(transform, _startScale, _startScale * 1.5f, 0.2f), _turnDelay + 0.5f),
			new EffectTools.FunctionGroup(EffectTools.StretchFromTo(transform, transform.localScale, _startScale, 0.2f), 0.2f),
		}));
	}
}
