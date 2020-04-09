using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    public List<ItemQuantity> inventory = new List<ItemQuantity>();

    public Transform dropList;
    private ItemInfoGameObject[] itemInfoGameObjects;

    class ItemInfoGameObject
    {
        public Image icon;
        public Text text;
    }

    public class ItemQuantity
    {
        public int count;
        public Items.ItemInfo item;

    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        dropList = UIController.ItemDropListGameObject;

        itemInfoGameObjects = new ItemInfoGameObject[dropList.childCount];
        for (int i = 0; i < dropList.childCount; i++)
        {
            var child = dropList.GetChild(i);
            itemInfoGameObjects[i] = new ItemInfoGameObject() { icon = child.GetComponentInChildren<Image>(), text = child.GetComponentInChildren<Text>() };
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            string s = "";
            foreach (var v in inventory)
            {
                s += v.item.name + " x" + v.count + "\n";
            }
            print(s);
        }
    }


    public void ProcessDrops(List<ItemDrop> drops, bool showDropList = true) 
    {
        List<ItemQuantity> dropsProcessed = new List<ItemQuantity>();
        foreach (var v in drops)
        {
            var d = RollForDrop(v);
            if (d != null) dropsProcessed.Add(d);
        }

        if (showDropList)
        {
            dropList.gameObject.SetActive(true);
            if (dropsProcessed.Count == 0)
            {
                itemInfoGameObjects[0].icon.transform.parent.gameObject.SetActive(true);
                itemInfoGameObjects[0].icon.sprite = null;
                itemInfoGameObjects[0].text.text = "None";
            }

            for (int i = 0; i < itemInfoGameObjects.Length; i++)
            {
                bool inBounds = i < dropsProcessed.Count;
                itemInfoGameObjects[i].icon.transform.parent.gameObject.SetActive(inBounds);
                if (inBounds)
                {
                    itemInfoGameObjects[i].icon.sprite = dropsProcessed[i].item.sprite;
                    itemInfoGameObjects[i].text.text = $"{dropsProcessed[i].item.name} x{dropsProcessed[i].count}";
                }


            }
        }

        StartCoroutine(EffectTools.DeactivateGameObject(dropList.gameObject, 3));
    }


    /// <summary>
    /// Rolls for an item, then adds it to the inventory.
    /// </summary>
    /// <param name="item">Drop to be rolled for.</param>
    ItemQuantity RollForDrop(ItemDrop item)
    {
        if (Random.Range(1, 1001) <= item.permille) //if the roll is equal to or less than the permille it was a success
        {
            var drop = new ItemQuantity() 
            { 
                item = item.item, 
                count = Random.Range(item.minCount, item.maxCount + 1) 
            }; //roll for quantity
            print($"drop [{item.item.name}]  min: {item.minCount} max: {item.maxCount} rolled {drop.count}");

            var duplicate = inventory.Find(x => x.item.name == item.item.name); //look for  a duplicate
            if (duplicate == null) //if none were found
            {
                inventory.Add(drop); //add to the inventory the rolled one
            }
            else
                duplicate.count += drop.count; //add the count to the duplicate

            return drop;
        }

        return null;
    }
}
