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
		Poison = 128,
		Electricity = 256,
		Steam = 512,
		Light = 1024,
		Unlife = 2048,
		Void = 4096,
	}

	public struct TargetData
	{
		public TargetData(Ability _currentAbility, CombatController _self = null, CombatController _target = null, int _bonus = 0, Elementals _element = default, Vector3 _centerPos = default, StatBlock.Race _race = default, bool _useOwnStats = true)
		{
			self = _self;
			target = _target;
			bonus = _bonus;
			element = _element;
			centerPos = _centerPos;
			targetRace = _race;
			ability = _currentAbility;
			useOwnStats = _useOwnStats;
		}

		public CombatController self;
		public CombatController target;
		public int bonus;
		public Elementals element;
		public Vector3 centerPos;
		public StatBlock.Race targetRace;
		public Ability ability;
		public bool useOwnStats;
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
			abilityClass = _skill;

			string _descriptionPath = "Assets/Resources/Descriptions/Abilities/";
			string _effectPath = "Assets/Resources/Sprites/Effects/AnimationTexts/";

			string _standardizedName = _name.Replace(" ", "_").ToLower();
			string _animationExtention = "_a_text";
			string _descriptionExtention = "_description";

			var _descriptionTextAsset = Resources.Load<TextAsset>("Descriptions/Abilities/" + _standardizedName + _descriptionExtention);
			if (_descriptionTextAsset == null) //if the text asset wasn't found
			{
				_descriptionTextAsset = AnimationTextParser.GetNewTextAssetOrAddNewToAssetDatabase(_descriptionPath + "EMPTY_" + _standardizedName + _descriptionExtention + ".txt");
			}
			description = _descriptionTextAsset.text;
			description = description.Replace("$none", "<color=#333333>none</color>");
			description = description.Replace("$physical", "<color=#61737d>physical</color>");
			description = description.Replace("$fire", "<color=#a8270d>fire</color>");
			description = description.Replace("$water", "<color=#2706bd>water</color>");
			description = description.Replace("$earth", "<color=#654321>earth</color>");
			description = description.Replace("$air", "<color=#999999>air</color>");
			description = description.Replace("$plasma", "<color=#c712db>plasma</color>");
			description = description.Replace("$ice", "<color=#83d6eb>ice</color>");
			description = description.Replace("$poison", "<color=#367d49>poison</color>");
			description = description.Replace("$electricity", "<color=#fff645>electricity</color>");
			description = description.Replace("$steam", "<color=#b0b1d6>steam</color>");
			description = description.Replace("$light", "<color=#fffa78>light</color>");
			description = description.Replace("$unlife", "<color=#4960ab>unlife</color>");
			description = description.Replace("$void", "<color=#531c59>void</color>");

			/*


		Light = 1024,
		Unlife = 2048,
		Void = 4096,
			*/

			var _effectTextAsset = Resources.Load<TextAsset>("Sprites/Effects/AnimationTexts/" + _standardizedName + _animationExtention);
			if (_effectTextAsset == null) //if the text asset wasn't found
			{
				_effectTextAsset = AnimationTextParser.GetNewTextAssetOrAddNewToAssetDatabase(_effectPath + "EMPTY_" + _standardizedName + _animationExtention + ".txt");
			}

			//Debug.Log(_standardizedName + " parsed: ");
			var _spriteArray = AnimationTextParser.ParseDocument(_effectTextAsset, AnimationTextParser.Type.Effect);

			EffectTools.AddToEffectDictionary(name, _spriteArray);
	}

	
		public string name;
		public FunctionToCall function;
		public int manaCost;
		public Elementals element;
		public AbilityType abilityType;
		public SkillUsed abilityClass;
		public ExtraData extraData;
		public string description;
	}

	/*
	TextAsset FindOrMakeText(string _path, string _name, string _extention)
	{
		string _standardizedName = _name.Replace(" ", "_").ToLower();

		var _found = Resources.Load<TextAsset>("Sprites/Effects/AnimationTexts/" + _standardizedName + _extention);
		if (_effectTextAsset == null) //if the text asset wasn't found
		{
			_effectTextAsset = AnimationTextParser.GetNewTextAssetOrAddNewToAssetDatabase(_effectPath + "EMPTY_" + _standardizedName + _animationExtention + ".txt");
		}
	}
	*/
}


