using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class MainMenuCamera : MonoBehaviour
{
    private bool Transposing;

    private Func<float, float> f;
    private Func<float, float> g;
    private Func<float, float> h;

    private float duration;

    void Start() {
        Transposing = false;

        f = (t => 0.0f);
        g = (t => 0.0f);
        h = (t => 0.0f);

        duration = 0.0f;
    }

    public void translateCamera( Func<float, float> _f,
                                 Func<float, float> _g,
                                 Func<float, float> _h,
                                 float _duration)
    {
        if (Transposing) { return; }

        // -- Store the given function
        f = _f;
        g = _g;
        h = _h;
        duration = _duration;

        interpolate(false).Forget();
    }

    public void rewindTranslation() {
        // -- Use the last functions and duration
        interpolate(true).Forget();
    }



    // -- given a function 
    private async UniTaskVoid interpolate(bool reverse)
    {
        float elapsedTime = 0.0f;
        Transposing = true;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            Vector3 delta = (Time.deltaTime * new Vector3(f(t), g(t), h(t)));
            delta *= (reverse) ? -1.0f : 1.0f;

            gameObject.transform.position += delta;
            gameObject.transform.LookAt(new Vector3(0.0f, 1.0f, 0.0f));

            await UniTask.Yield();
        }

        Transposing = false;
    }


}
