using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain
{
    public class StarWaveColliderController : MonoBehaviour
    {
        /// <summary>
        /// ¯‚Ì‰‡Œì
        /// </summary>
        /// <param name="_collider"></param>
        void OnTriggerEnter2D(Collider2D _collider)
        {
            SupportItemsMgr.StarSupport(_collider.gameObject);
        }
    }
}