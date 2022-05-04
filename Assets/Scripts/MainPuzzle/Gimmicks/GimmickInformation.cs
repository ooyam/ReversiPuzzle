using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;

public class GimmickInformation : MonoBehaviour
{
    //コンポーネント
    [System.NonSerialized] public Animator ani;
    [System.NonSerialized] public Transform tra;
    [System.NonSerialized] public SpriteRenderer   spriRen;
    [System.NonSerialized] public SpriteRenderer[] spriRenChild;

    //ギミック情報
    [System.NonSerialized] public int     startSquareId;  //初期マス番号
    [System.NonSerialized] public int     groupId;        //グループ番号
    [System.NonSerialized] public int     id;             //ギミック番号
    [System.NonSerialized] public int     colorId;        //色番号
    [System.NonSerialized] public int     remainingTimes; //残りダメージ回数
    [System.NonSerialized] public bool    freeFall;       //自由落下フラグ
    [System.NonSerialized] public bool    destructible;   //連続ダメージフラグ(trueの時破壊可能)
    [System.NonSerialized] public bool    nowTurnDamage;  //今のターンにダメージを受けたかのフラグ
    [System.NonSerialized] public Vector3 defaultPos;     //基準座標

    /// <summary>
    /// コンポーネントの設定
    /// </summary>
    void ComponentSetting()
    {
        ani     = this.GetComponent<Animator>();
        tra     = this.transform;
        spriRen = this.GetComponent<SpriteRenderer>();

        int SpriRenCount = tra.childCount;
        spriRenChild = new SpriteRenderer[SpriRenCount];
        for (int i = 0; i < SpriRenCount; i++)
        { spriRenChild[i] = tra.GetChild(i).GetComponent<SpriteRenderer>(); }
    }

    /// <summary>
    /// ギミック情報の設定
    /// </summary>
    /// <param name="_index">ステージ毎のギミック管理番号</param>
    public void InformationSetting(int _index)
    {
        ComponentSetting();
        var gimmickData = GIMMICKS_DATA.param[GIMMICKS_INFO_ARR[_index][GIMMICK]];
        startSquareId   = GIMMICKS_INFO_ARR[_index][SQUARE];
        groupId         = NOT_GROUP_ID;
        id              = gimmickData.id;
        colorId         = GIMMICKS_INFO_ARR[_index][COLOR];
        remainingTimes  = gimmickData.damage_times;
        freeFall        = gimmickData.free_fall;
        destructible    = !gimmickData.continuous;
        defaultPos      = new Vector3(gimmickData.position_x, gimmickData.position_y, PIECE_DEFAULT_POS.z);
    }

    /// <summary>
    /// グループギミック情報の設定
    /// </summary>
    /// <param name="_groupId">グループ番号</param>
    public void GroupInformationSetting(int _groupId)
    {
        for (int i = 0; i < GIMMICKS_COUNT; i++)
        {
            if (GIMMICKS_INFO_ARR[i][GROUP] == _groupId)
            {
                InformationSetting(i);
                break;
            }
        }
        groupId = _groupId;
    }
}