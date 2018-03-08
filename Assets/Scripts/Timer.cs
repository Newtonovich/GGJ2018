using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Text timerText;

    public void Set(float timer, System.Action callbackUponFinish)
    {
        StartCoroutine(CountDown(timer, callbackUponFinish));
    }

    IEnumerator CountDown(float timer, System.Action callback)
    {
        float originalTIme = timer;
        while(true)
        {

            if (timer < 0)
            {
                if (callback != null)
                {
                    callback();
                }
                timer = originalTIme;
            }
            timerText.text = ":" + timer.ToString("00");

            yield return new WaitForSeconds(1f);
            timer--;

        }

    }
}
