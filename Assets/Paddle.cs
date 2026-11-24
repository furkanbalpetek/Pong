using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    public float speed = 8f;
    public bool isLeft = true;

    [Header("Yapay zeka")]
    public bool aiControlled = false;
    public Transform ball;
    public float aiSpeed = 5f;
    public float aiDeadzone = 0.3f;

    void Start()
    {
        if (ball == null)
        {
            GameObject b = GameObject.Find("Ball");
            if (b != null) ball = b.transform;
        }
    }

    public void SetAI(bool on, int level)
    {
        aiControlled = on;
        float t = Mathf.Clamp01((level - 1) / 9f);
        aiSpeed = Mathf.Lerp(2.5f, 8.5f, t);
        aiDeadzone = Mathf.Lerp(0.9f, 0.2f, t);
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        float move = 0f;
        float currentSpeed = speed;

        if (aiControlled && ball != null)
        {
            float diff = ball.position.y - transform.position.y;
            if (Mathf.Abs(diff) > aiDeadzone)
            {
                move = Mathf.Sign(diff);
                currentSpeed = aiSpeed;
            }
        }
        else if (isLeft)
        {
            if (kb.wKey.isPressed) move = 1f;
            if (kb.sKey.isPressed) move = -1f;
        }
        else
        {
            if (kb.upArrowKey.isPressed) move = 1f;
            if (kb.downArrowKey.isPressed) move = -1f;
        }

        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y + move * currentSpeed * Time.deltaTime, -3.7f, 3.7f);
        transform.position = pos;
    }
}