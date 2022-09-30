using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DescriptionOverlay : MonoBehaviour
{
    private bool open;
    private float elapsedTime;
    private float forceOpenTime;

    private TextMeshProUGUI itemDescription;
    private Image itemSprite;

    // Start is called before the first frame update
    void Start() {
        forceOpenTime = 2.0f;
        elapsedTime = 0.0f;

        open = true;
        //gameObject.SetActive(false);

        itemDescription = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        itemSprite = gameObject.transform.GetChild(1).GetComponent<Image>();
    }

    void Update() {
        if (!open) { return; }

        elapsedTime += Time.deltaTime;

        if (elapsedTime > forceOpenTime) {
            if (Input.anyKey) {
                // -- close.
                open = false;
                gameObject.SetActive(false);
            }
        }

    }

    public void DisplayOverlay(ItemData data, bool pauseGame) {
        if (pauseGame) {
            Debug.Log("SLOW!");
        }
        // -- Change UI text and icon.
        itemSprite.sprite = data.itemIcon;
        itemDescription.text = data.itemDescription;

        elapsedTime = 0.0f;

        open = true;
        gameObject.SetActive(true);

    } 


}
