using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
	private StatBlock myStats;

    // Start is called before the first frame update
    void Start()
    {
		myStats = GetComponent<CombatController>().myStats;

		SpriteRenderer _rend = GetComponentInChildren<SpriteRenderer>();
		Image _img = GetComponentInChildren<Image>();
		
		if (_img != null)
			StartCoroutine(Animate(_img));
		else if (_rend != null)
			StartCoroutine(Animate(_rend));
	}

	/*
	int Counter(int[] _numbers, int index)
	{
		index++;
		index %= _numbers.Length;

		return _numbers[index];
	}
	*/

	IEnumerator Animate(SpriteRenderer _targetRenderer)
	{
		int _index = 0;
		while (true)
		{
			_index++;
			_index %= myStats.idleAnimation.Length;
			_targetRenderer.sprite = myStats.idleAnimation[_index];
			yield return new WaitForSeconds(0.2f);
		}
	}

	IEnumerator Animate(Image _targetImage)
	{
		int _index = 0;
		while (true)
		{
			_index++;
			_index %= myStats.idleAnimation.Length;
			_targetImage.sprite = myStats.idleAnimation[_index];
			yield return new WaitForSeconds(0.2f);
		}
	}
}
