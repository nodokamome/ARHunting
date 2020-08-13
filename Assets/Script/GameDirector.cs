using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    public Text TextYourName;
    public Text TextYourHP;
    public Text TextMonsterDiastance;
    public Text TextTrackCareful;
    public Slider SliderYourHP;

    public GameObject ARCamera;
    public GameObject Monster;

    [System.NonSerialized]
    public static bool isBattle;
    private float intervalTime;
    private float intervalTime2;

    private GameResultDialog gameResultDialog;
    private GameDataSharing script;
    private BulletGenerate bulletGenerate;
    private UserDataDB userDataDB = new UserDataDB();

    private int initialHP;
    public static int currentHP;

    //item
    public static int plusDefence;
    public Text itemEffectText;
    private float time = 0;
    private int second = 0;
    private bool isitem;
    private int skullDamage = 4;
    public Text AttackText;
    public Text ShieldText;

    public Sprite attackImage;
    public Sprite bulletImage;
    public Sprite HPImage;
    public Sprite shieldImage;
    public Sprite skullImage;
    public Sprite specialImage;
    public Sprite itemBoxImage;
    public Image itemBox;
    //
    Vector2 ARCameraTmp;
    private float time2 = 0;
    private int second2 = 0;
    public static bool isARBox;

    //color
    private Color attackColor = new Color(243f / 255f, 106f / 255f, 49f / 255f);
    private Color bulletColor = new Color(243f / 255f, 226f / 255f, 49f / 255f);
    private Color hpColor = new Color(74f / 255f, 243f / 255f, 49f / 255f);
    private Color shieldColor = new Color(49f / 255f, 201f / 255f, 243f / 255f);
    private Color skullColor = new Color(243f / 255f, 49f / 255f, 213f / 255f);
    private Color specialColor = new Color(49f / 255f, 243f / 255f, 243f / 255f);

    void Start()
    {

        intervalTime = 0;
        intervalTime2 = 0;
        isBattle = true;
        isitem = false;
        isARBox = false;
        GameObject resultDialogDirector = GameObject.Find("ResultDialog");
        gameResultDialog = resultDialogDirector.GetComponent<GameResultDialog>();

        GameObject gameDataSharing = GameObject.Find("GameDataSharing");
        script = gameDataSharing.GetComponent<GameDataSharing>();

        GameObject bulletGenerateDirector = GameObject.Find("BulletGenerate");
        bulletGenerate = bulletGenerateDirector.GetComponent<BulletGenerate>();
        //自分の名前
        TextYourName.text = userDataDB.GetUserData("name");

        //自分のHPの表示
        currentHP = int.Parse(userDataDB.GetUserData("currentHP"));
        initialHP = int.Parse(userDataDB.GetUserData("initialHP"));

        TextYourHP.text = currentHP + "/" + initialHP;

        SliderYourHP.value = currentHP;
        SliderYourHP.maxValue = initialHP;

        itemBox.sprite = itemBoxImage;

    }


    void Update()
    {
        if (BulletGenerate.weponPower * BulletGenerate.plusPower > 999)
        {
            AttackText.text = "攻撃：" + 999;
        }
        else
        {
            AttackText.text = "攻撃：" + BulletGenerate.weponPower * BulletGenerate.plusPower;
        }

        if (plusDefence > 999)
        {
            ShieldText.text = "防御：" + 999;
        }
        else
        {
            ShieldText.text = "防御：" + plusDefence;
        }

        Vector2 Apos = new Vector2(ARCamera.transform.position.x, ARCamera.transform.position.z);
        Vector2 Bpos = new Vector2(Monster.transform.position.x, Monster.transform.position.z);

        time2 += Time.deltaTime;
        second2 = (int)time2 % 60;
        if (second2 == 1)
        {
            if (Apos != ARCameraTmp)
            {
                isARBox = true;
                ARCameraTmp = Apos;
            }
            else
            {
                isARBox = false;
            }
            second2 = 0;
            time2 = 0;
        }

        if (isARBox)
        {
            //距離の表示
            float dis = Vector2.Distance(Apos, Bpos);
            dis = dis * 0.005f;
            TextMonsterDiastance.text = "モンスターとの距離\n" + dis.ToString("f2") + " m";
            TextTrackCareful.text = "";
            intervalTime2 = 0;

            intervalTime += Time.deltaTime;
            if (dis < 0.1)
            {
                if (intervalTime >= 0.5f)
                {
                    if (GameDataSharing.isopenSpecial)
                    {
                        if (currentHP > 0)
                        {
                            int receiveDamage = DLDirector.power - plusDefence;
                            if (receiveDamage < 0)
                            {
                                receiveDamage = 0;
                            }
                            currentHP = currentHP - receiveDamage;
                            if (isBattle && receiveDamage != 0)
                            {
                                UpdateHP();
                                script.receivedDamage();
                                if (SystemInfo.supportsVibration)
                                {
                                    Handheld.Vibrate();
                                }
                            }
                        }
                    }
                    intervalTime = 0;
                }
            }
        }
        else
        {
            //マーカーが画面に無い時の処理
            TextMonsterDiastance.text = "モンスターとの距離\n" + "???" + " m";
            TextTrackCareful.text = "*マーカーが見つかりません*\nダメージを受けます";
            intervalTime2 += Time.deltaTime;
            if (intervalTime2 >= 3.0f)
            {
                if (GameDataSharing.isopenSpecial)
                {
                    if (currentHP > 0)
                    {
                        currentHP = currentHP - 1;
                        if (isBattle)
                        {
                            UpdateHP();
                            script.receivedDamage();
                        }
                        if (SystemInfo.supportsVibration)
                        {
                            Handheld.Vibrate();
                        }
                    }
                }
                intervalTime2 = 0;
            }
        }

        //アイテム画像の表示
        if (GameDataSharing.haveItem != "")
        {
            switch (GameDataSharing.haveItem)
            {
                case "attack":
                    itemBox.sprite = attackImage;
                    break;
                case "bullet":
                    itemBox.sprite = bulletImage;
                    break;
                case "HP":
                    itemBox.sprite = HPImage;
                    break;
                case "shield":
                    itemBox.sprite = shieldImage;
                    break;
                case "skull":
                    itemBox.sprite = skullImage;
                    break;
                case "special":
                    itemBox.sprite = specialImage;
                    break;
            }
        }
        else
        {
            itemBox.sprite = itemBoxImage;
        }

        //骸骨を拾った時の処理
        if (GameDataSharing.haveItem == "skull")
        {
            itemEffectText.text = "呪われました";
            itemEffectText.color = skullColor;

            time += Time.deltaTime;
            second = (int)time % 60;
            if (second == 1)
            {
                currentHP = currentHP - skullDamage;
                if (currentHP < 0)
                {
                    currentHP = 0;
                }
                if (isBattle)
                {
                    UpdateHP();
                    script.receivedDamage();
                }
                second = 0;
                time = 0;
            }
        }

        //体力が0以下になった時
        if (currentHP <= 0)
        {
            currentHP = 0;
            gameResultDialog.StartDialog("敗北", "残念!!\n回復して再戦しましょう");
            isBattle = false;
        }
        //アイテム効果の表示
        if (isitem)
        {
            time += Time.deltaTime;
            second = (int)time % 60;
            if (second == 3)
            {
                itemEffectText.text = "";
                second = 0;
                time = 0;
                isitem = false;
            }
        }
    }



    public void UpdateHP()
    {
        TextYourHP.text = currentHP + "/" + initialHP;
        SliderYourHP.value = currentHP;
        userDataDB.SetUserData("currentHP", currentHP.ToString());
    }
    public void onClickItem()
    {
        isitem = true;
        switch (GameDataSharing.haveItem)
        {
            case "attack":
                BulletGenerate.plusPower = BulletGenerate.plusPower * 2;
                itemEffectText.text = "攻撃力２倍";
                itemEffectText.color = attackColor;
                itemBox.sprite = itemBoxImage;
                break;
            case "bullet":
                bulletGenerate.plusDoubleBullet();
                itemEffectText.text = "最大装填２倍";
                itemEffectText.color = bulletColor;
                itemBox.sprite = itemBoxImage;
                break;
            case "HP":
                userDataDB.SetUserData("currentHP", initialHP.ToString());
                currentHP = initialHP;
                if (isBattle)
                {
                    UpdateHP();
                }
                itemEffectText.text = "体力最大回復";
                itemEffectText.color = hpColor;
                itemBox.sprite = itemBoxImage;
                break;
            case "shield":
                plusDefence += 2;
                if (plusDefence > 999)
                {
                    plusDefence = 999;
                }
                itemEffectText.text = "防御2上昇";
                itemEffectText.color = shieldColor;
                itemBox.sprite = itemBoxImage;
                break;
            case "special":
                itemEffectText.text = "Special";
                itemEffectText.color = specialColor;
                itemBox.sprite = itemBoxImage;
                break;
        }
    }
}
