using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTime : MonoBehaviour
{
    public static BulletTime Main;

    private float timer;
    private float duration = 0.2f;
    private float strength = 0.9f;
    private bool triggered = false;

    void Awake() {
        Main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!triggered) return;

        var t = (timer + duration - Time.time) / duration;

        if (t >= 0.0f) {
            Time.timeScale = 1 - (t * strength);
        } else {
            Time.timeScale = 1.0f;
        }
    }

    public void Trigger() {
        timer = Time.time;
        triggered = true;
    }
}
