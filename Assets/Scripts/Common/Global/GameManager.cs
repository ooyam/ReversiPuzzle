using UnityEngine;
using UnityEngine.SceneManagement;
using Sound;

public class GameManager : MonoBehaviour
{
    [Header("SceneFader")]
    [SerializeField]
    SceneFader mSceneFader;

    [Header("SoundManager")]
    [SerializeField]
    SoundManager mSoundMgr;

    [Header("TapEffectPlayer")]
    [SerializeField]
    TapEffectPlayer mTapEffPlayer;

    public static GameManager instance;
    const string MAIN_OBJ_TAG = "MainObject";

    public enum SceneType
    { Title, Puzzle }
    public static SceneType sSceneType;

    /// <summary>
    /// 起動
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            //グローバルクラスの初期化
            mSceneFader.Initialize();
            mSoundMgr.Initialize();
            mTapEffPlayer.Initialize();

            //シーン初期化
            SceneInitialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //選択ステージ番号
    public static StagesDataData SelectStageData { get; set; }

    /// <summary>
    /// シーン初期化
    /// </summary>
    public static void SceneInitialize()
    {
        //各シーンのメインクラス初期化
        switch (SceneManager.GetActiveScene().name)
        {
            //タイトルシーン開始
            case CommonDefine.TITLE_SCENE_NAME:
                sSceneType = SceneType.Title;
                GameObject.FindWithTag(MAIN_OBJ_TAG).GetComponent<Title.TitleMain>().Initialize();
                break;

            //パズルシーン開始
            case CommonDefine.PUZZLE_SCENE_NAME:
                sSceneType = SceneType.Puzzle;
                GameObject.FindWithTag(MAIN_OBJ_TAG).GetComponent<PuzzleMain.PuzzleMain>().Initialize();
                break;
        }
    }

    /// <summary>
    /// シーン移管後フェード終了時の処理
    /// </summary>
    public static void SceneChangeEnd()
    {
        //フェード完了判定
        if (sSceneType == SceneType.Puzzle)
            PuzzleDefine.FlagOff(PuzzleDefine.PuzzleFlag.GamePreparation);
    }

#if UNITY_EDITOR
    /// <summary>
    /// 更新
    /// </summary>
    void Update()
    {
        //ゲーム強制終了
        if (Input.GetKey(KeyCode.Escape)) QuitGame();
    }
#endif

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}