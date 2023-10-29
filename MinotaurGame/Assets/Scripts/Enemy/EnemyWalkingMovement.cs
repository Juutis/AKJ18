using UnityEngine;

public class MovingEnemy : MonoBehaviour
{
    [SerializeField]
    private CollidingEntity collidingEntity;

    private int direction = 1;
    private float horizInput = 1f;

    private bool canChangeDirection = true;

    private void Start()
    {
        collidingEntity.Init();
        collidingEntity.UseCoyoteTime(false);
    }

    // Update is called once per frame
    void Update()
    {
        collidingEntity.SetHorizontalInput(direction * horizInput);
        if (canChangeDirection && collidingEntity.IsTouchingWall)
        {
            direction = -direction;
            canChangeDirection = false;
        }
        if (!collidingEntity.IsTouchingWall)
        {
            canChangeDirection = true;
        }
    }
}
