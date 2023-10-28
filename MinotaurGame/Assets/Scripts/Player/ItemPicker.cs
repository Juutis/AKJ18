using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPicker : MonoBehaviour
{
    [SerializeField]
    private float radius = 2f;

    public float Radius { get { return radius; } }

    [SerializeField]
    private LayerMask itemMask;

    [SerializeField]
    private LayerMask doorMask;

    /*void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawSphere(transform.position, radius);
    }*/

    private void Update()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0f, itemMask);
        if (hit.collider != null)
        {
            PickupableItem pickupItem = hit.collider.GetComponent<PickupableItem>();
            Item item = pickupItem.Item;
            Debug.Log($"Picked up {item.Name} (Type: {item.Type})!");
            GameManager.main.PickupItem(item);

            pickupItem.Kill();
        }
        RaycastHit2D hitDoor = Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0f, doorMask);
        if (hitDoor.collider != null)
        {
            Door door = hitDoor.collider.GetComponent<Door>();
            if (door != null && door.IsOpen)
            {
                GameManager.main.OpenNextLevel();
            }
        }
    }


}


