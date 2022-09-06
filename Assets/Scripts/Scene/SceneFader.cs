using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using ObjectMove;

public class SceneFader : MonoBehaviour
{
    //�V�[���t�F�[�h���x
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
    /// �t�F�[�h�ŃV�[���ڊ�
    /// </summary>
    /// <param name="_sceneName">�V�[����</param>
    public static void SceneChangeFade(string _sceneName)
    {
        instance.StartCoroutine(FadeOut(_sceneName));
    }

    /// <summary>
    /// �t�F�[�h�A�E�g
    /// </summary>
    /// <param name="_sceneName">�V�[����</param>
    static IEnumerator FadeOut(string _sceneName)
    {
        yield return instance.StartCoroutine(ObjectMove_UI.ImagePaletteChange(mFilter, FADE_SPEED, mFadeOutColors));
        SceneManager.LoadScene(_sceneName);
        yield return null;
        instance.StartCoroutine(FadeIn());
    }

    /// <summary>
    /// �t�F�[�h�C��
    /// </summary>

    static IEnumerator FadeIn()
    {
        yield return instance.StartCoroutine(ObjectMove_UI.ImagePaletteChange(mFilter, FADE_SPEED, mFadeInColors));
        GameManager.SceneChangeEnd();
    }
}
