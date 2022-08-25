using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ObjectMove_UI.ObjectMove_UI;

namespace Ui
{
    public class CanvasManager : MonoBehaviour
    {
        [Header("フィルター")]
        [SerializeField]
        Image mFilterImg;

        readonly Color32 FILTER_ON_COLOR  = new Color32(0, 0, 0, 144);
        readonly Color32 FILTER_OFF_COLOR = Color.clear;
        Color32[] mFadeOnColorArr;
        Color32[] mFadeOffColorArr;
        const float COLOR_CHANGE_SPEED = 0.3f;

        private void Start()
        {
            mFadeOnColorArr  = new Color32[] { FILTER_OFF_COLOR, FILTER_ON_COLOR };
            mFadeOffColorArr = new Color32[] { FILTER_ON_COLOR, FILTER_OFF_COLOR };
        }

        /// <summary>
        /// フィルターをかける
        /// </summary>
        /// <param name="on"></param>
        public IEnumerator SetFilter(bool on)
        {
            //Color32にキャスト
            Color32 nowColor = mFilterImg.color;

            //目標アルファ値と現在のアルファ値が同じの場合動作させない
            if (on)
            {
                mFilterImg.gameObject.SetActive(true);
                if (nowColor.a == FILTER_ON_COLOR.a) yield break;
                yield return StartCoroutine(ImagePaletteChange(mFilterImg, COLOR_CHANGE_SPEED, mFadeOnColorArr));
            }
            else
            {
                if (nowColor.a == FILTER_OFF_COLOR.a) yield break;
                yield return StartCoroutine(ImagePaletteChange(mFilterImg, COLOR_CHANGE_SPEED, mFadeOffColorArr));
                mFilterImg.gameObject.SetActive(false);
            }
        }
    }
}