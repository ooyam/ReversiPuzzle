using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;
using static Sound.SoundManager;

namespace PuzzleMain
{
    public class SupportItemsAnimationTrigger : MonoBehaviour
    {
        /// <summary>
        /// �A�q���̉���
        /// </summary>
        /// <param name="_column">�����ԍ�</param>
        void DuckSupportTrigger(int _column)
        {
            SupportItemsMgr.DuckSupport(_column);
        }

        /// <summary>
        /// �ԉ΂̉���
        /// </summary>
        /// <param name="_place">����ꏊ</param>
        void FireworkSupportTrigger(FireworkSupportPlace _place)
        {
            SupportItemsMgr.FireworkSupport(_place);
        }

        /// <summary>
        /// ���P�b�g(��)�̉���
        /// </summary>
        /// <param name="_supportNum">����ԍ�(10�̈�:�s�w�� 1�̈�:��w��)</param>
        void RocketLineSupportTrigger(int _supportNum)
        {
            SupportItemsMgr.RocketLineSupport(_supportNum);
        }

        /// <summary>
        /// ���P�b�g(�c)�̉���
        /// </summary>
        /// <param name="_supportNum">����ԍ�(10�̈�:��w�� 1�̈�:�s�w��)</param>
        void RocketColumnSupportTrigger(int _supportNum)
        {
            SupportItemsMgr.RocketColumnSupport(_supportNum);
        }

        /// <summary>
        /// SE�Đ�
        /// </summary>
        void SupportItemSePlay(SE_Type _seType)
        {
            switch (_seType)
            {
                //�A���Đ�
                case SE_Type.Duck:
                    SE_ContinuousPlay(_seType);
                    break;

                //�P���Đ�
                default:
                    SE_OneShot(_seType);
                    break;
            }
        }
    }
}
