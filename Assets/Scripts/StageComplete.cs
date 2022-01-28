using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageComplete : MonoBehaviour
{
    public void GoHomeRoger()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
