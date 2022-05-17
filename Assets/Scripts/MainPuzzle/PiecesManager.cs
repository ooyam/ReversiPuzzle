using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static GameManager;
using static PuzzleDefine;
using static ObjectMove_2D.ObjectMove_2D;

namespace PuzzleMain
{
    public class PiecesManager : MonoBehaviour
    {
        [Header("GimmicksManager�̎擾")]
        [SerializeField]
        GimmicksManager gimmicksMan;

        [Header("��v���n�u�̎擾")]
        [SerializeField]
        GameObject[] piecePrefabArr;

        [Header("�M�~�b�N�v���n�u�̎擾")]
        public gimmickArr[] gimmickPrefabArr;
        [System.Serializable]
        public class gimmickArr
        { public GameObject[] prefab; }

        [Header("���o�[�V�Ղ̎擾")]
        [SerializeField]
        Transform reversiBoardTra;

        [Header("�ҋ@��{�b�N�X�̎擾")]
        [SerializeField]
        Transform nextPieceBoxesTra;

        GameObject[] pieceObjArr;               //��I�u�W�F�N�g�z��
        Transform[]  pieceTraArr;               //��Transform�z��
        GameObject[] squareObjArr;              //�}�X�I�u�W�F�N�g�z��
        public static Transform[] squareTraArr; //�}�XTransform�z��
        SpriteRenderer[] squareSpriRenArr;      //�}�XSpriteRenderer�z��
        GameObject[] nextPieceObjArr;           //�ҋ@��I�u�W�F�N�g�z��
        Transform[]  nextPieceTraArr;           //�ҋ@��Transform�z��
        GameObject[] nextPieceBoxObjArr;        //�ҋ@��I�u�W�F�N�g�z��
        Transform[]  nextPieceBoxTraArr;        //�ҋ@�Transform�z��
        Transform    nextPieceFrameTra;         //���ɒu���R�}�̎w��t���[��
        GameObject[] gimmickObjArr;             //�M�~�b�N�I�u�W�F�N�g�z��
        int nextPuPieceIndex = 0;               //���ɒu����̊Ǘ��ԍ�
        int squaresCount;                       //�}�X�̌�
        int nextPiecesCount;                    //�ҋ@��̌�
        int[] directionsIntArr;                 //8�����̊Ǘ��ԍ��z��

        public static PieceInformation[] pieceInfoArr;                          //��̏��z��
        public static GimmickInformation[] gimmickInfoArr;                      //�M�~�b�N�̏��z��
        public static List<int> destroyPiecesIndexList = new List<int>();       //�폜��̊Ǘ��ԍ����X�g
        public static List<Coroutine> gimmickCorList = new List<Coroutine>();   //���쒆�M�~�b�N���X�g

        //==========================================================//
        //----------------------�����ݒ�,�擾-----------------------//
        //==========================================================//

        void Start()
        {
            //8�����̊Ǘ��ԍ��擾
            System.Array directions = Enum.GetValues(typeof(Directions));
            directionsIntArr = new int[directions.Length];
            foreach (Directions direction in directions)
            { directionsIntArr[(int)direction] = (int)direction; }

            //�}�X�擾
            squaresCount = BOARD_COLUMN_COUNT * BOARD_LINE_COUNT;
            pieceTraArr  = new Transform[squaresCount];
            pieceObjArr  = new GameObject[squaresCount];
            pieceInfoArr = new PieceInformation[squaresCount];
            squareObjArr = new GameObject[squaresCount];
            squareTraArr = new Transform[squaresCount];
            squareSpriRenArr = new SpriteRenderer[squaresCount];
            for (int i = 0; i < squaresCount; i++)
            {
                squareObjArr[i] = reversiBoardTra.GetChild(i).gameObject;
                squareTraArr[i] = squareObjArr[i].transform;
                squareSpriRenArr[i] = squareObjArr[i].GetComponent<SpriteRenderer>();
            }

            //�g�p���Ȃ��}�X���\��
            foreach (int i in HIDE_SQUARE_ARR)
            { squareObjArr[i].SetActive(false); }

            //�M�~�b�N��z�u
            gimmickObjArr  = new GameObject[GIMMICKS_COUNT];
            gimmickInfoArr = new GimmickInformation[GIMMICKS_COUNT];
            List<int> notPlaceIndex = new List<int>();  //���z�u���Ȃ��}�X�ԍ�
            bool notSquare = false;                     //��Ƃ��ĊǗ����Ȃ��M�~�b�N������
            for (int i = 0; i < GIMMICKS_COUNT; i++)
            {
                //��Ƃ��ĊǗ�����M�~�b�N
                if (GIMMICKS_DATA.param[GIMMICKS_INFO_ARR[i][GIMMICK]].in_square)
                {
                    GeneraeGimmick(i);
                    notPlaceIndex.Add(GIMMICKS_INFO_ARR[i][SQUARE]);
                }
                else notSquare = true;
            }
            //��Ƃ��Ĕz�u���Ȃ��M�~�b�N
            if (notSquare) gimmicksMan.PlaceGimmickNotInSquare();

            //��̃����_���z�u
            for (int i = 0; i < squaresCount; i++)
            {
                if (!squareObjArr[i].activeSelf) continue; //��\���}�X�͏������΂�
                if (notPlaceIndex.Contains(i))   continue; //�M�~�b�N�}�X�͏������΂�

                int pieceGeneIndex = UnityEngine.Random.Range(0, USE_PIECE_COUNT);
                GeneratePiece(pieceGeneIndex, i, true);
            }

            //�ҋ@��̔��擾
            nextPiecesCount    = nextPieceBoxesTra.childCount;
            nextPieceBoxObjArr = new GameObject[nextPiecesCount];
            nextPieceBoxTraArr = new Transform[nextPiecesCount];
            for (int i = 0; i < nextPiecesCount; i++)
            {
                nextPieceBoxObjArr[i] = nextPieceBoxesTra.GetChild(i).gameObject;
                nextPieceBoxTraArr[i] = nextPieceBoxObjArr[i].transform;

            }

            //���ɒu���R�}�̎w��t���[���擾
            nextPieceFrameTra = nextPieceBoxTraArr[0].GetChild(0).gameObject.transform;

            //�ҋ@���
            nextPieceObjArr = new GameObject[nextPiecesCount];
            nextPieceTraArr = new Transform[nextPiecesCount];
            for (int i = 0; i < nextPiecesCount; i++)
            {
                int pieceGeneIndex = UnityEngine.Random.Range(0, USE_PIECE_COUNT);
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
            pieceObjArr[pieceIndex]  = Instantiate(piecePrefabArr[prefabIndex]);
            pieceInfoArr[pieceIndex] = pieceObjArr[pieceIndex].GetComponent<PieceInformation>();
            pieceInfoArr[pieceIndex].InformationSetting(pieceIndex, startGenerate, gimmickInfoArr);
            pieceTraArr[pieceIndex]  = pieceInfoArr[pieceIndex].tra;
            pieceTraArr[pieceIndex].SetParent(squareTraArr[pieceIndex], false);
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
            Destroy(pieceObjArr[pieceIndex]);
            pieceObjArr[pieceIndex]  = null;
            pieceTraArr[pieceIndex]  = null;
            pieceInfoArr[pieceIndex] = null;
        }

        /// <summary>
        /// �ҋ@���Ֆʂɒu��
        /// </summary>
        /// <param name="squareId">�z�u�}�X�Ǘ��ԍ�</param>
        void PutPiece(int squareId)
        {
            //�Ֆʂ̋�폜,�Ǘ��z�񍷂��ւ�
            DeletePiece(squareId);
            pieceObjArr[squareId]  = nextPieceObjArr[nextPuPieceIndex];
            pieceTraArr[squareId]  = nextPieceTraArr[nextPuPieceIndex];
            pieceInfoArr[squareId] = pieceObjArr[squareId].GetComponent<PieceInformation>();
            pieceInfoArr[squareId].InformationSetting(squareId, false);

            //�ҋ@���
            int pieceGeneIndex = UnityEngine.Random.Range(0, USE_PIECE_COUNT);
            GenerateNextPiece(pieceGeneIndex, nextPuPieceIndex);
        }

        /// <summary>
        /// �M�~�b�N�쐬
        /// </summary>
        /// /// <param name="gimmickIndex">�M�~�b�N�Ǘ��ԍ�</param>
        void GeneraeGimmick(int gimmickIndex)
        {
            int colorId = (GIMMICKS_INFO_ARR[gimmickIndex][COLOR] < 0) ? 0 : GIMMICKS_INFO_ARR[gimmickIndex][COLOR];
            gimmickObjArr[gimmickIndex] = Instantiate(gimmickPrefabArr[GIMMICKS_INFO_ARR[gimmickIndex][GIMMICK]].prefab[colorId]);

            //Component�擾
            gimmickInfoArr[gimmickIndex] = gimmickObjArr[gimmickIndex].GetComponent<GimmickInformation>();
            gimmickInfoArr[gimmickIndex].InformationSetting(gimmickIndex);

            //��Ƃ��Ă��Ǘ�����
            int pieceIndex = GIMMICKS_INFO_ARR[gimmickIndex][SQUARE];
            pieceObjArr[pieceIndex] = gimmickObjArr[gimmickIndex];
            pieceTraArr[pieceIndex] = gimmickInfoArr[gimmickIndex].tra;
            pieceTraArr[pieceIndex].SetParent(squareTraArr[GIMMICKS_INFO_ARR[gimmickIndex][SQUARE]], false);
            pieceTraArr[pieceIndex].SetSiblingIndex(0);
            pieceTraArr[pieceIndex].localPosition = gimmickInfoArr[gimmickIndex].defaultPos;
        }

        /// <summary>
        /// �}�X�Ƃ��ĊǗ����Ȃ��M�~�b�N�z�u
        /// </summary>
        /// <param name="gimmickObj">�z�u�I�u�W�F�N�g</param>
        /// <param name="squareId">  �z�u�}�X�Ǘ��ԍ�</param>
        public void PlaceGimmick(GameObject gimmickObj, int squareId)
        {
            gimmickObj.transform.SetParent(squareTraArr[squareId], false);
        }

        /// <summary>
        /// �M�~�b�N�폜
        /// </summary>
        /// /// <param name="pieceIndex">�폜��̊Ǘ��ԍ�</param>
        void DeleteGimmick(GameObject gimmickObj)
        {
            int gimmickIndex = Array.IndexOf(gimmickObjArr, gimmickObj);
            gimmickObjArr[gimmickIndex] = null;

            DeletePiece(Array.IndexOf(pieceObjArr, gimmickObj));
        }


        /// <summary>
        /// �Ǘ��ԍ��̍X�V
        /// </summary>
        /// <param name="oldIndex">�X�V�O�̊Ǘ��ԍ�</param>
        /// <param name="newIndex">�X�V��̊Ǘ��ԍ�</param>
        void UpdateManagementIndex(int oldIndex, int newIndex)
        {
            pieceObjArr[newIndex]  = pieceObjArr[oldIndex];
            pieceTraArr[newIndex]  = pieceTraArr[oldIndex];
            pieceInfoArr[newIndex] = pieceInfoArr[oldIndex];
            pieceTraArr[newIndex].SetParent(squareTraArr[newIndex], true);
            pieceTraArr[newIndex].SetSiblingIndex(0);
            pieceObjArr[oldIndex]  = null;
            pieceTraArr[oldIndex]  = null;
            pieceInfoArr[oldIndex] = null;
            if (pieceInfoArr[newIndex] != null) pieceInfoArr[newIndex].squareId = newIndex;
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
            if (on) pieceInfoArr[squareId].OperationFlagON();
            else    pieceInfoArr[squareId].OperationFlagOFF();
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
            //�M�~�b�N�ɂ͒��u���ł��Ȃ�
            if (tapObj.tag == GIMMICK_TAG) return;

            //�Տ�̋�̏ꍇ
            int pieceObjIndex = Array.IndexOf(pieceObjArr, tapObj);
            if (pieceObjIndex >= 0)
            {
                //���]�֎~�t���O�m�F
                if (!pieceInfoArr[pieceObjIndex].invertable) return;
                StartCoroutine(PutPieceToSquare(tapObj));
            }

            //�ҋ@��̏ꍇ
            if (Array.IndexOf(nextPieceObjArr, tapObj) >= 0) MoveNextPieceFrame(tapObj);
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
            int putIndex = Array.IndexOf(pieceObjArr, deletePiece);
            nextPieceTraArr[nextPuPieceIndex].SetParent(squareTraArr[putIndex], true);
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

            //���]��X�g�擾
            int colorId = pieceInfoArr[putIndex].colorId;       //�u������̐F�ԍ��擾
            List<int[]> reversIndexList = new List<int[]>();    //���]��̊Ǘ��ԍ��i�[���X�g
            if (GetReversIndex(putIndex, ref colorId, ref reversIndexList))
            {
                //�폜�Ώۂɒu������̊Ǘ��ԍ��ǉ�
                destroyPiecesIndexList.Add(putIndex);

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
            int squaresCount_Up    = putPieceIndex % BOARD_LINE_COUNT;            //�u������̏�ɂ���}�X�̐�
            int squaresCount_Left  = putPieceIndex / BOARD_LINE_COUNT;            //�u������̍��ɂ���}�X�̐�
            int squaresCount_Down  = BOARD_LINE_COUNT - squaresCount_Up - 1;      //�u������̉��ɂ���}�X�̐�
            int squaresCount_Right = BOARD_COLUMN_COUNT - squaresCount_Left - 1;  //�u������̉E�ɂ���}�X�̐�

            //�������Ƃɔ��]��̔ԍ��擾
            foreach (int directionsInt in directionsIntArr)
            {
                int loopCount = SetLoopCount(directionsInt, ref squaresCount_Up, ref squaresCount_Right, ref squaresCount_Down, ref squaresCount_Left);
                if (loopCount == 0) continue;
                reversIndexList.Add(GetReversIndex_SpecifiedDirection(ref putPieceIndex, ref putPieceColorId, ref loopCount, directionsInt));
            }

            //���]��̗L����Ԃ�
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
        int SetLoopCount(int directions, ref int up, ref int right, ref int down, ref int left)
        {
            switch (directions)
            {
                case (int)Directions.Up:        return up;                              //��
                case (int)Directions.UpRight:   return (up <= right) ? up : right;      //�E��
                case (int)Directions.Right:     return right;                           //�E
                case (int)Directions.DownRight: return (down <= right) ? down : right;  //�E��
                case (int)Directions.Down:      return down;                            //��
                case (int)Directions.DownLeft:  return (down <= left) ? down : left;    //����
                case (int)Directions.Left:      return left;                            //��
                case (int)Directions.UpLeft:    return (up <= left) ? up : left;        //����
                default: return 0;
            }
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
            for (int i = 1; i <= loopCount; i++)
            {
                //�w������̃C���f�b�N�X�ԍ��擾
                int refIndex = GetPlaceIndex(ref direction, ref putPieceIndex, ref i);

                //��}�X�̏ꍇ��null��Ԃ�
                if (pieceObjArr[refIndex] == null) return null;

                //�M�~�b�N�}�X�̏ꍇ
                if (pieceObjArr[refIndex].tag == GIMMICK_TAG)
                {
                    //�_���[�W��^�����邩�̊m�F,�_���[�W���^�����Ȃ��ꍇ��null��Ԃ�
                    int gimmickIndex = Array.IndexOf(gimmickObjArr, pieceObjArr[refIndex]);
                    if (!gimmicksMan.DamageCheck(ref putPieceColorId, ref gimmickIndex))
                        return null;
                    reversIndexList.Add(refIndex);
                }
                else
                {
                    //���]�֎~��̏ꍇ��null��Ԃ�
                    if (!pieceInfoArr[refIndex].invertable) return null;

                    //���F�������
                    if (pieceInfoArr[refIndex].colorId == putPieceColorId)
                    {
                        //�ׂ����^�O�̏ꍇ��null��Ԃ�
                        if (i == 1) return null;

                        //�폜�Ώۂɓ��^�O��̊Ǘ��ԍ��ǉ�
                        destroyPiecesIndexList.Add(refIndex);
                        break;
                    }
                    reversIndexList.Add(refIndex);
                }

                //�Ō�܂œ��^�O��Ȃ��ꍇ��null��Ԃ�
                if (i == loopCount) return null;
            }
            return reversIndexList.ToArray();
        }

        /// <summary>
        /// �w��ꏊ�̊Ǘ��ԍ��擾
        /// </summary>
        /// <param name="direction">�����̊Ǘ��ԍ�</param>
        /// <param name="baseIndex">��I�u�W�F�N�g�̊Ǘ��ԍ�</param>
        /// <param name="distance"> ����</param>
        /// <returns>�w��ꏊ�̊Ǘ��ԍ�</returns>
        int GetPlaceIndex(ref int direction, ref int baseIndex, ref int distance)
        {
            switch (direction)
            {
                case (int)Directions.Up:        return baseIndex - distance;                                //��
                case (int)Directions.UpRight:   return baseIndex + BOARD_LINE_COUNT * distance - distance;  //�E��
                case (int)Directions.Right:     return baseIndex + BOARD_LINE_COUNT * distance;             //�E
                case (int)Directions.DownRight: return baseIndex + BOARD_LINE_COUNT * distance + distance;  //�E��
                case (int)Directions.Down:      return baseIndex + distance;                                //��
                case (int)Directions.DownLeft:  return baseIndex - BOARD_LINE_COUNT * distance + distance;  //����
                case (int)Directions.Left:      return baseIndex - BOARD_LINE_COUNT * distance;             //��
                case (int)Directions.UpLeft:    return baseIndex - BOARD_LINE_COUNT * distance - distance;  //����
                default: return 0;
            }
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
        /// ��]�J�n
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
                    int gimmickIndex = Array.IndexOf(gimmickObjArr, pieceObjArr[reversIndex]);
                    if (gimmickIndex >= 0)
                    {
                        gimmicksMan.DamageGimmick(ref gimmickIndex, reversIndex);
                        yield return PIECE_REVERSAL_INTERVAL;
                        continue;
                    }

                    //�폜�Ώۂɔ��]��̊Ǘ��ԍ��ǉ�
                    destroyPiecesIndexList.Add(reversIndex);

                    //���]
                    coroutine = StartCoroutine(ReversingPieces(reversIndex, putPieceColorId));
                    yield return PIECE_REVERSAL_INTERVAL;
                }
                yield return PIECE_GROUP_REVERSAL_INTERVAL;
            }

            //�M�~�b�N�I���ҋ@
            foreach (Coroutine gimmickCor in gimmickCorList)
            { yield return gimmickCor; }
            gimmickCorList = new List<Coroutine>();

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
        /// <param name="prefabIndex">     ������v���n�u�ԍ�</param>
        IEnumerator ReversingPieces(int reversPieceIndex, int prefabIndex)
        {
            //��90����],�g��
            StartCoroutine(AllScaleChange(pieceTraArr[reversPieceIndex], REVERSE_PIECE_SCALING_SPEED, REVERSE_PIECE_CHANGE_SCALE));
            yield return StartCoroutine(RotateMovement(pieceTraArr[reversPieceIndex], REVERSE_PIECE_ROT_SPEED, REVERSE_PIECE_SWITCH_ROT));

            //����폜,�V���
            DeletePiece(reversPieceIndex);
            GeneratePiece(prefabIndex, reversPieceIndex);

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
        /// ��j��J�n
        /// </summary>
        IEnumerator StartDestroyingPieces()
        {
            //�j�󒆃t���O�Z�b�g
            NOW_DESTROYING_PIECES = true;

            //��k��
            Coroutine coroutine = null;
            List<Coroutine> gimCorList = new List<Coroutine>();
            foreach (int index in destroyPiecesIndexList)
            {
                if (pieceTraArr[index].gameObject.tag == GIMMICK_TAG) continue;
                coroutine = StartCoroutine(AllScaleChange(pieceTraArr[index], DESTROY_PIECE_SCALING_SPEED, DESTROY_PIECE_CHANGE_SCALE));

                //��J�E���g�M�~�b�N�Ƀ_���[�W
                gimCorList.Add(StartCoroutine(gimmicksMan.DamageCage(pieceInfoArr[index].colorId)));    //�B
            }
            yield return coroutine;
            foreach (Coroutine cor in gimCorList)
            { yield return cor; }

            //��폜
            foreach (int pieceIndex in destroyPiecesIndexList)
            {
                GameObject obj = pieceObjArr[pieceIndex];
                if (obj.tag == GIMMICK_TAG) DeleteGimmick(obj);
                else DeletePiece(pieceIndex);
            }

            //�폜��X�g�̏�����
            destroyPiecesIndexList = new List<int>();

            //��̗����J�n
            StartCoroutine(StratFallingPieces());

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
                    int gimmickIndex = Array.IndexOf(gimmickObjArr, pieceTraArr[fallPieceIndex].gameObject);
                    if (gimmickIndex >= 0) targetPos = gimmickInfoArr[gimmickIndex].defaultPos;
                    coroutineList.Add(StartCoroutine(ConstantSpeedMovement(pieceTraArr[fallPieceIndex], FALL_PIECE_MOVE_SPEED, targetPos, FALL_PIECE_ACCELE_RATE)));
                }
            }
            foreach (Coroutine coroutine in coroutineList)
            { yield return coroutine; }

            //�^�[���I�������J�n
            StartCoroutine(TurnEnd());

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
            for (int i = squaresCount - 1; i >= 0; i--)
            {
                //��}�X�łȂ���Ώ������X�L�b�v
                if (pieceObjArr[i] != null) continue;

                //���������X�g�ɒǉ�
                fallPiecesIndexList.Add(i);

                //��̏�ɂ���Ǘ��ԍ������ׂĒ���
                int loopCount = i % BOARD_LINE_COUNT;
                for (int n = 0; n <= loopCount; n++)
                {
                    //���g���n��̔ԍ�����łȂ��ꍇ
                    int refIndex = i - n;
                    if (pieceObjArr[refIndex] != null)
                    {
                        //���R�炩�\�I�u�W�F�N�g�̏ꍇ,�Ǘ��ԍ��X�V
                        if (FreeFallJudgement(ref pieceObjArr[refIndex]))
                            UpdateManagementIndex(i - n, i);
                        break;
                    }

                    //�����ł����Տ�ɑ��݂��Ȃ��ꍇ
                    if (n == loopCount)
                    {
                        //�V�K�����_������
                        int prefabIndex = UnityEngine.Random.Range(0, USE_PIECE_COUNT);
                        GeneratePiece(prefabIndex, i);
                        pieceTraArr[i].localPosition = FALL_PIECE_GENERATE_POS;
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
            if (pieceObj.tag == PIECE_TAG)
            {
                //��̏ꍇ
                int pieceObjIndex = Array.IndexOf(pieceObjArr, pieceObj);
                return pieceInfoArr[pieceObjIndex].freeFall;
            }
            else if (pieceObj.tag == GIMMICK_TAG)
            {
                //�M�~�b�N�̏ꍇ
                int gimmickObjIndex = Array.IndexOf(gimmickObjArr, pieceObj);
                return gimmickInfoArr[gimmickObjIndex].freeFall;
            }

            return false;
        }


        //==========================================================//
        //-------------------�^�[���I�����̏���---------------------//
        //==========================================================//

        /// <summary>
        /// �M�~�b�N�j��m�F
        /// </summary>
        IEnumerator GimmickDeleteCheck()
        {
            //�M�~�b�N�_���[�W�ҋ@���t���O�Z�b�g
            NOW_GIMMICK_DAMAGE_WAIT = true;

            //�g�̃_���[�W�m�F
            yield return StartCoroutine(gimmicksMan.DamageFrame());

            //�M�~�b�N�_���[�W�ҋ@���t���O���Z�b�g
            NOW_GIMMICK_DAMAGE_WAIT = false;
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
            yield return StartCoroutine(gimmicksMan.ChangeGimmickState());

            //�M�~�b�N��ԕω����t���O���Z�b�g
            NOW_GIMMICK_STATE_CHANGE = false;
        }

        /// <summary>
        /// �^�[���I��
        /// </summary>
        IEnumerator TurnEnd()
        {
            //����M�~�b�N�j�󔻒�J�n
            yield return StartCoroutine(GimmickDeleteCheck());

            //�M�~�b�N��ԕω��J�n
            yield return StartCoroutine(StartChangeGimmickState());

            //����M�~�b�N�j�󔻒�J�n
            yield return StartCoroutine(GimmickDeleteCheck());

            //�M�~�b�N�̃t���O���Z�b�g
            foreach (GimmickInformation gimmickInfo in gimmickInfoArr)
            {
                if (gimmickInfo == null) continue;
                gimmickInfo.nowTurnDamage = false;
            }
        }
    }
}