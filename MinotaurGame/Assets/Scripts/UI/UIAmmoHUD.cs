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
