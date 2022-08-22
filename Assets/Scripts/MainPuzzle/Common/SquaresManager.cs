using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static CommonDefine;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static ObjectMove_2D.ObjectMove_2D;
using static Sound.SoundManager;

namespace PuzzleMain
{
    public class SquaresManager : MonoBehaviour
    {
        [Header("���o�[�V�Ղ̎擾")]
        [SerializeField]
        Transform mReversiBoardTra;

        [Header("�ҋ@��{�b�N�X�̎擾")]
        [SerializeField]
        Transform mNextPieceBoxesTra;

        Transform mNextPieceFrameTra;         //���ɒu���R�}�̎w��t���[��
        SpriteRenderer[] mSquareSpriRenArr;   //�}�XSpriteRenderer�z��

        /// <summary>
        /// PiecesManager�̏�����
        /// </summary>
        public void Initialize()
        {
            //�}�X�擾
            sSquareObjArr = new GameObject[SQUARES_COUNT];
            sSquareTraArr = new Transform[SQUARES_COUNT];
            mSquareSpriRenArr = new SpriteRenderer[SQUARES_COUNT];
            for (int i = 0; i < SQUARES_COUNT; i++)
            {
                sSquareObjArr[i] = mReversiBoardTra.GetChild(i).gameObject;
                sSquareTraArr[i] = sSquareObjArr[i].transform;
                mSquareSpriRenArr[i] = sSquareObjArr[i].GetComponent<SpriteRenderer>();
            }

            //�g�p���Ȃ��}�X���\��
            foreach (int i in HIDE_SQUARE_ARR)
            {
                if (i < 0 || i >= SQUARES_COUNT) continue;
                sSquareObjArr[i].SetActive(false);
            }

            //�ҋ@��̔��擾
            sNextPiecesCount = mNextPieceBoxesTra.childCount;
            sNextPieceBoxTraArr = new Transform[sNextPiecesCount];
            for (int i = 0; i < sNextPiecesCount; i++)
            {
                sNextPieceBoxTraArr[i] = mNextPieceBoxesTra.GetChild(i).transform;
            }

            //���ɒu���R�}�̎w��t���[���擾
            mNextPieceFrameTra = sNextPieceBoxTraArr[0].GetChild(0).gameObject.transform;
        }

        /// <summary>
        /// ��ԍ��擾
        /// </summary>
        /// <param name="_squareId">�}�X�Ǘ��ԍ�</param>
        /// <returns></returns>
        public int GetColumnNumber(int _squareId)
        {
            return _squareId / BOARD_LINE_COUNT;
        }

        /// <summary>
        /// �s�ԍ��擾
        /// </summary>
        /// <param name="_squareId">�}�X�Ǘ��ԍ�</param>
        /// <returns></returns>
        public int GetLineNumber(int _squareId)
        {
            return _squareId % BOARD_LINE_COUNT;
        }

        /// <summary>
        /// �}�X�����݂��邩�H
        /// </summary>
        /// <param name="_squareId">�}�X�Ǘ��ԍ�</param>
        /// <returns></returns>
        public bool IsSquareExists(int _squareId)
        {
            if (0 <= _squareId && _squareId < SQUARES_COUNT)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// �}�X�̓A�N�e�B�u���H
        /// </summary>
        /// <param name="_squareId">�}�X�Ǘ��ԍ�</param>
        /// <returns></returns>
        public bool IsSquareActive(int _squareId)
        {
            if (IsSquareExists(_squareId))
            {
                return sSquareObjArr[_squareId].activeSelf;
            }
            return false;
        }

        /// <summary>
        /// �}�X�̐F�ύX
        /// </summary>
        /// <param name="_afterColor"> �ω���̐F</param>
        /// <param name="_squareId">   �}�X�Ǘ��ԍ�</param>
        /// <param name="_fade">       �t�F�[�h�̗L��</param>
        public IEnumerator SquareColorChange(Color _afterColor, int _squareId, bool _fade)
        {
            if (!_fade) mSquareSpriRenArr[_squareId].color = _afterColor;
            else yield return StartCoroutine(SpriteRendererPaletteChange(mSquareSpriRenArr[_squareId], SQUARE_CHANGE_SPEED, new Color[] { mSquareSpriRenArr[_squareId].color, _afterColor }));
        }

        /// <summary>
        /// �I�u�W�F�N�g������}�X�Ǘ��ԍ����擾
        /// </summary>
        /// <param name="_obj"></param>
        /// <returns></returns>
        public int GetSquareId(GameObject _obj)
        {
            return Array.IndexOf(sPieceObjArr, _obj);
        }

        /// <summary>
        /// ��������̎w��t���[���ړ�
        /// </summary>
        /// <param name="_nextPuPieceIndex">��������ԍ�</param>
        /// <returns></returns>v
        public void MoveNextPieceFrame(int _nextPuPieceIndex)
        {
            //SE�Đ�
            SE_Onshot(SE_Type.PieceSelect);

            //�ړ�
            mNextPieceFrameTra.SetParent(sNextPieceBoxTraArr[_nextPuPieceIndex], false);
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
        public int SetLoopCount(Directions directions, ref int up, ref int right, ref int down, ref int left)
        {
            return directions switch
            {
                Directions.Up        => up,                              //��
                Directions.UpRight   => (up <= right) ? up : right,      //�E��
                Directions.Right     => right,                           //�E
                Directions.DownRight => (down <= right) ? down : right,  //�E��
                Directions.Down      => down,                            //��
                Directions.DownLeft  => (down <= left) ? down : left,    //����
                Directions.Left      => left,                            //��
                Directions.UpLeft    => (up <= left) ? up : left,        //����
                _ => 0, //default
            };
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
            //������0�ȉ��̏ꍇ�̓}�X��������
            if (distance <= 0) return INT_NULL;

            bool difDir = false;    //�����Y���t���O
            int square = INT_NULL;  //�w��ꏊ�}�X�ԍ�

            //��������m�F�p�ϐ�
            int baseLine = GetLineNumber(baseIndex);
            int baseColumn = GetColumnNumber(baseIndex);
            int offsetLine;
            int offsetColumn;

            switch (direction)
            {
                //��
                case (int)Directions.Up:
                    square = baseIndex - distance;
                    difDir = GetColumnNumber(baseIndex) != GetColumnNumber(square);
                    break;

                //�E��
                case (int)Directions.UpRight:
                    square = baseIndex + BOARD_LINE_COUNT * distance - distance;
                    offsetLine = GetLineNumber(square) - baseLine;
                    offsetColumn = GetColumnNumber(square) - baseColumn;
                    difDir = offsetLine >= 0 || offsetColumn <= 0 || -offsetLine != offsetColumn;
                    break;

                //�E
                case (int)Directions.Right:
                    square = baseIndex + BOARD_LINE_COUNT * distance;
                    break;

                //�E��
                case (int)Directions.DownRight:
                    square = baseIndex + BOARD_LINE_COUNT * distance + distance;
                    offsetLine = GetLineNumber(square) - baseLine;
                    offsetColumn = GetColumnNumber(square) - baseColumn;
                    difDir = offsetLine <= 0 || offsetColumn <= 0 || offsetLine != offsetColumn;
                    break;

                //��
                case (int)Directions.Down:
                    square = baseIndex + distance;
                    difDir = GetColumnNumber(baseIndex) != GetColumnNumber(square);
                    break;

                //����
                case (int)Directions.DownLeft:
                    square = baseIndex - BOARD_LINE_COUNT * distance + distance;
                    offsetLine = GetLineNumber(square) - baseLine;
                    offsetColumn = GetColumnNumber(square) - baseColumn;
                    difDir = offsetLine <= 0 || offsetColumn >= 0 || offsetLine != -offsetColumn;
                    break;

                //��
                case (int)Directions.Left:
                    square = baseIndex - BOARD_LINE_COUNT * distance;
                    break;

                //����
                case (int)Directions.UpLeft:
                    square = baseIndex - BOARD_LINE_COUNT * distance - distance;
                    offsetLine = GetLineNumber(square) - baseLine;
                    offsetColumn = GetColumnNumber(square) - baseColumn;
                    difDir = offsetLine >= 0 || offsetColumn >= 0 || offsetLine != offsetColumn;
                    break;
            }

            //�����Y���������̓}�X�����݂��Ȃ��ꍇ
            if (difDir || !IsSquareExists(square)) square = INT_NULL;

            //�}�X�ԍ���Ԃ�
            return square;
        }
    }
}