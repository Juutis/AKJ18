using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 5;
    }

    private bool gravity = true;

    private float horizontalSpeed = 0f;
    private float verticalSpeed = 0f;
    private float gravityStrength = 50f;
    private float maxFallSpeed = 15f;

    private float minHorizontalInput = 0.0001f;
    private float minVerticalInput = 0.0001f;

    private float jumpSpeed = 15f;

    private float runSpeed = 5f;
    private float maxRunSpeed = 2f;

    private bool isOnGround;
    private bool isTouchingWall;
    private float acceleration = 0f;

    private int horizontalDirection = 1;

    [SerializeField]
    private Transform playerContainer;

    [SerializeField]
    private Transform wallRaycastPosition;
    [SerializeField]
    private Transform floorRaycastPosition;

    [SerializeField]
    private LayerMask floorLayer;

    [SerializeField]
    private LayerMask wallLayer;

    private float horizontalMovementPerFrame = 0f;

    private float fallSpeed = 0f;

    private float horizontalInput = 0f;

    private float size = 1f;

    // Update is called once per frame
    void Update()
    {
        CheckOnGroundStatus();
        CheckWallStatus();
        if (Input.GetKeyDown(KeyCode.P))
        {
            transform.position = new Vector2(0, 5f);
        }
        float verticalInput = Input.GetAxisRaw("Vertical");
        if (isOnGround && (verticalInput > minVerticalInput))
        {
            verticalSpeed = jumpSpeed;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput > minHorizontalInput)
        {
            horizontalSpeed = runSpeed * Time.deltaTime;
            SetPlayerDirection(1);
        }
        else if (horizontalInput < -minHorizontalInput)
        {
            horizontalSpeed = -runSpeed * Time.deltaTime;
            SetPlayerDirection(-1);
        }
        else
        {
            horizontalSpeed = 0f;
        }
        if (isTouchingWall)
        {
            horizontalSpeed = 0f;
        }

        if (gravity && !isOnGround)
        {
            acceleration = Time.deltaTime * gravityStrength;
            verticalSpeed -= acceleration / 2.0f;
        }
        Vector3 pos = transform.position;
        float vertSpeed = isOnGround ? Mathf.Clamp(verticalSpeed, 0, verticalSpeed) : verticalSpeed;
        fallSpeed = Mathf.Clamp(vertSpeed, -maxFallSpeed, jumpSpeed);
        horizontalMovementPerFrame = Mathf.Clamp(horizontalSpeed, -maxRunSpeed, maxRunSpeed);
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

    private void SetPlayerDirection(int direction)
    {
        horizontalDirection = direction;
        Vector2 scale = playerContainer.localScale;
        scale.x = horizontalDirection;
        playerContainer.localScale = scale;
    }

    private void CheckWallStatus()
    {
        Vector3 topRaycastOrigin = wallRaycastPosition.position - new Vector3(0f, size * 0.5f);
        Vector3 botRaycastOrigin = wallRaycastPosition.position + new Vector3(0f, size * 0.5f);

        isTouchingWall = RayCastToWall(topRaycastOrigin) || RayCastToWall(botRaycastOrigin);
    }

    private bool RayCastToWall(Vector3 raycastOrigin)
    {
        float rayDistance = Mathf.Abs(horizontalMovementPerFrame) + size;
        RaycastHit2D ray = Physics2D.Raycast(raycastOrigin, horizontalDirection * Vector2.right, rayDistance, wallLayer);
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

                    pos.x += Mathf.Clamp(Mathf.Abs(runSpeed * Time.deltaTime), 0, maxMove) * horizontalDirection;
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
        if (verticalSpeed > 0)
        {
            isOnGround = false;
            return;
        }

        Vector3 leftRaycastOrigin = floorRaycastPosition.position - new Vector3(size * 0.5f, 0f);
        Vector3 rightRaycastOrigin = floorRaycastPosition.position + new Vector3(size * 0.5f, 0f);

        isOnGround = RaycastToGround(leftRaycastOrigin) || RaycastToGround(rightRaycastOrigin);
    }


    private bool RaycastToGround(Vector3 raycastOrigin)
    {
        float rayDistance = Time.deltaTime * Mathf.Abs(fallSpeed * 2) + size;
        RaycastHit2D ray = Physics2D.Raycast(raycastOrigin, Vector2.down, rayDistance, floorLayer);
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
