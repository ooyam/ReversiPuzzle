using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;

public class GimmickInformation : MonoBehaviour
{
    //コンポーネント
    [System.NonSerialized] public SpriteRenderer spriRen;
    [System.NonSerialized] public Animator ani;
    [System.NonSerialized] public Transform tra;

    //ギミック情報
    [System.NonSerialized] public int     objIndex;       //ステージ毎の管理番号
    [System.NonSerialized] public int     id;             //ギミック番号
    [System.NonSerialized] public int     colorNum;       //色番号
    [System.NonSerialized] public int     remainingTimes; //残りダメージ回数
    [System.NonSerialized] public bool    freeFall;       //自由落下フラグ
    [System.NonSerialized] public bool    destructible;   //連続ダメージフラグ(trueの時破壊可能)
    [System.NonSerialized] public bool    nowTurnDamage;  //今のターンにダメージを受けたかのフラグ
    [System.NonSerialized] public Vector3 defaultPos;     //基準座標

    void Start()
    {
        spriRen = this.GetComponent<SpriteRenderer>();
        ani     = this.GetComponent<Animator>();
        tra     = this.transform;
    }

    /// <summary>
    /// ギミック情報の設定
    /// </summary>
    /// <param name="index"></param>
    public void informationSetting(int index)
    {
        objIndex       = index;
        id             = GIMMICK_INFO_ARR[index][GIMMICK];
        colorNum       = GIMMICK_INFO_ARR[index][COLOR];
        remainingTimes = GIMMICK_DAMAGE_TIMES[id];
        freeFall       = GIMMICK_FREE_FALL[id];
        destructible   = !GIMMICK_CONTINUOUS[id];
        defaultPos = new Vector3(GIMMICK_POS_X[id], GIMMICK_POS_Y[id], PIECE_DEFAULT_POS.z);
    }
}