using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    private bool gravity = true;

    private float horizontalSpeed = 0f;
    private float verticalSpeed = 0f;
    private float gravityStrength = 50f;
    private float maxFallSpeed = 10f;

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

    private float floorRayDistance = 0.6f;
    private float wallRayDistance = 0.6f;

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

        float horizontalInput = Input.GetAxisRaw("Horizontal");
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
        pos.y += Mathf.Clamp(vertSpeed, -maxFallSpeed, jumpSpeed) * Time.deltaTime;
        pos.x += Mathf.Clamp(horizontalSpeed, -maxRunSpeed, maxRunSpeed);
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
        RaycastHit2D ray = Physics2D.Raycast(raycastOrigin, horizontalDirection * Vector2.right, wallRayDistance, wallLayer);
        Debug.DrawRay(raycastOrigin, horizontalDirection * Vector2.right, Color.blue);
        if (ray.collider != null)
        {
            if (!isTouchingWall)
            {
                //Vector3 pos = transform.position;
                //pos.y = ray.point.x + size * 0.5f;
                //transform.position = pos;
            }
            Debug.Log("We are hitting wall.");
            return true;
        }
        else
        {
            Debug.Log("We are NOT hitting wall");
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
        RaycastHit2D ray = Physics2D.Raycast(raycastOrigin, Vector2.down, floorRayDistance, floorLayer);
        Debug.DrawRay(raycastOrigin, Vector2.down, Color.red);
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
