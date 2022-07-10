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
    [System.NonSerialized] public GameObject       obj;
    [System.NonSerialized] public GameObject[]     objChild;

    //ギミック情報
    [System.NonSerialized] public int     settingIndex;         //ステージ毎の設定番号
    [System.NonSerialized] public int     startSquareId;        //初期マス番号
    [System.NonSerialized] public int     nowSquareId;          //現在のマス番号
    [System.NonSerialized] public int     groupId;              //グループ番号
    [System.NonSerialized] public int     id;                   //ギミック番号
    [System.NonSerialized] public int     colorId;              //色番号
    [System.NonSerialized] public int     remainingTimes;       //残りダメージ回数(ギミックの固定数)
    [System.NonSerialized] public int     remainingQuantity;    //残りダメージ回数(ステージ毎の指定数)
    [System.NonSerialized] public int     order;                //指定番号
    [System.NonSerialized] public bool    freeFall;             //自由落下フラグ
    [System.NonSerialized] public bool    destructible;         //破壊可能フラグ(true：破壊可能)
    [System.NonSerialized] public bool    nowTurnDamage;        //今のターンにダメージを受けたかのフラグ
    [System.NonSerialized] public bool    assaultOnly;          //強撃でのみ破壊可能
    [System.NonSerialized] public bool    inSquare;             //駒として配置するかのフラグ(true：駒として配置)
    [System.NonSerialized] public Vector3 defaultPos;           //基準座標
    [System.NonSerialized] public Vector3 defaultScale;         //基準スケール
    [System.NonSerialized] public int[]   innerSquaresId;       //内側のマス番号

    //駒として管理する場合の情報
    [System.NonSerialized] public bool freeFall_Piece;          //自由落下フラグ(駒として)
    [System.NonSerialized] public bool destructible_Piece;      //破壊可能フラグ(true：破壊可能)


    /// <summary>
    /// コンポーネントの設定
    /// </summary>
    void ComponentSetting()
    {
        ani     = this.GetComponent<Animator>();
        tra     = this.transform;
        spriRen = this.GetComponent<SpriteRenderer>();
        obj     = this.gameObject;

        int childCount = tra.childCount;
        objChild     = new GameObject[childCount];
        spriRenChild = new SpriteRenderer[childCount];
        for (int i = 0; i < childCount; i++)
        {
            objChild[i]     = tra.GetChild(i).gameObject;
            spriRenChild[i] = objChild[i].GetComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// ギミック情報の設定
    /// </summary>
    /// <param name="_index">ステージ毎のギミック管理番号</param>
    public void InformationSetting(int _index)
    {
        ComponentSetting();
        var gimmickData     = GIMMICKS_DATA.param[GIMMICKS_INFO_ARR[_index][GIMMICK]];
        settingIndex        = _index;
        startSquareId       = GIMMICKS_INFO_ARR[_index][SQUARE];
        nowSquareId         = startSquareId;
        groupId             = GIMMICKS_INFO_ARR[_index][GROUP];
        id                  = gimmickData.id;
        colorId             = GIMMICKS_INFO_ARR[_index][COLOR];
        remainingTimes      = gimmickData.damage_times;
        remainingQuantity   = GIMMICKS_INFO_ARR[_index][QUANTITY];
        order               = GIMMICKS_INFO_ARR[_index][ORDER];
        freeFall            = gimmickData.free_fall;
        destructible        = !gimmickData.continuous;
        assaultOnly         = gimmickData.assault_only;
        inSquare            = gimmickData.in_square;
        defaultPos          = new Vector3(gimmickData.position_x, gimmickData.position_y, (inSquare) ? Z_PIECE : Z_GIMMICK);
        defaultScale        = new Vector3(gimmickData.scale_x, gimmickData.scale_y, 1.0f);

        switch (id)
        {
            //ギミック内側の駒を操作禁止にするギミック
            case (int)Gimmicks.Cage:    //檻
                innerSquaresId = new int[GIMMICKS_INFO_ARR[_index][WIDTH] * GIMMICKS_INFO_ARR[_index][HEIGHT]];
                int i = 0;
                for (int w = 0; w < GIMMICKS_INFO_ARR[_index][WIDTH]; w++)      //幅分ループ
                {
                    for (int h = 0; h < GIMMICKS_INFO_ARR[_index][HEIGHT]; h++) //高さ分ループ
                    {
                        innerSquaresId[i] = startSquareId + w * BOARD_LINE_COUNT + h;
                        i++;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// ギミック情報の設定(マス番号から)
    /// </summary>
    /// <param name="_squareIndex">マス管理番号</param>
    /// <param name="_gimmickId">  ギミック番号</param>
    /// <param name="_groupId">    グループ番号</param>
    public void InformationSetting_SquareIndex(int _squareIndex, int _gimmickId, int _groupId)
    {
        for (int i = 0; i < GIMMICKS_COUNT; i++)
        {
            if (GIMMICKS_INFO_ARR[i][SQUARE] == _squareIndex && 
                (GIMMICKS_INFO_ARR[i][GIMMICK] == _gimmickId || GIMMICKS_INFO_ARR[i][GROUP] == _groupId))
            {
                InformationSetting(i);
                break;
            }
        }
    }


    /// <summary>
    /// 駒としてフラグ設定
    /// </summary>
    /// <param name="_squareIndex">     配置マス管理番号</param>
    /// <param name="_generate">        生成？</param>
    /// <param name="gimmickInfoArr">   ギミック情報配列</param>
    public void OperationFlagSetting(int _squareIndex, bool _generate, GimmickInformation[] gimmickInfoArr = null)
    {
        OperationFlagON();
        if (!_generate || !inSquare) return;

        foreach (GimmickInformation gimmickInfo in gimmickInfoArr)
        {
            if (gimmickInfo == null) continue;
            if (gimmickInfo.innerSquaresId == null) continue;
            foreach (int squareId in gimmickInfo.innerSquaresId)
            {
                //ギミックの内側に生成された場合
                if (squareId == _squareIndex)
                {
                    OperationFlagOFF();
                }
            }
        }
    }

    /// <summary>
    /// 操作フラグをオンにする
    /// </summary>
    public void OperationFlagON()
    {
        freeFall_Piece = true;
        destructible_Piece = true;
    }

    /// <summary>
    /// 操作フラグをオンにする
    /// </summary>
    public void OperationFlagOFF()
    {
        freeFall_Piece = false;
        destructible_Piece = false;
    }
}