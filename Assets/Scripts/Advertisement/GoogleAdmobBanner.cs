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
#if UNITY_ANDROID
        MobileAds.Initialize(initStatus => { });

        RequestBanner();
#endif
    }

    void RequestBanner()
    {
        //バナー作成
        bannerView = new BannerView(GoogleAdmobDefine.BANNER_AD_UNIT_ID, AdSize.Banner, AdPosition.Top);

        //空のバナーリクエスト作成
        AdRequest request = new AdRequest.Builder().Build();

        //バナーを読み込む
        bannerView.LoadAd(request);
    }

    void OnDestroy()
    {
        bannerView.Destroy();
    }
}