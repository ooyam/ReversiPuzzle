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
        [Header("�Q�[���N���A�v���n�u")]
        [SerializeField]
        GameObject mGameClearPre;

        [Header("�Q�[���I�[�o�[�v���n�u")]
        [SerializeField]
        GameObject[] mGameOverPreArr;   //0:�^�[���񕜗L, 1:�{�^������

        [Header("�m�F�E�B���h�E�v���n�u")]
        [SerializeField]
        GameObject mConfirmWinPre;

        [Header("Result�I�u�W�F�N�g�i�[�ꏊ")]
        [SerializeField]
        RectTransform mResultTra;

        /// <summary>
        /// �Q�[���N���A
        /// </summary>
        public IEnumerator GameClear()
        {
            GameObject obj = Instantiate(mGameClearPre);
            yield return StartCoroutine(ObjectAppearance(obj));
        }

        /// <summary>
        /// �Q�[���I�[�o�[
        /// </summary>
        public IEnumerator GameOver()
        {
            GameObject obj = Instantiate(mGameOverPreArr[(TURN_RECOVERED) ? 1 : 0]);
            yield return StartCoroutine(ObjectAppearance(obj));
        }

        /// <summary>
        /// �I�u�W�F�N�g�o��
        /// </summary>
        /// <param name="_obj">�����I�u�W�F�N�g</param>
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