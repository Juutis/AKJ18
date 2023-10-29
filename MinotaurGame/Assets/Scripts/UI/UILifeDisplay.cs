using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILifeDisplay : MonoBehaviour
{
    [SerializeField]
    private UILife uiLifePrefab;
    [SerializeField]
    private Transform lifeContainer;

    private List<UILife> lifeList = new();

    public static UILifeDisplay main;
    void Awake()
    {
        main = this;
    }

    public void AddLife()
    {
        UILife life = Instantiate(uiLifePrefab, lifeContainer);
        lifeList.Add(life);
    }

    public void RemoveLife()
    {
        if (lifeList.Count > 0)
        {
            UILife life = lifeList[0];
            lifeList.Remove(life);
            life.Kill();
        }
    }
}
