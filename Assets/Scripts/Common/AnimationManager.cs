using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace animation
{
    public class AnimationManager : MonoBehaviour
    {
        //�A�j���[�V�����X�e�[�g��
        public const string STATE_NAME_EMPTY        = "Empty";          //�������
        public const string STATE_NAME_WAIT         = "Wait";           //�ҋ@
        public const string STATE_NAME_DAMAGE       = "Damage";         //������_���[�W
        public const string STATE_NAME_BURST        = "Burst";          //�j��
        public const string STATE_NAME_COLOR_CHANGE = "ColorChange";    //�F�̍X�V
        public const string STATE_NAME_RETURN       = "Return";         //��Ԃ�߂�
        public const string STATE_NAME_ATTACK       = "Attack";         //�U��
        public const string STATE_NAME_SUPPORT      = "Support";        //����
        public const string STATE_NAME_READY        = "Ready";          //����
        public const string STATE_NAME_ACTIVE       = "Active";         //�A�N�e�B�u
        public const string STATE_NAME_INACTIVE     = "Inactive";       //��A�N�e�B�u
        public const string STATE_NAME_APPEARANCE   = "Appearance";     //�o��

        //===============================================//
        //===========�A�j���[�V��������֐�==============//
        //===============================================//

        /// <summary>
        /// �A�j���[�V�����Đ�
        /// </summary>
        /// <param name="ani">      �j�󂷂�I�u�W�F�N�g��Animator</param>
        /// <param name="stateName">�Đ��A�j���[�V�����X�e�[�g��</param>
        public static IEnumerator AnimationStart(Animator ani, string stateName)
        {
            //�A�j���[�V�����J�n
            ani.Play(stateName, 0, 0.0f);

            //state���؂�ւ��܂őҋ@
            yield return new WaitUntil(() => ani.GetCurrentAnimatorStateInfo(0).IsName(stateName));

            //�A�j���[�V�����I���ҋ@
            yield return new WaitWhile(() => ani.GetCurrentAnimatorStateInfo(0).IsName(stateName));
        }

        /// <summary>
        /// ���[�v�A�j���[�V�����Đ�
        /// </summary>
        /// <param name="ani">      �j�󂷂�I�u�W�F�N�g��Animator</param>
        /// <param name="stateName">�Đ��A�j���[�V�����X�e�[�g��</param>
        public static void LoopAnimationStart(Animator ani, string stateName = STATE_NAME_EMPTY)
        {
            //�A�j���[�V�����J�n
            ani.Play(stateName, 0, 0.0f);
        }
    }
}