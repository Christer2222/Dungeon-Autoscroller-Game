using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityInfo;

public class AbilityScript : MonoBehaviour// : AbilityData
{
	#region Abilities
	public static Ability punch				= new Ability("Punch",					Punch, Elementals.Physical, SkillUsed.heavy_hits, AbilityType.attack, 0);
	public static Ability fireball			= new Ability("Fireball",				Fireball, Elementals.Fire, SkillUsed.magic, AbilityType.attack, -2);
	public static Ability massExplosion		= new Ability("Mass Explosion",			MassExplosion, Elementals.Fire, SkillUsed.magic, AbilityType.attack, -4);
	public static Ability smiteUnlife		= new Ability("Smite Undead",			Smite, Elementals.None, SkillUsed.healing, AbilityType.attack, -1);
	public static Ability doubleKick		= new Ability("Double Kick",			DoubleKick, Elementals.Physical, SkillUsed.heavy_hits, AbilityType.attack, 0);
	public static Ability wildPunch			= new Ability("Wild Punch",				WildPunch, Elementals.Physical, SkillUsed.heavy_hits, AbilityType.attack, 0);
	public static Ability tiltSwing			= new Ability("Tilt Swing",				TiltSwing, Elementals.Physical, SkillUsed.heavy_hits, AbilityType.attack, 0);
	public static Ability forcePunch		= new Ability("Force Punch",			ForcePunch, Elementals.Air, SkillUsed.heavy_hits | SkillUsed.magic, AbilityType.attack, -1);
	public static Ability chaosThesis		= new Ability("Chaos Thesis",			ChaosThesis, Elementals.Void, SkillUsed.magic | SkillUsed.healing, AbilityType.attack, -1);
	public static Ability manaDrain			= new Ability("Mana Drain",				ManaDrain, Elementals.Water, SkillUsed.healing, AbilityType.attack, -3);
	public static Ability meteorShower		= new Ability("Meteor Shower",			MeteorShower, Elementals.Fire | Elementals.Earth, SkillUsed.magic, AbilityType.attack, -7);

	public static Ability siphonSoul		= new Ability("Siphon Soul",			SiphonSoul, Elementals.Unlife, SkillUsed.healing, AbilityType.attack | AbilityType.recovery, -1);

	public static Ability focus				= new Ability("Focus",					Focus, Elementals.Water, SkillUsed.magic, AbilityType.recovery, 0);
	public static Ability heal				= new Ability("Heal",					Heal, Elementals.Light, SkillUsed.healing, AbilityType.recovery, -2);
	public static Ability regeneration		= new Ability("Regeneration",			Regeneration, Elementals.Light, SkillUsed.healing, AbilityType.recovery, -1);
	public static Ability massHeal			= new Ability("Mass Heal",				MassHeal, Elementals.Light, SkillUsed.healing, AbilityType.recovery, -5);
	public static Ability restoreSoul		= new Ability("Restore Soul",			RestoreSoul, Elementals.Water, SkillUsed.healing, AbilityType.recovery, 0);
	public static Ability clense			= new Ability("Clense",					Clense, Elementals.Water, SkillUsed.healing, AbilityType.recovery, 0);

	public static Ability timeWarp			= new Ability("Time Warp",				TimeWarp, Elementals.Void, SkillUsed.magic, AbilityType.buff, -10);
	public static Ability divineLuck		= new Ability("Divine Luck",			DivineLuck, Elementals.Light, SkillUsed.healing | SkillUsed.heavy_hits, AbilityType.buff, -3);
	public static Ability bulkUp			= new Ability("Bulk Up",				BulkUp, Elementals.Physical, SkillUsed.heavy_hits, AbilityType.buff, -1);
	public static Ability divineFists		= new Ability("Divine Fists",			DivineFists, Elementals.Physical | Elementals.Light, SkillUsed.healing, AbilityType.buff, -6);
	public static Ability bless				= new Ability("Bless",					Bless, Elementals.Fire, SkillUsed.healing, AbilityType.buff, -10);

	public static Ability debulk			= new Ability("Debulk",					Debulk, Elementals.Unlife, SkillUsed.healing, AbilityType.debuff, -2);
	public static Ability curse				= new Ability("Curse",					Curse, Elementals.Unlife, SkillUsed.healing, AbilityType.debuff, -5);

	public static Ability eat				= new Ability("Eat",					Eat, Elementals.Physical, default, AbilityType.misc, 0);
	public static Ability keenSight			= new Ability("Keen Sight",				DisplayCritAreas, Elementals.Physical, SkillUsed.light_hits, AbilityType.misc, -1);
	public static Ability displayCritAreas	= new Ability("Display Crit Areas",		DisplayCritAreas, Elementals.Physical, SkillUsed.light_hits, AbilityType.misc, -1);
	public static Ability spotWeakness		= new Ability("Spot Weakness",			SpotWeakness, Elementals.Physical, SkillUsed.light_hits, AbilityType.misc, -1);
	public static Ability lifeTap			= new Ability("Life Tap",				LifeTap, Elementals.Unlife, SkillUsed.healing, AbilityType.misc, 0);
	public static Ability syncSoul			= new Ability("Sync Soul",				SyncSoul, Elementals.Void, SkillUsed.healing | SkillUsed.magic, AbilityType.misc, -10);
	#endregion

	protected static Vector3 lastClick;

	protected static Camera mainCamera;

	protected static Vector3 randomVector3
	{
		get
		{
			return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		}
	}


	protected void AddBuff(Buff _buff, CombatController _target)
	{
		if (_target == null) return;

		var _same = _target.myStats.buffList.Find(x => x.name == _buff.name);
		switch(_buff.stackType)
		{
			case StatBlock.StackType.Pick_Most_Potent:
				if (_same != null)
				{
					if(_same.constant < _buff.constant || (_same.constant == _buff.constant && _same.turns < _buff.turns))
					{
						_target.myStats.buffList.Remove(_same);
						_target.myStats.buffList.Add(_buff);
					}
				}
				else
					_target.myStats.buffList.Add(_buff);

				break;
			case StatBlock.StackType.Pick_Most_Turns:
				if(_same != null)
				{
					if(_same.turns < _buff.turns || (_same.turns == _buff.turns && _same.constant < _buff.constant))
					{
						_target.myStats.buffList.Remove(_same);
						_target.myStats.buffList.Add(_buff);
					}
				}
				else
					_target.myStats.buffList.Add(_buff);
				break;
			case StatBlock.StackType.Stack_Self:
				_target.myStats.buffList.Add(_buff);
				break;
			case StatBlock.StackType.Build_Up:
				if(_same != null)
				{
					_same.turns++;
					_same.constant += _buff.constant;
				}
				else
					_target.myStats.buffList.Add(_buff);
				break;
			default:
				Debug.LogError("ERROR when adding buff: " + _buff.name + " to: " + _target.transform.name);
				break;

		}
	}


	protected static RaycastHit2D CheckIfHit(Vector3 _clickPos)
	{
		Debug.DrawLine(_clickPos,_clickPos + Vector3.up * 0.1f,Color.red,4,false);
		Debug.DrawLine(_clickPos,_clickPos + Vector3.right * 0.1f,Color.blue,4,false);

		lastClick = _clickPos;

		RaycastHit2D _hit = Physics2D.Raycast(_clickPos,Vector2.zero,0.01f);
		return _hit;
	}


	static CombatController CheckForMultiHit(RaycastHit2D _hit)
	{
		CombatController _cc = null;
		if (_hit.transform != null)
		{
			_cc = _hit.transform.GetComponentInParent<CombatController>();
		}

		return _cc;
	}


	static IEnumerator CircleCollision(Transform _objectToCheck, float _frequency, System.Action<CombatController> _actionOnHit)
	{
		List<object[]> _justHit = new List<object[]>();

		while (true)
		{
			foreach (var _entry in _justHit)
			{
				print(_entry.Length);
				_entry[1] = (float)_entry[1] - Time.deltaTime;
				if ((float)_entry[1] <= 0)
				{
					_justHit.Remove(_entry);
				}
			}

			foreach (Collider2D _col in Physics2D.OverlapCircleAll(_objectToCheck.position, 1f)) //find all cols
			{
				bool _found = false;
				var _cc = _col.GetComponent<CombatController>();
				if (_cc != null) //if col has combat
				{
					for (int j = 0; j < _justHit.Count; j++) //go through all just hit
					{
						if ((CombatController)_justHit[j][0] == _cc) //if this is in just hit
						{
							_found = true;
							break;
						}
					}

					if (!_found)
					{
						_justHit.Add(new object[] { _cc, 1f });
						_actionOnHit(_cc);
					}
				}

			}

			yield return new WaitForSeconds(_frequency);
		}
	}


	protected static IEnumerator DivineFists(TargetData targetData)
	{
		yield return targetData.self.StartCoroutine(divineLuck.function(targetData));// DivineLuck(targetData._target));
		yield return targetData.self.StartCoroutine(bulkUp.function(targetData));//BulkUp(targetData._target));
	}

	protected static IEnumerator Punch(TargetData targetData)//CombatController _target, int _damage, Elementals _element = Elementals.Physical)
	{
		EffectTools.SpawnEffect(punch.name, lastClick, 1);
		if (targetData.target != null) targetData.target.AdjustHealth(-Mathf.Max(targetData.self.myStats.strength + targetData.bonus, 0), targetData.element);
		yield return null;
	}

	protected static IEnumerator DoubleKick(TargetData targetData)//(Vector3 _centerPos, CombatController _target, CombatController _self)
	{
		yield return targetData.self.StartCoroutine(punch.function(targetData)); //StartCoroutine(Punch(_target,_self.myStats.strength));
		targetData.target = null;
		yield return new WaitForSeconds(2);
		var _hit = CheckIfHit(targetData.centerPos);

		/*
		if (_hit.transform != null)
		{
			targetData.target = _hit.transform.GetComponent<CombatController>();
		}
		*/
		targetData.target = CheckForMultiHit(_hit);

		yield return targetData.self.StartCoroutine(punch.function(targetData)); //StartCoroutine(Punch(_target,_self.myStats.strength));


		//yield return StartCoroutine(Punch(_target, _self.myStats.strength));
	}


	protected static IEnumerator WildPunch(TargetData targetData)// (Vector3 _centerPos, CombatController _self)
	{
		targetData.target = null;
		var _hit = CheckIfHit(targetData.centerPos + randomVector3);
		/*
		if (_hit.transform != null)
		{
			targetData.target = _hit.transform.GetComponent<CombatController>();
		}
		*/
		targetData.target = CheckForMultiHit(_hit);
		yield return targetData.self.StartCoroutine(punch.function(targetData));// (Punch(_target, targetData._self.myStats.strength + 2));
		yield return null;
	}

	protected static IEnumerator ForcePunch(TargetData targetData)//(CombatController _target, CombatController _self)
	{
		yield return targetData.self.StartCoroutine(punch.function(targetData));// (Punch(_target, _self.myStats.strength + 2, Elementals.Air));
		yield return null;
	}

	protected static IEnumerator TiltSwing(TargetData targetData)//(CombatController _target, CombatController _self)
	{
		float _randomNumber = Random.Range(0, 2);
		int _result = (int)((_randomNumber-0.5f) * 2);

		targetData.bonus += _result;

		yield return targetData.self.StartCoroutine(punch.function(targetData));//Punch(_target, _self.myStats.strength + _r));
		yield return null;
	}

	protected static IEnumerator ChaosThesis(TargetData targetData)//(Vector3 _centerPos, CombatController _self)
	{
		int _r = Random.Range(0,3);

		if (_r == 0)
		{
			targetData.bonus += 2;
			yield return targetData.self.StartCoroutine(manaDrain.function(targetData));// ManaDrain(_target,_self, 2));
		}
		else if (_r == 1)
		{
			yield return targetData.self.StartCoroutine(fireball.function(targetData));//Fireball(_centerPos, _self, 2));
		}
		else if (_r == 2)
		{
			yield return targetData.self.StartCoroutine(heal.function(targetData));// Heal(_target, _self.myStats.luck));
		}

		yield return null;
	}

	protected static IEnumerator LifeTap(TargetData targetData)
	{
        EffectTools.SpawnEffect("blue blast", targetData.self.transform.position, 1);
		int _tapped = targetData.self.AdjustHealth(-Mathf.Min(5, targetData.self.currentHealth -1),Elementals.Void);
		targetData.self.AdjustMana(_tapped);
		yield return null;
	}

	protected static IEnumerator SiphonSoul(TargetData targetData)
	{
		if(targetData.target != null)
		{
			int _hpRecover = targetData.target.AdjustHealth(-Mathf.Max(targetData.self.myStats.luck,0),Elementals.Unlife);

			targetData.self.AdjustHealth(Mathf.Max(_hpRecover,0), targetData.element);
		}
		yield return null;
	}

	protected static IEnumerator Smite(TargetData targetData)
	{
		EffectTools.SpawnEffect("light cloud",lastClick,1);

		if (targetData.target != null)
		{

			//_self.AdjustMana(-manaCostDictionary["smite unlife"]);
			//EffectTools.SpawnEffect("punch",lastClick,1);
			int _smiteDamage = (targetData.target.myStats.race.HasFlag(targetData.targetRace)) ? 2 : 0;
			targetData.bonus += _smiteDamage;
			if (_smiteDamage > 0)
				targetData.element = Elementals.None;

			yield return targetData.self.StartCoroutine(Punch(targetData));
			//targetData.target.AdjustHealth(-Mathf.Max(targetData.self.myStats.luck + targetData.bonus + _smiteDamage, 0),Elementals.None);
		}
		yield return null;
	}

	protected static IEnumerator MeteorShower(TargetData targetData)
	{
		Vector3 _top = new Vector3(0, 0, targetData.centerPos.z) + Vector3.up * 6;
		List<Transform> _meteors = new List<Transform>();
		List<Coroutine> _moves = new List<Coroutine>();
		List<Coroutine> _collisionChecks = new List<Coroutine>();


		//Spawn meteors
		int _totalBalls = 4 + (targetData.self.myStats.intelligence/2);
		for (int i = 0; i < _totalBalls; i++)
		{
			Vector3 _randomDir = Vector3.right * Random.Range(-0.7f,0.7f);

			var _meteor = EffectTools.SpawnEffect((Random.Range(0,128) == 0)? punch.name :  fireball.name, _top + Vector3.left * (i - Random.Range(1f,1.5f)), 6);
			_moves.Add(CombatController.playerCombatController.StartCoroutine(EffectTools.MoveDirection(_meteor.transform,Vector3.down + _randomDir,3,5))); //global set the effect
			_meteors.Add(_meteor.transform);
			yield return new WaitForSeconds(Random.Range(0.3f,0.6f));
		}

		//make them do collision checks
		for (int i = 0; i < _meteors.Count; i++)
		{
			_collisionChecks.Add(
				CombatController.playerCombatController.StartCoroutine(CircleCollision(_meteors[i], 0.25f,
					delegate (CombatController _cc)
					{
						_cc.AdjustHealth(-Mathf.Max(targetData.self.myStats.intelligence, 0), targetData.element);
					}
				))
			);
		}

		//destroy them if they get too far off screen
		while (_meteors.Count > 0)
		{
			for (int i = 0; i < _meteors.Count; i++)
			{
				if (_meteors[i].position.y <= -1)
				{
					CombatController.playerCombatController.StopCoroutine(_moves[i]);
					CombatController.playerCombatController.StopCoroutine(_collisionChecks[i]);
					_moves.RemoveAt(i);
					_collisionChecks.RemoveAt(i);
					Destroy(_meteors[i].gameObject);
					_meteors.RemoveAt(i);
					i--;
					break;
				}
			}

			yield return new WaitForSeconds(0.2f);
		}

		yield return null;
	}

	protected static IEnumerator Fireball(TargetData targetData)
	{
		var _sprite = EffectTools.SpawnEffect(fireball.name,targetData.centerPos,0.6f);
		float _blastDiameter = 1;
		targetData.self.StartCoroutine(EffectTools.PingPongSize(_sprite.transform,Vector3.zero,Vector3.one * _blastDiameter,0.5f,1));

		Debug.DrawLine(_sprite.transform.position,_sprite.transform.position + Vector3.right * _blastDiameter * 0.5f,Color.white,5,false);
		Debug.DrawLine(_sprite.transform.position,_sprite.transform.position + Vector3.left * _blastDiameter * 0.5f,Color.white,5,false);
		Debug.DrawLine(_sprite.transform.position,_sprite.transform.position + Vector3.up * _blastDiameter * 0.5f,Color.white,5,false);
		Debug.DrawLine(_sprite.transform.position,_sprite.transform.position + Vector3.down * _blastDiameter * 0.5f,Color.white,5,false);

		foreach(Collider2D _col in Physics2D.OverlapCircleAll(targetData.centerPos,_blastDiameter * 0.5f))
		{
			var _cc = _col.GetComponent<CombatController>();
			if (_cc != null)
			{
				_cc.AdjustHealth(-Mathf.Max(targetData.self.myStats.intelligence,0), targetData.element);
			}
		}

		yield return null;
	}

	protected static IEnumerator MassExplosion(TargetData targetData)//(Vector3 _centerPos,CombatController _self)
	{
		targetData.centerPos += Vector3.left; //-1
		targetData.self.StartCoroutine(fireball.function(targetData));
		yield return new WaitForSeconds(0.1f);

		targetData.centerPos += Vector3.right; //0
		targetData.self.StartCoroutine(fireball.function(targetData));
		yield return new WaitForSeconds(0.1f);

		targetData.centerPos += Vector3.right; //+1
		targetData.self.StartCoroutine(fireball.function(targetData));
		yield return null;
	}

	protected static IEnumerator Heal(TargetData targetData)
	{
		if(targetData.target != null)
		{
			targetData.target.AdjustHealth(Mathf.CeilToInt(Mathf.Max(targetData.self.myStats.luck,0)),Elementals.Light);
		}

		if (lastClick != null)
			EffectTools.SpawnEffect("heal_circle",lastClick, 1);

		yield return null;
	}

	protected static IEnumerator ManaDrain(TargetData targetData)
	{
		if(targetData.target != null)
		{
			int _manaRecover = 0;

			_manaRecover = targetData.target.AdjustMana(-Mathf.Max((Mathf.CeilToInt((float)targetData.self.myStats.luck/2) + targetData.bonus),0));

			targetData.self.AdjustMana(Mathf.Max(_manaRecover,0));

			yield return null;
		}

		yield return null;
	}

	protected static IEnumerator RestoreSoul(TargetData targetData)//(CombatController _self)
	{
		targetData.self.myStats.buffList.Clear();
		yield return null;
	}

	protected static IEnumerator Clense(TargetData targetData)//(CombatController _self)
	{
		if (targetData.self.myStats.buffList.Count > 0)
		{
			targetData.self.myStats.buffList.RemoveAt(Random.Range(0, targetData.self.myStats.buffList.Count));
		}
		yield return null;
	}

	protected static IEnumerator SyncSoul(TargetData targetData)//(CombatController _target, CombatController _self)
	{
		if (targetData.target != null)
		{
			targetData.self.myStats.buffList.Clear();
			targetData.self.myStats.buffList.AddRange(targetData.target.myStats.buffList);
		}
		yield return null;
	}

	protected static IEnumerator Bless(TargetData targetData)//(CombatController _target)
	{
		EffectTools.SpawnEffect("bless", lastClick, 1);

		var _buff = new Buff(bless.name, new List<string> { "strenght_mutliplier", "dexterity_multiplier", "intelligence_multiplier", "luck_multiplier" }, 3, AbilityIcons.TryGetBuffIcon("bless"), StatBlock.StackType.Pick_Most_Turns, 2);
		targetData.self.AddBuff(_buff, targetData.target);

		yield return null;
	}

	protected static IEnumerator Curse(TargetData targetData)//(CombatController _target)
	{
		EffectTools.SpawnEffect("bless", lastClick, 1);

		var _buff = new Buff(curse.name, new List<string> { "strenght_mutliplier", "dexterity_multiplier", "intelligence_multiplier", "luck_multiplier" }, 3, AbilityIcons.TryGetBuffIcon("curse"), StatBlock.StackType.Pick_Most_Turns, 0.5f);
		targetData.self.AddBuff(_buff, targetData.target);

		yield return null;
	}

	protected static IEnumerator BulkUp(TargetData targetData)//(CombatController _self)
	{
		EffectTools.SpawnEffect("fist up",lastClick,1);

		var _buff = new Buff(bulkUp.name,"strength_constant",2, AbilityIcons.TryGetBuffIcon("pluss_strength"), StatBlock.StackType.Build_Up,1);
		targetData.self.AddBuff(_buff, targetData.self);

		yield return null;
	}

	protected static IEnumerator Debulk(TargetData targetData)
	{
		if (targetData.target != null)
		{
			var _buff = new Buff(debulk.name, "strength_constant", 3, AbilityIcons.TryGetBuffIcon("pluss_strength"), StatBlock.StackType.Pick_Most_Potent, -2);
			targetData.self.AddBuff(_buff, targetData.target);
		}

		yield return null;
	}

	protected static IEnumerator DivineLuck(TargetData targetData)
	{
		EffectTools.SpawnEffect("luck up",lastClick,1);

		var _buff = new Buff(divineLuck.name, "luck_constant", 3, AbilityIcons.TryGetBuffIcon("divine_luck"), StatBlock.StackType.Pick_Most_Potent,2);
		targetData.self.AddBuff(_buff, targetData.self);
		yield return null;
	}

	protected static IEnumerator Regeneration(TargetData targetData)
	{
		EffectTools.SpawnEffect("plusses",lastClick + Vector3.forward,1);

		if(targetData.target != null)
		{
			var _buff = new Buff(regeneration.name,heal.name,3, AbilityIcons.TryGetBuffIcon("yellow_pluss"), StatBlock.StackType.Pick_Most_Potent, Mathf.Max(targetData.self.myStats.luck,0));
			targetData.self.AddBuff(_buff,targetData.target);
		}
		yield return null;
	}

	protected static IEnumerator MassHeal(TargetData targetData)
	{
		CombatController.turnOrder.ForEach(x => {
			lastClick = x.transform.position;
			x.StartCoroutine(heal.function(targetData));
		});

		yield return null;
	}

	protected static IEnumerator Eat(TargetData targetData)
	{
		targetData.target.AdjustHealth(targetData.bonus, Elementals.Light);

		yield return null;
	}

	protected static IEnumerator Focus(TargetData targetData)
	{
		EffectTools.SpawnEffect("blue_sparkles", targetData.self.transform.position,1);
		targetData.self.AdjustMana(Mathf.Max(targetData.self.myStats.intelligence, 0));
		yield return null;
	}

	protected static IEnumerator SpotWeakness(TargetData targetData)
	{
		if (targetData.target != null)
		{
			targetData.target.StartCoroutine(displayCritAreas.function(targetData));
		}

		yield return null;
	}

	protected static IEnumerator TimeWarp(TargetData targetData)
	{
		EffectTools.SpawnEffect(timeWarp.name,lastClick,1);
		var _buff = new Buff(timeWarp.name,"extra turn",2, AbilityIcons.TryGetBuffIcon("pluss_time"),StatBlock.StackType.Stack_Self,1);
		targetData.self.AddBuff(_buff, targetData.self);
		yield return null;
	}

	/// <summary>
	/// Displays crit areas of all enemies if no parameter is given, otherwise, display the crit area of the enemy given.
	/// </summary>
	/// <param name="_self"></param>
	protected static IEnumerator DisplayCritAreas(TargetData targetData)//(CombatController _self,CombatController _target = null)
	{
		/*
		if(_manacost == 0)
			_self.AdjustMana(-manaCostDictionary["keen sight"]);
		else
			_self.AdjustMana(_manacost);
		*/

		var _checks = (targetData.target == null)? CombatController.turnOrder: new List<CombatController>() { targetData.target };

		foreach (CombatController _cc in _checks)
		{
			var _critArea = _cc.transform.Find("$CritArea");
			print("area: " + _critArea);
			if (_critArea != null)
			{
				var _critImage = _critArea.GetComponent<SpriteRenderer>();
				if (_critImage != null)
					targetData.target.StartCoroutine(EffectTools.BlinkImage(_critImage,Color.white,5.5f,10));
			}
		}

		yield return null;

	}

	protected static IEnumerator Spook(TargetData targetData)//(CombatController _target,CombatController _self)
	{
		yield return targetData.self.StartCoroutine(EffectTools.BlinkImage(targetData.self.transform.GetComponent<SpriteRenderer>(),new Color(1,1,1,0),1,1));
		targetData.target.AdjustHealth(-targetData.self.myStats.intelligence, Elementals.Unlife);

	}
}
