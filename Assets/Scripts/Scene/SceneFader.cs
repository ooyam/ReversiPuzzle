using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using ObjectMove;

public class SceneFader : MonoBehaviour
{
    //シーンフェード速度
    const float FADE_SPEED = 0.2f;

    static Image mFilter;
    static readonly Color32[] mFadeOutColors = new Color32[] { Color.clear, Color.black };
    static readonly Color32[] mFadeInColors  = new Color32[] { Color.black, Color.clear };
    public static SceneFader instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            mFilter = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        yield return instance.StartCoroutine(FadeOut());
        SceneManager.LoadScene(_sceneName);
        yield return null;
        instance.StartCoroutine(SceneChangeFadeIn());
    }

    /// <summary>
    /// シーン移管フェードイン
    /// </summary>

    static IEnumerator SceneChangeFadeIn()
    {
        yield return instance.StartCoroutine(FadeIn());
        GameManager.SceneChangeEnd();
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <returns></returns>
    public static IEnumerator FadeOut()
    {
        yield return instance.StartCoroutine(ObjectMove_UI.ImagePaletteChange(mFilter, FADE_SPEED, mFadeOutColors));
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    /// <returns></returns>
    static IEnumerator FadeIn()
    {
        yield return instance.StartCoroutine(ObjectMove_UI.ImagePaletteChange(mFilter, FADE_SPEED, mFadeInColors));
    }
}
