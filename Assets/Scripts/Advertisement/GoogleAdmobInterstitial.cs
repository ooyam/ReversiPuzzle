using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using System;

public class GoogleAdmobInterstitial : MonoBehaviour
{
    private InterstitialAd interstitial;
    private bool showing;

    //広告の状態
    public enum State
    {
        None,
        Loading,    //読み込み中
        loaded,     //読み込み完了
        loadFailed, //読み込み失敗
        Showing,    //表示中
        Closed,     //閉じた
    }
    public State AdState { get; private set; } = State.None;

    /// <summary>
    /// 広告読み込み開始
    /// </summary>
    public void AdInterstitialStart()
    {
        // 広告読み込み
        RequestInterstitial();
    }

    void Update()
    {
        // 広告表示
        if (interstitial != null)
        {
            if (interstitial.IsLoaded() && !showing)
            {
                interstitial.Show();
                showing = true;
            }
        }
    }

    /// <summary>
    /// 広告読み込み処理
    /// </summary>
    void RequestInterstitial()
    {
        //広告ユニットID
#if UNITY_ANDROID
        //string adUnitId = "ca-app-pub-6016270395550592/3862918575"; //本番
        string adUnitId = "ca-app-pub-3940256099942544/1033173712"; //テスト
#else
        string adUnitId = "unexpected_platform";
#endif

        //InterstitialAdを初期化
        interstitial = new InterstitialAd(adUnitId);

        //広告リクエストが正常に読み込まれたとき
        interstitial.OnAdLoaded += HandleOnAdLoaded;

        //広告リクエストの読み込みに失敗したとき
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;

        //広告が表示されたとき
        interstitial.OnAdOpening += HandleOnAdOpened;

        //広告が閉じられたとき
        interstitial.OnAdClosed += HandleOnAdClosed;

        // 空の広告リクエストを作成
        AdRequest request = new AdRequest.Builder().Build();

        //広告読み込み
        interstitial.LoadAd(request);
        AdState = State.Loading;
    }

    /// <summary>
    /// 広告破壊
    /// </summary>
    public void OnDestroy()
    {
        if (interstitial != null)
            interstitial.Destroy();
    }

    // ---以下、イベントハンドラー

    /// <summary>
    /// 広告の読み込み完了時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        AdState = State.loaded;
    }

    /// <summary>
    /// 広告の読み込み失敗時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        AdState = State.loadFailed;
    }

    /// <summary>
    /// 広告が表示されたとき
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        AdState = State.Showing;
    }

    /// <summary>
    /// 広告を閉じたとき
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        AdState = State.Closed;
    }
}