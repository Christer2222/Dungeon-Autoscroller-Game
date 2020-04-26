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
            //UIController.InventoryRootRectTransform.gameObject.SetActive(false);
            UIController.SetUIMode(UIController.UIMode.None);
        });

        ItemQuantity[] debugItems =
            {
            /*
            new ItemQuantity() { amount = 5, item = Items.Apple },
            new ItemQuantity() { amount = 5, item = Items.Orange },
            new ItemQuantity() { amount = 5, item = Items.Banana },

            new ItemQuantity() { amount = 5, item = Items.Stick },
            */

            new ItemQuantity() { amount = 1, item = Items.SteelSword },
            new ItemQuantity() { amount = 1, item = Items.SteelBroadSword },
            new ItemQuantity() { amount = 1, item = Items.GoldSword },
            new ItemQuantity() { amount = 1, item = Items.AdamantineSword },
            new ItemQuantity() { amount = 1, item = Items.AdamantineBroadSword },
            new ItemQuantity() { amount = 1, item = Items.PoisonedDagger },
             /*
            new ItemQuantity() { amount = 50, item = Items.GoldCoin },
            new ItemQuantity() { amount = 2, item = Items.Goldbar },

            new ItemQuantity() { amount = 1, item = Items.Headband },
            new ItemQuantity() { amount = 1, item = Items.SteelHelmet },
            new ItemQuantity() { amount = 1, item = Items.GladiatorsHelmet },

            new ItemQuantity() { amount = 1, item = Items.LeatherVest },
            new ItemQuantity() { amount = 1, item = Items.SteelChestplate },
            new ItemQuantity() { amount = 1, item = Items.AdamantineChestplate },

            new ItemQuantity() { amount = 1, item = Items.LeatherPants },
            new ItemQuantity() { amount = 1, item = Items.SteelLeggings },
            new ItemQuantity() { amount = 1, item = Items.AdamantineLeggings },

            new ItemQuantity() { amount = 1, item = Items.GoldRing },
            new ItemQuantity() { amount = 1, item = Items.StrikeRing },
            new ItemQuantity() { amount = 1, item = Items.BoltRing },
            new ItemQuantity() { amount = 1, item = Items.MeteorRing },
            */
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
                ItemQuantity _prevEquiped = null;
                var _allHanded = equippedItems.FindAll(x => (x.item.type & (Items.ItemType.OneHanded | Items.ItemType.TwoHanded)) != 0); //find all equipped item that is either one handed or twohanded
                bool _wasDualWielding = false;
                List<ItemQuantity> _allAccessories = null;

                if ((selectedItem.item.type & Items.ItemType.Acessory) != 0) //if accessory has been set
                {
                    _allAccessories = equippedItems.FindAll(x => x.item.type == Items.ItemType.Acessory);
                    if (_allAccessories.Count == 3)
                    {
                        _prevEquiped = _allAccessories[0];
                        print("removed oldest accessory, TODO: selection");
                    }
                }
                else if ((selectedItem.item.type & Items.ItemType.OneHanded) != 0 ) //if onehanded has been set
                {
                    print("eq onehand");
                    //nothing to do if no handedness detected
                    if (_allHanded.Count == 1) //if already holding one weapon
                    {
                        print("1 onehand");


                        if (_allHanded[0].item.type == Items.ItemType.TwoHanded) //if the previous item was twohanded
                        {
                            _prevEquiped = _allHanded[0]; //remove it
                            _wasDualWielding = true; //clear its sprite
                        }
                        //if it wasn't just equip it as if the slot was open
                    }
                    else if (_allHanded.Count >= 2) //if already dualwielding
                    {
                        print("2+ onehand");

                        _prevEquiped = _allHanded[0];

                        _wasDualWielding = true;
                        //print("removed oldest handed, TODO: selection");

                        //if (a.TrueForAll(x => x.item.type == Items.ItemType.OneHanded) && a.Count == 2)
                        //{ }
                    }

                    _prevEquiped = null;
                }
                else if ((selectedItem.item.type & Items.ItemType.TwoHanded) != 0) //if two handed has been set
                { 
                    equippedItems.RemoveAll(x => _allHanded.Contains(x)); //when equipping a twohanded, remove all other handed
                    _allHanded.ForEach(x => { AddItemToInventory(x); ChangeItemQuantity(x,1); }); //then add them back to the inventory
                    //then equip twohanded as normal
                }
                else //if a normal item, remove the previous one
                {
                    _prevEquiped = equippedItems.Find(x => x.item.type == selectedItem.item.type);
                }


                if (_prevEquiped != null) //if same type already equipped
                {
                    AddItemToInventory(_prevEquiped); //add it back to the inventory

                    ChangeItemQuantity(_prevEquiped, 1);
                }

                print("prev eq list: " + equippedItems.Contains(_prevEquiped) + " item: " + _prevEquiped);
                equippedItems.Remove(_prevEquiped);
                equippedItems.Add(selectedItem); //then add the selected item to equipped items


                Items.ItemType _cleared = selectedItem.item.type & Items.ItemType.Equipment;
                switch (_cleared)
                {
                    case Items.ItemType.Helmet:
                        UIController.CurrentEquippedHelmetImage.sprite = selectedItem.item.sprite;
                        UIController.CurrentEquippedHelmetImage.gameObject.SetActive(true);
                        break;
                    case Items.ItemType.Chestplate:
                        UIController.CurrentEquippedChestplateImage.sprite = selectedItem.item.sprite;
                        UIController.CurrentEquippedChestplateImage.gameObject.SetActive(true);
                        break;
                    case Items.ItemType.Leggings:
                        UIController.CurrentEquippedLeggingsImage.sprite = selectedItem.item.sprite;
                        UIController.CurrentEquippedLeggingsImage.gameObject.SetActive(true);
                        break;
                    case Items.ItemType.Boots:
                        UIController.CurrentEquippedBootsImage.sprite = selectedItem.item.sprite;
                        UIController.CurrentEquippedBootsImage.gameObject.SetActive(true);
                        break;
                    case Items.ItemType.OneHanded:

                        bool _hasSlotForWeapon = (_allHanded.Count >= 1)? (_allHanded[0].item.type & Items.ItemType.OneHanded) != 0: true;
                        
                        bool _lastHandedWas2Handed = (_allHanded.Count == 1) ? (_allHanded[0].item.type & Items.ItemType.TwoHanded) != 0 : false;

                        if (_lastHandedWas2Handed)
                        {
                            UIController.CurrentEquippedOffHandImage.sprite = null;
                            UIController.CurrentEquippedOffHandImage.gameObject.SetActive(false);
                        }
         
                        if (_hasSlotForWeapon)
                        {
                            UIController.CurrentEquippedMainHandImage.sprite = selectedItem.item.sprite;
                            UIController.CurrentEquippedMainHandImage.gameObject.SetActive(true);
                        }
                        else
                        {
                            UIController.CurrentEquippedOffHandImage.sprite = selectedItem.item.sprite;
                            UIController.CurrentEquippedOffHandImage.gameObject.SetActive(true);
                        }

                        if (_wasDualWielding)
                        {
                            //UIController.CurrentEquippedOffHandImage.sprite = null;
                            //UIController.CurrentEquippedOffHandImage.gameObject.SetActive(true);
                        }

                        break;
                    case Items.ItemType.TwoHanded:
                        UIController.CurrentEquippedMainHandImage.sprite = selectedItem.item.sprite;
                        UIController.CurrentEquippedOffHandImage.sprite = selectedItem.item.sprite;
                        UIController.CurrentEquippedMainHandImage.gameObject.SetActive(true);
                        UIController.CurrentEquippedOffHandImage.gameObject.SetActive(true);
                        break;
                    case Items.ItemType.Acessory:
                        switch (_allAccessories.Count)
                        {
                            case 0:
                                UIController.CurrentEquippedAccessory1Image.sprite = selectedItem.item.sprite;
                                UIController.CurrentEquippedAccessory1Image.gameObject.SetActive(true);
                                break;
                            case 1:
                                UIController.CurrentEquippedAccessory2Image.sprite = selectedItem.item.sprite;
                                UIController.CurrentEquippedAccessory2Image.gameObject.SetActive(true);
                                break;
                            case 2:
                                UIController.CurrentEquippedAccessory3Image.sprite = selectedItem.item.sprite;
                                UIController.CurrentEquippedAccessory3Image.gameObject.SetActive(true);
                                break;

                        }
                        break;
                }

                ChangeItemQuantity(selectedItem, -1); //and remove it from the inventory
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
        
        ClearInventory();
        RebuildInventory();
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
        //ClearInventory();
        //RebuildInventory();
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
            var contextMenu = UIController.InventoryContextMenu; //shortcut
            bool _wasLastItem = selectedItem == inventory[index];

            if (selectedItem != null)
            {
                selectedItem.selectionBox.color = Color.clear; //clear the previous item
            }

            selectedItem = (_wasLastItem)? null: inventory[index]; //check for double clicking
            if (selectedItem != null) //if not double clicked to remove
            {
                selectedItem.selectionBox.color = Color.white; //set the newly selected items color to selected

                //set context menus appropriately
                UIController.WeaponSlotContextMenu.gameObject.SetActive((selectedItem.item.type & Items.ItemType.OneHanded) != 0);
                UIController.AccessoryContextMenu.gameObject.SetActive((selectedItem.item.type & Items.ItemType.Acessory) != 0);
            }

            contextMenu.gameObject.SetActive(!_wasLastItem); //set context menu to wheter double clicking or not


            Vector3 orgPos = contextMenu.position; //get orgPos position of menu
            Vector3 targetPos = go.transform.position; //where to put this
            targetPos.x = contextMenu.position.x; //keep same x of menu
            contextMenu.position = targetPos; //update placement (target pos has a different bounds, and can't be used with ancoredPosition)

            orgPos = contextMenu.anchoredPosition; //get new ancored position
            orgPos.y = Mathf.Clamp(orgPos.y, UIController.InventoryRootRectTransform.rect.y + halfItemContextHeight, -UIController.InventoryRootRectTransform.rect.y - halfItemContextHeight); //clamp within bounds of inventory
            contextMenu.anchoredPosition = orgPos; //update position again


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
            //print($"drop [{item.item.name}]  min: {item.minCount} max: {item.maxCount} rolled {drop.amount}");

            AddItemToInventory(drop);

            return drop;
        }

        return null;
    }
}
