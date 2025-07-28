using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    private float _currentSpeed;

    private float _thrusterFuel = 100f;

    private float _thrustFuelBurnRate = 25f;

    private float _thrusterFuelRechargeRate = 5f;

    [SerializeField]
    private float _fireRate = .15f;
    [SerializeField]
    private float _thrusterCoolDown = .5f;
    [SerializeField] 
    private float _CoolingTimer;


    private float _canFire = -1f;
    [SerializeField]
    private int _playerLives = 3;


    Spawn_Manager _spawnManager;
    
    [SerializeField]
    private bool _isTripleShotEnabled = false;

    private bool _isPlayerShieldEnabled = false;
    [SerializeField]
    private SpriteRenderer _shieldStrength;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField] 
    private GameObject _playerShield;
 
    [SerializeField]
    private GameObject[] _damages;

    private List<GameObject> _damageLocation;

    private int _damageIndex;

    [SerializeField]
    private float _powerUpActiveTime = 5.0f;
    
    [SerializeField]
    private float _speedBoostMultiplier = 1.5f;

    //private float _baseMultiplier = 1.0f;

    private float _thrustMultiplier = 3f;

    private float _horizontalInput;

    private float _verticalInput;

    private Vector3 _playerDirection;

    private Vector3 _playerPosition;

    [SerializeField]
    private int _score;

    private UIManager _uiManager;
    
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _laserSound, _powerUpSound;
    
    [SerializeField]
    private GameObject _explosion;

    private GameObject Laser;

    private Laser _laser;

    private KeyCode _ThusterActive = KeyCode.LeftShift;

    private bool _isSpeedBoostActive = false;

    private bool _isThrustingActive = false;

    private bool _canUseThruters = true;

    private bool _isThrusterCooling = false;

    private bool _thrusterActive;

    private bool _thrusterOverHeating;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = Vector3.zero;

        _spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<Spawn_Manager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is not assigned");
        }

        _uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        if(_uiManager == null)
        {
            Debug.LogError("UI Manager is not assigned");
        }  
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();


        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }

    }
    void CalculateMovement()
    {
        _isThrustingActive = Input.GetKey(_ThusterActive) && _canUseThruters;

        _thrusterActive = _isThrustingActive && _canUseThruters;

        _thrusterOverHeating = _thrusterFuel < 100f && !_isThrusterCooling;

        _currentSpeed = _thrusterActive ? _speed * _thrustMultiplier : _speed;
               

        if(_isThrustingActive)
        {
            ThrusterFuelBurn();
        }
        else if(_thrusterOverHeating) 
        {
            StartThrusterCoolDown();
        }

        if(_isThrusterCooling)
        {
            if (_CoolingTimer > 0f)
            {
                _CoolingTimer -= Time.deltaTime;
            }
            else
            {
                ThrusterFuelRecharge();
            }           

        }        
        

        _currentSpeed = _isSpeedBoostActive ? _speed * _speedBoostMultiplier : _currentSpeed;
        
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        _playerDirection = transform.right * _horizontalInput + transform.up * _verticalInput;
        transform.Translate(_playerDirection * _currentSpeed  * Time.deltaTime);

        
        _playerPosition = transform.position;
        
        
        _playerPosition.y = Mathf.Clamp(_playerPosition.y, -3.98f, 0f);
        

        if (_playerPosition.x >= 11.24f)
        {
            _playerPosition.x = -11.27f;
        }
        else if (_playerPosition.x <= -11.27f)
        {
            _playerPosition.x = 11.24f;
        }
        
        transform.position = new Vector3(_playerPosition.x, _playerPosition.y, 0f);

       
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotEnabled == true)
        {

            Laser = Instantiate(_tripleShotPrefab, transform.position + new Vector3(0, .08f, 0), Quaternion.identity);            
                
        }
        else
        {

            Laser = Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.07f, 0), Quaternion.identity);
            
        }
        
        
    }    

    public void Damage()
    {
        if (_isPlayerShieldEnabled == true)
        {
            return;
        }

        _playerLives -= 1;
        
        PlayerDamage();

        _uiManager.UpdateLivesDisplay(_playerLives);

        if (_playerLives < 1)
        {
            if (_spawnManager != null)

            {
                _spawnManager.OnPlayerDeath();               

            }                        

            OnPlayerDeath();
        }
    }

    public void TripleShotEnabled()
    {
        _isTripleShotEnabled = true;
        StartCoroutine(TripleShootPowerDownRoutine());
    }

    IEnumerator TripleShootPowerDownRoutine()
    {
        yield return new WaitForSeconds(_powerUpActiveTime);
        _isTripleShotEnabled = false;
    }

    public void SpeedBoostEnabled()
    {
        
        StartCoroutine(SpeedBoostPowerDownRoutine());        
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        _isSpeedBoostActive = true;
        yield return new WaitForSeconds(_powerUpActiveTime);
        _isSpeedBoostActive = false;
        _speedBoostMultiplier = 1f;

    }

    public void PlayerShieldEnabled()
    {
        _isPlayerShieldEnabled = true;
        if(_playerShield != null)
        {
            _playerShield.SetActive(true);
        }
        
        StartCoroutine(PlayerShieldPowerDownRoutine()); 
    }

    IEnumerator PlayerShieldPowerDownRoutine()
    {

        yield return new WaitForSeconds(_powerUpActiveTime);
        _isPlayerShieldEnabled = false;
        if(_playerShield != null)
        {
            _playerShield.SetActive(false);
        }
    }

    public void ScoreUpdate(int points)
    {
        _score += points;
        _uiManager.UpdateScoretext(_score);
    }

    void PlayerDamage()
    {
        
       _damageLocation = _damages.Where(l => !l.activeSelf).ToList();
              

        if (_damageLocation.Count > 0)
        {
            _damageIndex = Random.Range(0, _damageLocation.Count);
            _damageLocation[_damageIndex].SetActive(true);
        }
               
    }    

    public void PowerUpSound()
    {
        if(_powerUpSound != null)
        {
            _audioSource.PlayOneShot(_powerUpSound);
        }
        
    }

    void OnPlayerDeath()
    {
        Instantiate(_explosion,transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
    
    void ThrusterFuelBurn()
    {
        _thrusterFuel = Mathf.MoveTowards(_thrusterFuel, 0f, _thrustFuelBurnRate * Time.deltaTime);
        if (_thrusterFuel == 0f)
        {
            _canUseThruters = false;
        }
        _uiManager.UpdateThrusterFuel(_thrusterFuel);
    }

    void ThrusterFuelRecharge()
    {
        _thrusterFuel = Mathf.MoveTowards(_thrusterFuel, 100f, _thrusterFuelRechargeRate * Time.deltaTime);
        if(_thrusterFuel == 100f)
        {
            _canUseThruters = true;
            _isThrusterCooling = false;
        }

        _uiManager.UpdateThrusterFuel(_thrusterFuel);
        // StartCoroutine(ThrusterCoolDown()); 
    }
    

    void StartThrusterCoolDown()
    {
        Debug.Log("Thruseter is cooling down");
        _isThrusterCooling = true;
        _CoolingTimer = _thrusterCoolDown;
        _canUseThruters = false;
    }
   
}
