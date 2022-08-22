using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static CommonDefine;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static ObjectMove_2D.ObjectMove_2D;
using static Sound.SoundManager;

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
            {
                if (i < 0 || i >= SQUARES_COUNT) continue;
                sSquareObjArr[i].SetActive(false);
            }

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
        /// 列番号取得
        /// </summary>
        /// <param name="_squareId">マス管理番号</param>
        /// <returns></returns>
        public int GetColumnNumber(int _squareId)
        {
            return _squareId / BOARD_LINE_COUNT;
        }

        /// <summary>
        /// 行番号取得
        /// </summary>
        /// <param name="_squareId">マス管理番号</param>
        /// <returns></returns>
        public int GetLineNumber(int _squareId)
        {
            return _squareId % BOARD_LINE_COUNT;
        }

        /// <summary>
        /// マスが存在するか？
        /// </summary>
        /// <param name="_squareId">マス管理番号</param>
        /// <returns></returns>
        public bool IsSquareExists(int _squareId)
        {
            if (0 <= _squareId && _squareId < SQUARES_COUNT)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// マスはアクティブか？
        /// </summary>
        /// <param name="_squareId">マス管理番号</param>
        /// <returns></returns>
        public bool IsSquareActive(int _squareId)
        {
            if (IsSquareExists(_squareId))
            {
                return sSquareObjArr[_squareId].activeSelf;
            }
            return false;
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
        /// オブジェクトがあるマス管理番号を取得
        /// </summary>
        /// <param name="_obj"></param>
        /// <returns></returns>
        public int GetSquareId(GameObject _obj)
        {
            return Array.IndexOf(sPieceObjArr, _obj);
        }

        /// <summary>
        /// 次投擲駒の指定フレーム移動
        /// </summary>
        /// <param name="_nextPuPieceIndex">次投擲駒番号</param>
        /// <returns></returns>v
        public void MoveNextPieceFrame(int _nextPuPieceIndex)
        {
            //SE再生
            SE_Onshot(SE_Type.PieceSelect);

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
                Directions.Up        => up,                              //上
                Directions.UpRight   => (up <= right) ? up : right,      //右上
                Directions.Right     => right,                           //右
                Directions.DownRight => (down <= right) ? down : right,  //右下
                Directions.Down      => down,                            //下
                Directions.DownLeft  => (down <= left) ? down : left,    //左下
                Directions.Left      => left,                            //左
                Directions.UpLeft    => (up <= left) ? up : left,        //左上
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
            //距離が0以下の場合はマス無し判定
            if (distance <= 0) return INT_NULL;

            bool difDir = false;    //方向ズレフラグ
            int square = INT_NULL;  //指定場所マス番号

            //方向ずれ確認用変数
            int baseLine = GetLineNumber(baseIndex);
            int baseColumn = GetColumnNumber(baseIndex);
            int offsetLine;
            int offsetColumn;

            switch (direction)
            {
                //上
                case (int)Directions.Up:
                    square = baseIndex - distance;
                    difDir = GetColumnNumber(baseIndex) != GetColumnNumber(square);
                    break;

                //右上
                case (int)Directions.UpRight:
                    square = baseIndex + BOARD_LINE_COUNT * distance - distance;
                    offsetLine = GetLineNumber(square) - baseLine;
                    offsetColumn = GetColumnNumber(square) - baseColumn;
                    difDir = offsetLine >= 0 || offsetColumn <= 0 || -offsetLine != offsetColumn;
                    break;

                //右
                case (int)Directions.Right:
                    square = baseIndex + BOARD_LINE_COUNT * distance;
                    break;

                //右下
                case (int)Directions.DownRight:
                    square = baseIndex + BOARD_LINE_COUNT * distance + distance;
                    offsetLine = GetLineNumber(square) - baseLine;
                    offsetColumn = GetColumnNumber(square) - baseColumn;
                    difDir = offsetLine <= 0 || offsetColumn <= 0 || offsetLine != offsetColumn;
                    break;

                //下
                case (int)Directions.Down:
                    square = baseIndex + distance;
                    difDir = GetColumnNumber(baseIndex) != GetColumnNumber(square);
                    break;

                //左下
                case (int)Directions.DownLeft:
                    square = baseIndex - BOARD_LINE_COUNT * distance + distance;
                    offsetLine = GetLineNumber(square) - baseLine;
                    offsetColumn = GetColumnNumber(square) - baseColumn;
                    difDir = offsetLine <= 0 || offsetColumn >= 0 || offsetLine != -offsetColumn;
                    break;

                //左
                case (int)Directions.Left:
                    square = baseIndex - BOARD_LINE_COUNT * distance;
                    break;

                //左上
                case (int)Directions.UpLeft:
                    square = baseIndex - BOARD_LINE_COUNT * distance - distance;
                    offsetLine = GetLineNumber(square) - baseLine;
                    offsetColumn = GetColumnNumber(square) - baseColumn;
                    difDir = offsetLine >= 0 || offsetColumn >= 0 || offsetLine != offsetColumn;
                    break;
            }

            //方向ズレもしくはマスが存在しない場合
            if (difDir || !IsSquareExists(square)) square = INT_NULL;

            //マス番号を返す
            return square;
        }
    }
}