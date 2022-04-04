using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuzzleDefine;

public class GimmicksController : MonoBehaviour
{
    [Header("ギミックプレハブの取得")]
    [SerializeField]
    GameObject[] gimmickPrefabArr;

    /// <summary>
    /// 風船破壊
    /// </summary>
    public static IEnumerator Balloon_Break()
    {
        yield return null;
    }
}
