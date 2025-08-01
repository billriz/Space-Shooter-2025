using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Manager : MonoBehaviour
{

    [SerializeField]
    private float _enemySpawnRate = 5f;

    [SerializeField]
    private float _powerupSpawnRate = 3.0f;

    private bool _stopSpawning = false;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private GameObject _enemyContainer;

    [SerializeField]
    private GameObject[] _powerUps;
    [SerializeField]
    private Vector3 _enemySpawnPos = new Vector3(0, 8f, 0);

    [SerializeField]
    private Vector3 _powerUpSpawnPos = new Vector3(0, 8f, 0);

    private int _randomPowerUpId;     


    // Start is called before the first frame update   
    public void StartSpawning()
    { 
        
        StartCoroutine(SpawnEnemyRoutine());

        StartCoroutine(SpawnPowerupRoutine());
        
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (_stopSpawning == false)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(_enemySpawnRate);
        }
    }

    void SpawnEnemy()
    {
        
        _enemySpawnPos.x = Random.Range(-9f, 9f);
        GameObject _newEnemy = Instantiate(_enemyPrefab,_enemySpawnPos, Quaternion.identity); 
        _newEnemy.transform.parent = _enemyContainer.transform;
        
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (_stopSpawning == false)
        {
            SpawnPowerups();
            _powerupSpawnRate = Random.Range(3f, 9f);
            yield return new WaitForSeconds(_powerupSpawnRate); 
        }
    }

    void SpawnPowerups()
    {
        _powerUpSpawnPos.x = Random.Range(-9f, 9f);
        _randomPowerUpId = Random.Range(0, 5);
        //_powerupSpawnPos = new Vector3(Random.Range(-9f, 9f), 8f, 0);
        GameObject _newTripleShotPowerup = Instantiate(_powerUps[_randomPowerUpId], _powerUpSpawnPos, Quaternion.identity);
    }

    public void OnPlayerDeath() 
    {
        _stopSpawning = true; 
    
    }    

}
