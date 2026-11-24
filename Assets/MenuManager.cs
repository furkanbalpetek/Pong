using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    // ---- Tema renkleri ----
    static readonly Color navy = new Color(0.06f, 0.09f, 0.16f, 1f);
    static readonly Color tableBlue = new Color(0.12f, 0.34f, 0.62f, 1f);
    static readonly Color lineWhite = new Color(0.95f, 0.97f, 1f, 1f);
    static readonly Color orange = new Color(1f, 0.55f, 0.10f, 1f);
    static readonly Color paddleRed = new Color(0.91f, 0.28f, 0.24f, 1f);
    static readonly Color paddleYellow = new Color(0.95f, 0.78f, 0.25f, 1f);

    static Sprite roundedSprite;
    static Sprite circleSprite;

    Ball ball;
    Paddle rightPaddle;
    Transform canvasRoot;
    GameObject mainPanel;
    GameObject difficultyPanel;
    readonly GameObject[] ticks = new GameObject[11];
    bool inMenu = true;

    void Start()
    {
        ball = FindAnyObjectByType<Ball>();
        foreach (Paddle p in FindObjectsByType<Paddle>(FindObjectsSortMode.None))
            if (!p.isLeft) rightPaddle = p;

        SetupEventSystem();
        CreateCanvas();
        BuildMainPanel();
        BuildDifficultyPanel();
        ShowMain();
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;
        if (kb.escapeKey.wasPressedThisFrame)
        {
            if (!inMenu || difficultyPanel.activeSelf) ShowMain();
        }
    }

    // ---------- Akis ----------
    public void ShowMain()
    {
        inMenu = true;
        Time.timeScale = 0f;
        mainPanel.SetActive(true);
        difficultyPanel.SetActive(false);
    }

    void ShowDifficulty()
    {
        mainPanel.SetActive(false);
        difficultyPanel.SetActive(true);
        for (int l = 1; l <= 10; l++)
            ticks[l].SetActive(GameSettings.IsBeaten(l));
    }

    void StartTwoPlayer()
    {
        GameSettings.singlePlayer = false;
        StartGame();
    }

    void OnLevel(int level)
    {
        GameSettings.singlePlayer = true;
        GameSettings.aiLevel = level;
        StartGame();
    }

    void StartGame()
    {
        mainPanel.SetActive(false);
        difficultyPanel.SetActive(false);
        inMenu = false;

        if (rightPaddle != null)
            rightPaddle.SetAI(GameSettings.singlePlayer, GameSettings.aiLevel);
        if (ball != null)
            ball.StartNewGame();

        Time.timeScale = 1f;
    }

    // ---------- Kurulum ----------
    void SetupEventSystem()
    {
        EventSystem es = FindAnyObjectByType<EventSystem>();
        if (es == null) es = new GameObject("EventSystem").AddComponent<EventSystem>();

        if (es.GetComponent<InputSystemUIInputModule>() == null)
        {
            StandaloneInputModule old = es.GetComponent<StandaloneInputModule>();
            if (old != null) Destroy(old);
            es.gameObject.AddComponent<InputSystemUIInputModule>();
        }
    }

    void CreateCanvas()
    {
        GameObject go = new GameObject("MenuCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas c = go.GetComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 100;

        CanvasScaler cs = go.GetComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.matchWidthOrHeight = 0.5f;

        canvasRoot = go.transform;
    }

    void BuildMainPanel()
    {
        mainPanel = CreatePanel("MainPanel", new Vector2(1300, 900));

        TMP_Text title = CreateText(mainPanel.transform, "PONG", 260, new Vector2(0, 290), new Vector2(1100, 300), orange, 30f);
        title.outlineWidth = 0.22f;
        title.outlineColor = navy;

        CreateText(mainPanel.transform, "TABLE TENNIS", 52, new Vector2(0, 110), new Vector2(900, 80), lineWhite, 40f);

        CreateButton(mainPanel.transform, "SINGLE PLAYER", 56, new Vector2(0, -40), new Vector2(720, 140), orange, ShowDifficulty);
        CreateButton(mainPanel.transform, "TWO PLAYERS", 56, new Vector2(0, -210), new Vector2(720, 140), orange, StartTwoPlayer);

        CreateText(mainPanel.transform, "LEFT: W / S      RIGHT: UP / DOWN ARROW      MENU: ESC", 34,
            new Vector2(0, -370), new Vector2(1200, 60), new Color(0.82f, 0.9f, 1f, 1f), 2f);

        AddDecor(mainPanel.transform);
    }

    void BuildDifficultyPanel()
    {
        difficultyPanel = CreatePanel("DifficultyPanel", new Vector2(1500, 960));

        TMP_Text title = CreateText(difficultyPanel.transform, "SELECT LEVEL", 120, new Vector2(0, 360), new Vector2(1400, 170), orange, 20f);
        title.outlineWidth = 0.2f;
        title.outlineColor = navy;

        CreateText(difficultyPanel.transform, "1 = EASY      10 = HARD      BEAT A LEVEL TO EARN A TICK", 36,
            new Vector2(0, 240), new Vector2(1400, 60), new Color(0.82f, 0.9f, 1f, 1f), 2f);

        for (int level = 1; level <= 10; level++)
        {
            int l = level;
            int col = (level - 1) % 5;
            int row = (level - 1) / 5;
            Vector2 pos = new Vector2((col - 2) * 270, 60 - row * 210);
            Color c = Color.Lerp(new Color(0.20f, 0.72f, 0.38f, 1f), new Color(0.88f, 0.27f, 0.27f, 1f), (level - 1) / 9f);

            Button b = CreateButton(difficultyPanel.transform, level.ToString(), 76, pos, new Vector2(210, 150), c, () => OnLevel(l));
            ticks[level] = CreateTick(b.transform);
        }

        CreateButton(difficultyPanel.transform, "BACK", 48, new Vector2(0, -340), new Vector2(340, 100), new Color(0.10f, 0.20f, 0.42f, 1f), ShowMain);
    }

    void AddDecor(Transform parent)
    {
        CreateImage(parent, "DecorPaddleL", RoundedSprite(), paddleRed, new Vector2(-820, -60), new Vector2(50, 300), true);
        CreateImage(parent, "DecorPaddleR", RoundedSprite(), paddleYellow, new Vector2(820, 90), new Vector2(50, 300), true);
        CreateImage(parent, "DecorBall", CircleSprite(), orange, new Vector2(-735, 140), new Vector2(46, 46), false);
    }

    // ---------- Sprite uretimi ----------
    static Sprite RoundedSprite()
    {
        if (roundedSprite == null)
        {
            int size = 64;
            int radius = 24;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float cx = Mathf.Clamp(x + 0.5f, radius, size - radius);
                    float cy = Mathf.Clamp(y + 0.5f, radius, size - radius);
                    float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(cx, cy));
                    float a = Mathf.Clamp01(radius - d + 0.5f);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            }
            tex.Apply();
            roundedSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0,
                SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
        }
        return roundedSprite;
    }

    static Sprite CircleSprite()
    {
        if (circleSprite == null)
        {
            int size = 128;
            float r = size / 2f;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r));
                    float a = Mathf.Clamp01(r - d);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            }
            tex.Apply();
            circleSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }
        return circleSprite;
    }

    // ---------- UI yardimcilari ----------
    GameObject CreatePanel(string panelName, Vector2 cardSize)
    {
        GameObject go = new GameObject(panelName, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(canvasRoot, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        go.GetComponent<Image>().color = navy;

        CreateImage(go.transform, "CardBorder", RoundedSprite(), lineWhite, Vector2.zero, cardSize + new Vector2(16, 16), true);
        CreateImage(go.transform, "Card", RoundedSprite(), tableBlue, Vector2.zero, cardSize, true);
        return go;
    }

    Image CreateImage(Transform parent, string objName, Sprite sprite, Color color, Vector2 pos, Vector2 size, bool sliced)
    {
        GameObject go = new GameObject(objName, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = go.GetComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        img.type = sliced ? Image.Type.Sliced : Image.Type.Simple;
        img.raycastTarget = false;
        return img;
    }

    TMP_Text CreateText(Transform parent, string text, float size, Vector2 pos, Vector2 boxSize, Color color, float spacing)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI t = go.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = size;
        t.fontStyle = FontStyles.Bold;
        t.characterSpacing = spacing;
        t.alignment = TextAlignmentOptions.Center;
        t.color = color;
        t.raycastTarget = false;
        t.rectTransform.anchoredPosition = pos;
        t.rectTransform.sizeDelta = boxSize;
        return t;
    }

    Button CreateButton(Transform parent, string label, float fontSize, Vector2 pos, Vector2 size, Color color, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = go.GetComponent<Image>();
        img.sprite = RoundedSprite();
        img.type = Image.Type.Sliced;
        img.color = color;

        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.4f);
        shadow.effectDistance = new Vector2(0f, -8f);

        Button b = go.GetComponent<Button>();
        b.targetGraphic = img;
        ColorBlock cb = b.colors;
        cb.normalColor = new Color(0.92f, 0.92f, 0.92f, 1f);
        cb.highlightedColor = Color.white;
        cb.pressedColor = new Color(0.72f, 0.72f, 0.72f, 1f);
        cb.selectedColor = new Color(0.92f, 0.92f, 0.92f, 1f);
        cb.fadeDuration = 0.08f;
        b.colors = cb;
        b.onClick.AddListener(onClick);

        CreateText(go.transform, label, fontSize, Vector2.zero, size, Color.white, 4f);
        return b;
    }

    GameObject CreateTick(Transform parent)
    {
        Image badge = CreateImage(parent, "Tick", CircleSprite(), Color.white, Vector2.zero, new Vector2(64, 64), false);
        RectTransform rt = badge.rectTransform;
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-4, -4);

        Color green = new Color(0.10f, 0.60f, 0.25f, 1f);
        CreateBar(badge.transform, new Vector2(-9, -6), new Vector2(8, 18), 45f, green);
        CreateBar(badge.transform, new Vector2(9, 0), new Vector2(8, 36), -45f, green);
        return badge.gameObject;
    }

    void CreateBar(Transform parent, Vector2 pos, Vector2 size, float angle, Color color)
    {
        GameObject go = new GameObject("Bar", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        rt.localRotation = Quaternion.Euler(0f, 0f, angle);
        Image img = go.GetComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
    }
}