using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HanteiImage : MonoBehaviour {

    [SerializeField]
    AnimationCurve PosyCurve, AlphaCurve;

    [SerializeField]
    Sprite sPF, sGD, sMS, sFA, sSL;

    float timer = 0.0f;

    public void Init(int bgk, int smk=0)
    {
        switch(bgk)
        {
            case 4:
                transform.Find("SpriteBigHantei").GetComponent<SpriteRenderer>().sprite = sPF;
                break;
            case 3:
                transform.Find("SpriteBigHantei").GetComponent<SpriteRenderer>().sprite = sPF;
                break;
            case 2:
                transform.Find("SpriteBigHantei").GetComponent<SpriteRenderer>().sprite = sGD;
                break;
            case 1:
            default:
                transform.Find("SpriteBigHantei").GetComponent<SpriteRenderer>().sprite = sMS;
                break;
        }
        switch (smk)
        {
            case 2:
                transform.Find("SpriteSmallHantei").GetComponent<SpriteRenderer>().sprite = sSL;
                break;
            case 1:
                transform.Find("SpriteSmallHantei").GetComponent<SpriteRenderer>().sprite = sFA;
                break;
            default:
                transform.Find("SpriteSmallHantei").GetComponent<SpriteRenderer>().sprite = null;
                break;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        timer += Time.deltaTime;
        //位置
        var pos = transform.position;
        pos.y = PosyCurve.Evaluate(timer);
        transform.position = pos;

        //半透明
        Color c = new Color(1, 1, 1, AlphaCurve.Evaluate(timer));
        transform.Find("SpriteBigHantei").GetComponent<SpriteRenderer>().color = c;
        transform.Find("SpriteSmallHantei").GetComponent<SpriteRenderer>().color = c;

        //タイムアップ
        if(timer>0.3f)
        {
            Destroy(gameObject);
        }
    }
}
