using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using WebSocketSharp;
using System.Text;
using System.IO;

public class GameDataSharing : MonoBehaviour
{
    ///Monster
    public Text MonsterNameText;
    public Text MonsterHPText;
    public Slider SliderMonsterHP;
    private int initialHPMonster;
    private int currentHPMonster;
    public static float targetPosX;
    public static float targetPosZ;
    public static int MonsterHP;
    public static string action = "Breathing";
    private float dyingTime = 0.0f;
    ///otherUser
    public Text otherUserName01Text, otherUserName02Text, otherUserName03Text;
    public Text otherUserHP01Text, otherUserHP02Text, otherUserHP03Text;
    public Slider otherUserHP01Slider, otherUserHP02Slider, otherUserHP03Slider;
    public GameObject otherUser01, otherUser02, otherUser03;
    public Text userCountText;
    private int otherUserNo01, otherUserNo02, otherUserNo03;
    //通信部分
    private ServerHostSetting serverHostSetting = new ServerHostSetting();
    private bool isStopWatch = true;
    private bool isStart = false;
    private bool isSocket = true;
    private string url;
    private WebSocket ws;
    //SendDataクラス
    private SendData sendData = new SendData();
    //UserDataDBクラス
    private UserDataDB userDataDB = new UserDataDB();
    //ReceiveDataクラス
    private ReceiveData receiveData = new ReceiveData();
    //FPS, ms（0.5秒ごとに表示更新）
    public Text fpsText;
    public Text msText;
    private float fps = 0;
    private float ms = 0;
    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    private float span = 0.05f;
    private float currentTime = 0.0f;
    private float nextTime = 0.0f;
    private float interval = 0.5f;
    //ARCamera, Monster
    public GameObject ARCamera;
    //public GameObject Monster;
    private GameResultDialog gameResultDialog;
    //SpecialDialog
    private DialogController SpecialDialogController;
    //csvファイルに書き込み
    private StreamWriter csvSW;
    private int messageNo = 0;
    //item
    public static string haveItem;
    private GameObject itemPrefab;
    public GameObject attackItem, bulletItem, HPItem, shieldItem, skullItem, specialItem;
    private GameObject itemMarker;
    public GameObject item01, item02, item03, item04, item05, item06, item07, item08;
    private Vector3 positem01, positem02, positem03, positem04, positem05, positem06, positem07, positem08;
    private float updateARMarkerTime;
    //itemの構造体
    public struct Item
    {
        public int itemNum;
        public string itemName;
    }
    public static Item[] item = new Item[8];
    //Special
    public Text startPeople;
    public Text readyToSpecial;
    public Text Step3, Step2, Step1, Step0;
    public Text result;
    private bool isStartSpecial = false;
    public static bool isopenSpecial = true;
    private bool isReadySpecial = false;
    private float specialTime = 0;
    private float specialSendTime = 0;
    private bool isSpecialSend = true;
    private bool isSpecialClickUP = true;
    public GameObject specialFire;
    public Transform special01, special02, special03, special04;
    public Transform special05, special06, special07;
    //Color
    private Color StandbyColor = new Color(176f / 255f, 162f / 255f, 162f / 255f);
    private Color SpecialStepColor = new Color(188f / 255f, 15f / 255f, 15f / 255f);
    private Color correctColor = new Color(234f / 255f, 15f / 255f, 15f / 255f);
    private Color failureColor = new Color(13f / 255f, 72f / 255f, 233f / 255f);
    //Sound
    private AudioSource[] audioSource;
    private bool isSound0 = true;
    private bool isSound1 = true;
    private bool isSound2 = true;
    private bool isSound3 = true;
    private bool isSoundc = true;
    private bool isSoundb = true;
    private int next = 0;
    private Queue<int> queue = new Queue<int>();

    void Start()
    {
        //初期化
        audioSource = GetComponents<AudioSource>();
        haveItem = "";
        for (int i = 0; i < 8; i++)
        {
            item[i].itemNum = 0;
            item[i].itemName = "";
        }
        //urlを取得
        url = serverHostSetting.GetURL("GameDataSharing");
        //sendDataの初期化
        sendData.sendNo = 0;
        sendData.escape = false;
        sendData.idUser = userDataDB.GetUserData("id");
        sendData.name = userDataDB.GetUserData("name");
        sendData.hpUser = int.Parse(userDataDB.GetUserData("currentHP"));
        sendData.entryMonsterNum = ScrollViewScript.entryMonsterNum;
        //DLのデータを書き込む
        MonsterNameText.text = DLDirector.monsterName;
        SliderMonsterHP.maxValue = DLDirector.initialHPMonster;
        MonsterHP = DLDirector.currentHPMonster;
        SliderMonsterHP.value = DLDirector.currentHPMonster;
        MonsterHPText.text = DLDirector.currentHPMonster + "/" + DLDirector.initialHPMonster;
        initialHPMonster = DLDirector.initialHPMonster;
        //WebSocket開始
        connect();
        string jsondata = JsonUtility.ToJson(sendData);
        if (isSocket)
        {
            ws.Send(jsondata);
        }
        //ResultDialog
        GameObject resultDialogDirector = GameObject.Find("ResultDialog");
        gameResultDialog = resultDialogDirector.GetComponent<GameResultDialog>();
        //SpecialDialog
        GameObject SpecialDialog = GameObject.Find("SpecialDialog");
        SpecialDialogController = SpecialDialog.GetComponent<DialogController>();
        //csv
        csvSW = new StreamWriter(Application.persistentDataPath + "/" + sendData.name + "_SettingNo" + serverHostSetting.GetSettingNo() + ".csv", true, Encoding.GetEncoding("UTF-8"));
    }

    void Update()
    {
        //FPS計算
        currentTime += Time.deltaTime;
        fps = 1f / Time.deltaTime;
        if (currentTime > span)
        {
            if (Time.time > nextTime)
            {
                nextTime += interval;
                fpsText.text = "FPS: " + fps.ToString("f0");
            }
            currentTime = 0.0f;
        }

        //isStartがtrueで更新
        if (isStart)
        {
            updateUI();
            updateARMarker();
            updateMonsterAction();
            updateSpecial();
            if (ms != 0)
            {
                msText.text = " ms: " + ms;
            }
            if (MonsterHP == 0)
            {
                dyingTime += Time.deltaTime;
                if (dyingTime >= 2.0f)
                {
                    gameResultDialog.StartDialog("勝利", "おめでとう!!\n次も勝ちましょう!");
                }
            }
        }


    }

    public void connect()
    {
        ws = new WebSocket(url);
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Open");
        };
        ws.OnMessage += (sender, e) =>
        {
            isStart = true;
            receiveData = JsonUtility.FromJson<ReceiveData>(e.Data);
            Debug.Log(e.Data);
            if (queue.Count > 0 && receiveData.from == sendData.idUser && !isStopWatch)
            {
                next = queue.Dequeue();
            }
            //Debug.Log("next:" + next + "   receiveNo:" + receiveData.receiveNo);
            if (queue.Count == 0 && !isStopWatch && receiveData.from == sendData.idUser && next == receiveData.receiveNo)
            {
                stopwatch.Stop();
                ms = stopwatch.ElapsedMilliseconds;
                Debug.Log("  ms:" + ms);
                messageNo++;
                // ヘッダー出力
                string[] s1;
                string s2;
                //表題
                //No.,日時, 参加人数, FPS, ms
                s1 = new string[] { messageNo.ToString(), DateTime.Now.ToString("yyyy年MM月dd日hh時mm分ss.ff秒"), receiveData.otherUserData.Length.ToString(), fps.ToString("f0"), ms.ToString() };
                s2 = string.Join(",", s1);
                csvSW.WriteLine(s2);
                // StreamWriterを閉じる
                //Debug.Log("stopwatch stop");
                isStopWatch = true;
            }
            MonsterHP = receiveData.currentHPMonster;

        };
        ws.OnError += (sender, e) =>
        {
            isSocket = false;
            gameResultDialog.StartDialog("エラー", "通信エラーです\nホームへ戻ります");
            //gameResultDialog.Escape();
            Debug.Log("WebSocket Error Message: " + e.Message);
            csvSW.Close();

        };
        ws.OnClose += (sender, e) =>
        {
            isSocket = false;
            //gameResultDialog.StartDialog("エラー","通信エラーです\nホームへ戻ります");
            //gameResultDialog.Escape();
            Debug.Log("WebSocket Close");
            csvSW.Close();
        };
        if (isSocket) ws.Connect();
    }
    //切断
    public void Close()
    {
        sendData.escape = true;
        string jsondata = JsonUtility.ToJson(sendData);
        if (isSocket)
        {
            ws.Send(jsondata);
        }

        isStart = false;
        //接続を切断
        ws.Close();
    }

    public void HitMonster(int weponPower)
    {
        if (weponPower - DLDirector.defence > 0)
        {
            Vector3 posUser = ARCamera.transform.position;
            sendData.posXUser = posUser.x;
            sendData.posYUser = posUser.y;
            sendData.posZUser = posUser.z;
            sendData.hpUser = int.Parse(userDataDB.GetUserData("currentHP"));
            sendData.damage = weponPower - DLDirector.defence;

            if (isStopWatch == true)
            {
                sendData.sendNo++;
                queue.Enqueue(sendData.sendNo);
                stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                //Debug.Log("stopwatch start");
                isStopWatch = false;
            }

            string jsondata = JsonUtility.ToJson(sendData);

            if (isSocket)
            {
                ws.Send(jsondata);
            }
            sendData.damage = 0;
        }
    }

    public void HitItem(int itemNum)
    {
        sendData.hitItemNum = itemNum;
        if (isStopWatch == true)
        {
            sendData.sendNo++;
            queue.Enqueue(sendData.sendNo);
            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            isStopWatch = false;
        }

        for (int i = 0; i < item.Length; i++)
        {
            if (item[i].itemNum == itemNum)
            {
                item[i].itemNum = 0;
                item[i].itemName = "";
            }
        }
        string jsondata = JsonUtility.ToJson(sendData);
        Debug.Log("SendNo:" + sendData.sendNo + "  HitItem:" + sendData.hitItemNum);
        if (isSocket)
        {
            ws.Send(jsondata);
        }
        sendData.hitItemNum = 0;
    }


    public void receivedDamage()
    {
        Vector3 posUser = ARCamera.transform.position;
        sendData.posXUser = posUser.x;
        sendData.posYUser = posUser.y;
        sendData.posZUser = posUser.z;
        sendData.hpUser = int.Parse(userDataDB.GetUserData("currentHP"));
        sendData.damage = 0;

        if (isStopWatch == true)
        {
            sendData.sendNo++;
            queue.Enqueue(sendData.sendNo);
            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            //Debug.Log("stopwatch start");
            isStopWatch = false;
        }
        string jsondata = JsonUtility.ToJson(sendData);

        if (isSocket)
        {
            ws.Send(jsondata);
        }
    }

    public void useItem()
    {
        if (haveItem != "skull")
        {
            Debug.Log(haveItem);
            if (haveItem == "special")
            {
                Debug.Log("specialStart");
                sendData.specialStart = 1;
            }
            sendData.haveItem = "";
            haveItem = "";
            string jsondata = JsonUtility.ToJson(sendData);

            if (isSocket)
            {
                ws.Send(jsondata);
            }
            if (sendData.specialStart == 1)
            {
                sendData.specialStart = 0;
            }
        }
    }


    public void updateUI()
    {
        for (int i = 0; i < receiveData.otherUserData.Length; i++)
        {
            if (receiveData.otherUserData[i].id == sendData.idUser)
            {
                haveItem = receiveData.otherUserData[i].haveItem;
                sendData.haveItem = haveItem;
                break;
            }
        }

        for (int i = 0; i < receiveData.itemData.Length; i++)
        {
            switch (i)
            {
                case 0:
                    itemMarker = item01;
                    break;
                case 1:
                    itemMarker = item02;
                    break;
                case 2:
                    itemMarker = item03;
                    break;
                case 3:
                    itemMarker = item04;
                    break;
                case 4:
                    itemMarker = item05;
                    break;
                case 5:
                    itemMarker = item06;
                    break;
                case 6:
                    itemMarker = item07;
                    break;
                case 7:
                    itemMarker = item08;
                    break;
            }

            if (receiveData.itemData[i].itemName == "")
            {
                item[i].itemNum = 0;
                item[i].itemName = "";
                if (itemMarker.transform.childCount != 0)
                {
                    foreach (Transform transform in itemMarker.transform)
                    {
                        // Transformからゲームオブジェクト取得・削除
                        var item = transform.gameObject;
                        Destroy(item);
                        Debug.Log("item0" + i + " Deleted");
                    }
                    //次の配列へ
                    continue;
                }
            }

            if (item[i].itemNum == 0 && item[i].itemNum != receiveData.itemData[i].itemNum)
            {
                switch (i)
                {
                    case 0:
                        itemMarker = item01;
                        break;
                    case 1:
                        itemMarker = item02;
                        break;
                    case 2:
                        itemMarker = item03;
                        break;
                    case 3:
                        itemMarker = item04;
                        break;
                    case 4:
                        itemMarker = item05;
                        break;
                    case 5:
                        itemMarker = item06;
                        break;
                    case 6:
                        itemMarker = item07;
                        break;
                    case 7:
                        itemMarker = item08;
                        break;
                }
                item[i].itemNum = receiveData.itemData[i].itemNum;
                item[i].itemName = receiveData.itemData[i].itemName;

                switch (item[i].itemName)
                {
                    case "attack":
                        itemPrefab = attackItem;
                        break;
                    case "bullet":
                        itemPrefab = bulletItem;
                        break;
                    case "HP":
                        itemPrefab = HPItem;
                        break;
                    case "shield":
                        itemPrefab = shieldItem;
                        break;
                    case "skull":
                        itemPrefab = skullItem;
                        break;
                    case "special":
                        itemPrefab = specialItem;
                        break;
                }
                if (itemPrefab != null && itemMarker.transform.childCount == 0)
                {
                    GameObject itemObject = (GameObject)Instantiate(itemPrefab);
                    itemObject.transform.SetParent(itemMarker.transform, false);
                    Debug.Log("item0" + i + " {itemNum:" + item[i].itemNum + " itemName:【" + item[i].itemName + "】} Instantiate");
                }
                itemPrefab = null;
            }
        }

        int userCount = 0;
        for (int i = 0; i < receiveData.otherUserData.Length; i++)
        {
            if (receiveData.otherUserData[i].monsterNum == sendData.entryMonsterNum)
            {
                userCount++;
            }
        }
        userCountText.text = userCount + " 人";
        if (userCount <= 1)
        {
            otherUser01.SetActive(false);
            otherUser02.SetActive(false);
            otherUser03.SetActive(false);
        }
        else
        {
            if (2 == userCount)
            {
                SerchOtherUserNo1();
                otherUser01.SetActive(true);
                otherUser02.SetActive(false);
                otherUser03.SetActive(false);
                SetOhterUserNo1(receiveData.otherUserData[otherUserNo01].name, receiveData.otherUserData[otherUserNo01].hp);
            }
            else if (3 == userCount)
            {
                SerchOtherUserNo1();
                SerchOtherUserNo2();
                otherUser01.SetActive(true);
                otherUser02.SetActive(true);
                otherUser03.SetActive(false);
                SetOhterUserNo1(receiveData.otherUserData[otherUserNo01].name, receiveData.otherUserData[otherUserNo01].hp);
                SetOhterUserNo2(receiveData.otherUserData[otherUserNo02].name, receiveData.otherUserData[otherUserNo02].hp);
            }
            else if (4 <= userCount)
            {
                SerchOtherUserNo1();
                SerchOtherUserNo2();
                SerchOtherUserNo3();

                otherUser01.SetActive(true);
                otherUser02.SetActive(true);
                otherUser03.SetActive(true);
                SetOhterUserNo1(receiveData.otherUserData[otherUserNo01].name, receiveData.otherUserData[otherUserNo01].hp);
                SetOhterUserNo2(receiveData.otherUserData[otherUserNo02].name, receiveData.otherUserData[otherUserNo02].hp);
                SetOhterUserNo3(receiveData.otherUserData[otherUserNo03].name, receiveData.otherUserData[otherUserNo03].hp);
            }
        }
        if (receiveData.numMonster == sendData.entryMonsterNum)
        {
            MonsterHPText.text = receiveData.currentHPMonster + "/" + initialHPMonster;
            SliderMonsterHP.value = receiveData.currentHPMonster;
        }
    }
    void SerchOtherUserNo1()
    {
        double minDistance1 = 100000000000000;
        for (int i = 0; i < receiveData.otherUserData.Length; i++)
        {
            if (receiveData.otherUserData[i].id != sendData.idUser && receiveData.otherUserData[i].monsterNum == sendData.entryMonsterNum)
            {
                Vector3 otherUserVector3 = new Vector3(receiveData.otherUserData[i].posX, receiveData.otherUserData[i].posY, receiveData.otherUserData[i].posZ);
                Vector3 posUser = ARCamera.transform.position;
                float distance = Vector3.Distance(otherUserVector3, posUser);

                if (distance < minDistance1)
                {
                    minDistance1 = distance;
                    otherUserNo01 = i;
                    break;
                }
            }
        }
    }

    void SerchOtherUserNo2()
    {
        double minDistance2 = 100000000000000;
        for (int i = 0; i < receiveData.otherUserData.Length; i++)
        {
            if (receiveData.otherUserData[i].id != sendData.idUser && receiveData.otherUserData[i].monsterNum == sendData.entryMonsterNum && receiveData.otherUserData[otherUserNo01].id != receiveData.otherUserData[i].id)
            {
                Vector3 otherUserVector3 = new Vector3(receiveData.otherUserData[i].posX, receiveData.otherUserData[i].posY, receiveData.otherUserData[i].posZ);
                Vector3 posUser = ARCamera.transform.position;
                float distance = Vector3.Distance(otherUserVector3, posUser);
                if (distance < minDistance2)
                {
                    minDistance2 = distance;
                    otherUserNo02 = i;
                    break;
                }
            }
        }
    }

    void SerchOtherUserNo3()
    {
        double minDistance3 = 100000000000000;
        for (int i = 0; i < receiveData.otherUserData.Length; i++)
        {
            if (receiveData.otherUserData[i].id != sendData.idUser && receiveData.otherUserData[i].monsterNum == sendData.entryMonsterNum && receiveData.otherUserData[otherUserNo01].id != receiveData.otherUserData[i].id && receiveData.otherUserData[otherUserNo02].id != receiveData.otherUserData[i].id)
            {
                Vector3 otherUserVector3 = new Vector3(receiveData.otherUserData[i].posX, receiveData.otherUserData[i].posY, receiveData.otherUserData[i].posZ);
                Vector3 posUser = ARCamera.transform.position;
                float distance = Vector3.Distance(otherUserVector3, posUser);

                if (distance < minDistance3)
                {
                    minDistance3 = distance;
                    otherUserNo03 = i;
                    break;
                }
            }
        }
    }
    void SetOhterUserNo1(string name, int hp)
    {
        otherUserName01Text.text = name;
        otherUserHP01Text.text = hp + "/" + "100";
        otherUserHP01Slider.value = hp;
    }
    void SetOhterUserNo2(string name, int hp)
    {
        otherUserName02Text.text = name;
        otherUserHP02Text.text = hp + "/" + "100";
        otherUserHP02Slider.value = hp;
    }
    void SetOhterUserNo3(string name, int hp)
    {
        otherUserName03Text.text = name;
        otherUserHP03Text.text = hp + "/" + "100";
        otherUserHP03Slider.value = hp;
    }

    public void onSpecialButtonClickDonw()
    {
        Debug.Log("SpecialButton ClickDown");
        sendData.specialPush = 1;
        string jsondata = JsonUtility.ToJson(sendData);
        if (isSocket && receiveData.specialData.result == "" && isSpecialSend)
        {
            Debug.Log("*******************");
            Debug.Log("開始いいいいいいいいいい");
            isSpecialSend = false;
            if (isSocket)
            {
                ws.Send(jsondata);
            }
        }
    }

    public void onSpecialButtonClickUP()
    {
        Debug.Log("SpecialButton ClickUP");
        sendData.specialPush = 0;
        string jsondata = JsonUtility.ToJson(sendData);
        if (isSocket && receiveData.specialData.result == "" && isSpecialSend && isSpecialClickUP)
        {
            isSpecialSend = false;
            ws.Send(jsondata);
            Debug.Log("*******************");
            Debug.Log("よろしくお願いしまああああああああすううううう");
        }
    }

    void updateSpecial()
    {
        if (isSpecialSend == false)
        {
            Debug.Log("+++++++++++");
            Debug.Log("ボタン回復します");
            specialSendTime += Time.deltaTime;
            if (specialSendTime >= 0.005f)
            {
                Debug.Log("おせえええええええええ");
                isSpecialSend = true;
                specialSendTime = 0;
            }
        }
        if (receiveData.specialData.start == 1 && isStartSpecial == false)
        {
            isStartSpecial = true;
            Step3.color = StandbyColor;
            Step2.color = StandbyColor;
            Step1.color = StandbyColor;
            Step0.color = StandbyColor;
            result.text = "";
            isReadySpecial = false;
            readyToSpecial.text = "パワー注入状態：準備中";
            specialTime = 0;
        }

        if (isStartSpecial && isopenSpecial)
        {
            Debug.Log("*************\nSpecial　開始");
            SpecialDialogController.OpenDialog();

            startPeople.text = receiveData.specialData.startPeople;
            isStartSpecial = false;
            isopenSpecial = false;
        }

        if (!isReadySpecial)
        {
            int last = receiveData.specialData.peopleCountMax - receiveData.specialData.peopleCount;

            if (last != 0)
            {
                readyToSpecial.text = "パワー注入状態：準備中（あと" + last + "人)";
            }
            else if (last == 0 && receiveData.specialData.peopleCount != 0 && receiveData.specialData.peopleCountMax != 0)
            {
                readyToSpecial.text = "パワー注入状態：準備完了！";
                Debug.Log("Step Start");
                isReadySpecial = true;
            }
        }

        if (receiveData.specialData.Step3 == "3" && isReadySpecial && receiveData.specialData.Step2 != "2")
        {
            //Debug.Log("receiveData.specialData.Step3:"+receiveData.specialData.Step3);
            Step3.color = SpecialStepColor;
            if (isSound3)
            {
                audioSource[2].PlayOneShot(audioSource[2].clip);
                isSound3 = false;
            }
        }
        if (receiveData.specialData.Step2 == "2" && isReadySpecial && receiveData.specialData.Step1 != "1")
        {
            //Debug.Log("receiveData.specialData.Step2:"+receiveData.specialData.Step2);
            Step2.color = SpecialStepColor;
            if (isSound2)
            {
                audioSource[2].PlayOneShot(audioSource[2].clip);
                isSound2 = false;
            }
        }
        if (receiveData.specialData.Step1 == "1" && isReadySpecial && receiveData.specialData.Step1 != "ClickUP")
        {
            //Debug.Log("receiveData.specialData.Step1:"+receiveData.specialData.Step1);
            Step1.color = SpecialStepColor;
            if (isSound1)
            {
                audioSource[2].PlayOneShot(audioSource[2].clip);
                isSound1 = false;
            }
        }
        if (receiveData.specialData.Step0 == "ClickUP" && isReadySpecial)
        {
            //Debug.Log("receiveData.specialData.Step0:"+receiveData.specialData.Step0);
            isSpecialClickUP = true;
            Step0.color = SpecialStepColor;
            if (isSound0)
            {
                audioSource[4].PlayOneShot(audioSource[4].clip);
                isSound0 = false;
            }

        }
        if (receiveData.specialData.result != "")
        {
            if (receiveData.specialData.result == "成功")
            {
                result.text = receiveData.specialData.result;
                result.color = correctColor;
                if (isSoundc)
                {
                    audioSource[3].PlayOneShot(audioSource[3].clip);
                    isSoundc = false;
                }
            }
            else if (receiveData.specialData.result == "失敗")
            {
                result.color = failureColor;
                result.text = receiveData.specialData.result;
                if (isSoundb)
                {
                    audioSource[2].PlayOneShot(audioSource[1].clip);
                    isSoundb = false;
                }
            }


        }

        if (!isSound3)
        {
            specialTime += Time.deltaTime;
            if (specialTime >= 6.5f)
            {
                SpecialDialogController.CloseDialog();
                Debug.Log("*************\nSpecial　終了");
                if (receiveData.specialData.result == "成功")
                {
                    specialExplosion();
                }

                isSpecialClickUP = false;
                isStartSpecial = false;
                isopenSpecial = true;
                isReadySpecial = false;
                startPeople.text = "";
                isSound0 = true;
                isSound1 = true;
                isSound2 = true;
                isSound3 = true;
                isSoundc = true;
                isSoundb = true;
            }
        }
    }

    void updateARMarker()
    {
        updateARMarkerTime += Time.deltaTime;
        if (updateARMarkerTime >= 0.5f)
        {
            if (positem01 != item01.transform.position)
            {
                item01.SetActive(true);
                positem01 = item01.transform.position;
            }
            else item01.SetActive(false);

            if (positem02 != item02.transform.position)
            {
                item02.SetActive(true);
                positem02 = item02.transform.position;
            }
            else item02.SetActive(false);

            if (positem03 != item03.transform.position)
            {
                item03.SetActive(true);
                positem03 = item03.transform.position;
            }
            else item03.SetActive(false);

            if (positem04 != item04.transform.position)
            {
                item04.SetActive(true);
                positem04 = item04.transform.position;
            }
            else item04.SetActive(false);

            if (positem05 != item05.transform.position)
            {
                item05.SetActive(true);
                positem05 = item05.transform.position;
            }
            else item05.SetActive(false);

            if (positem06 != item06.transform.position)
            {
                item06.SetActive(true);
                positem06 = item06.transform.position;
            }
            else item06.SetActive(false);

            if (positem07 != item07.transform.position)
            {
                item07.SetActive(true);
                positem07 = item07.transform.position;
            }
            else item07.SetActive(false);

            if (positem08 != item08.transform.position)
            {
                item08.SetActive(true);
                positem08 = item08.transform.position;
            }
            else item08.SetActive(false);

            updateARMarkerTime = 0;
        }
    }
    void updateMonsterAction()
    {
        if (receiveData.posXMonster != targetPosX)
        {
            targetPosX = receiveData.posXMonster;
        }
        if (receiveData.posZMonster != targetPosZ)
        {
            targetPosZ = receiveData.posZMonster;
        }
        if (receiveData.action != action)
        {
            action = receiveData.action;
        }
    }

    void specialExplosion()
    {
        /* audioSource[0].PlayOneShot(audioSource[0].clip);
         Destroy(Instantiate(specialFire, special01.position, Quaternion.identity), 3f);
         Destroy(Instantiate(specialFire, special02.position, Quaternion.identity), 3f);
         Destroy(Instantiate(specialFire, special03.position, Quaternion.identity), 3f);
         Destroy(Instantiate(specialFire, special04.position, Quaternion.identity), 3f);
         Destroy(Instantiate(specialFire, special05.position, Quaternion.identity), 3f);
         Destroy(Instantiate(specialFire, special06.position, Quaternion.identity), 3f);
         Destroy(Instantiate(specialFire, special07.position, Quaternion.identity), 3f);*/
    }
}
