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

    //�L���̏��
    public enum State
    {
        None,
        Loading,    //�ǂݍ��ݒ�
        loaded,     //�ǂݍ��݊���
        loadFailed, //�ǂݍ��ݎ��s
        Showing,    //�\����
        Closed,     //����
    }
    public State AdState { get; private set; } = State.None;

    /// <summary>
    /// �L���ǂݍ��݊J�n
    /// </summary>
    public void AdInterstitialStart()
    {
        // �L���ǂݍ���
        RequestInterstitial();
    }

    void Update()
    {
        // �L���\��
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
    /// �L���ǂݍ��ݏ���
    /// </summary>
    void RequestInterstitial()
    {
        //�L�����j�b�gID
#if UNITY_ANDROID
        //string adUnitId = "ca-app-pub-6016270395550592/3862918575"; //�{��
        string adUnitId = "ca-app-pub-3940256099942544/1033173712"; //�e�X�g
#else
        string adUnitId = "unexpected_platform";
#endif

        //InterstitialAd��������
        interstitial = new InterstitialAd(adUnitId);

        //�L�����N�G�X�g������ɓǂݍ��܂ꂽ�Ƃ�
        interstitial.OnAdLoaded += HandleOnAdLoaded;

        //�L�����N�G�X�g�̓ǂݍ��݂Ɏ��s�����Ƃ�
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;

        //�L�����\�����ꂽ�Ƃ�
        interstitial.OnAdOpening += HandleOnAdOpened;

        //�L��������ꂽ�Ƃ�
        interstitial.OnAdClosed += HandleOnAdClosed;

        // ��̍L�����N�G�X�g���쐬
        AdRequest request = new AdRequest.Builder().Build();

        //�L���ǂݍ���
        interstitial.LoadAd(request);
        AdState = State.Loading;
    }

    /// <summary>
    /// �L���j��
    /// </summary>
    public void OnDestroy()
    {
        if (interstitial != null)
            interstitial.Destroy();
    }

    // ---�ȉ��A�C�x���g�n���h���[

    /// <summary>
    /// �L���̓ǂݍ��݊�����
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        AdState = State.loaded;
    }

    /// <summary>
    /// �L���̓ǂݍ��ݎ��s��
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        AdState = State.loadFailed;
    }

    /// <summary>
    /// �L�����\�����ꂽ�Ƃ�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        AdState = State.Showing;
    }

    /// <summary>
    /// �L��������Ƃ�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        AdState = State.Closed;
    }
}