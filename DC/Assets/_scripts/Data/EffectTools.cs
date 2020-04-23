using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class EffectTools// : MonoBehaviour
{
	private static bool initialized;
	//private static Dictionary<string, Sprite> effectDictionary;
	private static readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
	private static readonly WaitForSeconds waitForPointOneSeconds = new WaitForSeconds(0.1f);
	private static GameObject textHolder;
	private static Dictionary<string, Sprite[]> effectDictionary = new Dictionary<string, Sprite[]>();


	public static void AddToEffectDictionary(string _key, Sprite[] _value)
	{
		if (!effectDictionary.ContainsKey(_key))
			effectDictionary.Add(_key, _value);
		else
			Debug.LogWarning("The effectDictionary already contains the key: " + _key);
	}

	public struct FunctionAndDelay
	{
		public FunctionAndDelay(IEnumerator _funtion, float _delay) : this(new List<IEnumerator>() { _funtion }, _delay) { }

		public FunctionAndDelay(List<IEnumerator> _funtions, float _delay)
		{
			functions = _funtions;
			_secToStart = _delay;
		}

		public List<IEnumerator> functions;
		public float _secToStart;
	}

	private static void Initialize()
	{
		initialized = true;

		//if (waitForEndOfFrame == null)
		//	waitForEndOfFrame = new WaitForEndOfFrame();

		if (textHolder == null)
			textHolder = Resources.Load<GameObject>("Prefabs/$TextHolder");

		//if (ed == null)//effectDictionary == null)
		//{
		//	effectDictionary = new Dictionary<string, Sprite>();

		//	var _sprites = Resources.LoadAll<Sprite>("Sprites/Effects");

		//	for (int i = 0; i < _sprites.Length; i++)
		//	{
		//		effectDictionary.Add(_sprites[i].name, _sprites[i]);
		//	}
		//}
	}

	static IEnumerator Delay(float _sec)
	{
		float _timer = 0;
		while (_timer < _sec)
		{
			yield return waitForEndOfFrame;
			_timer += Time.deltaTime;
		}
	}

	public static IEnumerator AnimateSlider(Slider _sliderToMove, Slider _targetSlider, float _speed, float _ratio)
	{
		yield return AnimateSlider(_sliderToMove, _targetSlider.value, _speed, _ratio);
	}

	public static IEnumerator AnimateSlider(Slider _sliderToMove, float _value, float _speed, float _ratio)
	{
		
		float _timer = 0;
		while (_timer < _speed)
		{
			yield return waitForEndOfFrame;
			_timer += Time.deltaTime;
			//if (_sliderToMove.maxValue != 15) print($"sliderVal: {_sliderToMove.value}, max: {_sliderToMove.maxValue} target: {_value * _ratio} at speed: {_timer / _speed}");
			_sliderToMove.value = Mathf.Lerp(_sliderToMove.value, _value * _ratio, _timer/_speed);
		}
	}

	public static IEnumerator ChangeTextAndReturn(Text _text, string _originalString, string _targetString, float _secToChangeBack)
	{
		_text.text = _targetString;

		yield return Delay(_secToChangeBack);

		_text.text = _originalString;
	}

	public static IEnumerator BlinkText(Text _text, Color _tempColor, float _sec)
	{
		Color _orgColor = _text.color;

		while (_sec > 0)
		{
			_text.color = (_text.color == _orgColor) ? _tempColor : _orgColor;
			yield return waitForPointOneSeconds;
			_sec -= 0.1f;
		}

		_text.color = _orgColor;
	}

	public static IEnumerator BlinkImage(SpriteRenderer _targetSprite, Color _tempColor, float _sec, float _times)
	{ 		
		Color _orgColor = _targetSprite.color;
		float _counter = 0;

		while (_counter < _sec)
		{
			_counter += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);

			float _pingPongTime = Mathf.PingPong((_counter / _sec) * (_times * 2), 1); //goes from 0 to 1 in order to average later. _counter/_sec gives a normalized time, and multiplying that by (times*2) means it will get to 1 "times" times

			//By skew averaging the colors, pingpong from one to the other
			_targetSprite.color = new Color(
				(_orgColor.r * (1 - _pingPongTime) + _pingPongTime * _tempColor.r),
				(_orgColor.g * (1 - _pingPongTime) + _pingPongTime * _tempColor.g),
				(_orgColor.b * (1 - _pingPongTime) + _pingPongTime * _tempColor.b),
				(_orgColor.a * (1 - _pingPongTime) + _pingPongTime * _tempColor.a)
				);
		}
	}

	public static IEnumerator PingPongSize(Transform _targetTransform, Vector3 _startSize, Vector3 _pongSize, float _sec, float _times)
	{
		_targetTransform.localScale = _startSize;
		float _counter = 0;
		while (_counter < _sec)
		{
			_counter += Time.deltaTime;
			yield return new WaitForEndOfFrame();//WaitForSeconds(Time.deltaTime);

			float _pingPongTime = Mathf.PingPong((_counter / _sec) * (_times * 2), 1); //goes from 0 to 1 in order to average later. _counter/_sec gives a normalized time, and multiplying that by (times*2) means it will get to 1 "times" times
			//print(_targetTransform.localScale * _pingPongTime);
			_targetTransform.localScale = _startSize + _pongSize * _pingPongTime;
			//_targetTransform.localScale = _targetTransform.localScale * (1 - _pingPongTime) + _targetTransform.localScale * _pingPongTime;
		}
	}

	public static SpriteRenderer SpawnEffect(Sprite _sprite, Vector3 _worldPos, float _destroyTime = 0, int _layerIndex = 10, string _sortingLayer = "UI")
	{
		if (!initialized)
			Initialize();

		if (_sprite != null)
		{
			var _go = new GameObject();
			var _sr = _go.AddComponent<SpriteRenderer>();
			_go.transform.position = _worldPos;
			_sr.sprite = _sprite; //effectDictionary[_name];
			_sr.sortingLayerName = _sortingLayer;
			_sr.sortingOrder = _layerIndex;

			//var _s = Instantiate(effectDictionary[_name], _worldPos,Quaternion.identity);
			if (_destroyTime > 0) Object.Destroy(_go, _destroyTime);
			return _sr;
		}
		else
		{
			Debug.LogError("Sprite attempted to be spawned was null");
			return null;
		}
	}

	public static SpriteRenderer SpawnEffect(string _name, Vector3 _worldPos, float _destroyTime = 0, int _layerIndex = 10, string _sortingLayer = "UI")
	{
		if (!initialized)
			Initialize();

		//print("wp: " + _worldPos + " pccp: " + CombatController.playerCombatController.transform.position.z);
		//if (_worldPos.z < CombatController.playerCombatController.transform.position.z + 1)
		//    _worldPos = CombatController.playe
		//_name = _name.ToLower();
		if (effectDictionary.ContainsKey(_name))//effectDictionary.ContainsKey(_name))
		{
			var _go = new GameObject();
			var _sr = _go.AddComponent<SpriteRenderer>();
			_go.transform.position = _worldPos;
			_sr.sprite = effectDictionary[_name][0]; //effectDictionary[_name];
			_sr.sortingLayerName = _sortingLayer;
			_sr.sortingOrder = _layerIndex;

			//var _s = Instantiate(effectDictionary[_name], _worldPos,Quaternion.identity);
			if (_destroyTime > 0) Object.Destroy(_go, _destroyTime);
			return _sr;
		}
		else
		{
			Debug.LogError($"No effect in dictionary with name: {_name}");
		}

		return null;
	}

	public static IEnumerator ActivateInOrder(MonoBehaviour _holder, List<FunctionAndDelay> _functionAndDelay)
	{
		for (int i = 0; i < _functionAndDelay.Count; i++)
		{
			yield return new WaitForSeconds(_functionAndDelay[i]._secToStart);
			for (int j = 0; j < _functionAndDelay[i].functions.Count; j++)
			{
				_holder.StartCoroutine(_functionAndDelay[i].functions[j]);
			}
		}
	}

	public static IEnumerator Shake(Transform _target, float _magnitude, int _amount)
	{
		for (int i = 0; i < _amount; i++)
		{
			float _ratrio = 1f - ((float)i /_amount);
			yield return MoveDirection(_target, Vector3.left, _magnitude * 10f * _ratrio, 0.01f);
			yield return new WaitForSeconds(0.1f);
			yield return MoveDirection(_target, Vector3.right, _magnitude * 15f * _ratrio, 0.01f);
			yield return new WaitForSeconds(0.1f);
			yield return MoveDirection(_target, Vector3.left, _magnitude * 5f * _ratrio, 0.01f);
			yield return null;
		}
	}

	public static IEnumerator StretchFromTo(Transform _target, Vector3 _startVector, Vector3 _endVector, float _time)
	{
		_target.transform.localScale = _startVector;
		yield return null;

		float _life = 0;
		var _current = _startVector;

		while (_life <= _time)
		{
			_life = Mathf.Min(_life + Time.deltaTime, _time);
			yield return waitForEndOfFrame;
			_current = Vector3.Lerp(_current,_endVector,_life/ _time);

			_target.transform.localScale = _current;
		}

	}

	public static Text SpawnText(Vector3 _pos, Transform _parent, Color _color, string _text, int _fontSize = 70)
	{
		if (!initialized)
			Initialize();

		var _go = Object.Instantiate(textHolder, null);
		var _goText = _go.GetComponentInChildren<Text>();

		_go.transform.localScale = Vector3.one * 0.005f;
		_go.transform.SetParent(_parent);
		_go.transform.position = _pos;// + Vector3.back * 0.01f;
		_goText.text = _text;
		_goText.color = _color;
		_goText.fontSize = _fontSize;
		return _goText;
	}

	public static IEnumerator MoveDirection(Transform _t, Vector3 _direction, float _speed, float _life)
	{
		while (_life > 0)
		{
			_life -= Time.deltaTime;
			_t.transform.position += _direction * _speed * Time.deltaTime;
			yield return waitForEndOfFrame;
		}
	}

	public static IEnumerator CurveDropMove(Transform _t, float _life, float _magnitude = 1, float _speed = 1)
	{
		Vector3 _direction = Vector3.up * (Random.Range(1f,2f)) + Vector3.right * (Random.Range(0f,1f) * 2 - 1);
		while (_life > 0)
		{
			_t.transform.position += _direction * Time.deltaTime * _magnitude * _speed;
			_direction.y -= 0.1f * _speed;
			yield return waitForEndOfFrame;
			_life -= Time.deltaTime;
		}
	}

	public static IEnumerator Wobble(Transform _t, float _magnitude, float _speed, float _life)
	{
		float _time = 0;

		while (_life > _time)
		{
			_time += Time.deltaTime;
			_t.position += _t.up * ((Mathf.Cos(_time) * Time.deltaTime * _magnitude));//_startPos + Vector3.up * Mathf.Sin(_startTime);
			yield return waitForEndOfFrame;
		}

		yield return null;
	}

	public static IEnumerator DeactivateGameObject(GameObject _go, float _time)
	{
		yield return new WaitForSeconds(_time);
		_go.SetActive(false);
	}
}
