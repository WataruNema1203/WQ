using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShopNPCController : MonoBehaviour, IInteract
{
    [SerializeField] ShopNPC shopNPC;
    public UnityAction<List<Item>,int> OnShopStart;

    public ShopNPC ShopNPC { get => shopNPC;}

    public void Interact(Transform trf)
    {
        int index = 0;
        for (int i = 0; i < GameController.Instance.ShopNPCs.Length; i++)
        {
            if (GameController.Instance.ShopNPCs[i].shopNPC.Base.Name== shopNPC.Base.Name)
            {
                index = i;
            }
            
        }
        StartCoroutine(DialogManager.Instance.ShopShowDialog(shopNPC.Base.Dialog,shopNPC.Base.Items,index,OnShopStart));
    }

}
