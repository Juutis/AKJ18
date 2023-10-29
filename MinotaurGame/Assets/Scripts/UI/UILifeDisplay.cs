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

    public void Clear()
    {
        for (int index = lifeList.Count - 1; index >= 0; index -= 1)
        {
            UILife life = lifeList[index];
            lifeList.Remove(life);
            Destroy(life);
        }
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
