using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PuzzleDefine;
using static PuzzleDefine.PuzzleDefine;
using static ObjectMove_2D.ObjectMove_2D;

public class PieceManager : MonoBehaviour
{
    [Header("��v���n�u�̎擾")]
    [SerializeField]
    GameObject[] piecePrefabArr;
 
    [Header("���o�[�V�Ղ̎擾")]
    [SerializeField]
    Transform reversiBoardTra;

    [Header("�ҋ@��{�b�N�X�̎擾")]
    [SerializeField]
    Transform nextPieceBoxesTra;

    string[]     pieceTagsArr;         //��^�O�z��
    GameObject[] pieceObjArr;          //��̃I�u�W�F�N�g�z��
    Transform[]  pieceTraArr;          //���Transform�z��
    GameObject[] squareObjArr;         //�}�X�̃I�u�W�F�N�g�z��
    Transform[]  squareTraArr;         //�}�X��Transform�z��
    GameObject[] nextPieceObjArr;      //�ҋ@��̃I�u�W�F�N�g�z��
    Transform[]  nextPieceTraArr;      //�ҋ@���Transform�z��
    GameObject[] nextPieceBoxObjArr;   //�ҋ@��̔��I�u�W�F�N�g�z��
    Transform[]  nextPieceBoxTraArr;   //�ҋ@��̔�Transform�z��
    int squaresCount;                  //�}�X�̌�
    int nextPiecesCount;               //�ҋ@��̌�
    int[] directionsIntArr;            //8�����̊Ǘ��ԍ��z��

    List<int> destroyPiecesIndexList = new List<int>(); //�폜��̊Ǘ��ԍ����X�g

    //==========================================================//
    //----------------------�����ݒ�,�擾-----------------------//
    //==========================================================//

    void Start()
    {
        //��̃^�O�擾
        System.Array pieceColors = Enum.GetValues(typeof(PieceColors));
        pieceTagsArr = new string[pieceColors.Length];
        foreach (PieceColors pieceColor in pieceColors)
        { pieceTagsArr[(int)pieceColor] = Enum.GetName(typeof(PieceColors), pieceColor); }

        //8�����̊Ǘ��ԍ��擾
        System.Array directions = Enum.GetValues(typeof(Directions));
        directionsIntArr = new int[directions.Length];
        foreach (Directions direction in directions)
        { directionsIntArr[(int)direction] = (int)direction; }

        //�}�X�擾
        squaresCount = BOARD_COLUMN_COUNT * BOARD_LINE_COUNT;
        squareObjArr = new GameObject[squaresCount];
        squareTraArr = new Transform[squaresCount];
        for (int i = 0; i < squaresCount; i++)
        {
            squareObjArr[i] = reversiBoardTra.GetChild(i).gameObject;
            squareTraArr[i] = squareObjArr[i].transform;
        }

        //�g�p���Ȃ��}�X���\��
        foreach (int i in HIDE_SQUARE)
        { squareObjArr[i].SetActive(false); }

        //��̃����_���z�u
        pieceTraArr = new Transform[squaresCount];
        pieceObjArr = new GameObject[squaresCount];
        for (int i = 0; i < squaresCount; i++)
        {
            //��\���}�X�͏������΂�
            if (!squareObjArr[i].activeSelf) continue;
            int pieceGeneIndex = UnityEngine.Random.Range(0, USE_PIECE_COUNT);
            GeneratePiece(pieceGeneIndex, i);
        }

        //�ҋ@��̔��擾
        nextPiecesCount     = nextPieceBoxesTra.childCount;
        nextPieceBoxObjArr  = new GameObject[nextPiecesCount];
        nextPieceBoxTraArr  = new Transform[nextPiecesCount];
        for (int i = 0; i < nextPiecesCount; i++)
        {
            nextPieceBoxObjArr[i] = nextPieceBoxesTra.GetChild(i).gameObject;
            nextPieceBoxTraArr[i] = nextPieceBoxObjArr[i].transform;
        }

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
    /// �}�X�ɋ��u��
    /// </summary>
    /// <param name="deletePiece">�폜��</param>
    public IEnumerator PutPieceToSquare(GameObject deletePiece)
    {
        //�Տ�̋�łȂ���Ώ������Ȃ�
        if (Array.IndexOf(pieceObjArr, deletePiece) < 0) yield break;

        //�z�u���t���O�Z�b�g
        NOW_PUTTING_PIECES = true;

        //�폜�����̃}�X�ɐ擪�̑ҋ@����Z�b�g����
        int putIndex = Array.IndexOf(pieceObjArr, deletePiece);
        nextPieceTraArr[0].SetParent(squareTraArr[putIndex], true);

        //��g��
        Coroutine scaleUpCoroutine = StartCoroutine(AllScaleChange(nextPieceTraArr[0], PUT_PIECE_SCALING_SPEED, PUT_PIECE_CHANGE_SCALE));

        //�ҋ@��̈ړ�
        Vector3 nowPos = nextPieceTraArr[0].localPosition;
        nextPieceTraArr[0].localPosition = new Vector3(nowPos.x, nowPos.y, PUT_PIECE_MOVE_START_Z);
        yield return StartCoroutine(DecelerationMovement(nextPieceTraArr[0], PUT_PIECE_MOVE_SPEED, PIECE_DEFAULT_POS));

        //��폜,�Ǘ��z�񍷂��ւ�
        DeletePiece(putIndex);
        pieceObjArr[putIndex] = nextPieceObjArr[0];
        pieceTraArr[putIndex] = nextPieceTraArr[0];

        //�ҋ@����J��グ��
        for (int i = 0; i < nextPiecesCount - 1; i++)
        {
            int n = i + 1;
            nextPieceTraArr[n].SetParent(nextPieceBoxTraArr[i], true);
            nextPieceObjArr[i] = nextPieceObjArr[n];
            nextPieceTraArr[i] = nextPieceTraArr[n];
            StartCoroutine(DecelerationMovement(nextPieceTraArr[i], NEXT_PIECE_SLIDE_SPEED, PIECE_DEFAULT_POS));
        }

        //�ҋ@���
        int pieceGeneIndex = UnityEngine.Random.Range(0, USE_PIECE_COUNT);
        int nextPieceIndex = nextPiecesCount - 1;
        GenerateNextPiece(pieceGeneIndex, nextPieceIndex);

        //90����]
        nextPieceTraArr[nextPieceIndex].localRotation = PIECE_GENERATE_QUA;
        StartCoroutine(RotateMovement(nextPieceTraArr[nextPieceIndex], REVERSE_PIECE_ROT_SPEED, REVERSE_PIECE_FRONT_ROT));

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
            case (int)Directions.Up: return up;                              //��
            case (int)Directions.UpRight: return (up <= right) ? up : right;      //�E��
            case (int)Directions.Right: return right;                           //�E
            case (int)Directions.DownRight: return (down <= right) ? down : right;  //�E��
            case (int)Directions.Down: return down;                            //��
            case (int)Directions.DownLeft: return (down <= left) ? down : left;    //����
            case (int)Directions.Left: return left;                            //��
            case (int)Directions.UpLeft: return (up <= left) ? up : left;        //����
            default: return 0;
        }
    }

    /// <summary>
    /// �w������̔��]�I�u�W�F�N�g�̊Ǘ��ԍ��擾
    /// </summary>
    /// <param name="putPieceIndex">  ���̊Ǘ��ԍ�</param>
    /// <param name="putPieceTag">    ���̃^�O</param>
    /// <param name="loopCount">      �w������ɂ���}�X�̐�</param>
    /// <param name="direction">      �w������̊Ǘ��ԍ�</param>
    /// <returns>�w������̔��]�I�u�W�F�N�g�̊Ǘ��ԍ��z��</returns>
    int[] GetReversIndex_SpecifiedDirection(ref int putPieceIndex, ref string putPieceTag, ref int loopCount, int direction)
    {
        List<int> reversIndexList = new List<int>();
        for (int i = 1; i <= loopCount; i++)
        {
            //�w������̓��^�O�������
            int refIndex = GetPlaceIndex(ref direction, ref putPieceIndex, ref i);
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
            case (int)Directions.Up: return baseIndex - distance;                                //��
            case (int)Directions.UpRight: return baseIndex + BOARD_LINE_COUNT * distance - distance;  //�E��
            case (int)Directions.Right: return baseIndex + BOARD_LINE_COUNT * distance;             //�E
            case (int)Directions.DownRight: return baseIndex + BOARD_LINE_COUNT * distance + distance;  //�E��
            case (int)Directions.Down: return baseIndex + distance;                                //��
            case (int)Directions.DownLeft: return baseIndex - BOARD_LINE_COUNT * distance + distance;  //����
            case (int)Directions.Left: return baseIndex - BOARD_LINE_COUNT * distance;             //��
            case (int)Directions.UpLeft: return baseIndex - BOARD_LINE_COUNT * distance - distance;  //����
            default: return 0;
        }
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
                //�폜�Ώۂɔ��]��̊Ǘ��ԍ��ǉ�
                destroyPiecesIndexList.Add(reversIndex);

                //���]
                coroutine = StartCoroutine(ReversingPieces(reversIndex, prefabIndex));
                yield return PIECE_REVERSAL_INTERVAL;
            }
            yield return PIECE_GROUP_REVERSAL_INTERVAL;
        }

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
        for (int i = 0; i < destroyPiecesCount; i++)
        {
            Coroutine coroutine = StartCoroutine(AllScaleChange(pieceTraArr[destroyPiecesIndexList[i]], DESTROY_PIECE_SCALING_SPEED, DESTROY_PIECE_CHANGE_SCALE));
            if (i == destroyPiecesCount - 1) yield return coroutine;
        }

        //��폜
        foreach (int pieceIndex in destroyPiecesIndexList)
        { DeletePiece(pieceIndex); }

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
        { coroutine = StartCoroutine(ConstantSpeedMovement(pieceTraArr[fallPieceIndex], FALL_PIECE_MOVE_SPEED, FALL_PIECE_ACCELE_RATE, PIECE_DEFAULT_POS)); }
        yield return coroutine;

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
                if (pieceObjArr[i - n] != null)
                {
                    //�Ǘ��ԍ��X�V
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
}
