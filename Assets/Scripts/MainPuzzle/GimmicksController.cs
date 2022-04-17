using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static PuzzleDefine;
using static PieceManager;

public class GimmicksController : MonoBehaviour
{
    /// <summary>
    /// �M�~�b�N�Ƀ_���[�W�����邩�̊m�F
    /// </summary>
    /// /// <param name="putPieceTag"> �u������̃^�O</param>
    /// /// <param name="gimmickIndex">�M�~�b�N�Ǘ��ԍ�(�X�e�[�W����)</param>
    /// /// <param name="squareIndex"> �M�~�b�N�z�u�ԍ�</param>
    /// /// <param name="gimmickObj">  �M�~�b�N�I�u�W�F�N�g</param>
    public bool DamageCheck(ref string putPieceTag, ref int gimmickIndex, int squareIndex, ref GameObject gimmickObj)
    {
        //�_���[�W�̗L���t���O
        bool damage = false;

        switch (GIMMICK_INFO_ARR[gimmickIndex][GIMMICK])
        {
            case (int)Gimmicks.Balloon:  //���D
            case (int)Gimmicks.Wall:     //��
                damage = true;
                break;

            case (int)Gimmicks.Balloon_Color: //���D(�F)
                //�F����
                int balloonColorNum = GIMMICK_INFO_ARR[gimmickIndex][COLOR];
                Colors pieceColor = (Colors)Enum.ToObject(typeof(Colors), balloonColorNum);
                if (pieceColor.ToString() == putPieceTag) damage = true;
                break;
        }

        //���ʂ�Ԃ�
        return damage;
    }

    /// <summary>
    /// �M�~�b�N�Ƀ_���[�W
    /// </summary>
    /// /// <param name="putPieceTag"> �u������̃^�O</param>
    /// /// <param name="gimmickIndex">�M�~�b�N�Ǘ��ԍ�(�X�e�[�W����)</param>
    /// /// <param name="squareIndex"> �M�~�b�N�z�u�ԍ�</param>
    /// /// <param name="gimmickObj">  �M�~�b�N�I�u�W�F�N�g</param>
    /// /// <param name="gimmickRemainingTimes"> �M�~�b�N�c��_���[�W��</param>
    public void DamageGimmick(ref string putPieceTag, ref int gimmickIndex, int squareIndex, ref GameObject gimmickObj, ref int gimmickRemainingTimes)
    {
        switch (GIMMICK_INFO_ARR[gimmickIndex][GIMMICK])
        {
            //----------------------
            //���D
            //----------------------
            case (int)Gimmicks.Balloon:
                gimmickCorList.Add(StartCoroutine(DamageAnimationStart(gimmickObj, COLORLESS_ANI_STATE_NAME)));
                break;

            //----------------------
            //���D(�F)
            //----------------------
            case (int)Gimmicks.Balloon_Color:
                gimmickCorList.Add(StartCoroutine(DamageAnimationStart(gimmickObj, putPieceTag)));
                break;

            //----------------------
            //��
            //----------------------
            case (int)Gimmicks.Wall:
                int fixNum = GIMMICK_DAMAGE_TIMES[(int)Gimmicks.Wall] + 1;
                string stateName = "WallDamage" + (-gimmickRemainingTimes + fixNum).ToString();
                gimmickCorList.Add(StartCoroutine(DamageAnimationStart(gimmickObj, stateName)));
                break;
        }

        //�_���[�W�񐔌v�Z
        gimmickRemainingTimes--;

        //�_���[�W�c�񐔂�0�Ŕj��
        if (gimmickRemainingTimes <= 0)
            destroyPiecesIndexList.Add(squareIndex);
    }

    /// <summary>
    /// �_���[�W�A�j���[�V�����Đ�
    /// </summary>
    /// <param name="obj">      �j�󂷂�I�u�W�F�N�g</param>
    /// <param name="stateName">�Đ��A�j���[�V�����X�e�[�g��</param>
    IEnumerator DamageAnimationStart(GameObject obj, string stateName)
    {
        Animator ani = obj.GetComponent<Animator>();

        //�A�j���[�V�����J�n
        ani.Play(stateName, 0, 0.0f);
        ani.speed = 1.0f;

        //�A�j���[�V�����I���ҋ@
        yield return null;
        yield return new WaitUntil(() => ani.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        ani.speed = 0.0f;
    }
}
