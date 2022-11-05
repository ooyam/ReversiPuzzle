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
    /// �N��
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            //�O���[�o���N���X�̏�����
            mSceneFader.Initialize();
            mSoundMgr.Initialize();
            mTapEffPlayer.Initialize();

            //�V�[��������
            SceneInitialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //�I���X�e�[�W�ԍ�
    public static StagesDataData SelectStageData { get; set; }

    /// <summary>
    /// �V�[��������
    /// </summary>
    public static void SceneInitialize()
    {
        //�e�V�[���̃��C���N���X������
        switch (SceneManager.GetActiveScene().name)
        {
            //�^�C�g���V�[���J�n
            case CommonDefine.TITLE_SCENE_NAME:
                sSceneType = SceneType.Title;
                GameObject.FindWithTag(MAIN_OBJ_TAG).GetComponent<Title.TitleMain>().Initialize();
                break;

            //�p�Y���V�[���J�n
            case CommonDefine.PUZZLE_SCENE_NAME:
                sSceneType = SceneType.Puzzle;
                GameObject.FindWithTag(MAIN_OBJ_TAG).GetComponent<PuzzleMain.PuzzleMain>().Initialize();
                break;
        }
    }

    /// <summary>
    /// �V�[���ڊǌ�t�F�[�h�I�����̏���
    /// </summary>
    public static void SceneChangeEnd()
    {
        //�t�F�[�h��������
        if (sSceneType == SceneType.Puzzle)
            PuzzleDefine.FlagOff(PuzzleDefine.PuzzleFlag.GamePreparation);
    }

#if UNITY_EDITOR
    /// <summary>
    /// �X�V
    /// </summary>
    void Update()
    {
        //�Q�[�������I��
        if (Input.GetKey(KeyCode.Escape)) QuitGame();
    }
#endif

    /// <summary>
    /// �Q�[���I��
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