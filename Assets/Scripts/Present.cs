using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Present : MonoBehaviour
{
    public int totalPresents;
    public GameObject barrier;
    public AudioClip collectedClip;
    public AudioClip barrierOpenedClip;
    public GameObject lightRed;
    public GameObject lightGreen;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SantaController controller = collision.GetComponent<SantaController>();

        if(controller != null)
        {
            controller.collectedPresent();
            if (controller.presentsCollected >= totalPresents)
            {
                barrier.GetComponent<Animator>().SetBool("Open", true);
                controller.PlaySound(barrierOpenedClip);
                lightRed.SetActive(false);
                lightGreen.SetActive(true);
                Debug.Log("Barrier opened!");
                //reset present count in case of two part level
                controller.presentsCollected = 0;
            }
            else{
                controller.PlaySound(collectedClip);
            }
            
            Destroy(gameObject);
            Debug.Log("Presents Collected: " + controller.presentsCollected);
        }
    }
}
