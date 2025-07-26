using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;

    [SerializeField]
    private GameObject _gameOverText;

    [SerializeField]
    private GameObject _restartText;

    [SerializeField]
    private Image _livesImage;

    
    [SerializeField]
    private Sprite[] _livesSprite;

    [SerializeField]
    private float _flickerRate = 0.5f;

    GameManager _gameManager;

    
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateScoretext(0);
        UpdateLivesDisplay(3);
        _gameOverText.SetActive(false);
        _restartText.SetActive(false);

        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (_gameManager == null )
        {
            Debug.LogError("Game Manager is not assigned");
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScoretext(int score)
    {

        _scoreText.text = "Score: " + score.ToString();
        
        
    }

    public void UpdateLivesDisplay(int lives)
    {
        if (lives < 0)
        {
            lives = 0;  
        }
        _livesImage.sprite = _livesSprite[lives];      
        if (lives <= 0)
        {
            StartCoroutine(GameoverFlicker());
            _restartText.SetActive(true);
            _gameManager.SetGameOver();
        }
    }

    public void DisplayGameOver()
    {
        //_gameOverText.SetActive(true);
        StartCoroutine(GameoverFlicker());
    }

    IEnumerator GameoverFlicker()
    {
        while (true)
        {
            _gameOverText.SetActive(true);
            yield return new WaitForSeconds(_flickerRate);
            _gameOverText.SetActive(false);
            yield return new WaitForSeconds(_flickerRate);  

        }

    }
}
