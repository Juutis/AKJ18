using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumping : MonoBehaviour
{
    [SerializeField]
    private CollidingEntity collidingEntity;

    private float lastJump = 0f;
    private float jumpCD = 2f;
    private float jumpSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        lastJump = jumpCD;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(lastJump);
        if ((Time.time - lastJump) < jumpCD)
        {
            collidingEntity.SetVerticalInput(0);
            return;
        }

        collidingEntity.SetVerticalInput(jumpSpeed);
        lastJump = Time.time;
    }
}
