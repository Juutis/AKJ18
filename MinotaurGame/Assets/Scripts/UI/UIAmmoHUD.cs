using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAmmoHUD : MonoBehaviour
{
    [SerializeField]
    private GameObject uiAmmoPrefab;
    [SerializeField]
    private Transform ammoContainer;

    private List<GameObject> ammoList = new();

    public static UIAmmoHUD main;
    void Awake()
    {
        main = this;
    }

    public void Clear()
    {
        for (int index = ammoList.Count - 1; index >= 0; index -= 1)
        {
            GameObject ammo = ammoList[index];
            ammoList.Remove(ammo);
            Destroy(ammo);
        }
    }

    public void AddAmmo()
    {
        GameObject ammo = Instantiate(uiAmmoPrefab, ammoContainer);
        ammoList.Add(ammo);
    }

    public void RemoveAmmo()
    {
        if (ammoList.Count > 0)
        {
            GameObject ammo = ammoList[0];
            ammoList.Remove(ammo);
            Destroy(ammo);
        }
    }
}
