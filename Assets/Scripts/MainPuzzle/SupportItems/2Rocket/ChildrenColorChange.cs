using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenColorChange : MonoBehaviour
{
    SpriteRenderer mSpriRen;
    SpriteRenderer[] mChildrenSpriRen;

    void Start()
    {
        //自身のSpriteRenderer取得
        mSpriRen = GetComponent<SpriteRenderer>();

        //子オブジェクトのSpriteRenderer取得
        Transform tra = transform;
        int childCount = tra.childCount;
        mChildrenSpriRen = new SpriteRenderer[childCount];
        for (int i = 0; i < childCount; i++)
        {
            mChildrenSpriRen[i] = tra.GetChild(i).GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        //自身の色を子オブジェクトに反映
        Color myColor = mSpriRen.color;
        for (int i = 0; i < mChildrenSpriRen.Length; i++)
        {
            SpriteRenderer spriRen = mChildrenSpriRen[i];
            if (spriRen == null) continue;
            spriRen.color = myColor;
        }
    }
}
