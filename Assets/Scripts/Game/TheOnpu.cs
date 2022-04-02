using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheOnpu : MonoBehaviour
{
    TheGameManager gameManager;
    InputManager inputManager;
    SpriteRenderer spriteRenderer;
    [SerializeField]
    private Material _material1, _material2, _material3, _material4, _material5, _material6;
    private Mesh _mesh;
    public MeshDrawer mDrawer;

    public AnimationCurve acNSC;
    bool hantei_flag = false;

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

    public class OnpuData
    {
        public int id;
        public int realid;
        public int kind;
        public float ichi;
        public float pos;
        public float width;
        public string nsc;
        public bool isnadnsc;
        public float insc;
        public int parent;
        public float maxtime = 0.0f;
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
        public List<int> GuardList = new List<int>();

    }

    OnpuData onpuData = new OnpuData();

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
        onpuData.id = i;
        onpuData.ichi = ic;
        onpuData.pos = p;
        onpuData.center = c;
        onpuData.mode = md;
        onpuData.nsc = n;
        onpuData.ms = m;
        onpuData.dms = d;
        onpuData.width = w;
        onpuData.kind = k;
        onpuData.parent = pa;
        onpuData.parent_ms = pm;
        onpuData.parent_dms = pd;
        onpuData.parent_pos = pp;
        onpuData.parent_width = pw;

        onpuData.isNear = near;
        onpuData.isWaitForGD = false;
        onpuData.isWaitForPF = false;
        onpuData.WaitForSec = 0.0f;

        spriteRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer arrorRenderer = transform.Find("Arror").GetComponent<SpriteRenderer>();

        spriteRenderer.size = new Vector2(onpuData.width, 1.0f);
        if (onpuData.kind == 6 || onpuData.kind == 11 || onpuData.kind == 12)
        {
            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.01f);
        }
        if (onpuData.kind == 1)
        {
            arrorRenderer.sprite = gameManager.GetSpriteArror(0);
            arrorRenderer.transform.position += new Vector3(0, 0.6f, 0);
            arrorRenderer.transform.localScale += new Vector3( 2, 2, 0);
            arrorRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else if (onpuData.kind == 2)
        {
            arrorRenderer.sprite = gameManager.GetSpriteArror(1);
            arrorRenderer.transform.position += new Vector3(0, 0.6f, 0);
            arrorRenderer.transform.localScale += new Vector3(2,2, 0);
            arrorRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else if (onpuData.kind == 13)
        {
            arrorRenderer.sprite = gameManager.GetSpriteArror(2);
            arrorRenderer.transform.position += new Vector3(0, 1.2f, 0);
            arrorRenderer.transform.localScale += new Vector3(4, 4, 0);
            arrorRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        }
        else if (onpuData.kind == 14)
        {
            arrorRenderer.sprite = gameManager.GetSpriteArror(3);
            arrorRenderer.transform.position += new Vector3(0, 1.2f, 0);
            arrorRenderer.transform.localScale += new Vector3(4, 4, 0);
            arrorRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        }
        else if (onpuData.kind == 15)
        {
            arrorRenderer.sprite = gameManager.GetSpriteArror(4);
            arrorRenderer.transform.position += new Vector3(0, 1.2f, 0);
            arrorRenderer.transform.localScale += new Vector3(4, 4, 0);
            arrorRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        }
        else if (onpuData.kind == 16)
        {
            arrorRenderer.sprite = gameManager.GetSpriteArror(5);
            arrorRenderer.transform.position += new Vector3(0, 1.2f, 0);
            arrorRenderer.transform.localScale += new Vector3(4, 4, 0);
            arrorRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        }
        else if (onpuData.kind == 9)
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

        effect_center_start = (onpuData.parent_pos + onpuData.parent_width * 0.5f) / 16.0f;
        effect_center_end = (onpuData.pos + onpuData.width * 0.5f) / 16.0f;
        angle_center_start = (onpuData.parent_pos + onpuData.parent_width * 0.5f - 8.0f) / 4.0f;
        angle_center_end = (onpuData.pos + onpuData.width * 0.5f - 8.0f) / 4.0f;

        if(IsTail(onpuData.kind))
        {
            onpuData.nsc = "0";
        }

        if(onpuData.mode =="P")
        {
            onpuData.nsc = "1:0;0.9:0.31;0.8:0.59;0.7:0.81;0.6:0.95;0.5:1;0.4:0.95;0.3:0.81;0.2:0.59;0.1:0.31;0:0";
            onpuData.isnadnsc = true;
            onpuData.insc = 1.0f;
        }

        //nsc処理
        if(!onpuData.nsc.Contains(":"))
        {
            onpuData.isnadnsc = false;
            onpuData.insc = float.Parse(onpuData.nsc);
            if (onpuData.insc == 0.0f) onpuData.insc = 1.0f;
        }
        else
        {
            onpuData.isnadnsc = true;
            onpuData.insc = 1.0f;
            string[] nscs = onpuData.nsc.Split(';');

            Keyframe[] nsckey = new Keyframe[nscs.Length];
            for(int ii=0;ii< nscs.Length;ii++)
            {
                nsckey[ii] = new Keyframe(gameManager.BPMCurve.Evaluate(onpuData.ichi -float.Parse(nscs[ii].Split(':')[0])), gameManager.BPMCurve.Evaluate(onpuData.ichi - float.Parse(nscs[ii].Split(':')[1])));
            }
            onpuData.maxtime = nsckey[0].time;
            
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
            if (onpuData.kind == 6 || onpuData.kind == 7)
            {
                if (gameManager.SHINDO + 100 >= onpuData.parent_ms && gameManager.SHINDO + 100 < onpuData.ms)
                {
                    float p = ((float)gameManager.SHINDO + 100 - onpuData.parent_ms) / (onpuData.ms - onpuData.parent_ms);
                    gameManager.AddEQ(effect_center_start + (effect_center_end - effect_center_start) * p);
                }
            }

            if (onpuData.kind == 19 || onpuData.kind == 20)
            {
                if (gameManager.SHINDO + 100 >= onpuData.parent_ms && gameManager.SHINDO + 100 < onpuData.ms)
                {
                    float p = ((float)gameManager.SHINDO + 100 - onpuData.parent_ms) / (onpuData.ms - onpuData.parent_ms);
                    gameManager.AddHP(effect_center_start + (effect_center_end - effect_center_start) * p);
                }
            }
            if (onpuData.kind == 21 || onpuData.kind == 22)
            {
                if (gameManager.SHINDO + 100 >= onpuData.parent_ms && gameManager.SHINDO + 100 < onpuData.ms)
                {
                    float p = ((float)gameManager.SHINDO + 100 - onpuData.parent_ms) / (onpuData.ms - onpuData.parent_ms);
                    gameManager.AddLP(effect_center_start + (effect_center_end - effect_center_start) * p);
                }
            }
        }

        //斜め処理
        if (IsTail(onpuData.kind))
        {
            if (gameManager.SHINDO >= onpuData.parent_ms && gameManager.SHINDO < onpuData.ms)
            {
                float p = ((float)gameManager.SHINDO - onpuData.parent_ms) / (onpuData.ms - onpuData.parent_ms);
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
        if (onpuData.isnadnsc)
        {
            z = onpuData.maxtime > shindo ? 300.0f : 0.01f * (onpuData.ms - acNSC.Evaluate((float)shindo)) * gameManager.NoteSpeed;
            pz = 0.01f * (float)(onpuData.parent_dms - dshindo);
        }
        else
        {
            z = 0.01f * (float)(onpuData.dms - dshindo) * onpuData.insc * gameManager.NoteSpeed;
            pz = 0.01f * (float)(onpuData.parent_dms - dshindo) * gameManager.NoteSpeed;
        }
        if (onpuData.mode == "L")
        {
            x = onpuData.center - 8.0f - z * 0.2f;
        }
        else if (onpuData.mode == "R")
        {
            x = onpuData.center - 8.0f + z * 0.2f;
        }
        else
        {
            x = onpuData.center - 8.0f;
        }
        if (onpuData.mode == "H")
        {
            y = IsTail(onpuData.kind) ? 0.0f : 0.1f + z * 0.1f;
        }
        else
        {
            y = IsTail(onpuData.kind) ? 0.0f : 0.1f;
        }

        transform.position = new Vector3(x,y , z);
        spriteRenderer.size = new Vector2(onpuData.width, 1.0f + (z / 80.0f));

        if (IsTail(onpuData.kind))
        {
            _positions = new Vector3[]{
                new Vector3(onpuData.parent_pos - 8.0f, -0.1f, pz),
                new Vector3(onpuData.pos - 8.0f, -0.1f, z),
                new Vector3(onpuData.parent_pos+0.4f - 8.0f, -0.1f, pz),
                new Vector3(onpuData.pos+0.4f - 8.0f, -0.1f, z),
                new Vector3(onpuData.parent_pos+onpuData.parent_width-0.4f - 8.0f, -0.1f, pz),
                new Vector3(onpuData.pos+onpuData.width-0.4f - 8.0f, -0.1f, z),
                new Vector3(onpuData.parent_pos+onpuData.parent_width - 8.0f, -0.1f, pz),
                new Vector3(onpuData.pos+onpuData.width - 8.0f, -0.1f, z),
            };

            _mesh.vertices = _positions;
            _mesh.RecalculateBounds();
            //Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, _material, 0);
            if (onpuData.kind == 4 || onpuData.kind == 11)
            {
                mDrawer.AddQue(_mesh, _material1);
            }
            if (onpuData.kind == 6 || onpuData.kind == 7)
            {
                mDrawer.AddQue(_mesh, _material2);
            }
            if (onpuData.kind == 17 || onpuData.kind == 18)
            {
                mDrawer.AddQue(_mesh, _material3);
            }
            if (onpuData.kind == 19 || onpuData.kind == 20)
            {
                mDrawer.AddQue(_mesh, _material4);
            }
            if (onpuData.kind == 21 || onpuData.kind == 22)
            {
                mDrawer.AddQue(_mesh, _material5);
            }
            if (onpuData.kind == 23 || onpuData.kind == 24)
            {
                mDrawer.AddQue(_mesh, _material6);
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
        if (k == 19) return true;
        if (k == 20) return true;
        if (k == 21) return true;
        if (k == 22) return true;
        if (k == 23) return true;
        if (k == 24) return true;

        return false;
    }

    IEnumerator CHANTEI()
    {

        if (hantei_flag) yield break;

        while (gameManager.SHINDO < onpuData.ms - gameManager.GDms)
        {
            yield return null;
        }
        while (gameManager.SHINDO <= onpuData.ms + gameManager.GDms)
        {
            //TOUCH操作
            if (!gameManager.isPause)
            {
                switch (onpuData.kind)
                {
                    //TAP必要
                    case 1:
                        if (onpuData.isNear)
                        {

                            if (onpuData.isWaitForGD)
                            {
                                if (gameManager.SHINDO > onpuData.ms + gameManager.PFms)
                                {
                                    gameManager.PANDING(onpuData.WaitForSec, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                                    
                                    inputManager.SetBeamColor(onpuData.pos, onpuData.pos + onpuData.width, GDColor);

                                    hantei_flag = true;
                                    DistroyThis();
                                    goto caseend;
                                }
                            }
                            if (onpuData.isWaitForPF)
                            {
                                if (gameManager.SHINDO > onpuData.ms + gameManager.PJms)
                                {
                                    gameManager.PANDING(onpuData.WaitForSec, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                                    hantei_flag = true;
                                    DistroyThis();
                                    goto caseend;
                                }
                            }


                            if (inputManager.GetTrigger(onpuData.pos, onpuData.pos + onpuData.width))
                            {
                                if (gameManager.SHINDO < onpuData.ms - gameManager.PFms)
                                {
                                    onpuData.isWaitForGD = true;
                                    onpuData.WaitForSec = (float)(gameManager.SHINDO - onpuData.ms);

                                    //inputManager.SetBeamColor(pos, pos + width, GDColor);
                                }
                                else if (gameManager.SHINDO < onpuData.ms - gameManager.PJms)
                                {
                                    onpuData.isWaitForPF = true;
                                    onpuData.WaitForSec = (float)(gameManager.SHINDO - onpuData.ms);
                                    inputManager.SetBeamColor(onpuData.pos, onpuData.pos + onpuData.width, PFColor);

                                }
                                else//(gameManager.SHINDO >= ms - gameManager.PJms)
                                {
                                    gameManager.PANDING((float)(gameManager.SHINDO - onpuData.ms), onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                                    inputManager.SetBeamColor(onpuData.pos, onpuData.pos + onpuData.width, Mathf.Abs((float)(gameManager.SHINDO - onpuData.ms)) <= gameManager.PJms ? PJColor : Mathf.Abs((float)(gameManager.SHINDO - onpuData.ms)) <= gameManager.PFms ? PFColor : GDColor);
                                    hantei_flag = true;
                                    DistroyThis();
                                }


                            }
                        }
                        else
                        {
                            if (inputManager.GetTrigger(onpuData.pos, onpuData.pos + onpuData.width))
                            {
                                gameManager.PANDING((float)(gameManager.SHINDO - onpuData.ms), onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                                inputManager.SetBeamColor(onpuData.pos, onpuData.pos + onpuData.width, Mathf.Abs((float)(gameManager.SHINDO - onpuData.ms)) <= gameManager.PJms ? PJColor : Mathf.Abs((float)(gameManager.SHINDO - onpuData.ms)) <= gameManager.PFms ? PFColor : GDColor);
                                hantei_flag = true;
                                DistroyThis();
                            }
                        }
                    caseend:
                        break;
                    case 2:
                        if (inputManager.GetTrigger(onpuData.pos, onpuData.pos + onpuData.width))
                        {
                            gameManager.PANDING(0, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                            inputManager.SetBeamColor(onpuData.pos, onpuData.pos + onpuData.width, PJColor);
                            hantei_flag = true;
                            DistroyThis();
                        }
                        break;
                    //PRESS RELEASE OK
                    case 4:
                    case 6:
                    case 7:
                    case 11:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                        if (inputManager.GetRelease(onpuData.pos, onpuData.pos + onpuData.width) && !flag)
                        {
                            gameManager.PANDING(0, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                            flag = true;
                        }
                        else if (inputManager.GetPressed(onpuData.pos, onpuData.pos + onpuData.width) && !flag)
                        {
                            gameManager.PANDING(0, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                            flag = true;
                        }
                        if (gameManager.SHINDO >= onpuData.ms && flag)
                        {
                            hantei_flag = true;
                            DistroyThis();
                        }
                        break;
                    //PRESS 必要
                    case 3:
                    case 5:
                        if (inputManager.GetPressed(onpuData.pos, onpuData.pos + onpuData.width) && !flag)
                        {
                            gameManager.PANDING(0, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                            flag = true;
                        }
                        if (gameManager.SHINDO >= onpuData.ms && flag)
                        {
                            hantei_flag = true;
                            DistroyThis();
                        }
                        break;
                    //FLICK必要
                    case 9:
                        if (inputManager.GetFlick(onpuData.pos, onpuData.pos + onpuData.width))
                        {
                            gameManager.PANDING(0, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                            hantei_flag = true;
                            DistroyThis();
                        }
                        break;
                    case 13:
                        if (inputManager.GetFlickLeft(onpuData.pos, onpuData.pos + onpuData.width))
                        {
                            gameManager.PANDING(0, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                            hantei_flag = true;
                            DistroyThis();
                        }
                        break;
                    case 14:
                        if (inputManager.GetFlickRight(onpuData.pos, onpuData.pos + onpuData.width))
                        {
                            gameManager.PANDING(0, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                            hantei_flag = true;
                            DistroyThis();
                        }
                        break;
                    case 15:
                        if (inputManager.GetFlickUp(onpuData.pos, onpuData.pos + onpuData.width))
                        {
                            gameManager.PANDING(0, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                            hantei_flag = true;
                            DistroyThis();
                        }
                        break;
                    case 16:
                        if (inputManager.GetFlickDown(onpuData.pos, onpuData.pos + onpuData.width))
                        {
                            gameManager.PANDING(0, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                            hantei_flag = true;
                            DistroyThis();
                        }
                        break;

                    //避ける必要
                    case 10:
                    case 17:
                    case 18:
                        if (gameManager.SHINDO >= onpuData.ms)
                        {
                            if (inputManager.GetPressed(onpuData.pos + 1, onpuData.pos + onpuData.width - 1))
                            {
                                gameManager.PANDING(200, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                                //血が出る警告
                                gameManager.HPMaskOut();
                                hantei_flag = true;
                                DistroyThis();
                            }
                            else
                            {
                                gameManager.PANDING(0, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
                                hantei_flag = true;
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
        if (!hantei_flag)
        {
            gameManager.PANDING(2 * gameManager.GDms, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
            //if (onpuData.kind == 1)
            //{
            //    global.AccMSList.Add(0.0f);
            //}
            hantei_flag = true;
            DistroyThis();
        }

    }

    IEnumerator CHANTEIA()
    {
        while (gameManager.SHINDO < onpuData.ms)
        {
            yield return null;
        }
        float randomms = Random.Range(-gameManager.PJms * 0.9f, gameManager.PJms * 0.9f);
        //if (onpuData.kind == 1) global.AccMSList.Add(100.0f - Mathf.Abs(randomms));
        gameManager.PANDING(onpuData.kind != 1 ? 0 : randomms, onpuData.kind, new Vector3(transform.position.x, 0.0f, 0.0f), onpuData.width, onpuData.kind);
        if (onpuData.kind == 1 || onpuData.kind == 2)
        {
            inputManager.SetBeamColor(onpuData.pos, onpuData.pos + onpuData.width, PJColor);
        }
        hantei_flag = true;
        DistroyThis();
    }


    public void SetGMIMG(TheGameManager gm, InputManager im)
    {
        gameManager = gm;
        inputManager = im;
    }
}