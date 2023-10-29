using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField]
    private UIButtonAction action;

    [SerializeField]
    private Image imgButtonBg;
    [SerializeField]
    private Image imgButtonOutline;
    [SerializeField]
    private TextMeshProUGUI txtTitle;
    [SerializeField]
    private Color txtHighlightColor;
    private Color txtOriginalColor;
    [SerializeField]
    private Color bgHighlightColor;
    private Color bgOriginalColor;
    [SerializeField]
    private Color outlineHighlightColor;
    private Color outlineOriginalColor;
    private RectTransform rectTransform;
    private float selectedYRot = 15f;

    public void Init()
    {
        txtOriginalColor = txtTitle.color;
        bgOriginalColor = imgButtonBg.color;
        outlineOriginalColor = imgButtonOutline.color;
        rectTransform = GetComponent<RectTransform>();
    }

    public void Select()
    {
        txtTitle.color = txtHighlightColor;
        imgButtonBg.color = bgHighlightColor;
        imgButtonOutline.color = outlineHighlightColor;
        rectTransform.eulerAngles = new Vector3(0, selectedYRot, 0);
    }

    public void Deselect()
    {
        txtTitle.color = txtOriginalColor;
        imgButtonBg.color = bgOriginalColor;
        imgButtonOutline.color = outlineOriginalColor;
        rectTransform.eulerAngles = Vector3.zero;
    }

    public void PerformAction()
    {
        if (action == UIButtonAction.Continue)
        {
            GameManager.main.ContinueAction();
        }
        if (action == UIButtonAction.Restart)
        {
            GameManager.main.RestartAction();
        }
    }
}

public enum UIButtonAction
{
    Continue,
    Restart
}
