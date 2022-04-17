using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;
using static GimmicksController;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        GimmickSetting();
        StageSetting();
    }
}