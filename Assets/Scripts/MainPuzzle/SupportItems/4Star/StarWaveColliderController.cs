using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain
{
    public class StarWaveColliderController : MonoBehaviour
    {
        SupportItemsManager mStItemsMgr;
        void Start()
        {
            mStItemsMgr = sPuzzleMain.GetSupportItemsManager();
        }

        /// <summary>
        /// êØÇÃâáåÏ
        /// </summary>
        /// <param name="_collider"></param>
        void OnTriggerEnter2D(Collider2D _collider)
        {
            mStItemsMgr.StarSupport(_collider.gameObject);
        }
    }
}