﻿// From https://forum.unity.com/threads/how-to-cancel-and-restart-a-coroutine.435493/#post-3257127

using System.Collections;
using UnityEngine;

namespace GameDevHQ.Scripts
{
    public static class CoroutineExtensions
    {
        /// <summary>
        /// Tries to stop a coroutine based on a Coroutine Handle.
        /// will only stop the Coroutine if the handle is not null
        /// </summary>
        /// <returns>the Monobehaviour script running the coroutine, allowing chained commands</returns>
        /// <param name="handle">Handle.</param>
        public static MonoBehaviour TryStopCoroutine(this MonoBehaviour script, ref Coroutine handle)
        {
            if (!script) return null;
            if (handle != null) script.StopCoroutine(handle);
            handle = null;
            return script;
        }

        /// <summary>
        /// Starts the coroutine and sets the routine to a Coroutine handle.
        /// </summary>
        /// <returns>the Monobehaviour script running the coroutine, allowing chained commands</returns>
        /// <param name="routine">Routine.</param>
        /// <param name="handle">Handle.</param>
        public static MonoBehaviour StartCoroutine(this MonoBehaviour script, IEnumerator routine, ref Coroutine handle)
        {
            if (!script)
            {
                #if UNITY_EDITOR
                Debug.LogWarning("A coroutine cannot run while it is null or being destroyed");
                #endif
                return null;
            }

            if (!script.enabled || !script.gameObject.activeInHierarchy)
            {
                #if UNITY_EDITOR
                Debug.LogWarningFormat(script, "The Script {0} is currently disabled and cannot start coroutines",
                    script);
                #endif
                return script;
            }

            handle = script.StartCoroutine(routine);

            return script;
        }


        /// <summary>
        /// Stops any possible coroutine running on the specified handle and runs a new routine in its place
        /// </summary>
        /// <returns>the Monobehaviour script running the coroutine, allowing chained commands</returns>
        /// <param name="script">Script.</param>
        /// <param name="routine">Routine.</param>
        /// <param name="handle">Handle.</param>
        public static MonoBehaviour RestartCoroutine(this MonoBehaviour script, IEnumerator routine,
            ref Coroutine handle)
        {
            return script.TryStopCoroutine(ref handle)
                .StartCoroutine(routine, ref handle);
        }
    }
}