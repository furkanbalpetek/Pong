using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    public float speed = 8f;
    public bool isLeft = true;

    void Update()
    {
        float move = 0f;
        var kb = Keyboard.current;
        if (kb == null) return;

        if (isLeft)
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
        pos.y = Mathf.Clamp(pos.y + move * speed * Time.deltaTime, -3.7f, 3.7f);
        transform.position = pos;
    }
}