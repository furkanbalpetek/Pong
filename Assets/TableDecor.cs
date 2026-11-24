using UnityEngine;

public class TableDecor : MonoBehaviour
{
    public Color background = new Color(0.07f, 0.10f, 0.17f);
    public Color tableColor = new Color(0.12f, 0.34f, 0.62f);
    public Color lineColor = new Color(0.95f, 0.97f, 1f);
    public Color leftPaddleColor = new Color(0.91f, 0.28f, 0.24f);
    public Color rightPaddleColor = new Color(0.95f, 0.78f, 0.25f);
    public Color ballColor = new Color(1f, 0.55f, 0.1f);

    static Sprite whiteSprite;

    static Sprite WhiteSprite()
    {
        if (whiteSprite == null)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            whiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }
        return whiteSprite;
    }

    void Awake()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = background;
        }

        BuildTable();

        ColorObject("PaddleLeft", leftPaddleColor);
        ColorObject("PaddleRight", rightPaddleColor);
        ColorObject("Ball", ballColor);
        ColorObject("WallTop", lineColor);
        ColorObject("WallBottom", lineColor);

        AddTrail();
    }

    void BuildTable()
{
    // Masa yuzeyi
    MakeRect("TableSurface", new Vector2(0, 0), new Vector2(40f, 9.5f), tableColor, -10);

    // Orta uzun cizgi
    Color soft = lineColor;
    soft.a = 0.45f;
    MakeRect("CenterLine", new Vector2(0, 0), new Vector2(40f, 0.06f), soft, -9);

    // File: tam boy
    Color netSoft = lineColor;
    netSoft.a = 0.30f;
    MakeRect("NetStrip", new Vector2(0, 0), new Vector2(0.18f, 9.5f), netSoft, -8);

    for (float y = -4.4f; y < 4.5f; y += 0.6f)
    {
        MakeRect("NetDash", new Vector2(0, y), new Vector2(0.12f, 0.35f), lineColor, -7);
    }
}

    GameObject MakeRect(string objName, Vector2 pos, Vector2 size, Color color, int order)
    {
        GameObject go = new GameObject(objName);
        go.transform.SetParent(transform, false);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = WhiteSprite();
        sr.color = color;
        sr.sortingOrder = order;
        return go;
    }

    void ColorObject(string objName, Color color)
    {
        GameObject go = GameObject.Find(objName);
        if (go == null) return;
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = color;
    }

    void AddTrail()
    {
        GameObject ball = GameObject.Find("Ball");
        if (ball == null || ball.GetComponent<TrailRenderer>() != null) return;

        TrailRenderer tr = ball.AddComponent<TrailRenderer>();
        tr.time = 0.25f;
        tr.widthMultiplier = 0.45f;
        tr.material = new Material(Shader.Find("Sprites/Default"));
        tr.sortingOrder = 5;

        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(ballColor, 0f), new GradientColorKey(ballColor, 1f) },
            new[] { new GradientAlphaKey(0.6f, 0f), new GradientAlphaKey(0f, 1f) });
        tr.colorGradient = g;
    }
}