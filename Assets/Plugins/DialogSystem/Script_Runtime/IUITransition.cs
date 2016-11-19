using UnityEngine;
using System.Collections;

public interface IUITransition {

    /// <summary>
    /// Change opacity from 100% to 0% in the designated duration.
    /// </summary>
    IEnumerator FadeIn(float duration);

    /// <summary>
    /// Change opacity from "from"  to "to" in the designated duration.
    /// </summary>
    IEnumerator FadeIn(float from, float to, float duration);

    /// <summary>
    /// Change opacity from 100% to 0% in the designated duration.
    /// </summary>
    IEnumerator FadeOut(float duration);

    /// <summary>
    /// Change opacity from "from"  to "to" in the designated duration.
    /// </summary>
    IEnumerator FadeOut(float from, float to, float duration);
}
