using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // laser speed of 8f
    [SerializeField]
    private float _laserSpeed = 8f;

    private Vector3 _laserDirection = Vector3.up;

        
    // Update is called once per frame
    void Update()
    {
        // move laser up


        CalculateMovemnet();

        
      
    }

    void CalculateMovemnet()
    {
        transform.Translate(_laserDirection * _laserSpeed * Time.deltaTime);

        if (CompareTag("Enemy_Laser"))
        {
            ChangeDirrection(Vector3.down);
        }
        else if (CompareTag("Laser"))
        {
            ChangeDirrection(Vector3.up);
        }

        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject, 6f);
        }
        else
        {
            Destroy(this.gameObject, 6f);
        }

    }
    public void ChangeDirrection(Vector3 direction)
    {
        _laserDirection = direction;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CompareTag("Enemy_Laser"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
                Destroy(this.gameObject);
            }
        }
    }
}
