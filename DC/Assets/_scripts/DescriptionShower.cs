using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DescriptionShower : MonoBehaviour, IPointerEnterHandler
{
    private static RectTransform background;
    private static Text description;
    
    private string myText;

    // Start is called before the first frame update
    void Start()
    {
        print("on");
    }

    // Update is called once per frame
    void Update()
    {
        print(EventSystem.current.IsPointerOverGameObject());//.currentSelectedGameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //do stuff
    }
}
