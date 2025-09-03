using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Загружает сцену по имени.
    /// </summary>
    /// <param name="sceneName">Имя сцены, добавленной в Build Settings.</param>
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("SceneLoader: Имя сцены не задано!");
        }
    }

    /// <summary>
    /// Загружает сцену по индексу.
    /// </summary>
    /// <param name="sceneIndex">Индекс сцены в Build Settings.</param>
    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogWarning($"SceneLoader: Индекс {sceneIndex} вне диапазона!");
        }
    }
}