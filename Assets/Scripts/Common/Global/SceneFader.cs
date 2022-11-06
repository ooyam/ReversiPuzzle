using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using ObjectMove;

public class SceneFader : MonoBehaviour
{
    [Header("GlobalFilter")]
    [SerializeField]
    Image mGlobalFilter;

    //シーンフェード速度
    const float FADE_SPEED = 0.2f;

    static readonly Color32[] mFadeOutColors = new Color32[] { Color.clear, Color.black };
    static readonly Color32[] mFadeInColors  = new Color32[] { Color.black, Color.clear };
    public static SceneFader instance;
    public static bool IsSceneChanging { get; private set; } = false;  //シーン移管中？

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize() => instance = this;

    /// <summary>
    /// フェードでシーン移管
    /// </summary>
    /// <param name="_sceneName">シーン名</param>
    public static void SceneChangeFade(string _sceneName)
    {
        instance.StartCoroutine(SceneChangeFadeOut(_sceneName));
    }

    /// <summary>
    /// シーン移管フェードアウト
    /// </summary>
    /// <param name="_sceneName">シーン名</param>
    static IEnumerator SceneChangeFadeOut(string _sceneName)
    {
        if (IsSceneChanging) yield break;
        IsSceneChanging = true;
        yield return instance.StartCoroutine(FadeOut());
        SceneManager.LoadScene(_sceneName);
        yield return null;
        GameManager.SceneInitialize();
        instance.StartCoroutine(SceneChangeFadeIn());
    }

    /// <summary>
    /// シーン移管フェードイン
    /// </summary>

    static IEnumerator SceneChangeFadeIn()
    {
        yield return instance.StartCoroutine(FadeIn());
        GameManager.SceneChangeEnd();
        IsSceneChanging = false;
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <returns></returns>
    public static IEnumerator FadeOut()
    {
        yield return instance.StartCoroutine(ObjectMove_UI.ImagePaletteChange(instance.mGlobalFilter, FADE_SPEED, mFadeOutColors));
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    /// <returns></returns>
    static IEnumerator FadeIn()
    {
        yield return instance.StartCoroutine(ObjectMove_UI.ImagePaletteChange(instance.mGlobalFilter, FADE_SPEED, mFadeInColors));
    }
}
