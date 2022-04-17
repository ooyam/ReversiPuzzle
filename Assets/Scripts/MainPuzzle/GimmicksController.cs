using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static PuzzleDefine;
using static PieceManager;

public class GimmicksController : MonoBehaviour
{
    /// <summary>
    /// ギミックにダメージがあるかの確認
    /// </summary>
    /// /// <param name="putPieceTag"> 置いた駒のタグ</param>
    /// /// <param name="gimmickIndex">ギミック管理番号(ステージ毎の)</param>
    /// /// <param name="squareIndex"> ギミック配置番号</param>
    /// /// <param name="gimmickObj">  ギミックオブジェクト</param>
    public bool DamageCheck(ref string putPieceTag, ref int gimmickIndex, int squareIndex, ref GameObject gimmickObj)
    {
        //ダメージの有無フラグ
        bool damage = false;

        switch (GIMMICK_INFO_ARR[gimmickIndex][GIMMICK])
        {
            case (int)Gimmicks.Balloon:  //風船
            case (int)Gimmicks.Wall:     //壁
                damage = true;
                break;

            case (int)Gimmicks.Balloon_Color: //風船(色)
                //色判定
                int balloonColorNum = GIMMICK_INFO_ARR[gimmickIndex][COLOR];
                Colors pieceColor = (Colors)Enum.ToObject(typeof(Colors), balloonColorNum);
                if (pieceColor.ToString() == putPieceTag) damage = true;
                break;
        }

        //結果を返す
        return damage;
    }

    /// <summary>
    /// ギミックにダメージ
    /// </summary>
    /// /// <param name="putPieceTag"> 置いた駒のタグ</param>
    /// /// <param name="gimmickIndex">ギミック管理番号(ステージ毎の)</param>
    /// /// <param name="squareIndex"> ギミック配置番号</param>
    /// /// <param name="gimmickObj">  ギミックオブジェクト</param>
    /// /// <param name="gimmickRemainingTimes"> ギミック残りダメージ回数</param>
    public void DamageGimmick(ref string putPieceTag, ref int gimmickIndex, int squareIndex, ref GameObject gimmickObj, ref int gimmickRemainingTimes)
    {
        switch (GIMMICK_INFO_ARR[gimmickIndex][GIMMICK])
        {
            //----------------------
            //風船
            //----------------------
            case (int)Gimmicks.Balloon:
                gimmickCorList.Add(StartCoroutine(DamageAnimationStart(gimmickObj, COLORLESS_ANI_STATE_NAME)));
                break;

            //----------------------
            //風船(色)
            //----------------------
            case (int)Gimmicks.Balloon_Color:
                gimmickCorList.Add(StartCoroutine(DamageAnimationStart(gimmickObj, putPieceTag)));
                break;

            //----------------------
            //壁
            //----------------------
            case (int)Gimmicks.Wall:
                int fixNum = GIMMICK_DAMAGE_TIMES[(int)Gimmicks.Wall] + 1;
                string stateName = "WallDamage" + (-gimmickRemainingTimes + fixNum).ToString();
                gimmickCorList.Add(StartCoroutine(DamageAnimationStart(gimmickObj, stateName)));
                break;
        }

        //ダメージ回数計算
        gimmickRemainingTimes--;

        //ダメージ残回数が0で破壊
        if (gimmickRemainingTimes <= 0)
            destroyPiecesIndexList.Add(squareIndex);
    }

    /// <summary>
    /// ダメージアニメーション再生
    /// </summary>
    /// <param name="obj">      破壊するオブジェクト</param>
    /// <param name="stateName">再生アニメーションステート名</param>
    IEnumerator DamageAnimationStart(GameObject obj, string stateName)
    {
        Animator ani = obj.GetComponent<Animator>();

        //アニメーション開始
        ani.Play(stateName, 0, 0.0f);
        ani.speed = 1.0f;

        //アニメーション終了待機
        yield return null;
        yield return new WaitUntil(() => ani.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        ani.speed = 0.0f;
    }
}
