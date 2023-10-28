using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CollidingEntity collidingEntity;

    // Update is called once per frame
    void Update()
    {
        collidingEntity.SetVerticalInput(Input.GetAxisRaw("Vertical"));
        collidingEntity.SetHorizontalInput(Input.GetAxisRaw("Horizontal"));
    }

}
