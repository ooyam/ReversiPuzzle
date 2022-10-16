using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class GoogleAdmobBanner : MonoBehaviour
{
    BannerView bannerView;

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        this.RequestBanner();
    }

    private void RequestBanner()
    {
        //広告ユニットID
#if UNITY_ANDROID
        //string adUnitId = "ca-app-pub-6016270395550592/9969748657"; //本番
        string adUnitId = "ca-app-pub-3940256099942544/6300978111"; //テスト
#else
        string adUnitId = "unexpected_platform";
#endif
        //バナー作成
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        //空のバナーリクエスト作成
        AdRequest request = new AdRequest.Builder().Build();

        //バナーを読み込む
        this.bannerView.LoadAd(request);
    }

    private void OnDestroy()
    {
        bannerView.Destroy();
    }
}