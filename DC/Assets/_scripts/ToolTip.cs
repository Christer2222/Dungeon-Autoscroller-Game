using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{    
    private string toolTipString = string.Empty;

    private static Transform selectedGameObject;
    private RectTransform descriptionBackgroundParent;
    private Vector2 outPos;
    private Vector3 lastMousePos;

    private const float MINIMUM_SHOW_TIME = 1.0f;
    private float timer;

    private readonly Color descriptionOriginalColor = new Color(0.25f,0.25f,0.25f);

    //private Coroutine hoverCoroutine;
    private Coroutine fadeInCoroutine;

    //public static ToolTip currentToolTip;

    // Start is called before the first frame update
    void Start()
    {
        //toolTipString = ProcessString(transform.name + " ABCDEFGHIJKLMN OPQRSTUVWXYZ abcdefghijklmn opqrstuvwxyz 1234567890 !?.,:;-_");// " bl afs1 blag h2bls hjah3 vlsf ha4 bad jfg5 alds jfm6 dlst kyafd7 ba skh8 sbs kakds9 1"); // + System.DateTime.Now;
        descriptionBackgroundParent = UIController.DescriptionBackground.parent.transform as RectTransform;
    }

    public void ChangeToolTipText(string _target)
    {
        toolTipString = ProcessString(_target);

        if (selectedGameObject != null) //if there is a selected gameobject
            if (selectedGameObject.transform == transform) //check if the transform is the same as this transform.  (Apparently gameobjects change?)
            {
                UIController.DescriptionText.text = toolTipString;
            }
    }

    static string ProcessString(string _input)
    {
        var _charInput = _input.ToCharArray();
        int _lastSpace = 0;
        //bool _escaped = false;
        //int potentialSpace = 0;
        for (int i = 0; i < _charInput.Length; i++)
        {
            /*
            if (i > lastSpace + 15)
            {
                if (charInput[i] == ' ')
                    potentialSpace = i;
            }
            */

                /*
            if (_charInput[i] == '<')
                _escaped = true;
            if (_charInput[i] == '>')
                _escaped = false;
                */

            if (_charInput[i] == '\n')//10)
            {
                _lastSpace = i;
                //'\n')
            }

            if (i > _lastSpace + 20)// && !_escaped)
            {
                //print($"{i}: {(int)charInput[i]}|");
                if (_charInput[i] == ' ')
                {
                    _charInput[i] = '\n';
                    _lastSpace = i;
                }
            }
        }

        string _result = new string(_charInput);

        _result = _result.Replace("$none", "<color=#333333>none</color>");
        _result = _result.Replace("$physical", "<color=#61737d>physical</color>");
        _result = _result.Replace("$fire", "<color=#a8270d>fire</color>");
        _result = _result.Replace("$water", "<color=#2706bd>water</color>");
        _result = _result.Replace("$earth", "<color=#654321>earth</color>");
        _result = _result.Replace("$air", "<color=#999999>air</color>");
        _result = _result.Replace("$plasma", "<color=#c712db>plasma</color>");
        _result = _result.Replace("$ice", "<color=#83d6eb>ice</color>");
        _result = _result.Replace("$poison", "<color=#367d49>poison</color>");
        _result = _result.Replace("$electricity", "<color=#fff645>electricity</color>");
        _result = _result.Replace("$steam", "<color=#b0b1d6>steam</color>");
        _result = _result.Replace("$light", "<color=#fffa78>light</color>");
        _result = _result.Replace("$unlife", "<color=#4960ab>unlife</color>");
        _result = _result.Replace("$void", "<color=#531c59>void</color>");

        return _result;
    }


    IEnumerator HoverCheck()
    {
        //if (toolTipString == string.Empty)
        //    yield break;

        while (true)
        {
            /*
            if (Input.GetMouseButtonDown(0))
            {

                UIController.DescriptionBackground.gameObject.SetActive(false);// EventSystem.current.IsPointerOverGameObject());
                fadeInCoroutine = null;

                UIController.DescriptionBackgroundImage.color = Color.clear;
                UIController.DescriptionText.text = toolTipString;

                //hoverCoroutine = StartCoroutine(HoverCheck());
                SetSize();
                yield return new WaitForEndOfFrame();
                StartCoroutine(HoverCheck());
                yield break;

                OnPointerExit(null);
                var a = new PointerEventData(EventSystem.current);
                OnPointerOver(a);//null);

                UIController.DescriptionBackground.gameObject.SetActive(false);// EventSystem.current.IsPointerOverGameObject());
                fadeInCoroutine = null;
                //selectedGameObject = null;
                StopAllCoroutines();
                
                //UIController.DescriptionBackground.gameObject.SetActive(false);// EventSystem.current.IsPointerOverGameObject());
            }
            */

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
        //currentToolTip = this;

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
        if (selectedGameObject != null)
            if (selectedGameObject.transform == transform)
            {

                UIController.DescriptionBackground.gameObject.SetActive(false);// EventSystem.current.IsPointerOverGameObject());
                StopAllCoroutines();
                fadeInCoroutine = null;
                selectedGameObject = null;
            }
        //StopCoroutine(hoverCoroutine);
    }
}
