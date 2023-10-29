using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private CollidingEntity collidingEntity;

    private int numOfProjectiles = 20;
    private List<Projectile> projectiles = new List<Projectile>();
    private bool canShoot = false;

    // Start is called before the first frame update
    void Start()
    {
        collidingEntity = GetComponent<CollidingEntity>();
        Invoke("EnableInput", 0.75f);
    }

    public void EnableInput() {
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canShoot) return;

        if (Input.GetButtonDown("Fire1"))
        {
            if (!GameManager.main.SpendProjectile())
            {
                return;
            }
            if (projectiles.Count < numOfProjectiles)
            {
                GameObject projectileInstance = Instantiate(projectilePrefab, transform.parent);
                Projectile projectile = projectileInstance.GetComponent<Projectile>();
                projectile.Activate(transform.position, collidingEntity.HorizontalDirection);
                projectiles.Add(projectile);
            }
            else
            {
                IEnumerable<Projectile> actives = projectiles.Where(x => !x.IsActive);
                if (actives.Any())
                {
                    actives.First().Activate(transform.position, collidingEntity.HorizontalDirection);
                }
            }
        }
    }
}
