using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CollidingEntity collidingEntity;
    private bool canMove = false;

    private void Start()
    {
        collidingEntity.Init();
        Invoke("EnableMovement", 0.75f);
    }

    public void EnableMovement() {
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;

        collidingEntity.SetVerticalInput(Input.GetAxisRaw("Vertical"));
        collidingEntity.SetHorizontalInput(Input.GetAxisRaw("Horizontal"));
    }

}
