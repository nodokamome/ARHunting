using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


public class Bullet : MonoBehaviour
{

    public GameObject Explosion;
    private GameObject gameDataSharing;
    private GameObject bulletGenerate;
    public int weponPower;
    private int i;
    private BulletGenerate bulletGenerateScript;
    private GameDataSharing gameDataSharingScript;

    void Start()
    {
        bulletGenerate = GameObject.Find("BulletGenerate");
        bulletGenerateScript = bulletGenerate.GetComponent<BulletGenerate>();
        gameDataSharing = GameObject.Find("GameDataSharing");
        gameDataSharingScript = gameDataSharing.GetComponent<GameDataSharing>();
    }


    //衝突判定
    //相手のタグがMonsterならば、自分を消す
    void OnCollisionEnter(Collision collision)
    {
        //monster
        if (collision.gameObject.tag == "Monster")
        {
            bulletGenerateScript.SoundExplosion();

            if (BulletGenerate.weponPower * BulletGenerate.plusPower > 999)
            {
                gameDataSharingScript.HitMonster(999);
            }
            else
            {
                gameDataSharingScript.HitMonster(BulletGenerate.weponPower * BulletGenerate.plusPower);
            }
            Destroy(Instantiate(Explosion, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity), 3f);
            Destroy(this.gameObject);
        }
        //item
        else
        {
            string strItem = collision.gameObject.name;

            if (strItem.LastIndexOf("(Clone)") == strItem.Length - 7)  //末尾が特定の文字であれば
            {
                strItem = strItem.Substring(0, strItem.Length - 7);   //末尾1文字を除く。結果は「,a,b,c」
            }

            switch (collision.gameObject.transform.parent.name)
            {
                case "Item00":
                    i = 0;
                    break;
                case "Item01":
                    i = 1;
                    break;
                case "Item02":
                    i = 2;
                    break;
                case "Item03":
                    i = 3;
                    break;
                case "Item04":
                    i = 4;
                    break;
                case "Item05":
                    i = 5;
                    break;
                case "Item06":
                    i = 6;
                    break;
                case "Item07":
                    i = 7;
                    break;
            }

            bulletGenerateScript.SoundItem(strItem);
            if (GameDataSharing.haveItem != "skull")
            {
                gameDataSharingScript.HitItem(GameDataSharing.item[i].itemNum);
                Debug.Log("hitItemNum:" + GameDataSharing.item[i].itemNum);
            }
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }
}
