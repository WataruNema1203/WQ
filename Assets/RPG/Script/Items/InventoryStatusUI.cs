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
            nameText.text = $"���O�F{ player.Battlers[0].Base.Name}";
            hpText.text = $"���F{ player.Battlers[0].Base.GetEquipWeapon().Base.GetKanjiName()}";
            mpText.text = $"�h�F{ player.Battlers[0].Base.GetEquipArmor().Base.GetKanjiName()}";
                conditionText.text = $"���F{player.Battlers[0].Base.GetEquipAccessory().Base.GetKanjiName()}";
            if (player.Battlers.Count == 2)
            {
                aminaNameText.text = $"���O�F{ player.Battlers[1].Base.Name}";
                aminaHpText.text = $"���F{ player.Battlers[1].Base.GetEquipWeapon().Base.GetKanjiName()}";
                aminaMpText.text = $"�h�F{ player.Battlers[1].Base.GetEquipArmor().Base.GetKanjiName()}";
                    aminaConditionText.text = $"���F{player.Battlers[1].Base.GetEquipAccessory().Base.GetKanjiName()}";
            }
        }
        else
        {
            nameText.text = $"���O�F{ player.Battlers[0].Base.Name}";
            hpText.text = $"HP�F{ player.Battlers[0].HP}";
            mpText.text = $"MP�F{ player.Battlers[0].MP}";
            if (player.Battlers[0].Status == null)
            {
                conditionText.text = "��ԁF����";
            }
            else
            {
                conditionText.text = $"��ԁF{player.Battlers[0].Status.Name}";
            }
            if (player.Battlers.Count == 2)
            {
                aminaNameText.text = $"���O�F{ player.Battlers[1].Base.Name}";
                aminaHpText.text = $"HP�F{ player.Battlers[1].HP}";
                aminaMpText.text = $"MP�F{ player.Battlers[1].MP}";
                if (player.Battlers[1].Status == null)
                {
                    aminaConditionText.text = "��ԁF����";
                }
                else
                {
                    aminaConditionText.text = $"��ԁF{player.Battlers[1].Status.Name}";
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
            hpText.text = $"���F{ battler.Base.GetEquipWeapon().Base.GetKanjiName()}";
            mpText.text = $"�h�F{ battler.Base.GetEquipWeapon().Base.GetKanjiName()}";
            conditionText.text = $"���F{battler.Base.GetEquipAccessory().Base.GetKanjiName()}";

        }
        else
        {
        hpText.text = $"HP�F{ battler.HP}";
        mpText.text = $"MP�F{ battler.MP}";
        if (battler.Status == null)
        {
            conditionText.text = "��ԁF����";
        }
        else
        {
            conditionText.text = $"��ԁF{battler.Status.Name}";
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
            aminaHpText.text = $"���F{ battler.Base.GetEquipWeapon().Base.GetKanjiName()}";
            aminaMpText.text = $"�h�F{ battler.Base.GetEquipWeapon().Base.GetKanjiName()}";
            aminaConditionText.text = $"���F{battler.Base.GetEquipAccessory().Base.GetKanjiName()}";

        }
        else
        {
            aminaHpText.text = $"HP�F{ battler.HP}";
            aminaMpText.text = $"MP�F{ battler.MP}";
            if (battler.Status == null)
            {
                aminaConditionText.text = "��ԁF����";
            }
            else
            {
                aminaConditionText.text = $"��ԁF{battler.Status.Name}";
            }

        }
    }


}
