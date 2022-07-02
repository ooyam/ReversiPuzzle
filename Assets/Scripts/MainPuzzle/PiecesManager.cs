using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static ObjectMove_2D.ObjectMove_2D;

namespace PuzzleMain
{
    public class PiecesManager : MonoBehaviour
    {
        GimmicksManager     gimmicksMgr;    //GimmicksManager
        SupportItemsManager stItemsMgr;     //SupportItemsManager

        [Header("��v���n�u�̎擾")]
        [SerializeField]
        GameObject[] piecePrefabArr;

        [Header("����v���n�u�̎擾")]
        [SerializeField]
        GameObject blackPiecePrefab;

        [Header("�M�~�b�N�v���n�u�̎擾")]
        public GimmickArr[] gimmickPrefabArr;
        [Serializable]
        public class GimmickArr
        { public GameObject[] prefab; }

        [Header("���o�[�V�Ղ̎擾")]
        [SerializeField]
        Transform reversiBoardTra;

        [Header("�ҋ@��{�b�N�X�̎擾")]
        [SerializeField]
        Transform nextPieceBoxesTra;

        Transform[]  pieceTraArr;               //��Transform�z��
        GameObject[] squareObjArr;              //�}�X�I�u�W�F�N�g�z��
        SpriteRenderer[] squareSpriRenArr;      //�}�XSpriteRenderer�z��
        GameObject[] nextPieceObjArr;           //�ҋ@��I�u�W�F�N�g�z��
        Transform[]  nextPieceTraArr;           //�ҋ@��Transform�z��
        Transform[]  nextPieceBoxTraArr;        //�ҋ@�Transform�z��
        Transform    nextPieceFrameTra;         //���ɒu���R�}�̎w��t���[��
        GameObject[] gimmickObjArr;             //�M�~�b�N�I�u�W�F�N�g�z��
        int nextPuPieceIndex = 0;               //���ɒu����̊Ǘ��ԍ�
        int nextPiecesCount;                    //�ҋ@��̌�

        //==========================================================//
        //----------------------�����ݒ�,�擾-----------------------//
        //==========================================================//

        /// <summary>
        /// PiecesManager�̏�����
        /// </summary>
        public void Initialize()
        {
            //���N���X�擾
            gimmicksMgr = sPuzzleMain.GetGimmicksManager();
            stItemsMgr = sPuzzleMain.GetSupportItemsManager();

            //�}�X�擾
            pieceTraArr         = new Transform[SQUARES_COUNT];
            sPieceObjArr        = new GameObject[SQUARES_COUNT];
            sPieceInfoArr       = new PieceInformation[SQUARES_COUNT];
            squareObjArr        = new GameObject[SQUARES_COUNT];
            sSquareTraArr       = new Transform[SQUARES_COUNT];
            squareSpriRenArr    = new SpriteRenderer[SQUARES_COUNT];
            for (int i = 0; i < SQUARES_COUNT; i++)
            {
                squareObjArr[i] = reversiBoardTra.GetChild(i).gameObject;
                sSquareTraArr[i] = squareObjArr[i].transform;
                squareSpriRenArr[i] = squareObjArr[i].GetComponent<SpriteRenderer>();
            }

            //�g�p���Ȃ��}�X���\��
            foreach (int i in HIDE_SQUARE_ARR)
            { squareObjArr[i].SetActive(false); }

            //�M�~�b�N��z�u
            gimmickObjArr  = new GameObject[GIMMICKS_COUNT];
            sGimmickInfoArr = new GimmickInformation[GIMMICKS_COUNT];
            gimmicksMgr.PlaceGimmickNotInSquare();      //��Ƃ��Ĕz�u���Ȃ��M�~�b�N�̐���
            List<int> notPlaceIndex = new List<int>();  //���z�u���Ȃ��}�X�ԍ�
            for (int i = 0; i < GIMMICKS_COUNT; i++)
            {
                //��Ƃ��ĊǗ�����M�~�b�N
                if (GIMMICKS_DATA.param[GIMMICKS_INFO_ARR[i][GIMMICK]].in_square)
                {
                    GeneraeGimmick(i);
                    notPlaceIndex.Add(GIMMICKS_INFO_ARR[i][SQUARE]);
                }
            }

            //��̃����_���z�u
            for (int i = 0; i < SQUARES_COUNT; i++)
            {
                if (!squareObjArr[i].activeSelf) continue; //��\���}�X�͏������΂�
                if (notPlaceIndex.Contains(i))   continue; //�M�~�b�N�}�X�͏������΂�

                int pieceGeneIndex = GetRandomPieceColor();
                GeneratePiece(pieceGeneIndex, i, true);
            }

            //�ҋ@��̔��擾
            nextPiecesCount    = nextPieceBoxesTra.childCount;
            nextPieceBoxTraArr = new Transform[nextPiecesCount];
            for (int i = 0; i < nextPiecesCount; i++)
            {
                nextPieceBoxTraArr[i] = nextPieceBoxesTra.GetChild(i).transform;
            }

            //���ɒu���R�}�̎w��t���[���擾
            nextPieceFrameTra = nextPieceBoxTraArr[0].GetChild(0).gameObject.transform;

            //�ҋ@���
            nextPieceObjArr = new GameObject[nextPiecesCount];
            nextPieceTraArr = new Transform[nextPiecesCount];
            for (int i = 0; i < nextPiecesCount; i++)
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
            nextPieceTraArr[pieceIndex].SetParent(nextPieceBoxTraArr[pieceIndex], false);
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
            //�Ֆʂ̋�폜,�Ǘ��z�񍷂��ւ�
            DeletePiece(squareId);
            sPieceObjArr[squareId]  = nextPieceObjArr[nextPuPieceIndex];
            pieceTraArr[squareId]  = nextPieceTraArr[nextPuPieceIndex];
            sPieceInfoArr[squareId] = sPieceObjArr[squareId].GetComponent<PieceInformation>();
            sPieceInfoArr[squareId].InformationSetting(squareId, false);

            //�ҋ@���
            int pieceGeneIndex = GetRandomPieceColor();
            GenerateNextPiece(pieceGeneIndex, nextPuPieceIndex);
        }

        /// <summary>
        /// �M�~�b�N�쐬(��Ƃ��ĊǗ�����)
        /// </summary>
        /// /// <param name="gimmickIndex"> �M�~�b�N�Ǘ��ԍ�</param>
        /// /// <param name="startGenerate">���������H</param>
        void GeneraeGimmick(int gimmickIndex, bool startGenerate = true)
        {
            int colorId = (GIMMICKS_INFO_ARR[gimmickIndex][COLOR] < 0) ? 0 : GIMMICKS_INFO_ARR[gimmickIndex][COLOR];
            gimmickObjArr[gimmickIndex] = Instantiate(gimmickPrefabArr[GIMMICKS_INFO_ARR[gimmickIndex][GIMMICK]].prefab[colorId]);

            //Component�擾
            sGimmickInfoArr[gimmickIndex] = gimmickObjArr[gimmickIndex].GetComponent<GimmickInformation>();
            sGimmickInfoArr[gimmickIndex].InformationSetting(gimmickIndex);

            //�ԍ��D�̏ꍇ
            if (sGimmickInfoArr[gimmickIndex].id == (int)Gimmicks.NumberTag)
                gimmicksMgr.NumberTagOrderSetting(ref sGimmickInfoArr[gimmickIndex]);

            //��Ƃ��Ă��Ǘ�����
            int pieceIndex = GIMMICKS_INFO_ARR[gimmickIndex][SQUARE];
            sPieceObjArr[pieceIndex] = gimmickObjArr[gimmickIndex];
            pieceTraArr[pieceIndex] = sGimmickInfoArr[gimmickIndex].tra;
            pieceTraArr[pieceIndex].SetParent(sSquareTraArr[GIMMICKS_INFO_ARR[gimmickIndex][SQUARE]], false);
            pieceTraArr[pieceIndex].SetSiblingIndex(0);
            pieceTraArr[pieceIndex].localPosition = sGimmickInfoArr[gimmickIndex].defaultPos;
            sGimmickInfoArr[gimmickIndex].OperationFlagSetting(pieceIndex, startGenerate, sGimmickInfoArr);
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
        /// /// <param name="pieceIndex">�폜��̊Ǘ��ԍ�</param>
        public void DeleteGimmick(GameObject gimmickObj)
        {
            int gimmickIndex = Array.IndexOf(gimmickObjArr, gimmickObj);
            gimmickObjArr[gimmickIndex] = null;

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

            int gimIndex = Array.IndexOf(gimmickObjArr, sPieceObjArr[newIndex]);
            if (gimIndex >= 0) sGimmickInfoArr[gimIndex].nowSquareId = newIndex;
        }

        /// <summary>
        /// �}�X�̐F�ύX
        /// </summary>
        /// <param name="afterColor"> �ω���̐F</param>
        /// <param name="squareId">   �}�X�Ǘ��ԍ�</param>
        /// <param name="fade">       �t�F�[�h�̗L��</param>
        public IEnumerator SquareColorChange(Color afterColor, int squareId, bool fade)
        {
            if (!fade) squareSpriRenArr[squareId].color = afterColor;
            else yield return StartCoroutine(SpriteRendererPaletteChange(squareSpriRenArr[squareId], SQUARE_CHANGE_SPEED, new Color[] { squareSpriRenArr[squareId].color, afterColor }));
        }

        /// <summary>
        /// ��̑���\�t���O�ؑ�
        /// </summary>
        /// <param name="squareId">�}�X�Ǘ��ԍ�</param>
        /// <param name="on">      true:����\�ɂ���</param>
        public void PieceOperationFlagChange(int squareId, bool on)
        {
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
        /// <returns>��̃����_���ȃv���n�u�̃C���f�b�N�X</returns>
        public int GetRandomPieceColor()
        {
            return USE_COLOR_TYPE_ARR[UnityEngine.Random.Range(0, USE_COLOR_COUNT)];
        }

        /// <summary>
        /// �}�X�ɂ����̐FID�擾
        /// </summary>
        /// <returns>�FID</returns>
        public int GetSquarePieceColorId(int squareIndex)
        {
            if (sPieceInfoArr[squareIndex] == null) return INT_NULL;
            return sPieceInfoArr[squareIndex].colorId;
        }

        /// <summary>
        /// �}�X�ɂ���M�~�b�N�I�u�W�F�N�g�ԍ��擾
        /// </summary>
        /// <returns>�M�~�b�N�̊Ǘ��ԍ�</returns>
        int GetSquareGimmickIndex(int squareIndex)
        {
            GameObject pieceObj = sPieceObjArr[squareIndex];
            if (pieceObj == null) return INT_NULL;
            return Array.IndexOf(gimmickObjArr, pieceObj);
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
            if (tapObj.CompareTag(GIMMICK_TAG) && !NOW_SUPPORT_ITEM_READY) return;

            //�Տ�̋�̏ꍇ
            int pieceObjIndex = Array.IndexOf(sPieceObjArr, tapObj);
            if (pieceObjIndex >= 0)
            {
                //����A�C�e���������̏ꍇ
                if (NOW_SUPPORT_ITEM_READY)
                {
                    StartCoroutine(stItemsMgr.UseItems(pieceObjIndex));
                }
                //��](���]�t���O�m�F)
                else if (sPieceInfoArr[pieceObjIndex].invertable)
                {
                    StartCoroutine(PutPieceToSquare(tapObj));
                }
            }
            //�ҋ@��̏ꍇ
            else if (Array.IndexOf(nextPieceObjArr, tapObj) >= 0)
            {
                //����A�C�e���������̏ꍇ�͉���
                if (NOW_SUPPORT_ITEM_READY)
                {
                    stItemsMgr.ResetWaitItemReady();
                }
                MoveNextPieceFrame(tapObj);
            }
        }

        /// <summary>
        /// �w��}�X�̃_���[�W
        /// </summary>
        /// <param name="squareIndex">   �}�X�ԍ�</param>
        /// <param name="reversiColorId">���]��̐FID</param>
        /// <param name="instantly">     �P�́E���j��</param>
        public void DamageSpecifiedSquare(int squareIndex, int reversiColorId, bool instantly = false)
        {
            if (sPieceObjArr[squareIndex] == null) return;   //��}�X

            //�M�~�b�N
            if (sPieceObjArr[squareIndex].CompareTag(GIMMICK_TAG))
            {
                //�_���[�W����
                int gimmickIndex = GetSquareGimmickIndex(squareIndex);
                bool damage = gimmicksMgr.DamageCheck(ref sGimmickInfoArr[gimmickIndex].colorId, ref gimmickIndex);

                //�_���[�W�L�̏ꍇ
                if (damage) gimmicksMgr.DamageGimmick(ref gimmickIndex, squareIndex);
            }
            //��
            else
            {
                //���]�\����
                if (sPieceInfoArr[squareIndex].invertable)
                {
                    //��J�E���g�M�~�b�N�Ƀ_���[�W(���]��)
                    StartCoroutine(gimmicksMgr.DamageCage(sPieceInfoArr[squareIndex].colorId));    //�B

                    //���]�J�n
                    if (!instantly) StartCoroutine(ReversingPieces(squareIndex, reversiColorId));
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
            //�z�u���t���O�Z�b�g
            NOW_PUTTING_PIECES = true;

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

            //�ҋ@���u��
            PutPiece(putIndex);

            //90����]
            nextPieceTraArr[nextPuPieceIndex].localRotation = PIECE_GENERATE_QUA;
            StartCoroutine(RotateMovement(nextPieceTraArr[nextPuPieceIndex], REVERSE_PIECE_ROT_SPEED, REVERSE_PIECE_FRONT_ROT));

            //��k��
            StopCoroutine(scaleUpCoroutine);
            yield return StartCoroutine(AllScaleChange(pieceTraArr[putIndex], PUT_PIECE_SCALING_SPEED, PIECE_DEFAULT_SCALE));
            pieceTraArr[putIndex].localScale = new Vector2(PIECE_DEFAULT_SCALE, PIECE_DEFAULT_SCALE);

            //��J�E���g�M�~�b�N�Ƀ_���[�W(�u������)
            StartCoroutine(gimmicksMgr.DamageCage(sPieceInfoArr[putIndex].colorId));    //�B

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
                StartCoroutine(TurnEnd());
            }

            //�z�u���t���O���Z�b�g
            NOW_PUTTING_PIECES = false;
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
            int sqrCountUp    = putPieceIndex % BOARD_LINE_COUNT;       //�u������̏�ɂ���}�X�̐�
            int sqrCountLeft  = putPieceIndex / BOARD_LINE_COUNT;       //�u������̍��ɂ���}�X�̐�
            int sqrCountDown  = BOARD_LINE_COUNT - sqrCountUp - 1;      //�u������̉��ɂ���}�X�̐�
            int sqrCountRight = BOARD_COLUMN_COUNT - sqrCountLeft - 1;  //�u������̉E�ɂ���}�X�̐�

            //���]����������Ԃ̐ݒ�
            int[] loopCounts = new int[DIRECTIONS_COUNT];
            int[][] dummyArr = new int[DIRECTIONS_COUNT][];
            foreach (Directions directions in Enum.GetValues(typeof(Directions)))
            {
                //�e�������̏����񐔐ݒ�
                loopCounts[(int)directions] = SetLoopCount(directions, ref sqrCountUp, ref sqrCountRight, ref sqrCountDown, ref sqrCountLeft);

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
        /// ���u�񐔂̐ݒ�
        /// </summary>
        /// <param name="directions">�����̊Ǘ��ԍ�</param>
        /// <param name="up">        ��ɂ���}�X�̐�</param>
        /// <param name="right">     �E�ɂ���}�X�̐�</param>
        /// <param name="down">      ���ɂ���}�X�̐�</param>
        /// <param name="left">      ���ɂ���}�X�̐�</param>
        /// <returns>������</returns>
        int SetLoopCount(Directions directions, ref int up, ref int right, ref int down, ref int left)
        {
            return directions switch
            {
                Directions.Up           => up,                              //��
                Directions.UpRight      => (up <= right) ? up : right,      //�E��
                Directions.Right        => right,                           //�E
                Directions.DownRight    => (down <= right) ? down : right,  //�E��
                Directions.Down         => down,                            //��
                Directions.DownLeft     => (down <= left) ? down : left,    //����
                Directions.Left         => left,                            //��
                Directions.UpLeft       => (up <= left) ? up : left,        //����
                _ => 0, //default
            };
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
                int refIndex = GetDesignatedDirectionIndex(direction, putPieceIndex, i);

                //��}�X�̏ꍇ��null��Ԃ�
                if (sPieceObjArr[refIndex] == null) return NOT_NUM;

                //�M�~�b�N�}�X�̏ꍇ
                if (sPieceObjArr[refIndex].CompareTag(GIMMICK_TAG))
                {
                    //���Ԏw�肪����ꍇ
                    int gimmickIndex = Array.IndexOf(gimmickObjArr, sPieceObjArr[refIndex]);
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
                int refIndex = GetDesignatedDirectionIndex(direction, putPieceIndex, i);

                //��}�X�̏ꍇ��null��Ԃ�
                if (sPieceObjArr[refIndex] == null) break;

                //�M�~�b�N�}�X�̏ꍇ
                if (sPieceObjArr[refIndex].CompareTag(GIMMICK_TAG))
                {
                    //�_���[�W��^�����邩�̊m�F,�_���[�W���^�����Ȃ��ꍇ��null��Ԃ�
                    int gimmickIndex = GetSquareGimmickIndex(refIndex);
                    if (!sGimmickInfoArr[gimmickIndex].destructible_Piece) break;
                    if (!gimmicksMgr.DamageCheck(ref putPieceColorId, ref gimmickIndex)) break;
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

        /// <summary>
        /// �w������Ƀ}�X�����邩�H
        /// </summary>
        /// <param name="direction">����</param>
        /// <param name="baseIndex">��̃}�X�Ǘ��ԍ�</param>
        /// <returns>�w��ꏊ�̊Ǘ��ԍ�</returns>
        public bool IsSquareSpecifiedDirection(Directions direction, int baseIndex)
        {
            switch (direction)
            {
                //��
                case Directions.Up:
                case Directions.UpLeft:
                case Directions.UpRight:
                    if (baseIndex % BOARD_LINE_COUNT == 0) return false;
                    break;
                //��
                case Directions.Down:
                case Directions.DownLeft:
                case Directions.DownRight:
                    if ((baseIndex + 1) % BOARD_LINE_COUNT == 0) return false;
                    break;
            }
            switch (direction)
            {
                //��
                case Directions.Left:
                case Directions.UpLeft:
                case Directions.DownLeft:
                    if (baseIndex < BOARD_LINE_COUNT) return false;
                    break;
                //�E
                case Directions.Right:
                case Directions.UpRight:
                case Directions.DownRight:
                    if (baseIndex >= SQUARES_COUNT - BOARD_LINE_COUNT) return false;
                    break;
            }

            //�}�X������
            return true;
        }

        /// <summary>
        /// �w��ꏊ�̊Ǘ��ԍ��擾
        /// </summary>
        /// <param name="direction">�����̊Ǘ��ԍ�</param>
        /// <param name="baseIndex">��I�u�W�F�N�g�̊Ǘ��ԍ�</param>
        /// <param name="distance"> ����</param>
        /// <returns>�w��ꏊ�̊Ǘ��ԍ�</returns>
        public int GetDesignatedDirectionIndex(int direction, int baseIndex, int distance = 1)
        {
            return direction switch
            {
                (int)Directions.Up          => baseIndex - distance,                                //��
                (int)Directions.UpRight     => baseIndex + BOARD_LINE_COUNT * distance - distance,  //�E��
                (int)Directions.Right       => baseIndex + BOARD_LINE_COUNT * distance,             //�E
                (int)Directions.DownRight   => baseIndex + BOARD_LINE_COUNT * distance + distance,  //�E��
                (int)Directions.Down        => baseIndex + distance,                                //��
                (int)Directions.DownLeft    => baseIndex - BOARD_LINE_COUNT * distance + distance,  //����
                (int)Directions.Left        => baseIndex - BOARD_LINE_COUNT * distance,             //��
                (int)Directions.UpLeft      => baseIndex - BOARD_LINE_COUNT * distance - distance,  //����
                _ => INT_NULL,  //default
            };
        }
        //==========================================================//


        //==========================================================//
        //---------------���ɒu����w��t���[���̓���---------------//
        //==========================================================//

        /// <summary>
        /// ��������̎w��t���[���ړ�
        /// </summary>
        /// <param name="tapPieceObj"></param>
        /// <returns></returns>
        void MoveNextPieceFrame(GameObject tapPieceObj)
        {
            //���ɒu���R�}�̊Ǘ��ԍ��X�V
            nextPuPieceIndex = Array.IndexOf(nextPieceObjArr, tapPieceObj);

            //�ړ�
            nextPieceFrameTra.SetParent(nextPieceBoxTraArr[nextPuPieceIndex], false);
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
            NOW_REVERSING_PIECES = true;

            //�M�~�b�N
            if (sPieceObjArr[reversIndex].CompareTag(GIMMICK_TAG))
            {
                //�M�~�b�N�I���ҋ@
                yield return sGimmickCorList[sGimmickCorList.Count - 1];
            }
            //��
            else
            {
                //���]
                Coroutine coroutine = StartCoroutine(ReversingPieces(reversIndex, putPieceColorId));

                //��J�E���g�M�~�b�N�Ƀ_���[�W(���]��)
                StartCoroutine(gimmicksMgr.DamageCage(putPieceColorId));    //�B

                //���]�I���ҋ@
                yield return coroutine;
            }

            //���]��̔j��
            StartCoroutine(StartDestroyingPiece(reversIndex));

            //���]���t���O���Z�b�g
            NOW_REVERSING_PIECES = false;
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
            NOW_REVERSING_PIECES = true;

            //���]�J�n
            Coroutine coroutine = null;
            foreach (int[] reversIndexArr in reversIndexList)
            {
                if (reversIndexArr == null) continue;
                foreach (int reversIndex in reversIndexArr)
                {
                    //�M�~�b�N�ł���΃M�~�b�N�j�󓮍�Ɉڂ�
                    int gimmickIndex = GetSquareGimmickIndex(reversIndex);
                    if (gimmickIndex >= 0)
                    {
                        gimmicksMgr.DamageGimmick(ref gimmickIndex, reversIndex);
                        yield return PIECE_REVERSAL_INTERVAL;
                        continue;
                    }

                    //�폜�Ώۂɔ��]��̊Ǘ��ԍ��ǉ�
                    sDestroyPiecesIndexList.Add(reversIndex);

                    //��J�E���g�M�~�b�N�Ƀ_���[�W(���]��)
                    StartCoroutine(gimmicksMgr.DamageCage(sPieceInfoArr[reversIndex].colorId));    //�B

                    //���]
                    coroutine = StartCoroutine(ReversingPieces(reversIndex, putPieceColorId));

                    yield return PIECE_REVERSAL_INTERVAL;
                }
                yield return PIECE_GROUP_REVERSAL_INTERVAL;
            }

            //��J�E���g�M�~�b�N�Ƀ_���[�W(���]��)
            StartCoroutine(gimmicksMgr.DamageCage(putPieceColorId));    //�B

            //�M�~�b�N�I���ҋ@
            foreach (Coroutine c in sGimmickCorList)
            { yield return c; }

            //���]�I���ҋ@
            yield return coroutine;

            //���]��̔j��
            StartCoroutine(StartDestroyingPieces());

            //���]���t���O���Z�b�g
            NOW_REVERSING_PIECES = false;
        }

        /// <summary>
        /// ��̔��]
        /// </summary>
        /// <param name="reversPieceIndex">���Ԃ���̊Ǘ��ԍ�</param>
        /// <param name="generateColorId"> ������̐F�ԍ�</param>
        public IEnumerator ReversingPieces(int reversPieceIndex, int generateColorId)
        {
            if (pieceTraArr[reversPieceIndex] == null) yield break;

            //��90����],�g��
            StartCoroutine(AllScaleChange(pieceTraArr[reversPieceIndex], REVERSE_PIECE_SCALING_SPEED, REVERSE_PIECE_CHANGE_SCALE));
            yield return StartCoroutine(RotateMovement(pieceTraArr[reversPieceIndex], REVERSE_PIECE_ROT_SPEED, REVERSE_PIECE_SWITCH_ROT));

            //����폜,�V���
            DeletePiece(reversPieceIndex);
            GeneratePiece(generateColorId, reversPieceIndex);

            //��90����],�k��
            pieceTraArr[reversPieceIndex].localScale    = new Vector3(REVERSE_PIECE_CHANGE_SCALE, REVERSE_PIECE_CHANGE_SCALE, 0.0f);
            pieceTraArr[reversPieceIndex].localRotation = PIECE_GENERATE_QUA;
            StartCoroutine(AllScaleChange(pieceTraArr[reversPieceIndex], REVERSE_PIECE_SCALING_SPEED, PIECE_DEFAULT_SCALE));
            yield return StartCoroutine(RotateMovement(pieceTraArr[reversPieceIndex], REVERSE_PIECE_ROT_SPEED, REVERSE_PIECE_FRONT_ROT));
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
            NOW_DESTROYING_PIECES = true;

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
            NOW_DESTROYING_PIECES = false;
        }

        /// <summary>
        /// ��j��J�n(����)
        /// </summary>
        /// <param name="useSupport">����A�C�e���g�p�H</param>
        public IEnumerator StartDestroyingPieces(bool useSupport = false)
        {
            //�j�󒆃t���O�Z�b�g
            NOW_DESTROYING_PIECES = true;

            //��k��
            Coroutine coroutine = null;
            foreach (int index in sDestroyPiecesIndexList)
            {
                if (sPieceObjArr[index].CompareTag(GIMMICK_TAG)) continue;
                coroutine = StartCoroutine(AllScaleChange(pieceTraArr[index], DESTROY_PIECE_SCALING_SPEED, DESTROY_PIECE_CHANGE_SCALE));
            }
            yield return coroutine;
            
            //����A�C�e������
           �@if (!useSupport) stItemsMgr.GenerateItems();

            //��폜
            foreach (int pieceIndex in sDestroyPiecesIndexList)
            {
                GameObject obj = sPieceObjArr[pieceIndex];
                if (obj.CompareTag(GIMMICK_TAG)) DeleteGimmick(obj);
                else DeletePiece(pieceIndex);
            }

            //�폜��X�g�̏�����
            sDestroyPiecesIndexList = new List<int>();

            //��̗����J�n
            yield return StartCoroutine(StratFallingPieces());

            //�^�[���I��
            StartCoroutine(TurnEnd(useSupport));

            //�j�󒆃t���O���Z�b�g
            NOW_DESTROYING_PIECES = false;
        }

        /// <summary>
        /// ��̗����J�n
        /// </summary>
        IEnumerator StratFallingPieces()
        {
            //������t���O�Z�b�g
            NOW_FALLING_PIECES = true;

            //�S��Ǘ��ԍ��̍X�V,������X�g�̎擾
            List<int> fallPiecesIndexList = SettingOfFallingPieces();

            //�����J�n
            List<Coroutine> coroutineList = new List<Coroutine>();
            foreach (int fallPieceIndex in fallPiecesIndexList)
            {
                if (pieceTraArr[fallPieceIndex] != null)
                {
                    Vector3 targetPos = PIECE_DEFAULT_POS;
                    int gimmickIndex = GetSquareGimmickIndex(fallPieceIndex);
                    if (gimmickIndex >= 0) targetPos = sGimmickInfoArr[gimmickIndex].defaultPos;
                    coroutineList.Add(StartCoroutine(ConstantSpeedMovement(pieceTraArr[fallPieceIndex], FALL_PIECE_MOVE_SPEED, targetPos, FALL_PIECE_ACCELE_RATE)));
                }
            }
            foreach (Coroutine coroutine in coroutineList)
            { yield return coroutine; }

            //������t���O���Z�b�g
            NOW_FALLING_PIECES = false;
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
                if (!squareObjArr[i].activeSelf) continue;

                //��}�X�łȂ���Ώ������X�L�b�v
                if (sPieceObjArr[i] != null) continue;

                //���������X�g�ɒǉ�
                fallPiecesIndexList.Add(i);

                //��̏�ɂ���Ǘ��ԍ������ׂĒ���
                int loopCount = i % BOARD_LINE_COUNT;
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
                        //�V�K�����_������
                        int prefabIndex = GetRandomPieceColor();
                        GeneratePiece(prefabIndex, i);
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
                int gimmickObjIndex = Array.IndexOf(gimmickObjArr, pieceObj);
                if (sGimmickInfoArr[gimmickObjIndex].inSquare && !sGimmickInfoArr[gimmickObjIndex].freeFall_Piece)
                {
                    return false;
                }
                return sGimmickInfoArr[gimmickObjIndex].freeFall;
            }

            return false;
        }


        //==========================================================//
        //-------------------�^�[���I�����̏���---------------------//
        //==========================================================//

        /// <summary>
        /// �M�~�b�N�j��m�F
        /// </summary>
        IEnumerator GimmickDestroyCheck()
        {
            //�M�~�b�N�j��ҋ@���t���O�Z�b�g
            NOW_GIMMICK_DESTROY_WAIT = true;

            //�j��J�n
            yield return StartCoroutine(gimmicksMgr.DestroyGimmicks_TurnEnd());

            //�M�~�b�N�j��ҋ@���t���O���Z�b�g
            NOW_GIMMICK_DESTROY_WAIT = false;
        }

        /// <summary>
        /// �M�~�b�N�̏�ԕω��J�n
        /// </summary>
        /// <returns></returns>
        IEnumerator StartChangeGimmickState()
        {
            //�M�~�b�N��ԕω����t���O�Z�b�g
            NOW_GIMMICK_STATE_CHANGE = true;

            //��ԕω��ҋ@
            yield return StartCoroutine(gimmicksMgr.ChangeGimmickState());

            //�M�~�b�N��ԕω����t���O���Z�b�g
            NOW_GIMMICK_STATE_CHANGE = false;
        }

        /// <summary>
        /// �^�[���I��
        /// </summary>
        /// <param name="supportItem">����A�C�e���H</param>
        public IEnumerator TurnEnd(bool supportItem = false)
        {
            //�^�[���I���������t���O�Z�b�g
            NOW_TURN_END_PROCESSING = true;

            //����A�C�e���g�p��
            if (supportItem)
            {
                //����M�~�b�N�j�󔻒�J�n
                yield return StartCoroutine(GimmickDestroyCheck());

                //���R����
                yield return StartCoroutine(StratFallingPieces());

                //����M�~�b�N�j�󔻒�J�n
                yield return StartCoroutine(GimmickDestroyCheck());

                //�M�~�b�N�̃t���O���Z�b�g
                foreach (GimmickInformation gimmickInfo in sGimmickInfoArr)
                {
                    if (gimmickInfo == null) continue;
                    gimmickInfo.nowTurnDamage = false;
                }
            }
            //����A�C�e�����g�p��
            else
            {
                //����M�~�b�N�j�󔻒�J�n
                yield return StartCoroutine(GimmickDestroyCheck());

                //���R����
                yield return StartCoroutine(StratFallingPieces());

                //�M�~�b�N��ԕω��J�n
                yield return StartCoroutine(StartChangeGimmickState());

                //����M�~�b�N�j�󔻒�J�n
                yield return StartCoroutine(GimmickDestroyCheck());

                //�M�~�b�N�̃t���O���Z�b�g
                foreach (GimmickInformation gimmickInfo in sGimmickInfoArr)
                {
                    if (gimmickInfo == null) continue;
                    gimmickInfo.nowTurnDamage = false;
                }
            }

            //�M�~�b�N�ҋ@���X�g�̏�����
            sGimmickCorList = new List<Coroutine>();

            //�^�[���I���������t���O���Z�b�g
            NOW_TURN_END_PROCESSING = false;
        }
    }
}