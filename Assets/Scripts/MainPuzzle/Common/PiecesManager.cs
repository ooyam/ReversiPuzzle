using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static CommonDefine;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static ObjectMove.ObjectMove_2D;
using static Sound.SoundManager;

namespace PuzzleMain
{
    public class PiecesManager : MonoBehaviour
    {
        [Header("��v���n�u�̎擾")]
        [SerializeField]
        GameObject[] piecePrefabArr;

        [Header("����v���n�u�̎擾")]
        [SerializeField]
        GameObject blackPiecePrefab;

        Transform[]  pieceTraArr;       //��Transform�z��
        GameObject[] nextPieceObjArr;   //�ҋ@��I�u�W�F�N�g�z��
        Transform[]  nextPieceTraArr;   //�ҋ@��Transform�z��

        int nextPuPieceIndex = 0;       //���ɒu����̊Ǘ��ԍ�

        //==========================================================//
        //----------------------�����ݒ�,�擾-----------------------//
        //==========================================================//

        /// <summary>
        /// PiecesManager�̏�����
        /// </summary>
        public void Initialize()
        {
            pieceTraArr   = new Transform[SQUARES_COUNT];
            sPieceObjArr  = new GameObject[SQUARES_COUNT];
            sPieceInfoArr = new PieceInformation[SQUARES_COUNT];

            //�M�~�b�N���ݒ�i��Ƃ��ĊǗ�����j
            List<int> notPlaceIndex = new List<int>();  //���z�u���Ȃ��}�X�ԍ�
            for (int i = 0; i < GIMMICKS_DEPLOY_COUNT; i++)
            {
                //��Ƃ��ĊǗ�����M�~�b�N
                if (GIMMICKS_DATA.dataArray[GIMMICKS_INFO_ARR[i][SET_GMCK_TYPE]].In_Square)
                {
                    SetGimmicksInfomation(i, GIMMICKS_INFO_ARR[i][SET_GMCK_SQUARE], true);
                    notPlaceIndex.Add(GIMMICKS_INFO_ARR[i][SET_GMCK_SQUARE]);
                }
            }

            //��̃����_���z�u
            for (int i = 0; i < SQUARES_COUNT; i++)
            {
                if (!sSquareObjArr[i].activeSelf) continue; //��\���}�X�͏������΂�
                if (notPlaceIndex.Contains(i)) continue;    //�M�~�b�N�}�X�͏������΂�

                int pieceGeneIndex = GetRandomPieceColor();
                GeneratePiece(pieceGeneIndex, i, true);
            }

            //�ҋ@���
            nextPieceObjArr = new GameObject[sNextPiecesCount];
            nextPieceTraArr = new Transform[sNextPiecesCount];
            for (int i = 0; i < sNextPiecesCount; i++)
            {
                int pieceGeneIndex = GetRandomPieceColor();
                GenerateNextPiece(pieceGeneIndex, i);
            }
        }

        //==========================================================//


        //==========================================================//
        //-----------------------�ėp����---------------------------//
        //==========================================================//

        /// <summary>
        /// ��쐬
        /// </summary>
        /// /// <param name="prefabIndex">   ������v���n�u�ԍ�</param>
        /// /// <param name="pieceIndex">    ��Ǘ��ԍ�</param>
        /// /// <param name="startGenerate"> ��������</param>
        void GeneratePiece(int prefabIndex, int pieceIndex, bool startGenerate = false)
        {
            if(prefabIndex == COLORLESS_ID) sPieceObjArr[pieceIndex] = Instantiate(blackPiecePrefab);
            else sPieceObjArr[pieceIndex] = Instantiate(piecePrefabArr[prefabIndex]);
            sPieceInfoArr[pieceIndex] = sPieceObjArr[pieceIndex].GetComponent<PieceInformation>();
            sPieceInfoArr[pieceIndex].InformationSetting(pieceIndex, startGenerate, sGimmickInfoArr);
            pieceTraArr[pieceIndex] = sPieceInfoArr[pieceIndex].tra;
            pieceTraArr[pieceIndex].SetParent(sSquareTraArr[pieceIndex], false);
            pieceTraArr[pieceIndex].SetSiblingIndex(0);
        }

        /// <summary>
        /// �ҋ@��쐬
        /// </summary>
        /// /// <param name="prefabIndex">������v���n�u�ԍ�</param>
        /// /// <param name="pieceIndex"> �ҋ@��Ǘ��ԍ�</param>
        void GenerateNextPiece(int prefabIndex, int pieceIndex)
        {
            nextPieceObjArr[pieceIndex] = Instantiate(piecePrefabArr[prefabIndex]);
            nextPieceTraArr[pieceIndex] = nextPieceObjArr[pieceIndex].transform;
            nextPieceTraArr[pieceIndex].SetParent(sNextPieceBoxTraArr[pieceIndex], false);
        }

        /// <summary>
        /// ��폜
        /// </summary>
        /// /// <param name="pieceIndex">�폜��̊Ǘ��ԍ�</param>
        void DeletePiece(int pieceIndex)
        {
            Destroy(sPieceObjArr[pieceIndex]);
            sPieceObjArr[pieceIndex]  = null;
            pieceTraArr[pieceIndex]   = null;
            sPieceInfoArr[pieceIndex] = null;
        }

        /// <summary>
        /// �ҋ@���Ֆʂɒu��
        /// </summary>
        /// <param name="squareId">�z�u�}�X�Ǘ��ԍ�</param>
        void PutPiece(int squareId)
        {
            //SE�Đ�
            SE_OneShot(SE_Type.PiecePut);

            //�Ֆʂ̋�폜,�Ǘ��z�񍷂��ւ�
            DeletePiece(squareId);
            sPieceObjArr[squareId]  = nextPieceObjArr[nextPuPieceIndex];
            pieceTraArr[squareId]   = nextPieceTraArr[nextPuPieceIndex];
            sPieceInfoArr[squareId] = sPieceObjArr[squareId].GetComponent<PieceInformation>();
            sPieceInfoArr[squareId].InformationSetting(squareId, false);

            //�ҋ@���
            int pieceGeneIndex = GetRandomPieceColor();
            GenerateNextPiece(pieceGeneIndex, nextPuPieceIndex);
        }

        /// <summary>
        /// �M�~�b�N���ݒ�i��Ƃ��ĊǗ�����j
        /// </summary>
        /// /// <param name="gimmickIndex"> �M�~�b�N�Ǘ��ԍ�</param>
        void SetGimmicksInfomation(int gimmickIndex, int squareId, bool startGenerate)
        {
            //��Ƃ��Ă��Ǘ�����
            sPieceObjArr[squareId] = sGimmickObjArr[gimmickIndex];
            pieceTraArr[squareId] = sGimmickInfoArr[gimmickIndex].tra;
            pieceTraArr[squareId].SetParent(sSquareTraArr[squareId], false);
            pieceTraArr[squareId].SetSiblingIndex(0);
            pieceTraArr[squareId].localPosition = sGimmickInfoArr[gimmickIndex].defaultPos;
            sGimmickInfoArr[gimmickIndex].OperationFlagSetting(squareId, startGenerate, sGimmickInfoArr);
        }

        /// <summary>
        /// �}�X�Ƃ��ĊǗ����Ȃ��M�~�b�N�z�u
        /// </summary>
        /// <param name="gimmickObj">�z�u�I�u�W�F�N�g</param>
        /// <param name="squareId">  �z�u�}�X�Ǘ��ԍ�</param>
        public void PlaceGimmick(GameObject gimmickObj, int squareId)
        {
            gimmickObj.transform.SetParent(sSquareTraArr[squareId], false);
        }

        /// <summary>
        /// �M�~�b�N�폜
        /// </summary>
        /// /// <param name="gimmickObj">�폜��</param>
        public void DeleteGimmick(GameObject gimmickObj)
        {
            sGimmickObjArr[GimmicksMgr.GetGimmickIndex_Obj(gimmickObj)] = null;

            DeletePiece(Array.IndexOf(sPieceObjArr, gimmickObj));
        }

        /// <summary>
        /// �Ǘ��ԍ��̍X�V
        /// </summary>
        /// <param name="oldIndex">�X�V�O�̊Ǘ��ԍ�</param>
        /// <param name="newIndex">�X�V��̊Ǘ��ԍ�</param>
        void UpdateManagementIndex(int oldIndex, int newIndex)
        {
            sPieceObjArr[newIndex]  = sPieceObjArr[oldIndex];
            pieceTraArr[newIndex]   = pieceTraArr[oldIndex];
            sPieceInfoArr[newIndex] = sPieceInfoArr[oldIndex];
            pieceTraArr[newIndex].SetParent(sSquareTraArr[newIndex], true);
            pieceTraArr[newIndex].SetSiblingIndex(0);
            sPieceObjArr[oldIndex]  = null;
            pieceTraArr[oldIndex]   = null;
            sPieceInfoArr[oldIndex] = null;
            if (sPieceInfoArr[newIndex] != null) sPieceInfoArr[newIndex].squareId = newIndex;

            int gimIndex = GimmicksMgr.GetGimmickIndex_Square(newIndex);
            if (gimIndex >= 0) sGimmickInfoArr[gimIndex].nowSquareId = newIndex;
        }

        /// <summary>
        /// ��̑���\�t���O�ؑ�
        /// </summary>
        /// <param name="squareId">�}�X�Ǘ��ԍ�</param>
        /// <param name="on">      true:����\�ɂ���</param>
        public void PieceOperationFlagChange(int squareId, bool on)
        {
            //��}�X�͏������Ȃ�
            if (!SquaresMgr.IsSquareExists(squareId)) return;
            if (!SquaresMgr.IsSquareActive(squareId)) return;

            //��
            if (sPieceObjArr[squareId].CompareTag(PIECE_TAG))
            {
                if (on) sPieceInfoArr[squareId].OperationFlagON();
                else sPieceInfoArr[squareId].OperationFlagOFF();
            }
            //�M�~�b�N
            else
            {
                foreach (GimmickInformation gimInfo in sGimmickInfoArr)
                {
                    if (gimInfo == null) continue;
                    if (gimInfo.startSquareId == squareId && gimInfo.inSquare)
                    {
                        if (on) gimInfo.OperationFlagON();
                        else gimInfo.OperationFlagOFF();
                    }
                }
            }
        }

        /// <summary>
        /// ��̃����_���F�擾
        /// </summary>
        /// <param name="exclusionColor">���O�F</param>
        /// <returns>��̃����_���ȃv���n�u�̃C���f�b�N�X</returns>
        public int GetRandomPieceColor(int exclusionColor = COLORLESS_ID)
        {
            int returnColor = USE_COLOR_TYPE_ARR[UnityEngine.Random.Range(0, USE_COLOR_COUNT)];
            if (exclusionColor != COLORLESS_ID)
            {
                //���O�F�Ɠ��F�������ꍇ��10��܂ōĎ��s����
                for (int i = 0; i < 10; i++)
                {
                    if (exclusionColor != returnColor) break;
                    else returnColor = USE_COLOR_TYPE_ARR[UnityEngine.Random.Range(0, USE_COLOR_COUNT)];
                }
            }
            return returnColor;
        }

        /// <summary>
        /// �}�X�ɂ����̐FID�擾
        /// </summary>
        /// <param name="squareId">�}�X�Ǘ��ԍ�</param>
        /// <returns>�FID</returns>
        public int GetSquarePieceColorId(int squareId)
        {
            if (sPieceInfoArr[squareId] == null) return INT_NULL;
            return sPieceInfoArr[squareId].colorId;
        }

        //==========================================================//



        //==========================================================//
        //-----------------------��z�u����-------------------------//
        //==========================================================//

        /// <summary>
        /// �I�u�W�F�N�g���^�b�v���ꂽ
        /// </summary>
        /// <param name="tapObj"></param>
        public void TapObject(GameObject tapObj)
        {
            //����A�C�e���������ȊO�̓M�~�b�N���^�b�v�ł��Ȃ�
            if (tapObj.CompareTag(GIMMICK_TAG) && !GetFlag(PuzzleFlag.NowSupportItemReady)) return;

            //�I�u�W�F�N�g�ԍ��擾
            int pieceObjIndex = Array.IndexOf(sPieceObjArr, tapObj);
            int nextPieceObjIndex = Array.IndexOf(nextPieceObjArr, tapObj);

            //�Տ�̋�̏ꍇ
            if (pieceObjIndex >= 0)
            {
                //����A�C�e���������̏ꍇ
                if (GetFlag(PuzzleFlag.NowSupportItemReady))
                {
                    StartCoroutine(SupportItemsMgr.UseItems(pieceObjIndex));
                }
                //��]�t���O�m�F
                else if (sPieceInfoArr[pieceObjIndex].invertable)
                {
                    StartCoroutine(PutPieceToSquare(tapObj));
                }
            }
            //�ҋ@��̏ꍇ
            else if (nextPieceObjIndex >= 0)
            {
                //����A�C�e���������̏ꍇ�͉���
                if (GetFlag(PuzzleFlag.NowSupportItemReady))
                {
                    SupportItemsMgr.ResetWaitItemReady();
                }
                nextPuPieceIndex = nextPieceObjIndex;
                SquaresMgr.MoveNextPieceFrame(nextPuPieceIndex);
            }
        }

        /// <summary>
        /// �w��}�X�̃_���[�W
        /// </summary>
        /// <param name="squareIndex">   �}�X�ԍ�</param>
        /// <param name="reversiColorId">���]��̐FID</param>
        /// <param name="instantly">     �P�́E���j��</param>
        /// <param name="assault">       ����</param>
        /// <param name="stateAddName">  �X�e�[�g���ǉ�������</param>
        public void DamageSpecifiedSquare(int squareIndex, int reversiColorId, bool instantly, bool assault = false, string stateAddName = "")
        {
            if (sPieceObjArr[squareIndex] == null) return;   //��}�X

            //�M�~�b�N
            if (sPieceObjArr[squareIndex].CompareTag(GIMMICK_TAG))
            {
                //�_���[�W����
                int gimmickIndex = GimmicksMgr.GetGimmickIndex_Square(squareIndex);
                bool damage = GimmicksMgr.DamageCheck(ref sGimmickInfoArr[gimmickIndex].colorId, ref gimmickIndex, assault);

                //�_���[�W�L�̏ꍇ
                if (damage) GimmicksMgr.DamageGimmick(ref gimmickIndex, squareIndex, stateAddName);
            }
            //��
            else
            {
                //���]�\����
                if (sPieceInfoArr[squareIndex].invertable)
                {
                    //���j��łȂ��ꍇ
                    if (!instantly)
                    {
                        //���]�J�n
                        StartCoroutine(ReversingPieces(squareIndex, reversiColorId));

                        //��]�J�E���g
                        PieceReverseCount(sPieceInfoArr[squareIndex].colorId);
                    }
                    sDestroyPiecesIndexList.Add(squareIndex);
                }
            }

            //���j��
            if (instantly)
            {
                foreach (int i in sDestroyPiecesIndexList)
                { StartCoroutine(StratReversingPiece(reversiColorId, i)); }

                //�j�󃊃X�g���Z�b�g
                sDestroyPiecesIndexList = new List<int>();
            }
        }

        /// <summary>
        /// �}�X�ɋ��u��
        /// </summary>
        /// <param name="deletePiece">�폜��</param>
        IEnumerator PutPieceToSquare(GameObject deletePiece)
        {
            //�ړ�SE�Đ�
            SE_OneShot(SE_Type.PieceMove);

            //�^�[��������
            TurnMgr.TurnDecrease();

            //�z�u���t���O�Z�b�g
            FlagOn(PuzzleFlag.NowPuttingPieces);

            //�폜�����̃}�X�Ɏw�蒆�̑ҋ@����Z�b�g����
            int putIndex = Array.IndexOf(sPieceObjArr, deletePiece);
            nextPieceTraArr[nextPuPieceIndex].SetParent(sSquareTraArr[putIndex], true);
            nextPieceTraArr[nextPuPieceIndex].SetSiblingIndex(0);

            //��g��
            Coroutine scaleUpCoroutine = StartCoroutine(AllScaleChange(nextPieceTraArr[nextPuPieceIndex], PUT_PIECE_SCALING_SPEED, PUT_PIECE_CHANGE_SCALE));

            //�ҋ@��̈ړ�
            Vector3 nowPos = nextPieceTraArr[nextPuPieceIndex].localPosition;
            nextPieceTraArr[nextPuPieceIndex].localPosition = new Vector3(nowPos.x, nowPos.y, PUT_PIECE_MOVE_START_Z);
            yield return StartCoroutine(DecelerationMovement(nextPieceTraArr[nextPuPieceIndex], PUT_PIECE_MOVE_SPEED, PIECE_DEFAULT_POS));

            //��]�J�E���g
            PieceReverseCount(sPieceInfoArr[putIndex].colorId);

            //�ҋ@���u��
            PutPiece(putIndex);

            //90����]
            nextPieceTraArr[nextPuPieceIndex].localRotation = PIECE_GENERATE_QUEST;
            StartCoroutine(RotateMovement(nextPieceTraArr[nextPuPieceIndex], REVERSE_PIECE_ROT_SPEED, REVERSE_PIECE_FRONT_ROT));

            //��k��
            StopCoroutine(scaleUpCoroutine);
            yield return StartCoroutine(AllScaleChange(pieceTraArr[putIndex], PUT_PIECE_SCALING_SPEED, PIECE_DEFAULT_SCALE));
            pieceTraArr[putIndex].localScale = new Vector2(PIECE_DEFAULT_SCALE, PIECE_DEFAULT_SCALE);

            //���]��X�g�擾
            int colorId = sPieceInfoArr[putIndex].colorId;      //�u������̐F�ԍ��擾
            List<int[]> reversIndexList = new List<int[]>();    //���]��̊Ǘ��ԍ��i�[���X�g
            if (GetReversIndex(putIndex, ref colorId, ref reversIndexList))
            {
                //�폜�Ώۂɒu������̊Ǘ��ԍ��ǉ�
                sDestroyPiecesIndexList.Insert(sDestroyBasePieceIndex, putIndex);

                //���]�J�n
                StartCoroutine(StratReversingPieces(colorId, reversIndexList));
            }
            else
            {
                //�^�[���I�������J�n
                StartCoroutine(TurnMgr.TurnEnd());
            }

            //�z�u���t���O���Z�b�g
            FlagOff(PuzzleFlag.NowPuttingPieces);
        }

        /// <summary>
        /// ��̔��]����
        /// </summary>
        /// <param name="putPieceIndex">  �u������̊Ǘ��ԍ�</param>
        /// <param name="putPieceColorId">�u������̐F�ԍ�</param>
        /// <param name="reversIndexList">���]��i�[���X�g(�Q��)</param>
        /// <returns>���]��̗L��</returns>
        bool GetReversIndex(int putPieceIndex, ref int putPieceColorId, ref List<int[]> reversIndexList)
        {
            //���^�O�ԍ��擾
            int sqrCountUp    = SquaresMgr.GetLineNumber(putPieceIndex);    //�u������̏�ɂ���}�X�̐�
            int sqrCountLeft  = SquaresMgr.GetColumnNumber(putPieceIndex);  //�u������̍��ɂ���}�X�̐�
            int sqrCountDown  = BOARD_LINE_COUNT - sqrCountUp - 1;          //�u������̉��ɂ���}�X�̐�
            int sqrCountRight = BOARD_COLUMN_COUNT - sqrCountLeft - 1;      //�u������̉E�ɂ���}�X�̐�

            //���]����������Ԃ̐ݒ�
            int[] loopCounts = new int[DIRECTIONS_COUNT];
            int[][] dummyArr = new int[DIRECTIONS_COUNT][];
            foreach (Directions directions in Enum.GetValues(typeof(Directions)))
            {
                //�e�������̏����񐔐ݒ�
                loopCounts[(int)directions] = SquaresMgr.SetLoopCount(directions, ref sqrCountUp, ref sqrCountRight, ref sqrCountDown, ref sqrCountLeft);

                //���Ԕ���M�~�b�N�����݂��邩�H
                dummyArr[(int)directions] = new int[] { (int)directions, GetOrderIndex(ref putPieceIndex, ref loopCounts[(int)directions], (int)directions) };
            }

            //���ԃM�~�b�N�������Ƀ\�[�g
            const int ARRAY_INDEX = 0;    //�z��ԍ��i�[index
            const int ORDER_INDEX = 1;    //���Ԕԍ��i�[index
            int[] dirOrder = new int[DIRECTIONS_COUNT];
            for (int i = 0; i < DIRECTIONS_COUNT; i++)
            {
                int[] minOrder = null;
                int useIndex = -1;
                int n = -1;
                foreach (int[] arr in dummyArr)
                {
                    n++;
                    if (arr == null) continue;
                    if (minOrder == null)
                    {
                        minOrder = arr;
                        useIndex = n;
                    }
                    else if (minOrder[ORDER_INDEX] > arr[ORDER_INDEX])
                    {
                        minOrder = arr;
                        useIndex = n;
                    }
                }
                dirOrder[i] = minOrder[ARRAY_INDEX];
                dummyArr[useIndex] = null;
            }

            //�w������̔��]�I�u�W�F�N�g�̊Ǘ��ԍ��擾
            foreach (int dir in dirOrder)
            {
                if (loopCounts[dir] == 0) continue;
                reversIndexList.Add(GetReversIndex_SpecifiedDirection(ref putPieceIndex, ref putPieceColorId, ref loopCounts[dir], dir));
            }

            //null�����ׂč폜
            reversIndexList.RemoveAll(item => item == null);
            return reversIndexList.Count > 0;
        }

        /// <summary>
        /// �w������̏��ԃM�~�b�N�ԍ��擾
        /// </summary>
        /// <param name="putPieceIndex">  ���̊Ǘ��ԍ�</param>
        /// <param name="loopCount">      �w������ɂ���}�X�̐�</param>
        /// <param name="direction">      �w������̊Ǘ��ԍ�</param>
        /// <returns>�w������̔��]�I�u�W�F�N�g�̊Ǘ��ԍ��z��</returns>
        int GetOrderIndex(ref int putPieceIndex, ref int loopCount, int direction)
        {
            if (loopCount == 0) return NOT_NUM;
            for (int i = 1; i <= loopCount; i++)
            {
                //�w������̃C���f�b�N�X�ԍ��擾
                int refIndex = SquaresMgr.GetDesignatedDirectionIndex(direction, putPieceIndex, i);

                //��}�X�̏ꍇ��null��Ԃ�
                if (sPieceObjArr[refIndex] == null) return NOT_NUM;

                //�M�~�b�N�}�X�̏ꍇ
                if (sPieceObjArr[refIndex].CompareTag(GIMMICK_TAG))
                {
                    //���Ԏw�肪����ꍇ
                    int gimmickIndex = GimmicksMgr.GetGimmickIndex_Square(refIndex);
                    if (sGimmickInfoArr[gimmickIndex].order != NOT_NUM)
                        return sGimmickInfoArr[gimmickIndex].order;
                }
            }
            return NOT_NUM;
        }

        /// <summary>
        /// �w������̔��]�I�u�W�F�N�g�̊Ǘ��ԍ��擾
        /// </summary>
        /// <param name="putPieceIndex">  ���̊Ǘ��ԍ�</param>
        /// <param name="putPieceColorId">���̐F�ԍ�</param>
        /// <param name="loopCount">      �w������ɂ���}�X�̐�</param>
        /// <param name="direction">      �w������̊Ǘ��ԍ�</param>
        /// <returns>�w������̔��]�I�u�W�F�N�g�̊Ǘ��ԍ��z��</returns>
        int[] GetReversIndex_SpecifiedDirection(ref int putPieceIndex, ref int putPieceColorId, ref int loopCount, int direction)
        {
            List<int> reversIndexList = new List<int>();
            int orderCount = 0; //���ԃM�~�b�N���J�E���g������(���]���Ȃ��Ɣ��肵���ꍇ�ɃJ�E���g��߂�����)
            for (int i = 1; i <= loopCount; i++)
            {
                //�w������̃C���f�b�N�X�ԍ��擾
                int refIndex = SquaresMgr.GetDesignatedDirectionIndex(direction, putPieceIndex, i);

                //��}�X�̏ꍇ��null��Ԃ�
                if (sPieceObjArr[refIndex] == null) break;

                //�M�~�b�N�}�X�̏ꍇ
                if (sPieceObjArr[refIndex].CompareTag(GIMMICK_TAG))
                {
                    //�_���[�W��^�����邩�̊m�F,�_���[�W���^�����Ȃ��ꍇ��null��Ԃ�
                    int gimmickIndex = GimmicksMgr.GetGimmickIndex_Square(refIndex);
                    if (!sGimmickInfoArr[gimmickIndex].destructible_Piece) break;
                    if (!GimmicksMgr.DamageCheck(ref putPieceColorId, ref gimmickIndex)) break;
                    if (sGimmickInfoArr[gimmickIndex].order != NOT_NUM) orderCount++;    //���Ԕ����ʉ߂����ꍇ�̓J�E���g
                    reversIndexList.Add(refIndex);
                }
                else
                {
                    //���]�֎~��̏ꍇ��null��Ԃ�
                    if (!sPieceInfoArr[refIndex].invertable) break;

                    //���F�������
                    if (sPieceInfoArr[refIndex].colorId == putPieceColorId)
                    {
                        //�ׂ����^�O�̏ꍇ��null��Ԃ�
                        if (i == 1) break;

                        //�폜�Ώۂɓ��F��̊Ǘ��ԍ��ǉ�
                        sDestroyPiecesIndexList.Add(refIndex);

                        //���]���X�g��Ԃ�(����)
                        return reversIndexList.ToArray();
                    }
                    reversIndexList.Add(refIndex);
                }
            }

            //���ԃM�~�b�N�̃J�E���g��߂�
            sNumberTagNextOrder -= orderCount;

            //null��Ԃ�(���s)
            return null;
        }
        //==========================================================//


        //==========================================================//
        //-----------------------��]����-------------------------//
        //==========================================================//

        /// <summary>
        /// ��]�J�n(�P��)
        /// </summary>
        /// <param name="putPieceColorId">�u������̐F�ԍ�</param>
        /// <param name="reversIndex">    ���]��Ǘ��ԍ�</param>
        /// <returns></returns>
        IEnumerator StratReversingPiece(int putPieceColorId, int reversIndex)
        {
            //���]���t���O�Z�b�g
            FlagOn(PuzzleFlag.NowReversingPieces);

            //�M�~�b�N
            if (sPieceObjArr[reversIndex].CompareTag(GIMMICK_TAG))
            {
                //�M�~�b�N�I���ҋ@
                yield return sGimmickCorList[sGimmickCorList.Count - 1];
            }
            //��
            else
            {
                //��]�J�E���g
                PieceReverseCount(sPieceInfoArr[reversIndex].colorId);

                //���]
                yield return StartCoroutine(ReversingPieces(reversIndex, putPieceColorId));
            }

            //���]��̔j��
            StartCoroutine(StartDestroyingPiece(reversIndex));

            //���]���t���O���Z�b�g
            FlagOff(PuzzleFlag.NowReversingPieces);
        }

        /// <summary>
        /// ��]�J�n(����)
        /// </summary>
        /// <param name="putPieceColorId">�u������̐F�ԍ�</param>
        /// <param name="reversIndexList">���]��i�[���X�g</param>
        /// <returns></returns>
        IEnumerator StratReversingPieces(int putPieceColorId, List<int[]> reversIndexList)
        {
            //���]���t���O�Z�b�g
            FlagOn(PuzzleFlag.NowReversingPieces);

            //���]�J�n
            Coroutine coroutine = null;
            foreach (int[] reversIndexArr in reversIndexList)
            {
                if (reversIndexArr == null) continue;
                foreach (int reversIndex in reversIndexArr)
                {
                    //�M�~�b�N�ł���΃M�~�b�N�j�󓮍�Ɉڂ�
                    int gimmickIndex = GimmicksMgr.GetGimmickIndex_Square(reversIndex);
                    if (gimmickIndex >= 0)
                    {
                        GimmicksMgr.DamageGimmick(ref gimmickIndex, reversIndex);
                        yield return PIECE_REVERSAL_INTERVAL;
                        continue;
                    }

                    //�폜�Ώۂɔ��]��̊Ǘ��ԍ��ǉ�
                    sDestroyPiecesIndexList.Add(reversIndex);

                    //��]�J�E���g
                    PieceReverseCount(sPieceInfoArr[reversIndex].colorId);

                    //���]
                    coroutine = StartCoroutine(ReversingPieces(reversIndex, putPieceColorId));

                    yield return PIECE_REVERSAL_INTERVAL;
                }

                //��]�J�E���g(���]���Ȃ��Ō�̋���ǉ�)
                PieceReverseCount(putPieceColorId);

                yield return PIECE_GROUP_REVERSAL_INTERVAL;
            }

            //�M�~�b�N�I���ҋ@
            foreach (Coroutine c in sGimmickCorList)
            { yield return c; }

            //���]�I���ҋ@
            yield return coroutine;

            //�F�g�j��m�F
            yield return StartCoroutine(GimmicksMgr.DestroyFrame());

            //���]��̔j��
            StartCoroutine(StartDestroyingPieces());

            //���]���t���O���Z�b�g
            FlagOff(PuzzleFlag.NowReversingPieces);
        }

        /// <summary>
        /// ��̔��]
        /// </summary>
        /// <param name="reversPieceIndex">���Ԃ���̊Ǘ��ԍ�</param>
        /// <param name="generateColorId"> ������̐F�ԍ�</param>
        public IEnumerator ReversingPieces(int reversPieceIndex, int generateColorId)
        {
            //�w��C���f�b�N�X���������Ȃ��ꍇ�͋����I��
            if (pieceTraArr[reversPieceIndex] == null) yield break;

            //��90����],�g��
            StartCoroutine(AllScaleChange(pieceTraArr[reversPieceIndex], REVERSE_PIECE_SCALING_SPEED, REVERSE_PIECE_CHANGE_SCALE));
            yield return StartCoroutine(RotateMovement(pieceTraArr[reversPieceIndex], REVERSE_PIECE_ROT_SPEED, REVERSE_PIECE_SWITCH_ROT));

            //����폜,�V���
            DeletePiece(reversPieceIndex);
            GeneratePiece(generateColorId, reversPieceIndex);

            //SE�Đ�
            SE_OneShot(SE_Type.PiecePut);

            //��90����],�k��
            pieceTraArr[reversPieceIndex].localScale    = new Vector3(REVERSE_PIECE_CHANGE_SCALE, REVERSE_PIECE_CHANGE_SCALE, 0.0f);
            pieceTraArr[reversPieceIndex].localRotation = PIECE_GENERATE_QUEST;
            StartCoroutine(AllScaleChange(pieceTraArr[reversPieceIndex], REVERSE_PIECE_SCALING_SPEED, PIECE_DEFAULT_SCALE));
            yield return StartCoroutine(RotateMovement(pieceTraArr[reversPieceIndex], REVERSE_PIECE_ROT_SPEED, REVERSE_PIECE_FRONT_ROT));
        }

        /// <summary>
        /// ��]���̃J�E���g����
        /// </summary>
        /// <param name="colorId">�F�ԍ�</param>
        void PieceReverseCount(int colorId)
        {
            //��J�E���g�M�~�b�N�Ƀ_���[�W(���]��)
            StartCoroutine(GimmicksMgr.DamageCage(colorId));    //�B

            //�ڕW�����m�F
            TargetMgr.TargetDecreaseCheck(colorId);
        }

        //==========================================================//



        //==========================================================//
        //-----------------------��j�󓮍�-------------------------//
        //==========================================================//

        /// <summary>
        /// ��j��J�n(�P��)
        /// </summary>
        /// <param name="destroyIndex">��Ǘ��ԍ�</param>
        public IEnumerator StartDestroyingPiece(int destroyIndex)
        {
            //�j�󒆃t���O�Z�b�g
            FlagOn(PuzzleFlag.NowDestroyingPieces);

            //��k��
            if (sPieceObjArr[destroyIndex].CompareTag(PIECE_TAG))
            yield return StartCoroutine(AllScaleChange(pieceTraArr[destroyIndex], DESTROY_PIECE_SCALING_SPEED, DESTROY_PIECE_CHANGE_SCALE));

            //��폜
            GameObject obj = sPieceObjArr[destroyIndex];
            if (obj.CompareTag(GIMMICK_TAG)) DeleteGimmick(obj);
            else DeletePiece(destroyIndex);

            //��̗����J�n
            yield return StartCoroutine(StratFallingPieces());

            //�j�󒆃t���O���Z�b�g
            FlagOff(PuzzleFlag.NowDestroyingPieces);
        }

        /// <summary>
        /// ��j��J�n(����)
        /// </summary>
        /// <param name="useSupport">����A�C�e���g�p�H</param>
        public IEnumerator StartDestroyingPieces(bool useSupport = false)
        {
            //�j�󒆃t���O�Z�b�g
            FlagOn(PuzzleFlag.NowDestroyingPieces);

            //��k��
            Coroutine coroutine = null;
            foreach (int index in sDestroyPiecesIndexList)
            {
                if (sPieceObjArr[index].CompareTag(GIMMICK_TAG)) continue;
                coroutine = StartCoroutine(AllScaleChange(pieceTraArr[index], DESTROY_PIECE_SCALING_SPEED, DESTROY_PIECE_CHANGE_SCALE));
            }
            yield return coroutine;
            
            //����A�C�e������
           �@if (!useSupport) SupportItemsMgr.GenerateItems();

            //��폜
            foreach (int pieceIndex in sDestroyPiecesIndexList)
            {
                GameObject obj = sPieceObjArr[pieceIndex];
                if (obj == null) continue;
                if (obj.CompareTag(GIMMICK_TAG)) DeleteGimmick(obj);
                else DeletePiece(pieceIndex);
            }

            //�폜��X�g�̏�����
            sDestroyPiecesIndexList = new List<int>();

            //��̗����J�n
            yield return StartCoroutine(StratFallingPieces());

            //�^�[���I��
            StartCoroutine(TurnMgr.TurnEnd(useSupport));

            //�j�󒆃t���O���Z�b�g
            FlagOff(PuzzleFlag.NowDestroyingPieces);
        }

        /// <summary>
        /// ��̗����J�n
        /// </summary>
        public IEnumerator StratFallingPieces()
        {
            //������t���O�Z�b�g
            FlagOn(PuzzleFlag.NowFallingPieces);

            //�S��Ǘ��ԍ��̍X�V,������X�g�̎擾
            List<int> fallPiecesIndexList = SettingOfFallingPieces();

            //�����J�n
            List<Coroutine> coroutineList = new List<Coroutine>();
            foreach (int fallPieceIndex in fallPiecesIndexList)
            {
                if (pieceTraArr[fallPieceIndex] != null)
                {
                    Vector3 targetPos = PIECE_DEFAULT_POS;
                    int gimmickIndex = GimmicksMgr.GetGimmickIndex_Square(fallPieceIndex);
                    if (gimmickIndex >= 0) targetPos = sGimmickInfoArr[gimmickIndex].defaultPos;
                    coroutineList.Add(StartCoroutine(ConstantSpeedMovement(pieceTraArr[fallPieceIndex], FALL_PIECE_MOVE_SPEED, targetPos, FALL_PIECE_ACCELE_RATE)));
                }
            }
            foreach (Coroutine coroutine in coroutineList)
            { yield return coroutine; }

            //������t���O���Z�b�g
            FlagOff(PuzzleFlag.NowFallingPieces);
        }

        /// <summary>
        /// ������̐ݒ�
        /// </summary>
        /// <returns>������X�g</returns>
        List<int> SettingOfFallingPieces()
        {
            List<int> fallPiecesIndexList = new List<int>();
            for (int i = SQUARES_COUNT - 1; i >= 0; i--)
            {
                //��\���}�X�̏ꍇ�͏������X�L�b�v
                if (!sSquareObjArr[i].activeSelf) continue;

                //��}�X�łȂ���Ώ������X�L�b�v
                if (sPieceObjArr[i] != null) continue;

                //���������X�g�ɒǉ�
                fallPiecesIndexList.Add(i);

                //��̏�ɂ���Ǘ��ԍ������ׂĒ���
                int loopCount = SquaresMgr.GetLineNumber(i);
                for (int n = 0; n <= loopCount; n++)
                {
                    //���g���n��̔ԍ�����łȂ��ꍇ
                    int refIndex = i - n;
                    if (sPieceObjArr[refIndex] != null)
                    {
                        //���R�炩�\�I�u�W�F�N�g�̏ꍇ,�Ǘ��ԍ��X�V
                        if (FreeFallJudgement(ref sPieceObjArr[refIndex]))
                            UpdateManagementIndex(i - n, i);
                        break;
                    }

                    //�����ł����Տ�ɑ��݂��Ȃ��ꍇ
                    if (n == loopCount)
                    {
                        //�����M�~�b�N�̃����_������
                        int gimIndex = GimmicksMgr.GenerateFallGimmick();
                        if (gimIndex != INT_NULL)
                        {
                            //�����M�~�b�N�̋�Ǘ��ݒ�
                            SetGimmicksInfomation(gimIndex, i, false);

                            //�}�X�ԍ��w��
                            sGimmickInfoArr[gimIndex].nowSquareId = i;
                        }
                        //�M�~�b�N�̐������Ȃ������ꍇ
                        else
                        {
                            //�V�K�����_������
                            int prefabIndex = GetRandomPieceColor();
                            GeneratePiece(prefabIndex, i);
                        }

                        //�����J�n�ʒu�ɔz�u
                        pieceTraArr[i].localPosition = new Vector3(PIECE_DEFAULT_POS.x, BOARD_LINE_COUNT * SQUARE_DISTANCE, Z_PIECE);
                    }
                }
            }

            return fallPiecesIndexList;
        }

        /// <summary>
        /// ���R��������
        /// </summary>
        /// <param name="pieceObj"></param>
        /// <returns>true:���R����</returns>
        bool FreeFallJudgement(ref GameObject pieceObj)
        {
            if (pieceObj.CompareTag(PIECE_TAG))
            {
                //��̏ꍇ
                int pieceObjIndex = Array.IndexOf(sPieceObjArr, pieceObj);
                return sPieceInfoArr[pieceObjIndex].freeFall;
            }
            else if (pieceObj.CompareTag(GIMMICK_TAG))
            {
                //�M�~�b�N�̏ꍇ
                int gimmickObjIndex = GimmicksMgr.GetGimmickIndex_Obj(pieceObj);
                if (sGimmickInfoArr[gimmickObjIndex].inSquare && !sGimmickInfoArr[gimmickObjIndex].freeFall_Piece)
                {
                    return false;
                }
                return sGimmickInfoArr[gimmickObjIndex].freeFall;
            }

            return false;
        }
    }
}