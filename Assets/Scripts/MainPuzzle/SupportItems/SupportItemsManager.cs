using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static ObjectMove_2D.ObjectMove_2D;
using static animation.AnimationManager;

namespace PuzzleMain
{
    public class SupportItemsManager : MonoBehaviour
    {
        PiecesManager piecesMgr;    //PiecesManager

        [Header("����A�C�e���̎擾")]
        [SerializeField]
        GameObject[] mItemsArr;

        [Header("�⏕�A�C�e���ҋ@�{�b�N�X�̎擾")]
        [SerializeField]
        Transform mWaitItemBoxesTra;

        SupportItemInformation[] mItemsInfoArr; //����A�C�e�����

        Transform[]  mWaitItemBoxsTraArr;       //�ҋ@����A�C�e����Transform
        GameObject[] mWaitItemObjArr;           //�ҋ@����A�C�e��
        Animator[]   mWaitItemAniArr;           //�ҋ@����A�C�e��Animator
        bool[]       mWaitItemReadyUse;         //�ҋ@����A�C�e���g�p�\���

        int mReadyItemNumber;           //�������̃A�C�e���ԍ�
        int mDuckSupportLineNum;        //�A�q���̉���s�ԍ�

        //1�^�[���̔j����
        int  mDelPieceCount = 0;        //�j�󂵂���̐�
        int  mDelDirCount   = 0;        //�j�󂵂������̐�
        bool mDelLine       = false;    //1�s�j��H
        bool mDelColumn     = false;    //1��j��H

        const int DUCK_USE_DEL_PIECE_COUNT = 6;     //�A�q����������
        const int FIREWORK_USE_DIR_PIECE_COUNT = 4; //�ԉΐ�������

        /// <summary>
        /// ����A�C�e���̏�����
        /// </summary>
        public void Initialize()
        {
            piecesMgr = sPuzzleMain.GetPiecesManager();

            mReadyItemNumber    = INT_NULL;
            mWaitItemBoxsTraArr = new Transform[SUPPORT_ITEMS_COUNT];
            mWaitItemObjArr     = new GameObject[SUPPORT_ITEMS_COUNT];
            mWaitItemAniArr     = new Animator[SUPPORT_ITEMS_COUNT];
            mWaitItemReadyUse   = new bool[SUPPORT_ITEMS_COUNT];
            mItemsInfoArr       = new SupportItemInformation[SUPPORT_ITEMS_COUNT];
            for (int i = 0; i < SUPPORT_ITEMS_COUNT; i++)
            {
                mWaitItemBoxsTraArr[i]  = mWaitItemBoxesTra.GetChild(i).transform;
                mWaitItemObjArr[i]      = mWaitItemBoxsTraArr[i].GetChild(0).gameObject;
                mWaitItemAniArr[i]      = mWaitItemObjArr[i].GetComponent<Animator>();
                mWaitItemReadyUse[i]    = false;
                mItemsInfoArr[i]        = mItemsArr[i].GetComponent<SupportItemInformation>();
                mItemsInfoArr[i].InformationSetting();
            }
        }

        /// <summary>
        /// �ҋ@�A�C�e���̃^�b�v����
        /// </summary>
        /// <param name="_tapObj">�^�b�v�I�u�W�F�N�g</param>
        public void TapJudgmentWaitItem(GameObject _tapObj)
        {
            int waitItemIndex = Array.IndexOf(mWaitItemObjArr, _tapObj);
            if (waitItemIndex < 0) return;

            if (mWaitItemReadyUse[waitItemIndex])
                ResetWaitItemReady();             //��������
            else
                SetWaitItemReady(waitItemIndex);  //����
        }

        /// <summary>
        /// �ҋ@�A�C�e����������Ԃɂ���
        /// </summary>
        /// <param name="_itemNum">����A�C�e���ԍ�</param>
        public void SetWaitItemReady(int _itemNum)
        {
            //�t���O�Z�b�g
            NOW_SUPPORT_ITEM_READY = true;

            //�w��A�C�e����������Ԃ�
            mWaitItemReadyUse[_itemNum] = true;
            StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_READY));

            //�w��A�C�e���ȊO��������Ԃɂ���
            if (mReadyItemNumber >= 0)
            {
                mWaitItemReadyUse[mReadyItemNumber] = false;
                LoopAnimationStart(mWaitItemAniArr[mReadyItemNumber]);
            }

            //�������̃A�C�e���ԍ��X�V
            mReadyItemNumber = _itemNum;
        }

        /// <summary>
        /// �ҋ@�A�C�e����������Ԃ���������
        /// </summary>
        public void ResetWaitItemReady()
        {
            //�t���O���Z�b�g
            NOW_SUPPORT_ITEM_READY = false;

            //�Z�b�g���̃A�C�e�����Ȃ��ꍇ�͏������X�L�b�v
            if (mReadyItemNumber < 0) return;

            //������Ԃɂ���
            mWaitItemReadyUse[mReadyItemNumber] = false;
            LoopAnimationStart(mWaitItemAniArr[mReadyItemNumber]);

            //�������̃A�C�e���ԍ����Z�b�g
            mReadyItemNumber = INT_NULL;
        }

        /// <summary>
        /// �ҋ@����A�C�e���̕\���ؑ�
        /// </summary>
        /// <param name="_itemNum">����A�C�e���ԍ�</param>
        /// <param name="_active"> �\�����</param>
        void SetWaitItemsActive(int _itemNum, bool _active)
        {
            mWaitItemObjArr[_itemNum].SetActive(_active);

            if (_active) StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_ACTIVE));
            else         StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_INACTIVE));
        }

        /// <summary>
        /// ��j����ݒ�
        /// </summary>
        /// <param name="_delPieceCount">��]��</param>
        /// <param name="_dirCount">     ���]������</param>
        /// <param name="_line">         1�s���]</param>
        /// <param name="_column">       1�񔽓]</param>
        public void SetPieceDeleteInfomation(int _delPieceCount, int _dirCount, bool _line, bool _column)
        {
            mDelPieceCount = _delPieceCount;    
            mDelDirCount   = _dirCount;    
            mDelLine       = _line;
            mDelColumn     = _column;
        }

        /// <summary>
        /// ��j���񃊃Z�b�g
        /// </summary>
        public void ResetPieceDeleteInfomation()
        {
            SetPieceDeleteInfomation(0, 0, false, false);
        }

        /// <summary>
        /// �A�C�e���g�p�\����
        /// </summary>
        public void SetItems()
        {
            if (mDelLine && mDelColumn)
            {
                //�S����
            }
            else if (mDelColumn)
            {
                //���P�b�g(�c)
            }
            else if (mDelLine)
            {
                //���P�b�g(��)
            }
            else if (mDelDirCount >= FIREWORK_USE_DIR_PIECE_COUNT)
            {
                //�ԉ�
            }

            //���ق�Ƃ�else if
            if (mDelPieceCount >= DUCK_USE_DEL_PIECE_COUNT)
            {
                //�A�q������
                SetWaitItemsActive((int)SupportItems.Duck, true);
            }

            //��j���񃊃Z�b�g
            ResetPieceDeleteInfomation();
        }

        /// <summary>
        /// ����A�C�e���̕\���ؑ�
        /// </summary>
        /// <param name="_itemNum">����A�C�e���ԍ�</param>
        /// <param name="_active"> �\�����</param>
        void SetItemsActive(int _itemNum, bool _active)
        {
            mItemsArr[_itemNum].SetActive(_active);
        }

        /// <summary>
        /// ����A�C�e���̎g�p
        /// </summary>
        /// <param name="_tapSquare">�w��}�X�ԍ�</param>
        public IEnumerator UseItems(int _tapSquare)
        {
            //�A�C�e���g�p�t���O�Z�b�g
            NOW_SUPPORT_ITEM_USE = true;

            //�A�C�e���ԍ��ێ�
            int itemNum = mReadyItemNumber;

            //������ԉ���
            ResetWaitItemReady();

            switch (itemNum)
            {
                //�A�q��
                case (int)SupportItems.Duck:
                    yield return StartCoroutine(UseDuck(_tapSquare % BOARD_LINE_COUNT));
                    break;
            }

            //�M�~�b�N�j��ҋ@
            foreach (Coroutine gimmickCor in sGimmickCorList)
            { yield return gimmickCor; }
            sGimmickCorList = new List<Coroutine>();

            //�ҋ@�A�C�e����\��
            SetWaitItemsActive(itemNum, false);

            //��j��J�n
            StartCoroutine(piecesMgr.StartDestroyingPieces());

            //�A�C�e���g�p�t���O���Z�b�g
            NOW_SUPPORT_ITEM_USE = false;
        }

        /// <summary>
        /// �A�q���̎g�p
        /// </summary>
        /// <param name="_lineNum">�w��s</param>
        /// <returns></returns>
        IEnumerator UseDuck(int _lineNum)
        {
            mDuckSupportLineNum = _lineNum;
            int duckNum = (int)SupportItems.Duck;

            //�z�u���W�w��
            Vector2 setPos = new Vector2(0.0f, -SQUARE_DISTANCE * _lineNum);
            mItemsInfoArr[duckNum].tra.position = setPos;

            //�A�j���Đ�
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[duckNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(duckNum, false);

            //��j��J�n
            yield return StartCoroutine(piecesMgr.StartDestroyingPieces());
        }

        /// <summary>
        /// �A�q���̉���
        /// </summary>
        /// <param name="_attackColumn">�U����ԍ�</param>
        public void DuckSupport(int _attackColumn)
        {
            int squareIndex = mDuckSupportLineNum + (_attackColumn * BOARD_LINE_COUNT);
            piecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID);
        }
    }
}