using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    public float speed = 8f;
    public bool isLeft = true;

    [Header("Yapay zeka (sadece sag raket icin)")]
    public bool aiControlled = false;
    public Transform ball;
    public float aiSpeed = 5f;

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // Sag raketteyken 1 = tek kisilik (AI), 2 = iki kisilik
        if (!isLeft)
        {
            if (kb.digit1Key.wasPressedThisFrame) aiControlled = true;
            if (kb.digit2Key.wasPressedThisFrame) aiControlled = false;
        }

        float move = 0f;
        float currentSpeed = speed;

        if (aiControlled && ball != null)
        {
            float diff = ball.position.y - transform.position.y;
            if (Mathf.Abs(diff) > 0.3f)
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