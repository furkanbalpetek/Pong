using System.Collections.Generic;
using UnityEngine;

// Hit sparks, goal bursts, win confetti and screen shake. Added automatically by GameUI.
public class Juice : MonoBehaviour
{
    class Particle
    {
        public Transform t;
        public SpriteRenderer sr;
        public Vector2 vel;
        public float life, maxLife, gravity, spin, size;
        public Color color;
    }

    static readonly Color orange = new Color(1f, 0.55f, 0.10f, 1f);
    static readonly Color paddleRed = new Color(0.91f, 0.28f, 0.24f, 1f);
    static readonly Color paddleYellow = new Color(0.95f, 0.78f, 0.25f, 1f);

    static readonly Color[] confettiColors =
    {
        new Color(0.91f, 0.28f, 0.24f, 1f),
        new Color(0.95f, 0.78f, 0.25f, 1f),
        new Color(0.35f, 0.85f, 0.45f, 1f),
        new Color(0.30f, 0.65f, 1f, 1f),
        new Color(1f, 0.55f, 0.10f, 1f),
        Color.white
    };

    Ball ball;
    Camera cam;
    Vector3 camBase;
    float shake;
    Sprite square;
    readonly List<Particle> live = new List<Particle>();
    readonly Stack<Particle> pool = new Stack<Particle>();

    void Start()
    {
        ball = FindAnyObjectByType<Ball>();
        cam = Camera.main != null ? Camera.main : FindAnyObjectByType<Camera>();
        if (cam != null) camBase = cam.transform.position;

        Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        Color[] px = new Color[16];
        for (int i = 0; i < px.Length; i++) px[i] = Color.white;
        tex.SetPixels(px);
        tex.Apply();
        square = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f); // 1 unit wide

        if (ball != null)
        {
            ball.onHit += OnHit;
            ball.onScore += OnScore;
            ball.onWin += OnWin;
        }
    }

    void OnDestroy()
    {
        if (ball != null)
        {
            ball.onHit -= OnHit;
            ball.onScore -= OnScore;
            ball.onWin -= OnWin;
        }
    }

    // ---------- Events ----------
    void OnHit(Vector2 pos, bool paddle)
    {
        if (paddle)
        {
            Burst(pos, 12, orange, 6f, 0.35f, 0.20f, 0f);
            Burst(pos, 6, Color.white, 4f, 0.25f, 0.14f, 0f);
            shake = Mathf.Max(shake, 0.05f);
        }
        else
        {
            Burst(pos, 6, Color.white, 3.5f, 0.25f, 0.13f, 0f);
        }
    }

    void OnScore(Vector2 pos, bool leftScored)
    {
        float x = Mathf.Clamp(pos.x, -9f, 9f);
        Color c = leftScored ? paddleRed : paddleYellow;
        Burst(new Vector2(x, pos.y), 36, c, 10f, 0.6f, 0.28f, 4f);
        Burst(new Vector2(x, pos.y), 14, Color.white, 7f, 0.45f, 0.18f, 4f);
        shake = Mathf.Max(shake, 0.28f);
    }

    void OnWin(bool leftWon)
    {
        for (int i = 0; i < 140; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-9.5f, 9.5f), Random.Range(5.5f, 12f));
            Vector2 vel = new Vector2(Random.Range(-1.5f, 1.5f), -Random.Range(0.5f, 3f));
            Spawn(pos, vel, confettiColors[Random.Range(0, confettiColors.Length)],
                  Random.Range(3f, 4.5f), Random.Range(0.2f, 0.38f), 5f, Random.Range(-360f, 360f));
        }
        shake = Mathf.Max(shake, 0.4f);
    }

    // ---------- Particles ----------
    void Burst(Vector2 pos, int count, Color color, float speed, float life, float size, float gravity)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            Spawn(pos, dir * speed * Random.Range(0.4f, 1f), color,
                  life * Random.Range(0.7f, 1.2f), size * Random.Range(0.6f, 1.2f), gravity, Random.Range(-200f, 200f));
        }
    }

    void Spawn(Vector2 pos, Vector2 vel, Color color, float life, float size, float gravity, float spin)
    {
        Particle p;
        if (pool.Count > 0)
        {
            p = pool.Pop();
        }
        else
        {
            GameObject go = new GameObject("Particle");
            go.transform.SetParent(transform, false);
            p = new Particle();
            p.t = go.transform;
            p.sr = go.AddComponent<SpriteRenderer>();
            p.sr.sprite = square;
            p.sr.sortingOrder = 100;
        }

        p.t.position = pos;
        p.t.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        p.vel = vel;
        p.life = p.maxLife = life;
        p.gravity = gravity;
        p.spin = spin;
        p.size = size;
        p.color = color;
        p.sr.color = color;
        p.sr.enabled = true;
        live.Add(p);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        for (int i = live.Count - 1; i >= 0; i--)
        {
            Particle p = live[i];
            p.life -= dt;
            if (p.life <= 0f)
            {
                p.sr.enabled = false;
                live.RemoveAt(i);
                pool.Push(p);
                continue;
            }

            p.vel.y -= p.gravity * dt;
            p.t.position += (Vector3)(p.vel * dt);
            p.t.Rotate(0f, 0f, p.spin * dt);

            float k = p.life / p.maxLife;
            p.sr.color = new Color(p.color.r, p.color.g, p.color.b, Mathf.Clamp01(k * 1.5f));
            p.t.localScale = Vector3.one * p.size * Mathf.Lerp(0.4f, 1f, k);
        }
    }

    void LateUpdate()
    {
        if (cam == null) return;
        if (shake > 0.003f)
        {
            cam.transform.position = camBase + (Vector3)(Random.insideUnitCircle * shake);
            shake *= Mathf.Exp(-9f * Time.unscaledDeltaTime);
        }
        else
        {
            shake = 0f;
            cam.transform.position = camBase;
        }
    }
}
