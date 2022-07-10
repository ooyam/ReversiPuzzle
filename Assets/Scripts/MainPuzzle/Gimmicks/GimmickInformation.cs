using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;

public class GimmickInformation : MonoBehaviour
{
    //�R���|�[�l���g
    [System.NonSerialized] public Animator ani;
    [System.NonSerialized] public Transform tra;
    [System.NonSerialized] public SpriteRenderer   spriRen;
    [System.NonSerialized] public SpriteRenderer[] spriRenChild;
    [System.NonSerialized] public GameObject       obj;
    [System.NonSerialized] public GameObject[]     objChild;

    //�M�~�b�N���
    [System.NonSerialized] public int     settingIndex;         //�X�e�[�W���̐ݒ�ԍ�
    [System.NonSerialized] public int     startSquareId;        //�����}�X�ԍ�
    [System.NonSerialized] public int     nowSquareId;          //���݂̃}�X�ԍ�
    [System.NonSerialized] public int     groupId;              //�O���[�v�ԍ�
    [System.NonSerialized] public int     id;                   //�M�~�b�N�ԍ�
    [System.NonSerialized] public int     colorId;              //�F�ԍ�
    [System.NonSerialized] public int     remainingTimes;       //�c��_���[�W��(�M�~�b�N�̌Œ萔)
    [System.NonSerialized] public int     remainingQuantity;    //�c��_���[�W��(�X�e�[�W���̎w�萔)
    [System.NonSerialized] public int     order;                //�w��ԍ�
    [System.NonSerialized] public bool    freeFall;             //���R�����t���O
    [System.NonSerialized] public bool    destructible;         //�j��\�t���O(true�F�j��\)
    [System.NonSerialized] public bool    nowTurnDamage;        //���̃^�[���Ƀ_���[�W���󂯂����̃t���O
    [System.NonSerialized] public bool    assaultOnly;          //�����ł̂ݔj��\
    [System.NonSerialized] public bool    inSquare;             //��Ƃ��Ĕz�u���邩�̃t���O(true�F��Ƃ��Ĕz�u)
    [System.NonSerialized] public Vector3 defaultPos;           //����W
    [System.NonSerialized] public Vector3 defaultScale;         //��X�P�[��
    [System.NonSerialized] public int[]   innerSquaresId;       //�����̃}�X�ԍ�

    //��Ƃ��ĊǗ�����ꍇ�̏��
    [System.NonSerialized] public bool freeFall_Piece;          //���R�����t���O(��Ƃ���)
    [System.NonSerialized] public bool destructible_Piece;      //�j��\�t���O(true�F�j��\)


    /// <summary>
    /// �R���|�[�l���g�̐ݒ�
    /// </summary>
    void ComponentSetting()
    {
        ani     = this.GetComponent<Animator>();
        tra     = this.transform;
        spriRen = this.GetComponent<SpriteRenderer>();
        obj     = this.gameObject;

        int childCount = tra.childCount;
        objChild     = new GameObject[childCount];
        spriRenChild = new SpriteRenderer[childCount];
        for (int i = 0; i < childCount; i++)
        {
            objChild[i]     = tra.GetChild(i).gameObject;
            spriRenChild[i] = objChild[i].GetComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// �M�~�b�N���̐ݒ�
    /// </summary>
    /// <param name="_index">�X�e�[�W���̃M�~�b�N�Ǘ��ԍ�</param>
    public void InformationSetting(int _index)
    {
        ComponentSetting();
        var gimmickData     = GIMMICKS_DATA.param[GIMMICKS_INFO_ARR[_index][GIMMICK]];
        settingIndex        = _index;
        startSquareId       = GIMMICKS_INFO_ARR[_index][SQUARE];
        nowSquareId         = startSquareId;
        groupId             = GIMMICKS_INFO_ARR[_index][GROUP];
        id                  = gimmickData.id;
        colorId             = GIMMICKS_INFO_ARR[_index][COLOR];
        remainingTimes      = gimmickData.damage_times;
        remainingQuantity   = GIMMICKS_INFO_ARR[_index][QUANTITY];
        order               = GIMMICKS_INFO_ARR[_index][ORDER];
        freeFall            = gimmickData.free_fall;
        destructible        = !gimmickData.continuous;
        assaultOnly         = gimmickData.assault_only;
        inSquare            = gimmickData.in_square;
        defaultPos          = new Vector3(gimmickData.position_x, gimmickData.position_y, (inSquare) ? Z_PIECE : Z_GIMMICK);
        defaultScale        = new Vector3(gimmickData.scale_x, gimmickData.scale_y, 1.0f);

        switch (id)
        {
            //�M�~�b�N�����̋�𑀍�֎~�ɂ���M�~�b�N
            case (int)Gimmicks.Cage:    //�B
                innerSquaresId = new int[GIMMICKS_INFO_ARR[_index][WIDTH] * GIMMICKS_INFO_ARR[_index][HEIGHT]];
                int i = 0;
                for (int w = 0; w < GIMMICKS_INFO_ARR[_index][WIDTH]; w++)      //�������[�v
                {
                    for (int h = 0; h < GIMMICKS_INFO_ARR[_index][HEIGHT]; h++) //���������[�v
                    {
                        innerSquaresId[i] = startSquareId + w * BOARD_LINE_COUNT + h;
                        i++;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// �M�~�b�N���̐ݒ�(�}�X�ԍ�����)
    /// </summary>
    /// <param name="_squareIndex">�}�X�Ǘ��ԍ�</param>
    /// <param name="_gimmickId">  �M�~�b�N�ԍ�</param>
    /// <param name="_groupId">    �O���[�v�ԍ�</param>
    public void InformationSetting_SquareIndex(int _squareIndex, int _gimmickId, int _groupId)
    {
        for (int i = 0; i < GIMMICKS_COUNT; i++)
        {
            if (GIMMICKS_INFO_ARR[i][SQUARE] == _squareIndex && 
                (GIMMICKS_INFO_ARR[i][GIMMICK] == _gimmickId || GIMMICKS_INFO_ARR[i][GROUP] == _groupId))
            {
                InformationSetting(i);
                break;
            }
        }
    }


    /// <summary>
    /// ��Ƃ��ăt���O�ݒ�
    /// </summary>
    /// <param name="_squareIndex">     �z�u�}�X�Ǘ��ԍ�</param>
    /// <param name="_generate">        �����H</param>
    /// <param name="gimmickInfoArr">   �M�~�b�N���z��</param>
    public void OperationFlagSetting(int _squareIndex, bool _generate, GimmickInformation[] gimmickInfoArr = null)
    {
        OperationFlagON();
        if (!_generate || !inSquare) return;

        foreach (GimmickInformation gimmickInfo in gimmickInfoArr)
        {
            if (gimmickInfo == null) continue;
            if (gimmickInfo.innerSquaresId == null) continue;
            foreach (int squareId in gimmickInfo.innerSquaresId)
            {
                //�M�~�b�N�̓����ɐ������ꂽ�ꍇ
                if (squareId == _squareIndex)
                {
                    OperationFlagOFF();
                }
            }
        }
    }

    /// <summary>
    /// ����t���O���I���ɂ���
    /// </summary>
    public void OperationFlagON()
    {
        freeFall_Piece = true;
        destructible_Piece = true;
    }

    /// <summary>
    /// ����t���O���I���ɂ���
    /// </summary>
    public void OperationFlagOFF()
    {
        freeFall_Piece = false;
        destructible_Piece = false;
    }
}