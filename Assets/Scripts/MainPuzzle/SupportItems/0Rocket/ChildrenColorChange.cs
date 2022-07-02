using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenColorChange : MonoBehaviour
{
    SpriteRenderer mSpriRen;
    SpriteRenderer[] mChildrenSpriRen;

    void Start()
    {
        //���g��SpriteRenderer�擾
        mSpriRen = GetComponent<SpriteRenderer>();

        //�q�I�u�W�F�N�g��SpriteRenderer�擾
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
        //���g�̐F���q�I�u�W�F�N�g�ɔ��f
        Color myColor = mSpriRen.color;
        for (int i = 0; i < mChildrenSpriRen.Length; i++)
        {
            SpriteRenderer spriRen = mChildrenSpriRen[i];
            if (spriRen == null) continue;
            spriRen.color = myColor;
        }
    }
}
