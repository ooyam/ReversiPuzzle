using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;

public class GoogleAdmobReward : MonoBehaviour
{
    private RewardedAd rewardedAd = null;

    //�L�����j�b�gID
    private const string adId = "ca-app-pub-3940256099942544/5224354917"; //�e�X�g
    private bool rewardedAdRetry = false;

    //������
    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });
        LoadRewardAd();
    }

    //�X�V
    void Update()
    {
        if (rewardedAdRetry)
        {
            LoadRewardAd();
            rewardedAdRetry = false;
        }
    }

    /// <summary>
    /// �L���J�n
    /// </summary>
    /// <returns></returns>
    public bool ShowRewardAd()
    {
        if (rewardedAd.IsLoaded())
        {
            rewardedAd.Show();
            return true;
        }
        else
        {
            //Debug.Log("not loaded");
            return false;
        }
    }

    /// <summary>
    /// �L���ǂݍ���
    /// </summary>
    void LoadRewardAd()
    {
        // Clean up banner ad before creating a new one.
        if (rewardedAd != null)
        {
            rewardedAd = null;
        }

        rewardedAd = new RewardedAd(adId);
        // Register for ad events.
        rewardedAd.OnAdLoaded += HandleRewardAdLoaded;
        rewardedAd.OnAdFailedToLoad += HandleRewardAdFailedToLoad;
        rewardedAd.OnAdOpening += HandleRewardedAdAdOpened;
        rewardedAd.OnAdClosed += HandleRewardedAdAdClosed;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;

        AdRequest adRequest = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(adRequest);
    }

    /// <summary>
    /// �ǂݍ��݊���
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleRewardAdLoaded(object sender, EventArgs args)
    {
        //Debug.Log("HandleRewardAdLoaded event received with message: " + args);
        rewardedAdRetry = false;
        ResultMgr.mRewardState = PuzzleMain.ResultManager.AdRewardState.Loaded;
    }

    /// <summary>
    /// �ǂݍ��ݎ��s
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleRewardAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //LoadAdError loadAdError = args.LoadAdError;
        //int code = loadAdError.GetCode();
        //string message = loadAdError.GetMessage();

        //Debug.Log("Load error string: " + loadAdError.ToString());
        //Debug.Log("code: " + code.ToString());
        //MonoBehaviour.print("HandleRewardedAdFailedToLoad event received with message: " + message);

        //if (code == 2 || code == 0)
        //{
        //    Debug.Log("error");
        //}
        //else
        //{
        //    Debug.Log("error no fill");
        //}
        //rewardedAdRetry = true;
        ResultMgr.mRewardState = PuzzleMain.ResultManager.AdRewardState.FailedToLoad;
    }

    /// <summary>
    /// �L�����J����
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleRewardedAdAdOpened(object sender, EventArgs args)
    {
        //Debug.Log("HandleRewardedAdAdOpened event received");
        ResultMgr.mRewardState = PuzzleMain.ResultManager.AdRewardState.AdOpen;
    }

    /// <summary>
    /// �L���\�����s
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        //MonoBehaviour.print("HandleRewardedAdFailedToShow event received with message: " + args.AdError.GetMessage());
        ResultMgr.mRewardState = PuzzleMain.ResultManager.AdRewardState.FailedToOpen;
    }

    /// <summary>
    /// ��V�l��
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        //string type = args.Type;
        //double amount = args.Amount;
        //MonoBehaviour.print("HandleRewardedAdRewarded event received for " + amount.ToString() + " " + type);
        ResultMgr.mRewardState = PuzzleMain.ResultManager.AdRewardState.EarnedReward;
    }

    /// <summary>
    /// �I��
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleRewardedAdAdClosed(object sender, EventArgs args)
    {
        //Debug.Log("HandleRewardedAdClosed event received");
        ResultMgr.mRewardState = PuzzleMain.ResultManager.AdRewardState.AdClosed;
    }

    /// <summary>
    /// �I�u�W�F�N�g�j��
    /// </summary>
    public void RewardOnDestroy()
    {
        rewardedAd.Destroy();
        Destroy(this.gameObject);
    }
}
