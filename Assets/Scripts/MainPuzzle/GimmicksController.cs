using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuzzleDefine;

public class GimmicksController : MonoBehaviour
{
    [Header("�M�~�b�N�v���n�u�̎擾")]
    [SerializeField]
    GameObject[] gimmickPrefabArr;

    /// <summary>
    /// ���D�j��
    /// </summary>
    public static IEnumerator Balloon_Break()
    {
        yield return null;
    }
}
