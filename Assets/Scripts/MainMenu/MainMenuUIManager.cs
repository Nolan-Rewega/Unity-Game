using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;


public class MainMenuUIManager : MonoBehaviour
{
    // -- Transition buttons.
    [SerializeField] private Button play;
    [SerializeField] private Button options;

    private GameObject currentMenu;
    private GameObject previousMenu;

    private MainMenuCamera menuCamera;
    private bool transitioning;

    void Start() {
        menuCamera = GameObject.Find("MainMenu Camera").GetComponent<MainMenuCamera>();
        currentMenu = GameObject.Find("Main Menu");

        transitioning = false;

        // -- Transition button event listeners.
        play.onClick.AddListener(() => menuCamera.translateCamera(t => 0.0f, t => t, t => 0.0f, 3.0f));
        options.onClick.AddListener(() => menuCamera.translateCamera(Mathf.Cos, t => 0.0f, Mathf.Sin, 3.0f));
    }


    // -- Menu swaping methods.
    public void setCurrentMenu(GameObject menu) { if (!transitioning) { test1(menu).Forget(); } }
    public void onBackClicked() { 
        if (!transitioning) {
            menuCamera.rewindTranslation();
            test2().Forget(); 
        } 
    }

    private async UniTaskVoid test1(GameObject menu) {
        transitioning = true;
        // -- Fade out current menu
        await fadeMenuUI(currentMenu, true);

        previousMenu = currentMenu;
        currentMenu = menu;

        // -- Fade in the new Menu.
        await fadeMenuUI(currentMenu, false);
        transitioning = false;
    }
    private async UniTaskVoid test2() {
        transitioning = true;
        // -- Fade Out the current Menu.
        await fadeMenuUI(currentMenu, true);

        var temp = currentMenu;
        currentMenu = previousMenu;
        previousMenu = temp;

        // -- Fade In the previous Menu.
        await fadeMenuUI(currentMenu, false);
        transitioning = false;
    }


    // -- Specific button methods.
    public void onCreditsClicked() {

    }
    public void onExitClicked() {
        Debug.Log("Exited.");
        //Application.Quit();
    }


    // -- interpolation method.
    private async UniTask fadeMenuUI(GameObject menu, bool fadeOut) {

        float elaspedTime = 0.0f;
        float duration = 1.6f;

        // -- Interpolations function selection
        Func<float, float> f = (fadeOut) ?  (t => 1.0f - t) : (t => t);

        // -- Get all Graphics from the menu excluding Buttons.
        //    This is also really slow, but its a menu so whatever.
        var graphics = from graphic in menu.GetComponentsInChildren(typeof(Graphic))
                       where graphic.tag != "Button"
                       select graphic;

        // -- Before the fade in enable the menu.
        if (!fadeOut) { menu.SetActive(true); }

        while (elaspedTime < duration){

            elaspedTime += Time.deltaTime;
            float t = elaspedTime / duration;

            foreach (Graphic g in graphics){
                g.color = new Color(g.color.r, g.color.g, g.color.b, f(t));
            }

            await UniTask.Yield();
        }

        // -- After fade out disable the menu.
        if (fadeOut) { menu.SetActive(false); }
    }
}
