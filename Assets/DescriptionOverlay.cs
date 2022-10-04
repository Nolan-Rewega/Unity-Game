using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DescriptionOverlay : MonoBehaviour
{
    private bool isOverlayOpen;
    private float elapsedTime;
    private float forceOpenTime;

    private TextMeshProUGUI itemDescription;
    private Image itemSprite;

    // Start is called before the first frame update
    void Start() {
        forceOpenTime = 1.5f;
        elapsedTime = 0.0f;

        isOverlayOpen = false;
        gameObject.SetActive(false);

        itemDescription = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        itemSprite = gameObject.transform.GetChild(1).GetComponent<Image>();
    }

    void Update() {
        if (!isOverlayOpen) { return; }

        elapsedTime += Time.unscaledDeltaTime;

        if (elapsedTime > forceOpenTime) {
            if (Input.anyKey) {
                isOverlayOpen = false;
                gameObject.SetActive(false);
                Time.timeScale = 1.0f;
            }
        }

    }

    public void DisplayOverlay(ItemData data, bool pauseGame) {
        if (pauseGame) {
            Time.timeScale = 0.0f;
        }

        // -- Change UI text and icon.
        itemSprite.sprite = data.itemIcon;
        itemDescription.text = data.itemDescription;

        elapsedTime = 0.0f;

        isOverlayOpen = true;
        gameObject.SetActive(true);

    }

    public bool getIsOverlayOpen() {
        return isOverlayOpen;
    } 
}
