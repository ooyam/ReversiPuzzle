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

    //選択ステージ番号
    public static StagesDataData SelectStageData { get; set; }

    /// <summary>
    /// シーン移管終了時の処理
    /// </summary>
    public static void SceneChangeEnd()
    {
        //パズルシーンに移管した場合
        if (SceneManager.GetActiveScene().name == CommonDefine.PUZZLE_SCENE_NAME)
        {
            //準備中フラグのリセット
            PuzzleDefine.FlagOff(PuzzleDefine.PuzzleFlag.GamePreparation);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) QuitGame();
    }

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