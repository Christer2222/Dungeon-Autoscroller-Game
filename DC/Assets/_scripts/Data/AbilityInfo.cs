using System.Collections;
using UnityEngine;

namespace AbilityInfo
{
	public enum ExtraData
	{
		none = 0,
		nonPiercing = 1,
		makes_contact_with_user = 2,
		magic = 4,
	}

	public enum SkillUsed
	{
		none = 0,
		heavy_hits = 1,
		light_hits = 2,
		healing = 4,
		magic = 8,
	}

	public enum AbilityType
	{
		none = 0,
		misc = 1,
		debuff = 2,
		recovery = 4,
		offensive = 8,
		attack = 16,
		buff = 32,
		defensive = 64,
	}

	//can combine elements by doing Elem | Elem and check by doing 
	public enum Elementals
	{
		None = 0,
		Physical = 1,
		Fire = 2,
		Water = 4,
		Earth = 8,
		Air = 16,
		Plasma = 32,
		Ice = 64,
		Poision = 128,
		Electricity = 256,
		Steam = 512,
		Light = 1024,
		Unlife = 2048,
		Void = 4096,
	}

	public struct TargetData
	{
		public TargetData(Ability _currentAbility, CombatController _self = null, CombatController _target = null, int _constant = 0, Elementals _element = default, Vector3 _centerPos = default, StatBlock.Race _race = default)
		{
			self = _self;
			target = _target;
			bonus = _constant;
			element = _element;
			centerPos = _centerPos;
			targetRace = _race;
			ability = _currentAbility;
		}

		public CombatController self;
		public CombatController target;
		public int bonus;
		public Elementals element;
		public Vector3 centerPos;
		public StatBlock.Race targetRace;
		public Ability ability;
	}

	public class Ability
	{
		public delegate IEnumerator FunctionToCall(TargetData inputData);
		
		public Ability(string _name, FunctionToCall _function, Elementals _element, SkillUsed _skill, AbilityType _type, int _manaCost, ExtraData _extraData)
		{
			name = _name;
			function = _function;
			manaCost = _manaCost;
			element = _element;
			abilityType = _type;
			extraData = _extraData;

			string _path = "Assets/Resources/Sprites/Effects/AnimationTexts/";
			string _standardizedName = _name.Replace(" ", "_").ToLower();
			string _extention = "_a_text";


			var _textAsset = Resources.Load<TextAsset>("Sprites/Effects/AnimationTexts/" + _standardizedName + _extention);
			if (_textAsset == null) //if the text asset wasn't found
			{
				_textAsset = AnimationTextParser.GetNewTextAssetOrAddNewToAssetDatabase(_path + "EMPTY_" + _standardizedName + _extention + ".txt");
			}

			//Debug.Log(_standardizedName + " parsed: ");
			var _spriteArray = AnimationTextParser.ParseDocument(_textAsset, AnimationTextParser.Type.effect);

			EffectTools.AddToEffectDictionary(name, _spriteArray);
	}

	
		public string name;
		public FunctionToCall function;
		public int manaCost;
		public Elementals element;
		public AbilityType abilityType;
		public SkillUsed abilityClass;
		public ExtraData extraData;
	}
}


