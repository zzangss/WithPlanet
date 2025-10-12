using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitButton : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private InventoryMain mInventory;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        mInventory.TryOpenCloseInventory();
    }

}
