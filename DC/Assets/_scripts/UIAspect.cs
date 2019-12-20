using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAspect : MonoBehaviour
{

	// Start is called before the first frame update
	void Start()
    {
		var _aspectFloat = Camera.main.aspect;
		if (_aspectFloat >= 1)
		{
			RectTransform _abRec = GameObject.Find("$AbilityButton").GetComponent<RectTransform>();
			SetApectUI(_abRec, 0);

			RectTransform _itRec = GameObject.Find("$ItemsButton").GetComponent<RectTransform>();
			SetApectUI(_itRec, 1);

			RectTransform _flRec = GameObject.Find("$FleeButton").GetComponent<RectTransform>();
			SetApectUI(_flRec, 2);

			RectTransform _opRec = GameObject.Find("$OptionsButton").GetComponent<RectTransform>();
			SetApectUI(_opRec, 3);

			RectTransform _plRec = GameObject.Find("$PlayerPortrait").GetComponent<RectTransform>();
			_plRec.anchorMax = new Vector2(0, 0);
			_plRec.anchorMin = new Vector2(0, 0);
			_plRec.offsetMin = new Vector2(960, 0);
			_plRec.offsetMax = new Vector2(1216, 256);
			//SetApectUI(_plRec, 4);
			/*
			_plRec.anchorMax = new Vector2(1, 0);
			_plRec.anchorMin = new Vector2(1, 0);
			_plRec.offsetMin = new Vector2(-256, 0);
			_plRec.offsetMax = new Vector2(0, 256);
			*/
			RectTransform _hpRec = GameObject.Find("$HealthSlider").GetComponent<RectTransform>();
			_hpRec.anchorMin = new Vector2(0, 0);
			_hpRec.anchorMax = new Vector2(1, 0);
			_hpRec.offsetMin = new Vector2(Camera.main.scaledPixelWidth* 1.06f, 128);
			_hpRec.offsetMax = new Vector2(0, 256);
			print(_plRec.localPosition.x);
			/*
			_hpRec.anchorMin = new Vector2(1, 0);
			_hpRec.anchorMax = new Vector2(1, 0);
			_hpRec.offsetMin = new Vector2(_opRec.localPosition.x - _hpRec.localPosition.x, 128);
			_hpRec.offsetMax = new Vector2(-256, 256);
			*/
			RectTransform _mpRec = GameObject.Find("$ManaSlider").GetComponent<RectTransform>();
			_mpRec.anchorMin = _hpRec.anchorMin;
			_mpRec.anchorMax = _hpRec.anchorMax;
			_mpRec.offsetMin = new Vector2(_hpRec.offsetMin.x, 0);
			_mpRec.offsetMax = new Vector2(_hpRec.offsetMax.x, 128);
			/*
			_mpRec.anchorMin = _hpRec.anchorMin;
			_mpRec.anchorMax = _hpRec.anchorMax;
			_mpRec.offsetMin = new Vector2(_opRec.localPosition.x - _mpRec.localPosition.x, 0);
			_mpRec.offsetMax = new Vector2(-256, 128);
			*/
		}
	}

	void SetApectUI(RectTransform _recTrans, int _index)
	{
		var _baseOffset = new Vector2(30, 30);
		var _buttonRect = new Vector2(_recTrans.rect.width, _recTrans.rect.height);

		_recTrans.anchorMax = Vector2.zero;
		_recTrans.anchorMin = Vector2.zero;
		_recTrans.offsetMin = new Vector2(_baseOffset.x * (_index + 1) + (_buttonRect.x) * _index, _baseOffset.y);
		_recTrans.offsetMax = new Vector2(_baseOffset.x * (_index + 1) + (_buttonRect.x) * (_index + 1), _baseOffset.y + _buttonRect.y);
	}
}
