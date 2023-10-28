using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int dir = 0;
    private float currentSpeed = 0f;
    private float projectileSpeed = 10f;
    private float size = 0.75f;
    private float lifeTime = 2f;
    private float startLifetime = 0f;

    [SerializeField]
    private CollidingEntity collidingEntity;

    public bool IsActive { get; private set; }

    void Update()
    {
        // Vector3 pos = transform.position;
        // pos.x += currentSpeed * Time.deltaTime;
        // transform.position = pos;
        collidingEntity.SetHorizontalInput(currentSpeed * dir);

        if (Time.time - startLifetime > lifeTime)
        {
            Deactivate();
        }
    }

    public void Deactivate()
    {
        transform.position = new(10000, 10000);
        currentSpeed = 0f;
        IsActive = false;
        startLifetime = 0f;
        collidingEntity.SetHorizontalInput(0);
        collidingEntity.SetGravity(false);
        collidingEntity.Reset();
        currentSpeed = 0;
    }

    public void Activate(Vector3 pos, int direction)
    {
        collidingEntity.Init();
        currentSpeed = projectileSpeed;
        transform.position = pos;
        IsActive = true;
        startLifetime = Time.time;
        dir = direction;
        collidingEntity.SetGravity(false);
        collidingEntity.SetWallCallback(HitWall);
        collidingEntity.Reset();
        collidingEntity.SetCheckFloor(false);
    }

    public void HitWall()
    {
        collidingEntity.SetGravity(true);
        collidingEntity.SetCheckFloor(true);
    }
}
