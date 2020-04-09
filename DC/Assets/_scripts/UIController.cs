using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{
    public static Transform ItemDropListGameObject { get; private set; }
    
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
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
