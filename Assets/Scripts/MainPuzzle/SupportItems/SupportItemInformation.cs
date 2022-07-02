using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;

public class SupportItemInformation : MonoBehaviour
{
    [System.NonSerialized] public Animator   ani;
    [System.NonSerialized] public Transform  tra;
    [System.NonSerialized] public GameObject obj;

    public Vector3 pos; //座標

    /// <summary>
    /// コンポーネントの設定
    /// </summary>
    void ComponentSetting()
    {
        ani = GetComponent<Animator>();
        tra = transform;
        obj = gameObject;
    }

    /// <summary>
    /// 援護アイテム情報の設定
    /// </summary>
    public void InformationSetting()
    {
        ComponentSetting();
        pos = tra.localPosition;
    }
}
