using UnityEngine;
using UnityEngine.SceneManagement;
public class Exit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            SceneManager.LoadSceneAsync("Level 1");
            PlayerData.meleeAttacks = 0;
            PlayerData.rangedAttacks = 0;
            PlayerData.totalDistance = 0;
            PlayerData.timeToCompleteLevel = 0;
        }
       if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            Application.Quit();
        }
    }
}
