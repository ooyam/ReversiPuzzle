using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonAnimation : MonoBehaviour
{
    [Header("破裂スプライト")]
    [SerializeField]
    Sprite burstSpr;

    SpriteRenderer sprRen;

    void Start()
    {
        sprRen = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 破裂スプライト差し替え
    /// </summary>
    public void AnimationBurstSprite()
    {
        Debug.Log("BurstSprite");
        sprRen.sprite = burstSpr;
    }
}
