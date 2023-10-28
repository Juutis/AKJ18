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

    private int numOfProjectiles = 2;
    private List<Projectile> projectiles = new List<Projectile>();

    // Start is called before the first frame update
    void Start()
    {
        collidingEntity = GetComponent<CollidingEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Fire1"))
        {
            if (projectiles.Count < numOfProjectiles)
            {
                GameObject projectileInstance = Instantiate(projectilePrefab);
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
