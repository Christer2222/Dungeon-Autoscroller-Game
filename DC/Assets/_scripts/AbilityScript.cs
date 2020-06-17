using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityInfo;
using System.Runtime.InteropServices;

public class AbilityScript : MonoBehaviour// : AbilityData
{
	/*
	#region Abilities
	protected static readonly Ability punch				= new Ability("Punch",					Punch,				Elementals.Physical, SkillUsed.heavy_hits, AbilityType.attack, 0, ExtraData.blockable | ExtraData.makes_contact_with_user);
	protected static readonly Ability fireball			= new Ability("Fireball",				Fireball,			Elementals.Fire, SkillUsed.magic, AbilityType.attack, -2, ExtraData.blockable | ExtraData.magic);
	protected static readonly Ability massExplosion		= new Ability("Mass Explosion",			MassExplosion,		Elementals.Fire, SkillUsed.magic, AbilityType.attack, -4, ExtraData.blockable);
	protected static readonly Ability smiteUnlife		= new Ability("Smite Undead",			Smite,				Elementals.None, SkillUsed.healing, AbilityType.attack, -1, ExtraData.blockable);
	protected static readonly Ability doubleKick		= new Ability("Double Kick",			DoubleKick,			Elementals.Physical, SkillUsed.heavy_hits, AbilityType.attack, 0, ExtraData.blockable | ExtraData.makes_contact_with_user);
	protected static readonly Ability wildPunch			= new Ability("Wild Punch",				WildPunch,			Elementals.Physical, SkillUsed.heavy_hits, AbilityType.attack, 0, ExtraData.blockable | ExtraData.makes_contact_with_user);
	protected static readonly Ability tiltSwing			= new Ability("Tilt Swing",				TiltSwing,			Elementals.Physical, SkillUsed.heavy_hits, AbilityType.attack, 0, ExtraData.blockable | ExtraData.makes_contact_with_user);
	protected static readonly Ability forcePunch		= new Ability("Force Punch",			ForcePunch,			Elementals.Air, SkillUsed.heavy_hits | SkillUsed.magic, AbilityType.attack, -1, ExtraData.blockable | ExtraData.makes_contact_with_user);
	protected static readonly Ability chaosThesis		= new Ability("Chaos Thesis",			ChaosThesis,		Elementals.Void, SkillUsed.magic | SkillUsed.healing, AbilityType.attack, -1, ExtraData.none);
	protected static readonly Ability manaDrain			= new Ability("Mana Drain",				ManaDrain,			Elementals.Water, SkillUsed.healing, AbilityType.attack, -3, ExtraData.magic);
	protected static readonly Ability meteorShower		= new Ability("Meteor Shower",			MeteorShower,		Elementals.Fire | Elementals.Earth, SkillUsed.magic, AbilityType.attack, -7, ExtraData.blockable | ExtraData.magic);
	protected static readonly Ability freezingStrike	= new Ability("Freezing Strike",		FreezingStrike,		Elementals.Ice, SkillUsed.magic, AbilityType.attack, -1, ExtraData.blockable | ExtraData.magic | ExtraData.makes_contact_with_user);
	protected static readonly Ability thunderbolt		= new Ability("Thunderbolt",			Thunderbolt,		Elementals.Electricity, SkillUsed.magic, AbilityType.attack, -4, ExtraData.blockable | ExtraData.magic);
	protected static readonly Ability eruption			= new Ability("Eruption",				Eruption,			Elementals.Fire, SkillUsed.magic, AbilityType.attack, -5, ExtraData.blockable | ExtraData.magic);
	protected static readonly Ability airSlash			= new Ability("Air Slash",				AirSlash,			Elementals.Air, SkillUsed.magic, AbilityType.attack, -3, ExtraData.blockable | ExtraData.magic);
	protected static readonly Ability bubble			= new Ability("Bubble",					Bubble,				Elementals.Water, SkillUsed.magic, AbilityType.attack, -2, ExtraData.blockable | ExtraData.magic);
	protected static readonly Ability poision			= new Ability("Poision",				Poision,			Elementals.Poision, SkillUsed.light_hits | SkillUsed.magic, AbilityType.attack, -2, ExtraData.none);
	protected static readonly Ability poisionBite		= new Ability("Poision Bite",			PoisionBite,		Elementals.Physical | Elementals.Poision, SkillUsed.heavy_hits | SkillUsed.light_hits, AbilityType.attack, 0, ExtraData.blockable | ExtraData.makes_contact_with_user);
	
	protected static readonly Ability siphonSoul		= new Ability("Siphon Soul",			SiphonSoul,			Elementals.Unlife, SkillUsed.healing, AbilityType.attack | AbilityType.recovery, -1, ExtraData.magic);

	protected static readonly Ability crystalLance		= new Ability("Crystal Lance",			CrystalLance,		Elementals.Earth | Elementals.Air, SkillUsed.heavy_hits, AbilityType.attack | AbilityType.defensive, -2, ExtraData.blockable | ExtraData.magic);

	protected static readonly Ability focus				= new Ability("Focus",					Focus,				Elementals.Water, SkillUsed.magic, AbilityType.recovery, 0, ExtraData.magic);
	protected static readonly Ability heal				= new Ability("Heal",					Heal,				Elementals.Light, SkillUsed.healing, AbilityType.recovery, -2, ExtraData.blockable | ExtraData.magic);
	protected static readonly Ability regeneration		= new Ability("Regeneration",			Regeneration,		Elementals.Light, SkillUsed.healing, AbilityType.recovery, -1, ExtraData.blockable | ExtraData.magic);
	protected static readonly Ability massHeal			= new Ability("Mass Heal",				MassHeal,			Elementals.Light, SkillUsed.healing, AbilityType.recovery, -5, ExtraData.blockable | ExtraData.magic);
	protected static readonly Ability restoreSoul 		= new Ability("Restore Soul",			RestoreSoul,		Elementals.Water, SkillUsed.healing, AbilityType.recovery, 0, ExtraData.magic);
	protected static readonly Ability clense			= new Ability("Clense",					Clense,				Elementals.Water, SkillUsed.healing, AbilityType.recovery, 0, ExtraData.magic);

	protected static readonly Ability timeWarp			= new Ability("Time Warp",				TimeWarp,			Elementals.Void, SkillUsed.magic, AbilityType.buff, -10, ExtraData.magic);
	protected static readonly Ability divineLuck		= new Ability("Divine Luck",			DivineLuck,			Elementals.Light, SkillUsed.healing | SkillUsed.heavy_hits, AbilityType.buff, -3, ExtraData.magic);
	protected static readonly Ability bulkUp			= new Ability("Bulk Up",				BulkUp,				Elementals.Physical, SkillUsed.heavy_hits, AbilityType.buff, -1, ExtraData.none);
	protected static readonly Ability divineFists		= new Ability("Divine Fists",			DivineFists,		Elementals.Physical | Elementals.Light, SkillUsed.healing, AbilityType.buff, -6, ExtraData.magic);
	protected static readonly Ability bless				= new Ability("Bless",					Bless,				Elementals.Fire, SkillUsed.healing, AbilityType.buff, -10, ExtraData.magic);
	
	protected static readonly Ability debulk			= new Ability("Debulk",					Debulk,				Elementals.Unlife, SkillUsed.healing, AbilityType.debuff, -2, ExtraData.magic);
	protected static readonly Ability curse				= new Ability("Curse",					Curse,				Elementals.Unlife, SkillUsed.healing, AbilityType.debuff, -5, ExtraData.magic);
	
	protected static readonly Ability hardenSkin		= new Ability("Harden Skin",			Harden,				Elementals.Physical, SkillUsed.heavy_hits, AbilityType.defensive, -3, ExtraData.none);
	protected static readonly Ability magicShield		= new Ability("Magic Shield",			MagicShield,		Elementals.Water, SkillUsed.magic, AbilityType.defensive, -3, ExtraData.none);

	//public static Ability eat							= new Ability("Eat",					Eat, default, default, AbilityType.misc, 0);
	protected static readonly Ability keenSight			= new Ability("Keen Sight",				DisplayCritAreas,	Elementals.Physical, SkillUsed.light_hits, AbilityType.misc, -1, ExtraData.none);
	protected static readonly Ability spotWeakness		= new Ability("Spot Weakness",			SpotWeakness,		Elementals.Physical, SkillUsed.light_hits, AbilityType.misc, -1, ExtraData.none);
	protected static readonly Ability lifeTap			= new Ability("Life Tap",				LifeTap,			Elementals.Unlife, SkillUsed.healing, AbilityType.misc, 0, ExtraData.magic);
	protected static readonly Ability syncSoul			= new Ability("Sync Soul",				SyncSoul,			Elementals.Void, SkillUsed.healing | SkillUsed.magic, AbilityType.misc, -10, ExtraData.magic);

	protected static readonly Ability wobble			= new Ability("Wobble",					Wobble,				Elementals.None, SkillUsed.none, AbilityType.none, 0, ExtraData.none);
	private static readonly Ability poisionTick			= new Ability("Poision Tick",			PoisionTick,		Elementals.Poision, SkillUsed.none, AbilityType.none, 0, ExtraData.none);
	#endregion
	*/
	protected static Vector3 lastClick;

	private const float CURSOR_HIT_DISTANCE = 90;

	public static Vector3 HitPosition
	{
		get 
		{
			Vector3 outPos = Vector3.zero;
			RectTransformUtility.ScreenPointToWorldPointInRectangle(UIController.UICanvas, Input.mousePosition, UIController.MainCamera, out outPos);
			//print(outPos);
			//return mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, EncounterController.ENEMY_SPAWN_DISTANCE)); 
			return outPos;// mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, EncounterController.ENEMY_SPAWN_DISTANCE));
		} //store where to click
	}

	protected static Camera mainCamera;

	protected static Vector3 RandomVector3
	{
		get
		{
			return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		}
	}

	protected static Vector3 TopPosition
	{
		get
		{
			return new Vector3(0, 6, mainCamera.transform.position.z + EncounterController.ENEMY_SPAWN_DISTANCE);
		}
	}

	/// <summary>
	/// Adds a buff to the target (checks if target is null)
	/// </summary>
	public static void AddBuff(Buff _buff, StatBlock _target)
	{
		if (_target == null) return;

		var _same = _target.buffList.Find(x => x.name == _buff.name);
		switch(_buff.stackType)
		{
			case Buff.StackType.Pick_Most_Potent:
				if (_same != null)
				{
					if(_same.constant < _buff.constant || (_same.constant == _buff.constant && _same.turns < _buff.turns))
					{
						_target.buffList.Remove(_same);
						_target.buffList.Add(_buff);
					}
				}
				else
					_target.buffList.Add(_buff);

				break;
			case Buff.StackType.Pick_Most_Turns:
				if(_same != null)
				{
					if(_same.turns < _buff.turns || (_same.turns == _buff.turns && _same.constant < _buff.constant))
					{
						_target.buffList.Remove(_same);
						_target.buffList.Add(_buff);
					}
				}
				else
					_target.buffList.Add(_buff);
				break;
			case Buff.StackType.Add_Duplicate:
				_target.buffList.Add(_buff);
				break;
			case Buff.StackType.Add_One_Duration_Add_All_Potency:
				if(_same != null)
				{
					_same.turns++;
					_same.constant += _buff.constant;
				}
				else
					_target.buffList.Add(_buff);
				break;
			case Buff.StackType.Add_One_Duration_And_One_Potency:
				if (_same != null)
				{
					_same.turns++;
					_same.constant += 1 * Mathf.Sign(_same.constant);
				}
				else
					_target.buffList.Add(_buff);
				break;
			default:
				Debug.LogError("ERROR when adding buff: " + _buff.name + " to: " + _target.name);
				break;

		}
	}


	public static RaycastHit2D CheckIfHit(Vector3 _clickPos)
	{
		Debug.DrawLine(_clickPos,_clickPos + Vector3.up * 0.1f,Color.red,4,false);
		Debug.DrawLine(_clickPos,_clickPos + Vector3.right * 0.1f,Color.blue,4,false);

		lastClick = _clickPos;

		//RaycastHit2D _hit = Physics2D.Raycast(_clickPos,Vector2.zero,0.01f);


		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		var _screenRayHit = Physics2D.GetRayIntersection(ray, CURSOR_HIT_DISTANCE);
		
		if (_screenRayHit.transform != null)
		{
			print("hot: " + _screenRayHit.transform.name + " at dist: " + _screenRayHit.distance);//Instantiate(particle, transform.position, transform.rotation);
		}
		
		Debug.DrawRay(ray.origin, ray.direction * 200, Color.cyan, 5, false);

		/*
		 * Get Pixel Test
		 * 
		if (_hit.transform != null)
		{
			//print(_hit);
			var localPos = ((Vector2)_hit.transform.position) - _hit.point;
			localPos = localPos*(2 / 2.56f * 256) - Vector2.one * 128;
			localPos *= -1;
			//print(-localPos);


			Vector2Int intPos = new Vector2Int((int)localPos.x, (int)localPos.y); //(Vector2Int)localPos;


			print(_hit.transform.GetComponentInChildren<UnityEngine.UI.Image>().sprite.texture.GetPixel(intPos.x,intPos.y));
		}
		print(_screenRayHit.transform);
		*/
		return _screenRayHit;
		//return _hit;
	}



	static CombatController GetHitCombatController(RaycastHit2D _hit)
	{
		CombatController _cc = null;
		if (_hit.transform != null)
		{
			_cc = _hit.transform.GetComponentInParent<CombatController>();
		}

		return _cc;
	}


	static IEnumerator CircleCollision(Transform _objectToCheck, float _frequency, float _radius, System.Action<CombatController> _actionOnHit)
	{
		List<object[]> _justHit = new List<object[]>();

		while (true)
		{
			Debug.DrawRay(_objectToCheck.position,Vector3.down * _radius,Color.yellow,0,false);
			Debug.DrawRay(_objectToCheck.position, Vector3.left * _radius, Color.cyan, 0, false);

			for (int i = 0; i < _justHit.Count; i++)
			{
				_justHit[i][1] = (float)_justHit[i][1] - Time.deltaTime;
				if ((float)_justHit[i][1] <= 0)
				{
					_justHit.Remove(_justHit[i]);
					i--;
				}
			}
			/*
			foreach (var _entry in _justHit)
			{
				_entry[1] = (float)_entry[1] - Time.deltaTime;
				if ((float)_entry[1] <= 0)
				{
					_justHit.Remove(_entry);
				}
			}
			*/
			//_justHit.ForEach(x => { x[1] = (float)x[1] - Time.deltaTime;  if ((float)x[1] <= 0) _justHit.Remove(x); });

			foreach (Collider2D _col in Physics2D.OverlapCircleAll(_objectToCheck.position, _radius)) //find all cols
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

	public static IEnumerator PoisonBite(TargetData targetData)
	{
		targetData.self.StartCoroutine(Punch(targetData));
		//if (Random.Range(0,2) == 0)
			targetData.self.StartCoroutine(Poison(targetData));

		yield return null;
	}

	public static IEnumerator Poison(TargetData targetData)
	{
		var _t = EffectTools.SpawnEffect(targetData.ability.name, targetData.centerPos, 1).transform;
		_t.SetParent(targetData.target.MyMono.transform);

		int _potency = Mathf.Clamp(targetData.self.MyStats.Intelligence,1,5);

		//var _buff = new Buff("Poisioned", AbilityClass.poisionTick, _potency, BuffIcons.TryGetBuffIcon("poision"), Buff.StackType.Pick_Most_Turns, _potency);
		var _buff = new Buff("Poisioned", AbilityCollection.poisonTick, _potency, BuffIcons.TryGetBuffIcon(21), Buff.StackType.Pick_Most_Turns, _potency);
		AddBuff(_buff, targetData.target.MyStats);
		
		yield return null;
	}
	
	public static IEnumerator PoisonTick(TargetData targetData) //only available through the poision ability
	{
		var _previousBuff = targetData.target.MyStats.buffList.Find(x => x.name == "Poisioned");
		
		if (_previousBuff != null)
		{
			_previousBuff.constant = _previousBuff.turns;

			targetData.target.AdjustHealth(-Mathf.Max((int)_previousBuff.constant,0),targetData.element, ExtraData.none);
			var _t = EffectTools.SpawnEffect(AbilityCollection.poison.name, targetData.target.MyMono.transform.position,1).transform;
			_t.SetParent(targetData.target.MyMono.transform);
		}
		yield return null;
	}

	public static IEnumerator Thunderbolt(TargetData targetData)
	{
		var _topPress = targetData.centerPos;
		_topPress.y = TopPosition.y;

		var _bolt = EffectTools.SpawnEffect(targetData.ability.name, _topPress); //spawn effect
		var _mono = _bolt.gameObject.AddComponent<EmptyMonoBehaviour>(); //add monoholder
		List<GameObject> _spawnedMonos = new List<GameObject>();
		_bolt.name = "orgBolt";

		_spawnedMonos.Add(_mono.gameObject);
		Destroy(_bolt.gameObject, 3);

		_mono.StartCoroutine(EffectTools.ActivateInOrder(_mono, new List<EffectTools.FunctionGroup>()
		{
				new EffectTools.FunctionGroup(EffectTools.MoveDirection(_bolt.transform, Vector3.down,2,0.1f),0), //move effect
		}));

		//var _moDel = new EffectTools.FunctionAndDelay(EffectTools.MoveDirection(_bolt.transform, Vector3.down + Vector3.right * randomVector3.x, 2, 0.1f), 0);
		for (int i = 1; i < 100; i++)
		{
			//_moDel._secToStart = i * 0.1f;
			_mono.StartCoroutine(EffectTools.ActivateInOrder(_mono, new List<EffectTools.FunctionGroup>() {
				//_moDel,
				new EffectTools.FunctionGroup(EffectTools.MoveDirection(_bolt.transform, Vector3.down + Vector3.right * RandomVector3.x,2,0.1f),i * 0.1f), //move effect
			}));
		}

		_mono.StartCoroutine(CircleCollision(_bolt.transform,0.1f, 0.1f,
			delegate (CombatController _cc) //check for collisions, when found...
			{
				_cc.AdjustHealth(-Mathf.Max(targetData.self.MyStats.Intelligence, 0), targetData.element, targetData.ability.extraData); //change health
				GameObject _orgHit = _cc.gameObject;
				
				var _hits = Physics2D.OverlapCircleAll(_bolt.transform.position, 1f); //check for nearby colliders
				for (int i = 0; i < _hits.Length; i++) //for all found
				{
					if (_hits[i].gameObject == _cc.gameObject) continue;
					if (_hits[i].GetComponent<CombatController>() == null) continue;

					var _spawnedBolt = EffectTools.SpawnEffect(targetData.ability.name, _bolt.transform.position); //spawn new effect
					var _spawnedMono = _spawnedBolt.gameObject.AddComponent<EmptyMonoBehaviour>(); //add mono holder
					Destroy(_spawnedBolt.gameObject, 3);
					_spawnedMonos.Add(_spawnedMono.gameObject);
					_spawnedBolt.name = "spawnBolt " + i;

					_spawnedMono.StartCoroutine(EffectTools.ActivateInOrder(_spawnedMono, new List<EffectTools.FunctionGroup>() {
						new EffectTools.FunctionGroup(EffectTools.MoveDirection(_spawnedBolt.transform, _hits[i].transform.position - _spawnedBolt.transform.position,2,2f),0.5f), //move effect
					}));
					//_spawnedMono.StartCoroutine(EffectTools.MoveDirection(_spawnedBolt.transform, _hits[i].transform.position - _spawnedBolt.transform.position,2,2)); //move effect 2

					_spawnedMono.StartCoroutine(CircleCollision(_spawnedBolt.transform, 0.1f, 0.1f, //check for colliders
					delegate (CombatController _cc2) //when a collider is found
					{
						if (_cc2.gameObject != _orgHit) //if the hit is not the original one
						{
							_cc2.AdjustHealth(-Mathf.Max(Mathf.CeilToInt((float)targetData.self.MyStats.Intelligence/2), 0), targetData.element, targetData.ability.extraData);
							Destroy(_spawnedBolt.gameObject);
						}
					}));
				}
				

				Destroy(_bolt.gameObject);
			}));


		
		do
		{
			print(_spawnedMonos.Count);
			for(int i = 0; i < _spawnedMonos.Count; i++)
				if (_spawnedMonos[i] == null)
				{
					_spawnedMonos.RemoveAt(i);
					i--;
				}
			yield return new WaitForSeconds(0.1f);
		}
		while (_spawnedMonos.Count > 0);
		

		yield return null;
	}

	public static IEnumerator CrystalLance(TargetData targetData)
	{
		var _buff = new Buff("Crystalized", new List<Buff.TraitType> { Buff.TraitType.Physical_Defence_Constant, Buff.TraitType.Magic_Defence_Constant }, 1, BuffIcons.TryGetBuffIcon(17), Buff.StackType.Pick_Most_Turns, 99);
		AddBuff(_buff, targetData.self.MyStats);

		var _t = EffectTools.SpawnEffect(targetData.ability.name, targetData.self.transform.position,10).transform;
		_t.SetParent(targetData.self.transform);

		var _mono = _t.gameObject.AddComponent<EmptyMonoBehaviour>();
		_mono.StartCoroutine(EffectTools.MoveDirection(_t, targetData.centerPos - _t.transform.position, 2, 10));
		_mono.StartCoroutine(CircleCollision(_t, 0.1f, 0.1f, delegate (CombatController _cc) {
			if (_cc.transform != targetData.self.transform)
			{
				print(_cc.transform.name +  " " + _t.name);
				_cc.AdjustHealth(-Mathf.Max(targetData.self.MyStats.Strength, 0), targetData.element, targetData.ability.extraData);
				Destroy(_t.gameObject);
			}
		}));

		//yield return new WaitForSeconds(0.5f);

		//var _buff = new Buff("Crystalized", new List<string> { "defense_constant", "magicDefense_constant" }, 1, BuffIcons.TryGetBuffIcon("Crystalized"), Buff.StackType.Pick_Most_Turns, 99);

		yield return null;
	}

	public static IEnumerator Harden(TargetData targetData)
	{
		var _t = EffectTools.SpawnEffect(targetData.ability.name, targetData.self.transform.position, 1).transform;
		_t.SetParent(targetData.self.transform);

		//var _buff = new Buff("Hardened Skin","defense_constant",3, BuffIcons.TryGetBuffIcon("Hardened"), Buff.StackType.Pick_Most_Turns, 2);
		var _buff = new Buff("Hardened Skin", Buff.TraitType.Physical_Defence_Constant, 3, BuffIcons.TryGetBuffIcon(15), Buff.StackType.Pick_Most_Turns, 2);

		AddBuff(_buff, targetData.self.MyStats);
		yield return null;
	}

	public static IEnumerator MagicShield(TargetData targetData)
	{
		var _t = EffectTools.SpawnEffect(targetData.ability.name, targetData.self.transform.position, 1).transform;
		_t.SetParent(targetData.self.transform);

		//var _t2 = EffectTools.SpawnEffect("blue_sparkles", targetData.self.transform.position, 1, 11).transform;
		//_t2.SetParent(targetData.self.transform);

		//var _buff = new Buff("Magic Shield", "magicDefense_constant", 3, BuffIcons.TryGetBuffIcon("MagicShielded"), Buff.StackType.Pick_Most_Turns, 2);
		var _buff = new Buff("Magic Shield", Buff.TraitType.Magic_Defence_Constant, 3, BuffIcons.TryGetBuffIcon(14), Buff.StackType.Pick_Most_Turns, 2);

		AddBuff(_buff, targetData.self.MyStats);
		//print(targetData.self.name + " has now buffs: " + targetData.self.MyStats.buffList.Count);
		yield return null;
	}

	public static IEnumerator Bubble(TargetData targetData)
	{
		var _t = EffectTools.SpawnEffect(targetData.ability.name, targetData.centerPos, 10).transform;
		var _mono = _t.gameObject.AddComponent<EmptyMonoBehaviour>();

		_mono.StartCoroutine(EffectTools.MoveDirection(_t,Vector3.right,0.4f,10));
		_mono.StartCoroutine(CircleCollision(_t, 0.2f, 0.5f, delegate (CombatController _cc) {
			_cc.AdjustHealth(-Mathf.Max(targetData.self.MyStats.Intelligence,0),targetData.element, targetData.ability.extraData);
		}));

		_mono.StartCoroutine(EffectTools.Wobble(_t,0.75f, 0.75f, 10));
		
		yield return null;
	}

	public static IEnumerator AirSlash(TargetData targetData)
	{
		var _t = EffectTools.SpawnEffect(targetData.ability.name, targetData.centerPos, 1).transform;
		var _mono = _t.gameObject.AddComponent<EmptyMonoBehaviour>();
		_mono.StartCoroutine(EffectTools.MoveDirection(_t,Vector3.left,5,1) );
		_mono.StartCoroutine(CircleCollision(_t,0.05f,0.5f, delegate (CombatController cc) {
			cc.AdjustHealth(-Mathf.Clamp(targetData.self.MyStats.Intelligence * 2 ,0 , 10),targetData.element, targetData.ability.extraData);
			Destroy(_t.gameObject);
		}));


		yield return null;
	}

	public static IEnumerator DivineFists(TargetData targetData)
	{
		yield return targetData.self.StartCoroutine(AbilityCollection.divineLuck.function(targetData));// DivineLuck(targetData._target));
		yield return targetData.self.StartCoroutine(AbilityCollection.bulkUp.function(targetData));//BulkUp(targetData._target));
	}
	
	public static IEnumerator Punch(TargetData targetData)//CombatController _target, int _damage, Elementals _element = Elementals.Physical)
	{
		int _num = 0;
		if (targetData.useOwnStats)
			_num = 1;
		_num += targetData.bonus;

		var _t = EffectTools.SpawnEffect(targetData.ability.name, lastClick, 1).transform;
		if (targetData.target != null)
		{
			_t.SetParent(targetData.target.MyMono.transform);

			targetData.target.AdjustHealth(-Mathf.Max(_num, 0), targetData.element, targetData.ability.extraData);
		}

		yield return null;
	}

	public static IEnumerator FreezingStrike(TargetData targetData)
	{
		//targetData.element = Elementals.Ice;
		yield return targetData.self.StartCoroutine(Punch(targetData));
		//var _buff =  new Buff("Frozen", new List<string> { "dexterity_constant",}, 2, BuffIcons.TryGetBuffIcon("Frozen"), Buff.StackType.Add_One_Duration_And_One_Potency, -1);
		var _buff = new Buff("Frozen", new List<Buff.TraitType> { Buff.TraitType.Dexterity_Constant, }, 2, BuffIcons.TryGetBuffIcon(18), Buff.StackType.Add_One_Duration_And_One_Potency, -1);
		//var _buff2 = new Buff("Hardened Skin", "defense_constant", 2, BuffIcons.TryGetBuffIcon("Hardened"), Buff.StackType.Add_One_Duration_And_One_Potency, 1, _shouldBeDisplyed: false);

		AddBuff(_buff, targetData.target.MyStats);
		//targetData.self.AddBuff(_buff, targetData.target);
	}

	public static IEnumerator DoubleKick(TargetData targetData)//(Vector3 _centerPos, CombatController _target, CombatController _self)
	{
		yield return targetData.self.StartCoroutine(AbilityCollection.punch.function(targetData)); //StartCoroutine(Punch(_target,_self.myStats.strength));
		targetData.target = null;
		yield return new WaitForSeconds(2);
		var _hit = CheckIfHit(targetData.centerPos);

		/*
		if (_hit.transform != null)
		{
			targetData.target = _hit.transform.GetComponent<CombatController>();
		}
		*/
		targetData.target = GetHitCombatController(_hit);

		yield return targetData.self.StartCoroutine(AbilityCollection.punch.function(targetData)); //StartCoroutine(Punch(_target,_self.myStats.strength));


		//yield return StartCoroutine(Punch(_target, _self.myStats.strength));
	}


	public static IEnumerator WildPunch(TargetData targetData)// (Vector3 _centerPos, CombatController _self)
	{
		targetData.target = null;
		targetData.bonus = 3;
		var _hit = CheckIfHit(targetData.centerPos + RandomVector3);
		/*
		if (_hit.transform != null)
		{
			targetData.target = _hit.transform.GetComponent<CombatController>();
		}
		*/
		targetData.target = GetHitCombatController(_hit);
		yield return targetData.self.StartCoroutine(AbilityCollection.punch.function(targetData));// (Punch(_target, targetData._self.myStats.strength + 2));
		yield return null;
	}

	public static IEnumerator ForcePunch(TargetData targetData)//(CombatController _target, CombatController _self)
	{
		int _num = 0;
		if (targetData.useOwnStats)
		{
			_num = 2;
			if (targetData.self.MyStats.Intelligence > 5) _num += 3; //targetData.bonus += 2;
		}


		yield return targetData.target.AdjustHealth(-_num, targetData.ability.element, targetData.ability.extraData); //targetData.self.StartCoroutine(AbilityCollection.punch.function(targetData));// (Punch(_target, _self.myStats.strength + 2, Elementals.Air));
		yield return null;
	}

	public static IEnumerator TiltSwing(TargetData targetData)//(CombatController _target, CombatController _self)
	{
		//float _randomNumber = Random.Range(0, 2);
		//int _result = (int)((_randomNumber-0.5f) * 2);

		targetData.bonus += Random.Range(0, 2) + Random.Range(0, 2); //25%: 0 50%: 1 25%: 2

		yield return targetData.self.StartCoroutine(AbilityCollection.punch.function(targetData));//Punch(_target, _self.myStats.strength + _r));
		yield return null;
	}

	public static IEnumerator ChaosThesis(TargetData targetData)//(Vector3 _centerPos, CombatController _self)
	{
		int _r = Random.Range(0,3);

		if (_r == 0)
		{
			targetData.bonus += 2;
			yield return targetData.self.StartCoroutine(AbilityCollection.manaDrain.function(targetData));// ManaDrain(_target,_self, 2));
		}
		else if (_r == 1)
		{
			yield return targetData.self.StartCoroutine(AbilityCollection.fireball.function(targetData));//Fireball(_centerPos, _self, 2));
		}
		else if (_r == 2)
		{
			yield return targetData.self.StartCoroutine(AbilityCollection.heal.function(targetData));// Heal(_target, _self.myStats.luck));
		}

		yield return null;
	}

	public static IEnumerator LifeTap(TargetData targetData)
	{
        EffectTools.SpawnEffect(targetData.ability.name, targetData.self.transform.position, 1);
		int _tapped = targetData.self.AdjustHealth(-Mathf.Min(5, targetData.self.MyStats.currentHealth -1),Elementals.Void, targetData.ability.extraData);
		targetData.self.AdjustMana(_tapped);
		yield return null;
	}

	public static IEnumerator SiphonSoul(TargetData targetData)
	{
		if(targetData.target != null)
		{
			int _hpRecover = targetData.target.AdjustHealth(-Mathf.Max(targetData.self.MyStats.Luck,0),Elementals.Unlife, targetData.ability.extraData);

			targetData.self.AdjustHealth(Mathf.Max(_hpRecover,0), targetData.element, targetData.ability.extraData);
		}
		yield return null;
	}

	public static IEnumerator Smite(TargetData targetData)
	{
		EffectTools.SpawnEffect(targetData.ability.name,lastClick,1);

		if (targetData.target != null)
		{

			//_self.AdjustMana(-manaCostDictionary["smite unlife"]);
			//EffectTools.SpawnEffect("punch",lastClick,1);
			int _smiteDamage = (targetData.target.MyStats.race.HasFlag(targetData.targetRace)) ? 2 : 0;
			targetData.bonus += _smiteDamage;
			if (_smiteDamage > 0)
				targetData.element = Elementals.None;

			yield return targetData.self.StartCoroutine(Punch(targetData));
			//targetData.target.AdjustHealth(-Mathf.Max(targetData.self.myStats.luck + targetData.bonus + _smiteDamage, 0),Elementals.None);
		}
		yield return null;
	}

	public static IEnumerator MeteorShower(TargetData targetData)
	{
		Vector3 _top = TopPosition;
		List<EmptyMonoBehaviour> _meteors = new List<EmptyMonoBehaviour>();

		//Spawn meteors
		int _totalBalls = 4 + (targetData.self.MyStats.Intelligence/2);
		for (int i = 0; i < _totalBalls; i++)
		{
			Vector3 _randomDir = Vector3.right * Random.Range(-0.7f,0.7f);

			var _meteor = EffectTools.SpawnEffect((Random.Range(0,128) == 0)? AbilityCollection.punch.name :  targetData.ability.name, _top + Vector3.left * (i - Random.Range(1f,1.5f)), 6);
			var _meteorMono = _meteor.gameObject.AddComponent<EmptyMonoBehaviour>();
			_meteorMono.StartCoroutine(EffectTools.MoveDirection(_meteor.transform, Vector3.down + _randomDir, 3, 5)); //global set the effect
			_meteors.Add(_meteorMono);
			yield return new WaitForSeconds(Random.Range(0.3f,0.6f));
		}

		//make them do collision checks
		for (int i = 0; i < _meteors.Count; i++)
		{
			_meteors[i].StartCoroutine(CircleCollision(_meteors[i].transform, 0.25f, 1f,
				delegate (CombatController _cc)
				{
					_cc.AdjustHealth(-Mathf.Max(targetData.self.MyStats.Intelligence, 0), targetData.element, targetData.ability.extraData);
				}
			));
		}

		//destroy them if they get too far off screen
		while (_meteors.Count > 0)
		{
			for (int i = 0; i < _meteors.Count; i++)
			{
				if (_meteors[i].transform.position.y <= -1)
				{
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

	public static IEnumerator Eruption(TargetData targetData)
	{
		for (int i = 0; i < targetData.self.MyStats.Intelligence; i++)
		{
			var _rock =	EffectTools.SpawnEffect(targetData.ability.name,targetData.self.transform.position + Vector3.up,5);
			var _rockMono = _rock.gameObject.AddComponent<EmptyMonoBehaviour>();
			_rockMono.StartCoroutine(EffectTools.CurveDropMove(_rock.transform, 5, 14, 1));
			Destroy(_rock,4);
			_rockMono.StartCoroutine(CircleCollision(_rock.transform, 0.1f, 0.3f, delegate (CombatController _cc)
			{
				if (_cc.gameObject != targetData.self.gameObject)
					_cc.AdjustHealth(-Mathf.Max(targetData.self.MyStats.Intelligence,0), targetData.element, targetData.ability.extraData);
			}));

			yield return new WaitForSeconds(Random.Range(0.2f,0.5f));
		}
	}

	public static IEnumerator Fireball(TargetData targetData)
	{
		var _sprite = EffectTools.SpawnEffect(targetData.ability.name,targetData.centerPos,0.6f);
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
				_cc.AdjustHealth(-Mathf.Max(targetData.self.MyStats.Intelligence,0), targetData.element, targetData.ability.extraData);
			}
		}

		yield return null;
	}

	public static IEnumerator MassExplosion(TargetData targetData)//(Vector3 _centerPos,CombatController _self)
	{
		targetData.centerPos += Vector3.left; //-1
		targetData.self.StartCoroutine(AbilityCollection.fireball.function(targetData));
		yield return new WaitForSeconds(0.1f);

		targetData.centerPos += Vector3.right; //0
		targetData.self.StartCoroutine(AbilityCollection.fireball.function(targetData));
		yield return new WaitForSeconds(0.1f);

		targetData.centerPos += Vector3.right; //+1
		targetData.self.StartCoroutine(AbilityCollection.fireball.function(targetData));
		yield return null;
	}

	public static IEnumerator Heal(TargetData targetData)
	{
		int num = 0;
		if (targetData.useOwnStats)
			num = 3;//targetData.self.myStats.Luck;

		if (targetData.target != null)
		{
			targetData.target.AdjustHealth(Mathf.CeilToInt(Mathf.Max(num + targetData.bonus,0)),Elementals.Light, targetData.ability.extraData);

			EffectTools.SpawnEffect(targetData.ability.name, targetData.target.MyMono.transform.position, 1).transform.SetParent(targetData.target.MyMono.transform);
		}
		else if (lastClick != null)
			EffectTools.SpawnEffect(targetData.ability.name, lastClick, 1);

		yield return null;
	}

	public static IEnumerator ManaDrain(TargetData targetData)
	{
		if(targetData.target != null)
		{
			int _manaRecover = 0;

			_manaRecover = targetData.target.AdjustMana(-Mathf.Max((Mathf.CeilToInt((float)targetData.self.MyStats.Luck/2) + targetData.bonus),0));

			targetData.self.AdjustMana(Mathf.Max(_manaRecover,0));

			yield return null;
		}

		yield return null;
	}

	public static IEnumerator RestoreSoul(TargetData targetData)//(CombatController _self)
	{
		targetData.self.MyStats.buffList.Clear();
		yield return null;
	}

	public static IEnumerator Clense(TargetData targetData)//(CombatController _self)
	{
		if (targetData.self.MyStats.buffList.Count > 0)
		{
			targetData.self.MyStats.buffList.RemoveAt(Random.Range(0, targetData.self.MyStats.buffList.Count));
			EffectTools.SpawnEffect(targetData.ability.name, targetData.self.transform.position,1);
		}
		yield return null;
	}

	public static IEnumerator SyncSoul(TargetData targetData)//(CombatController _target, CombatController _self)
	{
		if (targetData.target != null)
		{
			targetData.self.MyStats.buffList.Clear();
			targetData.self.MyStats.buffList.AddRange(targetData.target.MyStats.buffList);
		}
		yield return null;
	}

	public static IEnumerator Bless(TargetData targetData)//(CombatController _target)
	{
		EffectTools.SpawnEffect(targetData.ability.name, lastClick, 1);

		//var _buff = new Buff(AbilityClass.bless.name, new List<string> { "strenght_mutliplier", "dexterity_multiplier", "intelligence_multiplier", "luck_multiplier" }, 3, BuffIcons.TryGetBuffIcon("bless"), Buff.StackType.Pick_Most_Turns, 2);
		var _buff = new Buff(AbilityCollection.bless.name, new List<Buff.TraitType> { Buff.TraitType.Strength_Multiplier, Buff.TraitType.Dexterity_Multiplier, Buff.TraitType.Intelligence_Multiplier, Buff.TraitType.Luck_Multiplier }, 3, BuffIcons.TryGetBuffIcon(22), Buff.StackType.Pick_Most_Turns, 2);

		AddBuff(_buff, targetData.target.MyStats);

		yield return null;
	}

	public static IEnumerator Curse(TargetData targetData)//(CombatController _target)
	{
		EffectTools.SpawnEffect(targetData.ability.name, lastClick, 1);

		//var _buff = new Buff(AbilityClass.curse.name, new List<string> { "strenght_mutliplier", "dexterity_multiplier", "intelligence_multiplier", "luck_multiplier" }, 3, BuffIcons.TryGetBuffIcon("curse"), Buff.StackType.Pick_Most_Turns, 0.5f);
		var _buff = new Buff(AbilityCollection.curse.name, new List<Buff.TraitType> { Buff.TraitType.Strength_Multiplier, Buff.TraitType.Dexterity_Multiplier, Buff.TraitType.Intelligence_Multiplier, Buff.TraitType.Luck_Constant }, 3, BuffIcons.TryGetBuffIcon(23), Buff.StackType.Pick_Most_Turns, 0.5f);

		AddBuff(_buff, targetData.target.MyStats);

		yield return null;
	}

	public static IEnumerator BulkUp(TargetData targetData)//(CombatController _self)
	{
		EffectTools.SpawnEffect(targetData.ability.name,lastClick,1);

		//var _buff = new Buff(AbilityClass.bulkUp.name,"strength_constant",2, BuffIcons.TryGetBuffIcon("pluss_strength"), Buff.StackType.Add_One_Duration_Add_All_Potency,1);
		var _buff = new Buff(AbilityCollection.bulkUp.name, Buff.TraitType.Strength_Constant, 2, BuffIcons.TryGetBuffIcon(3), Buff.StackType.Add_One_Duration_Add_All_Potency, 1);

		AddBuff(_buff, targetData.self.MyStats);

		yield return null;
	}

	public static IEnumerator Debulk(TargetData targetData)
	{
		if (targetData.target != null)
		{
			//var _buff = new Buff(AbilityClass.debulk.name, "strength_constant", 3, BuffIcons.TryGetBuffIcon("pluss_strength"), Buff.StackType.Pick_Most_Potent, -2);
			var _buff = new Buff(AbilityCollection.debulk.name, Buff.TraitType.Strength_Constant, 3, BuffIcons.TryGetBuffIcon(4), Buff.StackType.Pick_Most_Potent, -2);

			AddBuff(_buff, targetData.target.MyStats);
		}

		yield return null;
	}

	public static IEnumerator DivineLuck(TargetData targetData)
	{
		EffectTools.SpawnEffect(targetData.ability.name,lastClick,1);

		//var _buff = new Buff(AbilityClass.divineLuck.name, "luck_constant", 3, BuffIcons.TryGetBuffIcon("divine_luck"), Buff.StackType.Pick_Most_Potent,2);
		var _buff = new Buff(AbilityCollection.divineLuck.name, Buff.TraitType.Luck_Constant, 3, BuffIcons.TryGetBuffIcon(9), Buff.StackType.Pick_Most_Potent, 2);

		AddBuff(_buff, targetData.self.MyStats);
		yield return null;
	}

	public static IEnumerator Regeneration(TargetData targetData)
	{
		EffectTools.SpawnEffect(targetData.ability.name, targetData.centerPos, 1);// lastClick + Vector3.forward,1);

		int _amount = 0;
		if (targetData.useOwnStats)
		{
			_amount = 2;
		}
		_amount += targetData.bonus;

		if(targetData.target != null)
		{
			//var _buff = new Buff(AbilityClass.regeneration.name, AbilityClass.heal.name,3, BuffIcons.TryGetBuffIcon("yellow_pluss"), Buff.StackType.Pick_Most_Potent, Mathf.Max(targetData.self.myStats.Luck,0));
			//var _buff = new Buff(AbilityCollection.regeneration.name, AbilityCollection.heal.name, 3, BuffIcons.TryGetBuffIcon(12), Buff.StackType.Pick_Most_Potent, Mathf.Max(_amount, 0));
			var _buff = new Buff(AbilityCollection.regeneration.name, AbilityCollection.heal, 3, BuffIcons.TryGetBuffIcon(12), Buff.StackType.Pick_Most_Potent, Mathf.Max(_amount, 0));

			AddBuff(_buff,targetData.target.MyStats);
		}
		yield return null;
	}

	public static IEnumerator MassHeal(TargetData targetData)
	{
		CombatController.turnOrder.ForEach(x => {
			lastClick = x.transform.position;
			x.StartCoroutine(AbilityCollection.heal.function(targetData));
		});

		yield return null;
	}

	public static IEnumerator Eat(TargetData targetData)
	{
		targetData.target.AdjustHealth(targetData.bonus, Elementals.Light, targetData.ability.extraData);

		yield return null;
	}

	public static IEnumerator Focus(TargetData targetData)
	{
		int _mana = 0;
		if (targetData.useOwnStats)
		{
			_mana = 5;
		}
		_mana += targetData.bonus;
		
		EffectTools.SpawnEffect(targetData.ability.name, targetData.self.transform.position,1);
		targetData.self.AdjustMana(Mathf.Max(_mana, 0));
		yield return null;
	}

	public static IEnumerator SpotWeakness(TargetData targetData)
	{
		if (targetData.target != null)
		{
			targetData.target.MyMono.StartCoroutine(AbilityCollection.keenSight.function(targetData));// displayCritAreas.function(targetData));
		}

		yield return null;
	}

	public static IEnumerator TimeWarp(TargetData targetData)
	{
		EffectTools.SpawnEffect(targetData.ability.name,lastClick,1);
		//var _buff = new Buff(AbilityClass.timeWarp.name,"extra turn",2, BuffIcons.TryGetBuffIcon("pluss_time"), Buff.StackType.Add_Duplicate,1);
		var _buff = new Buff(AbilityCollection.timeWarp.name, Buff.TraitType.Extra_Turn, 2, BuffIcons.TryGetBuffIcon(11), Buff.StackType.Add_Duplicate, 1);

		AddBuff(_buff, targetData.self.MyStats);
		yield return null;
	}

	/// <summary>
	/// Displays crit areas of all enemies if no parameter is given, otherwise, display the crit area of the enemy given.
	/// </summary>
	/// <param name="_self"></param>
	public static IEnumerator DisplayCritAreas(TargetData targetData)//(CombatController _self,CombatController _target = null)
	{
		/*
		if(_manacost == 0)
			_self.AdjustMana(-manaCostDictionary["keen sight"]);
		else
			_self.AdjustMana(_manacost);
		*/

		//var _checks = (targetData.target == null)? CombatController.turnOrder: new List<CombatController>() { targetData.target.MyMono };

		foreach (CombatController _cc in CombatController.turnOrder)//_checks)
		{
			var _critArea = _cc.transform.Find("$CritArea");
			print("area: " + _critArea);
			if (_critArea != null)
			{
				var _critImage = _critArea.GetComponent<SpriteRenderer>();
				if (_critImage != null)
					targetData.target.MyMono.StartCoroutine(EffectTools.BlinkImage(_critImage,Color.white,5.5f,10));
			}
		}

		yield return null;

	}

	public static IEnumerator Wobble(TargetData targetData)
	{		
		targetData.self.StartCoroutine(EffectTools.ActivateInOrder(targetData.self, new List<EffectTools.FunctionGroup>()
		{
			new EffectTools.FunctionGroup(EffectTools.MoveDirection(targetData.self.transform,Vector3.right,1,0.2f), 0),
			new EffectTools.FunctionGroup(EffectTools.MoveDirection(targetData.self.transform,Vector3.left,1,0.3f), 0.2f),
			new EffectTools.FunctionGroup(EffectTools.MoveDirection(targetData.self.transform,Vector3.right,1,0.1f), 0.3f),
		}));
		yield return null;
	}


	public static IEnumerator Spook(TargetData targetData)//(CombatController _target,CombatController _self)
	{
		yield return targetData.self.StartCoroutine(EffectTools.BlinkImage(targetData.self.transform.GetComponent<SpriteRenderer>(),new Color(1,1,1,0),1,1));
		targetData.target.AdjustHealth(-targetData.self.MyStats.Intelligence, Elementals.Unlife, targetData.ability.extraData);
	}
}

/*
public static class Vector2Extentions
{
	public static Vector2Int ToVector2Int(this Vector2 v2)
	{
		return new Vector2Int((int)v2.x, (int)v2.y);
	}
}
*/
