using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class InputManager : MonoBehaviour
{
    static int TOUCH_MAX = 20;

    [SerializeField]
    AnimationCurve curve;

    public float LeftPoint, RightPoint, HeightPoint;

    public List<TouchData> touchPoint, oldPoint, triggerPoint, releasePoint;
    public List<float> triggerTime = new List<float>();

    [SerializeField]
    SpriteRenderer[] Beams;
    float[] BeamsAlpha = new float[16];

    [SerializeField]
    Text debugtext;

    public class TouchData
    {
        public bool isTouch;
        public float posx;
        public float posy;
        public TouchData(bool i, float x, float y)
        {
            isTouch = i;
            posx = x;
            posy = y;
        }
    }

    int updatecd = 30;

    // Use this for initialization
    void Start()
    {
        //配列初期化
        touchPoint = new List<TouchData>();
        oldPoint = new List<TouchData>();
        triggerPoint = new List<TouchData>();
        releasePoint = new List<TouchData>();

        //座標原点規定
        LeftPoint = Camera.main.WorldToScreenPoint(new Vector3(-8, 0, 0)).x;
        RightPoint = Camera.main.WorldToScreenPoint(new Vector3(+8, 0, 0)).x;
        HeightPoint = Camera.main.WorldToScreenPoint(new Vector3(+8, 0, 0)).y;

        Keyframe[] keys = new Keyframe[2];
        keys[0] = new Keyframe(LeftPoint - (RightPoint - LeftPoint), -16.0f);
        keys[1] = new Keyframe(RightPoint + (RightPoint - LeftPoint), 32.0f);

        LinearKeyframe(keys);
        curve = new AnimationCurve(keys);

        for (int i = 0; i < TOUCH_MAX; i++)
        {
            touchPoint.Add(new TouchData(false, -1, -1));
            triggerPoint.Add(new TouchData(false, -1, -1));
            releasePoint.Add(new TouchData(false, -1, -1));
            oldPoint.Add(new TouchData(false, -1, -1));

            triggerTime.Add(0.0f);
        }

    }//start end

    // Update is called once per frame
    void Update()
    {
        if(updatecd>0)
        {
            updatecd--;
            return;
        }

        //座標原点規定
        LeftPoint = Camera.main.WorldToScreenPoint(new Vector3(-8, 0, 0)).x;
        RightPoint = Camera.main.WorldToScreenPoint(new Vector3(+8, 0, 0)).x;
        HeightPoint = Camera.main.WorldToScreenPoint(new Vector3(+8, 0, 0)).y;
        Keyframe[] keys = new Keyframe[2];
        keys[0] = new Keyframe(LeftPoint - (RightPoint - LeftPoint), -16.0f);
        keys[1] = new Keyframe(RightPoint + (RightPoint - LeftPoint), 32.0f);
        LinearKeyframe(keys);
        curve = new AnimationCurve(keys);

        //タッチ操作によるデータ記録
        for (int i = 0; i < TOUCH_MAX; i++)
        {
            oldPoint[i] = new TouchData(touchPoint[i].isTouch, touchPoint[i].posx, touchPoint[i].posy);

            touchPoint[i].isTouch = false;
            triggerPoint[i].isTouch = false;
            releasePoint[i].isTouch = false;

            triggerTime[i] = 0.0f;
        }

        //BeamsAlpha計算
        for (int i = 0; i < 16; i++)
        {
            if (BeamsAlpha[i] > 0)
            {
                BeamsAlpha[i] -= Time.deltaTime * 6.0f;
                if (BeamsAlpha[i] <= 0)
                {
                    BeamsAlpha[i] = 0.0f;
                }
            }
            Beams[i].color = new Color(Beams[i].color.r, Beams[i].color.g, Beams[i].color.b, BeamsAlpha[i]);
        }

        //タッチ入力

        if (Input.touchCount > 0)
        {

            for (int cnt = 0; cnt < Mathf.Min(TOUCH_MAX, Input.touchCount); cnt++)
            {
                Vector2 pos = new Vector2(curve.Evaluate(Input.GetTouch(cnt).position.x), Input.GetTouch(cnt).position.y);

                touchPoint[Input.GetTouch(cnt).fingerId] = new TouchData(true, pos.x, pos.y);

                if (Input.GetTouch(cnt).phase == TouchPhase.Began)
                {
                    triggerPoint[Input.GetTouch(cnt).fingerId] = new TouchData(true, pos.x, pos.y);
                    if (pos.x >= 0.0f && pos.x < 16.0f)
                    {
                        Beams[(int)pos.x].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        BeamsAlpha[(int)pos.x] = 1.0f;
                    }
                }
                else if (Input.GetTouch(cnt).phase == TouchPhase.Ended)
                {
                    releasePoint[Input.GetTouch(cnt).fingerId] = new TouchData(true, pos.x, pos.y);
                }
            }
        }
        //テスト用
#if UNITY_EDITOR
        //マウス座標入力
        if (Input.GetMouseButton(0))
        {
            Vector2 pos = new Vector2(curve.Evaluate(Input.mousePosition.x), Input.mousePosition.y);

            touchPoint[0] = new TouchData(true, pos.x, pos.y);

            if (Input.GetMouseButtonDown(0))
            {
                triggerPoint[0] = new TouchData(true, pos.x, pos.y);
                if (pos.x >= 0.0f && pos.x < 16.0f)
                {
                    Beams[(int)pos.x].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    BeamsAlpha[(int)pos.x] = 1.0f;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 pos = new Vector2(curve.Evaluate(Input.mousePosition.x), Input.mousePosition.y);
            releasePoint[0] = new TouchData(true, pos.x, pos.y);
        }
        if (touchPoint.Count > 0)
        {
            //print("tp:" + touchPoint[0].x + " " + touchPoint[0].y);
        }
        if (oldPoint.Count > 0)
        {
            //print("od:" + oldPoint[0].x + " " + oldPoint[0].y);
        }
#endif

        debugtext.text = "";
        for (int i = 0; i < 10; i++)
        {
            debugtext.text += touchPoint[i].isTouch + " | " + triggerPoint[i].isTouch + " | " + releasePoint[i].isTouch + " | " + oldPoint[i].isTouch + "\n";
        }

    }//update end

    //タッチしてるかどうか取得
    public bool GetPressed(float left, float right)
    {

        for (int i = 0; i < TOUCH_MAX; i++)
        {
            if (touchPoint[i].isTouch)
            {
                if (touchPoint[i].posx > left - 1 && touchPoint[i].posx < right + 1)
                {
                    return true;
                }
            }
        }

        return false;
    }
    //接触瞬間取得
    public bool GetTrigger(float left, float right, float time = 0.0f)
    {
        for (int i = 0; i < TOUCH_MAX; i++)
        {
            if (triggerPoint[i].isTouch)
            {
                if (triggerPoint[i].posx > left - 1 && triggerPoint[i].posx < right + 1)
                {
                    if (triggerTime[i] == 0 || (triggerTime[i] == time))
                    {
                        triggerTime[i] = time;
                        return true;
                    }
                }
            }
        }

        return false;
    }



    //離す瞬間取得
    public bool GetRelease(float left, float right)
    {
        for (int i = 0; i < TOUCH_MAX; i++)
        {
            if (touchPoint[i].isTouch)
            {
                if (releasePoint[i].posx > left - 1 && releasePoint[i].posx < right + 1)
                {
                    return true;
                }
            }
        }

        return false;
    }
    //FLICK瞬間取得
    public bool GetFlick(float left, float right)
    {

        for (int i = 0; i < TOUCH_MAX; i++)
        {
            if (touchPoint[i].isTouch && oldPoint[i].isTouch)
            {
                if (touchPoint[i].posx > left - 1 && touchPoint[i].posx < right + 1)
                {
                    if (Mathf.Abs(oldPoint[i].posx - touchPoint[i].posx) > 0.1f)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    //方向付きフリック判定
    public bool GetFlickLeft(float left, float right)
    {
        for (int i = 0; i < TOUCH_MAX; i++)
        {
            if (touchPoint[i].isTouch && oldPoint[i].isTouch)
            {
                if (touchPoint[i].posx > left - 1 && touchPoint[i].posx < right + 1)
                {
                    if (touchPoint[i].posx < oldPoint[i].posx)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    public bool GetFlickRight(float left, float right)
    {
        for (int i = 0; i < TOUCH_MAX; i++)
        {
            if (touchPoint[i].isTouch && oldPoint[i].isTouch)
            {
                if (touchPoint[i].posx > left - 1 && touchPoint[i].posx < right + 1)
                {
                    if (touchPoint[i].posx > oldPoint[i].posx)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    public bool GetFlickUp(float left, float right)
    {
        for (int i = 0; i < TOUCH_MAX; i++)
        {
            if (touchPoint[i].isTouch && oldPoint[i].isTouch)
            {
                if (touchPoint[i].posx > left - 1 && touchPoint[i].posx < right + 1)
                {
                    if (touchPoint[i].posy > oldPoint[i].posy)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    public bool GetFlickDown(float left, float right)
    {
        for (int i = 0; i < TOUCH_MAX; i++)
        {
            if (touchPoint[i].isTouch && oldPoint[i].isTouch)
            {
                if (touchPoint[i].posx > left - 1 && touchPoint[i].posx < right + 1)
                {
                    if (touchPoint[i].posy < oldPoint[i].posy)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void LinearKeyframe(Keyframe[] keys)
    {
        if (keys.Length >= 2)
        {
            for (int i = 0; i < keys.Length - 1; i++)
            {
                keys[i].outTangent = keys[i + 1].inTangent = (keys[i + 1].value - keys[i].value) / (keys[i + 1].time - keys[i].time);
            }
        }
    }

    public void SetBeamColor(float start, float end, Color col)
    {
        int s = Mathf.FloorToInt(start);
        int e = Mathf.CeilToInt(end);
        for (int i = s; i < e; i++)
        {
            if (i >= 0 && i < 16)
            {
                Beams[i].color = col;
                BeamsAlpha[i] = 1.0f;
            }
        }
    }

    


}
