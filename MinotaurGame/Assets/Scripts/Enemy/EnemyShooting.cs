using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private CollidingEntity collidingEntity;

    private int numOfProjectiles = 4;
    private List<EnemyProjectile> projectiles = new List<EnemyProjectile>();
    private float lastShot = 0f;
    private float shootCD = 1.5f;


    // Start is called before the first frame update
    void Start()
    {
        collidingEntity = GetComponent<CollidingEntity>();
        lastShot = shootCD;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.time - lastShot) < shootCD) return;

        if (projectiles.Count < numOfProjectiles)
        {
            GameObject projectileInstance = Instantiate(projectilePrefab);
            EnemyProjectile projectile = projectileInstance.GetComponent<EnemyProjectile>();
            projectile.Activate(transform.position, collidingEntity.HorizontalDirection);
            projectiles.Add(projectile);
        }
        else
        {
            IEnumerable<EnemyProjectile> actives = projectiles.Where(x => !x.IsActive);
            if (actives.Any())
            {
                actives.First().Activate(transform.position, collidingEntity.HorizontalDirection);
            }
        }

        lastShot = Time.time;
    }
}
