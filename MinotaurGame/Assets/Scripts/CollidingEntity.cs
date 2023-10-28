using System;
using UnityEngine;

public class CollidingEntity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 5;
    }

    public int HorizontalDirection { get { return horizontalDirection; } }

    private float horizontalSpeed = 0f;
    private float verticalSpeed = 0f;

    private float minHorizontalInput = 0.0001f;
    private float minVerticalInput = 0.0001f;


    private bool isOnGround;
    private bool isTouchingWall;
    private bool hasGravity = true;
    private bool checkFloor = true;

    public bool IsTouchingWall { get { return isTouchingWall; } }
    public bool IsOnGround { get { return isOnGround; } }
    private float acceleration = 0f;

    private int horizontalDirection = 1;

    [SerializeField]
    private Transform graphicsContainer;

    [SerializeField]
    private Transform wallRaycastPosition;
    [SerializeField]
    private Transform floorRaycastPosition;

    private float horizontalMovementPerFrame = 0f;

    private float fallSpeed = 0f;

    private float horizontalInput = 0f;
    private float verticalInput = 0f;

    private Action hitWallCallback;
    private Action hitFloorCallback;

    private float size = 1f;
    [SerializeField]
    private CollidingEntityConfig config;

    public void Init(CollidingEntityConfig config)
    {
        this.config = config;
        hasGravity = config.Gravity;
    }

    public void Init()
    {
        hasGravity = config.Gravity;
    }

    public void SetHorizontalInput(float input)
    {
        horizontalInput = input;
    }

    public void SetVerticalInput(float input)
    {
        verticalInput = input;
    }

    public void SetWallCallback(Action callback)
    {
        hitWallCallback = callback;
    }

    public void SetFloorCallback(Action callback)
    {
        hitFloorCallback = callback;
    }

    public void SetGravity(bool gravity)
    {
        hasGravity = gravity;
    }

    public void SetCheckFloor(bool check)
    {
        checkFloor = check;
    }

    public void Reset()
    {
        fallSpeed = 0f;
        horizontalSpeed = 0f;
        verticalSpeed = 0f;
        acceleration = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        CheckOnGroundStatus();
        CheckWallStatus();
        if (isOnGround && (verticalInput > minVerticalInput))
        {
            verticalSpeed = config.JumpSpeed;
        }

        if (horizontalInput > minHorizontalInput)
        {
            SetDirection(1);
            horizontalSpeed = horizontalDirection * config.RunSpeed * Time.deltaTime;
        }
        else if (horizontalInput < -minHorizontalInput)
        {
            SetDirection(-1);
            horizontalSpeed = horizontalDirection * config.RunSpeed * Time.deltaTime;
        }
        else
        {
            horizontalSpeed = 0f;
        }
        if (isTouchingWall)
        {
            horizontalSpeed = 0f;
        }

        if (hasGravity && !isOnGround)
        {
            acceleration = Time.deltaTime * config.GravityStrength;
            verticalSpeed -= acceleration / 2.0f;
        }
        Vector3 pos = transform.position;
        float vertSpeed = isOnGround ? Mathf.Clamp(verticalSpeed, 0, verticalSpeed) : verticalSpeed;
        fallSpeed = Mathf.Clamp(vertSpeed, -config.MaxFallSpeed, config.JumpSpeed);
        horizontalMovementPerFrame = horizontalSpeed;
        pos.y += fallSpeed * Time.deltaTime;
        pos.x += horizontalMovementPerFrame;
        transform.position = pos;
        verticalSpeed -= acceleration / 2.0f;
        if ((transform.position.y + size) < -Camera.main.orthographicSize)
        {
            Vector2 loopPos = transform.position;
            loopPos.y = Camera.main.orthographicSize + size;
            transform.position = loopPos;
        }
    }

    public void SetDirection(int direction)
    {
        horizontalDirection = direction;
        Vector2 scale = graphicsContainer.localScale;
        scale.x = horizontalDirection;
        graphicsContainer.localScale = scale;
    }

    private void CheckWallStatus()
    {
        Vector3 topRaycastOrigin = wallRaycastPosition.position - new Vector3(0f, size * 0.5f);
        Vector3 botRaycastOrigin = wallRaycastPosition.position + new Vector3(0f, size * 0.5f);

        isTouchingWall = RayCastToWall(topRaycastOrigin) || RayCastToWall(botRaycastOrigin);

        if (isTouchingWall)
        {
            hitWallCallback?.Invoke();
        }
    }

    private bool RayCastToWall(Vector3 raycastOrigin)
    {
        float rayDistance = Mathf.Abs(horizontalMovementPerFrame) + size;
        RaycastHit2D ray = Physics2D.Raycast(raycastOrigin, horizontalDirection * Vector2.right, rayDistance, config.WallLayer);
        Debug.DrawLine(raycastOrigin, new Vector3(raycastOrigin.x + rayDistance * horizontalDirection, raycastOrigin.y, raycastOrigin.z), Color.blue);
        if (ray.collider != null)
        {
            if (Mathf.Abs(horizontalInput) > minHorizontalInput)
            {
                int direction = ray.point.x > transform.position.x ? 1 : -1;
                if (horizontalDirection == direction)
                {
                    Vector3 pos = transform.position;

                    float targetX = ray.point.x + (0.5f * -horizontalDirection);

                    float maxMove;
                    if (horizontalDirection == 1)
                    {
                        maxMove = targetX - pos.x;
                    }
                    else
                    {
                        maxMove = pos.x - targetX;
                    }

                    pos.x += Mathf.Clamp(Mathf.Abs(config.RunSpeed * Time.deltaTime), 0, maxMove) * horizontalDirection;
                    transform.position = pos;
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    private void CheckOnGroundStatus()
    {
        if (verticalSpeed > 0 || !checkFloor)
        {
            isOnGround = false;
            return;
        }

        Vector3 leftRaycastOrigin = floorRaycastPosition.position - new Vector3(size * 0.47f, 0f);
        Vector3 rightRaycastOrigin = floorRaycastPosition.position + new Vector3(size * 0.47f, 0f);

        isOnGround = RaycastToGround(leftRaycastOrigin) || RaycastToGround(rightRaycastOrigin);

        if (isOnGround)
        {
            hitFloorCallback?.Invoke();
        }
    }


    private bool RaycastToGround(Vector3 raycastOrigin)
    {
        float rayDistance = Time.deltaTime * Mathf.Abs(fallSpeed * 2) + size;
        RaycastHit2D ray = Physics2D.Raycast(raycastOrigin, Vector2.down, rayDistance, config.FloorLayer);
        Debug.DrawLine(raycastOrigin, new Vector3(raycastOrigin.x, raycastOrigin.y - rayDistance, raycastOrigin.z), Color.red);
        if (ray.collider != null)
        {
            if (!isOnGround)
            {
                Vector3 pos = transform.position;
                pos.y = ray.point.y + size * 0.5f;
                transform.position = pos;
            }
            Debug.Log("We are on ground.");
            return true;
        }
        else
        {
            Debug.Log("We are NOT on ground");
            return false;
        }
    }
}
