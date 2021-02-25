using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class hook : MonoBehaviour
{
    // Variables and constants for main character and game controls

    private float holdTime = 0.3f;
    private float touchTime = 0f;

    Vector2 lookDirection;

    private Touch touch;

    public ParticleSystem ps;
    public ParticleSystem wallParticles;
    public ParticleSystem death;
    public ParticleSystem deathMouth;

    public Animator animator;
    public static bool boosted = false;
    bool isDead = false;

    public static int deathCounter = 0;

    public Material[] materials;
    private Renderer rend;

    public LayerMask canHold;

    bool onlyOne = true;

    public float distance = 5f;

    LineRenderer line;
    DistanceJoint2D rope;
    Vector2 temp;

    public Transform first;

    bool canHook = true;

    Rigidbody2D rb;

    // Variables and constants are over

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        Time.timeScale = 1f;
      
        // Getting object's components
        rope = GetComponent<DistanceJoint2D>();
        line = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<Renderer>();
        // Getting object components section is over

        isDead = false;

        rend.sharedMaterial = materials[0]; // Setting up the material to original
        rope.enabled = false;
        line.enabled = false;

    }


    void Update()
    {
        line.SetPosition(0, transform.position);
        lookDirection = (transform.up + transform.right + transform.up) * 100f; // Setting the line shooter to North East direction

        if (Input.touchCount > 0 && canHook)
        {
            touch = Input.GetTouch(0);
            touchTime += touch.deltaTime;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDirection, distance, canHold); // Setting hit variable with raycasting

            if (hit.point.x >= temp.x + 6f || hit.point.x <= first.position.x + (first.localScale.x / 2))
            {
                onlyOne = true; // This variable checks if the second ray is hitting the same ceiling or not
            }

            else
            {
                onlyOne = false;
            }
            
            if (hit.collider != null && onlyOne && touchTime < holdTime) // If you successfully hit the untouched ceiling
            {
                canHook = false; // Can't hook again until you release
                SetRope(hit); // Set the rope for visibility and gameplay
            }
        }

        if (Input.touchCount > 0) // If you release your finger
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended && !canHook)
            {
                canHook = true; // You can hook again
                touchTime = 0f; // Set the touchTime counter to 0f
                DestroyRope(); // And destroy the rope that you already shooted before
            }

            else if(Input.GetTouch(0).phase == TouchPhase.Ended && canHook) // If you are not touching the screen but also you don't have a hook from before
            {
                touchTime = 0f; // Only set the touchTime counter to 0f
            }
        }

        if (rb.velocity.magnitude <= 9f) // If your velocity reaches to 9f or less
        {
            // Go back to original look
            rend.sharedMaterial = materials[0];
            animator.SetBool("isJumping", false);
            if (ps.isPlaying)
            {
                ps.Stop();
            }
            boosted = false;
        }


        if(isDead) // If you died, check the highscore
        {
            if (SetScore.distance > PlayerPrefs.GetInt("HighScore", 0))
            {
                PlayerPrefs.SetInt("HighScore", SetScore.distance);
            }
        }

        if (deathCounter % 3 == 0 && deathCounter != 0 && isDead) // If you died, show an ad in every 3 deaths
        {
            AdController.instance.ShowAd();
        }
    }

    GameObject tempObject; // Ceiling objects that you hook ropes

    void SetRope(RaycastHit2D hit) // Function for setting the rope
    {
        tempObject = hit.collider.gameObject;
        tempObject.GetComponent<Renderer>().sharedMaterial = materials[2];

        FindObjectOfType<AudioManager>().Play("Swing");
        FindObjectOfType<AudioManager>().Play("Rope");

        rope.enabled = true;
        rope.connectedAnchor = hit.point;
        temp = hit.point;

        // Show particles on every hit
        wallParticles.gameObject.SetActive(true);
        wallParticles.transform.position = new Vector2(hit.point.x, hit.point.y + 1f);
        // Particle section is over

        if(!wallParticles.isPlaying)
        {
            wallParticles.Play();
        }

        line.enabled = true;
        line.SetPosition(1, hit.point);
    }

    void DestroyRope() // Function for destroying the rope when you release the screen or die
    {
        rb.AddForce((Vector2.up * 250)); // Add extra force on upwards direction to make gameplay more convenient in every rope release
        rope.enabled = false;
        line.enabled = false;
        tempObject.GetComponent<Renderer>().sharedMaterial = materials[0];

        if (wallParticles.isPlaying)
        {
            wallParticles.Stop();
        }

    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Boost") // If you get boost
        {
            rb.AddForce(Vector2.right * 500 + Vector2.up*50); // Add extra forces
            Destroy(col.gameObject); // Destroy boost object
            DestroyRope(); // Also destroy the rope

            StartCoroutine(ChangeMaterial()); // Change the material for going into to boost mode
            boosted = true;
            FindObjectOfType<AudioManager>().Play("Boost");
        }

        if(col.gameObject.tag == "MainCamera") // If you exit from screen boundries, you'll die
        {
            isDead = true;
            deathCounter++;
            Destroy(gameObject, 2f);
            SceneManager.LoadScene(0);
        }

        if (col.gameObject.tag == "Mouth") // If you hit a mouth(enemy)
        {
            if(!boosted) // If you are not boosted you'll die
            {
                death.gameObject.SetActive(true);
                FindObjectOfType<AudioManager>().Play("MouthDeath");

                if (!death.isPlaying)
                {
                    death.transform.position = transform.position;
                    death.Play();
                }

                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                rb.velocity = new Vector2(0f, 0f);
                rb.gravityScale = 0f;

                DestroyRope();

                isDead = true;
                deathCounter++;

                if(ps.isPlaying)
                {
                    ps.Stop();
                }

                StartCoroutine(Restart());
            }

            else // If you are boosted, you'll destroy the mouth enemies
            {
                deathMouth.gameObject.SetActive(true);

                if (!deathMouth.isPlaying)
                {
                    deathMouth.transform.position = col.transform.position;
                    deathMouth.Play();
                }
                FindObjectOfType<AudioManager>().Play("MouthDeath");

                Destroy(col.gameObject);
            }
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Obstacle" && rb.velocity.magnitude <= 9f) // If you hit an obstacle you'll slow down and your boost will be gone(if you have it)
        {
            rend.sharedMaterial = materials[0];
            animator.SetBool("isJumping", false);
            boosted = false;
        }
    }

    IEnumerator ChangeMaterial()
    {
        rend.sharedMaterial = materials[1];
        animator.SetBool("isJumping", true);
        ps.gameObject.SetActive(true);
        if (!ps.isPlaying)
        {
            ps.Play();
        }
        yield return new WaitForSeconds(7);
        boosted = false;
        rend.sharedMaterial = materials[0];
        animator.SetBool("isJumping", false);
        if (ps.isPlaying)
        {
            ps.Stop();
        }
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(1.4f);
        SceneManager.LoadScene(0);
    }
}
