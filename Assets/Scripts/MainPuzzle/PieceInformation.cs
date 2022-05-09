using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;

public class PieceInformation : MonoBehaviour
{
    //コンポーネント
    [System.NonSerialized] public Transform tra;

    //駒情報
    [Header("駒の色選択")]
    public Colors color;                            //色
    [System.NonSerialized] public int  colorId;     //色番号
    [System.NonSerialized] public int  squareId;    //マス管理番号
    [System.NonSerialized] public bool freeFall;    //自由落下フラグ
    [System.NonSerialized] public bool invertable;  //反転可能フラグ

    /// <summary>
    /// 駒情報の設定
    /// </summary>
    /// <param name="_squareIndex">     配置マス管理番号</param>
    /// <param name="_generate">        生成？</param>
    /// <param name="gimmickInfoArr">   ギミック情報配列</param>
    public void InformationSetting(int _squareIndex, bool _generate, GimmickInformation[] gimmickInfoArr = null)
    {
        tra        = this.transform;
        colorId    = (int)color;
        squareId   = _squareIndex;
        freeFall   = true;
        invertable = true;
        if (_generate)
        {
            foreach (GimmickInformation gimmickInfo in gimmickInfoArr)
            {
                foreach (int innerSquareId in gimmickInfo.innerSquaresId)
                {
                    //ギミックの内側に生成された場合
                    if (innerSquareId == _squareIndex)
                    {
                        freeFall   = false;
                        invertable = false;
                    }
                }
            }
        }
    }
}
