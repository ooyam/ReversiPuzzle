using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonAnimation : MonoBehaviour
{
    [Header("�j��X�v���C�g")]
    [SerializeField]
    Sprite burstSpr;

    SpriteRenderer sprRen;

    void Start()
    {
        sprRen = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// �j��X�v���C�g�����ւ�
    /// </summary>
    public void AnimationBurstSprite()
    {
        Debug.Log("BurstSprite");
        sprRen.sprite = burstSpr;
    }
}
