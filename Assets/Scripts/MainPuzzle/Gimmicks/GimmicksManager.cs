using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PieceManager;
using static ObjectMove_2D.ObjectMove_2D;
using static GimmickInformation;

public class GimmicksManager : MonoBehaviour
{
    [Header("��΂�sprite")]
    [SerializeField]
    Sprite[] jewelrySpr;

    //�A�j���[�V�����X�e�[�g��
    const string STATE_NAME_EMPTY        = "Empty";       //�������
    const string STATE_NAME_WAIT         = "Wait";        //�ҋ@
    const string STATE_NAME_COLORLESS    = "Colorless";   //�F�w�薳��
    const string STATE_NAME_BURST        = "Burst";       //�F�w��L��
    const string STATE_NAME_DAMAGE       = "Damage";      //������_���[�W
    const string STATE_NAME_RETURN_STATE = "ReturnState"; //��Ԃ�߂�

    void Awake()
    {
        //�M�~�b�N�̐ݒ�ǂݍ���
        GimmickSetting();
        StageSetting();
    }

    /// <summary>
    /// �M�~�b�N�Ƀ_���[�W�����邩�̊m�F
    /// </summary>
    /// /// <param name="putPieceTag"> �u������̃^�O</param>
    /// /// <param name="gimmickIndex">�M�~�b�N�Ǘ��ԍ�(�X�e�[�W����)</param>
    public bool DamageCheck(ref string putPieceTag, ref int gimmickIndex)
    {
        //�_���[�W�̗L���t���O
        bool damage = false;

        switch (gimmickInfoArr[gimmickIndex].id)
        {
            case (int)Gimmicks.Balloon:  //���D
            case (int)Gimmicks.Wall:     //��
            case (int)Gimmicks.Flower:   //��
            case (int)Gimmicks.Hamster:  //�n���X�^�[
                damage = true;
                break;

            case (int)Gimmicks.Balloon_Color: //���D(�F)
            case (int)Gimmicks.Jewelry:       //���
                //�F����
                int colorIndex = gimmickInfoArr[gimmickIndex].colorNum;
                Colors gimmickColor = (Colors)Enum.ToObject(typeof(Colors), colorIndex);
                if (gimmickColor.ToString() == putPieceTag) damage = true;
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
    public void DamageGimmick(ref string putPieceTag, ref int gimmickIndex, int squareIndex)
    {
        int damageTimesfixNum = 0; //�X�e�[�g���Z�o�p
        string stateName = "";     //�X�e�[�g��
        GimmickInformation gimmInfo = gimmickInfoArr[gimmickIndex]; //�M�~�b�N�̏��擾

        switch (gimmInfo.id)
        {
            //������
            case (int)Gimmicks.Balloon: //���D
                stateName = STATE_NAME_COLORLESS;
                break;

            //�F�w��
            case (int)Gimmicks.Balloon_Color: //���D(�F)
            case (int)Gimmicks.Jewelry:       //���
                stateName = STATE_NAME_BURST + putPieceTag;
                break;

            //������_���[�W
            case (int)Gimmicks.Wall:    //��
            case (int)Gimmicks.Flower:  //��
            case (int)Gimmicks.Hamster: //�n���X�^�[
                damageTimesfixNum = GIMMICK_DAMAGE_TIMES[gimmInfo.id] + 1;
                stateName = STATE_NAME_DAMAGE + (-gimmInfo.remainingTimes + damageTimesfixNum).ToString();
                if (!gimmInfo.destructible) gimmInfo.destructible = true;
                break;
        }

        //�_���[�W�A�j���[�V�����J�n
        gimmickCorList.Add(StartCoroutine(AnimationStart(gimmInfo.ani, stateName)));

        //�_���[�W�񐔌v�Z
        gimmInfo.remainingTimes--;

        //���̃^�[���Ƀ_���[�W���󂯂����̃t���OON
        gimmInfo.nowTurnDamage = true;

        //�_���[�W�c�񐔂�0�Ŕj��
        if (gimmInfo.remainingTimes <= 0)
            destroyPiecesIndexList.Add(squareIndex);
    }

    /// <summary>
    /// �M�~�b�N�̏�ԕω�
    /// </summary>
    /// <param name="gimmickIndex">�M�~�b�N�Ǘ��ԍ�(�X�e�[�W����)</param>
    /// <returns></returns>
    public IEnumerator ChangeGimmickState(int gimmickIndex)
    {
        //�M�~�b�N�̏��擾
        GimmickInformation gimmInfo = gimmickInfoArr[gimmickIndex];

        switch (gimmInfo.id)
        {
            //���(sprite�ؑ�)
            case (int)Gimmicks.Jewelry:

                //�q�I�u�W�F�N�g��sprit�X�V
                SpriteRenderer spriRenChild = gimmInfo.tra.GetChild(0).GetComponent<SpriteRenderer>();
                gimmInfo.colorNum++;
                if (gimmInfo.colorNum >= USE_PIECE_COUNT) gimmInfo.colorNum = 0;
                Sprite newSprite = jewelrySpr[gimmInfo.colorNum];

                //�؂�ւ����sprite���q�I�u�W�F�N�g�ɐݒ�
                gimmInfo.ani.enabled = false;
                spriRenChild.sprite = newSprite;

                //�F�ύX
                yield return StartCoroutine(SpriteRendererPaletteChange(gimmInfo.spriRen, JEWELRY_CHANGE_SPEED, COLOR_FADE_OUT, COLOR_FADE_COMPARE_INDEX));
                gimmInfo.spriRen.sprite = newSprite;
                gimmInfo.spriRen.color = COLOR_PRIMARY;
                gimmInfo.ani.enabled = true;
                Colors colorEnum = (Colors)Enum.ToObject(typeof(Colors), gimmInfo.colorNum);
                LoopAnimationStart(gimmInfo.ani, STATE_NAME_WAIT + colorEnum.ToString());
                break;

            //�n���X�^�[(�A���t���O�m�F)
            case (int)Gimmicks.Hamster:

                //�_���[�W1���
                if (gimmInfo.destructible)
                {
                    //���̃^�[���Ƀ_���[�W���󂯂�
                    if (gimmInfo.nowTurnDamage)
                    {
                        //�_���[�W1�ҋ@���
                        LoopAnimationStart(gimmInfo.ani, STATE_NAME_WAIT);
                    }
                    else
                    {
                        //������Ԃɖ߂�
                        gimmInfo.destructible = false;
                        gimmInfo.remainingTimes++;
                        yield return StartCoroutine(AnimationStart(gimmInfo.ani, STATE_NAME_RETURN_STATE));
                        LoopAnimationStart(gimmInfo.ani);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// �A�j���[�V�����Đ�
    /// </summary>
    /// <param name="ani">      �j�󂷂�I�u�W�F�N�g��Animator</param>
    /// <param name="stateName">�Đ��A�j���[�V�����X�e�[�g��</param>
    IEnumerator AnimationStart(Animator ani, string stateName)
    {
        //�A�j���[�V�����J�n
        ani.Play(stateName, 0, 0.0f);
        //ani.speed = 1.0f;

        //�A�j���[�V�����I���ҋ@
        yield return null;
        yield return new WaitUntil(() => ani.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f);
        //ani.speed = 0.0f;
    }

    /// <summary>
    /// ���[�v�A�j���[�V�����Đ�
    /// </summary>
    /// <param name="ani">      �j�󂷂�I�u�W�F�N�g��Animator</param>
    /// <param name="stateName">�Đ��A�j���[�V�����X�e�[�g��</param>
    void LoopAnimationStart(Animator ani, string stateName = STATE_NAME_EMPTY)
    {
        //�A�j���[�V�����J�n
        ani.Play(stateName, 0, 0.0f);
        //ani.speed = 1.0f;
    }
}
