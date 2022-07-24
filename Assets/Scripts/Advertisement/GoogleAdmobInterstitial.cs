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
        // �L���ǂݍ���
        RequestInterstitial();
    }

    void Update()
    {
        // �L���\��
        if (interstitial.IsLoaded() && !showing)
        {
            interstitial.Show();
            showing = true;
        }
    }

    // �L���ǂݍ��ݏ���
    private void RequestInterstitial()
    {
        // �������[�X���Ɏ�����ID�ɕύX����
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-6016270395550592/3554910086"; //�{��
        //string adUnitId = "ca-app-pub-3940256099942544/1033173712"; //�e�X�g
#else
        string adUnitId = "unexpected_platform";
#endif

        //InterstitialAd��������
        interstitial = new InterstitialAd(adUnitId);

        //�L�����N�G�X�g������ɓǂݍ��܂ꂽ�Ƃ�
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;

        //�L�����N�G�X�g�̓ǂݍ��݂Ɏ��s�����Ƃ�
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;

        //�L�����\�����ꂽ�Ƃ�
        this.interstitial.OnAdOpening += HandleOnAdOpened;

        //�L��������ꂽ�Ƃ�
        this.interstitial.OnAdClosed += HandleOnAdClosed;

        // ��̍L�����N�G�X�g���쐬
        AdRequest request = new AdRequest.Builder().Build();

        //�L���ǂݍ���
        interstitial.LoadAd(request);
    }

    // �V�[���J�ڏ���
    private void LoadNextScene()
    {
        SceneNavigator.Instance.Change("TitleScene", 1.0f);
    }

    private void OnDestroy()
    {
        // �I�u�W�F�N�g�̔j��
        interstitial.Destroy();
    }

    // ---�ȉ��A�C�x���g�n���h���[

    // �L���̓ǂݍ��݊�����
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
    }

    // �L���̓ǂݍ��ݎ��s��
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        // ���̃V�[���ɑJ��
        LoadNextScene();
    }

    // �L�����f�o�C�X�̉�ʂ����ς��ɕ\�����ꂽ�Ƃ�
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
    }

    // �L��������Ƃ�
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        // ���̃V�[���ɑJ��
        LoadNextScene();
    }
}