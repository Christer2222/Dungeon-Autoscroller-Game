using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    private readonly List<ItemQuantity> inventory = new List<ItemQuantity>();
    public ItemQuantity selectedItem;

    public List<ItemQuantity> equippedItems = new List<ItemQuantity>();

    private const string USE_STRING = "Use";
    private const string BUSY_STRING = "Busy";
    private const string CONSUME_STRING = "Consume";
    private const string EQUIP_STRING = "Equip";
    private const string CANT_STRING = "Can't";
    private const string WAIT_STRING = "Wait";

	private readonly Color dropListAddColor = new Color(0, 1, 0, 0.5f), dropListDontAddColor = new Color(0.25f, 0.25f, 0.25f, 0.5f);

    public Transform dropList;
    private ItemInfoGameObject[] itemInfoGameObjects;

    private GameObject inventoryItemEntryPrefab;
    private float itemDropPrefabHeight;
    private float itemSpacing;

    private float halfItemContextHeight;

    private class ItemInfoGameObject
    {
        public Image icon;
        public Text text;
        public Image background;
        public ToolTip toolTip;
        public Button button;
        public bool selected = true;
    }

    public class ItemQuantity
    {
        public int amount;
        public Items.ItemInfo item;
        public Image selectionBox;
        public Text title;
        public GameObject entryGameObject;
    }

    public class EquipmentSlot
    {
        public Image displayImage;
        public ItemQuantity itemEquipped;
        public ToolTip toolTip;
    }

    public class Reference<T> 
    { 
        public T Value { get; set; } //make a property, which is a reference
        public Reference(T val) //constructor
        { 
            Value = val;
        } 
    }

    public static Reference<EquipmentSlot> //make all equipment slots references, so they can easier be manipulated in a list
        helmetSlot = new Reference<EquipmentSlot>(new EquipmentSlot()),
        chestplateSlot = new Reference<EquipmentSlot>(new EquipmentSlot()), 
        leggingsSlot = new Reference<EquipmentSlot>(new EquipmentSlot()), 
        bootsSlot = new Reference<EquipmentSlot>(new EquipmentSlot()), 
        mainHandSlot = new Reference<EquipmentSlot>(new EquipmentSlot()), 
        offHandSlot = new Reference<EquipmentSlot>(new EquipmentSlot()), 
        accessory1Slot = new Reference<EquipmentSlot>(new EquipmentSlot()), 
        accessory2Slot = new Reference<EquipmentSlot>(new EquipmentSlot()), 
        accessory3Slot = new Reference<EquipmentSlot>(new EquipmentSlot());

    public static readonly List<Reference<EquipmentSlot>> allEquipmentSlots = new List<Reference<EquipmentSlot>>()
    { 
        helmetSlot,
        chestplateSlot,
        leggingsSlot,
        bootsSlot,
        mainHandSlot,
        offHandSlot,
        accessory1Slot,
        accessory2Slot,
        accessory3Slot,
    };


    private UnityAction addToInventoryAction;
    //private static readonly List<EquipmentSlot> allEquipmentSlots = new List<EquipmentSlot>() { helmetSlot, chestplateSlot, leggingsSlot, bootsSlot, mainHandSlot, offHandSlot, accessory1Slot, accessory2Slot, accessory3Slot };
    //private readonly List<EquipmentSlot> allEquipmentSlots = new List<EquipmentSlot>() { helmetSlot, chestplateSlot, leggingsSlot, bootsSlot, mainHandSlot, offHandSlot, accessory1Slot, accessory2Slot, accessory3Slot };
    //private EquipmentSlot[] allEquipmentSlots = new EquipmentSlot[] { helmetSlot, chestplateSlot, leggingsSlot, bootsSlot, mainHandSlot, offHandSlot, accessory1Slot, accessory2Slot, accessory3Slot };
    // Start is called before the first frame update
    void Start()
    {
        helmetSlot.Value.displayImage = UIController.CurrentEquippedHelmetImage;
        chestplateSlot.Value.displayImage = UIController.CurrentEquippedChestplateImage;
        leggingsSlot.Value.displayImage = UIController.CurrentEquippedLeggingsImage;
        bootsSlot.Value.displayImage = UIController.CurrentEquippedBootsImage;
        mainHandSlot.Value.displayImage = UIController.CurrentEquippedMainHandImage;
        offHandSlot.Value.displayImage = UIController.CurrentEquippedOffHandImage;
        accessory1Slot.Value.displayImage = UIController.CurrentEquippedAccessory1Image;
        accessory2Slot.Value.displayImage = UIController.CurrentEquippedAccessory2Image;
        accessory3Slot.Value.displayImage = UIController.CurrentEquippedAccessory3Image;

        helmetSlot.Value.toolTip = UIController.CurrentEquippedHelmetImage.GetComponent<ToolTip>();
        chestplateSlot.Value.toolTip = UIController.CurrentEquippedChestplateImage.GetComponent<ToolTip>();
        leggingsSlot.Value.toolTip = UIController.CurrentEquippedLeggingsImage.GetComponent<ToolTip>();
        bootsSlot.Value.toolTip = UIController.CurrentEquippedBootsImage.GetComponent<ToolTip>();
        mainHandSlot.Value.toolTip = UIController.CurrentEquippedMainHandImage.GetComponent<ToolTip>();
        offHandSlot.Value.toolTip = UIController.CurrentEquippedOffHandImage.GetComponent<ToolTip>();
        accessory1Slot.Value.toolTip = UIController.CurrentEquippedAccessory1Image.GetComponent<ToolTip>();
        accessory2Slot.Value.toolTip = UIController.CurrentEquippedAccessory2Image.GetComponent<ToolTip>();
        accessory3Slot.Value.toolTip = UIController.CurrentEquippedAccessory3Image.GetComponent<ToolTip>();


        instance = this;
        inventoryItemEntryPrefab = Resources.Load<GameObject>("Prefabs/InventoryItemEntry");
        itemDropPrefabHeight = inventoryItemEntryPrefab.GetComponent<RectTransform>().rect.height;

        halfItemContextHeight = UIController.InventoryGeneralContextMenu.GetComponentInChildren<Image>().rectTransform.rect.height/2;
        itemSpacing = UIController.InventoryItemContent.GetComponent<VerticalLayoutGroup>().spacing;

        /*
        UIController.InventoryButton.onClick.AddListener(delegate {
            print("click");
            UIController.SetUIMode(UIController.UIMode.Inventory);
        });
        */
        UIController.InventoryCloseButton.onClick.AddListener(delegate {
            if (selectedItem != null)
                selectedItem.selectionBox.color = Color.clear;

            selectedItem = null;
            UIController.InventoryGeneralContextMenu.gameObject.SetActive(false);
            //UIController.InventoryRootRectTransform.gameObject.SetActive(false);
            UIController.SetUIMode(UIController.UIMode.None);
        });

#if UNITY_EDITOR
        ItemQuantity[] debugItems =
            {
            new ItemQuantity() { amount = 5, item = Items.Rock },

            new ItemQuantity() { amount = 5, item = Items.Apple },
            /*
            new ItemQuantity() { amount = 5, item = Items.Orange },
            new ItemQuantity() { amount = 5, item = Items.Banana },

            new ItemQuantity() { amount = 5, item = Items.MinorHealthPotion },
            new ItemQuantity() { amount = 5, item = Items.HealthPotion },
            new ItemQuantity() { amount = 5, item = Items.GreaterHealthPotion },
            new ItemQuantity() { amount = 5, item = Items.MinorManaPotion },
            new ItemQuantity() { amount = 5, item = Items.ManaPotion },
            new ItemQuantity() { amount = 5, item = Items.GreaterManaPotion },

            new ItemQuantity() { amount = 5, item = Items.Stick },
            
            */
            new ItemQuantity() { amount = 1, item = Items.SteelSword },
            new ItemQuantity() { amount = 1, item = Items.SteelBroadSword },
            new ItemQuantity() { amount = 1, item = Items.GoldSword },
            /*
            new ItemQuantity() { amount = 1, item = Items.AdamantineSword },
            new ItemQuantity() { amount = 1, item = Items.AdamantineBroadSword },
            new ItemQuantity() { amount = 1, item = Items.PoisonedDagger },
            
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
#endif

        //Info about the drops after battle
        dropList = UIController.ItemDropListGameObject;
        itemInfoGameObjects = new ItemInfoGameObject[dropList.childCount];
        for (int i = 0; i < dropList.childCount; i++)
        {
            var child = dropList.GetChild(i);
            itemInfoGameObjects[i] = new ItemInfoGameObject() {
                icon = child.Find("$Icon").GetComponent<Image>(),
                text = child.GetComponentInChildren<Text>(),
                background = child.Find("$Background").GetComponent<Image>(),
                toolTip = child.GetComponentInChildren<ToolTip>(),
                button = child.GetComponent<Button>(),
            };

            int _index = i;
            itemInfoGameObjects[_index].background.color = (itemInfoGameObjects[_index].selected) ? dropListAddColor : dropListDontAddColor;

            itemInfoGameObjects[i].button.onClick.AddListener(delegate { 
                itemInfoGameObjects[_index].selected = !itemInfoGameObjects[_index].selected;
                itemInfoGameObjects[_index].background.color = (itemInfoGameObjects[_index].selected) ? dropListAddColor : dropListDontAddColor;
            });
        }

		#region Buttons
		#region Use Button
		//----------USE BUTTON
		UIController.InventoryUseButton.onClick.AddListener(() => {  
            if (selectedItem !=  null)
            {
                				
                if ((selectedItem.item.type & Items.ItemType.Consumable) != 0 && ((EncounterController.instance.currentGameState & EncounterController.GameState.ConfirmingDrops) == 0)) //if item has consumable flag
                {
                    if (CombatController.turnOrder.Count > 0)
                    {
                        if (CombatController.turnOrder[0] != CombatController.playerCombatController)
                        {
                            StartCoroutine(EffectTools.ChangeTextAndReturn(UIController.InventoryUseButtonText, USE_STRING, WAIT_STRING, 0.5f));
                            return; 
                        }
                    }
                    else if (CombatController.playerCombatController.CheckIfHasBuff("Busy"))
                    {
                        StartCoroutine(EffectTools.ChangeTextAndReturn(UIController.InventoryUseButtonText, USE_STRING, BUSY_STRING, 0.5f));
                        return;
                    }

                    UIController.SetUIMode(UIController.UIMode.None);
                    selectedItem.selectionBox.color = Color.clear;
                    //UIController.InventoryRootRectTransform.gameObject.SetActive(false);
                    UIController.InventoryGeneralContextMenu.gameObject.SetActive(false);
                    for (int i = 0; i < selectedItem.item.activeAbilities.Count; i++)
                    {
                        AbilityInfo.TargetData targetData = new AbilityInfo.TargetData(
                            selectedItem.item.activeAbilities[i], //ability
                            CombatController.playerCombatController, //self
                            CombatController.playerCombatController, //target 
                            selectedItem.item.activeConstants[i], //bonus
                            selectedItem.item.activeAbilities[i].element, //element
                            CombatController.playerCombatController.transform.position, //where to show icon. Broken?
                            _useOwnStats: false
                            );


                        CombatController.playerCombatController.StartCoroutine(
                            CombatController.playerCombatController.SimpleInvokeAbility(targetData,false,(CombatController.turnOrder.Count > 0)? ((i == selectedItem.item.activeAbilities.Count - 1)? true: false) :false, 1, true, delegate {
 
                            }));

                    }

                    ChangeItemQuantity(selectedItem, -1); //remove an item after it is used
                    selectedItem = null;
                }
                else if ((selectedItem.item.type & Items.ItemType.Targetable) != 0 && ((EncounterController.instance.currentGameState & EncounterController.GameState.ConfirmingDrops) == 0)) //if item has targetable flag
                {
                    UIController.SetUIMode(UIController.UIMode.None);
                    UIController.InventoryButtonText.text = selectedItem.item.name;
                    selectedItem.selectionBox.color = Color.clear;
                    UIController.InventoryGeneralContextMenu.gameObject.SetActive(false);
                }
                else
                   StartCoroutine(EffectTools.ChangeTextAndReturn(UIController.InventoryUseButtonText, USE_STRING, CANT_STRING, 0.5f));
            }

        });
		#endregion
		#region Equip Buttons
		//----------EUQIP BUTTON
		UIController.InventoryEquipButton.onClick.AddListener(() => {
            if (UIController.InventoryWeaponSlotContextMenu.gameObject.activeSelf || UIController.InventoryAccessoryContextMenu.gameObject.activeSelf)
            {
                return;
            }

            Items.ItemType _cleared = (selectedItem.item.type & Items.ItemType.Equipment);
            if (_cleared != 0) //equipment has item flag
            {
                EquipmentSlot _slotToGo = null;
                switch (_cleared)
                {
                    case Items.ItemType.Helmet:
                        _slotToGo = helmetSlot.Value;
                        break;
                    case Items.ItemType.Chestplate:
                        _slotToGo = chestplateSlot.Value;
                        break;
                    case Items.ItemType.Leggings:
                        _slotToGo = leggingsSlot.Value;
                        break;
                    case Items.ItemType.Boots:
                        _slotToGo = bootsSlot.Value;
                        break;
                    case Items.ItemType.TwoHanded:
                        _slotToGo = mainHandSlot.Value;
                        break;
                }
                EquipItem(_slotToGo);
            }
            else
               StartCoroutine(EffectTools.ChangeTextAndReturn(UIController.InventoryEquipText, EQUIP_STRING, CANT_STRING, 0.5f));
        });

        //---------SPECIAL EQUIP BUTTONS
        UIController.InventoryEquipInMainHandButton.onClick.AddListener(    () => EquipItem(mainHandSlot.Value));
        UIController.InventoryEquipInOffHandButton.onClick.AddListener(     () => EquipItem(offHandSlot.Value));
        UIController.InventoryEquipInAccessory1Button.onClick.AddListener(  () => EquipItem(accessory1Slot.Value));
        UIController.InventoryEquipInAccessory2Button.onClick.AddListener(  () => EquipItem(accessory2Slot.Value));
        UIController.InventoryEquipInAccessory3Button.onClick.AddListener(  () => EquipItem(accessory3Slot.Value));
		#endregion
		#region Unequip Buttons
		//-----------UNEQUIP BUTTONS
		UIController.UnequipHelmetButton.onClick.AddListener(       () => UnequipItem(helmetSlot.Value));
        UIController.UnequipChestplateButton.onClick.AddListener(   () => UnequipItem(chestplateSlot.Value));
        UIController.UnequipLeggingsButton.onClick.AddListener(     () => UnequipItem(leggingsSlot.Value));
        UIController.UnequipBootsButton.onClick.AddListener(        () => UnequipItem(bootsSlot.Value));
        UIController.UnequipAccessory1Button.onClick.AddListener(   () => UnequipItem(accessory1Slot.Value));
        UIController.UnequipAccessory2Button.onClick.AddListener(   () => UnequipItem(accessory2Slot.Value));
        UIController.UnequipAccessory3Button.onClick.AddListener(   () => UnequipItem(accessory3Slot.Value));

        UIController.UnequipMainHandButton.onClick.AddListener(     () => 
        {
            if (mainHandSlot.Value.itemEquipped != null) //if there is an item in the mainhand 
                if ((mainHandSlot.Value.itemEquipped.item.type & Items.ItemType.TwoHanded) != 0) //check if it is twohanded
                    UnequipItem(offHandSlot.Value); //if it is unequip offhand too

            UnequipItem(mainHandSlot.Value); //either way, unequip main hand
        });

        UIController.UnequipOffHandButton.onClick.AddListener(      () => 
        {
            if (mainHandSlot.Value.itemEquipped != null) //chack if there is an item in mainhand
                if ((mainHandSlot.Value.itemEquipped.item.type & Items.ItemType.TwoHanded) != 0)  //if it is twohanded
                    UnequipItem(mainHandSlot.Value); //unequip it, as that was the real item

            UnequipItem(offHandSlot.Value); //either way, unequip off hand
        });
        #endregion
        #region Toss Buttons
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
    
        UIController.ItemDropListConfirmButton.onClick.AddListener(delegate { EncounterController.instance.SetGameState(EncounterController.GameState.Walking); UIController.ItemDropListGameObject.parent.gameObject.SetActive(false); });
		#endregion


		UIController.InventoryGeneralContextMenu.gameObject.SetActive(false);
        
        //ClearInventory();
        RebuildInventory();
    }


    void UnequipItem(EquipmentSlot _slot)
    {
        if (_slot.itemEquipped != null) //if same type already equipped
        {
            CombatController.playerCombatController.RemoveAllBufsWithName(_slot.itemEquipped.item.name + _slot.displayImage.gameObject.name);
            
            //add it back to the inventory
            AddItemToInventory(_slot.itemEquipped);
            ChangeItemQuantity(_slot.itemEquipped, 1);
        }

        _slot.toolTip.SetToolTipText(string.Empty);

        _slot.itemEquipped = null; //set slots item to selected one

        _slot.displayImage.gameObject.SetActive(false); //show it
        _slot.displayImage.sprite = null;// _slot.itemEquipped.item.sprite; //show it



        _slot.toolTip.OnPointerExit(null);//ChangeToolTipText(string.Empty);
        //ToolTip.currentToolTip.OnPointerExit(null);
    }


    void EquipItem(EquipmentSlot _slot)
    {
        if (CombatController.turnOrder.Count > 0) //if there are participants in combat
            if (CombatController.turnOrder[0] != CombatController.playerCombatController) //if it is not the players turn
            {
                StartCoroutine(EffectTools.ChangeTextAndReturn(UIController.InventoryEquipText, EQUIP_STRING, WAIT_STRING, 1)); //tell the player they cant
                return;
            }


        bool _wasDualWielding = (((mainHandSlot.Value.itemEquipped != null? mainHandSlot.Value.itemEquipped.item.type : 0) | (offHandSlot.Value.itemEquipped != null? offHandSlot.Value.itemEquipped.item.type : 0)) & Items.ItemType.TwoHanded) != 0;

        //remove previous item in same slot
        if (_slot.itemEquipped != null)
            UnequipItem(_slot);

        _slot.itemEquipped = selectedItem; //set slots item to selected one

        _slot.displayImage.gameObject.SetActive(true); //show it
        _slot.displayImage.sprite = _slot.itemEquipped.item.sprite; //show it

        print(_slot.toolTip);
        _slot.toolTip.SetToolTipText(_slot.itemEquipped.item.description);

        if ((selectedItem.item.type & Items.ItemType.TwoHanded) != 0) //if item was twohanded
        {
            UnequipItem(offHandSlot.Value);

            offHandSlot.Value.displayImage.gameObject.SetActive(true); //either way, show sprite
            offHandSlot.Value.displayImage.sprite = _slot.itemEquipped.item.sprite; //then use correct one

            offHandSlot.Value.toolTip.SetToolTipText(_slot.itemEquipped.item.description);
        }
        else if ((selectedItem.item.type & Items.ItemType.OneHanded) != 0 && _wasDualWielding) //if weapon was one handed instead, AND player was dualwielding
        {
            UnequipItem(_slot.Equals(offHandSlot.Value) ? mainHandSlot.Value : offHandSlot.Value);
        }

        AbilityScript.AddBuff(new Buff(_slot.itemEquipped.item.name + _slot.displayImage.gameObject.name, Buff.TraitType.Physical_Defence_Constant, int.MaxValue, null, Buff.StackType.Add_Duplicate, selectedItem.item.physicalDefense,null,false), CombatController.playerCombatController.MyStats);
        AbilityScript.AddBuff(new Buff(_slot.itemEquipped.item.name + _slot.displayImage.gameObject.name, Buff.TraitType.Magic_Defence_Constant, int.MaxValue, null, Buff.StackType.Add_Duplicate, selectedItem.item.magicDefense,null,false), CombatController.playerCombatController.MyStats);

        print(CombatController.playerCombatController.CheckIfHasBuff(_slot.itemEquipped.item.name + _slot.displayImage.gameObject.name));

        ChangeItemQuantity(selectedItem, -1); //finally remove selected item from inventory

        if (CombatController.turnOrder.Count > 0) //if there are participants in combat
            if (CombatController.turnOrder[0] == CombatController.playerCombatController) //if it is the players turn, they was successful
            {
                UIController.SetUIMode(UIController.UIMode.None);
                StartCoroutine(EffectTools.ChangeTextAndReturn(UIController.InventoryButtonText, UIController.ITEMS_BUTTON_STRING, "Equip!", 1));
                StartCoroutine(CombatController.playerCombatController.EndTurn());
            }
    }


    public void ChangeItemQuantity(ItemQuantity entry, int changeAmount)
    {
        entry.amount += changeAmount;
        if (entry.amount <= 0)
        {
            Destroy(entry.entryGameObject);
            inventory.Remove(entry);
            if (entry == selectedItem)
                selectedItem = null;
            UIController.InventoryGeneralContextMenu.gameObject.SetActive(false);
            UpdateInventorySize();
            return;
        }

        entry.title.text = GetItemText(entry);
    }


    string GetItemText(ItemQuantity entry)
    {
        if ((entry.item.type & Items.ItemType.Equipment) == 0)
            return $"{entry.item.name} x{entry.amount}";
        else
            return $"{entry.item.name}";
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
        go.transform.Find("$Icon").GetComponent<Image>().sprite = item.item.sprite;
        item.title = go.GetComponentInChildren<Text>();
        item.title.text = GetItemText(item);
        item.selectionBox = go.transform.Find("$Background").GetComponent<Image>();//.GetComponent<RawImage>();
        item.entryGameObject = go;

        item.selectionBox.color = Color.clear;

        Button butt = go.GetComponent<Button>();
        butt.onClick.RemoveAllListeners();

        go.GetComponent<ToolTip>().SetToolTipText(item.item.description);

        butt.onClick.AddListener(() => {
            int index = go.transform.GetSiblingIndex();
            var contextMenu = UIController.InventoryGeneralContextMenu; //shortcut
            bool _wasLastItem = selectedItem == inventory[index];

            if (selectedItem != null)
            {
                if (selectedItem.item != null)
                    selectedItem.selectionBox.color = Color.clear; //clear the previous item
            }

            selectedItem = (_wasLastItem)? null: inventory[index]; //check for double clicking
            if (selectedItem != null) //if not double clicked to remove
            {
                selectedItem.selectionBox.color = Color.white; //set the newly selected items color to selected

                //set context menus appropriately
                UIController.InventoryWeaponSlotContextMenu.gameObject.SetActive((selectedItem.item.type & Items.ItemType.OneHanded) != 0);
                UIController.InventoryAccessoryContextMenu.gameObject.SetActive((selectedItem.item.type & Items.ItemType.Acessory) != 0);

                if ((selectedItem.item.type & Items.ItemType.Consumable) != 0) 
                    UIController.InventoryUseButtonText.text = CONSUME_STRING;
                else 
                    UIController.InventoryUseButtonText.text = USE_STRING;
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


    public void AddItemToInventory(ItemQuantity item)
    {
        var duplicate = inventory.Find(x => (Items.ItemType.Equipment & item.item.type) == 0 && x.item.name == item.item.name); //look for  a duplicate, that has none of the equipment flags set
        if (duplicate == null) //if none were found
        {
            //InstantiateItemToInventory(item);
            //AddItemToInventory(item); //add to the inventory the rolled one 
            inventory.Add(item);
            InstantiateItemToInventory(item);
            UpdateInventorySize();
        }
        else
        {
            ChangeItemQuantity(duplicate, item.amount); //add the count to the duplicate
        }

    }


    public void ProcessDrops(List<ItemDrop> _drops, bool _showDropList = true) 
    {
        List<ItemQuantity> _dropsProcessed = new List<ItemQuantity>();

        //Get all items dropped
        foreach (var _val in _drops) 
        {
            var _gottenDrop = RollForDrop(_val); //roll for each individual item on the table
            if (_gottenDrop != null) _dropsProcessed.Add(_gottenDrop); //if successful add item
        }


        //Show items that rolled a success
        if (_showDropList)
        {
            dropList.parent.gameObject.SetActive(true); //show items dropped
            if (_dropsProcessed.Count == 0) //if no item dropped
            {
                itemInfoGameObjects[0].icon.transform.parent.gameObject.SetActive(true); //activate the first drop
                itemInfoGameObjects[0].icon.color = Color.clear;// = null; //remove its icon
                itemInfoGameObjects[0].text.text = "None"; //then write that no drops were gained
                itemInfoGameObjects[0].toolTip.SetToolTipText("No drops were found.");
                itemInfoGameObjects[0].button.enabled = false;

            }

            for (int i = 0; i < itemInfoGameObjects.Length; i++)
            {
                bool inBounds = i < _dropsProcessed.Count;

                itemInfoGameObjects[i].selected = inBounds;
                itemInfoGameObjects[i].button.enabled = true;

                itemInfoGameObjects[i].background.color = dropListAddColor;

                itemInfoGameObjects[i].icon.transform.parent.gameObject.SetActive(inBounds);
                if (inBounds)
                {
                    itemInfoGameObjects[i].icon.color = Color.white;
                    itemInfoGameObjects[i].icon.sprite = _dropsProcessed[i].item.sprite;
                    itemInfoGameObjects[i].text.text = $"{_dropsProcessed[i].item.name} x{_dropsProcessed[i].amount}";
                    itemInfoGameObjects[i].toolTip.SetToolTipText(_dropsProcessed[i].item.description);
                }


            }
        }

        
        if (addToInventoryAction != null)
            UIController.ItemDropListConfirmButton.onClick.RemoveListener(addToInventoryAction);//RemoveAllListeners();


        addToInventoryAction = delegate
        {
            print(_dropsProcessed.Count);
            for (int i = 0; i < itemInfoGameObjects.Length; i++)
            {
                print("i: " + i + " selecte: " + itemInfoGameObjects[i].selected + " drop i: " + _dropsProcessed);
                if (itemInfoGameObjects[i].selected)
                {
                    AddItemToInventory(_dropsProcessed[i]);
                }
            }

        };

        //_dropsProcessed.ForEach(x => AddItemToInventory(x)); //then add the item to inventory

        //StartCoroutine(EffectTools.DeactivateGameObject(dropList.gameObject, 3));

        SetInventorySlotText();

        UIController.ItemDropListConfirmButton.onClick.AddListener(addToInventoryAction);

        EncounterController.instance.currentGameState = EncounterController.GameState.ConfirmingDrops;
    }

    public void SetInventorySlotText()
	{
        UIController.InventorySlotSizeText.text = $"Slots: {inventory.Count}/{CombatController.playerCombatController.MyStats.CarryingSlots}";
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

            return drop;
        }

        return null;
    }


    public void ResetItemButton()
    {
        instance.selectedItem = null;
        UIController.InventoryButtonText.text = UIController.ITEMS_BUTTON_STRING;
    }
}
