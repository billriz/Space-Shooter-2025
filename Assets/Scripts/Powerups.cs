﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerups : MonoBehaviour
{
    [SerializeField]
    private float _powerUpSpeed = 3.0f;
    [SerializeField]
    private int _powerupID; // 0 = Triple Shot; 1 = Speed Boost; 2 = Shield
    [SerializeField]
    private Player _player;
       

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _powerUpSpeed * Time.deltaTime);

        if (transform.position.y <= -6.7f)
        {
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _player = other.GetComponent<Player>();
            if (_player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        _player.TripleShotEnabled();
                        break;
                    case 1:
                        _player.SpeedBoostEnabled();
                        break;
                    case 2:                        
                        _player.PlayerShieldEnabled();
                        break;

                    default:
                        break;

                }
                _player.PowerUpSound();
                Destroy(this.gameObject);
            }

            
        }

    }
}


