using UnityEngine;

public class MovingEnemy : MonoBehaviour
{
    [SerializeField]
    private CollidingEntity collidingEntity;

    private int direction = 1;
    private float horizInput = 1f;

    private bool canChangeDirection = true;

    // Update is called once per frame
    void Update()
    {
        if (collidingEntity.IsOnGround)
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
}
