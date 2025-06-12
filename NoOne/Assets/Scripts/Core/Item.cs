using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    public string itemName;
    public int itemID;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("觸發了碰撞！碰撞對象：" + collision.name + "，Tag：" + collision.tag);
        if (collision.CompareTag("Player"))
        {
            Debug.Log("確認是玩家觸發！");
            GameManagerLevel1.instance.ItemCollected(this);
            gameObject.SetActive(false);
        }
    }
}