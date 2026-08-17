using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyCounter : MonoBehaviour
{
    public int enemiesKilled;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemiesKilled = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesKilled == 33)
        {
            LevelTime.timeStarted = false;
            Debug.Log("Melee: " + PlayerData.meleeAttacks);
            Debug.Log("Ranged: " + PlayerData.rangedAttacks);
            Debug.Log("Time: " + PlayerData.timeToCompleteLevel);
            SceneManager.LoadSceneAsync("Menu");
        }
    }

    public void AddToCounter()
    {
        enemiesKilled++;
    }
}