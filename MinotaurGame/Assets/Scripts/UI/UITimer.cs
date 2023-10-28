using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{

    public static UITimer main;
    [SerializeField]
    private TextMeshProUGUI txtTimer;
    public Timer timer { private get; set; }

    void Awake()
    {
        main = this;
    }

    void Update()
    {
        if (timer != null)
        {
            txtTimer.text = timer.GetString();
        }
    }
}