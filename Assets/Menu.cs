using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void PlayLevel2BSP()
    {
        SceneManager.LoadSceneAsync("Level 2 BSP");
    }

    public void PlayLevel2CA()
    {
        SceneManager.LoadSceneAsync("Level 2 Cellular Automata");
    }

}