using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;
using System.Threading.Tasks;


/*   Opps,  This class is not need and can be swapped for an Animator and   * 
 *   animator controller. None the less, I'm keeping it because I've fal-   *
 *   len victim to Sunk Cost Fallacy.                                       */


public class TitleScreenAnimation : MonoBehaviour
{
    // -- Time durations.
    [SerializeField] private float m_forceIntroSkipTime;
    [SerializeField] private float m_timeBeforeCrash;

    // -- Graphic to be animated.
    [SerializeField] private List<Graphic> graphicsList;

    // -- Lerp variables
    private float lerpElaspedTime;
    private delegate float interpFunction(float t);

    // -- Time variables
    private float autoStartTime;
    private float countDownTime;

    // -- Boolean variables
    private bool IntroFadeInCompleted;
    private bool startedCrashSequence;



    void Start() {
        autoStartTime = 0.0f;
        countDownTime = 0.0f;

        IntroFadeInCompleted = false;
        startedCrashSequence = false;

        animateFadeInSequence();
    }

    void Update(){
        autoStartTime += Time.deltaTime;
        float t1 = autoStartTime / m_forceIntroSkipTime;

        if (!IntroFadeInCompleted) { return; }

        // -- Count down to crash sequence.
        if ( (Input.GetKey(KeyCode.Space) || t1 > 1.0f) && !startedCrashSequence){
            startedCrashSequence = true;

            // -- Fade Out Logo and text.
            interpGraphicAlpha(new List<int> {1,2}, fadeOut, 1.0f);
            crashAnimation();
        }
       
    }


    // -- Lerp the graphics alpha.
    private async Task interpGraphicAlpha(List<int> graphicIndices, interpFunction f, float duration){
        // -- f must be bound between 0.0f and 1.0f
        Debug.Log("A");
        while (lerpElaspedTime < duration){

            lerpElaspedTime += Time.deltaTime;
            float t = lerpElaspedTime / duration;

            // -- Set Alpha to f(t).
            foreach (int idx in graphicIndices) {
                Graphic g = graphicsList[idx];
                g.color = new Color(g.color.r, g.color.g, g.color.b, f(t));
            }

            await Task.Yield();
        }
        // -- call the callback event if there is one

        lerpElaspedTime = 0.0f;
    }

    private float fadeIn(float x){
        return 1.0f - Mathf.Pow((x - 1.0f), 2);
    }

    private float fadeOut(float x) {
        return 1.0f - Mathf.Pow(((1.0f - x) - 1.0f), 2);
    }





    private async void animateFadeInSequence() {
        // -- Fade in Black screen then Logo and text.
        await interpGraphicAlpha(new List<int> { 0 }   , fadeOut, 4.0f);
        await interpGraphicAlpha(new List<int> { 1, 2 }, fadeIn , 3.0f);

        IntroFadeInCompleted = true;
    }


    public void crashAnimation() {
        Debug.Log("AND SCENE");
        //SceneManager.LoadScene(1);
    }


}
