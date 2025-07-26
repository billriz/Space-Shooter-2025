using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Astroid : MonoBehaviour
{
    [SerializeField]
    private Vector3 _rotateOnAxis = Vector3.forward;

    [SerializeField]
    private float _rotationSpeed = 60f;

    [SerializeField]
    private Spawn_Manager _spawnManager;    

    [SerializeField]
    private GameObject _explosion; 

    // Start is called before the first frame update
    void Start()
    {
        //_spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<Spawn_Manager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Sapwn Manager is not assigned");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_rotateOnAxis * _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            Instantiate(_explosion, transform.position,Quaternion.identity);
            AstroidDestroyed();
        }
    }

    public void AstroidDestroyed()
    {
        _spawnManager.StartSpawning();
        Destroy(this.gameObject);
    }
   
}
