using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoundFunction;
using UnityEngine.SceneManagement;

public class ResultController : MonoBehaviour
{
    void Start()
    {
        //ボタンに関数追加
        Transform tra = transform;
        tra.GetChild(2).GetComponent<Button>().onClick.AddListener(() => TitlBackButtonDown());
        tra.GetChild(1).GetComponent<Button>().onClick.AddListener(() => RetryButtonDown());
    }

    //===================================================
    //タイトルに戻る
    //===================================================
    void TitlBackButtonDown()
    {
        GameObject SoundManObj = GameObject.FindWithTag("SoundManager");
        if (EnvironmentalSetting.se) GetComponent<AudioSource>().Play();
        Destroy(SoundManObj);
        SceneNavigator.Instance.Change("TitleScene", 0.5f);
    }
    //===================================================
    //もう一度
    //===================================================
    void RetryButtonDown()
    {
        GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>().YesTapSE();
        SceneNavigator.Instance.Change(SceneManager.GetActiveScene().name, 0.5f);
    }
}
