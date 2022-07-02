using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static ObjectMove_2D.ObjectMove_2D;

namespace PuzzleMain
{
    public class SquaresManager : MonoBehaviour
    {
        [Header("リバーシ盤の取得")]
        [SerializeField]
        Transform mReversiBoardTra;

        [Header("待機駒ボックスの取得")]
        [SerializeField]
        Transform mNextPieceBoxesTra;

        Transform mNextPieceFrameTra;         //次に置くコマの指定フレーム
        SpriteRenderer[] mSquareSpriRenArr;   //マスSpriteRenderer配列

        /// <summary>
        /// PiecesManagerの初期化
        /// </summary>
        public void Initialize()
        {
            //マス取得
            sSquareObjArr = new GameObject[SQUARES_COUNT];
            sSquareTraArr = new Transform[SQUARES_COUNT];
            mSquareSpriRenArr = new SpriteRenderer[SQUARES_COUNT];
            for (int i = 0; i < SQUARES_COUNT; i++)
            {
                sSquareObjArr[i] = mReversiBoardTra.GetChild(i).gameObject;
                sSquareTraArr[i] = sSquareObjArr[i].transform;
                mSquareSpriRenArr[i] = sSquareObjArr[i].GetComponent<SpriteRenderer>();
            }

            //使用しないマスを非表示
            foreach (int i in HIDE_SQUARE_ARR)
            { sSquareObjArr[i].SetActive(false); }

            //待機駒の箱取得
            sNextPiecesCount = mNextPieceBoxesTra.childCount;
            sNextPieceBoxTraArr = new Transform[sNextPiecesCount];
            for (int i = 0; i < sNextPiecesCount; i++)
            {
                sNextPieceBoxTraArr[i] = mNextPieceBoxesTra.GetChild(i).transform;
            }

            //次に置くコマの指定フレーム取得
            mNextPieceFrameTra = sNextPieceBoxTraArr[0].GetChild(0).gameObject.transform;
        }

        /// <summary>
        /// マスの色変更
        /// </summary>
        /// <param name="_afterColor"> 変化後の色</param>
        /// <param name="_squareId">   マス管理番号</param>
        /// <param name="_fade">       フェードの有無</param>
        public IEnumerator SquareColorChange(Color _afterColor, int _squareId, bool _fade)
        {
            if (!_fade) mSquareSpriRenArr[_squareId].color = _afterColor;
            else yield return StartCoroutine(SpriteRendererPaletteChange(mSquareSpriRenArr[_squareId], SQUARE_CHANGE_SPEED, new Color[] { mSquareSpriRenArr[_squareId].color, _afterColor }));
        }

        /// <summary>
        /// 次投擲駒の指定フレーム移動
        /// </summary>
        /// <param name="_nextPuPieceIndex">次投擲駒番号</param>
        /// <returns></returns>v
        public void MoveNextPieceFrame(int _nextPuPieceIndex)
        {
            //移動
            mNextPieceFrameTra.SetParent(sNextPieceBoxTraArr[_nextPuPieceIndex], false);
        }

        /// <summary>
        /// 処置回数の設定
        /// </summary>
        /// <param name="directions">方向の管理番号</param>
        /// <param name="up">        上にあるマスの数</param>
        /// <param name="right">     右にあるマスの数</param>
        /// <param name="down">      下にあるマスの数</param>
        /// <param name="left">      左にあるマスの数</param>
        /// <returns>処理回数</returns>
        public int SetLoopCount(Directions directions, ref int up, ref int right, ref int down, ref int left)
        {
            return directions switch
            {
                Directions.Up           => up,                              //上
                Directions.UpRight      => (up <= right) ? up : right,      //右上
                Directions.Right        => right,                           //右
                Directions.DownRight    => (down <= right) ? down : right,  //右下
                Directions.Down         => down,                            //下
                Directions.DownLeft     => (down <= left) ? down : left,    //左下
                Directions.Left         => left,                            //左
                Directions.UpLeft       => (up <= left) ? up : left,        //左上
                _ => 0, //default
            };
        }

        /// <summary>
        /// 指定方向にマスがあるか？
        /// </summary>
        /// <param name="direction">方向</param>
        /// <param name="baseIndex">基準のマス管理番号</param>
        /// <returns>指定場所の管理番号</returns>
        public bool IsSquareSpecifiedDirection(Directions direction, int baseIndex)
        {
            switch (direction)
            {
                //上
                case Directions.Up:
                case Directions.UpLeft:
                case Directions.UpRight:
                    if (baseIndex % BOARD_LINE_COUNT == 0) return false;
                    break;
                //下
                case Directions.Down:
                case Directions.DownLeft:
                case Directions.DownRight:
                    if ((baseIndex + 1) % BOARD_LINE_COUNT == 0) return false;
                    break;
            }
            switch (direction)
            {
                //左
                case Directions.Left:
                case Directions.UpLeft:
                case Directions.DownLeft:
                    if (baseIndex < BOARD_LINE_COUNT) return false;
                    break;
                //右
                case Directions.Right:
                case Directions.UpRight:
                case Directions.DownRight:
                    if (baseIndex >= SQUARES_COUNT - BOARD_LINE_COUNT) return false;
                    break;
            }

            //マスがある
            return true;
        }

        /// <summary>
        /// 指定場所の管理番号取得
        /// </summary>
        /// <param name="direction">方向の管理番号</param>
        /// <param name="baseIndex">基準オブジェクトの管理番号</param>
        /// <param name="distance"> 距離</param>
        /// <returns>指定場所の管理番号</returns>
        public int GetDesignatedDirectionIndex(int direction, int baseIndex, int distance = 1)
        {
            return direction switch
            {
                (int)Directions.Up          => baseIndex - distance,                                //上
                (int)Directions.UpRight     => baseIndex + BOARD_LINE_COUNT * distance - distance,  //右上
                (int)Directions.Right       => baseIndex + BOARD_LINE_COUNT * distance,             //右
                (int)Directions.DownRight   => baseIndex + BOARD_LINE_COUNT * distance + distance,  //右下
                (int)Directions.Down        => baseIndex + distance,                                //下
                (int)Directions.DownLeft    => baseIndex - BOARD_LINE_COUNT * distance + distance,  //左下
                (int)Directions.Left        => baseIndex - BOARD_LINE_COUNT * distance,             //左
                (int)Directions.UpLeft      => baseIndex - BOARD_LINE_COUNT * distance - distance,  //左上
                _ => INT_NULL,  //default
            };
        }
    }
}