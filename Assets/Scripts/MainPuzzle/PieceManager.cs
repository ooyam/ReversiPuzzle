using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static GameManager;
using static PuzzleDefine;
using static ObjectMove_2D.ObjectMove_2D;

namespace PuzzleMain
{
    public class PieceManager : MonoBehaviour
    {
        [Header("GimmicksManager�̎擾")]
        [SerializeField]
        GimmicksManager gimmicksMan;

        [Header("��v���n�u�̎擾")]
        [SerializeField]
        GameObject[] piecePrefabArr;

        [Header("�M�~�b�N�v���n�u�̎擾")]
        [SerializeField]
        gimmickArr[] gimmickPrefabArr;
        [System.Serializable]
        public class gimmickArr
        { public GameObject[] prefab; }

        [Header("���o�[�V�Ղ̎擾")]
        [SerializeField]
        Transform reversiBoardTra;

        [Header("�ҋ@��{�b�N�X�̎擾")]
        [SerializeField]
        Transform nextPieceBoxesTra;

        GameObject[] pieceObjArr;          //��I�u�W�F�N�g�z��
        Transform[]  pieceTraArr;          //��Transform�z��
        GameObject[] squareObjArr;         //�}�X�I�u�W�F�N�g�z��
        Transform[]  squareTraArr;         //�}�XTransform�z��
        GameObject[] nextPieceObjArr;      //�ҋ@��I�u�W�F�N�g�z��
        Transform[]  nextPieceTraArr;      //�ҋ@��Transform�z��
        GameObject[] nextPieceBoxObjArr;   //�ҋ@��I�u�W�F�N�g�z��
        Transform[]  nextPieceBoxTraArr;   //�ҋ@�Transform�z��
        Transform    nextPieceFrameTra;    //���ɒu���R�}�̎w��t���[��
        GameObject[] gimmickObjArr;        //�M�~�b�N�I�u�W�F�N�g�z��
        int nextPuPieceIndex = 0;          //���ɒu����̊Ǘ��ԍ�
        int gimmicksCount;                 //�z�u�M�~�b�N�̐�
        int squaresCount;                  //�}�X�̌�
        int nextPiecesCount;               //�ҋ@��̌�
        string[] pieceTagsArr;             //��^�O�z��
        int[] directionsIntArr;            //8�����̊Ǘ��ԍ��z��

        //�M�~�b�N�̏��z��
        [System.NonSerialized]
        public static GimmickInformation[] gimmickInfoArr;

        //�폜��̊Ǘ��ԍ����X�g
        [System.NonSerialized]
        public static List<int> destroyPiecesIndexList = new List<int>();

        //���쒆�M�~�b�N���X�g
        [System.NonSerialized]
        public static List<Coroutine> gimmickCorList = new List<Coroutine>();

        //==========================================================//
        //----------------------�����ݒ�,�擾-----------------------//
        //==========================================================//

        void Start()
        {
            //��̃^�O�擾
            System.Array pieceColors = Enum.GetValues(typeof(Colors));
            pieceTagsArr = new string[pieceColors.Length];
            foreach (Colors pieceColor in pieceColors)
            { pieceTagsArr[(int)pieceColor] = Enum.GetName(typeof(Colors), pieceColor); }

            //8�����̊Ǘ��ԍ��擾
            System.Array directions = Enum.GetValues(typeof(Directions));
            directionsIntArr = new int[directions.Length];
            foreach (Directions direction in directions)
            { directionsIntArr[(int)direction] = (int)direction; }

            //�}�X�擾
            squaresCount = BOARD_COLUMN_COUNT * BOARD_LINE_COUNT;
            squareObjArr = new GameObject[squaresCount];
            squareTraArr = new Transform[squaresCount];
            pieceTraArr  = new Transform[squaresCount];
            pieceObjArr  = new GameObject[squaresCount];
            for (int i = 0; i < squaresCount; i++)
            {
                squareObjArr[i] = reversiBoardTra.GetChild(i).gameObject;
                squareTraArr[i] = squareObjArr[i].transform;
            }

            //�g�p���Ȃ��}�X���\��
            foreach (int i in HIDE_SQUARE_ARR)
            { squareObjArr[i].SetActive(false); }

            //�M�~�b�N��z�u
            gimmicksCount  = GIMMICK_INFO_ARR.Length;
            gimmickObjArr  = new GameObject[gimmicksCount];
            gimmickInfoArr = new GimmickInformation[gimmicksCount];
            for (int i = 0; i < gimmicksCount; i++)
            { GeneraeGimmick(i); }

            //��̃����_���z�u
            for (int i = 0; i < squaresCount; i++)
            {
                if (!squareObjArr[i].activeSelf) continue; //��\���}�X�͏������΂�
                if (pieceObjArr[i] != null)      continue; //�M�~�b�N�}�X�͏������΂�

                int pieceGeneIndex = UnityEngine.Random.Range(0, USE_PIECE_COUNT);
                GeneratePiece(pieceGeneIndex, i);
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
        /// /// <param name="prefabIndex">������v���n�u�ԍ�</param>
        /// /// <param name="pieceIndex"> ��Ǘ��ԍ�</param>
        void GeneratePiece(int prefabIndex, int pieceIndex)
        {
            pieceObjArr[pieceIndex] = Instantiate(piecePrefabArr[prefabIndex]);
            pieceTraArr[pieceIndex] = pieceObjArr[pieceIndex].transform;
            pieceTraArr[pieceIndex].SetParent(squareTraArr[pieceIndex], false);
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
            pieceObjArr[pieceIndex] = null;
            pieceObjArr[pieceIndex] = null;
        }

        /// <summary>
        /// �M�~�b�N�쐬
        /// </summary>
        /// /// <param name="gimmickIndex">�M�~�b�N�Ǘ��ԍ�</param>
        void GeneraeGimmick(int gimmickIndex)
        {
            int colorNum = (GIMMICK_INFO_ARR[gimmickIndex][COLOR] < 0) ? 0 : GIMMICK_INFO_ARR[gimmickIndex][COLOR];
            gimmickObjArr[gimmickIndex] = Instantiate(gimmickPrefabArr[GIMMICK_INFO_ARR[gimmickIndex][GIMMICK]].prefab[colorNum]);

            //Component�擾
            gimmickInfoArr[gimmickIndex] = gimmickObjArr[gimmickIndex].GetComponent<GimmickInformation>();
            gimmickInfoArr[gimmickIndex].informationSetting(gimmickIndex);

            //��Ƃ��Ă��Ǘ�����
            pieceObjArr[GIMMICK_INFO_ARR[gimmickIndex][SQUARE]] = gimmickObjArr[gimmickIndex];
            pieceTraArr[GIMMICK_INFO_ARR[gimmickIndex][SQUARE]] = gimmickObjArr[gimmickIndex].transform;
            pieceTraArr[GIMMICK_INFO_ARR[gimmickIndex][SQUARE]].SetParent(squareTraArr[GIMMICK_INFO_ARR[gimmickIndex][SQUARE]], false);
            pieceTraArr[GIMMICK_INFO_ARR[gimmickIndex][SQUARE]].localPosition = gimmickInfoArr[gimmickIndex].defaultPos;
        }

        /// <summary>
        /// �M�~�b�N�폜
        /// </summary>
        /// /// <param name="pieceIndex">�폜��̊Ǘ��ԍ�</param>
        void DeleteGimmick(GameObject gimmickObj)
        {
            int gimmickIndex = Array.IndexOf(gimmickObjArr, gimmickObj);
            gimmickObjArr[gimmickIndex] = null;

            int pieceIndex = Array.IndexOf(pieceObjArr, gimmickObj);
            DeletePiece(pieceIndex);
        }


        /// <summary>
        /// �Ǘ��ԍ��̍X�V
        /// </summary>
        /// <param name="oldIndex">�X�V�O�̊Ǘ��ԍ�</param>
        /// <param name="newIndex">�X�V��̊Ǘ��ԍ�</param>
        void UpdateManagementIndex(int oldIndex, int newIndex)
        {
            pieceObjArr[newIndex] = pieceObjArr[oldIndex];
            pieceTraArr[newIndex] = pieceTraArr[oldIndex];
            pieceTraArr[newIndex].SetParent(squareTraArr[newIndex], true);
            pieceObjArr[oldIndex] = null;
            pieceTraArr[oldIndex] = null;
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
            if (Array.IndexOf(pieceObjArr, tapObj) >= 0) StartCoroutine(PutPieceToSquare(tapObj));

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

            //�폜�����̃}�X�ɐ擪�̑ҋ@����Z�b�g����
            int putIndex = Array.IndexOf(pieceObjArr, deletePiece);
            nextPieceTraArr[nextPuPieceIndex].SetParent(squareTraArr[putIndex], true);

            //��g��
            Coroutine scaleUpCoroutine = StartCoroutine(AllScaleChange(nextPieceTraArr[nextPuPieceIndex], PUT_PIECE_SCALING_SPEED, PUT_PIECE_CHANGE_SCALE));

            //�ҋ@��̈ړ�
            Vector3 nowPos = nextPieceTraArr[nextPuPieceIndex].localPosition;
            nextPieceTraArr[nextPuPieceIndex].localPosition = new Vector3(nowPos.x, nowPos.y, PUT_PIECE_MOVE_START_Z);
            yield return StartCoroutine(DecelerationMovement(nextPieceTraArr[nextPuPieceIndex], PUT_PIECE_MOVE_SPEED, PIECE_DEFAULT_POS));

            //��폜,�Ǘ��z�񍷂��ւ�
            DeletePiece(putIndex);
            pieceObjArr[putIndex] = nextPieceObjArr[nextPuPieceIndex];
            pieceTraArr[putIndex] = nextPieceTraArr[nextPuPieceIndex];

            //�ҋ@���
            int pieceGeneIndex = UnityEngine.Random.Range(0, USE_PIECE_COUNT);
            GenerateNextPiece(pieceGeneIndex, nextPuPieceIndex);

            //90����]
            nextPieceTraArr[nextPuPieceIndex].localRotation = PIECE_GENERATE_QUA;
            StartCoroutine(RotateMovement(nextPieceTraArr[nextPuPieceIndex], REVERSE_PIECE_ROT_SPEED, REVERSE_PIECE_FRONT_ROT));

            //��k��
            StopCoroutine(scaleUpCoroutine);
            yield return StartCoroutine(AllScaleChange(pieceTraArr[putIndex], PUT_PIECE_SCALING_SPEED, PIECE_DEFAULT_SCALE));
            pieceTraArr[putIndex].localScale = new Vector2(PIECE_DEFAULT_SCALE, PIECE_DEFAULT_SCALE);

            //���]��X�g�擾
            string putPieceTag = pieceObjArr[putIndex].tag;  //�u������̃^�O�擾
            List<int[]> reversIndexList = new List<int[]>(); //���]��̊Ǘ��ԍ��i�[���X�g
            if (GetReversIndex(putIndex, ref putPieceTag, ref reversIndexList))
            {
                //�폜�Ώۂɒu������̊Ǘ��ԍ��ǉ�
                destroyPiecesIndexList.Add(putIndex);

                //���]�J�n
                StartCoroutine(StratReversingPieces(putPieceTag, reversIndexList));
            }
            else
            {
                //�M�~�b�N��ԕω��J�n
                StartCoroutine(StartChangeGimmickState());
            }

            //�z�u���t���O���Z�b�g
            NOW_PUTTING_PIECES = false;
        }

        /// <summary>
        /// ��̔��]����
        /// </summary>
        /// <param name="putPieceIndex">  �u������̊Ǘ��ԍ�</param>
        /// <param name="putPieceTag">    �u������̃^�O</param>
        /// <param name="reversIndexList">���]��i�[���X�g(�Q��)</param>
        /// <returns>���]��̗L��</returns>
        bool GetReversIndex(int putPieceIndex, ref string putPieceTag, ref List<int[]> reversIndexList)
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
                reversIndexList.Add(GetReversIndex_SpecifiedDirection(ref putPieceIndex, ref putPieceTag, ref loopCount, directionsInt));
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
        /// <param name="putPieceIndex">���̊Ǘ��ԍ�</param>
        /// <param name="putPieceTag">  ���̃^�O</param>
        /// <param name="loopCount">    �w������ɂ���}�X�̐�</param>
        /// <param name="direction">    �w������̊Ǘ��ԍ�</param>
        /// <returns>�w������̔��]�I�u�W�F�N�g�̊Ǘ��ԍ��z��</returns>
        int[] GetReversIndex_SpecifiedDirection(ref int putPieceIndex, ref string putPieceTag, ref int loopCount, int direction)
        {
            List<int> reversIndexList = new List<int>();
            for (int i = 1; i <= loopCount; i++)
            {
                //�w������̃C���f�b�N�X�ԍ��擾
                int refIndex = GetPlaceIndex(ref direction, ref putPieceIndex, ref i);

                //��}�X�̏ꍇ��null��Ԃ�
                if (pieceObjArr[refIndex] == null) return null;

                //�M�~�b�N�}�X�̏ꍇ�̓_���[�W��^�����邩�̊m�F
                if (pieceObjArr[refIndex].tag == GIMMICK_TAG)
                {
                    //�_���[�W���^�����Ȃ��ꍇ��null��Ԃ�
                    int gimmickIndex = Array.IndexOf(gimmickObjArr, pieceObjArr[refIndex]);
                    if (!gimmicksMan.DamageCheck(ref putPieceTag, ref gimmickIndex))
                        return null;
                }

                //���^�O�������
                if (pieceObjArr[refIndex].tag == putPieceTag)
                {
                    //�ׂ����^�O�̏ꍇ��null��Ԃ�
                    if (i == 1) return null;

                    //�폜�Ώۂɓ��^�O��̊Ǘ��ԍ��ǉ�
                    destroyPiecesIndexList.Add(refIndex);
                    break;
                }
                reversIndexList.Add(refIndex);

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
        /// <param name="putPieceTag">    �u������̃^�O</param>
        /// <param name="reversIndexList">���]��i�[���X�g</param>
        /// <returns></returns>
        IEnumerator StratReversingPieces(string putPieceTag, List<int[]> reversIndexList)
        {
            //���]���t���O�Z�b�g
            NOW_REVERSING_PIECES = true;

            //���]�J�n
            int prefabIndex = Array.IndexOf(pieceTagsArr, putPieceTag);
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
                        gimmicksMan.DamageGimmick(ref putPieceTag, ref gimmickIndex, reversIndex);
                        yield return PIECE_REVERSAL_INTERVAL;
                        continue;
                    }

                    //�폜�Ώۂɔ��]��̊Ǘ��ԍ��ǉ�
                    destroyPiecesIndexList.Add(reversIndex);

                    //���]
                    coroutine = StartCoroutine(ReversingPieces(reversIndex, prefabIndex));
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
            int destroyPiecesCount = destroyPiecesIndexList.Count;
            Coroutine coroutine = null;
            for (int i = 0; i < destroyPiecesCount; i++)
            {
                if (pieceTraArr[destroyPiecesIndexList[i]].gameObject.tag == GIMMICK_TAG) continue;
                coroutine = StartCoroutine(AllScaleChange(pieceTraArr[destroyPiecesIndexList[i]], DESTROY_PIECE_SCALING_SPEED, DESTROY_PIECE_CHANGE_SCALE));
            }
            yield return coroutine;

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
            Coroutine coroutine = null;
            foreach (int fallPieceIndex in fallPiecesIndexList)
            {
                if (pieceTraArr[fallPieceIndex] != null)
                {
                    Vector3 targetPos = PIECE_DEFAULT_POS;
                    int gimmickIndex = Array.IndexOf(gimmickObjArr, pieceTraArr[fallPieceIndex].gameObject);
                    if (gimmickIndex >= 0) targetPos = gimmickInfoArr[gimmickIndex].defaultPos;
                    coroutine = StartCoroutine(ConstantSpeedMovement(pieceTraArr[fallPieceIndex], FALL_PIECE_MOVE_SPEED, targetPos, FALL_PIECE_ACCELE_RATE));
                }
            }
            yield return coroutine;

            //�M�~�b�N��ԕω��J�n
            StartCoroutine(StartChangeGimmickState());

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
                        //���R���������M�~�b�N�ł͂Ȃ��ꍇ�A�Ǘ��ԍ��X�V
                        if (pieceObjArr[refIndex].tag != GIMMICK_TAG || GimmickFreeFallFlag(ref pieceObjArr[refIndex]))
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
        /// �M�~�b�N�̎��R�����t���O�擾
        /// </summary>
        /// <param name="pieceObj"></param>
        /// <returns></returns>
        bool GimmickFreeFallFlag(ref GameObject pieceObj)
        {
            int gimObjIndex = Array.IndexOf(gimmickObjArr, pieceObj);
            int gimId = GIMMICK_INFO_ARR[gimObjIndex][GIMMICK];
            return GIMMICK_FREE_FALL[gimId];
        }


        //==========================================================//
        //------------------�M�~�b�N��ԕω�����--------------------//
        //==========================================================//

        /// <summary>
        /// �M�~�b�N�̏�ԕω��J�n
        /// </summary>
        /// <returns></returns>
        IEnumerator StartChangeGimmickState()
        {
            Coroutine coroutine = null;
            for (int i = 0; i < gimmicksCount; i++)
            {
                if (gimmickObjArr[i] == null) continue;
                switch (GIMMICK_INFO_ARR[i][GIMMICK])
                {
                    case (int)Gimmicks.Jewelry: //���
                    case (int)Gimmicks.Hamster: //�n���X�^�[
                        coroutine = StartCoroutine(gimmicksMan.ChangeGimmickState(i));
                        break;
                }
            }
            yield return coroutine;

            //�^�[���I��
            TurnEnd();
        }

        /// <summary>
        /// �^�[���I��
        /// </summary>
        void TurnEnd()
        {
            //�M�~�b�N�̃t���O���Z�b�g
            foreach (GimmickInformation gimmickInfo in gimmickInfoArr)
            { gimmickInfo.nowTurnDamage = false; }
        }
    }
}