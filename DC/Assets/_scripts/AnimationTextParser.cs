using UnityEngine;
using System;

public class AnimationTextParser
{
	public enum Type
	{
		enemy,
		effect,
	}

	private static Sprite[] enemySpriteArray;
	private static Sprite[] effectSpriteArray;

	public static TextAsset GetNewTextAssetOrAddNewToAssetDatabase(string _pathWithExtention)
	{
		var _textAsset = new TextAsset("0");
#if UNITY_EDITOR
		var _storedAsset = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(_pathWithExtention); //figure out if there is an empty
		if (_storedAsset.Length == 0) //if not create it
		{
			Debug.LogWarning("created: " + _pathWithExtention);
			UnityEditor.AssetDatabase.CreateAsset(_textAsset, _pathWithExtention);
			UnityEditor.AssetDatabase.Refresh();
			Debug.Log(_textAsset);
		}
		else
			_textAsset = (TextAsset)_storedAsset[0]; //if there is an empty, use it
#endif
		return _textAsset;
	}

	public static Sprite[] ParseDocument(TextAsset _textAsset, Type _type)
	{
		if (_textAsset == null)
		{
			Debug.Log("text was null");
			return null;
		}

		if (enemySpriteArray == null) enemySpriteArray = Resources.LoadAll<Sprite>("Sprites/Enemies/EnemySpriteSheet");
		if (effectSpriteArray == null) effectSpriteArray = Resources.LoadAll<Sprite>("Sprites/Effects/effectSpriteSheet");


		string[] _entries = _textAsset.text.Split(new string[]{Environment.NewLine},StringSplitOptions.RemoveEmptyEntries);
		Sprite[] _result = new Sprite[_entries.Length];

		//Debug.Log(" lenght: " + _result.Length + " ");

		Sprite[] _targetArray = null;
		switch (_type)
		{
			case Type.enemy:
				_targetArray = enemySpriteArray;
				break;
			case Type.effect:
				_targetArray = effectSpriteArray;
				break;
		}


		for (int i = 0; i < _entries.Length; i++)
		{
			if (_entries[i].Contains("_"))
			{
				_entries[i] = _entries[i].Remove(0, _entries[i].IndexOf("_") + 1);
			}

			_result[i] = _targetArray[int.TryParse(_entries[i], out int x)? x : 0];
		}
		/*
		string _all = string.Empty;
		for (int i = 0; i < _entries.Length; i++)
		{
			_all += "| " + _entries[i];
		}
		Debug.Log(_all);
		*/
		return _result;
	}
}
