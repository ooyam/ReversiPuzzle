using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;

public class GimmickInformation : MonoBehaviour
{
    //�R���|�[�l���g
    [System.NonSerialized] public SpriteRenderer spriRen;
    [System.NonSerialized] public Animator ani;
    [System.NonSerialized] public Transform tra;

    //�M�~�b�N���
    [System.NonSerialized] public int     objIndex;       //�X�e�[�W���̊Ǘ��ԍ�
    [System.NonSerialized] public int     id;             //�M�~�b�N�ԍ�
    [System.NonSerialized] public int     colorNum;       //�F�ԍ�
    [System.NonSerialized] public int     remainingTimes; //�c��_���[�W��
    [System.NonSerialized] public bool    freeFall;       //���R�����t���O
    [System.NonSerialized] public bool    destructible;   //�A���_���[�W�t���O(true�̎��j��\)
    [System.NonSerialized] public bool    nowTurnDamage;  //���̃^�[���Ƀ_���[�W���󂯂����̃t���O
    [System.NonSerialized] public Vector3 defaultPos;     //����W

    void Start()
    {
        spriRen = this.GetComponent<SpriteRenderer>();
        ani     = this.GetComponent<Animator>();
        tra     = this.transform;
    }

    /// <summary>
    /// �M�~�b�N���̐ݒ�
    /// </summary>
    /// <param name="index"></param>
    public void informationSetting(int index)
    {
        objIndex       = index;
        id             = GIMMICK_INFO_ARR[index][GIMMICK];
        colorNum       = GIMMICK_INFO_ARR[index][COLOR];
        remainingTimes = GIMMICK_DAMAGE_TIMES[id];
        freeFall       = GIMMICK_FREE_FALL[id];
        destructible   = !GIMMICK_CONTINUOUS[id];
        defaultPos = new Vector3(GIMMICK_POS_X[id], GIMMICK_POS_Y[id], PIECE_DEFAULT_POS.z);
    }
}