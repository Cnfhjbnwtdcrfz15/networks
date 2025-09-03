using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// ��������� ����� �� �����.
    /// </summary>
    /// <param name="sceneName">��� �����, ����������� � Build Settings.</param>
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("SceneLoader: ��� ����� �� ������!");
        }
    }

    /// <summary>
    /// ��������� ����� �� �������.
    /// </summary>
    /// <param name="sceneIndex">������ ����� � Build Settings.</param>
    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogWarning($"SceneLoader: ������ {sceneIndex} ��� ���������!");
        }
    }
}