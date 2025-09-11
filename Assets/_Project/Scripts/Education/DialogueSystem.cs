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

        [Tooltip("Prefix of the wall to dismantle when this step ends (e.g. \"Wall_1\"). Leave empty to skip.")]
        public string wallNameToDismantle;
    }

    [Header("Dialogue Steps")]
    public TutorialStep[] steps;

    [Header("UI Components")]
    public CanvasGroup uiGroup;
    public TMP_Text uiText;
    public TypewriterTMP typewriter;
    public float fadeDuration = 0.5f;
    public float autoNextDelay = 1f;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Wall Assembler (to call dismantle)")]
    [Tooltip("Assign your RedWallAssembler instance here")]
    public RedWallAssembler wallAssembler;

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
        var step = steps[index];

        // Fade in UI if needed
        if (uiGroup != null && (!uiGroup.gameObject.activeSelf || uiGroup.alpha == 0f))
        {
            uiGroup.gameObject.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(uiGroup, 0f, 1f, fadeDuration));
        }

        // Show text
        string content = step.Subtitles.Text ?? "";
        float speed = step.typeSpeed > 0f ? step.typeSpeed : 0.05f;
        if (typewriter != null)
            typewriter.ShowText(content, speed);
        else if (uiText != null)
            uiText.text = content;

        // Optionally enable an object after a delay
        if (step.objectToEnable != null && step.enableObjectAfter >= 0f)
            StartCoroutine(EnableObjectAfterDelay(step.objectToEnable, step.enableObjectAfter));

        // Play audio or estimate read time
        if (audioSource != null && step.Subtitles.Clip != null)
        {
            audioSource.clip = step.Subtitles.Clip;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }
        else
        {
            float est = content.Length * speed;
            if (est > 0f)
                yield return new WaitForSeconds(est);
        }

        // Wait a bit before clearing
        yield return new WaitForSeconds(autoNextDelay);

        // Instantly finish typewriter and clear text
        if (typewriter != null)
            typewriter.ShowFullText();
        if (uiText != null)
            uiText.text = "";

        // Dismantle a wall if specified
        if (!string.IsNullOrEmpty(step.wallNameToDismantle) && wallAssembler != null)
        {
            wallAssembler.DismantleWall(step.wallNameToDismantle);
        }

        isPlayingStep = false;

        bool hasNext = index + 1 < steps.Length;
        if (step.autoNext && hasNext)
        {
            StartStep(index + 1);
        }
        else if (uiGroup != null)
        {
            // Fade out UI
            yield return StartCoroutine(FadeCanvasGroup(uiGroup, 1f, 0f, fadeDuration));
            uiGroup.gameObject.SetActive(false);
        }
    }

    private IEnumerator EnableObjectAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
            obj.SetActive(true);
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