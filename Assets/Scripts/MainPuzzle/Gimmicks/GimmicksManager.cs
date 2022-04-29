using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PieceManager;
using static ObjectMove_2D.ObjectMove_2D;
using static GimmickInformation;

public class GimmicksManager : MonoBehaviour
{
    [Header("宝石のsprite")]
    [SerializeField]
    Sprite[] jewelrySpr;

    //アニメーションステート名
    const string STATE_NAME_EMPTY        = "Empty";       //初期状態
    const string STATE_NAME_WAIT         = "Wait";        //待機
    const string STATE_NAME_COLORLESS    = "Colorless";   //色指定無し
    const string STATE_NAME_BURST        = "Burst";       //色指定有り
    const string STATE_NAME_DAMAGE       = "Damage";      //複数回ダメージ
    const string STATE_NAME_RETURN_STATE = "ReturnState"; //状態を戻す

    void Awake()
    {
        //ギミックの設定読み込み
        GimmickSetting();
        StageSetting();
    }

    /// <summary>
    /// ギミックにダメージがあるかの確認
    /// </summary>
    /// /// <param name="putPieceTag"> 置いた駒のタグ</param>
    /// /// <param name="gimmickIndex">ギミック管理番号(ステージ毎の)</param>
    public bool DamageCheck(ref string putPieceTag, ref int gimmickIndex)
    {
        //ダメージの有無フラグ
        bool damage = false;

        switch (gimmickInfoArr[gimmickIndex].id)
        {
            case (int)Gimmicks.Balloon:  //風船
            case (int)Gimmicks.Wall:     //壁
            case (int)Gimmicks.Flower:   //花
            case (int)Gimmicks.Hamster:  //ハムスター
                damage = true;
                break;

            case (int)Gimmicks.Balloon_Color: //風船(色)
            case (int)Gimmicks.Jewelry:       //宝石
                //色判定
                int colorIndex = gimmickInfoArr[gimmickIndex].colorNum;
                Colors gimmickColor = (Colors)Enum.ToObject(typeof(Colors), colorIndex);
                if (gimmickColor.ToString() == putPieceTag) damage = true;
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
    public void DamageGimmick(ref string putPieceTag, ref int gimmickIndex, int squareIndex)
    {
        int damageTimesfixNum = 0; //ステート名算出用
        string stateName = "";     //ステート名
        GimmickInformation gimmInfo = gimmickInfoArr[gimmickIndex]; //ギミックの情報取得

        switch (gimmInfo.id)
        {
            //無条件
            case (int)Gimmicks.Balloon: //風船
                stateName = STATE_NAME_COLORLESS;
                break;

            //色指定
            case (int)Gimmicks.Balloon_Color: //風船(色)
            case (int)Gimmicks.Jewelry:       //宝石
                stateName = STATE_NAME_BURST + putPieceTag;
                break;

            //複数回ダメージ
            case (int)Gimmicks.Wall:    //壁
            case (int)Gimmicks.Flower:  //花
            case (int)Gimmicks.Hamster: //ハムスター
                damageTimesfixNum = GIMMICK_DAMAGE_TIMES[gimmInfo.id] + 1;
                stateName = STATE_NAME_DAMAGE + (-gimmInfo.remainingTimes + damageTimesfixNum).ToString();
                if (!gimmInfo.destructible) gimmInfo.destructible = true;
                break;
        }

        //ダメージアニメーション開始
        gimmickCorList.Add(StartCoroutine(AnimationStart(gimmInfo.ani, stateName)));

        //ダメージ回数計算
        gimmInfo.remainingTimes--;

        //今のターンにダメージを受けたかのフラグON
        gimmInfo.nowTurnDamage = true;

        //ダメージ残回数が0で破壊
        if (gimmInfo.remainingTimes <= 0)
            destroyPiecesIndexList.Add(squareIndex);
    }

    /// <summary>
    /// ギミックの状態変化
    /// </summary>
    /// <param name="gimmickIndex">ギミック管理番号(ステージ毎の)</param>
    /// <returns></returns>
    public IEnumerator ChangeGimmickState(int gimmickIndex)
    {
        //ギミックの情報取得
        GimmickInformation gimmInfo = gimmickInfoArr[gimmickIndex];

        switch (gimmInfo.id)
        {
            //宝石(sprite切替)
            case (int)Gimmicks.Jewelry:

                //子オブジェクトのsprit更新
                SpriteRenderer spriRenChild = gimmInfo.tra.GetChild(0).GetComponent<SpriteRenderer>();
                gimmInfo.colorNum++;
                if (gimmInfo.colorNum >= USE_PIECE_COUNT) gimmInfo.colorNum = 0;
                Sprite newSprite = jewelrySpr[gimmInfo.colorNum];

                //切り替え後のspriteを子オブジェクトに設定
                gimmInfo.ani.enabled = false;
                spriRenChild.sprite = newSprite;

                //色変更
                yield return StartCoroutine(SpriteRendererPaletteChange(gimmInfo.spriRen, JEWELRY_CHANGE_SPEED, COLOR_FADE_OUT, COLOR_FADE_COMPARE_INDEX));
                gimmInfo.spriRen.sprite = newSprite;
                gimmInfo.spriRen.color = COLOR_PRIMARY;
                gimmInfo.ani.enabled = true;
                Colors colorEnum = (Colors)Enum.ToObject(typeof(Colors), gimmInfo.colorNum);
                LoopAnimationStart(gimmInfo.ani, STATE_NAME_WAIT + colorEnum.ToString());
                break;

            //ハムスター(連続フラグ確認)
            case (int)Gimmicks.Hamster:

                //ダメージ1状態
                if (gimmInfo.destructible)
                {
                    //このターンにダメージを受けた
                    if (gimmInfo.nowTurnDamage)
                    {
                        //ダメージ1待機状態
                        LoopAnimationStart(gimmInfo.ani, STATE_NAME_WAIT);
                    }
                    else
                    {
                        //初期状態に戻す
                        gimmInfo.destructible = false;
                        gimmInfo.remainingTimes++;
                        yield return StartCoroutine(AnimationStart(gimmInfo.ani, STATE_NAME_RETURN_STATE));
                        LoopAnimationStart(gimmInfo.ani);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// アニメーション再生
    /// </summary>
    /// <param name="ani">      破壊するオブジェクトのAnimator</param>
    /// <param name="stateName">再生アニメーションステート名</param>
    IEnumerator AnimationStart(Animator ani, string stateName)
    {
        //アニメーション開始
        ani.Play(stateName, 0, 0.0f);
        //ani.speed = 1.0f;

        //アニメーション終了待機
        yield return null;
        yield return new WaitUntil(() => ani.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f);
        //ani.speed = 0.0f;
    }

    /// <summary>
    /// ループアニメーション再生
    /// </summary>
    /// <param name="ani">      破壊するオブジェクトのAnimator</param>
    /// <param name="stateName">再生アニメーションステート名</param>
    void LoopAnimationStart(Animator ani, string stateName = STATE_NAME_EMPTY)
    {
        //アニメーション開始
        ani.Play(stateName, 0, 0.0f);
        //ani.speed = 1.0f;
    }
}
