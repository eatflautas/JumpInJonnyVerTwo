using UnityEngine;
using UnityEngine.Serialization;

public class EquipItems : MonoBehaviour
{
    // References
    public static EquipItems instance;
    public LayerMask equipable;
    public GameObject equippedItem;
    public Camera playerCamera;
    public Transform itemHolder;
    private Rigidbody itemRb;
    
    // Enums
    public InventoryState inventoryState;
    public EquippedItemType itemType;
    
    // Values
    public float grabRange = 2f;
    public float throwForce = 50f;
    
    // Inputs
    private bool interactPressed;
    private bool attackPressed;
    
    public enum InventoryState
    {
        full,
        empty
    }

    public enum EquippedItemType
    {
        weapon,
        none
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (equippedItem)
        {
            itemRb = equippedItem.GetComponent<Rigidbody>();
            itemRb.isKinematic = true;
        }
        else
        {
            inventoryState = InventoryState.empty;
            itemType = EquippedItemType.none;
        }
    }

    // Update is called once per frame
    void Update()
    {
        interactPressed = InputManager.instance.InteractJustPressed;
        attackPressed = InputManager.instance.AttackJustPressed;

        if (interactPressed)
        {
            InventoryAction();
        }

        if (attackPressed)
        {
            ItemAction();
        }
    }

    void InventoryAction()
    {
        if (inventoryState == InventoryState.full)
        {
            DropObject();
        }
        else if (inventoryState == InventoryState.empty)
        {
            GrabObject();
        }
    }

    void ItemAction()
    {
        if (inventoryState == InventoryState.full)
        {
            if (itemType == EquippedItemType.none)
            {
                ThrowObject();
            }
        }
    }

    
    
    #region Inventory Actions

    void DropObject()
    {
        DeInitializeObject();
    }
    
    void GrabObject()
    {
        // Check for object
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, grabRange, equipable))
        {
            equippedItem = hit.transform.gameObject;
            
            // Grab detected equipable object
            InitializeObject();
        }
    }
    
    #endregion
    
    #region Item Actions
    
    void ThrowObject()
    {
        itemRb.GetComponent<Rigidbody>().isKinematic = false;
        itemRb.AddForce(itemHolder.transform.forward * (throwForce * 10), ForceMode.Force);
        DeInitializeObject();
    }
    
    // void UseObject()
    // {
    // }
    
    #endregion

    void InitializeObject()
    {
        if (inventoryState == InventoryState.empty)
        {
            itemRb = equippedItem.GetComponent<Rigidbody>();
            itemRb.isKinematic = true;
            equippedItem.transform.position = itemHolder.transform.position;
            equippedItem.transform.rotation = itemHolder.transform.rotation;
            equippedItem.transform.SetParent(itemHolder);
            inventoryState = InventoryState.full;
        }
    }

    void DeInitializeObject()
    {
        if (inventoryState == InventoryState.full)
        { 
            itemHolder.transform.DetachChildren();
            itemRb.GetComponent<Rigidbody>().isKinematic = false;
            equippedItem = null;
            itemRb = null;
            inventoryState = InventoryState.empty;
        }
    }
}
