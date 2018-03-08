using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Explenation : MonoBehaviour
{
    private Text text;
    private Color textColor;
    private float counter;

    private void OnEnable()
    {
        text = GetComponent<Text>();
        text.color = Color.black;
        textColor = text.color;
        counter = 0;
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(1f);

        Color endcolor = text.color;
        endcolor.a = 0;
        while(text.color.a > 0)
        {
            text.color = Color.Lerp(textColor, endcolor, counter / 2f);
            counter += Time.deltaTime;
            yield return null;
        }
    }
}
