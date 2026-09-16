using UnityEngine;

public class TrashInteract : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other){
        ChefInventory inventory = other.GetComponent<ChefInventory>();
        Debug.Log(inventory==null);
        if(inventory==null)return;
        inventory.RemoveHeldItem();
    }
}
