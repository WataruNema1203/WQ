using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fungus;

public class BattleSystem : MonoBehaviour
{
    enum PatternState
    {
        Start,
        PlayerActionSelection,
        AminaActionSelection,
        PlayerMoveSelection,
        AminaMoveSelection,
        PlayerItemSelection,
        AminaItemSelection,
        PlayerEnemySelection,
        AminaEnemySelection,
        PlayerSelfMoveSelection,
        AminaSelfMoveSelection,
        RunTurns,
        BattleOver,
        Escape,
        Busy,
    }

    enum PlayerActionPattern
    {
        None,
        PlayerAttack,
        PlayerItem,
        PlayerSelf,
    }

    enum AminaActionPattern
    {
        None,
        AminaAttack,
        AminaItem,
        AminaSelf,
    }


    PatternState state;
    PlayerActionPattern playerActionPattern;
    AminaActionPattern aminaActionPattern;

    [SerializeField] ActionSelectionUI actionSelectionUI;
    [SerializeField] MoveSelectionUI playerMoveSelectionUI;
    [SerializeField] MoveSelectionUI aminaMoveSelectionUI;
    [SerializeField] CharaSelectionUI charaSelectionUI;
    [SerializeField] EnemySelectionUI enemySelectionUI;
    [SerializeField] InventoryUI inventoryUI;

    [SerializeField] EnemyUnitManager enemyUnitManager;
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit aminaUnit;

    public UnityAction<bool> OnBattleOver;
    public UnityAction<Item> OnDropItem;
    bool isBossBattle;
    int playerActionIndex;
    int aminaActionIndex;
    int playerMoveIndex;
    int aminaMoveIndex;
    int battleOverMenber;
    readonly int itemSelectPlayer = 0;
    readonly int itemSelectAmina = 1;
    int playerItemSelectIndex;
    int aminaItemSelectIndex;
    int playerUseCharaSelect;
    int aminaUseCharaSelect;
    int playerEnemySelect;
    int aminaEnemySelect;
    int playerSelfMoveSelect;
    int aminaSelfMoveSelect;


    public void HandleUpdate()
    {
        switch (state)
        {
            case PatternState.Start:
                break;
            case PatternState.PlayerActionSelection:
                HandlePlayerActionSelection();
                break;
            case PatternState.AminaActionSelection:
                HandleAminaActionSelection();
                break;
            case PatternState.PlayerMoveSelection:
                HandlePlayerMoveSelection();
                break;
            case PatternState.AminaMoveSelection:
                HandleAminaMoveSelection();
                break;
            case PatternState.PlayerEnemySelection:
                HandlePlayerEnemySelection();
                break;
            case PatternState.AminaEnemySelection:
                HandleAminaEnemySelection();
                break;
            case PatternState.PlayerSelfMoveSelection:
                HandlePlayerSelfSelection();
                break;
            case PatternState.AminaSelfMoveSelection:
                HandleAminaSelfSelection();
                break;
            case PatternState.PlayerItemSelection:
                HandlePlayerItemSelection();
                break;
            case PatternState.AminaItemSelection:
                HandleAminaItemSelection();
                break;
            case PatternState.RunTurns:
                break;
            case PatternState.Escape:
                HandleUpdateEscapeSelection();
                break;
        }
    }

    public void BattleStart(PlayerController player, List<Battler> enemes, bool isBossBattle = false)
    {
        gameObject.SetActive(true);
        this.isBossBattle = isBossBattle;
        state = PatternState.Start;
        actionSelectionUI.Init();
        playerMoveSelectionUI.SetText(player.Battlers[0].Moves);
        if (player.Battlers.Count == 2)
        {
            aminaMoveSelectionUI.SetText(player.Battlers[1].Moves);
        }
        PlayerActionSelection();
        StartCoroutine(SetupBattle(player, enemes));
    }


    IEnumerator SetupBattle(PlayerController player, List<Battler> enemes)
    {
        playerActionPattern = PlayerActionPattern.None;
        aminaActionPattern = AminaActionPattern.None;
        playerUnit.Setup(player.Battlers[0]);
        yield return StartCoroutine(enemyUnitManager.Init(enemes));
        enemySelectionUI.Init(enemyUnitManager.EnemyUnits);
        if (player.Battlers.Count == 2)
        {
            aminaUnit.gameObject.SetActive(true);
            aminaUnit.Setup(player.Battlers[1]);
            aminaUnit.Battler.isDai = false;
        }
        else
        {
            playerUnit.Battler.isDai = false;
        }
        state = PatternState.Busy;
        if (enemes.Count == 1)
        {
            yield return DialogManager.Instance.TypeDialog($"{enemes[0].Base.Name}が現れた！");
        }
        else
        {
            yield return DialogManager.Instance.TypeDialog("モンスター達が現れた！");

        }
        yield return new WaitForSeconds(0.2f);
        if (playerUnit.Battler.HP <= 0 && playerUnit.Battler.isDai == false)
        {
            playerUnit.Battler.isDai = true;
            if (player.Battlers.Count == 2)
            {
                if (aminaUnit.Battler.HP <= 0 && aminaUnit.Battler.isDai == true)
                {
                    aminaUnit.Battler.isDai = false;
                    yield return DialogManager.Instance.TypeDialog($"戦えるメンバーがいない・・・\n今は戦闘を避けよう");
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    yield return null;
                    state = PatternState.Escape;
                }
                else
                {
                    playerActionPattern = PlayerActionPattern.None;
                    StartCoroutine(DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は何の反応もない・・・\n" + $"{aminaUnit.Battler.Base.Name}" + "はどうする？"));
                    AminaActionSelection();


                }
            }
            else
            {
                yield return DialogManager.Instance.TypeDialog($"戦えるメンバーがいない・・・\n今は戦闘を避けよう");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                yield return null;
                state = PatternState.Escape;

            }

        }
        else
        {
            yield return DialogManager.Instance.TypeDialog($"どうする？");
            PlayerActionSelection();

        }
    }


    public void BattleOver(bool win)
    {
        state = PatternState.BattleOver;
        playerMoveSelectionUI.DeleteMoveText();
        aminaMoveSelectionUI.DeleteMoveText();
        enemyUnitManager.Cler();
        enemySelectionUI.BattleOver();
        OnBattleOver?.Invoke(win);
    }

    void PlayerActionSelection()
    {
        state = PatternState.PlayerActionSelection;
        actionSelectionUI.Open(PlayerController.Instance.Battlers[0]);
    }
    void AminaActionSelection()
    {
        state = PatternState.AminaActionSelection;
        playerMoveSelectionUI.Close();
        actionSelectionUI.Open(PlayerController.Instance.Battlers[1]);
    }
    void PlayerMoveSelection()
    {
        state = PatternState.PlayerMoveSelection;
        actionSelectionUI.Close();
        playerMoveSelectionUI.Open();
    }
    void AminaMoveSelection()
    {
        state = PatternState.AminaMoveSelection;
        actionSelectionUI.Close();
        aminaMoveSelectionUI.Open();
    }
    void PlayerSelfMoveSelection()
    {
        DialogManager.Instance.Close();
        state = PatternState.PlayerSelfMoveSelection;
        charaSelectionUI.Open(PlayerController.Instance);
    }
    void AminaSelfMoveSelection()
    {
        DialogManager.Instance.Close();
        state = PatternState.AminaSelfMoveSelection;
        charaSelectionUI.Open(PlayerController.Instance);
    }
    void PlayerItemSelection()
    {
        state = PatternState.PlayerItemSelection;
        inventoryUI.SelectedChara = 0;
        inventoryUI.ItemSelectOpen();
    }
    void AminaItemSelection()
    {
        state = PatternState.AminaItemSelection;
        inventoryUI.SelectedChara = 1;
        inventoryUI.ItemSelectOpen();
    }

    void PlayerEnemySelection()
    {
        DialogManager.Instance.Close();
        state = PatternState.PlayerEnemySelection;
        enemySelectionUI.Open(enemyUnitManager.EnemyUnits);
    }
    void AminaEnemySelection()
    {
        DialogManager.Instance.Close();
        state = PatternState.AminaEnemySelection;
        enemySelectionUI.Open(enemyUnitManager.EnemyUnits);
    }

    void HandlePlayerActionSelection()
    {
        if (playerUnit.Battler.HP <= 0 && playerUnit.Battler.isDai == false)
        {
            playerUnit.Battler.isDai = true;
            if (aminaUnit.gameObject.activeSelf == true)
            {
                if (aminaUnit.Battler.HP <= 0 && aminaUnit.Battler.isDai == true)
                {
                    StartCoroutine(DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は何の反応もない・・・"));
                    state = PatternState.Escape;
                }
                else
                {
                    playerActionPattern = PlayerActionPattern.None;
                    StartCoroutine(DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は何の反応もない・・・\n" + $"{aminaUnit.Battler.Base.Name}" + "はどうする？"));
                    AminaActionSelection();


                }
            }
        }
        else
        {
            actionSelectionUI.HandleUpdate();
            if (Input.GetKeyDown(KeyCode.Z))
            {
                playerActionIndex = actionSelectionUI.SelectedIndex;
                if (playerActionIndex == 0)
                {
                    playerActionPattern = PlayerActionPattern.PlayerAttack;
                    PlayerEnemySelection();

                }
                else if (playerActionIndex == 1)
                {
                    PlayerMoveSelection();
                }
                else if (playerActionIndex == 2)
                {
                    PlayerItemSelection();
                }
                else if (playerActionIndex == 3)
                {
                    if (isBossBattle)
                    {
                        state = PatternState.Busy;
                        StartCoroutine(MessageEscapeAboutBoss());
                    }
                    else
                    {
                        state = PatternState.Escape;
                    }
                }
            }
        }
    }
    void HandleAminaActionSelection()
    {
        actionSelectionUI.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            aminaActionIndex = actionSelectionUI.SelectedIndex;
            if (aminaActionIndex == 0)
            {
                aminaActionPattern = AminaActionPattern.AminaAttack;
                actionSelectionUI.Close();
                AminaEnemySelection();
            }
            else if (aminaActionIndex == 1)
            {
                AminaMoveSelection();
            }
            else if (aminaActionIndex == 2)
            {
                AminaItemSelection();
            }
            else if (aminaActionIndex == 3)
            {
                if (isBossBattle)
                {
                    state = PatternState.Busy;
                    StartCoroutine(MessageEscapeAboutBoss());
                }
                else
                {
                    state = PatternState.Escape;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (playerUnit.Battler.isDai == false)
            {
                PlayerActionSelection();
            }
            else
            {
                StartCoroutine(DialogManager.Instance.TypeDialog("主人公は息をしていない"));
            }
        }

    }
    void HandlePlayerMoveSelection()
    {
        playerMoveSelectionUI.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            playerMoveIndex = playerMoveSelectionUI.SelectedIndex;
            int useMP = playerUnit.Battler.Moves[playerMoveIndex].Base.Mp;
            if (useMP <= playerUnit.Battler.MP)
            {
                if (playerUnit.Battler.Moves[playerMoveIndex].Base.Category1 == MoveCategory1.FullHeal ||
                    playerUnit.Battler.Moves[playerMoveIndex].Base.Category1 == MoveCategory1.Heal || playerUnit.Battler.Moves[playerMoveIndex].Base.Category1 == MoveCategory1.Resuscitation)
                {
                    playerActionPattern = PlayerActionPattern.PlayerSelf;
                    playerMoveSelectionUI.Close();
                    PlayerSelfMoveSelection();
                }
                else if (playerUnit.Battler.Moves[playerMoveIndex].Base.Category1 == MoveCategory1.All)
                {
                    if (aminaUnit.gameObject.activeSelf == true)
                    {
                        if (aminaUnit.Battler.isDai == true)
                        {

                            StartCoroutine(DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は何の反応もない・・・"));
                            aminaActionPattern = AminaActionPattern.None;
                            StartCoroutine(RunTurn());
                        }
                        else
                        {

                            playerActionPattern = PlayerActionPattern.PlayerAttack;
                            playerMoveSelectionUI.Close();
                            StartCoroutine(DialogManager.Instance.TypeDialog($"{ aminaUnit.Battler.Base.Name}はどうする？"));
                            AminaActionSelection();
                        }
                    }
                    else
                    {
                        playerActionPattern = PlayerActionPattern.PlayerAttack;
                        playerMoveSelectionUI.Close();
                        StartCoroutine(RunTurn());
                    }
                }
                else
                {
                    playerActionPattern = PlayerActionPattern.PlayerAttack;
                    playerMoveSelectionUI.Close();
                    PlayerEnemySelection();
                }
            }
            else
            {
                StartCoroutine(DialogManager.Instance.TypeDialog($"MPが足りない!"));
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            playerMoveSelectionUI.Close();
            PlayerActionSelection();
        }
    }
    void HandleAminaMoveSelection()
    {
        aminaMoveSelectionUI.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            aminaMoveIndex = aminaMoveSelectionUI.SelectedIndex;
            int useMP = aminaUnit.Battler.Moves[aminaMoveIndex].Base.Mp;
            if (useMP <= aminaUnit.Battler.MP)
            {
                if (aminaUnit.Battler.Moves[aminaMoveIndex].Base.Category1 == MoveCategory1.FullHeal ||
                    aminaUnit.Battler.Moves[aminaMoveIndex].Base.Category1 == MoveCategory1.Heal || aminaUnit.Battler.Moves[aminaMoveIndex].Base.Category1 == MoveCategory1.Resuscitation)
                {
                    aminaMoveSelectionUI.Close();
                    aminaActionPattern = AminaActionPattern.AminaSelf;
                    AminaSelfMoveSelection();
                }
                else if (aminaUnit.Battler.Moves[aminaMoveIndex].Base.Category1 == MoveCategory1.All)
                {
                    aminaMoveSelectionUI.Close();
                    aminaActionPattern = AminaActionPattern.AminaAttack;
                    StartCoroutine(RunTurn());
                }
                else
                {
                    aminaMoveSelectionUI.Close();
                    aminaActionPattern = AminaActionPattern.AminaAttack;
                    AminaEnemySelection();
                }
            }
            else
            {
                StartCoroutine(DialogManager.Instance.TypeDialog($"MPが足りない!"));
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            aminaMoveSelectionUI.Close();
            AminaActionSelection();
        }

    }
    void HandlePlayerEnemySelection()
    {
        enemySelectionUI.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            playerEnemySelect = enemySelectionUI.SelectedIndex;
            if (enemyUnitManager.EnemyUnits[playerEnemySelect].Battler.HP > 0)
            {
                enemySelectionUI.Close();
                if (playerActionPattern == PlayerActionPattern.PlayerAttack)
                {
                    if (playerActionIndex == 0)
                    {
                        actionSelectionUI.Close();
                        if (aminaUnit.gameObject.activeSelf == true)
                        {
                            if (aminaUnit.Battler.isDai == true)
                            {

                                StartCoroutine(DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は何の反応もない・・・"));
                                aminaActionPattern = AminaActionPattern.None;
                                StartCoroutine(RunTurn());
                            }
                            else
                            {
                                StartCoroutine(DialogManager.Instance.TypeDialog($"{ aminaUnit.Battler.Base.Name}はどうする？"));
                                AminaActionSelection();
                            }
                        }
                        else
                        {
                            StartCoroutine(RunTurn());
                        }
                    }
                    else if (playerActionIndex == 1)
                    {
                        playerMoveSelectionUI.Close();
                        if (GameController.Instance.GetBattlerMember() == 2)
                        {
                            if (aminaUnit.Battler.isDai == false)
                            {
                                StartCoroutine(DialogManager.Instance.TypeDialog($"{ aminaUnit.Battler.Base.Name}はどうする？"));
                                AminaActionSelection();
                            }
                            else
                            {
                                // 技リストから技を出す
                                StartCoroutine(RunTurn());
                            }
                        }
                        else
                        {
                            // 技リストから技を出す
                            StartCoroutine(RunTurn());
                        }
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(DialogManager.Instance.TypeDialog("どうする？"));
            enemySelectionUI.Close();
            PlayerActionSelection();
        }

    }
    void HandleAminaEnemySelection()
    {
        enemySelectionUI.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            aminaEnemySelect = enemySelectionUI.SelectedIndex;
            if (enemyUnitManager.EnemyUnits[aminaEnemySelect].Battler.HP > 0)
            {
                enemySelectionUI.Close();
                if (aminaActionPattern == AminaActionPattern.AminaAttack)
                {
                    StartCoroutine(RunTurn());
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            enemySelectionUI.Close();
            AminaActionSelection();
        }
    }

    void HandlePlayerSelfSelection()
    {
        charaSelectionUI.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            playerSelfMoveSelect = charaSelectionUI.SelectedIndex;
            if (PlayerController.Instance.Battlers[playerSelfMoveSelect].HP != PlayerController.Instance.Battlers[playerSelfMoveSelect].MaxHP)
            {
                charaSelectionUI.Close();
                if (playerActionPattern == PlayerActionPattern.PlayerAttack)
                {
                    if (GameController.Instance.GetBattlerMember() == 2)
                    {
                        if (aminaUnit.Battler.isDai == false)
                        {
                            StartCoroutine(DialogManager.Instance.TypeDialog($"{ aminaUnit.Battler.Base.Name}はどうする？"));
                            AminaActionSelection();
                        }
                        else
                        {
                            // 技リストから技を出す
                            StartCoroutine(RunTurn());
                        }
                    }
                    else
                    {
                        // 技リストから技を出す
                        StartCoroutine(RunTurn());
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(DialogManager.Instance.TypeDialog("どうする？"));
            charaSelectionUI.Close();
            PlayerActionSelection();
        }

    }
    void HandleAminaSelfSelection()
    {
        charaSelectionUI.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            aminaSelfMoveSelect = charaSelectionUI.SelectedIndex;
            if (PlayerController.Instance.Battlers[aminaSelfMoveSelect].HP != PlayerController.Instance.Battlers[aminaSelfMoveSelect].MaxHP)
            {
                charaSelectionUI.Close();
                if (aminaActionPattern == AminaActionPattern.AminaSelf)
                {
                    StartCoroutine(RunTurn());
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(DialogManager.Instance.TypeDialog("どうする？"));
            charaSelectionUI.Close();
            AminaActionSelection();
        }
    }

    void HandlePlayerItemSelection()
    {
        inventoryUI.HandleUpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.Z) && inventoryUI.CanSelectedItem())
        {
            playerItemSelectIndex = inventoryUI.SelectedItem;
            if (inventoryUI.State == ItemStatus.ItemSelect)
            {
                inventoryUI.UseItemCharaSelectOpen();
            }
            else if (inventoryUI.State == ItemStatus.ItemUseCharaSelect && GameController.Instance.GetBattlerMember() == 2)
            {
                playerActionPattern = PlayerActionPattern.PlayerItem;
                playerUseCharaSelect = inventoryUI.SelectedItemUseChara;
                inventoryUI.UseItemCharaSelectClose();
                inventoryUI.ItemSelectClose();
                AminaActionSelection();
            }
            else if (inventoryUI.State == ItemStatus.ItemUseCharaSelect && GameController.Instance.GetBattlerMember() < 2)
            {
                playerActionPattern = PlayerActionPattern.PlayerItem;
                playerUseCharaSelect = inventoryUI.SelectedItemUseChara;
                inventoryUI.UseItemCharaSelectClose();
                inventoryUI.ItemSelectClose();
                StartCoroutine(RunTurn());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (inventoryUI.State == ItemStatus.ItemUseCharaSelect)
            {
                inventoryUI.UseItemCharaSelectClose();
                inventoryUI.ItemSelectOpen();
            }
            else
            {
                state = PatternState.PlayerActionSelection;
                inventoryUI.ItemSelectClose();
            }
        }
    }
    void HandleAminaItemSelection()
    {
        inventoryUI.HandleUpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.Z) && inventoryUI.CanSelectedItem())
        {
            if (inventoryUI.State == ItemStatus.ItemSelect)
            {
                aminaItemSelectIndex = inventoryUI.SelectedItem;
                inventoryUI.UseItemCharaSelectOpen();
            }
            else if (inventoryUI.State == ItemStatus.ItemUseCharaSelect)
            {
                aminaUseCharaSelect = inventoryUI.SelectedItemUseChara;
                inventoryUI.UseItemCharaSelectClose();
                inventoryUI.ItemSelectClose();
                aminaActionPattern = AminaActionPattern.AminaItem;
                StartCoroutine(RunTurn());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (inventoryUI.State == ItemStatus.ItemUseCharaSelect)
            {
                inventoryUI.UseItemCharaSelectClose();
                inventoryUI.ItemSelectOpen();
            }
            else
            {
                state = PatternState.AminaActionSelection;
                inventoryUI.ItemSelectClose();
            }
        }
    }

    void HandleUpdateEscapeSelection()
    {
        BattleOver(true);
    }

    IEnumerator ShowDamageDetails(DamageDetailes damageDetailes)
    {
        if (damageDetailes.Critical > 1f)
        {
            yield return DialogManager.Instance.TypeDialog($"会心の攻撃！");
        }
    }

    IEnumerator MessageEscapeAboutBoss()
    {
        yield return DialogManager.Instance.TypeDialog($"ボスからは逃れられない");
        state = PatternState.PlayerActionSelection;
    }

    IEnumerator RunTurn()
    {
        battleOverMenber = PlayerController.Instance.Battlers.Count;
        bool isAmina = (aminaUnit.gameObject.activeSelf);
        int playerIndex = 0;
        int aminaIndex = 0;
        Debug.Log(isAmina);
        List<BattleUnit> attackers = new List<BattleUnit>
        {
            playerUnit,
        };


        if (battleOverMenber > 1)
        {
            attackers.Add(aminaUnit);
        }

        attackers.AddRange(enemyUnitManager.EnemyUnits);

        for (int i = PlayerController.Instance.Battlers.Count; i < attackers.Count; i++)
        {
            Debug.Log(attackers[i].Battler.Name);
        }

        attackers.Sort((a, b) =>
        {
            int result = b.Battler.Speed - a.Battler.Speed;
            return result;
        });

        for (int i = 0; i < attackers.Count; i++)
        {
            Debug.Log($"ソート後{attackers[i].Battler.Name}");
        }
        for (int i = 0; i < attackers.Count; i++)
        {
            if (attackers[i].Battler.Base.CharaTypes.Chara == CharaType.Player)
            {
                playerIndex = i;
            }
            else if (attackers[i].Battler.Base.CharaTypes.Chara == CharaType.Amina)
            {
                aminaIndex = i;
            }
        }
        state = PatternState.RunTurns;
        for (int i = 0; i < attackers.Count; i++)
        {
            Debug.Log($"{i + 1}体目");
            if (attackers[i].Battler.Base.CharaTypes.Chara == CharaType.Enemy && attackers[i].Battler.isDai == false)
            {
                int mod = Random.Range(0, 2);
                if (mod == 0)
                {
                    int modTarget = Random.Range(0, 2);
                    Move enemyMove = attackers[i].Battler.GetRandomMove();
                    if (modTarget == 0)
                    {
                        yield return RunMove(attackers[i], attackers[playerIndex], enemyMove);

                    }
                    else if (modTarget == 1)
                    {
                        yield return RunMove(attackers[i], attackers[aminaIndex], enemyMove);
                    }
                }
                else if (mod == 1)
                {
                    int modTarget = Random.Range(0, 2);
                    if (modTarget == 0)
                    {
                        yield return RunNormalMove(attackers[i], attackers[playerIndex]);
                    }
                    else if (modTarget == 1)
                    {
                        yield return RunNormalMove(attackers[i], attackers[aminaIndex]);
                    }
                }
                if (attackers[playerIndex].Battler.HP <= 0 && playerUnit.Battler.isDai == true && attackers[playerIndex].Battler.Base.CharaTypes.Chara == CharaType.Player)
                {
                    playerUnit.Battler.isDai = false;
                    yield return DialogManager.Instance.TypeDialog($"{attackers[playerIndex].Battler.Name}は倒れて意識を失った...");
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                }
                else if (attackers[aminaIndex].Battler.HP <= 0 && aminaUnit.Battler.isDai == true && attackers[playerIndex].Battler.Base.CharaTypes.Chara == CharaType.Amina)
                {
                    aminaUnit.Battler.isDai = false;
                    yield return DialogManager.Instance.TypeDialog($"{attackers[aminaIndex].Battler.Name}は倒れて意識を失った...");
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

                }
                for (int j = 0; j < PlayerController.Instance.Battlers.Count; j++)
                {
                    if (PlayerController.Instance.Battlers[j].HP <= 0)
                    {
                        battleOverMenber--;
                    }
                }
                if (battleOverMenber <= 0)
                {
                    yield return DialogManager.Instance.TypeDialog($"全員潰れて行方が分からなくなった・・・");
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    BattleOver(false);
                }
                continue;

            }
            else if (attackers[i].Battler.Base.CharaTypes.Chara == CharaType.Player && attackers[i].Battler.isDai == false)
            {
                if (playerActionPattern == PlayerActionPattern.PlayerItem)
                {
                    yield return inventoryUI.UseInBattle(itemSelectPlayer, playerItemSelectIndex, playerUseCharaSelect);
                    attackers[playerIndex].Setup(attackers[playerIndex].Battler);
                    attackers[aminaIndex].Setup(attackers[aminaIndex].Battler);
                }
                else if (playerActionPattern == PlayerActionPattern.PlayerAttack)
                {
                    Move playerMove = attackers[playerIndex].Battler.Moves[playerMoveIndex];
                    if (isAmina == true)
                    {
                        if (playerActionIndex == 0)
                        {
                            yield return RunNormalMove(attackers[playerIndex], enemyUnitManager.EnemyUnits[playerEnemySelect]);
                            yield return OnCheckBattleOver(attackers, playerEnemySelect, playerIndex, aminaIndex, aminaUnit.Battler.isDai);
                        }
                        else
                        {

                            if (playerMove.Base.Category1 == MoveCategory1.All)
                            {
                                yield return RunAllMove(attackers[playerIndex], attackers, playerMove);
                                yield return OnCheckBattleOver(attackers, playerEnemySelect, playerIndex, aminaIndex, aminaUnit.Battler.isDai, true);
                            }
                            else
                            {
                                yield return RunMove(attackers[playerIndex], enemyUnitManager.EnemyUnits[playerEnemySelect], playerMove);
                                yield return OnCheckBattleOver(attackers, playerEnemySelect, playerIndex, aminaIndex, aminaUnit.Battler.isDai);
                            }
                        }
                    }
                    else
                    {
                        if (playerActionIndex == 0)
                        {
                            yield return RunNormalMove(attackers[playerIndex], enemyUnitManager.EnemyUnits[playerEnemySelect]);
                            yield return OnCheckBattleOver(attackers, playerEnemySelect, playerIndex, aminaIndex, false, isAmina);
                        }
                        else
                        {
                            if (playerMove.Base.Category1 == MoveCategory1.All)
                            {
                                yield return RunAllMove(attackers[playerIndex], attackers, playerMove);
                                yield return OnCheckBattleOver(attackers, playerEnemySelect, playerIndex, aminaIndex, false, isAmina);
                            }
                            else
                            {
                                yield return RunMove(attackers[playerIndex], enemyUnitManager.EnemyUnits[playerEnemySelect], playerMove);
                                yield return OnCheckBattleOver(attackers, playerEnemySelect, playerIndex, aminaIndex, false, isAmina);
                            }
                        }

                    }
                }
            }
            else if (attackers[i].Battler.Base.CharaTypes.Chara == CharaType.Amina && attackers[i].Battler.isDai == false)
            {
                if (aminaActionPattern == AminaActionPattern.AminaItem)
                {
                    yield return inventoryUI.UseInBattle(itemSelectAmina, aminaItemSelectIndex, aminaUseCharaSelect);
                    attackers[playerIndex].Setup(attackers[playerIndex].Battler);
                    attackers[aminaIndex].Setup(attackers[aminaIndex].Battler);

                }
                else if (aminaActionPattern == AminaActionPattern.AminaAttack)
                {
                    Move aminaMove = attackers[aminaIndex].Battler.Moves[aminaMoveIndex];
                    if (aminaActionIndex == 0)
                    {
                        yield return RunNormalMove(attackers[aminaIndex], enemyUnitManager.EnemyUnits[aminaEnemySelect]);
                        yield return OnCheckBattleOver(attackers, aminaEnemySelect, aminaIndex, playerIndex, playerUnit.Battler.isDai);

                    }
                    else
                    {
                        if (aminaMove.Base.Category1 == MoveCategory1.All)
                        {
                            yield return RunAllMove(attackers[aminaIndex], attackers, aminaMove);
                            yield return OnCheckBattleOver(attackers, aminaEnemySelect, aminaIndex, playerIndex, playerUnit.Battler.isDai, true);
                        }
                        else
                        {
                            yield return RunMove(attackers[aminaIndex], enemyUnitManager.EnemyUnits[aminaEnemySelect], aminaMove);
                            yield return OnCheckBattleOver(attackers, aminaEnemySelect, aminaIndex, playerIndex, playerUnit.Battler.isDai);
                        }
                    }
                }

            }
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        yield return DialogManager.Instance.TypeDialog("どうする？");
        PlayerActionSelection();
    }

    IEnumerator DropItemCheck()
    {
        for (int i = 0; i < enemyUnitManager.EnemyUnits.Count; i++)
        {
            if (Random.Range(0, 100) <= enemyUnitManager.EnemyUnits[i].Battler.Base.DropItem1.Chance)
            {
                yield return DialogManager.Instance.TypeDialog($"{enemyUnitManager.EnemyUnits[i].Battler.Name}が宝箱を落とした");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                OnDropItem?.Invoke(enemyUnitManager.EnemyUnits[i].Battler.Base.DropItem1.DropItem);
                yield return DialogManager.Instance.TypeDialog($"{enemyUnitManager.EnemyUnits[i].Battler.Base.DropItem1.DropItem.Base.GetKanjiName()}を拾った！");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            }
            if (Random.Range(0, 100) <= enemyUnitManager.EnemyUnits[i].Battler.Base.DropItem2.Chance)
            {
                yield return DialogManager.Instance.TypeDialog($"{enemyUnitManager.EnemyUnits[i].Battler.Name}が宝箱を落とした");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                OnDropItem?.Invoke(enemyUnitManager.EnemyUnits[i].Battler.Base.DropItem2.DropItem);
                yield return DialogManager.Instance.TypeDialog($"{enemyUnitManager.EnemyUnits[i].Battler.Base.DropItem2.DropItem.Base.GetKanjiName()}を拾った！");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            }

        }

    }

    //技の実装（実行するUnit、対象Unit,技）
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Battler.OnBeforeMove();　//TODO:技を使うモンスター(sourceUnit)が動けるかどうか
        yield return ShowStatusChanges(sourceUnit.Battler);

        if (!canRunMove)
        {
            //技が使えない場合でダメージを受ける場合もある（こんらん）
            yield break;
        }
        yield return DialogManager.Instance.TypeDialog($"{sourceUnit.Battler.Name}は{move.Base.Name}をつかった", auto: false);
        sourceUnit.Battler.MP -= move.Base.Mp;
        sourceUnit.UpdateUI();
        yield return new WaitForSeconds(0.2f);



        //ステータス変化なら
        if (move.Base.Category1 == MoveCategory1.Stat)
        {
            yield return RunMoveEffects(move.Base.Effects, sourceUnit.Battler, targetUnit.Battler, move.Base.Target);
        }
        else
        {
            //targetへのdamage計算
            DamageDetailes damageDetailes;
            damageDetailes = targetUnit.Battler.TakeDamage(move, sourceUnit.Battler, targetUnit.Battler);
            string resultText = targetUnit.Battler.ResultText;
            yield return DialogManager.Instance.TypeDialog(resultText);

            //クリティカルのメッセージ
            yield return ShowDamageDetails(damageDetailes);
        }
        //追加効果チェック
        //move.Base.Secondariesがnullではない
        //move.Base.Secondariesがある
        //ターゲットがやられていない
        if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Battler.HP > 0)
        {
            //それぞれの追加効果を反映
            foreach (SecondaryEffects secondary in move.Base.Secondaries)
            {
                //一定確率で状態異常
                if (Random.Range(1, 101) <= secondary.Chance)
                {
                    yield return RunMoveEffects(secondary, sourceUnit.Battler, targetUnit.Battler, move.Base.Target);
                }
            }
        }
        if (targetUnit.Battler.HP <= 0 && targetUnit.Battler.Base.CharaTypes.Chara == CharaType.Enemy && targetUnit.Battler.isDai == false)
        {
            targetUnit.Battler.isDai = true;
        }

        sourceUnit.UpdateUI();
        targetUnit.UpdateUI();

        yield return RunAfterTurn(sourceUnit);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
    }

    //技の実装（実行するUnit、対象Unit,技）
    IEnumerator RunAllMove(BattleUnit sourceUnit, List<BattleUnit> targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Battler.OnBeforeMove();　//TODO:技を使うモンスター(sourceUnit)が動けるかどうか
        yield return ShowStatusChanges(sourceUnit.Battler);
        List<BattleUnit> units = new List<BattleUnit>();
        units.AddRange(targetUnit);
        units.Sort((a, b) =>
        {
            int result = a.Battler.Base.CharaTypes.EnemyNumber - b.Battler.Base.CharaTypes.EnemyNumber;
            return result != 0 ? result : a.Battler.BattlerIndex - b.Battler.BattlerIndex;
        });

        if (!canRunMove)
        {
            //技が使えない場合でダメージを受ける場合もある（こんらん）
            yield break;
        }
        yield return DialogManager.Instance.TypeDialog($"{sourceUnit.Battler.Name}は{move.Base.Name}をつかった", auto: false);
        sourceUnit.Battler.MP -= move.Base.Mp;
        sourceUnit.UpdateUI();
        yield return new WaitForSeconds(0.2f);

        DamageDetailes damageDetailes = null;

        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].Battler.isDai == false && units[i].Battler.Base.CharaTypes.Chara == CharaType.Enemy)
            {
                //targetへのdamage計算
                damageDetailes = units[i].Battler.TakeDamage(move, sourceUnit.Battler, units[i].Battler);
                string resultText = units[i].Battler.ResultText;
                yield return DialogManager.Instance.TypeDialog(resultText);

                //追加効果チェック
                //move.Base.Secondariesがnullではない
                //move.Base.Secondariesがある
                //ターゲットがやられていない
                if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && units[i].Battler.HP > 0)
                {
                    //それぞれの追加効果を反映
                    foreach (SecondaryEffects secondary in move.Base.Secondaries)
                    {
                        //一定確率で状態異常
                        if (Random.Range(1, 101) <= secondary.Chance)
                        {
                            yield return RunMoveEffects(secondary, sourceUnit.Battler, units[i].Battler, move.Base.Target);
                        }
                    }
                }
                //クリティカルのメッセージ
                yield return ShowDamageDetails(damageDetailes);
                units[i].UpdateUI();
            }
            if (units[i].Battler.HP <= 0 && units[i].Battler.Base.CharaTypes.Chara == CharaType.Enemy && units[i].Battler.isDai == false)
            {
                units[i].Battler.isDai = true;
                yield return StartCoroutine(units[i].FadeBattleOver());
                yield return DialogManager.Instance.TypeDialog($"{units[i].Battler.Name}を倒した！");
            }

            sourceUnit.UpdateUI();

            yield return RunAfterTurn(sourceUnit);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }
    }

    //通常攻撃技の実装（実行するUnit、対象Unit,技）
    IEnumerator RunNormalMove(BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        bool canRunMove = sourceUnit.Battler.OnBeforeMove();　//TODO:技を使うモンスター(sourceUnit)が動けるかどうか
        yield return ShowStatusChanges(sourceUnit.Battler);

        if (!canRunMove)
        {
            //技が使えない場合でダメージを受ける場合もある（こんらん）
            yield break;
        }
        yield return DialogManager.Instance.TypeDialog($"{sourceUnit.Battler.Name}の通常攻撃！", auto: false);
        yield return new WaitForSeconds(0.2f);



        //Enemydamage計算
        DamageDetailes damageDetailes;
        damageDetailes = targetUnit.Battler.NormalAttackTakeDamage(sourceUnit.Battler, targetUnit.Battler);
        string resultText = targetUnit.Battler.ResultText;
        yield return DialogManager.Instance.TypeDialog(resultText);

        //クリティカルのメッセージ
        yield return ShowDamageDetails(damageDetailes);

        sourceUnit.UpdateUI();
        targetUnit.UpdateUI();


        if (targetUnit.Battler.HP <= 0 && targetUnit.Battler.isDai == false)
        {
            targetUnit.Battler.isDai = true;
        }
        yield return RunAfterTurn(sourceUnit);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {

        if (sourceUnit.Battler.HP <= 0)
        {
            yield break;
        }
        //RunningTurnまで待機する
        yield return new WaitUntil(() => state == PatternState.RunTurns);

        //ターン終了時
        //（状態異常ダメージを受ける）
        sourceUnit.Battler.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Battler);
        sourceUnit.UpdateUI();

        //戦闘不能ならメッセージ
        if (sourceUnit.Battler.HP <= 0)
        {
            yield return DialogManager.Instance.TypeDialog($"{sourceUnit.Battler.Name}が倒れた");
            sourceUnit.Battler.isDai = true;
            sourceUnit.Battler.OnBattleOver();
        }
    }


    IEnumerator RunMoveEffects(MoveEffects effects, Battler source, Battler Target, MoveTarget moveTarget)
    {
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                //自分に対してステータス変化をする
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                //相手に対してステータス変化をする
                Target.ApplyBoosts(effects.Boosts);

            }
            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(Target);
        }

        //何かしらの状態異常技であれば
        if (effects.Status != ConditionID.None)
        {
            Target.SetStatus(effects.Status);
            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(Target);



        }
        if (effects.VolatileStatus != ConditionID.None)
        {

            Target.SetVolatileStatus(effects.VolatileStatus);
            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(Target);



        }
    }
    //ステータス変化のログを表示する関数
    IEnumerator ShowStatusChanges(Battler battler)
    {
        //ログがなくなるまで繰り返す
        while (battler.StatusChanges.Count > 0)
        {
            //ログの取り出し
            string message = battler.StatusChanges.Dequeue();
            yield return DialogManager.Instance.TypeDialog(message);
        }
    }


    //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
    IEnumerator OnLevelUp(BattleUnit playerUnit)
    {
        while (playerUnit.Battler.HasExp >= (400 * playerUnit.Battler.Level * playerUnit.Battler.Level / (204 - playerUnit.Battler.Level)) + 5)
        {
            //一定以上経験値がたまると、レベルが上がる
            if (playerUnit.Battler.IsLevelUp(playerUnit))
            {
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Name}はレベル{playerUnit.Battler.Level}になった！");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                //特定のレベルなら技を覚える
                Move learnedMove = playerUnit.Battler.LearnedMove();
                if (learnedMove != null)
                {
                    yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Name}は{learnedMove.Base.Name}を覚えた！");
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                }
            }
        }
    }

    IEnumerator OnCheckBattleOver(List<BattleUnit> attackers, int enemySelectIndex, int sourceIndex, int partnerIndex, bool isDai, bool isPartner = true)
    {
        int sumExp = 0;
        int sumGold = 0;
        Move Move = null;
        if (attackers[sourceIndex].Battler.Base.CharaTypes.Chara == CharaType.Player)
        {
            Move = attackers[sourceIndex].Battler.Moves[playerMoveIndex];
        }
        else if (attackers[sourceIndex].Battler.Base.CharaTypes.Chara == CharaType.Amina)
        {
            Move = attackers[sourceIndex].Battler.Moves[aminaMoveIndex];
        }
        //戦闘不能ならメッセージ
        List<BattleUnit> units = new List<BattleUnit>();
        units.AddRange(attackers);
        units.Sort((a, b) =>
        {
            int result = a.Battler.Base.CharaTypes.EnemyNumber - b.Battler.Base.CharaTypes.EnemyNumber;
            return result != 0 ? result : a.Battler.BattlerIndex - b.Battler.BattlerIndex;
        });
        for (int i = 0; i < units.Count; i++)
        {
            Debug.Log("2回目ソート後");
            Debug.Log(units[i].Battler.Name);

        }

        if ((Move.Base.Category1 == MoveCategory1.All && attackers[sourceIndex].Battler.Base.CharaTypes.Chara == CharaType.Player && playerActionIndex == 1) ||
            (Move.Base.Category1 == MoveCategory1.All && attackers[sourceIndex].Battler.Base.CharaTypes.Chara == CharaType.Amina && aminaActionIndex == 1))
        {
            int enemyKill = enemyUnitManager.EnemyUnits.Count;
            for (int j = 0; j < units.Count; j++)
            {
                if (units[j].Battler.Base.CharaTypes.Chara == CharaType.Enemy && units[j].Battler.isDai == true)
                {
                    Debug.Log(enemyKill);
                    enemyKill--;
                }
            }
            if (enemyKill <= 0)
            {
                state = PatternState.BattleOver;
                units[sourceIndex].Battler.OnBattleOver();
                units[partnerIndex].Battler.OnBattleOver();
            }

        }
        else if(enemyUnitManager.EnemyUnits[enemySelectIndex].Battler.HP<=0&&enemyUnitManager.EnemyUnits[enemySelectIndex].Battler.isDai==true)
        {
            yield return enemyUnitManager.EnemyUnits[enemySelectIndex].FadeBattleOver();
            yield return DialogManager.Instance.TypeDialog($"{enemyUnitManager.EnemyUnits[enemySelectIndex].Battler.Name}を倒した！");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            int enemyKill = enemyUnitManager.EnemyUnits.Count;
            for (int j = 0; j < units.Count; j++)
            {
                if (units[j].Battler.Base.CharaTypes.Chara == CharaType.Enemy)
                {
                    Debug.Log(units[j].Battler.Base.Name + units[j].Battler.isDai);
                    Debug.Log(enemyKill);
                    enemyKill--;
                }
            }
            if (enemyKill <= 0)
            {
                state = PatternState.BattleOver;
                units[sourceIndex].Battler.OnBattleOver();
                units[partnerIndex].Battler.OnBattleOver();
            }
        }


        if (state == PatternState.BattleOver)
        {

            if (state == PatternState.BattleOver)
            {
                if (isPartner && !isDai)
                {
                    //倒した敵からその敵に設定しておいた経験値を得る
                    for (int j = 0; j < enemyUnitManager.EnemyUnits.Count; j++)
                    {
                        sumExp += enemyUnitManager.EnemyUnits[j].Battler.Base.Exp;
                        sumGold += enemyUnitManager.EnemyUnits[j].Battler.Base.Gold;
                    }
                    units[sourceIndex].Battler.HasExp += sumExp / 2;
                    units[sourceIndex].Battler.HasExp += sumExp / 2;
                    yield return DialogManager.Instance.TypeDialog($"{units[sourceIndex].Battler.Name}は{sumExp / 2}の経験値を獲得した");
                    //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
                    yield return OnLevelUp(units[sourceIndex]);
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    yield return DialogManager.Instance.TypeDialog($"{units[partnerIndex].Battler.Name}は{sumExp / 2}の経験値を獲得した");
                    //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
                    yield return OnLevelUp(units[partnerIndex]);
                    //ゴールド獲得
                    units[sourceIndex].Battler.Gold += sumGold;
                    yield return DialogManager.Instance.TypeDialog($"{sumGold}G手に入れた");
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    //ドロップアイテム取得
                    yield return DropItemCheck();
                    BattleOver(true);
                }
                else
                {
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    for (int j = 0; j < enemyUnitManager.EnemyUnits.Count; j++)
                    {
                        sumExp += enemyUnitManager.EnemyUnits[j].Battler.Base.Exp;
                        sumGold += enemyUnitManager.EnemyUnits[j].Battler.Base.Gold;
                    }
                    units[sourceIndex].Battler.HasExp += sumExp;
                    yield return DialogManager.Instance.TypeDialog($"{units[sourceIndex].Battler.Name}は{sumExp}の経験値を獲得した");
                    //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    yield return OnLevelUp(units[sourceIndex]);
                    //ゴールド獲得
                    units[sourceIndex].Battler.Gold += sumGold;
                    yield return DialogManager.Instance.TypeDialog($"{sumGold}G手に入れた");
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    //ドロップアイテム取得
                    yield return DropItemCheck();
                    BattleOver(true);
                }
                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].Battler.Base.CharaTypes.Chara == CharaType.Enemy)
                    {
                        units[i].ResetAnimation();
                    }
                }
            }

        }


    }

}
