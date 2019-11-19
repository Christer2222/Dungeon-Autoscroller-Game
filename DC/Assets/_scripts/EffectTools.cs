using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectTools : MonoBehaviour
{
	private static bool initialized;
	private static Dictionary<string,Sprite> effectDictionary;


	private static void Initialize()
	{
		initialized = true;
		if (effectDictionary == null)
		{
			effectDictionary = new Dictionary<string,Sprite>();

			var _sprites = Resources.LoadAll<Sprite>("Sprites/Effects");

			for (int i = 0; i < _sprites.Length; i++)
			{
				effectDictionary.Add(_sprites[i].name,_sprites[i]);
			}
		}
	}

	public static IEnumerator BlinkImage(SpriteRenderer _targetSprite, Color _tempColor, float _sec, float _times)
	{
		Color _orgColor = _targetSprite.color;
		float _counter = 0;

		while (_counter < _sec)
		{
			_counter += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);

			float _pingPongTime = Mathf.PingPong((_counter  / _sec) * (_times * 2),1); //goes from 0 to 1 in order to average later. _counter/_sec gives a normalized time, and multiplying that by (times*2) means it will get to 1 "times" times

			//By skew averaging the colors, pingpong from one to the other
			_targetSprite.color = new Color(
				(_orgColor.r * (1 - _pingPongTime) + _pingPongTime * _tempColor.r),
				(_orgColor.g * (1 - _pingPongTime) + _pingPongTime * _tempColor.g),
				(_orgColor.b * (1 - _pingPongTime) + _pingPongTime * _tempColor.b),
				(_orgColor.a * (1 - _pingPongTime) + _pingPongTime * _tempColor.a)
				);
		}
	}

	public static IEnumerator PingPongSize(Transform _targetTransform,Vector3 _startSize, Vector3 _pongSize, float _sec, float _times)
	{
		_targetTransform.localScale = _startSize;
		float _counter = 0;
		while (_counter < _sec)
		{
			_counter += Time.deltaTime;
			yield return new WaitForEndOfFrame();//WaitForSeconds(Time.deltaTime);

			float _pingPongTime = Mathf.PingPong((_counter / _sec) * (_times * 2),1); //goes from 0 to 1 in order to average later. _counter/_sec gives a normalized time, and multiplying that by (times*2) means it will get to 1 "times" times
			print(_targetTransform.localScale * _pingPongTime);
			_targetTransform.localScale = _startSize + _pongSize * _pingPongTime;
			//_targetTransform.localScale = _targetTransform.localScale * (1 - _pingPongTime) + _targetTransform.localScale * _pingPongTime;
		}
	}

	public static SpriteRenderer SpawnEffect(string _name, Vector3 _worldPos, float _destroyTime = 0, int _layerIndex = 10)
	{
        if (!initialized)
			Initialize();

        //print("wp: " + _worldPos + " pccp: " + CombatController.playerCombatController.transform.position.z);
        //if (_worldPos.z < CombatController.playerCombatController.transform.position.z + 1)
        //    _worldPos = CombatController.playe

		if(effectDictionary.ContainsKey(_name))
		{
			var _go = new GameObject();
			var _sr = _go.AddComponent<SpriteRenderer>();
			_go.transform.position = _worldPos;
			_sr.sprite = effectDictionary[_name];
			_sr.sortingOrder = _layerIndex;

			//var _s = Instantiate(effectDictionary[_name], _worldPos,Quaternion.identity);
			if(_destroyTime > 0) Destroy(_go,_destroyTime);
			return _sr;
		}
		else
			Debug.LogError("No effect in dictionary with name: " + _name);

		return null;
	}
}
