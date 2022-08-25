using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Preferences;
using static CommonDefine;
using static Title.TitleMain;
using static Sound.SoundManager;
using static animation.AnimationManager;

namespace Option
{
    public class OptionManager : MonoBehaviour
    {
        //�I�v�V�����^�C�v
        enum OptionType
        {
            Title,
            Puzzle,

            Count
        }
        OptionType mType;

        //�I�v�V�������
        enum OptionState
        {
            None,       //�ʏ�
            Credit,     //�N���W�b�g
            Confirm,    //�m�F�E�B���h�E

            Count       //����
        }
        OptionState mState = OptionState.None;

        [Header("�I�v�V�������")]
        [SerializeField]
        GameObject[] mOptionObjArr;  //0:�ʏ�, 1:�N���W�b�g, 2:�m�F�E�B���h�E

        [Header("BGM�ؑփ{�^��Image")]
        [SerializeField]
        Image mBgmBtnImg;

        [Header("SE�ؑփ{�^��Image")]
        [SerializeField]
        Image mSeBtnImg;

        [Header("�؂�ւ��{�^���X�v���C�g")]
        [SerializeField]
        Sprite[] mSwitchBtnSpr;

        //�X�v���C�g�^�C�v
        enum SwitchBtnType
        { On, Off }


        /// <summary>
        /// ������
        /// </summary>
        public void Initialize()
        {
            //�^�C�v�ݒ�
            switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                case TITLE_SCENE_NAME:  mType = OptionType.Title;  break;
                case PUZZLE_SCENE_NAME: mType = OptionType.Puzzle; break;
            }
        }


        //==========================================================//
        //-----------------------�{�^������-------------------------//
        //==========================================================//

        /// <summary>
        /// �I�v�V����
        /// </summary>
        public void IsPushOption()
        {
            //�t�B���^�[�\��
            StartCoroutine(CanvasMgr.SetFilter(true));

            //SE�Đ�
            SE_Onshot(SE_Type.BtnYes);
            StartCoroutine(ObjectAppearance(OptionState.None));
        }

        /// <summary>
        /// BGM_ON�EOFF
        /// </summary>
        public void IsPushBGM()
        {
            //BGM�ؑ�
            Bgm = !Bgm;
            mBgmBtnImg.sprite = Bgm ? mSwitchBtnSpr[(int)SwitchBtnType.On] : mSwitchBtnSpr[(int)SwitchBtnType.Off];

            //�ؑ�SE
            SE_Onshot(Bgm ? SE_Type.BtnYes : SE_Type.BtnNo);
        }

        /// <summary>
        /// SE_ON�EOFF
        /// </summary>
        public void IsPushSE()
        {
            //SE�ؑ�
            Se = !Se;
            mSeBtnImg.sprite = Se ? mSwitchBtnSpr[(int)SwitchBtnType.On] : mSwitchBtnSpr[(int)SwitchBtnType.Off];

            //�ؑ�SE
            if (Se) SE_Onshot(SE_Type.BtnYes);
        }

        /// <summary>
        /// �N���W�b�g
        /// </summary>
        public void IsPushCredit()
        {
            //SE�Đ�
            SE_Onshot(SE_Type.BtnYes);
            StartCoroutine(ObjectAppearance(OptionState.Credit));
        }

        /// <summary>
        /// �Q�[������߂�
        /// </summary>
        public void IsPushQuitGame()
        {
            //SE�Đ�
            SE_Onshot(SE_Type.BtnYes);
            StartCoroutine(ObjectAppearance(OptionState.Confirm, false));
        }

        /// <summary>
        /// �͂�
        /// </summary>
        public void IsPushYes()
        {
            //�Q�[���I��
            SE_Onshot(SE_Type.BtnYes);
            GameManager.QuitGame();
        }

        /// <summary>
        /// ������
        /// </summary>
        public void IsPushNo()
        {
            //SE�Đ�
            SE_Onshot(SE_Type.BtnYes);
            StartCoroutine(ObjectAppearance(OptionState.None));
        }

        /// <summary>
        /// ����
        /// </summary>
        public void IsPushClose()
        {
            switch (mState)
            {
                //�ʏ�\��
                case OptionState.None:
                    StartCoroutine(CanvasMgr.SetFilter(false)); //�t�B���^�[����
                    ObjInactive();  //�I�v�V�����I��
                    break;

                //�N���W�b�g�\��
                case OptionState.Credit:
                    StartCoroutine(ObjectAppearance(OptionState.None)); //�ʏ��Ԃ�
                    break;

                //���邱�Ƃ͂Ȃ��͂�
                default: return;
            }

            //SE�Đ�
            SE_Onshot(SE_Type.BtnYes);
        }


        //==========================================================//
        //---------------------�I�u�W�F�N�g����---------------------//
        //==========================================================//

        /// <summary>
        /// �w���ʂ̏o��
        /// </summary>
        /// <param name="_anime">�o���A�j���̗L��</param>
        IEnumerator ObjectAppearance(OptionState _state, bool _anime = true)
        {
            //��Ԑݒ�
            mState = _state;

            //��ʃA�N�e�B�u��Ԑؑ�
            ObjInactive();
            mOptionObjArr[(int)mState].SetActive(true);

            //�o���A�j���Đ�
            if (_anime)
            {
                Animator ani = mOptionObjArr[(int)mState].GetComponent<Animator>();
                yield return StartCoroutine(AnimationStart(ani, STATE_NAME_APPEARANCE));
            }
        }

        /// <summary>z
        /// �S��ʔ�A�N�e�B�u
        /// </summary>
        void ObjInactive()
        {
            foreach (GameObject obj in mOptionObjArr)
            { obj.SetActive(false); }
        }
    }
}