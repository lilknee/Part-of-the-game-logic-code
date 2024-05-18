using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class 怪物血条 : MonoBehaviour
{
    public MouseClickHandler mouseClickHandler;
    public ShellConsole shellConsole;
    public Transform playerTransform; // 玩家Transform
    public GameObject uiimage1;
    public Text healthText;
    public Text monsterdefense;
    public Text monsterattack;
    public Text playerdefense;
    public Text playerattack;
    public Button attackButton; // 公共按钮变量
    public Image healthBarImage;
    public PlayerAttributesclass playerAttributesScript; // 玩家属性脚本
    public MonsterData MonsterData;//怪物属性脚本
    public ButtonImageTransfer buttonImageTransfer;//技能脚本
    public AudioSource deathSound;
    public int levelMultiplier = 1;


    private Vector3Int playerTilePosition; // 玩家所在位置的瓦片坐标

    private void Start()
    {
        if (mouseClickHandler != null)
        {
            // 访问 monsterDataDict 字典
            Dictionary<Vector3Int, MonsterData> dict = mouseClickHandler.monsterDataDict;
        }
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 将场景索引值赋给怪物属性的倍数
        levelMultiplier = sceneIndex;

        // 添加攻击按钮的点击事件
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(AttackPlayer);
        }
    }
    public void plus()
    {
        levelMultiplier += 1;
    }

    private void Update()
    {
        if (playerTransform == null || mouseClickHandler == null)
            return;

        Vector3 modifiedPlayerPosition = playerTransform.position;
        modifiedPlayerPosition.y -= 0.5f;

        // 获取玩家所在位置的瓦片坐标
        playerTilePosition = mouseClickHandler.tilemap.WorldToCell(modifiedPlayerPosition);
        TileBase tile = mouseClickHandler.tilemap.GetTile(playerTilePosition);
        Debug.Log(tile.name);
        
        // 检查是否有怪物数据记录
        if (mouseClickHandler.monsterDataDict.ContainsKey(playerTilePosition))
        {
            // 获取怪物数据
            MonsterData monsterData = mouseClickHandler.monsterDataDict[playerTilePosition];
            // 更新 UI 文本
            healthText.text = monsterData.health.ToString();
            monsterdefense.text=monsterData.defense.ToString();
            monsterattack.text=monsterData.attackDamage.ToString();
            playerattack.text=playerAttributesScript.attackDamage.ToString();
            playerdefense.text=playerAttributesScript.defense.ToString();
            float fillAmount = (float)monsterData.health / monsterData.MAX_HEALTH;
            

            // 设置血条填充比例
            healthBarImage.fillAmount = fillAmount;
        }
        else
        {

            // 如果没有记录，则创建怪物数据
            CreateMonsterData(playerTilePosition,tile.name);
        }

        // 获取瓦片
        
        
        if (tile != null)
        {
            Image imageComponent = uiimage1.GetComponent<Image>();

            // 获取瓦片的 Sprite
            Sprite tileSprite = mouseClickHandler.tilemap.GetSprite(playerTilePosition);

            // 设置 Image 组件的 Sprite 属性为瓦片的 Sprite
            imageComponent.sprite = tileSprite;
            
        }
        

    }

    
    public void CreateMonsterData(Vector3Int position, string tileName)
    {
        // 创建 MonsterData 实例并设置属性值
        MonsterData monsterData = gameObject.AddComponent<MonsterData>();
        SetMonsterAttributesByTileName(monsterData, tileName);

        // 将怪物数据添加到字典中
        mouseClickHandler.monsterDataDict.Add(position, monsterData);

        // 更新 UI 文本
        healthText.text = monsterData.health.ToString();
        monsterdefense.text = monsterData.defense.ToString();
        monsterattack.text = monsterData.attackDamage.ToString();
    }

    private void SetMonsterAttributesByTileName(MonsterData monsterData, string tileName)
    {
        switch (tileName)
        {
            case "monster2":
                // 设置瓦片1对应的属性值
                monsterData.health = 5 * levelMultiplier;
                monsterData.attackDamage = 1 * levelMultiplier;
                monsterData.defense = 1 * levelMultiplier;
                monsterData.MAX_HEALTH = monsterData.health;
                break;
            case "启动弹窗":
                // 设置瓦片2对应的属性值
                monsterData.health = 3 * levelMultiplier;
                monsterData.attackDamage = 1 * levelMultiplier;
                monsterData.defense = 2 * levelMultiplier;
                monsterData.MAX_HEALTH = monsterData.health;
                break;
            case "系统垃圾文件":
                // 设置瓦片3对应的属性值
                monsterData.health = 8 * levelMultiplier;
                monsterData.attackDamage = 1 * levelMultiplier;
                monsterData.defense = 1 * levelMultiplier;
                monsterData.MAX_HEALTH = monsterData.health;
                break;
            case "恶意软件的启动扫描":
                // 设置瓦片4对应的属性值
                monsterData.health = 5 * levelMultiplier;
                monsterData.attackDamage = 2 * levelMultiplier;
                monsterData.defense = 0 * levelMultiplier;
                monsterData.MAX_HEALTH = monsterData.health;
                break;
            case "zanbom1":
                // 设置瓦片5对应的属性值
                monsterData.health = 30 * levelMultiplier;
                monsterData.attackDamage = 8 * levelMultiplier;
                monsterData.defense = 8 * levelMultiplier;
                monsterData.MAX_HEALTH = monsterData.health;
                break;
            case "chong":
                // 设置瓦片6对应的属性值
                monsterData.health = 10 * levelMultiplier;
                monsterData.attackDamage = 3 * levelMultiplier;
                monsterData.defense = 3 * levelMultiplier;
                monsterData.MAX_HEALTH = monsterData.health;
                break;
            case "zhangai":
                // 设置瓦片7对应的属性值
                monsterData.health = 5 * levelMultiplier;
                monsterData.attackDamage = 0 * levelMultiplier;
                monsterData.defense = 5 * levelMultiplier;
                monsterData.MAX_HEALTH = monsterData.health;
                break;
            case "feiyinweishi":
                // 设置瓦片8对应的属性值
                monsterData.health = 10 * levelMultiplier;
                monsterData.attackDamage = 5 * levelMultiplier;
                monsterData.defense = 5 * levelMultiplier;
                monsterData.MAX_HEALTH = monsterData.health;
                break;
            case "gulou2":
                // 设置瓦片9对应的属性值
                monsterData.health = 7 * levelMultiplier;
                monsterData.attackDamage = 2 * levelMultiplier;
                monsterData.defense = 3 * levelMultiplier;
                monsterData.MAX_HEALTH = monsterData.health;
                break;
            case "chuxian3":
                // 设置瓦片10对应的属性值
                monsterData.health = 10 * levelMultiplier;
                monsterData.attackDamage = 7 * levelMultiplier;
                monsterData.defense = 7 * levelMultiplier;
                monsterData.MAX_HEALTH = monsterData.health;
                break;
            case "zanbom3":
                // 设置瓦片5对应的属性值
                monsterData.health = 15 * levelMultiplier;
                monsterData.attackDamage = 5 * levelMultiplier;
                monsterData.defense = 5 * levelMultiplier;
                monsterData.MAX_HEALTH = monsterData.health;
                break;
            // 添加更多情况...



            // 添加更多瓦片名字和对应的属性值
            default:
                // 对于未知瓦片名字的处理，可以设置默认的属性值
                monsterData.health = 0;
                monsterData.attackDamage = 0;
                monsterData.defense = 0;
                monsterData.MAX_HEALTH = monsterData.health;
                break;
        }
    }







    public void AttackPlayer()
    {
        // 播放攻击音效

        // 获取玩家的属性
        PlayerAttributesclass playerAttributes = playerTransform.GetComponent<PlayerAttributesclass>();
        if (playerAttributes != null && mouseClickHandler.monsterDataDict.ContainsKey(playerTilePosition))
        {
            // 获取玩家当前血量和防御力
            int playerHealth = playerAttributes.currentHealth;
            int playerDefense = playerAttributes.defense;

            // 获取玩家当前位置对应的怪物数据
            MonsterData monsterData = mouseClickHandler.monsterDataDict[playerTilePosition];


            // 计算玩家受到的伤害，减去防御力
            int damageToPlayer = Mathf.Max(monsterData.attackDamage - playerDefense, 0);
            shellConsole.DisplayOutput("玩家受到伤害"+ damageToPlayer);

            // 怪物受到攻击
            int damageToMonster = Mathf.Max(playerAttributes.attackDamage - monsterData.defense, 0);
            shellConsole.DisplayOutput("011001受到伤害" + damageToMonster);


            //技能效果
            foreach (Image targetimage in buttonImageTransfer.targetImages)
            {
                // 遍历技能列表，检查每个技能的名字
                switch (targetimage.sprite.name)
                {
                    case "分析":
                        // 执行火球技能逻辑
                        // ...
                        if(damageToPlayer>0)
                        {
                            playerAttributes.maxHealth += 1;
                        }
                        break;
                    case "生命增长":
                        // 执行治疗技能逻辑
                        // ...
                        Debug.Log("生命之中");
                        break;
                    case "digui":
                        // 执行火球技能逻辑
                        // ...
                        if (Random.Range(0f, 1f) < 0.5f) // 假设几率为50%
                        {
                            // 在增加生命值前检查生命值是否小于最大生命值
                            if (playerAttributes.currentHealth < playerAttributes.maxHealth&& damageToMonster>0)
                            {
                                playerAttributes.currentHealth += 1;
                                shellConsole.DisplayOutput("触发 算法治疗 生命值+1");
                            }

                            // 更新 UI 文本显示
                            healthText.text = playerAttributes.currentHealth.ToString();
                        }
                        break;
                    case "yachi":
                        // 执行火球技能逻辑
                        // ...
                        if (Random.Range(0f, 1f) < 0.05f &&monsterData.health<= damageToMonster) // 假设几率为50%
                        {
                            playerAttributes.maxHealth += monsterData.MAX_HEALTH;
                            playerAttributes.attackDamage += monsterData.attackDamage;
                            playerAttributes.defense += monsterData.defense;
                            shellConsole.DisplayOutput("触发 数据掠夺 获得怪物属性值"+monsterData);
                        }
                        break;
                    case "lrwall3":
                        if (Random.Range(0f, 1f) < 0.5f && damageToPlayer>0)
                        {
                            playerAttributes.maxHealth += 1;
                            shellConsole.DisplayOutput("触发 位移补偿 最大生命值+1");
                        }
                        break;
                    case "fanshe":
                        if (damageToPlayer > 0)
                        {
                            damageToMonster+=playerAttributes.defense*10/100;
                            shellConsole.DisplayOutput("触发 反射 反弹攻击"+ playerAttributes.defense * 10 / 100);
                        }
                        break;
                    // 添加更多技能的 case 语句...
                    default:
                        // 对于未知技能名字的处理
                        break;

                }
            }





            
            monsterData.health -= damageToMonster;

            // 更新 UI 文本
            healthText.text = monsterData.health.ToString();
            float fillAmount = (float)monsterData.health / monsterData.MAX_HEALTH;

            // 设置血条填充比例
            healthBarImage.fillAmount = fillAmount;
            playerAttributes.currentHealth -= damageToPlayer;

            // 如果怪物的血量小于或等于 0，删除怪物数据并将瓦片设置为 null
            if (monsterData.health <= 0)
            {
                // 从怪物数据字典中删除对应位置的怪物数据
                mouseClickHandler.monsterDataDict.Remove(playerTilePosition);
                

                TileBase tile = mouseClickHandler.tilemap.GetTile(playerTilePosition);
                if (tile.name == "zanbom1")
                {
                    // 在同一位置生成另一个瓦片
                    TileBase chuansonTile = Resources.Load<TileBase>("chuanson1");
                    Debug.Log(chuansonTile.name);
                    mouseClickHandler.tilemap.SetTile(playerTilePosition, chuansonTile);
                    shellConsole.DisplayOutput("发现陌生程序接口");
                }

                // 将瓦片设置为 null
                else
                {
                    mouseClickHandler.tilemap.SetTile(playerTilePosition, null);
                }


                // 重置血条填充比例为满
                healthBarImage.fillAmount = 1f;

                // 消灭怪物时给玩家增加经验值
                playerAttributesScript.currentExperience += 10;
                playerAttributesScript.actionPoints += 1;

                // 如果当前经验值达到满值，升级并重置经验值
                if (playerAttributesScript.currentExperience >= playerAttributesScript.maxExperience)
                {
                    playerAttributesScript.level++;
                    playerAttributesScript.currentExperience = 0;
                    playerAttributesScript.maxExperience += 10;
                    playerAttributesScript.maxHealth += playerAttributesScript.maxHealthp;
                    playerAttributesScript.attackDamage += playerAttributesScript.attackDamagep;
                    shellConsole.DisplayOutput("玩家升级");

                }
            }
        }
    }

    
}
