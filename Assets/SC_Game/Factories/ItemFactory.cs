using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Список объектов - препятствия, бонусы и тд. в игре
/// </summary>
/// 
public enum MAP_ITEM
{
    BULLET_BOX = 1,
    COIN = 2,
    JEWEL = 3,
    WALK_BALL_A = 4,
    BOX_LONG = 5,
    LAZER_RED = 6,
    SPIKE_STICK = 7,
    BOX_SMALL = 8,
    BARREL_RED = 9,

    __end = 255
}

public interface IItemFactory
{
    IMapItem Create(ItemInfo itemInfo);
}


namespace RBGame.Factory
{
    public class ItemFactory : MonoBehaviour, IItemFactory
    {
        [System.Serializable]
        public class ItemDescription
        {
            public MAP_ITEM type;
            public GameObject prefab;
        }

        [SerializeField] List<ItemDescription> items;

        public IMapItem Create(ItemInfo itemInfo)
        {
            var etype = (MAP_ITEM)System.Enum.Parse(typeof(MAP_ITEM), itemInfo.prefab);
            var prefab = items.Find(x => x.type == etype);
            if (prefab != null)
            {
                var item = Instantiate(prefab.prefab).GetComponent<Item>();
                item.itemID = itemInfo.id;
                item.itemType = (MAP_ITEM_TYPE)System.Enum.Parse(typeof(MAP_ITEM_TYPE), itemInfo.item_type);
                return item;
            }
            return null;
        }
    }
}