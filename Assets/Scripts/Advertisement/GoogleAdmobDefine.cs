using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleAdmobDefine : MonoBehaviour
{
#if UNITY_ANDROID
    //�{��
    //public const string BANNER_AD_UNIT_ID       = "ca-app-pub-6016270395550592/9969748657"; //�o�i�[
    //public const string INTERSTITIAL_AD_UNIT_ID = "ca-app-pub-6016270395550592/3862918575"; //�C���^�[�X�e�B�V����
    //public const string REWARD_AD_UNIT_ID       = "ca-app-pub-6016270395550592/1566297415"; //�����[�h

    //�e�X�g
    public const string BANNER_AD_UNIT_ID = "ca-app-pub-6016270395550592/9969748657"; //�o�i�[
    public const string INTERSTITIAL_AD_UNIT_ID = "ca-app-pub-3940256099942544/1033173712"; //�C���^�[�X�e�B�V����
    public const string REWARD_AD_UNIT_ID = "ca-app-pub-3940256099942544/5224354917"; //�����[�h
#else
    //�e�v���b�g�t�H�[��
    public const string BANNER_AD_UNIT_ID       = "aaa"; //�o�i�[
    public const string INTERSTITIAL_AD_UNIT_ID = "aaa"; //�C���^�[�X�e�B�V����
    public const string REWARD_AD_UNIT_ID       = "aaa"; //�����[�h
#endif
}
