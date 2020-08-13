[System.SerializableAttribute]
public class UserData
{
    public string id;
    public string name;
    public int hp;
    public int entryMonsterNum;
}



[System.SerializableAttribute]
public class MonsterListData
{
    public int[] num;
    public string[] name;
    public int[] initialHP;
    public int[] currentHP;
    public int[] power;
    public int[] defence;
    public int[] userCount;
    public string[] popPlace;
    public string[] monsterImage;
}

[System.SerializableAttribute]
public class SendData
{
    public int sendNo;
    public string idUser;
    public string name;
    public int hpUser;
    public float posXUser;
    public float posYUser;
    public float posZUser;
    public int entryMonsterNum;
    public int damage;
    public bool escape;
    public int hitItemNum;
    public string haveItem;
    public int specialStart;
    public int specialPush;
}

[System.SerializableAttribute]
public class ReceiveData
{
    public int numMonster;
    public int receiveNo;
    public int currentHPMonster;
    public float posXMonster;
    public float posYMonster;
    public float posZMonster;
    public string action;
    public int userCount;
    public OtherUserData[] otherUserData;
    public ItemData[] itemData;
    public SpecialData specialData;
    public string from;
}

[System.Serializable]
public class OtherUserData
{
    public string id;
    public string name;
    public int hp;
    public float posX;
    public float posY;
    public float posZ;
    public int monsterNum;
    public string haveItem;
}

[System.Serializable]
public class ItemData
{
    public int itemNum;
    public string itemName;
}

[System.Serializable]
public class SpecialData
{
    public int start;
    public string startPeople;
    public int peopleCountMax;
    public int peopleCount;
    public string result;
    public string Step3;
    public string Step2;
    public string Step1;
    public string Step0;
}


[System.SerializableAttribute]
public class DLData
{
    public string monsterName;
    public int power;
    public int defence;
    public int initialHPMonster;
    public int currentHPMonster;
}


[System.SerializableAttribute]
public class EscapeData
{
    public string idUser;
    public int entryMonsterNum;
    public int escape;
}
