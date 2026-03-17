using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("RotatingCubeScene");
    }
}