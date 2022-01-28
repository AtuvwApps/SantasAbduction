using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private int nextSceneToLoad;
    public AudioClip finishClip;
    public GameManager gm;

    void Start()
    {
        nextSceneToLoad = SceneManager.GetActiveScene().buildIndex + 1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SantaController controller = collision.gameObject.GetComponent<SantaController>();

        if (controller != null && gameObject.GetComponent<Animator>().GetBool("Open"))
        {
            controller.PlaySound(finishClip);
            if(nextSceneToLoad > 30)
            {
                gm.stageComplete();
            }
            else
            {
                levelUnlock();
                gm.winGame();
            }
        }
    }

    public void levelUnlock()
    {
        int currentUnlock = PlayerPrefs.GetInt("levelReached");
        int nextUnlock = nextSceneToLoad;
        if(nextUnlock > currentUnlock)
        {
            PlayerPrefs.SetInt("levelReached", nextUnlock);
        }
    }
}
