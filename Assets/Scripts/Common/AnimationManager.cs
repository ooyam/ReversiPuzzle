using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace animation
{
    public class AnimationManager : MonoBehaviour
    {
        //アニメーションステート名
        public const string STATE_NAME_EMPTY        = "Empty";          //初期状態
        public const string STATE_NAME_WAIT         = "Wait";           //待機
        public const string STATE_NAME_DAMAGE       = "Damage";         //複数回ダメージ
        public const string STATE_NAME_BURST        = "Burst";          //破壊
        public const string STATE_NAME_COLOR_CHANGE = "ColorChange";    //色の更新
        public const string STATE_NAME_RETURN       = "Return";         //状態を戻す
        public const string STATE_NAME_ATTACK       = "Attack";         //攻撃
        public const string STATE_NAME_SUPPORT      = "Support";        //援護
        public const string STATE_NAME_READY        = "Ready";          //準備
        public const string STATE_NAME_ACTIVE       = "Active";         //アクティブ
        public const string STATE_NAME_INACTIVE     = "Inactive";       //非アクティブ
        public const string STATE_NAME_APPEARANCE   = "Appearance";     //出現

        //===============================================//
        //===========アニメーション動作関数==============//
        //===============================================//

        /// <summary>
        /// アニメーション再生
        /// </summary>
        /// <param name="ani">      破壊するオブジェクトのAnimator</param>
        /// <param name="stateName">再生アニメーションステート名</param>
        public static IEnumerator AnimationStart(Animator ani, string stateName)
        {
            //アニメーション開始
            ani.Play(stateName, 0, 0.0f);

            //stateが切り替わるまで待機
            yield return new WaitUntil(() => ani.GetCurrentAnimatorStateInfo(0).IsName(stateName));

            //アニメーション終了待機
            yield return new WaitWhile(() => ani.GetCurrentAnimatorStateInfo(0).IsName(stateName));
        }

        /// <summary>
        /// ループアニメーション再生
        /// </summary>
        /// <param name="ani">      破壊するオブジェクトのAnimator</param>
        /// <param name="stateName">再生アニメーションステート名</param>
        public static void LoopAnimationStart(Animator ani, string stateName = STATE_NAME_EMPTY)
        {
            //アニメーション開始
            ani.Play(stateName, 0, 0.0f);
        }
    }
}