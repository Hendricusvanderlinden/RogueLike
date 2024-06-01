using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Actor))]
public class Player : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;
    public Inventory inventory;

    private bool inventoryIsOpen = false;
    private bool droppingItem = false;
    private bool usingItem = false;

    private void Awake()
    {
        controls = new Controls();
    }

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
        GameManager.Get.Player = GetComponent<Actor>();
    }

    private void OnEnable()
    {
        controls.Player.SetCallbacks(this);
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.SetCallbacks(null);
        controls.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();

            if (inventoryIsOpen)
            {
                if (direction.y > 0)
                {
                    UIManager.Get.InventoryUI.SelectPreviousItem();
                }
                else if (direction.y < 0)
                {
                    UIManager.Get.InventoryUI.SelectNextItem();
                }
            }
            else
            {
                Move();
            }
        }
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (inventoryIsOpen)
            {
                UIManager.Get.InventoryUI.Hide();
                inventoryIsOpen = false;
                droppingItem = false;
                usingItem = false;
            }
        }
    }

    private void Move()
    {
        Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDirection = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));
        Action.MoveOrHit(GetComponent<Actor>(), roundedDirection); // Changed from Action.Move to Action.MoveOrHit
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector3 playerPosition = transform.position;
            Consumable item = GameManager.Get.GetItemAtLocation(playerPosition);

            if (item == null)
            {
                UIManager.Get.AddMessage("There is no item here to grab.", Color.yellow);
            }
            else if (!inventory.AddItem(item))
            {
                UIManager.Get.AddMessage("Your inventory is full.", Color.red);
            }
            else
            {
                item.gameObject.SetActive(false);
                GameManager.Get.RemoveItem(item);
                UIManager.Get.AddMessage($"You picked up a {item.Type}.", Color.green);
            }
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!inventoryIsOpen)
            {
                UIManager.Get.InventoryUI.Show(inventory.Items);
                inventoryIsOpen = true;
                droppingItem = true;
            }
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed && inventoryIsOpen)
        {
            int selectedIndex = UIManager.Get.InventoryUI.Selected;
            if (selectedIndex >= 0 && selectedIndex < inventory.Items.Count)
            {
                Consumable selectedItem = inventory.Items[selectedIndex];
                inventory.DropItem(selectedItem);

                if (droppingItem)
                {
                    selectedItem.transform.position = transform.position;
                    GameManager.Get.AddItem(selectedItem);
                    selectedItem.gameObject.SetActive(true);
                    UIManager.Get.AddMessage($"You dropped a {selectedItem.Type}.", Color.red);
                }
                else if (usingItem)
                {
                    UseItem(selectedItem);
                    Destroy(selectedItem.gameObject);
                }

                UIManager.Get.InventoryUI.Hide();
                inventoryIsOpen = false;
                droppingItem = false;
                usingItem = false;
            }
        }
    }


    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!inventoryIsOpen)
            {
                UIManager.Get.InventoryUI.Show(inventory.Items);
                inventoryIsOpen = true;
                usingItem = true;
            }
        }
    }

    private void UseItem(Consumable item)
    {
        switch (item.Type)
        {
            case Consumable.ItemType.HealthPotion:
                GetComponent<Actor>().Heal(item.HealingAmount);
                UIManager.Get.AddMessage($"You used a Health Potion and healed for {item.HealingAmount} hit points.", Color.green);
                break;
            case Consumable.ItemType.Fireball:
                List<Actor> nearbyEnemies = GameManager.Get.GetNearbyEnemies(transform.position);
                foreach (Actor enemy in nearbyEnemies)
                {
                    enemy.DoDamage(item.Damage);
                }
                UIManager.Get.AddMessage($"You used a Fireball and dealt {item.Damage} damage to nearby enemies.", Color.red);
                break;
            case Consumable.ItemType.ScrollOfConfusion:
                List<Actor> nearbyActors = GameManager.Get.GetNearbyEnemies(transform.position);
                foreach (Actor actor in nearbyActors)
                {
                    Enemy enemyComponent = actor.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        enemyComponent.Confuse();
                    }
                }
                UIManager.Get.AddMessage($"You used a Scroll of Confusion and confused nearby enemies.", Color.yellow);
                break;
            default:
                UIManager.Get.AddMessage("You cannot use this item.", Color.gray);
                break;
        }
    }
}