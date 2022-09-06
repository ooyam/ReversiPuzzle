using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static CommonDefine;
using static PuzzleDefine;
using static Sound.SoundManager;
using static animation.AnimationManager;
using static ObjectMove.ObjectMove_UI;

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
        OptionType mOptionType;

        //�I�v�V�������
        enum OptionState
        {
            None,           //�ʏ�
            Credit,         //�N���W�b�g
            Confirm,        //�m�F�E�B���h�E
            TutorialSel,    //�`���[�g���A��(�I��)
            TutorialView,   //�`���[�g���A��(����)

            Count           //����
        }
        OptionState mState = OptionState.None;
        
        //�m�F�^�C�v
        enum ConfirmType
        {
            Redo,           //��蒼��
            ReturnTitle,    //�^�C�g���ɖ߂�
            QiteGame,       //�Q�[�����I������
        }
        ConfirmType mConfirmType;

        //�m�F���̐ݒ�
        const string CONFIRM_TEXT = "\n��낵���ł����H";
        readonly Dictionary<ConfirmType, string> mConfirmTextDic = new Dictionary<ConfirmType, string>
        {
            { ConfirmType.Redo,         "���̃X�e�[�W����蒼���܂�" + CONFIRM_TEXT },
            { ConfirmType.ReturnTitle,  "�^�C�g���ɖ߂�܂�" + CONFIRM_TEXT },
            { ConfirmType.QiteGame,     "�Q�[�����I�����܂�" + CONFIRM_TEXT }
        };

        [Header("�I�v�V�������")]
        [SerializeField]
        GameObject[] mOptionObjArr = new GameObject[(int)OptionState.Count];

        [Header("BGM�ؑփ{�^��Image")]
        [SerializeField]
        Image mBgmBtnImg;

        [Header("SE�ؑփ{�^��Image")]
        [SerializeField]
        Image mSeBtnImg;

        [Header("�m�F�E�B���h�E�e�L�X�g")]
        [SerializeField]
        Text mConfirmText;

        [Header("�؂�ւ��{�^���X�v���C�g")]
        [SerializeField]
        Sprite[] mSwitchBtnSpr;

        //�X�v���C�g�^�C�v
        enum SwitchBtnType
        { On, Off }

        [Header("�`���[�g���A���{�^���i�[�I�u�W�F�N�g")]
        [SerializeField]
        RectTransform mTutorialBtnParentTra;

        [Header("�`���[�g���A���i�[�I�u�W�F�N�g")]
        [SerializeField]
        RectTransform mTutorialScreenParentTra;
        readonly GameObject[] mTutorialObjArr = new GameObject[(int)TutorialType.Count];
        readonly RectTransform[] mTutorialPagesTraArr = new RectTransform[(int)TutorialType.Count];
        readonly int[] mTutorialMaxPageArr = new int[(int)TutorialType.Count];

        //�`���[�g���A���^�C�v
        public enum TutorialType
        {
            None = -1,   //�f�[�^�x�[�X�����w��p

            //---����--//

            GameDescription,    //�Q�[���̐���
            BasicOperation,     //��{����
            SupportItem,        //����A�C�e��

            //---�M�~�b�N--//

            Balloon,    //���D
            Jewelry,    //���
            Wall,       //�Ε�
            Flower,     //�큨�Q������
            Frame,      //�F�g
            Hamster,    //�I���t
            Cage,       //�S�i�q
            NumberTag,  //�ԍ��D
            Thief,      //�D�_
            Steel,      //�|
            Tornado,    //����

            Count   //����
        }
        TutorialType mTutorialType;

        //�����\���`���[�g���A��
        public static TutorialType ForcedTutorialType { get; set; }

        [Header("���I�u�W�F�N�g")]
        [SerializeField]
        GameObject[] mArrowObjArr = new GameObject[(int)ArrowType.Count];

        //���^�C�v
        enum ArrowType
        {
            SelRight,   //�I���E
            SelLeft,    //�I����
            ViewRight,  //�\���E
            ViewLeft,   //�\����

            Count
        }

        //�\�����̂؁[�W
        int mTutorialSelectPage;    //�`���[�g���A���I��
        int mTutorialViewPage;      //�`���[�g���A���\��
        Coroutine mPageMoveCor;

        const int TUTORIAL_SELECT_MAX_PAGE_INDEX = 1;

        /// <summary>
        /// ������
        /// </summary>
        public void Initialize()
        {
            //�^�C�v�ݒ�
            switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                //�^�C�g���V�[��
                case TITLE_SCENE_NAME:
                    mOptionType = OptionType.Title;
                    break;

                //�p�Y���V�[��
                case PUZZLE_SCENE_NAME:
                    mOptionType = OptionType.Puzzle;

                    for (int i = 0; i < (int)TutorialType.Count; i++)
                    {
                        //�`���[�g���A����ʎ擾
                        Transform tra = mTutorialScreenParentTra.GetChild(i);
                        mTutorialObjArr[i] = tra.gameObject;
                        Transform pagesTra = tra.GetChild(0);
                        mTutorialPagesTraArr[i] = pagesTra.GetComponent<RectTransform>();
                        mTutorialMaxPageArr[i] = pagesTra.childCount;

                        //�`���[�g���A���{�^���ݒ�
                        int childIndex = (i <= (int)TutorialType.SupportItem || SaveDataManager.ViewedTutorialNum >= i) ? 0 : 1; //0:������,1:���J��
                        GameObject btnObj = mTutorialBtnParentTra.GetChild(i).GetChild(childIndex).gameObject;
                        btnObj.SetActive(true);
                        TutorialType type = (TutorialType)Enum.ToObject(typeof(TutorialType), i);
                        btnObj.GetComponent<Button>().onClick.AddListener(() => IsPushTutorialSelect(type));
                    }
                    break;
            }

            //BGM�ESE�\���ؑ�
            mBgmBtnImg.sprite = mSwitchBtnSpr[Preferences.Bgm ? (int)SwitchBtnType.On : (int)SwitchBtnType.Off];
            mSeBtnImg.sprite = mSwitchBtnSpr[Preferences.Se ? (int)SwitchBtnType.On : (int)SwitchBtnType.Off];
        }


        //===============================================//
        //---------------�`���[�g���A���֌W--------------//
        //===============================================//

        /// <summary>
        /// �����`���[�g���A��
        /// </summary>
        public void ForcedTutorial()
        {
            if (ForcedTutorialType == TutorialType.None) return;
            if ((int)ForcedTutorialType <= SaveDataManager.ViewedTutorialNum) return;

            //�����`���[�g���A�����t���O�Z�b�g
            FlagOn(PuzzleFlag.NowForcedTutorial);

            //�t�B���^�[�\��
            SetFilter(true);

            //�`���[�g���A���\��
            TutorialDisplay(ForcedTutorialType);
        }

        /// <summary>
        /// �����`���[�g���A���I��
        /// </summary>
        void ForcedTutorialEnd()
        {
            //�f�[�^�ۑ�
            SaveDataManager.SetViewedTutorialNum((int)ForcedTutorialType);
            SaveDataManager.DataSave();

            //�Q�[�������̏ꍇ
            if (ForcedTutorialType == TutorialType.GameDescription)
            {
                //�����Ċ�{�����\��
                ForcedTutorialType = TutorialType.BasicOperation;
                TutorialDisplay(ForcedTutorialType);
            }
            //���̑�
            else
            {
                //�t�B���^�[��\��
                SetFilter(false);

                //�I�v�V������ʑS��\��
                ObjInactive();

                //�����`���[�g���A�����t���O�Z�b�g
                FlagOff(PuzzleFlag.NowForcedTutorial);
            }
        }

        /// <summary>
        /// �`���[�g���A���\��
        /// </summary>
        /// <param name="_type">�`���[�g���A���^�C�v</param>
        void TutorialDisplay(TutorialType _type)
        {
            SetTutorialView(_type);
            StartCoroutine(ObjectAppearance(OptionState.TutorialView));

            //�y�[�W������
            mTutorialViewPage = 0;
            TutorialViewPageChange();
        }


        //==========================================================//
        //-----------------------�{�^������-------------------------//
        //==========================================================//

        /// <summary>
        /// �I�v�V����
        /// </summary>
        public void IsPushOption()
        {
            //�p�Y���V�[��
            if (mOptionType == OptionType.Puzzle)
            {
                //����֎~
                if (!IsOperable()) return;

                //�I�v�V�����\���t���O�Z�b�g
                FlagOn(PuzzleFlag.NowOptionView);
            }

            //�t�B���^�[�\��
            SetFilter(true);

            //SE�Đ�
            SE_OneShot(SE_Type.BtnYes);
            StartCoroutine(ObjectAppearance(OptionState.None));
        }

        /// <summary>
        /// BGM_ON�EOFF
        /// </summary>
        public void IsPushBGM()
        {
            //BGM�ؑ�
            Preferences.Bgm = !Preferences.Bgm;
            if (Preferences.Bgm)
            {
                //BGM�J�n
                SE_OneShot(SE_Type.BtnYes);
                BGM_FadeRestart();
                mBgmBtnImg.sprite = mSwitchBtnSpr[(int)SwitchBtnType.On];
            }
            else
            {
                //BGM�X�g�b�v
                SE_OneShot(SE_Type.BtnNo);
                BGM_Stop();
                mBgmBtnImg.sprite = mSwitchBtnSpr[(int)SwitchBtnType.Off];
            }
        }

        /// <summary>
        /// SE_ON�EOFF
        /// </summary>
        public void IsPushSE()
        {
            //SE�ؑ�
            Preferences.Se = !Preferences.Se;
            if (Preferences.Se)
            {
                //ON
                mSeBtnImg.sprite = mSwitchBtnSpr[(int)SwitchBtnType.On];
                SE_OneShot(SE_Type.BtnYes);
            }
            else
            {
                //OFF
                mSeBtnImg.sprite = mSwitchBtnSpr[(int)SwitchBtnType.Off];
                SE_StopAll();
            }
        }

        /// <summary>
        /// �N���W�b�g
        /// </summary>
        public void IsPushCredit()
        {
            //SE�Đ�
            SE_OneShot(SE_Type.BtnYes);
            StartCoroutine(ObjectAppearance(OptionState.Credit));
        }

        /// <summary>
        /// �Q�[������߂�
        /// </summary>
        public void IsPushQuitGame()
        {
            //SE�Đ�
            SE_OneShot(SE_Type.BtnYes);
            SetConfirmText(ConfirmType.QiteGame);
            StartCoroutine(ObjectAppearance(OptionState.Confirm, false));
        }

        /// <summary>
        /// �͂�
        /// </summary>
        public void IsPushYes()
        {
            //SE�Đ�
            SE_OneShot(SE_Type.BtnYes);
            
            //�V�[���ڊ�
            switch (mConfirmType)
            {
                //��蒼��
                case ConfirmType.Redo:
                    StartCoroutine(BGM_FadeStop()); //BGM�t�F�[�h�A�E�g
                    SceneFader.SceneChangeFade(PUZZLE_SCENE_NAME);
                    break;

                //�^�C�g���֖߂�
                case ConfirmType.ReturnTitle:
                    StartCoroutine(BGM_FadeStop()); //BGM�t�F�[�h�A�E�g
                    SceneFader.SceneChangeFade(TITLE_SCENE_NAME);
                    break;

                //�Q�[���I��
                case ConfirmType.QiteGame:
                    GameManager.QuitGame();
                    break;
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        public void IsPushNo()
        {
            //SE�Đ�
            SE_OneShot(SE_Type.BtnNo);
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
                    OptionEnd();
                    break;

                //�N���W�b�g�\��,�`���[�g���A��(�I��)
                case OptionState.Credit:
                case OptionState.TutorialSel:
                    StartCoroutine(ObjectAppearance(OptionState.None)); //�ʏ��Ԃ�
                    break;

                //�`���[�g���A��(����)
                case OptionState.TutorialView:
                    if (GetFlag(PuzzleFlag.NowForcedTutorial)) ForcedTutorialEnd();     //�����`���[�g���A���I��
                    else  StartCoroutine(ObjectAppearance(OptionState.TutorialSel));    //�`���[�g���A��(�I��)��
                    break;

                //���邱�Ƃ͂Ȃ��͂������O�̂���
                default: return;
            }

            //SE�Đ�
            SE_OneShot(SE_Type.BtnNo);
        }

        /// <summary>
        /// �`���[�g���A��
        /// </summary>
        public void IsPushTutorial()
        {
            //SE�Đ�
            SE_OneShot(SE_Type.BtnYes);
            StartCoroutine(ObjectAppearance(OptionState.TutorialSel));

            //�y�[�W������
            mTutorialSelectPage = 0;
            TutorialSelectPageChange();
        }

        /// <summary>
        /// ��蒼��
        /// </summary>
        public void IsPushRedo()
        {
            //SE�Đ�
            SE_OneShot(SE_Type.BtnYes);
            SetConfirmText(ConfirmType.Redo);
            StartCoroutine(ObjectAppearance(OptionState.Confirm, false));
        }

        /// <summary>
        /// �^�C�g���ɖ߂�
        /// </summary>
        public void IsPushReturnTitle()
        {
            //SE�Đ�
            SE_OneShot(SE_Type.BtnYes);
            SetConfirmText(ConfirmType.ReturnTitle);
            StartCoroutine(ObjectAppearance(OptionState.Confirm, false));
        }

        /// <summary>
        /// �`���[�g���A���I��
        /// </summary>
        void IsPushTutorialSelect(TutorialType _type)
        {
            //���J��
            if (SaveDataManager.ViewedTutorialNum < (int)_type)
            {
                //SE�Đ�
                SE_OneShot(SE_Type.BtnNo);
            }
            //�����
            else
            {
                //SE�Đ�
                SE_OneShot(SE_Type.BtnYes);
                TutorialDisplay(_type);
            }
        }

        /// <summary>
        /// �E���
        /// </summary>
        public void IsPushRightArrow()
        {
            switch (mState)
            {
                //�`���[�g���A���I�����
                case OptionState.TutorialSel:
                    mTutorialSelectPage++;
                    TutorialSelectPageChange();
                    break;

                //�`���[�g���A���\�����
                case OptionState.TutorialView:
                    mTutorialViewPage++;
                    TutorialViewPageChange();
                    break;
            }

            //SE�Đ�
            SE_OneShot(SE_Type.BtnYes);
        }

        /// <summary>
        /// �����
        /// </summary>
        public void IsPushLeftArrow()
        {
            switch (mState)
            {
                //�`���[�g���A���I�����
                case OptionState.TutorialSel:
                    mTutorialSelectPage--;
                    TutorialSelectPageChange();
                    break;

                //�`���[�g���A���\�����
                case OptionState.TutorialView:
                    mTutorialViewPage--;
                    TutorialViewPageChange();
                    break;
            }

            //SE�Đ�
            SE_OneShot(SE_Type.BtnYes);
        }

        //==================================================//
        //---------------------�\���֌W---------------------//
        //==================================================//

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
            {
                if (obj == null) continue;
                obj.SetActive(false);
            }
        }

        /// <summary>
        /// �m�F�E�B���h�E�e�L�X�g�̐ݒ�
        /// </summary>
        void SetConfirmText(ConfirmType _conType)
        {
            mConfirmType = _conType;
            mConfirmText.text = mConfirmTextDic[_conType];
        }

        /// <summary>
        /// �`���[�g���A���\���̐؂�ւ�
        /// </summary>
        void SetTutorialView(TutorialType _tutType)
        {
            mTutorialObjArr[(int)mTutorialType].SetActive(false);
            mTutorialObjArr[(int)_tutType].SetActive(true);
            mTutorialType = _tutType;
        }

        /// <summary>
        /// �t�B���^�[�ؑ�
        /// </summary>
        void SetFilter(bool on)
        {
            switch (mOptionType)
            {
                //�^�C�g���V�[��
                case OptionType.Title:
                    StartCoroutine(Title.TitleMain.CanvasMgr.SetFilter(on));
                    break;

                //�p�Y���V�[��
                case OptionType.Puzzle:
                    StartCoroutine(PuzzleMain.PuzzleMain.CanvasMgr.SetFilter(on));
                    break;
            }
        }

        /// <summary>
        /// �`���[�g���A���I���y�[�W�ύX
        /// </summary>
        void TutorialSelectPageChange()
        {
            mTutorialSelectPage = Mathf.Clamp(mTutorialSelectPage, 0, TUTORIAL_SELECT_MAX_PAGE_INDEX);

            //�y�[�W�ړ�
            Vector2 targetPos = new Vector2(mTutorialSelectPage * -PLAY_SCREEN_WIDTH, 0.0f);
            if (mPageMoveCor != null) StopCoroutine(mPageMoveCor);
            mPageMoveCor = StartCoroutine(DecelerationMovement(mTutorialBtnParentTra, PAGE_MOVE_SPEED, targetPos));

            //���\����ԕύX
            mArrowObjArr[(int)ArrowType.SelRight].SetActive(mTutorialSelectPage < TUTORIAL_SELECT_MAX_PAGE_INDEX);
            mArrowObjArr[(int)ArrowType.SelLeft].SetActive(mTutorialSelectPage > 0);
        }

        /// <summary>
        /// �`���[�g���A���\���y�[�W�ύX
        /// </summary>
        void TutorialViewPageChange()
        {
            mTutorialViewPage = Mathf.Clamp(mTutorialViewPage, 0, mTutorialMaxPageArr[(int)mTutorialType] - 1);

            //�y�[�W�ړ�
            Vector2 targetPos = new Vector2(mTutorialViewPage * -PLAY_SCREEN_WIDTH, 0.0f);
            if (mPageMoveCor != null) StopCoroutine(mPageMoveCor);
            mPageMoveCor = StartCoroutine(DecelerationMovement(mTutorialPagesTraArr[(int)mTutorialType], PAGE_MOVE_SPEED, targetPos));

            //���\����ԕύX
            mArrowObjArr[(int)ArrowType.ViewRight].SetActive(mTutorialViewPage < mTutorialMaxPageArr[(int)mTutorialType] - 1);
            mArrowObjArr[(int)ArrowType.ViewLeft].SetActive(mTutorialViewPage > 0);
        }

        /// <summary>
        /// �I�v�V�����I��
        /// </summary>
        void OptionEnd()
        {
            //�t�B���^�[����
            SetFilter(false);

            //�I�v�V������ʑS��\��
            ObjInactive();

            //�p�Y���V�[���̏ꍇ�̓I�v�V�����\���t���O���Z�b�g
            if (mOptionType == OptionType.Puzzle)
                FlagOff(PuzzleFlag.NowOptionView);
        }
    }
}