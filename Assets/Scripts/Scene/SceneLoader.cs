using Unity.VectorGraphics;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public void LoadScenePresenter(string sceneName)
    {
        SceneManager._instance.LoadScene(sceneName);
    }
    public void QuitGame()
    {
        SceneManager._instance.QuitGame();
    }
}
