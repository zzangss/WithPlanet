using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CartInteractor : MonoBehaviour
{
    private bool isOnCart = false;
    [SerializeField] private InventoryMain mInventory;

    private Collider other;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnCart)
        {
            mInventory.TryOpenInventory();
            if (Input.GetKey(KeyCode.Space))
            {
                this.other.transform.position = this.transform.position;
            }
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Cart"))
        {
            isOnCart = true;
            this.other = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cart"))
        {
            isOnCart = false;
            this.other = null;
        }
    }
}


