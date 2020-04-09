using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheOnpu : MonoBehaviour
{
    TheGameManager gameManager;
    InputManager inputManager;
    SpriteRenderer spriteRenderer;
    [SerializeField]
    private Material _material1, _material2, _material3;
    private Mesh _mesh;
    public MeshDrawer mDrawer;

    public AnimationCurve acNSC;

    private Vector3[] _positions = new Vector3[]{
        new Vector3(1000, 1000, 0),
        new Vector3(1000, 1000, 0),
        new Vector3(1000, 1000, 0),
        new Vector3(1000, 1000, 0),
        new Vector3(1000, 1000, 0),
        new Vector3(1000, 1000, 0),
        new Vector3(1000, 1000, 0),
        new Vector3(1000, 1000, 0),
    };
    private int[] _triangle = new int[] {
        0, 1, 2,
        2, 1, 3,
        2, 3, 4,
        4, 3, 5,
        4, 5, 6,
        6, 5, 7,
    };
    private Vector3[] _normals = new Vector3[]{
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1),
    };
    private Vector2[] _uvs = new Vector2[]{
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(0.25f, 0),
        new Vector2(0.25f, 1),
        new Vector2(0.75f, 0),
        new Vector2(0.75f, 1),
        new Vector2(1, 0),
        new Vector2(1, 1),
    };

    public int id;
    public int kind;
    public float ichi;
    public float pos;
    public float width;
    public string nsc;
    public bool isnadnsc;
    public float insc;
    public int parent;
    float maxtime=0.0f;
    public string mode = "n";

    public float parent_ms;
    public float parent_dms;
    public float parent_pos;
    public float parent_width;

    public float center;
    public float ms;
    public float dms;

    public bool isNear;
    public bool isWaitForGD;
    public bool isWaitForPF;
    public float WaitForSec;

    public bool flag = false;


    float effect_center_start, effect_center_end;
    float angle_center_start, angle_center_end;

    static Color PJColor = new Color(1.0f, 0.9f, 0.1f, 1.0f), PFColor = new Color(0.9f, 0.4f, 0.1f, 1.0f), GDColor = new Color(0.1f, 0.8f, 0.1f, 1.0f);

    // Use this for initialization
    void Start()
    {
        _mesh = new Mesh();

        _mesh.vertices = _positions;
        _mesh.triangles = _triangle;
        _mesh.normals = _normals;
        _mesh.uv = _uvs;

        //_mesh.RecalculateBounds();

    }

    public void Ready(int i, float ic, float p, float c,string md, string n, float m, float d, float w, int k, bool near, int pa = 0, float pm = 0, float pd = 0, float pp = 0, float pw = 0)
    {
        id = i;
        ichi = ic;
        pos = p;
        center = c;
        mode = md;
        nsc = n;
        ms = m;
        dms = d;
        width = w;
        kind = k;
        parent = pa;
        parent_ms = pm;
        parent_dms = pd;
        parent_pos = pp;
        parent_width = pw;

        isNear = near;
        isWaitForGD = false;
        isWaitForPF = false;
        WaitForSec = 0.0f;

        spriteRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer arrorRenderer = transform.Find("Arror").GetComponent<SpriteRenderer>();

        spriteRenderer.size = new Vector2(width, 1.0f);
        if (kind == 6 || kind == 11 || kind == 12)
        {
            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.01f);
        }
        if (kind == 1)
        {
            arrorRenderer.sprite = gameManager.GetSpriteArror(0);
            arrorRenderer.transform.position += new Vector3(0, 0.6f, 0);
            arrorRenderer.transform.localScale += new Vector3( 2, 2, 0);
            arrorRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else if (kind == 2)
        {
            arrorRenderer.sprite = gameManager.GetSpriteArror(1);
            arrorRenderer.transform.position += new Vector3(0, 0.6f, 0);
            arrorRenderer.transform.localScale += new Vector3(2,2, 0);
            arrorRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else if (kind == 13)
        {
            arrorRenderer.sprite = gameManager.GetSpriteArror(2);
            arrorRenderer.transform.position += new Vector3(0, 1.2f, 0);
            arrorRenderer.transform.localScale += new Vector3(4, 4, 0);
            arrorRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        }
        else if (kind == 14)
        {
            arrorRenderer.sprite = gameManager.GetSpriteArror(3);
            arrorRenderer.transform.position += new Vector3(0, 1.2f, 0);
            arrorRenderer.transform.localScale += new Vector3(4, 4, 0);
            arrorRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        }
        else if (kind == 15)
        {
            arrorRenderer.sprite = gameManager.GetSpriteArror(4);
            arrorRenderer.transform.position += new Vector3(0, 1.2f, 0);
            arrorRenderer.transform.localScale += new Vector3(4, 4, 0);
            arrorRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        }
        else if (kind == 16)
        {
            arrorRenderer.sprite = gameManager.GetSpriteArror(5);
            arrorRenderer.transform.position += new Vector3(0, 1.2f, 0);
            arrorRenderer.transform.localScale += new Vector3(4, 4, 0);
            arrorRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        }
        else if (kind == 9)
        {
            arrorRenderer.sprite = gameManager.GetSpriteArror(6);
            arrorRenderer.transform.position += new Vector3(0, 1.2f, 0);
            arrorRenderer.transform.localScale += new Vector3(4, 4, 0);
            arrorRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        }
        else
        {
            arrorRenderer.enabled = false;
        }

        effect_center_start = (parent_pos + parent_width * 0.5f) / 16.0f;
        effect_center_end = (pos + width * 0.5f) / 16.0f;
        angle_center_start = (parent_pos + parent_width * 0.5f - 8.0f) / 4.0f;
        angle_center_end = (pos + width * 0.5f - 8.0f) / 4.0f;

        if(IsTail(kind))
        {
            nsc = "0";
        }

        if(mode=="P")
        {
            nsc = "1:0;0.9:0.31;0.8:0.59;0.7:0.81;0.6:0.95;0.5:1;0.4:0.95;0.3:0.81;0.2:0.59;0.1:0.31;0:0";
            isnadnsc = true;
            insc = 1.0f;
        }

        //nsc処理
        if(!nsc.Contains(":"))
        {
            isnadnsc = false;
            insc = float.Parse(nsc);
            if (insc == 0.0f) insc = 1.0f;
        }
        else
        {
            isnadnsc = true;
            insc = 1.0f;
            string[] nscs = nsc.Split(';');

            Keyframe[] nsckey = new Keyframe[nscs.Length];
            for(int ii=0;ii< nscs.Length;ii++)
            {
                nsckey[ii] = new Keyframe(gameManager.BPMCurve.Evaluate(ichi-float.Parse(nscs[ii].Split(':')[0])), gameManager.BPMCurve.Evaluate(ichi - float.Parse(nscs[ii].Split(':')[1])));
            }
            maxtime = nsckey[0].time;
            
            LinearKeyframe(nsckey);
            acNSC = new AnimationCurve(nsckey);
        }
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
    public void StartC()
    {
        //判定処理
        if (gameManager.GameAuto)
        {
            StartCoroutine(CHANTEIA());
        }
        else
        {
            StartCoroutine(CHANTEI());
        }

    }

    void Update()
    {
        //Effect処理
        if (gameManager.GameEffectParamEQLevel >= 1)
        {
            if (kind == 6 || kind == 7)
            {
                if (gameManager.SHINDO + 100 >= parent_ms && gameManager.SHINDO + 100 < ms)
                {
                    float p = ((float)gameManager.SHINDO + 100 - parent_ms) / (ms - parent_ms);
                    gameManager.AddEQ(effect_center_start + (effect_center_end - effect_center_start) * p);
                }
            }
        }

        //斜め処理
        if (IsTail(kind))
        {
            if (gameManager.SHINDO >= parent_ms && gameManager.SHINDO < ms)
            {
                float p = ((float)gameManager.SHINDO - parent_ms) / (ms - parent_ms);
                gameManager.AddAng(angle_center_start + (angle_center_end - angle_center_start) * p);
            }
        }

    }

    void DistroyThis()
    {
        //DESTROY
        Destroy(gameObject);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        double dshindo = gameManager.DSHINDO,shindo = gameManager.SHINDO;
        float z, pz, x, y;
        if (isnadnsc)
        {
            z = maxtime > shindo ? 300.0f : 0.01f * (ms - acNSC.Evaluate((float)shindo)) * gameManager.NoteSpeed;
            pz = 0.01f * (float)(parent_dms - dshindo);
        }
        else
        {
            z = 0.01f * (float)(dms - dshindo) * insc * gameManager.NoteSpeed;
            pz = 0.01f * (float)(parent_dms - dshindo) * gameManager.NoteSpeed;
        }
        if (mode == "L")
        {
            x = center - 8.0f - z * 0.2f;
        }
        else if (mode == "R")
        {
            x = center - 8.0f + z * 0.2f;
        }
        else
        {
            x = center - 8.0f;
        }
        if (mode == "H")
        {
            y = IsTail(kind) ? 0.0f : 0.1f + z * 0.1f;
        }
        else
        {
            y = IsTail(kind) ? 0.0f : 0.1f;
        }

        transform.position = new Vector3(x,y , z);
        spriteRenderer.size = new Vector2(width, 1.0f + (z / 80.0f));

        if (IsTail(kind))
        {
            _positions = new Vector3[]{
                new Vector3(parent_pos - 8.0f, -0.1f, pz),
                new Vector3(pos - 8.0f, -0.1f, z),
                new Vector3(parent_pos+0.4f - 8.0f, -0.1f, pz),
                new Vector3(pos+0.4f - 8.0f, -0.1f, z),
                new Vector3(parent_pos+parent_width-0.4f - 8.0f, -0.1f, pz),
                new Vector3(pos+width-0.4f - 8.0f, -0.1f, z),
                new Vector3(parent_pos+parent_width - 8.0f, -0.1f, pz),
                new Vector3(pos+width - 8.0f, -0.1f, z),
            };

            _mesh.vertices = _positions;
            _mesh.RecalculateBounds();
            //Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, _material, 0);
            if (kind == 4 || kind == 11)
            {
                mDrawer.AddQue(_mesh, _material1);
            }
            if (kind == 6 || kind == 7)
            {
                mDrawer.AddQue(_mesh, _material2);
            }
            if (kind == 17 || kind == 18)
            {
                mDrawer.AddQue(_mesh, _material3);
            }
        }
    }

    bool IsTail(int k)
    {
        if (k == 4) return true;
        if (k == 6) return true;
        if (k == 7) return true;
        if (k == 8) return true;
        if (k == 11) return true;
        if (k == 12) return true;
        if (k == 17) return true;
        if (k == 18) return true;

        return false;
    }

    IEnumerator CHANTEI()
    {
        while (gameManager.SHINDO < ms - gameManager.GDms)
        {
            yield return null;
        }
        while (gameManager.SHINDO <= ms + gameManager.GDms)
        {
            //TOUCH操作
            if (!gameManager.isPause)
            {
                switch (kind)
                {
                    //TAP必要
                    case 1:
                        if (isNear)
                        {

                            if (isWaitForGD)
                            {
                                if (gameManager.SHINDO > ms + gameManager.PFms)
                                {
                                    gameManager.PANDING(WaitForSec, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                                    
                                    inputManager.SetBeamColor(pos, pos + width, GDColor);

                                    DistroyThis();
                                    goto caseend;
                                }
                            }
                            if (isWaitForPF)
                            {
                                if (gameManager.SHINDO > ms + gameManager.PJms)
                                {
                                    gameManager.PANDING(WaitForSec, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                                    DistroyThis();
                                    goto caseend;
                                }
                            }


                            if (inputManager.GetTrigger(pos, pos + width))
                            {
                                if (gameManager.SHINDO < ms - gameManager.PFms)
                                {
                                    isWaitForGD = true;
                                    WaitForSec = (float)(gameManager.SHINDO - ms);

                                    //inputManager.SetBeamColor(pos, pos + width, GDColor);
                                }
                                else if (gameManager.SHINDO < ms - gameManager.PJms)
                                {
                                    isWaitForPF = true;
                                    WaitForSec = (float)(gameManager.SHINDO - ms);
                                    inputManager.SetBeamColor(pos, pos + width, PFColor);

                                }
                                else//(gameManager.SHINDO >= ms - gameManager.PJms)
                                {
                                    gameManager.PANDING((float)(gameManager.SHINDO - ms), kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                                    inputManager.SetBeamColor(pos, pos + width, Mathf.Abs((float)(gameManager.SHINDO - ms)) <= gameManager.PJms ? PJColor : Mathf.Abs((float)(gameManager.SHINDO - ms)) <= gameManager.PFms ? PFColor : GDColor);
                                    DistroyThis();
                                }


                            }
                        }
                        else
                        {
                            if (inputManager.GetTrigger(pos, pos + width))
                            {
                                gameManager.PANDING((float)(gameManager.SHINDO - ms), kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                                inputManager.SetBeamColor(pos, pos + width, Mathf.Abs((float)(gameManager.SHINDO - ms)) <= gameManager.PJms ? PJColor : Mathf.Abs((float)(gameManager.SHINDO - ms)) <= gameManager.PFms ? PFColor : GDColor);
                                DistroyThis();
                            }
                        }
                    caseend:
                        break;
                    case 2:
                        if (inputManager.GetTrigger(pos, pos + width))
                        {
                            gameManager.PANDING(0, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                            inputManager.SetBeamColor(pos, pos + width, PJColor);
                            DistroyThis();
                        }
                        break;
                    //PRESS RELEASE OK
                    case 4:
                    case 6:
                    case 7:
                    case 11:
                        if (inputManager.GetRelease(pos, pos + width) && !flag)
                        {
                            gameManager.PANDING(0, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                            flag = true;
                        }
                        else if (inputManager.GetPressed(pos, pos + width) && !flag)
                        {
                            gameManager.PANDING(0, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                            flag = true;
                        }
                        if (gameManager.SHINDO >= ms && flag)
                        {
                            DistroyThis();
                        }
                        break;
                    //PRESS 必要
                    case 3:
                    case 5:
                        if (inputManager.GetPressed(pos, pos + width) && !flag)
                        {
                            gameManager.PANDING(0, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                            flag = true;
                        }
                        if (gameManager.SHINDO >= ms && flag)
                        {
                            DistroyThis();
                        }
                        break;
                    //FLICK必要
                    case 9:
                        if (inputManager.GetFlick(pos, pos + width))
                        {
                            gameManager.PANDING(0, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                            DistroyThis();
                        }
                        break;
                    case 13:
                        if (inputManager.GetFlickLeft(pos, pos + width))
                        {
                            gameManager.PANDING(0, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                            DistroyThis();
                        }
                        break;
                    case 14:
                        if (inputManager.GetFlickRight(pos, pos + width))
                        {
                            gameManager.PANDING(0, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                            DistroyThis();
                        }
                        break;
                    case 15:
                        if (inputManager.GetFlickUp(pos, pos + width))
                        {
                            gameManager.PANDING(0, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                            DistroyThis();
                        }
                        break;
                    case 16:
                        if (inputManager.GetFlickDown(pos, pos + width))
                        {
                            gameManager.PANDING(0, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                            DistroyThis();
                        }
                        break;

                    //避ける必要
                    case 10:
                    case 17:
                    case 18:
                        if (gameManager.SHINDO >= ms)
                        {
                            if (inputManager.GetPressed(pos + 1, pos + width - 1))
                            {
                                gameManager.PANDING(200, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                                //血が出る警告
                                gameManager.HPMaskOut();
                                DistroyThis();
                            }
                            else
                            {
                                gameManager.PANDING(0, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
                                DistroyThis();
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            yield return null;
        }

        //BOOM NOTE 以外
        gameManager.PANDING(200, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
        DistroyThis();

    }

    IEnumerator CHANTEIA()
    {
        while (gameManager.SHINDO < ms)
        {
            yield return null;
        }
        float randomms = Random.Range(-gameManager.PJms * 0.9f, gameManager.PJms * 0.9f);
        gameManager.PANDING(kind != 1 ? 0 : randomms, kind, new Vector3(transform.position.x, 0.0f, 0.0f), width, kind);
        if (kind == 1 || kind == 2)
        {
            inputManager.SetBeamColor(pos, pos + width, PJColor);
        }

        DistroyThis();
    }


    public void SetGMIMG(TheGameManager gm, InputManager im)
    {
        gameManager = gm;
        inputManager = im;
    }
}