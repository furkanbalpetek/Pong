using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Ball : MonoBehaviour
{
    public float startSpeed = 7f;
    public float speedIncrease = 0.4f;
    public float maxSpeed = 15f;
    public int winScore = 5;
    public TMP_Text scoreText;
    public TMP_Text messageText;

    Rigidbody2D rb;
    int scoreLeft;
    int scoreRight;
    bool gameOver;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartNewGame();
    }

    void StartNewGame()
    {
        scoreLeft = 0;
        scoreRight = 0;
        gameOver = false;
        if (messageText != null) messageText.text = "";
        UpdateScoreText();
        Launch();
    }

    void Launch()
    {
        transform.position = Vector3.zero;
        float dirX = Random.value < 0.5f ? -1f : 1f;
        float dirY = Random.Range(-0.5f, 0.5f);
        rb.linearVelocity = new Vector2(dirX, dirY).normalized * startSpeed;
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = scoreLeft + " - " + scoreRight;
    }

    void EndGame()
    {
        gameOver = true;
        rb.linearVelocity = Vector2.zero;
        transform.position = Vector3.zero;
        string winner = scoreLeft > scoreRight ? "SOL OYUNCU" : "SAG OYUNCU";
        if (messageText != null)
            messageText.text = winner + " KAZANDI!\nTEKRAR OYNAMAK ICIN SPACE TUSUNA BASINIZ.";
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Paddle>() != null)
        {
            float newSpeed = Mathf.Min(rb.linearVelocity.magnitude + speedIncrease, maxSpeed);
            rb.linearVelocity = rb.linearVelocity.normalized * newSpeed;
        }
    }

    void Update()
    {
        if (gameOver)
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                StartNewGame();
            return;
        }

        if (transform.position.x > 9.5f)
        {
            scoreLeft++;
            UpdateScoreText();
            if (scoreLeft >= winScore) EndGame(); else Launch();
        }
        else if (transform.position.x < -9.5f)
        {
            scoreRight++;
            UpdateScoreText();
            if (scoreRight >= winScore) EndGame(); else Launch();
        }
    }
}