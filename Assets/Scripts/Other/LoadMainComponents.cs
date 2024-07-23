using System.Collections;
using UnityEngine;

public class LoadMainComponents : MonoBehaviour
{
    public string sceneName = "MainComponents";
    void Awake()
    {
        StartCoroutine(LoadMainComponentsScene());
    }
    IEnumerator LoadMainComponentsScene()
    {
        yield return new WaitForSeconds(0.1f);
        if (!UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            yield return new WaitUntil(() => UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).isLoaded);
            Destroy(gameObject);
        }
    }
}
