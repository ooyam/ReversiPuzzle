using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ObjectMove;

namespace Ui
{
    public class CanvasManager : MonoBehaviour
    {
        [Header("�t�B���^�[")]
        [SerializeField]
        Image mFilterImg;

        static readonly Color32 FILTER_ON_COLOR  = new Color32(0, 0, 0, 144);
        static readonly Color32 FILTER_OFF_COLOR = Color.clear;
        readonly Color32[] mFadeOnColorArr = new Color32[] { FILTER_OFF_COLOR, FILTER_ON_COLOR };
        readonly Color32[] mFadeOffColorArr = new Color32[] { FILTER_ON_COLOR, FILTER_OFF_COLOR };
        const float COLOR_CHANGE_SPEED = 0.3f;

        /// <summary>
        /// �t�B���^�[��������
        /// </summary>
        /// <param name="on"></param>
        public IEnumerator SetFilter(bool on)
        {
            //Color32�ɃL���X�g
            Color32 nowColor = mFilterImg.color;

            //�ڕW�A���t�@�l�ƌ��݂̃A���t�@�l�������̏ꍇ���삳���Ȃ�
            if (on)
            {
                mFilterImg.gameObject.SetActive(true);
                if (nowColor.a == FILTER_ON_COLOR.a) yield break;
                yield return StartCoroutine(ObjectMove_UI.ImagePaletteChange(mFilterImg, COLOR_CHANGE_SPEED, mFadeOnColorArr));
            }
            else
            {
                if (nowColor.a == FILTER_OFF_COLOR.a) yield break;
                yield return StartCoroutine(ObjectMove_UI.ImagePaletteChange(mFilterImg, COLOR_CHANGE_SPEED, mFadeOffColorArr));
                mFilterImg.gameObject.SetActive(false);
            }
        }
    }
}