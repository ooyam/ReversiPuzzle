using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;

public class SupportItemInformation : MonoBehaviour
{
    [System.NonSerialized] public Animator   ani;
    [System.NonSerialized] public Transform  tra;
    [System.NonSerialized] public GameObject obj;

    /// <summary>
    /// �R���|�[�l���g�̐ݒ�
    /// </summary>
    void ComponentSetting()
    {
        ani = this.GetComponent<Animator>();
        tra = this.transform;
        obj = this.gameObject;
    }

    /// <summary>
    /// ����A�C�e�����̐ݒ�
    /// </summary>
    public void InformationSetting()
    {
        ComponentSetting();
    }
}
