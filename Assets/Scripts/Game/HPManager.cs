using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPManager : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer[] Sprites;//length:20

    [SerializeField]
    Sprite[] spr;

    public TheGameManager manager;

    public float HPNOW=100.0f, HPMAX=100.0f;

    bool isCheap = false;

    public void Init(float hp,float hpmax)
    {
        HPNOW = hp;
        HPMAX = hpmax;
    }

    // Update is called once per frame
    void Update()
    {
        int progress = (int)(HPNOW / HPMAX * 100.0f);

        int lightnum = Mathf.Min(16, (int)(HPNOW / HPMAX * 16.0f));

        int progresslength = ((int)HPNOW).ToString().Length;

        for (int i = 0; i < lightnum; i++)
        {
            if (Sprites[i].sprite != spr[11])
            {
                Sprites[i].sprite = spr[11];
            }
        }
        for (int i = 0; i < progresslength; i++)
        {
            Sprites[lightnum + i].sprite = spr[int.Parse(((int)HPNOW).ToString().Substring(i, 1))];
        }
        Sprites[lightnum + progresslength].sprite = spr[10];

        if (lightnum + progresslength < 19)
        {
            for (int i = 19; i > lightnum + progresslength; i--)
            {
                Sprites[i].sprite = null;
            }
        }

        for(int i=0;i<20;i++)
        {
            if(i== lightnum-1)
            {
                Sprites[i].color = new Color(1.0f, 1.0f, 1.0f, Random.Range(0.1f, 0.8f));
            }
            else
            {
                Sprites[i].color = new Color(1.0f, 1.0f, 1.0f, Random.Range(0.9f, 1.0f));
            }
        }

    }
    
    public void HPDOWN(float down)
    {
        HPNOW -= down;
        HPUPDATE();
    }
    public void HPUP(float up)
    {
        if (!isCheap)
        {
            HPNOW += up;
            HPUPDATE();
        }
    }

    public void HPUPDATE()
    {
        if (HPNOW <= 0 && !isCheap)
        {
            isCheap = true;
            
        }
        if (isCheap)
        {
            HPNOW = 0.0f;
        }
        else
        {
            HPNOW = Mathf.Clamp(HPNOW, 0, HPMAX);
        }

        manager.hp = HPNOW;
        manager.hpmax = HPMAX;
    }
}
