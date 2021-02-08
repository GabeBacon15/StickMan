using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStick : MonoBehaviour
{
    public GameObject joystick;
    public RectTransform stickTransform;
    private RectTransform joystickTransform;
    private Vector2 jsCenter;
    private int id;
    private Touch joyStickTouch;

    // Start is called before the first frame update
    void Start()
    {
        id = -1;
        joystickTransform = joystick.GetComponent<RectTransform>();
        jsCenter = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            checkForJoystickTouch(i, 100, 550, 100, 550);
        }
    }


    //JOYSTICK METHODS
    private void checkForJoystickTouch(int i, int xmin, int xmax, int ymin, int ymax)
    {
        Vector2 tempPos = Input.touches[i].position;
        if (tempPos.x <= xmax && tempPos.x >= xmin && tempPos.y <= ymax && tempPos.y >= ymin && id < 0 && Input.touches[i].phase.Equals(TouchPhase.Began))
        {
            id = Input.touches[i].fingerId;
            joystickTransform.position = Input.touches[i].position;
            jsCenter = Input.touches[i].position;
        }
    }
    private Vector2 handelJoystick(int range)
    {
        Vector2 jsInput = Vector2.zero;
        if (id > -1)
        {
            foreach (Touch t in Input.touches)
            {
                if (t.fingerId == id)
                    joyStickTouch = t;
            }
        }
        if (id > -1)
        {
            joystick.SetActive(true);
            jsInput = getJSInput(jsCenter, joyStickTouch.position, range, stickTransform);
            if (joyStickTouch.phase.Equals(TouchPhase.Ended) || joyStickTouch.phase.Equals(TouchPhase.Canceled))
            {
                id = -1;
                joystick.SetActive(false);
            }
        }
        return jsInput;
    }
    private Vector2 getJSInput(Vector2 center, Vector2 touchPos, int range, RectTransform stickTransform)
    {
        Vector2 input = Vector2.zero;
        Vector2 pos = touchPos - center;
        if (Mathf.Abs(pos.x) < range && Mathf.Abs(pos.y) < range)
        {
            input = pos / range;
        }
        else
        {
            float angle = Mathf.Atan2(pos.y, pos.x);
            input = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        stickTransform.localPosition = input * range;

        return input;
    }
    public Vector3 getInputFromJoystick3(int range)
    {
        Vector2 temp = handelJoystick(range);
        Vector3 final = Vector3.zero;
        int coefX = (int)(temp.x / Mathf.Abs(temp.x));
        int coefY = (int)(temp.y / Mathf.Abs(temp.y));
        final = new Vector3(Mathf.Pow(temp.x, 2) * coefX, 0f, Mathf.Pow(temp.y, 2) * coefY);
        return final;
    }
    public Vector2 getInputFromJoystick2(int range)
    {
        Vector2 temp = handelJoystick(range);
        Vector2 final = Vector2.zero;
        int coefX = (int)(temp.x / Mathf.Abs(temp.x));
        int coefY = (int)(temp.y / Mathf.Abs(temp.y));
        final = new Vector3(Mathf.Pow(temp.x, 2) * coefX, Mathf.Pow(temp.y, 2) * coefY);
        return final;
    }
}
