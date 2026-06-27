using System.Collections;
using UnityEngine;
using TMPro;

public class SheepSpeech : MonoBehaviour
{
    public string[] phrases = {
        "be be",
        "be be be",
        "1 2 3 4 5 con cừu",
        "ừm... cừu đi chơi",
        "be... be...",
        "đi thôi!"
    };

    public float minTalkInterval = 4f;
    public float maxTalkInterval = 7f;
    public float displayDuration = 2.2f;

    private TextMeshPro speechText;
    private Coroutine speechRoutine;

    void Awake()
    {
        CreateSpeechBubble();
    }

    void Start()
    {
        if (phrases.Length > 0)
        {
            speechRoutine = StartCoroutine(SpeechLoop());
        }
    }

    private void CreateSpeechBubble()
    {
        GameObject speechObj = new GameObject("SpeechBubble");
        speechObj.transform.SetParent(transform);
        speechObj.transform.localPosition = new Vector3(0f, 1.3f, 0f);
        speechObj.transform.localRotation = Quaternion.identity;
        speechObj.transform.localScale = Vector3.one;

        speechText = speechObj.AddComponent<TextMeshPro>();
        speechText.fontSize = 3f;
        speechText.alignment = TextAlignmentOptions.Center;
        speechText.color = Color.white;
        speechText.text = string.Empty;
        speechText.enableWordWrapping = false;
        speechText.enableAutoSizing = true;
        speechText.fontSizeMin = 2f;
        speechText.fontSizeMax = 4f;

        MeshRenderer mr = speechObj.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sortingOrder = 14;
        }
    }

    private IEnumerator SpeechLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minTalkInterval, maxTalkInterval);
            yield return new WaitForSeconds(waitTime);

            string phrase = phrases[Random.Range(0, phrases.Length)];
            speechText.text = phrase;

            yield return new WaitForSeconds(displayDuration);
            speechText.text = string.Empty;
        }
    }
}
