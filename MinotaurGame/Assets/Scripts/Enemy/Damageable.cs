using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    private int HP;
    [SerializeField]
    private ParticleSystem dieEffect;

    // Start is called before the first frame update
    void Start()
    {
        HP = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Kill()
    {
        Destroy(gameObject);
        if (dieEffect != null)
        {
            var fx = Instantiate(dieEffect);
            fx.transform.position = transform.position;
        }
    }
}
