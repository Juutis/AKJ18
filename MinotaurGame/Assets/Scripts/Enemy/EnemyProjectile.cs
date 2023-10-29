using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private int dir = 0;
    private float radius = 0.5f;
    private float projectileSpeed = 10f;
    private int damage = 1;

    [SerializeField]
    private CollidingEntity collidingEntity;

    [SerializeField]
    private LayerMask projectileTargetMask;
    public bool IsActive { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        collidingEntity.SetGravity(false);
    }

    // Update is called once per frame
    void Update()
    {
        collidingEntity.SetHorizontalInput(projectileSpeed * dir);

        if (damage > 0)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0f, projectileTargetMask);
            if (hit.collider != null)
            {
                Damageable damageableEntity = hit.collider.GetComponent<Damageable>();

                Debug.Log($"Killed {damageableEntity.name}!");
                damageableEntity.Kill();
            }
        }
    }

    public void Deactivate()
    {
        transform.position = new(10000, 10000);
        IsActive = false;
        collidingEntity.SetHorizontalInput(0);
        collidingEntity.SetGravity(false);
        collidingEntity.Reset();
    }

    public void Activate(Vector3 pos, int direction)
    {
        collidingEntity.Init();
        transform.position = pos;
        IsActive = true;
        dir = direction;
        collidingEntity.SetGravity(false);
        collidingEntity.SetWallCallback(HitWall);
        collidingEntity.Reset();
        collidingEntity.SetCheckFloor(false);
        damage = 1;
    }

    public void HitWall()
    {
        Deactivate();
    }
}
