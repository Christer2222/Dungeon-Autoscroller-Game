using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    public List<ItemQuantity> inventory = new List<ItemQuantity>();
    private ItemQuantity selectedItem;


    public Transform dropList;
    private ItemInfoGameObject[] itemInfoGameObjects;

    private GameObject itemDropPrefab;
    private float itemDropPrefabHeight;

    private float halfItemContextHeight;

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
        itemDropPrefab = Resources.Load<GameObject>("Prefabs/InventoryItemEntry");
        itemDropPrefabHeight = itemDropPrefab.GetComponent<RectTransform>().rect.height;

        halfItemContextHeight = UIController.InventoryContextMenu.GetComponentInChildren<Image>().rectTransform.rect.height/2;

        ItemQuantity[] debugItems =
            {
            new ItemQuantity() { count = 5, item = Items.apple },
            new ItemQuantity() { count = 5, item = Items.orange },
            new ItemQuantity() { count = 5, item = Items.apple },
            new ItemQuantity() { count = 5, item = Items.orange },
            new ItemQuantity() { count = 5, item = Items.orange },
            new ItemQuantity() { count = 5, item = Items.orange },
            new ItemQuantity() { count = 5, item = Items.orange },
            new ItemQuantity() { count = 5, item = Items.orange },
            new ItemQuantity() { count = 5, item = Items.orange },
            new ItemQuantity() { count = 5, item = Items.orange },
            new ItemQuantity() { count = 5, item = Items.orange },
            new ItemQuantity() { count = 5, item = Items.orange },
            new ItemQuantity() { count = 5, item = Items.banana }
        };

        inventory.AddRange(debugItems);

        instance = this;
        dropList = UIController.ItemDropListGameObject;

        itemInfoGameObjects = new ItemInfoGameObject[dropList.childCount];
        for (int i = 0; i < dropList.childCount; i++)
        {
            var child = dropList.GetChild(i);
            itemInfoGameObjects[i] = new ItemInfoGameObject() { icon = child.GetComponentInChildren<Image>(), text = child.GetComponentInChildren<Text>() };
        }

        ClearInventory();
        UpdateItemList();

        UIController.InventoryUseButton.onClick.AddListener(() => print("used"));
        UIController.InventoryEquipButton.onClick.AddListener(() => print("equiped"));
        UIController.InventoryTossButton.onClick.AddListener(() => print("tossed " + UIController.InventoryTossInputField.text));
        UIController.InventoryTossUp1Button.onClick.AddListener(() => ChangeTossNumber(+1)) ;
        UIController.InventoryTossUp10Button.onClick.AddListener(   () => ChangeTossNumber(+10));
        UIController.InventoryTossDown1Button.onClick.AddListener(  () => ChangeTossNumber(-1));
        UIController.InventoryTossDown10Button.onClick.AddListener( () => ChangeTossNumber(-10));
        
    }

    void ChangeTossNumber(int amount)
    {
        int newNum = int.TryParse(UIController.InventoryTossInputField.text, out newNum) ? newNum : 0;
        newNum = Mathf.Clamp(newNum + amount, 0, 99);
        UIController.InventoryTossInputField.text = newNum.ToString("00");
    }

    void ClearInventory()
    {
        for (int i = 0; i < UIController.InventoryItemContent.childCount; i++)
        {
            Destroy(UIController.InventoryItemContent.GetChild(i).gameObject);
        }
    }

    void UpdateItemList()
    {
        UIController.InventoryItemContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (itemDropPrefabHeight + 20) * inventory.Count - 5);
        for (int i = 0; i < inventory.Count; i++)
        {
            GameObject go = Instantiate(itemDropPrefab, UIController.InventoryItemContent);
            go.name = i + " " + inventory[i].item.name;
            go.GetComponentInChildren<Image>().sprite = inventory[i].item.sprite;
            go.GetComponentInChildren<Text>().text = $"{inventory[i].item.name} x{inventory[i].count}";
            int index = i; //reference by value, otherwise all items would point to out of range
            go.GetComponent<Button>().onClick.AddListener(() => {

                selectedItem = inventory[index];
                Vector3 pos = go.transform.position;
                pos.x = 0;
                //pos.y = Mathf.Clamp(pos.y,0 + halfItemContextHeight,1920 - halfItemContextHeight)/100;
                
                UIController.InventoryContextMenu.position = pos;
                print("clcik item " + selectedItem.item.name);
                print("halfItemContextHeight: " + halfItemContextHeight);
            });
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
