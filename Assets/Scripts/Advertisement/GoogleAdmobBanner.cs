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
        //�o�i�[�쐬
        bannerView = new BannerView(GoogleAdmobDefine.BANNER_AD_UNIT_ID, AdSize.Banner, AdPosition.Top);

        //��̃o�i�[���N�G�X�g�쐬
        AdRequest request = new AdRequest.Builder().Build();

        //�o�i�[��ǂݍ���
        bannerView.LoadAd(request);
    }

    void OnDestroy()
    {
        bannerView.Destroy();
    }
}