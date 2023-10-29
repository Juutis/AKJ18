using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILife : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    public void Kill()
    {
        animator.Play("lifeKill");
    }

    public void KillFinished()
    {
        Destroy(gameObject);
    }
}
