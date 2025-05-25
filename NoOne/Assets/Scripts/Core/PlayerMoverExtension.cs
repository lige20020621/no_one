// 使用您現有的PlayerMover.cs，只需要添加進度UI功能
// PlayerMoverExtension.cs - 為您的PlayerMover添加UI更新功能
using UnityEngine;

public class PlayerMoverExtension : MonoBehaviour
{
    private PlayerMover playerMover;

    void Start()
    {
        playerMover = GetComponent<PlayerMover>();
        if (playerMover == null)
        {
            Debug.LogError("PlayerMoverExtension需要與PlayerMover在同一個GameObject上！");
        }
    }


    // 獲取PlayerMover的引用，供其他腳本使用
    public PlayerMover GetPlayerMover()
    {
        return playerMover;
    }
}