using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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
    }

    enum AminaActionPattern
    {
        None,
        AminaAttack,
        AminaItem,
    }


    PatternState state;
    PlayerActionPattern playerActionPattern;
    AminaActionPattern aminaActionPattern;

    [SerializeField] ActionSelectionUI actionSelectionUI;
    [SerializeField] MoveSelectionUI playerMoveSelectionUI;
    [SerializeField] MoveSelectionUI aminaMoveSelectionUI;
    [SerializeField] InventoryUI inventoryUI;

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit aminaUnit;
    [SerializeField] BattleUnit enemyUnit;

    public UnityAction<bool> OnBattleOver;
    public UnityAction<Item> OnDropItem;
    bool isBossBattle;
    bool playerLive;
    bool aminaLive;
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

    public void BattleStart(PlayerController player, Battler enemy, bool isBossBattle = false)
    {
        battleOverMenber = 0;
        playerLive = true;
        aminaLive = true;
        this.isBossBattle = isBossBattle;
        state = PatternState.Start;
        gameObject.SetActive(true);
        actionSelectionUI.Init();
        playerMoveSelectionUI.SetText(player.Battlers[0].Moves);
        if (player.Battlers.Count == 2)
        {
            aminaMoveSelectionUI.SetText(player.Battlers[1].Moves);
        }
        PlayerActionSelection();
        StartCoroutine(SetupBattle(player, enemy));
    }


    IEnumerator SetupBattle(PlayerController player, Battler enemy)
    {
        playerActionPattern = PlayerActionPattern.None;
        aminaActionPattern = AminaActionPattern.None;
        playerUnit.Setup(player.Battlers[0]);
        if (player.Battlers.Count == 2)
        {
            aminaUnit.gameObject.SetActive(true);
            aminaUnit.Setup(player.Battlers[1]);
        }
        enemyUnit.Setup(enemy);
        state = PatternState.Busy;
        yield return DialogManager.Instance.TypeDialog($"{enemy.Base.Name}が現れた！");
        if (playerUnit.Battler.HP <= 0 && playerLive == true)
        {
            playerLive = false;
            battleOverMenber++;
            if (player.Battlers.Count == 2)
            {
                if (aminaUnit.Battler.HP <= 0 && aminaLive == true)
                {
                    aminaLive = false;
                    battleOverMenber++;
                    yield return DialogManager.Instance.TypeDialog($"戦えるメンバーがいない・・・\n今は戦闘を避けよう");
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    yield return null;
                    state = PatternState.Escape;
                }
                else
                {
                    playerActionPattern = PlayerActionPattern.None;
                    StartCoroutine(DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は何の反応もない・・・\nアミナはどうする？"));
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
        OnBattleOver?.Invoke(win);
    }

    void PlayerActionSelection()
    {
        state = PatternState.PlayerActionSelection;
        actionSelectionUI.Open();
    }
    void AminaActionSelection()
    {
        state = PatternState.AminaActionSelection;
        playerMoveSelectionUI.Close();
        actionSelectionUI.Open();
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

    void HandlePlayerActionSelection()
    {
        if (playerUnit.Battler.HP <= 0 && playerLive == true)
        {
            playerLive = false;
            battleOverMenber++;
            if (aminaUnit.gameObject.activeSelf == true)
            {
                if (aminaUnit.Battler.HP <= 0 && aminaLive == true)
                {
                    aminaLive = false;
                    battleOverMenber++;
                    StartCoroutine(DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は何の反応もない・・・"));
                    state = PatternState.Escape;
                }
                else
                {
                    playerActionPattern = PlayerActionPattern.None;
                    StartCoroutine(DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は何の反応もない・・・\nアミナはどうする？"));
                    AminaActionSelection();


                }
            }
        }
        else if (playerLive == false)
        {
            if (aminaUnit.gameObject.activeSelf == true)
            {
                if (aminaLive == false)
                {

                    StartCoroutine(DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は何の反応もない・・・"));
                    state = PatternState.Escape;
                }
                else
                {
                    StartCoroutine(DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は何の反応もない・・・\nアミナはどうする？"));
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

                    if (aminaUnit.gameObject.activeSelf == true)
                    {
                        if (aminaUnit.Battler.HP <= 0)
                        {
                            playerActionPattern = PlayerActionPattern.PlayerAttack;
                            battleOverMenber++;
                            StartCoroutine(DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は何の反応もない・・・"));
                            aminaActionPattern = AminaActionPattern.None;
                            StartCoroutine(RunTurns());
                        }
                        else
                        {
                            playerActionPattern = PlayerActionPattern.PlayerAttack;
                            AminaActionSelection();
                            StartCoroutine(DialogManager.Instance.TypeDialog("アミナはどうする？"));
                        }
                    }
                    else
                    {
                        playerActionPattern = PlayerActionPattern.PlayerAttack;
                        actionSelectionUI.Close();
                        StartCoroutine(RunTurns());
                    }
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
                StartCoroutine(RunTurns());
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
            if (playerLive == true)
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
                if (GameController.Instance.GetBattlerMember() == 2)
                {
                    playerMoveSelectionUI.Close();
                    if (aminaLive == true)
                    {
                        playerActionPattern = PlayerActionPattern.PlayerAttack;
                        AminaActionSelection();
                    }
                    else
                    {
                        playerActionPattern = PlayerActionPattern.PlayerAttack;
                        // 技リストから技を出す
                        StartCoroutine(RunTurns());
                    }
                }
                else
                {
                    playerActionPattern = PlayerActionPattern.PlayerAttack;
                    playerMoveSelectionUI.Close();
                    // 技リストから技を出す
                    StartCoroutine(RunTurns());
                }
            }
            else
            {
                StartCoroutine(DialogManager.Instance.TypeDialog($"MPが足りない"));
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
                aminaMoveSelectionUI.Close();
                if (GameController.Instance.GetBattlerMember() == 2)
                {
                    if (playerLive == true)
                    {
                        aminaActionPattern = AminaActionPattern.AminaAttack;
                        StartCoroutine(RunTurns());
                    }
                    else
                    {
                        aminaActionPattern = AminaActionPattern.AminaAttack;
                        // 技リストから技を出す
                        StartCoroutine(RunTurns());
                    }
                }
                else
                {
                    aminaActionPattern = AminaActionPattern.AminaAttack;
                    // 技リストから技を出す
                    StartCoroutine(RunTurns());
                }
            }
            else
            {
                StartCoroutine(DialogManager.Instance.TypeDialog($"MPが足りない"));
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            aminaMoveSelectionUI.Close();
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
                StartCoroutine(RunTurns());
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
                if (playerLive == false)
                {
                    aminaActionPattern = AminaActionPattern.AminaItem;
                    StartCoroutine(RunTurns());
                }
                else
                {
                    aminaActionPattern = AminaActionPattern.AminaItem;
                    StartCoroutine(RunTurns());
                }
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

    IEnumerator RunTurns()
    {
        int mod = Random.Range(0, 2);
        bool isWin = false;
        if (aminaActionPattern == AminaActionPattern.None && playerActionPattern == PlayerActionPattern.PlayerAttack)
        {
            state = PatternState.RunTurns;
            Move playerMove = playerUnit.Battler.Moves[playerMoveIndex];
            Debug.Log("プレイヤーターン");
            if (playerActionIndex == 0)
            {
                yield return RunNormalMove(playerUnit, enemyUnit);

            }
            else
            {
                yield return RunMove(playerUnit, enemyUnit, playerMove);
            }

            //戦闘不能ならメッセージ
            if (enemyUnit.Battler.HP <= 0)
            {
                state = PatternState.BattleOver;
                playerUnit.Battler.OnBattleOver();
            }

            if (state == PatternState.BattleOver)
            {
                yield return DialogManager.Instance.TypeDialog($"{enemyUnit.Battler.Base.Name}を潰した");
                enemyUnit.PlayerFaintAnimaion();
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                //倒した敵からその敵に設定しておいた経験値を得る
                playerUnit.Battler.HasExp += (enemyUnit.Battler.Level * enemyUnit.Battler.Base.Exp) / 7;
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は{enemyUnit.Battler.Base.Exp}リットルの酒を飲み切った！");
                //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
                yield return OnLevelUp(playerUnit);
                //ドロップアイテム取得
                yield return DropItemCheck();
                BattleOver(isWin);
                enemyUnit.PlayerResetAnimation();
                yield break;
            }
            if (mod == 0)
            {
                Move enemyMove = enemyUnit.Battler.GetRandomMove();
                Debug.Log("モンスターターン");
                yield return RunMove(enemyUnit, playerUnit, enemyMove);
            }
            else if (mod == 1)
            {
                Debug.Log("モンスターターン");
                yield return RunNormalMove(enemyUnit, playerUnit);
            }
            if (battleOverMenber == GameController.Instance.GetBattlerMember())
            {
                yield return DialogManager.Instance.TypeDialog("全員潰れて行方が分からなくなった・・・");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                BattleOver(!isWin);
                yield break;
            }

        }
        else if (aminaActionPattern == AminaActionPattern.AminaAttack && playerActionPattern == PlayerActionPattern.None)
        {
            state = PatternState.RunTurns;
            Move aminaMove = aminaUnit.Battler.Moves[aminaMoveIndex];
            Debug.Log("プレイヤーターン");
            if (playerActionIndex == 0)
            {
                yield return RunNormalMove(aminaUnit, enemyUnit);

            }
            else
            {
                yield return RunMove(aminaUnit, enemyUnit, aminaMove);
            }


            //戦闘不能ならメッセージ
            if (enemyUnit.Battler.HP <= 0)
            {
                state = PatternState.BattleOver;
                aminaUnit.Battler.OnBattleOver();
            }

            if (state == PatternState.BattleOver)
            {
                yield return DialogManager.Instance.TypeDialog($"{enemyUnit.Battler.Base.Name}を潰した");
                enemyUnit.PlayerFaintAnimaion();
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                //倒した敵からその敵に設定しておいた経験値を得る
                aminaUnit.Battler.HasExp += (enemyUnit.Battler.Level * enemyUnit.Battler.Base.Exp) / 7;
                yield return DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は{enemyUnit.Battler.Base.Exp}リットルの酒を飲み切った！");
                //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
                yield return OnLevelUp(aminaUnit);
                //ドロップアイテム取得
                yield return DropItemCheck();
                BattleOver(isWin);
                enemyUnit.PlayerResetAnimation();
                yield break;
            }
            if (mod == 0)
            {
                Move enemyMove = enemyUnit.Battler.GetRandomMove();
                Debug.Log("モンスターターン");
                yield return RunMove(enemyUnit, aminaUnit, enemyMove);
            }
            else if (mod == 1)
            {
                Debug.Log("モンスターターン");
                yield return RunNormalMove(enemyUnit, aminaUnit);
            }
            if (battleOverMenber == GameController.Instance.GetBattlerMember())
            {
                yield return DialogManager.Instance.TypeDialog("全員潰れて行方が分からなくなった・・・");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                BattleOver(!isWin);
                yield break;
            }

        }
        else if (playerActionPattern == PlayerActionPattern.PlayerItem && aminaActionPattern == AminaActionPattern.AminaAttack)
        {
            Debug.Log("PlayerItemAminaAttack");
            state = PatternState.RunTurns;
            Move aminaMove = aminaUnit.Battler.Moves[aminaMoveIndex];
            Debug.Log("プレイヤーターン");
            yield return inventoryUI.UseInBattle(itemSelectPlayer, playerItemSelectIndex, playerUseCharaSelect);
            playerUnit.Setup(playerUnit.Battler);
            aminaUnit.Setup(aminaUnit.Battler);
            Debug.Log("アミナターン");
            if (aminaActionIndex == 0)
            {
                yield return RunNormalMove(aminaUnit, enemyUnit);

            }
            else
            {
                yield return RunMove(aminaUnit, enemyUnit, aminaMove);
            }

            //戦闘不能ならメッセージ
            if (enemyUnit.Battler.HP <= 0)
            {
                state = PatternState.BattleOver;
                playerUnit.Battler.OnBattleOver();
            }

            if (state == PatternState.BattleOver)
            {
                yield return DialogManager.Instance.TypeDialog($"{enemyUnit.Battler.Base.Name}を潰した");
                enemyUnit.PlayerFaintAnimaion();
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                //倒した敵からその敵に設定しておいた経験値を得る
                playerUnit.Battler.HasExp += (enemyUnit.Battler.Base.Exp) / 2;
                aminaUnit.Battler.HasExp += (enemyUnit.Battler.Base.Exp) / 2;
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は{enemyUnit.Battler.Base.Exp / 2}リットルの酒を飲み切った！");
                //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
                yield return OnLevelUp(playerUnit);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                yield return DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は{enemyUnit.Battler.Base.Exp / 2}リットルの酒を飲み切った！");
                //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
                yield return OnLevelUp(aminaUnit);
                //ドロップアイテム取得
                yield return DropItemCheck();
                BattleOver(isWin);
                enemyUnit.PlayerResetAnimation();
                yield break;
            }
            if (mod == 0)
            {
                int modTarget = Random.Range(0, 1);
                Move enemyMove = enemyUnit.Battler.GetRandomMove();
                Debug.Log("モンスターターン");
                if (modTarget == 0)
                {
                    yield return RunMove(enemyUnit, playerUnit, enemyMove);
                }
                else if (modTarget == 1)
                {
                    yield return RunMove(enemyUnit, aminaUnit, enemyMove);
                }
            }
            if (playerUnit.Battler.HP <= 0 && playerLive == true)
            {
                playerLive = false;
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は潰れて意識を失った...");
            }
            if (aminaUnit.Battler.HP <= 0 && aminaLive == true)
            {
                aminaLive = false;
                yield return DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は潰れて意識を失った...");
            }

            if (mod == 1)
            {
                int modTarget = Random.Range(0, 1);
                Debug.Log("モンスターターン");
                if (modTarget == 0)
                {
                    yield return RunNormalMove(enemyUnit, playerUnit);
                }
                else if (modTarget == 1)
                {
                    yield return RunNormalMove(enemyUnit, aminaUnit);
                }
            }
            if (playerUnit.Battler.HP <= 0 && playerLive == true)
            {
                playerLive = false;
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は潰れて意識を失った...");
            }
            if (aminaUnit.Battler.HP <= 0 && aminaLive == true)
            {
                aminaLive = false;
                yield return DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は潰れて意識を失った...");
            }

            if (battleOverMenber == GameController.Instance.GetBattlerMember())
            {
                yield return DialogManager.Instance.TypeDialog($"全員潰れて行方が分からなくなった・・・");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                BattleOver(!isWin);
                yield break;
            }

        }
        else if (playerActionPattern == PlayerActionPattern.PlayerAttack && aminaActionPattern == AminaActionPattern.AminaItem)
        {
            Debug.Log("PlayerAttackAminaItem");
            state = PatternState.RunTurns;
            Move playerMove = playerUnit.Battler.Moves[playerMoveIndex];
            Move aminaMove = aminaUnit.Battler.Moves[aminaMoveIndex];
            Debug.Log("プレイヤーターン");
            if (playerActionIndex == 0)
            {
                yield return RunNormalMove(playerUnit, enemyUnit);

            }
            else
            {
                yield return RunMove(playerUnit, enemyUnit, playerMove);
            }

            //戦闘不能ならメッセージ
            if (enemyUnit.Battler.HP <= 0)
            {
                state = PatternState.BattleOver;
                playerUnit.Battler.OnBattleOver();
            }

            if (state == PatternState.BattleOver)
            {
                yield return DialogManager.Instance.TypeDialog($"{enemyUnit.Battler.Base.Name}を潰した");
                enemyUnit.PlayerFaintAnimaion();
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                //倒した敵からその敵に設定しておいた経験値を得る
                playerUnit.Battler.HasExp += (enemyUnit.Battler.Base.Exp) / 2;
                aminaUnit.Battler.HasExp += (enemyUnit.Battler.Base.Exp) / 2;
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は{enemyUnit.Battler.Base.Exp / 2}リットルの酒を飲み切った！");
                //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
                yield return OnLevelUp(playerUnit);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                yield return DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は{enemyUnit.Battler.Base.Exp / 2}リットルの酒を飲み切った！");
                //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
                yield return OnLevelUp(aminaUnit);
                //ドロップアイテム取得
                yield return DropItemCheck();
                BattleOver(isWin);
                enemyUnit.PlayerResetAnimation();
                yield break;
            }
            Debug.Log("アミナアイテムターン");
            yield return inventoryUI.UseInBattle(itemSelectAmina, aminaItemSelectIndex, aminaUseCharaSelect);
            playerUnit.Setup(playerUnit.Battler);
            aminaUnit.Setup(aminaUnit.Battler);
            if (mod == 0)
            {
                int modTarget = Random.Range(0, 1);
                Move enemyMove = enemyUnit.Battler.GetRandomMove();
                Debug.Log("モンスターターン");
                if (modTarget == 0)
                {
                    yield return RunMove(enemyUnit, playerUnit, enemyMove);
                }
                else if (modTarget == 1)
                {
                    yield return RunMove(enemyUnit, aminaUnit, enemyMove);
                }
            }
            if (playerUnit.Battler.HP <= 0 && playerLive == true)
            {
                playerLive = false;
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は潰れて意識を失った...");
            }
            if (aminaUnit.Battler.HP <= 0 && aminaLive == true)
            {
                aminaLive = false;
                yield return DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は潰れて意識を失った...");
            }

            if (mod == 1)
            {
                int modTarget = Random.Range(0, 1);
                Debug.Log("モンスターターン");
                if (modTarget == 0)
                {
                    yield return RunNormalMove(enemyUnit, playerUnit);
                }
                else if (modTarget == 1)
                {
                    yield return RunNormalMove(enemyUnit, aminaUnit);
                }
            }
            if (playerUnit.Battler.HP <= 0 && playerLive == true)
            {
                playerLive = false;
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は潰れて意識を失った...");
            }
            if (aminaUnit.Battler.HP <= 0 && aminaLive == true)
            {
                aminaLive = false;
                yield return DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は潰れて意識を失った...");
            }

            if (battleOverMenber == GameController.Instance.GetBattlerMember())
            {
                yield return DialogManager.Instance.TypeDialog($"全員潰れて行方が分からなくなった・・・");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                BattleOver(!isWin);
                yield break;
            }

        }
        else if (playerActionPattern == PlayerActionPattern.PlayerItem && aminaActionPattern == AminaActionPattern.AminaItem)
        {
            Debug.Log("PlayerItemAminaItem");
            state = PatternState.RunTurns;
            Debug.Log("プレイヤーターン");
            yield return inventoryUI.UseInBattle(itemSelectPlayer, playerItemSelectIndex, playerUseCharaSelect);
            playerUnit.Setup(playerUnit.Battler);
            aminaUnit.Setup(aminaUnit.Battler);
            Debug.Log("アミナアイテムターン");
            yield return inventoryUI.UseInBattle(itemSelectAmina, aminaItemSelectIndex, aminaUseCharaSelect);
            playerUnit.Setup(playerUnit.Battler);
            aminaUnit.Setup(aminaUnit.Battler);
            if (mod == 0)
            {
                int modTarget = Random.Range(0, 1);
                Move enemyMove = enemyUnit.Battler.GetRandomMove();
                Debug.Log("モンスターターン");
                if (modTarget == 0)
                {
                    yield return RunMove(enemyUnit, playerUnit, enemyMove);
                }
                else if (modTarget == 1)
                {
                    yield return RunMove(enemyUnit, aminaUnit, enemyMove);
                }
            }
            if (playerUnit.Battler.HP <= 0 && playerLive == true)
            {
                playerLive = false;
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は潰れて意識を失った...");
            }
            if (aminaUnit.Battler.HP <= 0 && aminaLive == true)
            {
                aminaLive = false;
                yield return DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は潰れて意識を失った...");
            }

            if (mod == 1)
            {
                int modTarget = Random.Range(0, 1);
                Debug.Log("モンスターターン");
                if (modTarget == 0)
                {
                    yield return RunNormalMove(enemyUnit, playerUnit);
                }
                else if (modTarget == 1)
                {
                    yield return RunNormalMove(enemyUnit, aminaUnit);
                }
            }
            if (playerUnit.Battler.HP <= 0 && playerLive == true)
            {
                playerLive = false;
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は潰れて意識を失った...");
            }
            if (aminaUnit.Battler.HP <= 0 && aminaLive == true)
            {
                aminaLive = false;
                yield return DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は潰れて意識を失った...");
            }

            if (battleOverMenber == GameController.Instance.GetBattlerMember())
            {
                yield return DialogManager.Instance.TypeDialog($"全員潰れて行方が分からなくなった・・・");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                BattleOver(!isWin);
                yield break;
            }

        }
        else if (aminaActionPattern == AminaActionPattern.None && playerActionPattern == PlayerActionPattern.PlayerItem)
        {
            Debug.Log("AminaNone");
            state = PatternState.RunTurns;
            actionSelectionUI.Close();
            inventoryUI.ItemSelectClose();
            yield return null;
            yield return inventoryUI.UseInBattle(itemSelectPlayer, playerItemSelectIndex, playerUseCharaSelect);
            playerUnit.Setup(playerUnit.Battler);
            if (GameController.Instance.GetBattlerMember() == 2)
            {
                aminaUnit.Setup(aminaUnit.Battler);
            }
            if (mod == 0)
            {
                Move enemyMove = enemyUnit.Battler.GetRandomMove();
                Debug.Log("モンスターターン");
                yield return RunMove(enemyUnit, playerUnit, enemyMove);
            }
            else if (mod == 1)
            {
                Debug.Log("モンスターターン");
                yield return RunNormalMove(enemyUnit, playerUnit);
            }
            if (playerLive == false)
            {
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は潰れて意識を失った...");


                BattleOver(!isWin);
                yield break;
            }
        }
        else if (aminaActionPattern == AminaActionPattern.AminaItem && playerActionPattern == PlayerActionPattern.None)
        {
            Debug.Log("PlayerNone");
            state = PatternState.RunTurns;
            actionSelectionUI.Close();
            inventoryUI.ItemSelectClose();
            yield return null;
            yield return inventoryUI.UseInBattle(itemSelectAmina, aminaItemSelectIndex, aminaUseCharaSelect);
            playerUnit.Setup(playerUnit.Battler);
            aminaUnit.Setup(aminaUnit.Battler);
            if (mod == 0)
            {
                Move enemyMove = enemyUnit.Battler.GetRandomMove();
                Debug.Log("モンスターターン");
                yield return RunMove(enemyUnit, playerUnit, enemyMove);
            }
            else if (mod == 1)
            {
                Debug.Log("モンスターターン");
                yield return RunNormalMove(enemyUnit, playerUnit);
            }
            if (aminaLive == false)
            {
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は潰れて意識を失った...");


                BattleOver(!isWin);
                yield break;
            }
        }
        else if (aminaActionPattern == AminaActionPattern.AminaAttack && playerActionPattern == PlayerActionPattern.PlayerAttack)
        {
            state = PatternState.RunTurns;
            Move playerMove = playerUnit.Battler.Moves[playerMoveIndex];
            Move aminaMove = aminaUnit.Battler.Moves[aminaMoveIndex];
            Debug.Log("プレイヤーターン");
            if (playerActionIndex == 0)
            {
                yield return RunNormalMove(playerUnit, enemyUnit);

            }
            else
            {
                yield return RunMove(playerUnit, enemyUnit, playerMove);
            }

            //戦闘不能ならメッセージ
            if (enemyUnit.Battler.HP <= 0)
            {
                state = PatternState.BattleOver;
                playerUnit.Battler.OnBattleOver();
            }

            if (state == PatternState.BattleOver)
            {
                yield return DialogManager.Instance.TypeDialog($"{enemyUnit.Battler.Base.Name}を潰した");
                enemyUnit.PlayerFaintAnimaion();
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                //倒した敵からその敵に設定しておいた経験値を得る
                playerUnit.Battler.HasExp += (enemyUnit.Battler.Base.Exp) / 2;
                aminaUnit.Battler.HasExp += (enemyUnit.Battler.Base.Exp) / 2;
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は{enemyUnit.Battler.Base.Exp / 2}リットルの酒を飲み切った！");
                //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
                yield return OnLevelUp(playerUnit);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                yield return DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は{enemyUnit.Battler.Base.Exp / 2}リットルの酒を飲み切った！");
                //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
                yield return OnLevelUp(aminaUnit);
                //ドロップアイテム取得
                yield return DropItemCheck();
                BattleOver(isWin);
                enemyUnit.PlayerResetAnimation();
                yield break;
            }
            Debug.Log("アミナターン");
            if (aminaActionIndex == 0)
            {
                yield return RunNormalMove(aminaUnit, enemyUnit);

            }
            else
            {
                yield return RunMove(aminaUnit, enemyUnit, aminaMove);
            }

            //戦闘不能ならメッセージ
            if (enemyUnit.Battler.HP <= 0)
            {
                state = PatternState.BattleOver;
                playerUnit.Battler.OnBattleOver();
            }

            if (state == PatternState.BattleOver)
            {
                yield return DialogManager.Instance.TypeDialog($"{enemyUnit.Battler.Base.Name}を潰した");
                enemyUnit.PlayerFaintAnimaion();
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                //倒した敵からその敵に設定しておいた経験値を得る
                playerUnit.Battler.HasExp += (enemyUnit.Battler.Base.Exp) / 2;
                aminaUnit.Battler.HasExp += (enemyUnit.Battler.Base.Exp) / 2;
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は{enemyUnit.Battler.Base.Exp / 2}リットルの酒を飲み切った！");
                //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
                yield return OnLevelUp(playerUnit);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                yield return DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は{enemyUnit.Battler.Base.Exp / 2}リットルの酒を飲み切った！");
                //所持経験値がレベルアップに必要な経験値量を超えているなら必要経験値量を下回るまでレベルアップし続ける
                yield return OnLevelUp(aminaUnit);
                //ドロップアイテム取得
                yield return DropItemCheck();
                BattleOver(isWin);
                enemyUnit.PlayerResetAnimation();
                yield break;
            }
            if (mod == 0)
            {
                int modTarget = Random.Range(0, 1);
                Move enemyMove = enemyUnit.Battler.GetRandomMove();
                Debug.Log("モンスターターン");
                if (modTarget == 0)
                {
                    yield return RunMove(enemyUnit, playerUnit, enemyMove);
                }
                else if (modTarget == 1)
                {
                    yield return RunMove(enemyUnit, aminaUnit, enemyMove);
                }
            }
            if (playerUnit.Battler.HP <= 0 && playerLive == true)
            {
                playerLive = false;
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は潰れて意識を失った...");
            }
            if (aminaUnit.Battler.HP <= 0 && aminaLive == true)
            {
                aminaLive = false;
                yield return DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は潰れて意識を失った...");
            }

            if (mod == 1)
            {
                int modTarget = Random.Range(0, 1);
                Debug.Log("モンスターターン");
                if (modTarget == 0)
                {
                    yield return RunNormalMove(enemyUnit, playerUnit);
                }
                else if (modTarget == 1)
                {
                    yield return RunNormalMove(enemyUnit, aminaUnit);
                }
            }
            if (playerUnit.Battler.HP <= 0 && playerLive == true)
            {
                playerLive = false;
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は潰れて意識を失った...");
            }
            if (aminaUnit.Battler.HP <= 0 && aminaLive == true)
            {
                aminaLive = false;
                yield return DialogManager.Instance.TypeDialog($"{aminaUnit.Battler.Base.Name}は潰れて意識を失った...");
            }

            if (battleOverMenber == GameController.Instance.GetBattlerMember())
            {
                yield return DialogManager.Instance.TypeDialog($"全員潰れて行方が分からなくなった・・・");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                BattleOver(!isWin);
                yield break;
            }

        }
        yield return DialogManager.Instance.TypeDialog("どうする？");
        PlayerActionSelection();
    }

    IEnumerator DropItemCheck()
    {
        if (Random.Range(0, 100) <= enemyUnit.Battler.Base.DropItem1.Chance)
        {
            yield return DialogManager.Instance.TypeDialog($"{enemyUnit.Battler.Base.Name}が宝箱を落とした");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            OnDropItem?.Invoke(enemyUnit.Battler.Base.DropItem1.DropItem);
            yield return DialogManager.Instance.TypeDialog($"{enemyUnit.Battler.Base.DropItem1.DropItem.Base.GetKanjiName()}を拾った！");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }
        if (Random.Range(0, 100) <= enemyUnit.Battler.Base.DropItem2.Chance)
        {
            yield return DialogManager.Instance.TypeDialog($"{enemyUnit.Battler.Base.Name}が宝箱を落とした");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            OnDropItem?.Invoke(enemyUnit.Battler.Base.DropItem2.DropItem);
            yield return DialogManager.Instance.TypeDialog($"{enemyUnit.Battler.Base.DropItem2.DropItem.Base.GetKanjiName()}を拾った！");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
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
        Debug.Log("実行");
        yield return DialogManager.Instance.TypeDialog($"{sourceUnit.Battler.Base.Name}は{move.Base.Name}をつかった", auto: false);
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
            DamageDetailes damageDetailes = targetUnit.Battler.TakeDamage(move, sourceUnit.Battler, targetUnit.Battler);
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

        sourceUnit.UpdateUI();
        targetUnit.UpdateUI();


        //戦闘不能ならメッセージ
        if (targetUnit.Battler.HP <= 0)
        {
            battleOverMenber++;
            sourceUnit.Battler.OnBattleOver();
        }
        yield return RunAfterTurn(sourceUnit);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
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
        yield return DialogManager.Instance.TypeDialog($"{sourceUnit.Battler.Base.Name}の通常攻撃！", auto: false);
        yield return new WaitForSeconds(0.2f);



        //Enemydamage計算
        DamageDetailes damageDetailes = targetUnit.Battler.NormalAttackTakeDamage(sourceUnit.Battler, targetUnit.Battler);
        string resultText = targetUnit.Battler.ResultText;
        yield return DialogManager.Instance.TypeDialog(resultText);

        //クリティカルのメッセージ
        yield return ShowDamageDetails(damageDetailes);

        sourceUnit.UpdateUI();
        targetUnit.UpdateUI();


        //戦闘不能ならメッセージ
        if (targetUnit.Battler.HP <= 0)
        {
            battleOverMenber++;
            sourceUnit.Battler.OnBattleOver();
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
            battleOverMenber++;
            yield return DialogManager.Instance.TypeDialog($"{sourceUnit.Battler.Base.Name}を潰した");
            sourceUnit.PlayerFaintAnimaion();
            yield return new WaitForSeconds(0.7f);
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
                yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}はレベル{playerUnit.Battler.Level}になった！");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                //特定のレベルなら技を覚える
                Move learnedMove = playerUnit.Battler.LearnedMove();
                if (learnedMove != null)
                {
                    yield return DialogManager.Instance.TypeDialog($"{playerUnit.Battler.Base.Name}は{learnedMove.Base.Name}を覚えた！");
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                }
            }
        }
    }



}
