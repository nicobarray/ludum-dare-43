using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    public Button throwDices;
    public Button nextStep;

    public TMPro.TextMeshProUGUI blockPointText;
    public TMPro.TextMeshProUGUI dicePointText;

    public GameObject eventFrame;
    public Button eventOption1;
    public Button eventOption2;
    public Image eventOption1Image;
    public Image eventOption2Image;
    public TMPro.TextMeshProUGUI eventOption1Text;
    public TMPro.TextMeshProUGUI eventOption2Text;

    [Serializable]
    public struct Timers
    {
        public Image stepClock;
        public TMPro.TextMeshProUGUI turnText;
    }

    public Timers timers;

    [Serializable]
    public struct Resources
    {
        public TMPro.TextMeshProUGUI dice;
        public TMPro.TextMeshProUGUI villager;
        public TMPro.TextMeshProUGUI food;
        public TMPro.TextMeshProUGUI wood;
        public TMPro.TextMeshProUGUI stone;
    }

    public Resources resourceTexts;

    public void ChangeStep(float angle)
    {
        StartCoroutine(RotateClock(timers.stepClock.transform.eulerAngles.z, angle));
    }

    IEnumerator RotateClock(float from, float to)
    {
        float t = 0;

        while (t < 1)
        {
            float z = Mathf.LerpAngle(from, to, t);
            timers.stepClock.transform.eulerAngles = new Vector3(0, 0, z);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}