using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectorLevel : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private int levelIndex;
    public LevelInfo LevelInfo { get; private set; }

    [SerializeField]
    private TMP_Text label;

    [SerializeField]
    private Image image;

    private float initialAlpha;
    private bool initialized = false;

    private float horizontalInput, verticalInput;
    private float inputMargin = 0.01f;
    private int maxLevels;
    private LevelSelectorGrid grid;
    private bool selected = false;
    private bool acceptInput = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init(int levelIndex, int maxLevels, LevelSelectorGrid grid) {
        this.levelIndex = levelIndex;
        this.maxLevels = maxLevels;
        this.grid = grid;
        initialAlpha = image.color.a;
        LevelInfo = new LevelInfo
        {
            LevelName = "Level " + (levelIndex + 1),
            BestTime = TimeSpan.FromMilliseconds(0)
        };
        label.text = (levelIndex + 1).ToString();
        if (levelIndex == GameManager.main.NextLevelIndex) {
            Select();
        }
        UpdateBestTime();
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        var newHorizontalInput = Input.GetAxis("Horizontal");
        var newVerticalInput = Input.GetAxis("Vertical");

        if (newHorizontalInput > inputMargin && horizontalInput < inputMargin) {
            selectLevel(levelIndex + 1);
        } else if (newHorizontalInput < -inputMargin && horizontalInput > -inputMargin) {
            selectLevel(levelIndex - 1);
        } else if (newVerticalInput > inputMargin && verticalInput < inputMargin) {
            selectLevel(levelIndex - 4);
        } else if (newVerticalInput < -inputMargin && verticalInput > -inputMargin) {
            selectLevel(levelIndex + 4);
        }
        if (selected && acceptInput && (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))) {
            GameManager.main.LoadLevel(levelIndex);
        }

        horizontalInput = newHorizontalInput;
        verticalInput = newVerticalInput;
        acceptInput = true;
    }

    private void selectLevel(int newLevelIndex) {
        if (selected && acceptInput && newLevelIndex >= 0 && newLevelIndex < maxLevels) {
            grid.Levels[newLevelIndex].Select();
        }
    }

    void OnEnable() {
        UpdateBestTime();
        if (initialized && levelIndex == GameManager.main.NextLevelIndex) {
            Select();
        }
    }

    public void Select() {
        UIManager.main.HoverLevel(this);
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.6f);
        selected = true;
        acceptInput = false;
    }

    public void Deselect() {
        image.color = new Color(image.color.r, image.color.g, image.color.b, initialAlpha);
        selected = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Select();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.main.LoadLevel(levelIndex);
    }

    public void UpdateBestTime() {
        if (LevelInfo == null) return;
        var bestTime = 0;
        if (PlayerPrefs.HasKey(levelIndex.ToString())) {
            bestTime = PlayerPrefs.GetInt(levelIndex.ToString());
        }
        LevelInfo.BestTime = TimeSpan.FromMilliseconds(bestTime);
    }
}
