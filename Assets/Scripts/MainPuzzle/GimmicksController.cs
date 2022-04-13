using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static PuzzleDefine;
using static PieceManager;

public class GimmicksController : MonoBehaviour
{
    public static GimmicksController sGimmicksController;
    void Awake()
    {
        if (sGimmicksController == null)
        {
            sGimmicksController = this;
        }
    }

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
            //----------------------
            //風船
            //----------------------
            case (int)Gimmicks.Balloon:
                damage = true;
                break;

            //----------------------
            //風船(色)
            //----------------------
            case (int)Gimmicks.Balloon_Color:
                //色判定
                int balloonColorNum = GIMMICK_INFO_ARR[gimmickIndex][COLOR];
                PieceColors pieceColor = (PieceColors)Enum.ToObject(typeof(PieceColors), balloonColorNum);
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
    public void DamageGimmick(ref string putPieceTag, ref int gimmickIndex, int squareIndex, ref GameObject gimmickObj)
    {
        switch (GIMMICK_INFO_ARR[gimmickIndex][GIMMICK])
        {
            //----------------------
            //風船
            //----------------------
            case (int)Gimmicks.Balloon:
                destroyPiecesIndexList.Add(squareIndex); //削除対象に反転駒の管理番号追加
                gimmickCorList.Add(StartCoroutine(BalloonBreak(gimmickObj)));
                break;

            //----------------------
            //風船(色)
            //----------------------
            case (int)Gimmicks.Balloon_Color:
                destroyPiecesIndexList.Add(squareIndex);
                gimmickCorList.Add(StartCoroutine(BalloonColorBreak(gimmickObj, putPieceTag)));
                break;
        }
    }


    /// <summary>
    /// 風船破壊
    /// </summary>
    /// <param name="obj">破壊するオブジェクト</param>
    IEnumerator BalloonBreak(GameObject obj)
    {
        Animator ani = obj.GetComponent<Animator>();
        string stateName = "Black";

        //アニメーション開始
        ani.Play(stateName, 0, 0.0f);

        //アニメーション終了待機
        yield return null;
        yield return new WaitUntil(() => ani.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        ani.speed = 0.0f;
    }

    /// <summary>
    /// 風船(色)破壊
    /// </summary>
    /// <param name="obj">      破壊するオブジェクト</param>
    /// <param name="colorName">色の名称(ステージ名)</param>
    /// <returns></returns>
    IEnumerator BalloonColorBreak(GameObject obj, string colorName)
    {
        Animator ani = obj.GetComponent<Animator>();

        //アニメーション開始
        ani.Play(colorName, 0, 0.0f);

        //アニメーション終了待機
        yield return null;
        yield return new WaitUntil(() => ani.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        ani.speed = 0.0f;
    }
}
