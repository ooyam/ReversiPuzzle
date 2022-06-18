using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;

public class SupportItemInformation : MonoBehaviour
{
    [System.NonSerialized] public Animator   ani;
    [System.NonSerialized] public Transform  tra;
    [System.NonSerialized] public GameObject obj;

    /// <summary>
    /// コンポーネントの設定
    /// </summary>
    void ComponentSetting()
    {
        ani = this.GetComponent<Animator>();
        tra = this.transform;
        obj = this.gameObject;
    }

    /// <summary>
    /// 援護アイテム情報の設定
    /// </summary>
    public void InformationSetting()
    {
        ComponentSetting();
    }
}
