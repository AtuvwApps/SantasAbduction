using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlienController : MonoBehaviour
{
    public Vector2[] points;
    private int pointIndex;
    public bool reverse;
    private bool forwards;

    [SerializeField] private Transform pfFieldOfView;
    private FieldOfView fieldOfView;
    public float fov;
    public float viewDistance;
    public float speed;
    new Rigidbody2D rigidbody2D;
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        fieldOfView = Instantiate(pfFieldOfView, null).GetComponent<FieldOfView>();
        fieldOfView.setFov(fov);
        fieldOfView.setViewDistance(viewDistance);

        pointIndex = 0;
        //Boolean used only if 'reverse' is selected, it just lets us know if we should be adding/subtracting from index
        forwards = true;
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2D.position;

        transform.position = Vector2.MoveTowards(transform.position, points[pointIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, points[pointIndex]) < 0.1f)
        {

            if (pointIndex == points.Length - 1 && !reverse)
            {
                //we use -1 because we only add to the index after these logic tests, so it will be 0 by the time we go to move again
                pointIndex = -1;
            }

            if (!forwards && pointIndex == 0)
            {
                forwards = true;
            }

            if (reverse && pointIndex == points.Length - 1)
            {
                forwards = false;
            }

            if (forwards)
            {
                pointIndex++;
            }
            else
            {
                pointIndex--;
            }
        }

        getDirection();

        fieldOfView.setAimDirection(lookDirection);
        fieldOfView.setOrigin(transform.position);
    }

    //Method used to set the right animation according to where the alien is facing/moving
    private void getDirection()
    {
        if (Mathf.Abs(transform.position.x - points[pointIndex].x) < 0.1f)
        {
            lookDirection.x = 0;
            animator.SetFloat("Move X", 0);
            if (points[pointIndex].y > transform.position.y)
            {
                lookDirection.y = 1;
                animator.SetFloat("Move Y", 1);
            }
            else
            {
                lookDirection.y = -1;
                animator.SetFloat("Move Y", -1);
            }
        }
        else
        {
            lookDirection.y = 0;
            animator.SetFloat("Move Y", 0);
            if (points[pointIndex].x > transform.position.x)
            {
                lookDirection.x = 1;
                animator.SetFloat("Move X", 1);
            }
            else
            {
                lookDirection.x = -1;
                animator.SetFloat("Move X", -1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<SantaController>().LoseGame();
        }
    }
}
