using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    public List<ItemQuantity> inventory = new List<ItemQuantity>();
    private ItemQuantity selectedItem;

    public List<ItemQuantity> equippedItems = new List<ItemQuantity>();

    private const string USE_STRING = "Use";
    private const string EQUIP_STRING = "Equip";
    private const string IMPOSSIBLE_STRING = "Impossible";
    private const string WAIT_STRING = "Wait";




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
        public int amount;
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
            new ItemQuantity() { amount = 5, item = Items.Apple },
            new ItemQuantity() { amount = 5, item = Items.Orange },
            new ItemQuantity() { amount = 1, item = Items.GoldRing },
            new ItemQuantity() { amount = 1, item = Items.Headband },
            new ItemQuantity() { amount = 1, item = Items.SteelHelmet },


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

        #region Buttons
        //----------USE BUTTON
        UIController.InventoryUseButton.onClick.AddListener(() => {  
            if (selectedItem !=  null)
            {
                if ((selectedItem.item.type & Items.ItemType.Consumable) != 0) //if item has consumable flag
                {
                    if (CombatController.turnOrder[0] != CombatController.playerCombatController)
                    {
                        StartCoroutine(EffectTools.ChangeTextAndReturn(UIController.InventoryUseButtonText, USE_STRING, WAIT_STRING, 0.5f));
                        return; 
                    }

                    UIController.InventoryRootRectTransform.gameObject.SetActive(false);
                    UIController.InventoryContextMenu.gameObject.SetActive(false);
                    for (int i = 0; i < selectedItem.item.activeAbilities.Count; i++)
                    {
                        AbilityInfo.TargetData targetData = new AbilityInfo.TargetData(
                            selectedItem.item.activeAbilities[i], //ability
                            null, //self
                            CombatController.playerCombatController, //target 
                            selectedItem.item.activeConstants[i], //bonus
                            selectedItem.item.activeAbilities[i].element, //element
                            CombatController.playerCombatController.transform.position //where to show icon. Broken?
                            );

                        CombatController.playerCombatController.StartCoroutine(
                            CombatController.playerCombatController.SimpleInvokeAbility(targetData, selectedItem.item.activeAbilities[i],false,true, 1, true, delegate {
                                ChangeItemQuantity(selectedItem, -1); //remove an item after it is used
                            }));
                    }
                }
                else if ((selectedItem.item.type & Items.ItemType.Targetable) != 0) //if item has targetable flag
                {

                }
                else
                   StartCoroutine(EffectTools.ChangeTextAndReturn(UIController.InventoryUseButtonText, USE_STRING, IMPOSSIBLE_STRING, 0.5f));
            }

        });
        //----------EUQIP BUTTON
        UIController.InventoryEquipButton.onClick.AddListener(() => {
            
            if ((selectedItem.item.type & Items.ItemType.Equipment) != 0) //equipment has item flag
            {
                if (selectedItem.item.type == Items.ItemType.Acessory)
                { }
                else if (selectedItem.item.type == Items.ItemType.OneHanded)
                { }
                else if (selectedItem.item.type == Items.ItemType.TwoHanded)
                { }
                else
                {
                    var _prevEquiped = equippedItems.Find(x => x.item.type == selectedItem.item.type);
                    if (_prevEquiped != null)
                        print($"prevEquip: {_prevEquiped.item.name} selectedItem: {selectedItem.item.name}");

                    if (_prevEquiped != null) //if same type already equipped
                    {
                        AddItemToInventory(_prevEquiped); //add it back to the inventory
                        print("added to inventory " + _prevEquiped.item.name);
                        //InstantiateItemToInventory(_prevEquiped);
                        ChangeItemQuantity(_prevEquiped, 1);
                    }
                    
                    equippedItems.Add(selectedItem); //then add the selected item to equipped items
                    ChangeItemQuantity(selectedItem, -1); //and remove it from the inventory
                }
            }
            else
               StartCoroutine(EffectTools.ChangeTextAndReturn(UIController.InventoryEquipText, EQUIP_STRING, IMPOSSIBLE_STRING, 0.5f));
        });

        //----------TOSS BUTTONS
        UIController.InventoryTossButton.onClick.AddListener(() => { 
            if (selectedItem != null) 
            {
                ChangeItemQuantity(selectedItem, -GetTossValue());
            }
        });
        UIController.InventoryTossUp1Button.onClick.AddListener(    () => ChangeTossNumber(+1)) ;
        UIController.InventoryTossUp10Button.onClick.AddListener(   () => ChangeTossNumber(+10));
        UIController.InventoryTossDown1Button.onClick.AddListener(  () => ChangeTossNumber(-1));
        UIController.InventoryTossDown10Button.onClick.AddListener( () => ChangeTossNumber(-10));
		#endregion


        UIController.InventoryContextMenu.gameObject.SetActive(false);
		//debug
		//OpenInventory();
	}

	void ChangeItemQuantity(ItemQuantity entry, int changeAmount)
    {
        entry.amount += changeAmount;
        if (entry.amount <= 0)
        {
            Destroy(entry.entryGameObject);
            inventory.Remove(entry);
            if (entry == selectedItem)
                selectedItem = null;
            UIController.InventoryContextMenu.gameObject.SetActive(false);
            UpdateInventorySize();
            return;
        }
        selectedItem.title.text = GetItemText(entry);
    }

    string GetItemText(ItemQuantity entry)
    {
        if ((entry.item.type & Items.ItemType.Equipment) == 0)
            return $"{entry.item.name} x{entry.amount}";
        else
            return $"{entry.item.name}";
    }

    void OpenInventory()
    {
        UIController.InventoryRootRectTransform.gameObject.SetActive(true);
        ClearInventory();
        RebuildInventory();
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


    void UpdateInventorySize()
    {
        UIController.InventoryItemContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (itemDropPrefabHeight + itemSpacing) * inventory.Count - 5);
    }

    void RebuildInventory()
    {
        ClearInventory();
        UpdateInventorySize();
        for (int i = 0; i < inventory.Count; i++)
        {
            InstantiateItemToInventory(inventory[i]);
        }
    }

    void InstantiateItemToInventory(ItemQuantity item)
    {
        GameObject go = Instantiate(inventoryItemEntryPrefab, UIController.InventoryItemContent);
        go.name = item.item.name;
        go.GetComponentInChildren<Image>().sprite = item.item.sprite;
        item.title = go.GetComponentInChildren<Text>();
        item.title.text = GetItemText(item);
        item.selectionBox = go.GetComponent<RawImage>();
        item.entryGameObject = go;

        Button butt = go.GetComponent<Button>();
        butt.onClick.RemoveAllListeners();

        butt.onClick.AddListener(() => {
            int index = go.transform.GetSiblingIndex();
            print("i: " +index + " c: " + go.transform.GetSiblingIndex() + inventory.Count);

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

    void AddItemToInventory(ItemQuantity item)
    {
        var duplicate = inventory.Find(x => (Items.ItemType.Equipment & item.item.type) == 0 && x.item.name == item.item.name); //look for  a duplicate, that has none of the equipment flags set
        if (duplicate == null) //if none were found
        {
            inventory.Add(item); //add to the inventory the rolled one
        }
        else
            duplicate.amount += item.amount; //add the count to the duplicate

        UpdateInventorySize();
        InstantiateItemToInventory(item);
    }

    public void ProcessDrops(List<ItemDrop> drops, bool showDropList = true) 
    {
        List<ItemQuantity> dropsProcessed = new List<ItemQuantity>();
        //Get all items dropped
        foreach (var v in drops) 
        {
            var d = RollForDrop(v); //roll for each individual item on the table
            if (d != null) dropsProcessed.Add(d); //if successful add item
        }

        //Show items that rolled a success
        if (showDropList)
        {
            dropList.gameObject.SetActive(true); //show items dropped
            if (dropsProcessed.Count == 0) //if no item dropped
            {
                itemInfoGameObjects[0].icon.transform.parent.gameObject.SetActive(true); //activate the first drop
                itemInfoGameObjects[0].icon.sprite = null; //remove its icon
                itemInfoGameObjects[0].text.text = "None"; //then write that no drops were gained
            }

            for (int i = 0; i < itemInfoGameObjects.Length; i++)
            {
                bool inBounds = i < dropsProcessed.Count;
                itemInfoGameObjects[i].icon.transform.parent.gameObject.SetActive(inBounds);
                if (inBounds)
                {
                    itemInfoGameObjects[i].icon.sprite = dropsProcessed[i].item.sprite;
                    itemInfoGameObjects[i].text.text = $"{dropsProcessed[i].item.name} x{dropsProcessed[i].amount}";
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
                amount = Random.Range(item.minCount, item.maxCount + 1), //roll for quantity
                selectionBox = null,
                title = null,
                entryGameObject = null
            }; 
            print($"drop [{item.item.name}]  min: {item.minCount} max: {item.maxCount} rolled {drop.amount}");

            AddItemToInventory(drop);

            return drop;
        }

        return null;
    }
}
