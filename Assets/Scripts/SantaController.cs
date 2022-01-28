using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SantaController : MonoBehaviour
{
    public int presentsCollected = 0;
    public float speed = 3.0f;
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);
    AudioSource audioSource;
    public GameManager gm;
    public AudioClip spottedClip;
    private bool clipPlaying;
    private bool gameOver;
    private bool moveRight;
    private bool moveLeft;
    private bool moveUp;
    private bool moveDown;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        clipPlaying = false;
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Move X", lookDirection.x);
        animator.SetFloat("Move Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if(moveRight)
        {
            horizontal = speed;
        }
        else if(moveLeft)
        {
            horizontal = -speed;
        }
        else
        {
            horizontal = 0;
        }

        if(moveUp)
        {
            vertical = speed;
        }
        else if(moveDown)
        {
            vertical = -speed;
        }
        else
        {
            vertical = 0;
        }
        
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + horizontal * Time.deltaTime;
        position.y = position.y + vertical * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void collectedPresent()
    {
        presentsCollected++;
    }

    public void MoveRight(bool move)
    {
        if(move)
        {
            moveRight = true;
        }
        else
        {
            moveRight = false;
        }
    }

    public void MoveLeft(bool move)
    {
        if(move)
        {
            moveLeft = true;
        }
        else
        {
            moveLeft = false;
        }
    }

    public void MoveUp(bool move)
    {
        if(move)
        {
            moveUp = true;
        }
        else
        {
            moveUp = false;
        }
    }

    public void MoveDown(bool move)
    {
        if(move)
        {
            moveDown = true;
        }
        else
        {
            moveDown = false;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void LoseGame()
    {
        if(!gameOver)
        {
            if(!clipPlaying)
            {
                PlaySound(spottedClip);
                clipPlaying = true;
            }
            gameOver = true;
            gm.loseGame();
        }
    }
}
