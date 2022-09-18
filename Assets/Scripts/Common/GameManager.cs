using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //�I���X�e�[�W�ԍ�
    public static StagesDataData SelectStageData { get; set; }

    /// <summary>
    /// �V�[���ڊǏI�����̏���
    /// </summary>
    public static void SceneChangeEnd()
    {
        //�p�Y���V�[���Ɉڊǂ����ꍇ
        if (SceneManager.GetActiveScene().name == CommonDefine.PUZZLE_SCENE_NAME)
        {
            //�������t���O�̃��Z�b�g
            PuzzleDefine.FlagOff(PuzzleDefine.PuzzleFlag.GamePreparation);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) QuitGame();
    }

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