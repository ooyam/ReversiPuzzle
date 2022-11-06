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

    //�V�[���t�F�[�h���x
    const float FADE_SPEED = 0.2f;

    static readonly Color32[] mFadeOutColors = new Color32[] { Color.clear, Color.black };
    static readonly Color32[] mFadeInColors  = new Color32[] { Color.black, Color.clear };
    public static SceneFader instance;
    public static bool IsSceneChanging { get; private set; } = false;  //�V�[���ڊǒ��H

    /// <summary>
    /// ������
    /// </summary>
    public void Initialize() => instance = this;

    /// <summary>
    /// �t�F�[�h�ŃV�[���ڊ�
    /// </summary>
    /// <param name="_sceneName">�V�[����</param>
    public static void SceneChangeFade(string _sceneName)
    {
        instance.StartCoroutine(SceneChangeFadeOut(_sceneName));
    }

    /// <summary>
    /// �V�[���ڊǃt�F�[�h�A�E�g
    /// </summary>
    /// <param name="_sceneName">�V�[����</param>
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
    /// �V�[���ڊǃt�F�[�h�C��
    /// </summary>

    static IEnumerator SceneChangeFadeIn()
    {
        yield return instance.StartCoroutine(FadeIn());
        GameManager.SceneChangeEnd();
        IsSceneChanging = false;
    }

    /// <summary>
    /// �t�F�[�h�A�E�g
    /// </summary>
    /// <returns></returns>
    public static IEnumerator FadeOut()
    {
        yield return instance.StartCoroutine(ObjectMove_UI.ImagePaletteChange(instance.mGlobalFilter, FADE_SPEED, mFadeOutColors));
    }

    /// <summary>
    /// �t�F�[�h�C��
    /// </summary>
    /// <returns></returns>
    static IEnumerator FadeIn()
    {
        yield return instance.StartCoroutine(ObjectMove_UI.ImagePaletteChange(instance.mGlobalFilter, FADE_SPEED, mFadeInColors));
    }
}
