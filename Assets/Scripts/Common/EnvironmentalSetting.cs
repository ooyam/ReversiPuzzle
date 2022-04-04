using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalSetting : MonoBehaviour
{
    public static bool bgm;
    public static bool se;

    public static EnvironmentalSetting instance = null;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            bgm = true;
            se = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
