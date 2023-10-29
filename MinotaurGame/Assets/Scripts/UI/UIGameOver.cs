using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIGameOver : MonoBehaviour
{

    public static UIGameOver main;
    void Awake()
    {
        main = this;
    }

    [SerializeField]
    private List<UIButton> uIButtons;

    private int selectedIndex = 0;

    [SerializeField]
    private GameObject container;

    private bool isShown = false;

    private float minVerticalInput = 0.001f;

    void Start()
    {
        foreach (UIButton button in uIButtons)
        {
            button.Init();
        }
    }

    public void Show()
    {
        SelectButton();
        container.SetActive(true);
        isShown = true;
    }

    public void Hide()
    {
        selectedIndex = 0;
        container.SetActive(false);
        isShown = false;
    }


    private void SelectButton()
    {
        foreach (UIButton button in uIButtons)
        {
            button.Deselect();
        }
        UIButton selectedButton = uIButtons[selectedIndex];
        selectedButton.Select();
    }

    private void SelectNextButton()
    {
        if (selectedIndex < uIButtons.Count - 1)
        {
            selectedIndex += 1;
            SelectButton();
        }
    }
    private void SelectPreviousButton()
    {
        if (selectedIndex > 0)
        {
            selectedIndex -= 1;
            SelectButton();
        }
    }

    void Update()
    {
        if (isShown)
        {
            float verticalAxis = Input.GetAxisRaw("Vertical");
            if (verticalAxis > minVerticalInput)
            {
                SelectPreviousButton();
            }
            else if (verticalAxis < -minVerticalInput)
            {
                SelectNextButton();
            }
            if (Input.GetButtonDown("Fire1"))
            {
                UIManager.main.CloseCurtains(delegate
                {
                    uIButtons[selectedIndex].PerformAction();
                    Hide();
                });
            }
        }
    }
}
