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
        //�L�����j�b�gID
#if UNITY_ANDROID
        //string adUnitId = "ca-app-pub-6016270395550592/9969748657"; //�{��
        string adUnitId = "ca-app-pub-3940256099942544/6300978111"; //�e�X�g
#else
        string adUnitId = "unexpected_platform";
#endif
        //�o�i�[�쐬
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        //��̃o�i�[���N�G�X�g�쐬
        AdRequest request = new AdRequest.Builder().Build();

        //�o�i�[��ǂݍ���
        this.bannerView.LoadAd(request);
    }

    private void OnDestroy()
    {
        bannerView.Destroy();
    }
}