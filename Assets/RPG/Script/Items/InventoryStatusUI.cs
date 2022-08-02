using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryStatusUI : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] GameObject inventoryStatusPanel;
    [SerializeField] Text nameText;
    [SerializeField] Text hpText;
    [SerializeField] Text mpText;
    [SerializeField] Text conditionText;
    [SerializeField] GameObject aminaInventoryStatusPanel;
    [SerializeField] Text aminaNameText;
    [SerializeField] Text aminaHpText;
    [SerializeField] Text aminaMpText;
    [SerializeField] Text aminaConditionText;


    private void Init()
    {
        if (inventoryUI.SelectedItemType == 1)
        {
            nameText.text = $"名前：{ player.Battlers[0].Base.Name}";
            hpText.text = $"武：{ player.Battlers[0].Base.GetEquipWeapon().Base.GetKanjiName()}";
            mpText.text = $"防：{ player.Battlers[0].Base.GetEquipArmor().Base.GetKanjiName()}";
                conditionText.text = $"装：{player.Battlers[0].Base.GetEquipAccessory().Base.GetKanjiName()}";
            if (player.Battlers.Count == 2)
            {
                aminaNameText.text = $"名前：{ player.Battlers[1].Base.Name}";
                aminaHpText.text = $"武：{ player.Battlers[1].Base.GetEquipWeapon().Base.GetKanjiName()}";
                aminaMpText.text = $"防：{ player.Battlers[1].Base.GetEquipArmor().Base.GetKanjiName()}";
                    aminaConditionText.text = $"装：{player.Battlers[1].Base.GetEquipAccessory().Base.GetKanjiName()}";
            }
        }
        else
        {
            nameText.text = $"名前：{ player.Battlers[0].Base.Name}";
            hpText.text = $"HP：{ player.Battlers[0].HP}";
            mpText.text = $"MP：{ player.Battlers[0].MP}";
            if (player.Battlers[0].Status == null)
            {
                conditionText.text = "状態：正常";
            }
            else
            {
                conditionText.text = $"状態：{player.Battlers[0].Status.Name}";
            }
            if (player.Battlers.Count == 2)
            {
                aminaNameText.text = $"名前：{ player.Battlers[1].Base.Name}";
                aminaHpText.text = $"HP：{ player.Battlers[1].HP}";
                aminaMpText.text = $"MP：{ player.Battlers[1].MP}";
                if (player.Battlers[1].Status == null)
                {
                    aminaConditionText.text = "状態：正常";
                }
                else
                {
                    aminaConditionText.text = $"状態：{player.Battlers[1].Status.Name}";
                }
            }

        }
    }

    public void Open()
    {
        inventoryStatusPanel.SetActive(true);
        if (player.Battlers.Count == 2)
        {
            aminaInventoryStatusPanel.SetActive(true);
        }
        Init();

    }
    public void Close()
    {
        inventoryStatusPanel.SetActive(false);
        aminaInventoryStatusPanel.SetActive(false);
    }

    public void PlayerSet(Battler battler)
    {
        nameText.text = battler.Base.Name;
        PlayerUpdateUI(battler);
    }

    public void PlayerUpdateUI(Battler battler)
    {
        if (inventoryUI.SelectedItemType==1)
        {
            hpText.text = $"武：{ battler.Base.GetEquipWeapon().Base.GetKanjiName()}";
            mpText.text = $"防：{ battler.Base.GetEquipWeapon().Base.GetKanjiName()}";
            conditionText.text = $"装：{battler.Base.GetEquipAccessory().Base.GetKanjiName()}";

        }
        else
        {
        hpText.text = $"HP：{ battler.HP}";
        mpText.text = $"MP：{ battler.MP}";
        if (battler.Status == null)
        {
            conditionText.text = "状態：正常";
        }
        else
        {
            conditionText.text = $"状態：{battler.Status.Name}";
        }

        }
    }

    public void AminaSet(Battler battler)
    {
        aminaNameText.text = battler.Base.Name;
        AminaUpdateUI(battler);
    }

    public void AminaUpdateUI(Battler battler)
    {
        if (inventoryUI.SelectedItemType == 1)
        {
            aminaHpText.text = $"武：{ battler.Base.GetEquipWeapon().Base.GetKanjiName()}";
            aminaMpText.text = $"防：{ battler.Base.GetEquipWeapon().Base.GetKanjiName()}";
            aminaConditionText.text = $"装：{battler.Base.GetEquipAccessory().Base.GetKanjiName()}";

        }
        else
        {
            aminaHpText.text = $"HP：{ battler.HP}";
            aminaMpText.text = $"MP：{ battler.MP}";
            if (battler.Status == null)
            {
                aminaConditionText.text = "状態：正常";
            }
            else
            {
                aminaConditionText.text = $"状態：{battler.Status.Name}";
            }

        }
    }


}
