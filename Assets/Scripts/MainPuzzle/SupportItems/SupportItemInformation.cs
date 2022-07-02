using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;

public class SupportItemInformation : MonoBehaviour
{
    [System.NonSerialized] public Animator   ani;
    [System.NonSerialized] public Transform  tra;
    [System.NonSerialized] public GameObject obj;

    public Vector3 pos; //���W

    /// <summary>
    /// �R���|�[�l���g�̐ݒ�
    /// </summary>
    void ComponentSetting()
    {
        ani = GetComponent<Animator>();
        tra = transform;
        obj = gameObject;
    }

    /// <summary>
    /// ����A�C�e�����̐ݒ�
    /// </summary>
    public void InformationSetting()
    {
        ComponentSetting();
        pos = tra.localPosition;
    }
}
