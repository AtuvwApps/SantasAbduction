using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button[] levelButtons;
    private int audio; //0 = muted, 1 = playing
    public Camera camera;

    private void Start()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        for(int i = levelReached; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = false;
        }

        audio = PlayerPrefs.GetInt("audio", 1);
        if(audio == 0)
        {
            camera.GetComponent<AudioListener>().enabled = false;
            AudioListener.volume = 0;
        }
    }

    public void Select(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
}
