using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleAdmobDefine : MonoBehaviour
{
#if UNITY_ANDROID
    //本番
    //public const string BANNER_AD_UNIT_ID       = "ca-app-pub-6016270395550592/9969748657"; //バナー
    //public const string INTERSTITIAL_AD_UNIT_ID = "ca-app-pub-6016270395550592/3862918575"; //インタースティシャル
    //public const string REWARD_AD_UNIT_ID       = "ca-app-pub-6016270395550592/1566297415"; //リワード

    //テスト
    public const string BANNER_AD_UNIT_ID = "ca-app-pub-6016270395550592/9969748657"; //バナー
    public const string INTERSTITIAL_AD_UNIT_ID = "ca-app-pub-3940256099942544/1033173712"; //インタースティシャル
    public const string REWARD_AD_UNIT_ID = "ca-app-pub-3940256099942544/5224354917"; //リワード
#else
    //各プラットフォーム
    public const string BANNER_AD_UNIT_ID       = "aaa"; //バナー
    public const string INTERSTITIAL_AD_UNIT_ID = "aaa"; //インタースティシャル
    public const string REWARD_AD_UNIT_ID       = "aaa"; //リワード
#endif
}
