using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{
    public static Transform ItemDropListGameObject { get; private set; }
    public static RectTransform InventoryItemContent { get; private set; }
    
    public static RectTransform InventoryContextMenu { get; private set; }
    public static Button InventoryUseButton { get; private set; }
    public static Button InventoryEquipButton { get; private set; }
    public static Button InventoryTossButton { get; private set; }

    public static Button InventoryTossUp1Button { get; private set; }
    public static Button InventoryTossUp10Button { get; private set; }
    public static Button InventoryTossDown1Button { get; private set; }
    public static Button InventoryTossDown10Button { get; private set; }
    public static InputField InventoryTossInputField { get; private set; }



    public static Slider HealthSlider { get; private set; }
    public static Slider ManaSlider { get; private set; }

    public static MinMax<Text> HealthTexts { get; private set; } //Text min, max;
    public static MinMax<Text> ManaTexts { get; private set; } //Text min, max;


    public class MinMax<T>
    {
        public T minValue;
        public T maxValue;
    }

    // Start is called before the first frame update
    void Start()
    {
        var childrenTransforms = GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < childrenTransforms.Length; i++)
        {
            Transform child = childrenTransforms[i];
            switch (child.name)
            {
                case "$ItemDropList":
                    ItemDropListGameObject = child;
                    break;
                case "$ItemListContent":
                    InventoryItemContent = child.GetComponent<RectTransform>();
                    break;
                case "$ContextMenu":
                    InventoryContextMenu = child.GetComponent<RectTransform>();
                    break;
                case "$UseButton":
                    InventoryUseButton = child.GetComponent<Button>();
                    break;
                case "$EquipButton":
                    InventoryEquipButton = child.GetComponent<Button>();
                    break;
                case "$TossButton":
                    InventoryTossButton = child.GetComponent<Button>();
                    break;
                case "$TossDown10Button":
                    InventoryTossDown10Button = child.GetComponent<Button>();
                    break;
                case "$TossUp10Button":
                    InventoryTossUp10Button = child.GetComponent<Button>();
                    break;
                case "$TossDown1Button":
                    InventoryTossDown1Button = child.GetComponent<Button>();
                    break;
                case "$TossUp1Button":
                    InventoryTossUp1Button = child.GetComponent<Button>();
                    break;
                case "$TossInputField":
                    InventoryTossInputField = child.GetComponent<InputField>();
                    break;
            }
        }
    }
}
