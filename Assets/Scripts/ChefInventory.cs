using UnityEngine;

public class ChefInventory:MonoBehaviour
{
    [SerializeField] private Transform HandPoint;

    
    private GameObject heldItem;
    public bool HasItem(){
        return heldItem !=null;
    }
    public GameObject GetHeldItem(){
        return heldItem;
    }
    public Ingredient GetHeldIngredient(){
        if(heldItem==null){
            return null;
        }
        return heldItem.GetComponentInChildren<Ingredient>();

    }
    public void PickUpItem(GameObject ingredientPrefab)
    {
        if(heldItem!=null){
            return;
        }
        heldItem = Instantiate(
            ingredientPrefab,
            HandPoint.position,
            HandPoint.rotation,
            HandPoint
        );
        heldItem.name =ingredientPrefab.name;
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;
    }
    public void RemoveHeldItem(){
        if(heldItem==null){
            return ;
        }
        Destroy(heldItem);
        heldItem=null;
    }
    public void PickUpExistingItem(GameObject existingItem){
        if(heldItem!=null){
            return;
        }
        heldItem = existingItem;
        heldItem.name=existingItem.name;
        heldItem.transform.SetParent(HandPoint);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;
        if(heldItem.TryGetComponent<Rigidbody>(out Rigidbody itemRb))
        {
            itemRb.isKinematic = true;
        }
    }
    public void PlaceItemAtPos(Vector3 targetLocalPos,Quaternion targetLocalRot,Transform newParent = null ){
        if(!HasItem()){
            return;
        }
        heldItem.transform.SetParent(newParent);
        heldItem.transform.localPosition = targetLocalPos;
        heldItem.transform.localRotation = targetLocalRot;
        if (heldItem.TryGetComponent<Rigidbody>(out Rigidbody itemRb))
        {
            itemRb.isKinematic = false; 
        }
        heldItem=null;


    }
}
