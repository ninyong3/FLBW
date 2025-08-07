using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextDisplay : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float messageDuration = 1.5f;

    private Queue<string> messageQueue = new Queue<string>();
    private bool isShowing = false;

    public void ShowLog(string message)
    {
        messageQueue.Enqueue(message);
        if (!isShowing)
            StartCoroutine(ProcessQueue());
    }

    public void ShowPriorityLog(string message)
    {
        StopAllCoroutines(); // 기존 대기 중단
        messageText.text = message;
        messageText.enabled = true;
        StartCoroutine(ResumeQueueAfterDelay());
    }

    private IEnumerator ResumeQueueAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);
        messageText.text = "";
        messageText.enabled = false;

        if (messageQueue.Count > 0)
        {
            StartCoroutine(ProcessQueue());
        }
        else
        {
            isShowing = false;
        }
    }

    private IEnumerator ProcessQueue()
    {
        isShowing = true;

        while (messageQueue.Count > 0)
        {
            string msg = messageQueue.Dequeue();
            messageText.text = msg;
            messageText.enabled = true;

            yield return new WaitForSeconds(messageDuration);

            messageText.text = "";
            messageText.enabled = false;

            yield return new WaitForSeconds(0.1f);
        }

        isShowing = false;
    }
}
