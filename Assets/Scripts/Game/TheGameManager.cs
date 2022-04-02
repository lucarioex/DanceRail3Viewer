using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using String;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;

public class TheGameManager : MonoBehaviour
{
    [SerializeField]
    string SongKeyword = "bluemelody";
    [SerializeField]
    int SongHard = 6;
    [SerializeField]
    string SongTitle = "Blue Melody", SongArtist = "波導Lucario";


    [SerializeField, Range(1, 30)]
    public int NoteSpeed = 12;
    [SerializeField, Range(-200, 200)]
    float NoteOffset = 0;
    [SerializeField]
    public bool GameAuto = true, GameMirror = false;
    [SerializeField, Range(0, 10)]
    public int GameEffectGaterLevel = 8, GameEffectParamEQLevel = 8, GameEffectTap = 6;

    [SerializeField]
    ComboDisp PlayerComboDisp = ComboDisp.COMBO;

    [SerializeField]
    public float hp = 100.0f, hpmax = 100.0f;
    [SerializeField]
    public float PJms = 30.0f, PFms = 60.0f, GDms = 100.0f;

    float SCORE = 0;
    float MSCORE = 3000000;
    int COMBO = 0;
    int MAXCOMBO = 0;
    int PERFECT_J = 0;
    int PERFECT = 0;
    int GOOD = 0;
    int MISS = 0;
    int FAST = 0;
    int SLOW = 0;

    enum ComboDisp
    {
        NONE = 0,
        COMBO = 1,
        SCORE = 2,
        MSCORE = 3,
    }

    //維持オブジェクト
    [SerializeField, HideInInspector]
    GameObject InputManager, OnpuPrefab, HanteiPrefab;

    [SerializeField, HideInInspector]
    Image ImageBackground;
    Vector3 backgroundStartPos;

    [SerializeField, HideInInspector]
    InputManager inputManager;

    float ReadyTime = 1.99f;

    [SerializeField, HideInInspector] AudioSource BGMManager;

    AudioClip ac_hit, ac_flick, ac_gameover;

    [SerializeField, HideInInspector] HPManager hpManager;
    [SerializeField, HideInInspector] MeshDrawer meshDrawer;
    [SerializeField, HideInInspector] Sprite[] SpriteNotes;
    [SerializeField, HideInInspector] Sprite[] SpriteArror;
    [SerializeField, HideInInspector] GameObject[] prefabEffect;
    [SerializeField, HideInInspector] GameObject notesUp, notesDown;

    [SerializeField, HideInInspector] Text textSongTitle, textSongArtist, textDif;
    [SerializeField, HideInInspector] Image sprSongImage;
    [SerializeField, HideInInspector] Slider sliderMusicTime;

    [SerializeField, HideInInspector] Text textScore, textMaxcombo, textPerfect, textPerfect2, textGood, textMiss;
    [SerializeField, HideInInspector] GameObject[] objCombo;
    [SerializeField, HideInInspector] Image[] imgHanteiBeam;
    [SerializeField, HideInInspector] Animator animSC;

    [SerializeField, HideInInspector] AnimationCurve EQCurve;
    List<float>
        EQList = new List<float>(),
        HPList = new List<float>(),
        LPList = new List<float>();
    float AudioMixerFreq = 1.0f, AudioMixerCenter = 1.0f,
        AudioMixerHiPass = 20.0f, AudioMixerLoPass = 10000.0f;
    List<float> AngList = new List<float>();
    float[] HgtList = new float[1000];
    [HideInInspector] public AnimationCurve HeightCurve;
    public AnimationCurve PositionCurve;

    [SerializeField, HideInInspector] GameObject TheCamera;

    [SerializeField, HideInInspector] AudioMixer audioMixer;

    [SerializeField, HideInInspector] Image HPMask;
    [SerializeField, HideInInspector] GameObject[] AnimationGrades;
    float SkillDamage = 1.0f;


    [HideInInspector]
    public AnimationCurve BPMCurve,   //ss to realtime s
                            SCCurve;    //realtime ms to drawtime

    [HideInInspector] public double SHINDO = 0.0, DSHINDO = 0.0;
    int CurrentSCn = 0;
    float CurrentSC = 1.0f;

    [HideInInspector] public bool isPause = false;

    public class DRBFile
    {
        public string ndname;
        public float offset;
        public float beat;
        public List<BPMS> bpms;
        public List<SCNS> scns;
        public List<TheOnpu.OnpuData> onpu;
        public int onpuWeightCount;
    }
    public class BPMS
    {
        public float bpm;
        public float bpms;
    }
    public class SCNS
    {
        public float sc;
        public float sci;
    }


    DRBFile drbfile;

    List<bool> isCreated = new List<bool>();
    int makenotestart = 0;

    int[] OnpuWeight = new int[28]
    {
        1,3,3,1,1,1,1,1,2,2,3,1,1,2,2,2,2,3,3,2,2,2,2,2,2,2,2,2
    };

    // Use this for initialization
    void Start()
    {
        //メモリ解放
        Resources.UnloadUnusedAssets();

        hpManager.manager = this;
        hpManager.Init(hp, hpmax);

        backgroundStartPos = ImageBackground.transform.position;

        //曲ロード
        BGMManager.clip = null;
        BGMManager.clip = Resources.Load<AudioClip>("SONGS/" + SongKeyword + "." + SongHard);
        if (BGMManager.clip == null)
        {
            BGMManager.clip = Resources.Load<AudioClip>("SONGS/" + SongKeyword);
        }
        //SCORE初期化
        SCORE_INIT();

        //曲データ表示
        if (textSongTitle)
        {
            textSongTitle.text = SongTitle;
            if (textSongTitle.preferredWidth > 320.0f)
            {
                textSongTitle.rectTransform.localScale = new Vector2(320.0f / textSongTitle.preferredWidth, 1.0f);
            }
        }
        if (textSongArtist)
        {
            textSongArtist.text = SongArtist;
            if (textSongArtist.preferredWidth > 320.0f)
            {
                textSongArtist.rectTransform.localScale = new Vector2(320.0f / textSongArtist.preferredWidth, 1.0f);
            }
        }
        if (textDif)
        {
            textDif.text = "Tier " + SongHard;
            textDif.color = TierColor[SongHard];
        }

        if (sprSongImage) sprSongImage.sprite = Resources.Load<Sprite>("IMAGES/" + SongKeyword);

        //譜面読み込み
        drbfile = new DRBFile();
        drbfile.bpms = new List<BPMS>();
        drbfile.scns = new List<SCNS>();
        drbfile.onpu = new List<TheOnpu.OnpuData>();
        SonglistReadin();

        //簡単整理
        drbfile.bpms.Sort((a, b) => Mathf.RoundToInt(a.bpms * 1000.0f - b.bpms * 1000.0f));
        drbfile.scns.Sort((a, b) => Mathf.RoundToInt(a.sci * 1000.0f - b.sci * 1000.0f));
        drbfile.onpu.Sort((a, b) => Mathf.RoundToInt(a.ichi * 1000.0f - b.ichi * 1000.0f));

        //複雑整理01:onpuのparentを探す
        for (int i = 0; i < drbfile.onpu.Count; i++)
        {
            for (int j = 0; j < drbfile.onpu.Count; j++)
            {
                if (drbfile.onpu[i].parent == drbfile.onpu[j].id)
                {
                    drbfile.onpu[i].parent = j;
                    break;
                }
            }
        }

        //複雑整理02:scによってdms算出
        //bpm
        Keyframe[] BPMKeyframe = new Keyframe[drbfile.bpms.Count + 1];

        BPMKeyframe[0] = new Keyframe(0.0f, drbfile.offset * 1000.0f);
        float[] BPM_REALTIME = new float[drbfile.bpms.Count + 1];

        for (int i = 0; i < drbfile.bpms.Count; i++)
        {
            if (i == 0)
            {
                BPM_REALTIME[i] = drbfile.offset * 1000.0f;
            }
            else
            {
                BPM_REALTIME[i] = (drbfile.bpms[i].bpms - drbfile.bpms[i - 1].bpms) * (60 / drbfile.bpms[i - 1].bpm * 4 * drbfile.beat) * 1000.0f + BPM_REALTIME[i - 1];
            }

        }
        BPM_REALTIME[drbfile.bpms.Count] = (10000 - drbfile.bpms[drbfile.bpms.Count - 1].bpms) * (60 / drbfile.bpms[drbfile.bpms.Count - 1].bpm * 4 * drbfile.beat) * 1000.0f + BPM_REALTIME[drbfile.bpms.Count - 1];
        for (int i = 1; i < drbfile.bpms.Count; i++)
        {
            BPMKeyframe[i] = new Keyframe(drbfile.bpms[i].bpms, BPM_REALTIME[i]);
        }
        BPMKeyframe[drbfile.bpms.Count] = new Keyframe(10000, BPM_REALTIME[drbfile.bpms.Count]);
        LinearKeyframe(BPMKeyframe);
        BPMCurve = new AnimationCurve(BPMKeyframe);

        //SC
        if (drbfile.scns.Count == 0)
        {
            SCNS sCNS = new SCNS();
            sCNS.sc = 1;
            sCNS.sci = 0;
            drbfile.scns.Add(sCNS);
        }
        float[] SCR = new float[drbfile.scns.Count + 1];
        for (int i = 0; i < drbfile.scns.Count; i++)
        {
            if (i == 0)
            {
                SCR[i] = BPMCurve.Evaluate(drbfile.scns[i].sci);
            }
            else
            {
                SCR[i] = SCR[i - 1] + (BPMCurve.Evaluate(drbfile.scns[i].sci) - BPMCurve.Evaluate(drbfile.scns[i - 1].sci)) * drbfile.scns[i - 1].sc;
            }
        }
        SCR[drbfile.scns.Count] = SCR[drbfile.scns.Count - 1] + (BPMCurve.Evaluate(10000) - BPMCurve.Evaluate(drbfile.scns[drbfile.scns.Count - 1].sci)) * drbfile.scns[drbfile.scns.Count - 1].sc;

        Keyframe[] SCKeyframe = new Keyframe[drbfile.scns.Count + 2];

        SCKeyframe[0] = new Keyframe(-10000.0f, -10000.0f);
        SCKeyframe[1] = new Keyframe(0.0f, 0.0f);

        for (int i = 0; i < drbfile.scns.Count; i++)
        {
            SCKeyframe[i + 1] = new Keyframe(BPMCurve.Evaluate(drbfile.scns[i].sci), SCR[i]);
        }
        SCKeyframe[drbfile.scns.Count + 1] = new Keyframe(BPMCurve.Evaluate(10000), SCR[drbfile.scns.Count]);

        LinearKeyframe(SCKeyframe);

        SCCurve = new AnimationCurve(SCKeyframe);

        //msデータ代入、効果音入れ

        //BGMManager.clip
        ac_hit = Resources.Load<AudioClip>("SE/hit");
        ac_flick = Resources.Load<AudioClip>("SE/flick");
        ac_gameover = Resources.Load<AudioClip>("SE/gameover");
        float[] f_song = new float[BGMManager.clip.samples * BGMManager.clip.channels],
                f_hit = new float[ac_hit.samples * ac_hit.channels],
                f_flick = new float[ac_flick.samples * ac_flick.channels];
        BGMManager.clip.GetData(f_song, 0);
        ac_hit.GetData(f_hit, 0);
        ac_flick.GetData(f_flick, 0);

        //音量半分下がる
        for (int i = 0; i < f_song.Length; i++) { f_song[i] *= 0.5f; }

        List<int> list_hit = new List<int>(), list_flick = new List<int>();

        for (int i = 0; i < drbfile.onpu.Count; i++)
        {
            //音符ごと位置計算
            drbfile.onpu[i].ms = BPMCurve.Evaluate(drbfile.onpu[i].ichi);
            drbfile.onpu[i].dms = SCCurve.Evaluate(BPMCurve.Evaluate(drbfile.onpu[i].ichi));
        }

        for (int i = 0; i < drbfile.onpu.Count; i++)
        {
            //parentms位置計算
            if (isTail(drbfile.onpu[i].kind))
            {
                drbfile.onpu[i].parent_ms = drbfile.onpu[drbfile.onpu[i].parent].ms;
                drbfile.onpu[i].parent_dms = drbfile.onpu[drbfile.onpu[i].parent].dms;
                drbfile.onpu[i].parent_pos = drbfile.onpu[drbfile.onpu[i].parent].pos;
                drbfile.onpu[i].parent_width = drbfile.onpu[drbfile.onpu[i].parent].width;
            }
        }

        //gater音を入れる
        if (GameEffectGaterLevel >= 1)
        {
            for (int i = 0; i < drbfile.onpu.Count; i++)
            {

                if (isBitCrash(drbfile.onpu[i].kind))
                {
                    int end = (int)(BGMManager.clip.samples * BGMManager.clip.channels * (drbfile.onpu[i].ms / 1000.0f / BGMManager.clip.length));
                    int start = (int)(BGMManager.clip.samples * BGMManager.clip.channels * (drbfile.onpu[drbfile.onpu[i].parent].ms / 1000.0f / BGMManager.clip.length));

                    if (end >= BGMManager.clip.samples * BGMManager.clip.channels)
                    {
                        continue;
                    }

                    //後ろ半分無音にする
                    start = (end - start) / 2 + start;

                    for (int c = start; c < end; c++)
                    {
                        f_song[c] *= (10.0f - GameEffectGaterLevel) / 10.0f;
                    }

                }
            }
        }

        //タップ音を強行入れる
        for (int i = 0; i < drbfile.onpu.Count; i++)
        {
            if (isTapSound(drbfile.onpu[i].kind))
            {
                int start = (int)(BGMManager.clip.samples * BGMManager.clip.channels * (drbfile.onpu[i].ms / 1000.0f / BGMManager.clip.length));
                if (start + f_hit.Length >= BGMManager.clip.samples * BGMManager.clip.channels)
                {
                    continue;
                }
                if (!list_hit.Contains(start))
                {
                    list_hit.Add(start);
                    for (int c = 0; c < f_hit.Length; c++)
                    {
                        if (start + c < f_song.Length) f_song[start + c] += f_hit[c] * 0.5f * ((GameEffectTap + 3) / 10.0f);
                    }
                }
            }
        }

        //フリック音を強行入れる
        for (int i = 0; i < drbfile.onpu.Count; i++)
        {
            if (isFlick(drbfile.onpu[i].kind))
            {
                int start = (int)(BGMManager.clip.samples * BGMManager.clip.channels * (drbfile.onpu[i].ms / 1000.0f / BGMManager.clip.length));
                if (start + f_flick.Length >= BGMManager.clip.samples * BGMManager.clip.channels)
                {
                    continue;
                }
                if (!list_flick.Contains(start))
                {
                    list_flick.Add(start);
                    for (int c = 0; c < f_flick.Length; c++)
                    {
                        if (start + c < f_song.Length) f_song[start + c] += f_flick[c] * 0.5f * ((GameEffectTap + 3) / 10.0f);
                    }
                }
            }
        }


        BGMManager.clip.SetData(f_song, 0);

        BGMManager.Play();
        BGMManager.Pause();



        //複雑整理03:ノーツ別front,back判定範囲算出

        for (int i = 0; i < drbfile.onpu.Count; i++)
        {
            isCreated.Add(false);
        }


        //横移動カーブ
        List<Keyframe> kfpos = new List<Keyframe>();
        kfpos.Add(new Keyframe(0, 0));
        float currentp = 0.0f;
        for (int i = 0; i < drbfile.onpu.Count; i++)
        {
            if (drbfile.onpu[i].kind == 23 || drbfile.onpu[i].kind == 24)
            {
                if (!kfpos.Exists(k => k.time == drbfile.onpu[i].parent_ms / 1000.0f))
                    kfpos.Add(new Keyframe(drbfile.onpu[i].parent_ms / 1000.0f, currentp));
                currentp += (drbfile.onpu[i].pos + drbfile.onpu[i].width * 0.5f) - (drbfile.onpu[i].parent_pos + drbfile.onpu[i].parent_width * 0.5f);
                kfpos.Add(new Keyframe(drbfile.onpu[i].ms / 1000.0f, currentp));
            }
        }
        Keyframe[] kfposa = kfpos.ToArray();
        LinearKeyframe(kfposa);
        PositionCurve = new AnimationCurve(kfposa);

        if (kfpos.Count <= 1)
        {
            //カメラ高さ調整
            for (int i = 0; i < drbfile.onpu.Count; i++)
            {
                if (drbfile.onpu[i].pos < 0)
                {
                    int s = (int)(drbfile.onpu[i].ms / 1000.0f);
                    if (s + 0 >= 0 && s + 0 < HgtList.Length) HgtList[s + 0] = Mathf.Max(drbfile.onpu[i].pos / (-16.0f), HgtList[s + 0]);
                    if (s + 1 >= 0 && s + 1 < HgtList.Length) HgtList[s + 1] = Mathf.Max(drbfile.onpu[i].pos / (-16.0f), HgtList[s + 1]);

                    if (drbfile.onpu[i].pos < -8)
                    {
                        if (s - 1 >= 0 && s - 1 < HgtList.Length) HgtList[s - 1] = Mathf.Max(drbfile.onpu[i].pos / (-32.0f), HgtList[s - 1]);
                        if (s + 2 >= 0 && s + 2 < HgtList.Length) HgtList[s + 2] = Mathf.Max(drbfile.onpu[i].pos / (-32.0f), HgtList[s + 2]);
                    }
                }
                if (drbfile.onpu[i].pos + drbfile.onpu[i].width > 16)
                {
                    int s = (int)(drbfile.onpu[i].ms / 1000.0f);
                    if (s + 0 >= 0 && s + 0 < HgtList.Length) HgtList[s + 0] = Mathf.Max((drbfile.onpu[i].pos + drbfile.onpu[i].width - 16.0f) / 16.0f, HgtList[s + 0]);
                    if (s + 1 >= 0 && s + 1 < HgtList.Length) HgtList[s + 1] = Mathf.Max((drbfile.onpu[i].pos + drbfile.onpu[i].width - 16.0f) / 16.0f, HgtList[s + 1]);

                    if (drbfile.onpu[i].pos > 24)
                    {
                        if (s - 1 >= 0 && s - 1 < HgtList.Length) HgtList[s - 1] = Mathf.Max((drbfile.onpu[i].pos + drbfile.onpu[i].width - 16.0f) / 32.0f, HgtList[s - 1]);
                        if (s + 2 >= 0 && s + 2 < HgtList.Length) HgtList[s + 2] = Mathf.Max((drbfile.onpu[i].pos + drbfile.onpu[i].width - 16.0f) / 32.0f, HgtList[s + 2]);
                    }
                }
            }
        }
        //高さカーブ生成
        for (int i = 0; i < 1000; i++)
        {
            HeightCurve.AddKey(i, HgtList[i]);
        }

        //タップだけ判定範囲を保護する
        for (int i = 0; i < drbfile.onpu.Count; i++)
        {
            if (isTap(drbfile.onpu[i].kind))
            {
                for (int j = 0; j < drbfile.onpu.Count; j++)
                {
                    //相手側は全ノーツ
                    //if (isTap(drbfile.onpu[j].kind))
                    {
                        //距離近い、そして、重ねていると
                        if (isCovering(drbfile.onpu[i], drbfile.onpu[j]) && Mathf.Abs(drbfile.onpu[j].ms - drbfile.onpu[i].ms) < GDms * 2.0f)
                        {
                            ////Back
                            //if (drbfile.onpu[j].ms > drbfile.onpu[i].ms)
                            //{
                            //    drbfile.onpu[i].back = Mathf.Min(drbfile.onpu[i].back, (drbfile.onpu[j].ms - drbfile.onpu[i].ms) * 0.5f);
                            //}
                            ////Front
                            //if (drbfile.onpu[j].ms < drbfile.onpu[i].ms)
                            //{
                            //    drbfile.onpu[i].front = Mathf.Min(drbfile.onpu[i].front, (drbfile.onpu[i].ms - drbfile.onpu[j].ms) * 0.5f);
                            //}
                            //isNear
                            if (drbfile.onpu[j].ms < drbfile.onpu[i].ms)
                            {
                                drbfile.onpu[i].isNear = true;
                            }

                            //コメント：等しい場合は保護範囲外
                        }
                    }
                }
            }
        }

        HPMask.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        HPMask.gameObject.SetActive(false);

        //背景


        //スコアコンボ表示
        if (PlayerComboDisp > ComboDisp.COMBO)
        {
            objCombo[0].GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            objCombo[1].GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            objCombo[2].GetComponent<Text>().color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
            objCombo[3].GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        if (PlayerComboDisp == ComboDisp.SCORE)
        {
            objCombo[0].GetComponent<Text>().text = "0";
            objCombo[1].GetComponent<Text>().text = "0";
            objCombo[2].GetComponent<Text>().text = "0";
            objCombo[3].GetComponent<Text>().text = "SCORE";
        }
        if (PlayerComboDisp == ComboDisp.MSCORE)
        {
            objCombo[0].GetComponent<Text>().text = "3,000,000";
            objCombo[1].GetComponent<Text>().text = "3,000,000";
            objCombo[2].GetComponent<Text>().text = "3,000,000";
            objCombo[3].GetComponent<Text>().text = "- SCORE";
        }

        StartCoroutine(Init());

    }

    IEnumerator Init()
    {
        float timer = ReadyTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            //DSHINDO = -timer * 1000.0 / (BGMManager.pitch > 0 ? BGMManager.pitch : 0.01) - 100.0 + NoteOffset;
            yield return null;
        }

        BGMManager.UnPause();

        SHINDO = BGMManager.time * 1000.0 - 100.0 + NoteOffset;

    }

    //譜面読み込み
    void SonglistReadin()
    {
        TextAsset textasset = new TextAsset();
        textasset = Resources.Load("SONGS/" + SongKeyword + "." + SongHard, typeof(TextAsset)) as TextAsset;
        string TextLines = textasset.text;
        TextLines = ScriptString.RemoveSlash(TextLines);    //コメントの場所を取り除く
        //TextLines = ScriptString.RemoveSpace(TextLines);    //スペースがある場所を取り除く
        TextLines = ScriptString.RemoveTab(TextLines);      //タッブがある場所を取り除く
        TextLines = ScriptString.RemoveEnter(TextLines);    //複数のエンターの場所を取り除く
        string[] s = TextLines.Split('\n');

        drbfile.onpuWeightCount = 0;

        for (int i = 0; i < s.Length; i++)
        {
            //空き行を抜く
            if (s[i] == "") continue;
            //命令行を認識
            if (s[i].Substring(0, 1) == "#")
            {
                //OFFSET認識
                if (s[i].Substring(0, Mathf.Min(s[i].Length, "#OFFSET".Length)) == "#OFFSET")
                {
                    string ss = s[i];
                    ss = ss.Replace("#OFFSET=", "");
                    ss = ss.Replace(";", "");
                    drbfile.offset = float.Parse(ss);
                }
                //BEAT認識
                if (s[i].Substring(0, Mathf.Min(s[i].Length, "#BEAT".Length)) == "#BEAT")
                {
                    string ss = s[i];
                    ss = ss.Replace("#BEAT=", "");
                    ss = ss.Replace(";", "");
                    drbfile.beat = float.Parse(ss);
                }
                //BPM_NUMBER認識
                //if (s[i].Substring(0, "#BPM_NUMBER".Length) == "#BPM_NUMBER")
                //{
                //    string ss = s[i];
                //    ss = ss.Replace("#BPM_NUMBER=", "");
                //    ss = ss.Replace(";", "");
                //    drbfile.bpms = new List<BPMS>();
                //}
                //SCN認識
                //if (s[i].Substring(0, "#SCN".Length) == "#SCN")
                //{
                //    string ss = s[i];
                //    ss = ss.Replace("#SCN=", "");
                //    ss = ss.Replace(";", "");
                //    drbfile.scns = new List<SCNS>();
                //}
                //BPM [i]認識
                if (s[i].Substring(0, Mathf.Min(s[i].Length, ("#BPM [" + drbfile.bpms.Count + "]").Length)) == ("#BPM [" + drbfile.bpms.Count + "]"))
                {
                    string ss = s[i];
                    string ss2 = s[i + 1];
                    ss = ss.Replace("#BPM [" + drbfile.bpms.Count + "]=", "");
                    ss = ss.Replace(";", "");
                    ss2 = ss2.Replace("#BPMS[" + drbfile.bpms.Count + "]=", "");
                    ss2 = ss2.Replace(";", "");
                    BPMS bpms = new BPMS();
                    bpms.bpm = float.Parse(ss);
                    bpms.bpms = float.Parse(ss2);
                    drbfile.bpms.Add(bpms);
                }
                //SC [i]認識
                if (s[i].Substring(0, Mathf.Min(s[i].Length, ("#SC [" + drbfile.scns.Count + "]").Length)) == ("#SC [" + drbfile.scns.Count + "]"))
                {
                    string ss = s[i];
                    string ss2 = s[i + 1];
                    ss = ss.Replace("#SC [" + drbfile.scns.Count + "]=", "");
                    ss = ss.Replace(";", "");
                    ss2 = ss2.Replace("#SCI[" + drbfile.scns.Count + "]=", "");
                    ss2 = ss2.Replace(";", "");
                    SCNS sc = new SCNS();
                    sc.sc = float.Parse(ss);
                    sc.sci = float.Parse(ss2);
                    drbfile.scns.Add(sc);
                }

                //NoteDesigner認識;
                if (s[i].Substring(0, Mathf.Min(s[i].Length, "#NDNAME".Length)) == "#NDNAME")
                {
                    string ss = s[i];
                    ss = ss.Replace("#NDNAME=", "");
                    ss = ss.Replace(";", "");
                    ss = ss.Replace("\'", "");
                    drbfile.ndname = ss;
                }
            }
            //ノーツ行を認識
            else
            {
                //Notesデータ取得
                string ss = s[i].Replace("<", "");
                string[] sss = ss.Substring(0, ss.Length - 2).Split('>');
                TheOnpu.OnpuData onpu = new TheOnpu.OnpuData();
                onpu.id = int.Parse(sss[0]);
                onpu.kind = int.Parse(sss[1]);
                if (onpu.kind == 12) onpu.kind = 6;
                if (onpu.kind == 8) onpu.kind = 7;
                onpu.ichi = float.Parse(sss[2]);
                onpu.pos = float.Parse(sss[3]);
                onpu.width = float.Parse(sss[4]);
                onpu.nsc = sss[5];
                onpu.isnadnsc = sss[5].Contains(":");
                if (!onpu.isnadnsc) onpu.insc = float.Parse(sss[5]);
                onpu.insc = onpu.insc == 0.0f ? 1.0f : onpu.insc;
                onpu.parent = int.Parse(sss[6]);
                if (sss.Length > 7)
                {
                    onpu.mode = sss[7];
                }
                else
                {
                    onpu.mode = "n";
                }
                if (onpu.mode == "P") onpu.isnadnsc = true;

                //ミラー処理
                if (GameMirror)
                {
                    onpu.pos = 16 - onpu.pos - onpu.width;
                    if (onpu.kind == 13 || onpu.kind == 14)
                    {
                        onpu.kind = onpu.kind == 13 ? 14 : 13;
                    }
                }

                drbfile.onpu.Add(onpu);

                drbfile.onpuWeightCount += OnpuWeight[onpu.kind];

            }
        }

    }

    // Update is called once per frame
    void Update()
    {


        if (BGMManager.isPlaying)
        {
            SHINDO += Time.deltaTime * 1000.0f * BGMManager.pitch;

            //EQEffect
            if (GameEffectParamEQLevel >= 1)
            {
                if (EQList.Count <= 0)
                {
                    AudioMixerFreq -= (AudioMixerFreq - 1.0f) * 20 * Time.deltaTime;
                    AudioMixerCenter -= (AudioMixerCenter - 0.5f) * 20 * Time.deltaTime;
                }
                else
                {
                    float adv = EQList.Average();

                    AudioMixerFreq -= (AudioMixerFreq - (1.0f + 0.15f * GameEffectParamEQLevel)) * 20.0f * Time.deltaTime;
                    AudioMixerCenter -= (AudioMixerCenter - adv) * 20.0f * Time.deltaTime;
                }

                //High Pass
                if (HPList.Count <= 0)
                {
                    AudioMixerHiPass = 0.0f;
                }
                else
                {
                    AudioMixerHiPass = HPList.Average();
                }

                //Low Pass
                if (LPList.Count <= 0)
                {
                    AudioMixerLoPass = 1.0f;
                }
                else
                {
                    AudioMixerLoPass = LPList.Average();
                }
                audioMixer.SetFloat("Center", EQCurve.Evaluate(AudioMixerCenter));
                audioMixer.SetFloat("Freq", AudioMixerFreq);
                audioMixer.SetFloat("HPFreq", EQCurve.Evaluate(AudioMixerHiPass));
                audioMixer.SetFloat("LPFreq", EQCurve.Evaluate(AudioMixerLoPass));

                EQList.Clear();
                HPList.Clear();
                LPList.Clear();
            }

            //斜め　AngList
            if (AngList.Count <= 0)
            {
                var angle = TheCamera.transform.eulerAngles;
                if (angle.z > 180.0f) angle.z -= 360.0f;
                angle.z -= (angle.z - 0.0f) * 20.0f * Time.deltaTime;
                if (angle.z < 0.0f) angle.z += 360.0f;
                TheCamera.transform.eulerAngles = angle;
            }
            else
            {
                var angle = TheCamera.transform.eulerAngles;
                if (angle.z > 180.0f) angle.z -= 360.0f;
                angle.z -= (angle.z - AngList.Average()) * 10.0f * Time.deltaTime;
                if (angle.z < 0.0f) angle.z += 360.0f;
                TheCamera.transform.eulerAngles = angle;
            }
            AngList.Clear();

            //カメラの高さ　HgtList

            var pos = TheCamera.transform.position;
            pos.y -= (pos.y - (9.0f + 18.0f * HeightCurve.Evaluate(BGMManager.time))) * 20.0f * Time.deltaTime;
            pos.z -= (pos.z - (-7.0f - 14.0f * HeightCurve.Evaluate(BGMManager.time))) * 20.0f * Time.deltaTime;
            pos.x -= (pos.x - PositionCurve.Evaluate(BGMManager.time)) * 30.0f * Time.deltaTime;
            TheCamera.transform.position = pos;

            //HPMask描画
            HPMask.color -= (HPMask.color - new Color(1.0f, 1.0f, 1.0f, 0.0f)) * 0.2f;
            if (HPMask.color.a <= 0.001f)
            {
                HPMask.gameObject.SetActive(false);
            }

            //HanteiBeam
            if (imgHanteiBeam[0] != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    float a = imgHanteiBeam[i].color.a;
                    if (a > 0.1f) { a -= 2.0f * Time.deltaTime; } else { a = 0.0f; }
                    imgHanteiBeam[i].color = new Color(1.0f, 1.0f, 1.0f, a);
                }
            }
        }
        DSHINDO = SCCurve.Evaluate((float)SHINDO);
        if (animSC)
        {
            for (int i = CurrentSCn; i < drbfile.scns.Count; i++)
            {
                if (SHINDO + 123.4f > BPMCurve.Evaluate(drbfile.scns[i].sci))
                {
                    if (CurrentSC != drbfile.scns[i].sc)
                    {
                        CurrentSC = drbfile.scns[i].sc;
                        animSC.SetFloat("CurrentSC", CurrentSC);
                    }
                    CurrentSCn = i;
                }
                else
                {
                    break;
                }
            }
        }

        //曲の進捗を表示する
        if (sliderMusicTime) sliderMusicTime.value = BGMManager.time / BGMManager.clip.length;

        //終了処理
        //なし

        //ノーツ創り出す 1 Frame Max 5 Notes
        int count = 0;
        for (int i = makenotestart; i < drbfile.onpu.Count; i++)
        {
            if (!isCreated[i])
            {
                if ((0.01f * ((isTail(drbfile.onpu[i].kind) ? drbfile.onpu[i].parent_dms : drbfile.onpu[i].dms) - DSHINDO) * drbfile.onpu[i].insc * NoteSpeed < 150.0f)
                    || (drbfile.onpu[i].ms - SHINDO < 1000)
                    || (drbfile.onpu[i].isnadnsc && drbfile.onpu[i].ms - SHINDO < 10000.0f))
                {

                    GameObject note = Instantiate(OnpuPrefab, isTap(drbfile.onpu[i].kind) ? notesUp.transform : notesDown.transform);
                    note.GetComponent<SpriteRenderer>().sprite = SpriteNotes[drbfile.onpu[i].kind];
                    if (isTap(drbfile.onpu[i].kind)) note.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    note.GetComponent<TheOnpu>().mDrawer = meshDrawer;
                    note.GetComponent<TheOnpu>().SetGMIMG(this, inputManager);
                    if (isTail(drbfile.onpu[i].kind))
                    {
                        note.GetComponent<TheOnpu>().Ready(
                            drbfile.onpu[i].id,
                            drbfile.onpu[i].ichi,
                            drbfile.onpu[i].pos,
                            drbfile.onpu[i].pos + drbfile.onpu[i].width * 0.5f,

                            drbfile.onpu[i].mode,
                            drbfile.onpu[i].nsc,
                            drbfile.onpu[i].ms,
                            drbfile.onpu[i].dms,
                            drbfile.onpu[i].width,
                            drbfile.onpu[i].kind,
                            drbfile.onpu[i].isNear,
                            drbfile.onpu[i].parent,
                            drbfile.onpu[i].parent_ms,
                            drbfile.onpu[i].parent_dms,
                            drbfile.onpu[i].parent_pos,
                            drbfile.onpu[i].parent_width

                            );
                    }
                    else
                    {
                        note.GetComponent<TheOnpu>().Ready(
                            drbfile.onpu[i].id,
                            drbfile.onpu[i].ichi,
                            drbfile.onpu[i].pos,
                            drbfile.onpu[i].pos + drbfile.onpu[i].width * 0.5f,
                            drbfile.onpu[i].mode,
                            drbfile.onpu[i].nsc,
                            drbfile.onpu[i].ms,
                            drbfile.onpu[i].dms,
                            drbfile.onpu[i].width,
                            drbfile.onpu[i].kind,
                            drbfile.onpu[i].isNear
                            );
                    }

                    note.GetComponent<TheOnpu>().StartC();


                    isCreated[i] = true;
                    count++;//Max 5 notes

                    for (int ii = 0; ii < drbfile.onpu.Count; ii++)
                    {
                        if (!isCreated[ii])
                        {
                            makenotestart = ii;
                            break;
                        }
                    }
                    //Max 5 notes
                    if (count >= 5)
                    {
                        break;
                    }
                }
            }
        }


        //SKYBOX移動
        //RenderSettings.skybox.SetFloat("_Rotation", 90.0f + 20.0f * Mathf.Sin(Time.realtimeSinceStartup / 100.0f * 2.0f * Mathf.PI));
        ImageBackground.transform.position = backgroundStartPos + new Vector3(20.0f * Mathf.Sin(Time.realtimeSinceStartup / 100.0f * 2.0f * Mathf.PI), 0.0f, 0.0f);

    }


    IEnumerator EndEvent(int grade)
    {
        if (AnimationGrades[grade])
            AnimationGrades[grade].SetActive(true);

        WaitForSeconds w01s = new WaitForSeconds(0.1f);

        for (int i = 0; i < 10; i++)
        {
            BGMManager.volume -= 0.1f;
            yield return w01s;
        }
        //終了処理


    }

    public void StartGameOverEvent()
    {
        //bgameover = true;
        StartCoroutine(GameOverEvent());
    }

    IEnumerator GameOverEvent()
    {
        if (AnimationGrades[0])
            AnimationGrades[0].SetActive(true);

        BGMManager.Pause();

        AudioSource ac = gameObject.AddComponent<AudioSource>();
        ac.volume = 0.5f;
        ac.PlayOneShot(ac_gameover);

        WaitForSeconds w01s = new WaitForSeconds(0.1f);

        for (int i = 0; i < 10; i++)
        {
            yield return w01s;
        }
        //GameOver処理


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

    bool isTail(int k)
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
    bool isTapSound(int k)
    {
        if (k == 1) return true;
        if (k == 2) return true;
        if (k == 3) return true;
        if (k == 4) return true;
        if (k == 5) return true;
        if (k == 7) return true;
        if (k == 10) return true;
        if (k == 18) return true;
        if (k == 20) return true;
        if (k == 22) return true;
        if (k == 24) return true;

        return false;
    }
    bool isTap(int k)
    {
        if (k == 1) return true;
        if (k == 2) return true;

        return false;
    }
    bool isFlick(int k)
    {
        if (k == 9) return true;
        if (k == 13) return true;
        if (k == 14) return true;
        if (k == 15) return true;
        if (k == 16) return true;

        return false;
    }
    bool isBitCrash(int k)
    {
        if (k == 4) return true;
        if (k == 11) return true;

        return false;
    }

    bool isCovering(float l1, float w1, float l2, float w2)
    {
        if (l2 + w2 <= l1) return false;
        if (l2 >= l1 + w1) return false;

        return true;
    }
    bool isCovering(TheOnpu.OnpuData o1, TheOnpu.OnpuData o2)
    {
        float l1, w1, l2, w2;
        /*l1 = o1.pos;
        w1 = o1.width;
        l2 = o2.pos;
        w2 = o2.width;
        */
        l1 = o1.pos - 1;
        w1 = o1.width + 2;
        l2 = o2.pos - 1;
        w2 = o2.width + 2;

        if (l2 + w2 <= l1) return false;
        if (l2 >= l1 + w1) return false;

        return true;
    }

    void TimeGoBack(float from, float to)
    {
        BGMManager.time -= (from - to);
        SHINDO -= (from - to) * 1000.0f;
        for (int i = 0; i < isCreated.Count; i++)
        {
            if (drbfile.onpu[i].ms >= to * 1000 && drbfile.onpu[i].ms < from * 1000)
            {
                isCreated[i] = false;
            }
        }
    }

    void SCORE_INIT()
    {
        SCORE = 0;
        MSCORE = 3000000;
        COMBO = 0;
        MAXCOMBO = 0;
        PERFECT_J = 0;
        PERFECT = 0;
        GOOD = 0;
        MISS = 0;
        FAST = 0;
        SLOW = 0;

    }

    //判定処理
    public void PANDING(float ms, int k, Vector3 pos, float width, int kind)
    {
        //LISTに登録


        var go2 = Instantiate(HanteiPrefab, pos, Quaternion.identity);
        GameObject go;

        //PERFECT JUSTICE 判定
        if (Mathf.Abs(ms) <= PJms)
        {
            PERFECT_J++;
            COMBOPLUS();
            go = Instantiate(prefabEffect[k], pos, Quaternion.identity);
            go2.GetComponent<HanteiImage>().Init(4, 0);
            if (imgHanteiBeam[0]) imgHanteiBeam[0].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //HP処理
            hpManager.HPUP(150.0f * OnpuWeight[kind] / drbfile.onpuWeightCount);

        }
        //PERFECT 判定
        else if (Mathf.Abs(ms) <= PFms)
        {
            PERFECT++;
            COMBOPLUS();
            FASTORSLOW(ms);
            go = Instantiate(prefabEffect[k], pos, Quaternion.identity);
            go2.GetComponent<HanteiImage>().Init(3, ms >= 0 ? 2 : 1);
            if (imgHanteiBeam[1]) imgHanteiBeam[1].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //HP処理
            hpManager.HPUP(75.0f * OnpuWeight[kind] / drbfile.onpuWeightCount);
            ////Event
            //if (isEvent)
            //{
            //    BossSkillCnt += 1;
            //    if (BossSkillCnt >= 20)
            //    {
            //        StartCoroutine(BossSkill());
            //        BossSkillCnt = 0;
            //    }
            //}
        }
        //GOOD 判定
        else if (Mathf.Abs(ms) <= GDms)
        {
            GOOD++;
            COMBOPLUS();
            FASTORSLOW(ms);
            go = Instantiate(prefabEffect[k], pos, Quaternion.identity);
            go2.GetComponent<HanteiImage>().Init(2, ms >= 0 ? 2 : 1);
            if (imgHanteiBeam[2]) imgHanteiBeam[2].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //HP処理
            hpManager.HPDOWN(0.5f * OnpuWeight[kind] * SkillDamage);
            //Event
            //if (isEvent)
            //{
            //    BossSkillCnt += 2;
            //    if (BossSkillCnt >= 20)
            //    {
            //        StartCoroutine(BossSkill());
            //        BossSkillCnt = 0;
            //    }
            //}
        }
        //MISS 判定
        else
        {
            MISS++;
            if (COMBO >= 30)
            {
                //血が出る警告
                HPMaskOut();
            }
            COMBO = 0;
            COMBOREFLASH();
            go = null;
            go2.GetComponent<HanteiImage>().Init(1, 0);
            if (imgHanteiBeam[3]) imgHanteiBeam[3].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //HP処理
            hpManager.HPDOWN(1.0f * OnpuWeight[kind] * SkillDamage);
            ////Event
            //if (isEvent)
            //{
            //    BossSkillCnt += 3;
            //    if (BossSkillCnt >= 20)
            //    {
            //        StartCoroutine(BossSkill());
            //        BossSkillCnt = 0;
            //    }
            //}
        }
        if (go)
        {
            go.transform.localScale = new Vector3(width * 0.2f + 2.0f, width * 0.2f + 2.0f, width * 0.2f + 2.0f);
        }
    }

    void COMBOPLUS()
    {
        COMBO++;
        MAXCOMBO = Mathf.Max(MAXCOMBO, COMBO);
        COMBOREFLASH();
    }

    void COMBOREFLASH()
    {
        //表示物反応
        SCORE = 3000000.0f * (PERFECT_J * 1.0f + PERFECT * 0.99f + GOOD / 3.0f) / drbfile.onpu.Count;
        MSCORE = 3000000.0f - 3000000.0f * (PERFECT * 0.01f + GOOD / 3.0f * 2.0f + MISS * 1.0f) / drbfile.onpu.Count;


        if (textScore) textScore.text = (SCORE).ToString("N0");
        if (textMaxcombo) textMaxcombo.text = (MAXCOMBO).ToString("N0");
        if (textPerfect) textPerfect.text = (PERFECT_J).ToString("N0");
        if (textPerfect2) textPerfect2.text = (PERFECT).ToString("N0");
        if (textGood) textGood.text = (GOOD).ToString("N0");
        if (textMiss) textMiss.text = (MISS).ToString("N0");


        if (objCombo[0] != null)
        {
            if (PlayerComboDisp == 0 || (PlayerComboDisp == ComboDisp.COMBO && COMBO <= 2))
            {
                objCombo[0].GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                objCombo[1].GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
                objCombo[2].GetComponent<Text>().color = new Color(0.0f, 1.0f, 1.0f, 0.0f);
                objCombo[3].GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
            else
            {
                objCombo[0].GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                objCombo[1].GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                objCombo[2].GetComponent<Text>().color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
                objCombo[3].GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                string disp = "";
                if (PlayerComboDisp == ComboDisp.COMBO)
                {
                    disp = COMBO.ToString();
                }
                else if (PlayerComboDisp == ComboDisp.SCORE)
                {
                    disp = SCORE.ToString("N0");
                }
                else if (PlayerComboDisp == ComboDisp.MSCORE)
                {
                    disp = MSCORE.ToString("N0");
                }

                objCombo[0].GetComponent<Text>().text = disp;
                objCombo[1].GetComponent<Text>().text = disp;
                objCombo[2].GetComponent<Text>().text = disp;

                objCombo[0].transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                objCombo[1].transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                objCombo[2].transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);

                objCombo[1].transform.position += new Vector3(Random.value < 0.5f ? Random.Range(-0.08f, -0.04f) : Random.Range(0.04f, 0.08f), Random.value < 0.5f ? Random.Range(-0.08f, -0.04f) : Random.Range(0.04f, 0.08f), 0);
                objCombo[2].transform.position += new Vector3(Random.value < 0.5f ? Random.Range(-0.08f, -0.04f) : Random.Range(0.04f, 0.08f), Random.value < 0.5f ? Random.Range(-0.08f, -0.04f) : Random.Range(0.04f, 0.08f), 0);

            }
        }
    }

    void FASTORSLOW(float ms)
    {
        if (ms >= 0)
        {
            SLOW++;
        }
        else
        {
            FAST++;
        }
    }

    public Sprite GetSpriteArror(int num)
    {
        return SpriteArror[num];
    }


    class EQ
    {
        public float center;//20~22000;
        public float range;//1-5;
        public float freq;//1-3;
    }

    public void AddEQ(float f)
    {
        EQList.Add(f);
    }
    public void AddHP(float f)
    {
        HPList.Add(f);
    }
    public void AddLP(float f)
    {
        LPList.Add(f);
    }
    public void AddAng(float f)
    {
        AngList.Add(f);
    }

    public void HPMaskOut()
    {
        HPMask.gameObject.SetActive(true);
        HPMask.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    Color[] TierColor = new Color[27]
    {
        new Color(1.0f,1.0f,1.0f),//0
        new Color(0.5f,0.5f,1.0f),
        new Color(0.5f,0.5f,1.0f),
        new Color(0.5f,0.5f,1.0f),
        new Color(0.5f,0.5f,1.0f),
        new Color(0.5f,0.5f,1.0f),//5
        new Color(1.0f,1.0f,0.5f),
        new Color(1.0f,1.0f,0.5f),
        new Color(1.0f,1.0f,0.5f),
        new Color(1.0f,1.0f,0.5f),
        new Color(1.0f,1.0f,0.5f),//10
        new Color(1.0f,0.5f,0.5f),
        new Color(1.0f,0.5f,0.5f),
        new Color(1.0f,0.5f,0.5f),
        new Color(1.0f,0.5f,0.5f),
        new Color(1.0f,0.5f,0.5f),//15
        new Color(1.0f,0.5f,1.0f),
        new Color(1.0f,0.5f,1.0f),
        new Color(1.0f,0.5f,1.0f),
        new Color(1.0f,0.5f,1.0f),
        new Color(1.0f,0.5f,1.0f),//20
        new Color(0.1f,0.1f,0.1f),
        new Color(0.1f,0.1f,0.1f),
        new Color(0.1f,0.1f,0.1f),
        new Color(0.1f,0.1f,0.1f),
        new Color(0.1f,0.1f,0.1f),//25
        new Color(0.1f,0.1f,0.1f)
    };

}