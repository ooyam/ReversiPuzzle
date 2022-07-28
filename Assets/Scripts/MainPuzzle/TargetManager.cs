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

        RectTransform[] mTargetTraArr;
        Text[] mCountTextsArr;  //残り数量の表示Text

        const float TARGET_COUNT_MAX = 5; //目標最大数

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
            float span = winWidth / TARGET_COUNT_MAX;
            float posX = (TARGETS_COUNT + 1) / 2.0f * -span;

            //目標の表示
            mTargetTraArr = new RectTransform[TARGETS_COUNT];
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
                mTargetTraArr[i] = obj.GetComponent<RectTransform>();
                mCountTextsArr[i] = mTargetTraArr[i].GetChild(0).GetComponent<Text>();

                //表示
                posX += span;
                mTargetTraArr[i].SetParent(mTexturesBoxTra);
                mTargetTraArr[i].localPosition = new Vector2(posX, 0.0f);
                mCountTextsArr[i].text = TARGETS_INFO_ARR[i][TARGET_INFO_COUNT].ToString();
            }
        }


    }
}
