using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] Items;
    public Dictionary<ItemObject, int> GetId = new Dictionary<ItemObject, int>();
    public Dictionary<int,ItemObject> GetItem = new Dictionary<int,ItemObject>();
    
    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        GetId = new Dictionary<ItemObject, int>();
        GetItem = new Dictionary<int, ItemObject>();
        for (int i = 0; i < Items.Length; i++)
        {
            GetId.Add(Items[i], i);
            GetItem.Add(i,Items[i]);
        }
    }
}
