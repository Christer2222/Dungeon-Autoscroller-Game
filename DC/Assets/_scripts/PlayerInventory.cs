using AbilityInfo;
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

    private GameObject inventoryItemEntryPrefab;
    private float itemDropPrefabHeight;
    private float itemSpacing;

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
        public RawImage selectionBox;
        public Text title;
        public GameObject entryGameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        inventoryItemEntryPrefab = Resources.Load<GameObject>("Prefabs/InventoryItemEntry");
        itemDropPrefabHeight = inventoryItemEntryPrefab.GetComponent<RectTransform>().rect.height;

        halfItemContextHeight = UIController.InventoryContextMenu.GetComponentInChildren<Image>().rectTransform.rect.height/2;
        itemSpacing = UIController.InventoryItemContent.GetComponent<VerticalLayoutGroup>().spacing;


        UIController.InventoryButton.onClick.AddListener(delegate {
            OpenInventory();
        });

        UIController.InventoryCloseButton.onClick.AddListener(delegate {
            if (selectedItem != null)
                selectedItem.selectionBox.color = Color.clear;

            selectedItem = null;
            UIController.InventoryContextMenu.gameObject.SetActive(false);
            UIController.InventoryRootRectTransform.gameObject.SetActive(false);
        });

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


        //Info about the drops after battle
        dropList = UIController.ItemDropListGameObject;
        itemInfoGameObjects = new ItemInfoGameObject[dropList.childCount];
        for (int i = 0; i < dropList.childCount; i++)
        {
            var child = dropList.GetChild(i);
            itemInfoGameObjects[i] = new ItemInfoGameObject() { icon = child.GetComponentInChildren<Image>(), text = child.GetComponentInChildren<Text>()};
        }

        UIController.InventoryUseButton.onClick.AddListener(() => {  
            if (selectedItem !=  null)
            {
                switch (selectedItem.item.type)
                {
                    case Items.ItemType.Consumable:
                    {
                        UIController.InventoryRootRectTransform.gameObject.SetActive(false);
                        for (int i = 0; i < selectedItem.item.abilities.Count; i++)
                        {
                            print(selectedItem.item.constant);
                            //CombatController.playerCombatController.selectedAbility = selectedItem.item.abilities[i];
                            TargetData targetData = new TargetData(
                                selectedItem.item.abilities[i],
                                null,
                                CombatController.playerCombatController, 
                                selectedItem.item.constant,
                                selectedItem.item.abilities[i].element,
                                CombatController.playerCombatController.transform.position
                                );

                            CombatController.playerCombatController.StartCoroutine(CombatController.playerCombatController.SimpleInvokeAbility(targetData, selectedItem.item.abilities[i],false,true));// CombatController.playerCombatController.InvokeActiveAbility(false));
                        }
                    }
                    break;
                }
            }

        });
        UIController.InventoryEquipButton.onClick.AddListener(() => print("equiped"));

        UIController.InventoryTossButton.onClick.AddListener(() => { 
            if (selectedItem != null) 
            { 
                selectedItem.count -= GetTossValue();
                if (selectedItem.count <= 0)
                {
                    Destroy(selectedItem.entryGameObject);
                    inventory.Remove(selectedItem);
                    selectedItem = null;
                    UIController.InventoryContextMenu.gameObject.SetActive(false);
                    return;
                }
                selectedItem.title.text = GetItemText(selectedItem); 
            }
        });
        UIController.InventoryTossUp1Button.onClick.AddListener(() => ChangeTossNumber(+1)) ;
        UIController.InventoryTossUp10Button.onClick.AddListener(   () => ChangeTossNumber(+10));
        UIController.InventoryTossDown1Button.onClick.AddListener(  () => ChangeTossNumber(-1));
        UIController.InventoryTossDown10Button.onClick.AddListener( () => ChangeTossNumber(-10));

        UIController.InventoryContextMenu.gameObject.SetActive(false);

        //debug
        //OpenInventory();
    }

    string GetItemText(ItemQuantity entry)
    {
        return $"{entry.item.name} x{entry.count}";
    }

    void OpenInventory()
    {
        UIController.InventoryRootRectTransform.gameObject.SetActive(true);
        ClearInventory();
        UpdateItemList();
    }

    void ChangeTossNumber(int amount)
    {
        int newNum = Mathf.Clamp(GetTossValue() + amount, 0, 99);
        UIController.InventoryTossInputField.text = newNum.ToString("00");
    }

    int GetTossValue()
    {
        int newNum = int.TryParse(UIController.InventoryTossInputField.text, out newNum) ? newNum : 0;
        return newNum;
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
        UIController.InventoryItemContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (itemDropPrefabHeight + itemSpacing) * inventory.Count - 5);
        for (int i = 0; i < inventory.Count; i++)
        {
            GameObject go = Instantiate(inventoryItemEntryPrefab, UIController.InventoryItemContent);
            go.name = i + " " + inventory[i].item.name;
            go.GetComponentInChildren<Image>().sprite = inventory[i].item.sprite;
            inventory[i].title = go.GetComponentInChildren<Text>();
            inventory[i].title.text = GetItemText(inventory[i]);
            inventory[i].selectionBox = go.GetComponent<RawImage>();
            inventory[i].entryGameObject = go;

            //reference by value, otherwise all items would point to out of range
            go.GetComponent<Button>().onClick.AddListener(() => {
                int index = go.transform.GetSiblingIndex();

                if (selectedItem != null)
                    selectedItem.selectionBox.color = Color.clear;

                selectedItem = inventory[index];
                selectedItem.selectionBox.color = Color.white;

                var contextMenu = UIController.InventoryContextMenu; //shortcut
                Vector3 orgPos = contextMenu.position; //get orgPos position of menu
                Vector3 targetPos = go.transform.position; //where to put this
                targetPos.x = contextMenu.position.x; //keep same x of menu
                contextMenu.position = targetPos; //update placement (target pos has a different bounds, and can't be used with ancoredPosition)

                orgPos = contextMenu.anchoredPosition; //get new ancored position
                orgPos.y = Mathf.Clamp(orgPos.y, UIController.InventoryRootRectTransform.rect.y + halfItemContextHeight, -UIController.InventoryRootRectTransform.rect.y - halfItemContextHeight); //clamp within bounds of inventory
                contextMenu.anchoredPosition = orgPos; //update position again

                contextMenu.gameObject.SetActive(true);
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
                count = Random.Range(item.minCount, item.maxCount + 1),
                selectionBox = null,
                title = null,
                entryGameObject = null
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
