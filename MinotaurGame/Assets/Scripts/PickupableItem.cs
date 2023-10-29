using UnityEngine;

public class PickupableItem : MonoBehaviour
{
    [SerializeField]
    private Item item;

    public Item Item { get { return item; } }

    [SerializeField]
    private CircleCollider2D circleCollider2D;

    public Projectile ParentProjectile;

    public void Kill()
    {
        if (ParentProjectile != null) {
            ParentProjectile.PickedUp();
        }
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
    Thread,
    Bonus,
    Heart
}