using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "PartyStatus", menuName = "CreatePartyStatus")]
public class PartyStatus : ScriptableObject
{
    [SerializeField]
    private int money = 0;
    [SerializeField]
    private List<Battler> partyMembers = null;

    public void SetMoney(int money)
    {
        this.money = money;
    }

    public int GetMoney()
    {
        return money;
    }

    public void SetAllyStatus(Battler battler)
    {
        if (!partyMembers.Contains(battler))
        {
            partyMembers.Add(battler);
        }
    }

    public List<Battler> GetAllyStatus()
    {
        return partyMembers;
    }

}