using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPoppingText : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private TextMeshProUGUI txtMessage;

    public void Show(Vector3 position, string message)
    {
        //transform.position = new Vector3(position.x, position.y + 2f, position.z);
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 offsetPos = Camera.main.WorldToScreenPoint(position);
        offsetPos.x -= rectTransform.sizeDelta.x / 2;
        Debug.Log(rectTransform.sizeDelta.x);
        transform.position = offsetPos;
        //txtMessage.color = color;
        txtMessage.text = message;
        //txtMessage.fontSize = fontSize;
        animator.Play("poppingTextShow");
    }

    public void ShowFinished()
    {
        Destroy(gameObject);
    }
}

