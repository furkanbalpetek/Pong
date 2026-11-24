using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    static readonly Color navy = new Color(0.06f, 0.09f, 0.16f, 1f);
    static readonly Color tableBlue = new Color(0.12f, 0.34f, 0.62f, 1f);
    static readonly Color lineWhite = new Color(0.95f, 0.97f, 1f, 1f);
    static readonly Color orange = new Color(1f, 0.55f, 0.10f, 1f);
    static readonly Color paddleRed = new Color(0.91f, 0.28f, 0.24f, 1f);
    static readonly Color paddleYellow = new Color(0.95f, 0.78f, 0.25f, 1f);
    static readonly Color winGreen = new Color(0.35f, 0.85f, 0.45f, 1f);
    static readonly Color loseRed = new Color(0.95f, 0.38f, 0.32f, 1f);

    static Sprite roundedSprite;
    static Sprite circleSprite;

    Ball ball;
    MenuManager menu;
    Transform boardRoot, endRoot;

    TMP_Text scoreL, scoreR, labelL, labelR, titleText, bigScore, subText;
    GameObject endPanel;
    RectTransform endCard;

    Camera gameCam;
    int lastSw, lastSh;
    int lastL, lastR;
    float punchL, punchR, showT;
    bool wasOver;

    void Start()
    {
        ball = FindAnyObjectByType<Ball>();
        menu = FindAnyObjectByType<MenuManager>();

        // Hide the old score/message texts
        foreach (string n in new[] { "ScoreText", "MessageText" })
        {
            GameObject g = GameObject.Find(n);
            if (g != null) g.SetActive(false);
        }

        // Keep the playfield 16:9 on every screen so paddles are never cropped
        gameCam = Camera.main != null ? Camera.main : FindAnyObjectByType<Camera>();
        CreateLetterboxCamera();
        FitCamera();

        // Scoreboard canvas: drawn BEHIND the ball and paddles (above the table)
        boardRoot = MakeCanvas("BoardCanvas", 1, true);
        // End screen canvas: drawn on top of everything
        endRoot = MakeCanvas("EndCanvas", 50, true);

        if (GetComponent<Juice>() == null) gameObject.AddComponent<Juice>();

        // Put ball, paddles and the ball trail clearly above the scoreboard
        foreach (SpriteRenderer sr in FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))
        {
            string n = sr.gameObject.name;
            if (n == "Ball" || n == "PaddleLeft" || n == "PaddleRight") sr.sortingOrder = 10;
        }
        if (ball != null)
        {
            TrailRenderer tr = ball.GetComponent<TrailRenderer>();
            if (tr != null) tr.sortingOrder = 10;
        }

        BuildBoard();
        BuildEndPanel();
    }

    void Update()
    {
        if (Screen.width != lastSw || Screen.height != lastSh) FitCamera();

        if (ball == null) return;

        if (ball.scoreLeft != lastL)
        {
            if (ball.scoreLeft > lastL) punchL = 1f;
            lastL = ball.scoreLeft;
            scoreL.text = lastL.ToString();
        }
        if (ball.scoreRight != lastR)
        {
            if (ball.scoreRight > lastR) punchR = 1f;
            lastR = ball.scoreRight;
            scoreR.text = lastR.ToString();
        }

        punchL = Mathf.MoveTowards(punchL, 0f, Time.unscaledDeltaTime * 3f);
        punchR = Mathf.MoveTowards(punchR, 0f, Time.unscaledDeltaTime * 3f);
        scoreL.rectTransform.localScale = Vector3.one * (1f + 0.35f * punchL);
        scoreR.rectTransform.localScale = Vector3.one * (1f + 0.35f * punchR);

        labelL.text = GameSettings.singlePlayer ? "YOU" : "LEFT";
        labelR.text = GameSettings.singlePlayer ? "BOT LV " + GameSettings.aiLevel : "RIGHT";

        if (ball.gameOver && !wasOver) ShowEnd();
        if (!ball.gameOver && wasOver) endPanel.SetActive(false);
        wasOver = ball.gameOver;

        if (endPanel.activeSelf)
        {
            showT += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(showT / 0.25f);
            k = 1f - (1f - k) * (1f - k);
            endCard.localScale = Vector3.one * Mathf.Lerp(0.8f, 1f, k);
        }
    }

    void CreateLetterboxCamera()
    {
        // Black background behind the 16:9 playfield (fills the bars)
        GameObject go = new GameObject("LetterboxCamera");
        Camera c = go.AddComponent<Camera>();
        c.clearFlags = CameraClearFlags.SolidColor;
        c.backgroundColor = Color.black;
        c.cullingMask = 0;
        c.depth = -100f;
    }

    void FitCamera()
    {
        lastSw = Screen.width;
        lastSh = Screen.height;
        if (gameCam == null || lastSh == 0) return;

        float target = 16f / 9f;
        float window = (float)lastSw / lastSh;
        Rect r;
        if (window < target)
        {
            float h = window / target;
            r = new Rect(0f, (1f - h) / 2f, 1f, h);
        }
        else
        {
            float w = target / window;
            r = new Rect((1f - w) / 2f, 0f, w, 1f);
        }
        gameCam.rect = r;
    }

    Transform MakeCanvas(string canvasName, int order, bool behindBall)
    {
        GameObject go = new GameObject(canvasName, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas c = go.GetComponent<Canvas>();
        Camera cam = Camera.main != null ? Camera.main : FindAnyObjectByType<Camera>();
        if (behindBall && cam != null)
        {
            c.renderMode = RenderMode.ScreenSpaceCamera;
            c.worldCamera = cam;
            c.planeDistance = 10f;
        }
        else
        {
            c.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        c.sortingOrder = order;
        CanvasScaler cs = go.GetComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.matchWidthOrHeight = 0.5f;
        return go.transform;
    }

    // ---------- Scoreboard ----------
    void BuildBoard()
    {
        GameObject board = new GameObject("Board", typeof(RectTransform));
        board.transform.SetParent(boardRoot, false);
        RectTransform brt = board.GetComponent<RectTransform>();
        brt.anchorMin = new Vector2(0.5f, 1f);
        brt.anchorMax = new Vector2(0.5f, 1f);
        brt.sizeDelta = Vector2.zero;
        brt.anchoredPosition = new Vector2(0f, -90f);

        MakeScoreCard(board.transform, -125f, paddleRed, Color.white, out scoreL, out labelL);
        MakeScoreCard(board.transform, 125f, paddleYellow, navy, out scoreR, out labelR);

    }

    void MakeScoreCard(Transform parent, float x, Color cardColor, Color textColor, out TMP_Text score, out TMP_Text label)
    {
        CreateImage(parent, "Border", RoundedSprite(), lineWhite, new Vector2(x, 0), new Vector2(236, 146), true);
        CreateImage(parent, "Card", RoundedSprite(), cardColor, new Vector2(x, 0), new Vector2(220, 130), true);
        score = CreateText(parent, "0", 110, new Vector2(x, 0), new Vector2(220, 130), textColor, 0f);
        label = CreateText(parent, "", 34, new Vector2(x, -110), new Vector2(260, 44), lineWhite, 4f);
    }

    // ---------- End screen ----------
    void BuildEndPanel()
    {
        endPanel = new GameObject("EndPanel", typeof(RectTransform), typeof(Image));
        endPanel.transform.SetParent(endRoot, false);
        RectTransform prt = endPanel.GetComponent<RectTransform>();
        prt.anchorMin = Vector2.zero;
        prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero;
        prt.offsetMax = Vector2.zero;
        endPanel.GetComponent<Image>().color = new Color(0.02f, 0.04f, 0.10f, 0.72f);

        GameObject card = new GameObject("EndCard", typeof(RectTransform));
        card.transform.SetParent(endPanel.transform, false);
        endCard = card.GetComponent<RectTransform>();
        endCard.sizeDelta = Vector2.zero;

        CreateImage(card.transform, "Border", RoundedSprite(), lineWhite, Vector2.zero, new Vector2(1116, 656), true);
        CreateImage(card.transform, "Face", RoundedSprite(), tableBlue, Vector2.zero, new Vector2(1100, 640), true);

        titleText = CreateText(card.transform, "", 110, new Vector2(0, 205), new Vector2(1000, 130), orange, 6f);
        titleText.enableAutoSizing = true;
        titleText.fontSizeMin = 40;
        titleText.fontSizeMax = 110;
        titleText.outlineWidth = 0.2f;
        titleText.outlineColor = navy;

        bigScore = CreateText(card.transform, "", 150, new Vector2(0, 55), new Vector2(800, 170), Color.white, 10f);
        subText = CreateText(card.transform, "", 46, new Vector2(0, -65), new Vector2(1000, 60), lineWhite, 6f);

        CreateButton(card.transform, "PLAY AGAIN", 46, new Vector2(-200, -185), new Vector2(360, 110), orange, () =>
        {
            if (ball != null) ball.StartNewGame();
        });
        CreateButton(card.transform, "MENU", 46, new Vector2(200, -185), new Vector2(360, 110), new Color(0.10f, 0.20f, 0.42f, 1f), () =>
        {
            if (menu != null) menu.ShowMain();
        });

        CreateText(card.transform, "SPACE: PLAY AGAIN      ESC: MENU", 28, new Vector2(0, -280), new Vector2(900, 40), new Color(0.82f, 0.9f, 1f, 1f), 3f);

        endPanel.SetActive(false);
    }

    void ShowEnd()
    {
        titleText.text = ball.resultTitle;
        if (GameSettings.singlePlayer) titleText.color = ball.leftWon ? winGreen : loseRed;
        else titleText.color = orange;

        bigScore.text = ball.scoreLeft + "  -  " + ball.scoreRight;
        subText.text = ball.resultSub;

        showT = 0f;
        endCard.localScale = Vector3.one * 0.8f;
        endPanel.SetActive(true);
    }

    // ---------- Helpers ----------
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
}
