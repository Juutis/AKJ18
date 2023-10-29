using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int dir = 0;
    private int damage = 1;
    private float currentSpeed = 0f;
    private float projectileSpeed = 10f;

    private float startLifetime = 0f;

    private float radius = 0.75f;

    private int comboKillCount = 0;

    [SerializeField]
    private LayerMask projectileTargetMask;

    [SerializeField]
    private CollidingEntity collidingEntity;

    public bool IsActive { get; private set; }

    [SerializeField]
    private PickupableItem pickupItemPrefab;

    private bool floorHasBeenHit = false;

    [SerializeField]
    private PickupableItem embeddedPickupablePrefab;
    private PickupableItem embeddedPickupable;



    void Update()
    {
        if (!wallHasBeenHit) {
            collidingEntity.SetHorizontalInput(currentSpeed * dir);
        }

        if (damage > 0)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0f, projectileTargetMask);
            if (hit.collider != null)
            {
                Damageable damageableEntity = hit.collider.GetComponent<Damageable>();

                Debug.Log($"Killed {damageableEntity.name}!");
                damageableEntity.Kill();
                comboKillCount++;

                SoundManager.main.PlaySound(GameSoundType.AxeHitEnemy);
                GameManager.main.ScoreKill(comboKillCount, hit.point);
            }
        }
    }

    public void PickedUp() {
        embeddedPickupable = null;
        Deactivate();
    }

    public void Deactivate()
    {
        transform.position = new(10000, 10000);
        currentSpeed = 0f;
        IsActive = false;
        wallHasBeenHit = false;
        startLifetime = 0f;
        collidingEntity.SetHorizontalInput(0);
        collidingEntity.SetGravity(false);
        collidingEntity.Reset();
        currentSpeed = 0;
        if (embeddedPickupable != null) {
            Destroy(embeddedPickupable.gameObject);
            embeddedPickupable = null;
        }
    }

    public void Activate(Vector3 pos, int direction)
    {
        SoundManager.main.PlaySound(GameSoundType.AxeThrow);
        floorHasBeenHit = false;
        collidingEntity.Init();
        currentSpeed = projectileSpeed;
        transform.position = pos;
        IsActive = true;
        startLifetime = Time.time;
        dir = direction;
        collidingEntity.SetGravity(false);
        collidingEntity.SetWallCallback(HitWall);
        collidingEntity.SetFloorCallback(HitFloor);
        collidingEntity.Reset();
        collidingEntity.SetCheckFloor(false);
        damage = 1;
        comboKillCount = 0;
    }

    private bool wallHasBeenHit = false;

    public void HitWall()
    {
        collidingEntity.SetGravity(true);
        collidingEntity.SetCheckFloor(true);
        collidingEntity.SetHorizontalInput(0);
        if (!wallHasBeenHit)
        {
            SoundManager.main.PlaySound(GameSoundType.AxeHitWall);
        }
        wallHasBeenHit = true;
        if (embeddedPickupable == null) {
            embeddedPickupable = Instantiate(embeddedPickupablePrefab, transform);
            embeddedPickupable.ParentProjectile = this;
        }
    }

    public void HitFloor()
    {
        damage = 0;
        if (!floorHasBeenHit)
        {
            PickupableItem pickupItem = Instantiate(pickupItemPrefab, transform.position, Quaternion.identity, transform.parent);
            floorHasBeenHit = true;
        }
        Deactivate();
        // GameManager.main.ScoreKill(comboKillCount);
        comboKillCount = 0;
        if (embeddedPickupable != null) {
            Destroy(embeddedPickupable.gameObject);
            embeddedPickupable = null;
        }
    }
}
