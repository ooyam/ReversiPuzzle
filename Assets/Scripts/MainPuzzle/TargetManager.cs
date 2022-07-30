using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain
{
    public class TargetManager : MonoBehaviour
    {
        [Header("目標表示ボックスの取得")]
        [SerializeField]
        RectTransform mTexturesBoxTra;

        [Header("目標表示用ギミックプレハブの取得")]
        public TargetGimmicksArr[] mTargetGimmicksPrefabArr;
        [Serializable]
        public class TargetGimmicksArr
        { public GameObject[] prefab; }

        [Header("目標表示用駒プレハブの取得")]
        public GameObject[] mTargetPiecesPrefabArr;

        Text[] mCountTextsArr;  //残り数量の表示Text

        const int TARGET_MAX = 5;           //目標最大数
        const int TARGET_COUNT_MAX = 999;   //目標k個数最大数

        //==========================================================//
        //----------------------初期設定,取得-----------------------//
        //==========================================================//

        /// <summary>
        /// TargetManagerの初期化
        /// </summary>
        public void Initialize()
        {
            //表示間隔の設定
            float winWidth = mTexturesBoxTra.rect.width;
            float span = winWidth / TARGET_MAX;
            float posX = (TARGETS_COUNT + 1) / 2.0f * -span;

            //目標の表示
            RectTransform[] targetTraArr = new RectTransform[TARGETS_COUNT];
            mCountTextsArr = new Text[TARGETS_COUNT];
            for (int i = 0; i < TARGETS_COUNT; i++)
            {
                //目標テクスチャーの生成
                GameObject obj;
                if (TARGETS_INFO_ARR[i][TARGET_INFO_OBJ] < 0)
                {
                    //駒
                    obj = Instantiate(mTargetPiecesPrefabArr[TARGETS_INFO_ARR[i][TARGET_INFO_COLOR]]);
                }
                else
                {
                    //ギミック
                    int colorId = (TARGETS_INFO_ARR[i][TARGET_INFO_COLOR] < 0) ? 0 : TARGETS_INFO_ARR[i][TARGET_INFO_COLOR];
                    obj = Instantiate(mTargetGimmicksPrefabArr[TARGETS_INFO_ARR[i][TARGET_INFO_OBJ]].prefab[colorId]);
                }

                //情報取得
                targetTraArr[i] = obj.GetComponent<RectTransform>();
                mCountTextsArr[i] = targetTraArr[i].GetChild(0).GetComponent<Text>();

                //表示
                posX += span;
                targetTraArr[i].SetParent(mTexturesBoxTra);
                targetTraArr[i].localPosition = new Vector2(posX, 0.0f);
                int count = TARGETS_INFO_ARR[i][TARGET_INFO_COUNT];
                if (count > TARGET_COUNT_MAX) count = TARGET_COUNT_MAX;
                mCountTextsArr[i].text = count.ToString();
            }
        }

        /// <summary>
        /// 目標の減少確認
        /// </summary>
        /// <param name="_colorId">駒色ID</param>
        /// <param name="_gimmickId">ギミックID</param>
        public void TargetDecreaseCheck(int _colorId, int _gimmickId = INT_NULL)
        {
            for (int i = 0; i < TARGETS_COUNT; i++)
            {
                if (_gimmickId == INT_NULL)
                {
                    //駒
                    if (TARGETS_INFO_ARR[i][TARGET_INFO_OBJ] < 0 &&
                        TARGETS_INFO_ARR[i][TARGET_INFO_COLOR] == _colorId)
                    {
                        TargetDecrease(i);
                    }
                }
                else
                {
                    //ギミック
                    if (TARGETS_INFO_ARR[i][TARGET_INFO_OBJ] == _gimmickId)
                    {
                        TargetDecrease(i);
                    }
                }
            }
        }

        /// <summary>
        /// 目標減少
        /// </summary>
        /// <param name="_targetIndex">目標管理番号</param>
        void TargetDecrease(int _targetIndex)
        {
            int count = int.Parse(mCountTextsArr[_targetIndex].text) - 1;
            if (count < 0) count = 0;
            mCountTextsArr[_targetIndex].text = count.ToString();
        }

        /// <summary>
        /// 目標確認
        /// </summary>
        public void TargetCheck()
        {
            //すべての目標残数確認
            foreach (Text countText in mCountTextsArr)
            {
                if (int.Parse(countText.text) > 0)
                {
                    //まだ残っている
                    return;
                }
            }

            //すべて0になった
            sPuzzleMain.GameClear();
        }
    }
}
