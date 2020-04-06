using System.Collections.Generic;
using UnityEngine;
using AbilityInfo;

public class Items : AbilityClass
{
	private const string ITEM_CONSTANT = "½";
	private const string ITEM_DEFENSE = "¼";
	private const string ITEM_MAGIC_DEFENSE = "¾";
	private const string ITEM_ABILITY_LIST = "¹";

	
	public static readonly ItemInfo apple = new ItemInfo("Apple", 4, heal, 2, ItemType.Consumable, $"Apparently wards against doctors.\nHeals {ITEM_CONSTANT}hp.");
	public static readonly ItemInfo orange = new ItemInfo("Orange", 6, heal, 4, ItemType.Consumable, $"Named after a color, or has a color named after it?\nHeals {ITEM_CONSTANT}hp.");
	public static readonly ItemInfo banana = new ItemInfo("Banana", 8, heal, 6, ItemType.Consumable, $"Is this a snack?\nHeals {ITEM_CONSTANT}hp.");

	public static readonly ItemInfo stick = new ItemInfo("Stick", 2, punch, 1, ItemType.Craftable | ItemType.OneHanded, $"En Garde!\nDeals {ITEM_CONSTANT} damage when held and used as a weapon.\nCan also be used to craft various items.");

	public static readonly ItemInfo goldCoin = new ItemInfo("Gold Coint", 1, ItemType.Valueable, $"Cash, Moola, Dough, whatever you wanna call it, its money.");
	public static readonly ItemInfo goldbar = new ItemInfo("Gold Bar", 25, ItemType.Valueable, $"This should be worht quite a bit.");

	public static readonly ItemInfo headband = new ItemInfo("Headband", 10, 1, 0, ItemType.Headgear, $"Protects your noggin.\nGives you {ITEM_DEFENSE} defense.");
	public static readonly ItemInfo goldRing = new ItemInfo("Gold Ring", 15, 0, 1, ItemType.Acessory, $"Fancy.\nGives you {ITEM_MAGIC_DEFENSE} magic defense.");
	public static readonly ItemInfo strikeRing = new ItemInfo("Strike Ring", 35, forcePunch, 1, ItemType.Acessory | ItemType.Targetable, $"A magic ring.\nGives you acces to {ITEM_ABILITY_LIST}.");
	
	public enum ItemType
	{
		Garbage = 1,
		Valueable = 2, //same as garbage, but just used to declare that it is worth something
		Consumable = 4,
		Targetable = 8,
		Craftable = 16,

		Headgear = 32,
		Chestplate = 64,
		Leggings = 128,
		Boots = 256,
		Cape = 512,
		Gauntlets = 1024,
		Acessory = 2048,

		OneHanded = 4096,
		TwoHanded = 8192,


		Equipment = Headgear | Chestplate | Leggings | Boots | Cape | Gauntlets | Acessory, //check against this to see if item is equiped
	}

	public class ItemInfo
	{
		public string name;
		public List<Ability> abilities;
		public int constant;
		public ItemType type;
		public Sprite sprite;
		public int price;
		public string description;
		public int defense;
		public int magicDefense;

		public ItemInfo(string _name, int _price, ItemType _type, string _description) : this(_name, _price, new List<Ability>(){}, 0, 0 , 0, _type, _description) { }
		public ItemInfo(string _name, int _price, int _defense, int _magicDefense, ItemType _type, string _description) : this(_name, _price, new List<Ability>() {}, 0, _defense, _magicDefense, _type, _description) { }

		public ItemInfo(string _name, int _price, Ability _ability, int _constant,  ItemType _type, string _description) : this(_name, _price, new List<Ability>() { _ability }, 0, 0, _constant, _type, _description) { }
		public ItemInfo(string _name, int _price, Ability _ability, int _constant, int _defense, int _magicDefense, ItemType _type, string _description) : this(_name, _price, new List<Ability>() { _ability }, _defense, _magicDefense, _constant, _type, _description) { }

		public ItemInfo(string _name, int _price, List<Ability> _ability, int _constant, int _bonusDefense, int _bonusMagicDefense, ItemType _type, string _description)
		{
			name = _name;
			price = _price;
			abilities = _ability;
			defense = _bonusDefense;
			magicDefense = _bonusMagicDefense;
			constant = _constant;
			type = _type;

			_description = _description.Replace(ITEM_CONSTANT, _constant.ToString());
			_description = _description.Replace(ITEM_DEFENSE, _bonusDefense.ToString());
			_description = _description.Replace(ITEM_MAGIC_DEFENSE, _bonusMagicDefense.ToString());
			_description = _description.Replace(ITEM_ABILITY_LIST, GetAbilityNameList(abilities));
			description = _description;

			string _path = "Sprites/Items/AnimationTexts/";
			string _standardizedName = _name.ToLower().Replace(" ", "_");
			string _extention = "_sprite_index.txt";
			var textAssetContaintinSpriteData = Resources.Load<TextAsset>(_path + _standardizedName + _extention);
			
			if (textAssetContaintinSpriteData == null)
				textAssetContaintinSpriteData = AnimationTextParser.GetNewTextAssetOrAddNewToAssetDatabase("Assets/Resources/" + _path + _standardizedName + _extention);

			var _sprites = AnimationTextParser.ParseDocument(textAssetContaintinSpriteData, AnimationTextParser.Type.effect);
			sprite = (_sprites != null)? (_sprites.Length > 0)? _sprites[0]: null: null; //if the spritesheet is found, but no sprites are given
		}

		string GetAbilityNameList(List<Ability> _abilities)
		{
			string _abilityNamesToString = "";
			for (int i = 0; i < _abilities.Count; i++)
			{
				_abilityNamesToString += _abilities[i].name;
				if (i + 1 < _abilities.Count) _abilityNamesToString += ", ";
			}
			return _abilityNamesToString;

		}
	}
}


public class ItemDrop
{
	public ItemDrop(float _permilleToDrop, Items.ItemInfo _item, int _maxCount = 1, int _minCount = 1)
	{
		permille = _permilleToDrop;
		item = _item;
		maxCount = _maxCount;
		minCount = _minCount;
	}

	public float permille;
	public Items.ItemInfo item;
	public int maxCount;
	public int minCount;
}
