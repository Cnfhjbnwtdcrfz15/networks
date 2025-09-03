using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public ResultSubtitles Subtitles;
        public float typeSpeed = 0.05f;
        public bool autoNext = false;
        public GameObject objectToEnable;
        public float enableObjectAfter = 1f;
    }

    public TutorialStep[] steps;
    public CanvasGroup uiGroup;
    public TMP_Text uiText;
    public AudioSource audioSource;
    public TypewriterTMP typewriter;
    public float fadeDuration = 0.5f;
    public float autoNextDelay = 1f;

    private bool isPlayingStep = false;

    public void StartStep(int index)
    {
        if (index < 0 || steps == null || index >= steps.Length) return;
        if (isPlayingStep) return;

        StartCoroutine(PlayStepRoutine(index));
    }

    private IEnumerator PlayStepRoutine(int index)
    {
        isPlayingStep = true;
        TutorialStep step = steps[index];

        if (uiGroup != null)
        {
            if (!uiGroup.gameObject.activeSelf || uiGroup.alpha == 0f)
            {
                uiGroup.gameObject.SetActive(true);
                yield return StartCoroutine(FadeCanvasGroup(uiGroup, 0f, 1f, fadeDuration));
            }
        }

        string content = step.Subtitles.Text ?? string.Empty;
        float speed = (step.typeSpeed > 0f) ? step.typeSpeed : 0.05f;

        if (typewriter != null)
            typewriter.ShowText(content, speed);
        else if (uiText != null)
            uiText.text = content;

        if (step.objectToEnable != null && step.enableObjectAfter >= 0f)
            StartCoroutine(EnableObjectAfterDelay(step.objectToEnable, step.enableObjectAfter));

        bool hadAudio = (audioSource != null && step.Subtitles.Clip != null);

        if (hadAudio)
        {
            audioSource.clip = step.Subtitles.Clip;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }
        else
        {
            float estimated = content.Length * speed;
            if (estimated > 0f)
                yield return new WaitForSeconds(estimated);
        }

        yield return new WaitForSeconds(autoNextDelay);

        if (typewriter != null) typewriter.ShowFullText();
        if (uiText != null) uiText.text = string.Empty;

        isPlayingStep = false;

        bool hasNext = (index + 1 < (steps?.Length ?? 0));

        if (step.autoNext && hasNext)
        {
            StartStep(index + 1);
            yield break;
        }

        if (uiGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(uiGroup, 1f, 0f, fadeDuration));
            uiGroup.gameObject.SetActive(false);
        }
    }

    private IEnumerator EnableObjectAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null) obj.SetActive(true);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        if (group == null) yield break;

        float t = 0f;
        group.alpha = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        group.alpha = to;
    }
}