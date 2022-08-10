using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CommonDefine;
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

        [Header("�����[�h�L���v���n�u")]
        [SerializeField]
        GameObject mAdRewardPre;

        [Header("���b�Z�[�W�E�B���h�E")]
        [SerializeField]
        GameObject mMessageWinPre;
        const string BUTTON_TEXT_CLOSE = "����";
        const string BUTTON_TEXT_CANCEL = "�L�����Z��";
        readonly Dictionary<string, float> BUTTON_WIDTH = new Dictionary<string, float>()
        {
            {BUTTON_TEXT_CLOSE,  350.0f },
            {BUTTON_TEXT_CANCEL, 450.0f }
        };

        [Header("Result�I�u�W�F�N�g�i�[�ꏊ")]
        [SerializeField]
        RectTransform mResultTra;

        GameObject mDisplayObj;             //�\�����I�u�W�F�N�g
        GoogleAdmobReward mAdReward;        //�����[�h�L���N���X
        Coroutine mAppearanceCor;           //�o���A�j���[�V�����R���[�`��
        Coroutine mStatusMonitoringAdsCor;  //�L���̏�ԊĎ��R���[�`��

        //==========================================================//
        //------------------------���U���g���----------------------//
        //==========================================================//

        /// <summary>
        /// �m�F�E�B���h�E�\�����
        /// </summary>
        public enum ConfirmWinStatus
        {
            TryAgain,       //�Ē��킷��H
            ReturnTitle     //�^�C�g���ɖ߂�H
        }
        [HideInInspector]
        public ConfirmWinStatus mConfirmWinState = ConfirmWinStatus.TryAgain;

        /// <summary>
        /// �Q�[���N���A
        /// </summary>
        public IEnumerator GenerateGameClearObj()
        {
            ObjectDestroy();
            mDisplayObj = Instantiate(mGameClearPre);
            yield return StartCoroutine(ObjectAppearance());
            SceneNavigator.Instance.Change(TITLE_SCENE_NAME);
        }

        /// <summary>
        /// �Q�[���I�[�o�[
        /// </summary>
        public IEnumerator GenerateGameOverObj()
        {
            StartCoroutine(CanvasMgr.SetFilter(true));
            ObjectDestroy();
            mDisplayObj = Instantiate(mGameOverPreArr[(TURN_RECOVERED) ? 1 : 0]);
            yield return StartCoroutine(ObjectAppearance());
        }

        /// <summary>
        /// �m�F�E�B���h�E
        /// </summary>
        public void GenerateConfirmObj()
        {
            //�m�F���e�ݒ�
            string msg = "";
            switch (mConfirmWinState)
            {
                case ConfirmWinStatus.TryAgain:
                    msg = "���������ǒ��킵�܂����H";
                    break;
                case ConfirmWinStatus.ReturnTitle:
                    msg = "�^�C�g����ʂɖ߂�܂�\n��낵���ł����H";
                    break;
            }

            //�����o�ϐ��̎Q�Ƃ����݂���(�I�u�W�F�N�g������)�ꍇ�͉��ߐ������Ȃ�
            if (!mConfirmMsgText)
            {
                //�E�B���h�E����
                ObjectDestroy();
                mDisplayObj = Instantiate(mConfirmWinPre);
                mConfirmMsgText = mDisplayObj.transform.GetChild(0).GetChild(0).GetComponent<Text>();
                StartCoroutine(ObjectAppearance(false));
            }

            //�e�L�X�g���f
            mConfirmMsgText.text = msg;
        }
        Text mConfirmMsgText;   //�m�F�E�B���h�E���b�Z�[�W�e�L�X�g

        /// <summary>
        /// �I�u�W�F�N�g�o��
        /// </summary>
        /// <param name="_anime">�o���A�j���̗L��</param>
        IEnumerator ObjectAppearance(bool _anime = true)
        {
            RectTransform tra = mDisplayObj.GetComponent<RectTransform>();
            tra.SetParent(mResultTra);
            tra.localPosition = Vector3.zero;
            tra.localScale = Vector3.one;
            if (_anime)
            {
                Animator ani = mDisplayObj.GetComponent<Animator>();
                mAppearanceCor = StartCoroutine(AnimationStart(ani, STATE_NAME_APPEARANCE));
                yield return mAppearanceCor;
            }
        }

        /// <summary>
        /// �I�u�W�F�N�g�폜
        /// </summary>
        void ObjectDestroy()
        {
            if (mDisplayObj != null)
            {
                StopCoroutine(mAppearanceCor);  //�o���A�j���[�V�������~
                Destroy(mDisplayObj);           //�I�u�W�F�N�g�j��
            }
        }


        //==========================================================//
        //------------------------�L���\��--------------------------//
        //==========================================================//

        /// <summary>
        /// �����[�h�L���\�����
        /// </summary>
        public enum AdRewardState
        {
            None,
            Loading,        //�ǂݍ��ݒ�
            LoadCancel,     //�ǂݍ��݃L�����Z��
            Loaded,         //�ǂݍ��݊���
            FailedToLoad,   //�ǂݍ��ݎ��s
            AdOpen,         //�L���\��
            FailedToOpen,   //�L���\�����s
            AdClosed,       //�L���I��
            EarnedReward    //��V�l��
        }
        [HideInInspector]
        public AdRewardState mRewardState = AdRewardState.None;

        /// <summary>
        /// �����[�h�L���\��
        /// </summary>
        void GenerateAdRewardObj()
        {
            //�\�����̏ꍇ�͏��������Ȃ�
            if (mRewardState != AdRewardState.None) return;

            //���U���g�I�u�W�F�N�g�̍폜
            ObjectDestroy();

            //�ǂݍ��݊J�n�\��
            mRewardState = AdRewardState.Loading;
            GenerateMessageWindow();

            //�I�u�W�F�N�g����
            GameObject obj = Instantiate(mAdRewardPre);
            mAdReward = obj.GetComponent<GoogleAdmobReward>();

            //��ԊĎ�
            mStatusMonitoringAdsCor = StartCoroutine(StatusMonitoringRewardAds());
        }

        /// <summary>
        /// �����[�h�L���̏�ԊĎ�
        /// </summary>
        /// <returns></returns>
        IEnumerator StatusMonitoringRewardAds()
        {
            //��ԊĎ�
            bool end = false;
            while (true)
            {
                switch (mRewardState)
                {
                    //�ǂݍ��݊���
                    case AdRewardState.Loaded:
                        //�L���J�n
                        yield return new WaitForSeconds(3.0f);//�e�X�g�ҋ@
                        end = !mAdReward.ShowRewardAd();
                        break;

                    case AdRewardState.LoadCancel:      //�ǂݍ��݃L�����Z��
                    case AdRewardState.FailedToLoad:    //�ǂݍ��ݎ��s
                    case AdRewardState.FailedToOpen:    //�L���\�����s
                        end = true;
                        break;

                    //��V�l��
                    case AdRewardState.EarnedReward:
                        StartCoroutine(TurnMgr.TurnRecovery_AdReward());
                        end = true;
                        break;

                        //case AdRewardState.Loading:     //�ǂݍ��ݒ�
                        //case AdRewardState.AdOpen:      //�L���\��
                        //case AdRewardState.AdClosed:    //�L���I��
                }

                if (end) break;
                yield return FIXED_UPDATE;
            }

            //���b�Z�[�W�\��
            GenerateMessageWindow();

            //�L���I�u�W�F�N�g�j��
            mAdReward.RewardOnDestroy();
        }

        /// <summary>
        /// ���b�Z�[�W�\��
        /// </summary>
        void GenerateMessageWindow()
        {
            //�\��������w��
            string msg = "";
            string btnText = BUTTON_TEXT_CLOSE;
            switch (mRewardState)
            {
                //�ǂݍ��ݒ�
                case AdRewardState.Loading:
                    msg = "�L����ǂݍ���ł��܂�\n���΂炭���҂���������";
                    btnText = BUTTON_TEXT_CANCEL;
                    break;

                //�ǂݍ��݃L�����Z��
                case AdRewardState.LoadCancel:
                    msg = "�L�����L�����Z�����܂���";
                    break;

                //�ǂݍ��݊���(�\�����s)
                case AdRewardState.Loaded:
                    msg = "�L���̕\���Ɏ��s���܂���\n������x��������������";
                    break;

                //�ǂݍ��ݎ��s
                case AdRewardState.FailedToLoad:
                    msg = "�L���̎擾�Ɏ��s���܂���\n������x��������������";
                    break;

                //�L���\�����s
                case AdRewardState.FailedToOpen:
                    msg = "�L���̕\���Ɏ��s���܂���\n������x��������������";
                    break;

                //��V�l��
                case AdRewardState.EarnedReward:
                    msg = "5�^�[���񕜂��܂���\n�Q�[�����ĊJ���܂�";
                    break;
            }

            //�e�����o�ϐ��̎Q�Ƃ����݂���(�I�u�W�F�N�g������)�ꍇ�͉��ߐ������Ȃ�
            if (!mMessageMsgText || !mMessageBtnText || !mMessageBtnTra)
            {
                //�I�u�W�F�N�g�\��
                ObjectDestroy();
                mDisplayObj = Instantiate(mMessageWinPre);
                StartCoroutine(ObjectAppearance(false));

                //�e�L�X�g�擾
                Transform winTra = mDisplayObj.transform.GetChild(0);
                mMessageMsgText = winTra.GetChild(0).GetComponent<Text>();

                //�{�^���e�L�X�g,�����擾
                mMessageBtnTra = winTra.GetChild(1).GetComponent<RectTransform>();
                mMessageBtnText = mMessageBtnTra.GetChild(0).GetChild(0).GetComponent<Text>();
            }

            //�e�L�X�g���f
            mMessageMsgText.text = msg;
            mMessageBtnText.text = btnText;
            mMessageBtnTra.sizeDelta = new Vector2(BUTTON_WIDTH[btnText], mMessageBtnTra.sizeDelta.y);
        }
        Text mMessageMsgText;           //���b�Z�[�W�e�L�X�g
        Text mMessageBtnText;           //�{�^���e�L�X�g
        RectTransform mMessageBtnTra;   //�{�^��RectTransform

        /// <summary>
        /// �Q�[���ĊJ
        /// </summary>
        /// <returns></returns>
        IEnumerator GameRestart()
        {
            //�t�B���^�[�̉���
            Coroutine cor = StartCoroutine(CanvasMgr.SetFilter(false));

            //�t�B���^�[�̉����ҋ@(����{�^���̃^�b�v�ɋ�𔽉������Ȃ����߂ł�����)
            yield return null;  //1�t���[���͕K���ҋ@
            yield return cor;

            //�t���O�̃��Z�b�g
            FlagReset();
            TURN_RECOVERED = true;
        }

        /// <summary>
        /// �Ē���
        /// </summary>
        /// <returns></returns>
        void TryAgain()
        {
            SceneNavigator.Instance.Change(PUZZLE_SCENE_NAME);
        }

        /// <summary>
        /// �^�C�g����
        /// </summary>
        /// <returns></returns>
        void ReturnTitle()
        {
            SceneNavigator.Instance.Change(TITLE_SCENE_NAME);
        }


        //==========================================================//
        //----------------------�{�^������--------------------------//
        //==========================================================//

        /// <summary>
        /// �͂�
        /// </summary>
        public void IsPushYes()
        {
            switch (mConfirmWinState)
            {
                //�Ē��킷��H
                case ConfirmWinStatus.TryAgain:
                    TryAgain();
                    break;

                //�^�C�g���ɖ߂�H
                case ConfirmWinStatus.ReturnTitle:
                    ReturnTitle();
                    break;
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        public void IsPushNo()
        {
            switch (mConfirmWinState)
            {
                //�Ē��킷��H
                case ConfirmWinStatus.TryAgain:
                    mConfirmWinState = ConfirmWinStatus.ReturnTitle;
                    GenerateConfirmObj();
                    break;

                //�^�C�g���ɖ߂�H
                case ConfirmWinStatus.ReturnTitle:
                    StartCoroutine(GenerateGameOverObj());
                    break;
            }
        }

        /// <summary>
        /// �L��������
        /// </summary>
        public void IsPushSeeAbs()
        {
            GenerateAdRewardObj();
        }

        /// <summary>
        /// ������߂�
        /// </summary>
        public void IsPushGiveUp()
        {
            mConfirmWinState = ConfirmWinStatus.TryAgain;
            GenerateConfirmObj();
        }

        /// <summary>
        /// ����E�L�����Z��
        /// </summary>
        public void IsPushClose()
        {
            switch (mRewardState)
            {
                case AdRewardState.Loading: //�ǂݍ��ݒ�
                case AdRewardState.Loaded:  //�ǂݍ��݊���(�\�����s)
                    //�L���I�u�W�F�N�g�̔j��
                    mRewardState = AdRewardState.LoadCancel;
                    StopCoroutine(mStatusMonitoringAdsCor);
                    mAdReward.RewardOnDestroy();
                    GenerateMessageWindow();
                    break;

                case AdRewardState.LoadCancel:      //�ǂݍ��݃L�����Z��
                case AdRewardState.FailedToLoad:    //�ǂݍ��ݎ��s
                case AdRewardState.FailedToOpen:    //�L���\�����s
                    //�Q�[���[�o�[�I�u�W�F�N�g����
                    StartCoroutine(GenerateGameOverObj());
                    mRewardState = AdRewardState.None;
                    break;

                //��V�l��
                case AdRewardState.EarnedReward:
                    //�Q�[���ĊJ
                    ObjectDestroy();
                    StartCoroutine(GameRestart());
                    mRewardState = AdRewardState.None;
                    break;

                //���̑�(���邱�Ƃ͂Ȃ��͂�)
                default:
                    ObjectDestroy();
                    mRewardState = AdRewardState.None;
                    break;
            }
        }

        /// <summary>
        /// �Ē��킷��
        /// </summary>
        public void IsPushTryAgain()
        {
            TryAgain();
        }

        /// <summary>
        /// �^�C�g���ɖ߂�
        /// </summary>
        public void IsPushReturnTitle()
        {
            ReturnTitle();
        }
    }
}