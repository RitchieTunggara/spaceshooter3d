using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float speed = 4f;
    [SerializeField]
    private GameObject laserPrefab;
    
    private Player player;
    private Animator anim;
    private AudioSource audioSource;
    private float fireRate = 3.0f;
    private float canFire = -1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();
        if (player == null)
        {
            Debug.LogError("Player NULL");
        }

        anim = GetComponent<Animator>();

        if (anim == null)
        {
            Debug.LogError("Anim Null");
        }

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > canFire)
        {
            fireRate = Random.Range(3f, 7f);
            canFire = Time.time + fireRate;
            GameObject enemyLaser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < -15f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        Debug.Log("Hit " + other.tag);
        Debug.Log("Tes");
        if (other.tag == "Player")
        { 
            Player player = other.transform.GetComponent<Player>();
            
            if (player != null)
            {
                player.damage();
            }
            anim.SetTrigger("OnEnemyDeath");
            speed = 0;
            Destroy(this.gameObject, 2.8f);
            audioSource.Play();
        }
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (player != null)
            {
                player.addScore(10);
            }
            anim.SetTrigger("OnEnemyDeath");
            speed = 0;
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
            audioSource.Play();
        }
    }
}
