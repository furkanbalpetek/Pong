using UnityEngine;
using UnityEngine.InputSystem;

public class Ball : MonoBehaviour
{
    public float startSpeed = 7f;
    public float speedIncrease = 0.4f;
    public float maxSpeed = 15f;
    public int winScore = 5;

    // GameUI reads these
    public int scoreLeft;
    public int scoreRight;
    public bool gameOver;
    public bool leftWon;
    public string resultTitle = "";
    public string resultSub = "";

    Rigidbody2D rb;
    AudioSource audioSource;
    AudioClip paddleClip, wallClip, scoreClip, winClip;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        paddleClip = SoundMaker.Beep("paddle", 660f, 0.08f);
        wallClip = SoundMaker.Beep("wall", 330f, 0.06f);
        scoreClip = SoundMaker.Beep("score", 180f, 0.4f);
        winClip = SoundMaker.Beep("win", 880f, 0.6f);

        StartNewGame();
    }

    void Play(AudioClip clip)
    {
        if (clip != null) audioSource.PlayOneShot(clip);
    }

    public void StartNewGame()
    {
        scoreLeft = 0;
        scoreRight = 0;
        gameOver = false;
        resultTitle = "";
        resultSub = "";
        Launch();
    }

    void Launch()
    {
        transform.position = Vector3.zero;

        TrailRenderer tr = GetComponent<TrailRenderer>();
        if (tr != null) tr.Clear();

        float dirX = Random.value < 0.5f ? -1f : 1f;
        float dirY = Random.Range(0.2f, 0.5f) * (Random.value < 0.5f ? -1f : 1f);
        rb.linearVelocity = new Vector2(dirX, dirY).normalized * startSpeed;
    }

    void EndGame()
    {
        gameOver = true;
        leftWon = scoreLeft > scoreRight;
        rb.linearVelocity = Vector2.zero;
        transform.position = Vector3.zero;
        Play(winClip);

        if (GameSettings.singlePlayer)
        {
            if (leftWon)
            {
                GameSettings.MarkBeaten(GameSettings.aiLevel);
                resultTitle = "YOU WIN!";
                resultSub = "LEVEL " + GameSettings.aiLevel + " CLEARED";
            }
            else
            {
                resultTitle = "YOU LOSE";
                resultSub = "THE COMPUTER WINS";
            }
        }
        else
        {
            resultTitle = leftWon ? "LEFT PLAYER WINS!" : "RIGHT PLAYER WINS!";
            resultSub = "";
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Paddle>() != null)
        {
            float newSpeed = Mathf.Min(rb.linearVelocity.magnitude + speedIncrease, maxSpeed);

            float half = collision.collider.bounds.extents.y;
            float offset = (transform.position.y - collision.transform.position.y) / half;
            offset = Mathf.Clamp(offset, -1f, 1f);
            if (Mathf.Abs(offset) < 0.15f)
                offset = 0.15f * (Random.value < 0.5f ? -1f : 1f);

            float dirX = transform.position.x > collision.transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(dirX, offset * 0.8f).normalized * newSpeed;

            Play(paddleClip);
        }
        else
        {
            Play(wallClip);
        }
    }

    void KeepAngle()
    {
        Vector2 v = rb.linearVelocity;
        float speed = v.magnitude;
        if (speed < 0.01f) return;

        float minY = speed * 0.15f;
        if (Mathf.Abs(v.y) < minY)
        {
            float newY = (v.y >= 0f ? 1f : -1f) * minY;
            rb.linearVelocity = new Vector2(v.x, newY).normalized * speed;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (gameOver)
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                StartNewGame();
            return;
        }

        KeepAngle();

        if (transform.position.x > 9.5f)
        {
            scoreLeft++;
            if (scoreLeft >= winScore) EndGame();
            else { Play(scoreClip); Launch(); }
        }
        else if (transform.position.x < -9.5f)
        {
            scoreRight++;
            if (scoreRight >= winScore) EndGame();
            else { Play(scoreClip); Launch(); }
        }
    }
}
