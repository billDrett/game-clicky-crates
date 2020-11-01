using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] int pointValue;
    [SerializeField] ParticleSystem explosionParticle;
    GameManager gameManager;
    Rigidbody targetRb;
    float minSpeed = 12;
    float maxSpeed = 16;
    float maxTorque = 500;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        targetRb = GetComponent<Rigidbody>();

        targetRb.AddForce(Vector3.up * Random.Range(minSpeed, maxSpeed), ForceMode.Impulse);
        targetRb.AddTorque(Random.Range(-maxTorque, maxTorque), Random.Range(-maxTorque, maxTorque), Random.Range(-maxTorque, maxTorque), ForceMode.Impulse);

        //transform.position = new Vector3(Random.Range(-maxXPos, maxXPos), startYpos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (!gameManager.isGameActive) return;

        gameManager.UpdateScore(pointValue);
        Instantiate(explosionParticle, transform.position, transform.rotation);
        Destroy(gameObject);

    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Destroy object with tag " + other.tag);
        if (!gameObject.CompareTag("Bad"))
        {
            
            gameManager.GameOver();
        }
        Destroy(gameObject);
    }
}
