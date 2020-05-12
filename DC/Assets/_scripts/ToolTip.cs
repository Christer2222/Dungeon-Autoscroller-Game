using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{    
    private string toolTipString;

    private static Transform selectedGameObject;
    private RectTransform descriptionBackgroundParent;
    private Vector2 outPos;
    private Vector3 lastMousePos;

    private const float MINIMUM_SHOW_TIME = 1.0f;
    private float timer;

    private readonly Color descriptionOriginalColor = new Color(0.25f,0.25f,0.25f);

    //private Coroutine hoverCoroutine;
    private Coroutine fadeInCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        //toolTipString = ProcessString(transform.name + " ABCDEFGHIJKLMN OPQRSTUVWXYZ abcdefghijklmn opqrstuvwxyz 1234567890 !?.,:;-_");// " bl afs1 blag h2bls hjah3 vlsf ha4 bad jfg5 alds jfm6 dlst kyafd7 ba skh8 sbs kakds9 1"); // + System.DateTime.Now;
        descriptionBackgroundParent = UIController.DescriptionBackground.parent.transform as RectTransform;
    }

    public void ChangeToolTipText(string _target)
    {
        toolTipString = ProcessString(_target);
    }

    static string ProcessString(string input)
    {
        var charInput = input.ToCharArray();
        int lastSpace = 0;
        //int potentialSpace = 0;
        for (int i = 0; i < charInput.Length; i++)
        {
            /*
            if (i > lastSpace + 15)
            {
                if (charInput[i] == ' ')
                    potentialSpace = i;
            }
            */

            if (charInput[i] == '\n')//10)
            {
                lastSpace = i;
                print("\\n :" + input + lastSpace);
                //'\n')
            }

            if (i > lastSpace + 20)
            {
                //print($"{i}: {(int)charInput[i]}|");
                if (charInput[i] == ' ')
                {
                    charInput[i] = '\n';
                    lastSpace = i;
                }
            }
        }
        return new string(charInput);
    }


    IEnumerator HoverCheck()
    {
        while (true)
        {
            if (selectedGameObject != null)
            {
                timer = (lastMousePos != Input.mousePosition) ? 0 : timer + Time.deltaTime;
                lastMousePos = Input.mousePosition;


                if (timer > MINIMUM_SHOW_TIME)
                {
                    if (fadeInCoroutine == null)
                        fadeInCoroutine = StartCoroutine(EffectTools.BlinkImage(UIController.DescriptionBackgroundImage, descriptionOriginalColor, 0.1f,  0.5f));


                    UIController.DescriptionBackground.gameObject.SetActive(true);// EventSystem.current.IsPointerOverGameObject());

                    var res = Screen.safeArea;
                    var _scrn = new Vector2(res.width,res.height)/2;

                    Vector2 _offset = UIController.DescriptionBackground.rect.size / 2 + Vector2.one;
                    _offset.x *= (Input.mousePosition.x > _scrn.x) ? -1 : 1;//mouseHigh ? -1 : 1;
                    _offset.y *= (Input.mousePosition.y > _scrn.y) ? -1 : 1;//mouseRight ? -1 : 1;

                    RectTransformUtility.ScreenPointToLocalPointInRectangle(descriptionBackgroundParent, Input.mousePosition, UIController.MainCamera, out outPos);
                    UIController.DescriptionBackground.position = UIController.DescriptionBackground.parent.transform.TransformPoint(outPos + _offset);
                }
                else
                {
                    UIController.DescriptionBackground.gameObject.SetActive(false);// EventSystem.current.IsPointerOverGameObject());
                    //break; //yield return null;
                    if (fadeInCoroutine != null)
                    {
                        StopCoroutine(fadeInCoroutine);
                        fadeInCoroutine = null;
                        UIController.DescriptionBackgroundImage.color = Color.clear;
                    }    
                }            
            }
        
           yield return null;
        }
    }

    void SetSize()
    {
        UIController.DescriptionBackground.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, UIController.DescriptionText.preferredWidth + 40);// 300);
        UIController.DescriptionBackground.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, UIController.DescriptionText.preferredHeight + 10);
    }

    /*
    void SetPosition()
    {
        var res = Screen.safeArea;
        var _scrn = new Vector2(res.width, res.height) / 2;

        Vector2 _offset = UIController.DescriptionBackground.rect.size / 2 + Vector2.one;
        _offset.x *= (UIController.DescriptionBackground.anchoredPosition.x > _scrn.x) ? -1 : 1;//mouseHigh ? -1 : 1;
        _offset.y *= (UIController.DescriptionBackground.anchoredPosition.y > _scrn.y) ? -1 : 1;//mouseRight ? -1 : 1;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(descriptionBackgroundParent, UIController.DescriptionBackground.anchoredPosition, UIController.MainCamera, out outPos);
        UIController.DescriptionBackground.position = UIController.DescriptionBackground.parent.transform.TransformPoint(outPos + _offset);
    }
    */

    public void OnPointerEnter(PointerEventData eventData)
    {

        selectedGameObject = eventData.pointerCurrentRaycast.gameObject.transform;//selectedObject.transform;
        //UIController.DescriptionBackground.gameObject.SetActive(true);// EventSystem.current.IsPointerOverGameObject());
        UIController.DescriptionBackgroundImage.color = Color.clear;
        UIController.DescriptionText.text = toolTipString;

        //hoverCoroutine = StartCoroutine(HoverCheck());
        SetSize();
        StartCoroutine(HoverCheck());
    }

    public void OnPointerOver(PointerEventData eventData)
    {
        UIController.DescriptionBackground.position = eventData.selectedObject.transform.position; // UIController.MainCamera.ScreenToWorldPoint(_mousePos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIController.DescriptionBackground.gameObject.SetActive(false);// EventSystem.current.IsPointerOverGameObject());
        StopAllCoroutines();
        fadeInCoroutine = null;
        selectedGameObject = null;
        //StopCoroutine(hoverCoroutine);
    }
}
