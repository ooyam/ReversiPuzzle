using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static animation.AnimationManager;

namespace PuzzleMain
{
    public class ResultManager : MonoBehaviour
    {
        [Header("ゲームクリアプレハブ")]
        [SerializeField]
        GameObject mGameClearPre;

        [Header("ゲームオーバープレハブ")]
        [SerializeField]
        GameObject[] mGameOverPreArr;   //0:ターン回復有, 1:ボタン無し

        [Header("確認ウィンドウプレハブ")]
        [SerializeField]
        GameObject mConfirmWinPre;

        [Header("Resultオブジェクト格納場所")]
        [SerializeField]
        RectTransform mResultTra;

        /// <summary>
        /// ゲームクリア
        /// </summary>
        public IEnumerator GameClear()
        {
            GameObject obj = Instantiate(mGameClearPre);
            yield return StartCoroutine(ObjectAppearance(obj));
        }

        /// <summary>
        /// ゲームオーバー
        /// </summary>
        public IEnumerator GameOver()
        {
            GameObject obj = Instantiate(mGameOverPreArr[(TURN_RECOVERED) ? 1 : 0]);
            yield return StartCoroutine(ObjectAppearance(obj));
        }

        /// <summary>
        /// オブジェクト出現
        /// </summary>
        /// <param name="_obj">生成オブジェクト</param>
        IEnumerator ObjectAppearance(GameObject _obj)
        {
            RectTransform tra = _obj.GetComponent<RectTransform>();
            tra.SetParent(mResultTra);
            tra.localPosition = Vector3.zero;
            tra.localScale = Vector3.one;
            Animator ani = _obj.GetComponent<Animator>();
            yield return StartCoroutine(AnimationStart(ani, STATE_NAME_APPEARANCE));
        }
    }
}