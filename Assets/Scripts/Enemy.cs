using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    private float _enemySpeed = 4f;
    [SerializeField]
    private float _enemyFireRate;

    private float _enemeyCanFire = 5f;

    [SerializeField]
    private Vector3 _enemyReSpawnPos = new Vector3(0, 8f, 0);

    private int _enemyPoints = 10;

    
    private Player _player;
    
    private Animator _animator;
    
    private AudioSource _audioSource;    

    [SerializeField]
    private GameObject _laserPrefab;

    private bool _isAlive =true;            
    
    private Laser _laser;



    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogError("Player is not asssigned");
        }
       
        _animator = GetComponent<Animator>();
        if(_animator == null)
        {
            Debug.LogError("Animator is not assigned");
        }

        _audioSource = GetComponent<AudioSource>();
        if(_audioSource == null)
        {
            Debug.LogError("AudioSource is ont assigned");
        }

 
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _enemeyCanFire && _isAlive == true && transform.position.y > -3f)
        {
            
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (transform.position.y <= -6f)
        {
            _enemyReSpawnPos.x = Random.Range(-9f, 9f);
            transform.position = _enemyReSpawnPos;
        }
    }

    void FireLaser()
    {
        _enemyFireRate = Random.Range(3f, 7f);
        _enemeyCanFire = Time.time + _enemyFireRate;

        Instantiate(_laserPrefab, transform.position, Quaternion.identity);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))

        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.ScoreUpdate(_enemyPoints);
            }
            
            if (_animator != null)
            {
                EnemeyExplosion();
            } 

        }
        else if (other.CompareTag("Player"))
        {            
            if (_player != null)
            {
                _player.Damage();
                EnemeyExplosion();
            }
        }

    }
    private void EnemeyExplosion()
    {
        _enemySpeed = 0;
        Destroy(GetComponent<Collider2D>());
        _isAlive = false;
        _animator.SetTrigger("OnEnemyDestroyed");
    }
    public void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }

    public void EnemeyExplosionSound()
    {
        _audioSource.Play();
    }

    
    
}
