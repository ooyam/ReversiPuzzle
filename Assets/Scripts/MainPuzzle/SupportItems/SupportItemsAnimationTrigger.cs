using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain
{
    public class SupportItemsAnimationTrigger : MonoBehaviour
    {
        SupportItemsManager mStItemsMgr;
        void Start()
        {
            mStItemsMgr = sPuzzleMain.GetSupportItemsManager();
        }

        /// <summary>
        /// �A�q���̉���
        /// </summary>
        /// <param name="_column">�����ԍ�</param>
        void DuckSupportTrigger(int _column)
        {
            mStItemsMgr.DuckSupport(_column);
        }

        /// <summary>
        /// �ԉ΂̉���
        /// </summary>
        /// <param name="_place">����ꏊ</param>
        void FireworkSupportTrigger(FireworkSupportPlace _place)
        {
            mStItemsMgr.FireworkSupport(_place);
        }

        /// <summary>
        /// ���P�b�g(��)�̉���
        /// </summary>
        /// <param name="_supportNum">����ԍ�(10�̈�:�s�w�� 1�̈�:��w��)</param>
        void RocketLineSupportTrigger(int _supportNum)
        {
            mStItemsMgr.RocketLineSupport(_supportNum);
        }

        /// <summary>
        /// ���P�b�g(�c)�̉���
        /// </summary>
        /// <param name="_supportNum">����ԍ�(10�̈�:��w�� 1�̈�:�s�w��)</param>
        void RocketColumnSupportTrigger(int _supportNum)
        {
            mStItemsMgr.RocketColumnSupport(_supportNum);
        }
    }
}
