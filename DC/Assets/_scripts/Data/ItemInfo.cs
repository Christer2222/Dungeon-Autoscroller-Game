using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityInfo;

public class Items : AbilityClass
{
	public static ItemInfo apple = new ItemInfo(heal, 2, 0, ItemType.Consumable);
	public static ItemInfo orange = new ItemInfo(heal, 4, 1, ItemType.Consumable);
	public static ItemInfo banana = new ItemInfo(heal, 6, 2, ItemType.Consumable);

	public enum ItemType
	{
		Garbage = 1,
		Consumable = 2,
		Consumable2 = 2,
		Consumable3 = 2,
		Value = 4,
		Crafting = 8,
		Equipment = 16,
	}

	public class ItemInfo
	{
		public Ability ability;
		public int constant;
		public int spriteIndex;
		public ItemType type;

		public ItemInfo(Ability _ability, int _constant, int _spriteIndex, ItemType _type)
		{
			ability = _ability;
			constant = _constant;
			spriteIndex = _spriteIndex;
			type = _type;

			string _path = "Sprites/Items/AnimationTexts/";
			string _standardizedName = _ability.name.ToLower().Replace(" ", "_");
			string _extention = "_sprite_index";
			var a = Resources.Load<TextAsset>(_path + _standardizedName + _extention);
			var _sprites = AnimationTextParser.ParseDocument(a, AnimationTextParser.Type.effect);
		}
	}
}


public class ItemDrop
{
	public ItemDrop(float _permilleToDrop, Items.ItemInfo _item)
	{
		permille = _permilleToDrop;
		item = _item;
	}

	public float permille;
	public Items.ItemInfo item;
}
