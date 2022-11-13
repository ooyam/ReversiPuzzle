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

    //L‚Ìó‘Ô
    public enum State
    {
        None,
        Loading,    //“Ç‚İ‚İ’†
        loaded,     //“Ç‚İ‚İŠ®—¹
        loadFailed, //“Ç‚İ‚İ¸”s
        Showing,    //•\¦’†
        Closed,     //•Â‚¶‚½
    }
    public State AdState { get; private set; } = State.None;

    /// <summary>
    /// L“Ç‚İ‚İŠJn
    /// </summary>
    public void AdInterstitialStart()
    {
        // L“Ç‚İ‚İ
        RequestInterstitial();
    }

    void Update()
    {
        // L•\¦
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
    /// L“Ç‚İ‚İˆ—
    /// </summary>
    void RequestInterstitial()
    {
        //InterstitialAd‚ğ‰Šú‰»
        interstitial = new InterstitialAd(GoogleAdmobDefine.INTERSTITIAL_AD_UNIT_ID);

        //LƒŠƒNƒGƒXƒg‚ª³í‚É“Ç‚İ‚Ü‚ê‚½‚Æ‚«
        interstitial.OnAdLoaded += HandleOnAdLoaded;

        //LƒŠƒNƒGƒXƒg‚Ì“Ç‚İ‚İ‚É¸”s‚µ‚½‚Æ‚«
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;

        //L‚ª•\¦‚³‚ê‚½‚Æ‚«
        interstitial.OnAdOpening += HandleOnAdOpened;

        //L‚ª•Â‚¶‚ç‚ê‚½‚Æ‚«
        interstitial.OnAdClosed += HandleOnAdClosed;

        // ‹ó‚ÌLƒŠƒNƒGƒXƒg‚ğì¬
        AdRequest request = new AdRequest.Builder().Build();

        //L“Ç‚İ‚İ
        interstitial.LoadAd(request);
        AdState = State.Loading;
    }

    /// <summary>
    /// L”j‰ó
    /// </summary>
    public void OnDestroy()
    {
        if (interstitial != null)
            interstitial.Destroy();
    }

    // ---ˆÈ‰ºAƒCƒxƒ“ƒgƒnƒ“ƒhƒ‰[

    /// <summary>
    /// L‚Ì“Ç‚İ‚İŠ®—¹
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        AdState = State.loaded;
    }

    /// <summary>
    /// L‚Ì“Ç‚İ‚İ¸”s
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        AdState = State.loadFailed;
    }

    /// <summary>
    /// L‚ª•\¦‚³‚ê‚½‚Æ‚«
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        AdState = State.Showing;
    }

    /// <summary>
    /// L‚ğ•Â‚¶‚½‚Æ‚«
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        AdState = State.Closed;
    }
}