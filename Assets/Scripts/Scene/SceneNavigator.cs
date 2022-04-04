//  SceneNavigator.cs
//  http://kan-kikuchi.hatenablog.com/entry/SceneNavigator
//
//  Created by kan.kikuchi on 2017.05.14.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// シーンの遷移を実行、管理するクラス
/// </summary>
public class SceneNavigator : SingletonMonoBehaviour<SceneNavigator>
{
    public static SceneNavigator instance = null;
    new void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) Quit();
    }
    void Quit()
    {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
    }

    //フェード中か否か
    public bool IsFading
    {
        get { return _fader.IsFading || _fader.Alpha != 0; }
    }

    //一個前と現在、次のシーン名
    private string _beforeSceneName = "";
    public string BeforeSceneName
    {
        get { return _beforeSceneName; }
    }

    private string _currentSceneName = "";
    public string CurrentSceneName
    {
        get { return _currentSceneName; }
    }

    private string _nextSceneName = "";
    public string NextSceneName
    {
        get { return _nextSceneName; }
    }

    //フェード後のイベント
    public event Action FadeOutFinished = delegate { };
    public event Action FadeInFinished = delegate { };

    //フェード用クラス
    [SerializeField]
    private CanvasFader _fader = null;

    //フェード時間
    public const float FADE_TIME = 0.5f;
    private float _fadeTime = FADE_TIME;

    //=================================================================================
    //初期化
    //=================================================================================

    /// <summary>
    /// 初期化(Awake時かその前の初アクセス時、どちらかの一度しか行われない)
    /// </summary>
    protected override void Init()
    {
        base.Init();

        //実機上やエディタを実行している時にはAddした場合はResetが実行されないので、Initから実行
        if (_fader == null)
        {
            Reset();
        }

        //最初のシーン名設定
        _currentSceneName = SceneManager.GetSceneAt(0).name;

        //永続化し、フェード用のキャンバスを非表示に
        DontDestroyOnLoad(gameObject);
        _fader.gameObject.SetActive(false);
    }

    //コンポーネント追加時に自動で実行される(実機上やエディタを実行している時には動作しない)
    private void Reset()
    {
        //オブジェクトの名前を設定
        gameObject.name = "SceneNavigator";

        //フェード用のキャンバス作成
        GameObject fadeCanvas = new GameObject("FadeCanvas");
        fadeCanvas.transform.SetParent(transform);
        fadeCanvas.SetActive(false);

        Canvas canvas = fadeCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        fadeCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        fadeCanvas.AddComponent<GraphicRaycaster>();
        _fader = fadeCanvas.AddComponent<CanvasFader>();
        _fader.Alpha = 0;

        //フェード用の画像作成
        GameObject imageObject = new GameObject("Image");
        imageObject.transform.SetParent(fadeCanvas.transform, false);
        imageObject.AddComponent<Image>().color = Color.black;
        imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, 2000);
    }

    //=================================================================================
    //シーンの変更
    //=================================================================================

    /// <summary>
    /// シーンの変更
    /// </summary>
    public void Change(string sceneName, float fadeTime = FADE_TIME)
    {
        if (IsFading)
        {
            Debug.LogError("フェード中です！");
            return;
        }

        //次のシーン名とフェード時間を設定
        _nextSceneName = sceneName;
        _fadeTime = fadeTime;

        //フェードアウト
        _fader.gameObject.SetActive(true);
        _fader.Play(isFadeOut: false, duration: _fadeTime, onFinished: OnFadeOutFinish);
    }

    //フェードアウト終了
    private void OnFadeOutFinish()
    {
        FadeOutFinished();

        //シーン読み込み、変更
        SceneManager.LoadScene(_nextSceneName);

        //シーン名更新
        _beforeSceneName = _currentSceneName;
        _currentSceneName = _nextSceneName;

        //フェードイン開始
        _fader.gameObject.SetActive(true);
        _fader.Alpha = 1;
        _fader.Play(isFadeOut: true, duration: _fadeTime, onFinished: OnFadeInFinish);
    }

    //フェードイン終了
    private void OnFadeInFinish()
    {
        _fader.gameObject.SetActive(false);
        FadeInFinished();
    }

}