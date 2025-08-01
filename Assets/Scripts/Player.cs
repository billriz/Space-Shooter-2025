using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Plaer Movement
    [SerializeField]
    private float _speed = 5f;
    
    private float _currentSpeed;

    private float _horizontalInput;

    private float _verticalInput;

    private Vector3 _playerDirection;

    private Vector3 _playerPosition;

    [SerializeField] 
    private float _speedBoostMultiplier = 1.5f;

    private float _thrustMultiplier = 3f;

    private float _thrusterFuel = 100f;

    private float _thrustFuelBurnRate = 25f;

    private float _thrusterFuelRechargeRate = 5f;

    private KeyCode _thusterActive = KeyCode.LeftShift;   

    [SerializeField]
    private float _thrusterCoolDown = .5f;

    [SerializeField]
    private float _CoolingTimer;    

    private bool _isThrustingActive = false;

    private bool _canUseThruters = true;

    private bool _isThrusterCooling = false;
    
    [SerializeField]
    private float _fireRate = .15f;

    private float _canFire = -1f;

    [SerializeField]
    private int _laserCount = 15;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private int _score;

    [SerializeField]
    private int _playerLives = 3;

    [SerializeField]
    private GameObject[] _damages;

    private List<GameObject> _damageLocation;

    private int _damageIndex;

    
    //Power Ups
    [SerializeField]
    private bool _isTripleShotEnabled = false;

    [SerializeField]
    private GameObject _tripleShotPrefab;

    private bool _isSpeedBoostActive = false;

    [SerializeField]
    private GameObject _playerShield;

    private bool _isPlayerShieldEnabled = false;

    [SerializeField]
    private SpriteRenderer _shieldStrength;

    [SerializeField]
    private float[] _ShieldStrengthLevel;

    private float _currentShieldLevel;

    private int _shieldHits;
    private int _maxShieldHits = 3;     

    [SerializeField]
    private float _powerUpActiveTime = 5.0f;     

    [SerializeField]
    private GameObject _explosion; 

    Spawn_Manager _spawnManager;

    private UIManager _uiManager;



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
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is not assigned");
        }

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _laserCount > 0)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        _isThrustingActive = Input.GetKey(_thusterActive) && _canUseThruters;

        if (_isThrustingActive)
        {
            _currentSpeed = _speed * _thrustMultiplier;
        }
        else if (_isSpeedBoostActive)
        {
            _currentSpeed = _speed * _speedBoostMultiplier;
        }
        else
        {
            _currentSpeed = _speed;
        }

        if (_isThrustingActive)
        {
            ThrusterFuelBurn();
        }
        else if (_thrusterFuel < 100f && !_isThrusterCooling)
        {
            StartThrusterCoolDown();
        }

        if (_isThrusterCooling)
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

        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        _playerDirection = transform.right * _horizontalInput + transform.up * _verticalInput;
        transform.Translate(_playerDirection * _currentSpeed * Time.deltaTime);


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
        if (_thrusterFuel == 100f)
        {
            _canUseThruters = true;
            _isThrusterCooling = false;
        }

        _uiManager.UpdateThrusterFuel(_thrusterFuel);

    }

    void StartThrusterCoolDown()
    {
        Debug.Log("Thruseter is cooling down");
        _isThrusterCooling = true;
        _CoolingTimer = _thrusterCoolDown;
        _canUseThruters = false;
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

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        _laserCount--;

        _uiManager.UpdateLasertext(_laserCount);

        if (_isTripleShotEnabled == true)
        {
            Instantiate(_tripleShotPrefab, transform.position + new Vector3(0, .08f, 0), Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.07f, 0), Quaternion.identity);
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

    public void Damage()
    {
        if (_isPlayerShieldEnabled == true)
        {
            _shieldHits += 1;
            ShieldStrength(_shieldHits);
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

    void PlayerDamage()
    {
        _damageLocation = _damages.Where(l => !l.activeSelf).ToList();

        if (_damageLocation.Count > 0)
        {
            _damageIndex = Random.Range(0, _damageLocation.Count);
            _damageLocation[_damageIndex].SetActive(true);
        }

    }

    void OnPlayerDeath()
    {
        Instantiate(_explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    public void PlayerShieldEnabled()
    {
        _isPlayerShieldEnabled = true;
        if (_playerShield != null)
        {
            _playerShield.SetActive(true);
        }

    }

    void ShieldStrength(int Hits)
    {
        _shieldHits = Mathf.Clamp(Hits, 0, _maxShieldHits);

        if (_shieldHits < _maxShieldHits)
        {
            _currentShieldLevel = (_shieldHits < _ShieldStrengthLevel.Length) ? _ShieldStrengthLevel[_shieldHits] : 0;
            SetShieldStrength(_currentShieldLevel);

        }
        else
        {
            ShieldDisabled();
        }

    }

    void ShieldDisabled()
    {
        _isPlayerShieldEnabled = false;
        if (_playerShield != null)
        {
            _playerShield.SetActive(false);
        }
        SetShieldStrength(1f);
    }

    void SetShieldStrength(float _alpha)
    {
        _shieldStrength.color = new Color(1, 1, 1, _alpha);
    }

    public void ScoreUpdate(int points)
    {
        _score += points;
        _uiManager.UpdateScoretext(_score);
    }       
   
}
