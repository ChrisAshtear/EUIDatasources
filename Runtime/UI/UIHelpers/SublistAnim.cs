using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SublistAnim : MonoBehaviour
{
    public bool opened = false;
    public Vector2 minSize = Vector2.one;
    public Vector2 maxSize = Vector2.one;

    bool animating = false;

    RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        StartCoroutine(delaySetup());
    }

    //Wait for all assets to load
    IEnumerator delaySetup()
    {
        yield return null;
        setup();
    }

    //Can be reset again if list size changes
    public void setup()
    {
        //Automatically sets max limit
        if (!name.StartsWith("Step"))
        {
            RectTransform lastRect = transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>();
            maxSize.y = -lastRect.anchoredPosition.y + lastRect.sizeDelta.y / 2 + 2.5f;

            if (!name.StartsWith("Step") && name != "MotorControls_")
                if (opened)
                    transform.GetChild(1).GetChild(1).localScale = Vector3.one;
                else
                    transform.GetChild(1).GetChild(1).localScale = new Vector3(1, -1, 1);

            if (!animating)
                rect.sizeDelta = (opened ? maxSize : minSize);
        }
    }

    //Function to start coroutine
    public void toggleList()
    {
        opened = !opened;
        if (gameObject.activeInHierarchy)
        {
            animating = true;
            StartCoroutine(toggleListAnim());
        }
        else
            setup();
    }

    IEnumerator toggleListAnim()
    {
        //Flips open/close symbol
        if (!name.StartsWith("Step") && name != "MotorControls_")
            if (opened)
                transform.GetChild(1).GetChild(1).localScale = Vector3.one;
            else
                transform.GetChild(1).GetChild(1).localScale = new Vector3(1, -1, 1);

        //Animate (w/ delta time for smoothness)
        for (float i = 0; i < 25; i += Time.deltaTime * 100)
        {
            rect.sizeDelta += (maxSize - minSize) * (opened ? 1 : -1) * 4 * Time.deltaTime;
            yield return null;
        }
        //Makes sure equal amount moved so doesn't desync after a while
        rect.sizeDelta = (opened ? maxSize : minSize);

        animating = false;
    }
}
