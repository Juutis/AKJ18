using UnityEngine;

public class PickupableItem : MonoBehaviour
{
    [SerializeField]
    private Item item;

    public Item Item { get { return item; } }

    public void Kill()
    {
        Destroy(gameObject);
    }

}

[System.Serializable]
public class Item
{

    public string Name = "Item";

    [SerializeField]
    private ItemType itemType;

    public ItemType Type { get { return itemType; } }

}

public enum ItemType
{
    Axe,
    Thread
}