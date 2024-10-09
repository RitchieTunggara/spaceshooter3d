using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 4f;
    private float speedMultiplier = 2f;
    [SerializeField]
    private GameObject laser;
    [SerializeField]
    private GameObject tripleLaser;
    private float fireRate = 0.5f;
    private float canFire = -1f;

    [SerializeField]
    private int lives = 3;

    private bool tripleShotActive = false;
    private bool speedBoostActive = false;
    private bool isShieldActive = false;

    [SerializeField]
    private GameObject leftEngine, rightEngine;

    [SerializeField]
    private int score;

    [SerializeField]
    private GameObject shieldVisualizer;

    private SpawnManager spawnManager;
    private UIManager uiManager;
    [SerializeField]
    private AudioClip laserSoundClip;
    [SerializeField]
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        audioSource = GetComponent<AudioSource>();

        if (spawnManager == null)
        {
            Debug.LogError("Spawn manager is null");
        }

        if (uiManager == null)
        {
            Debug.LogError("UI Manager is null");
        }

        if (audioSource == null)
        {
            Debug.LogError("Audiosource on the player is null");
        }
        else
        {
            audioSource.clip = laserSoundClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        calculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > canFire)
        {
            canFire = Time.time + fireRate;

            if (tripleShotActive == true)
            {
                Instantiate (tripleLaser, transform.position, Quaternion.identity);
            }
            else 
            {
                Instantiate (laser, transform.position + new Vector3(0, 1.08f, 0), Quaternion.identity);
            }

            audioSource.Play();
        }
    }

    void calculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * speed * Time.deltaTime);
        
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    public void damage()
    {

        if (isShieldActive == true)
        {
            isShieldActive = false;
            shieldVisualizer.SetActive(false);
            return;
        }

        lives--;

        if (lives == 2)
        {
            leftEngine.SetActive(true);
        }
        else if (lives == 1)
        {
            rightEngine.SetActive(true);
        }

        uiManager.updateLives(lives);

        if (lives < 1)
        {
            spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        tripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        tripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        speedBoostActive = true;
        speed *= speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        speedBoostActive = false;
        speed /= speedMultiplier;
    }

    public void ShieldActive()
    {
        isShieldActive = true;
        shieldVisualizer.SetActive(true);
    }

    public void addScore(int points)
    {
        score += points;
        uiManager.updateScore(score);
    }
}
