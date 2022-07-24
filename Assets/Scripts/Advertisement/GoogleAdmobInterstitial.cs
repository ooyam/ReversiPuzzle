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

    void Start()
    {
        showing = false;
        // 広告読み込み
        RequestInterstitial();
    }

    void Update()
    {
        // 広告表示
        if (interstitial.IsLoaded() && !showing)
        {
            interstitial.Show();
            showing = true;
        }
    }

    // 広告読み込み処理
    private void RequestInterstitial()
    {
        // ★リリース時に自分のIDに変更する
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-6016270395550592/3554910086"; //本番
        //string adUnitId = "ca-app-pub-3940256099942544/1033173712"; //テスト
#else
        string adUnitId = "unexpected_platform";
#endif

        //InterstitialAdを初期化
        interstitial = new InterstitialAd(adUnitId);

        //広告リクエストが正常に読み込まれたとき
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;

        //広告リクエストの読み込みに失敗したとき
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;

        //広告が表示されたとき
        this.interstitial.OnAdOpening += HandleOnAdOpened;

        //広告が閉じられたとき
        this.interstitial.OnAdClosed += HandleOnAdClosed;

        // 空の広告リクエストを作成
        AdRequest request = new AdRequest.Builder().Build();

        //広告読み込み
        interstitial.LoadAd(request);
    }

    // シーン遷移処理
    private void LoadNextScene()
    {
        SceneNavigator.Instance.Change("TitleScene", 1.0f);
    }

    private void OnDestroy()
    {
        // オブジェクトの破棄
        interstitial.Destroy();
    }

    // ---以下、イベントハンドラー

    // 広告の読み込み完了時
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
    }

    // 広告の読み込み失敗時
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        // 次のシーンに遷移
        LoadNextScene();
    }

    // 広告がデバイスの画面いっぱいに表示されたとき
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
    }

    // 広告を閉じたとき
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        // 次のシーンに遷移
        LoadNextScene();
    }
}