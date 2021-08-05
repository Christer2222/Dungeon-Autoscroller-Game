using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockController : MonoBehaviour
{

    [SerializeField] private UnityEngine.UI.Image dayNightGradient;
    [SerializeField] private UnityEngine.UI.Text dayCounterText;

    private const string DAY_STRING = "Day: ";
    private int dayCounter = 1;

    private Vector3 offset = new Vector3(0,0,0);


    // Start is called before the first frame update
    void Start()
    {
        dayCounterText.text = DAY_STRING + dayCounter;
    }

    // Update is called once per frame
    void Update()
    {
        offset.z += Time.deltaTime * 10;
        if (offset.z > 180)
		{
            offset.z -= 360;
            dayCounter++;
            dayCounterText.text = DAY_STRING + dayCounter;
		}
        
        dayNightGradient.gameObject.transform.eulerAngles = offset;

        //image.material.SetTextureOffset("_MainTex",new Vector2(xOffset,0));
        //sprite.sprite.textureRectOffset.Set(xOffset,0);
        //sprite.material.SetTextureOffset("_MainTex", new Vector2(xOffset,0));//.mainTextureOffset.Set(xOffset,0);
        //print(sprite.material.mainTextureOffset);
    }
}
