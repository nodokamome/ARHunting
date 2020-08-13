using UnityEngine;
using System.Collections;

public class MonsterMove : MonoBehaviour
{

    private Vector3 target;
    private Vector3 monster;
    private Vector2 MonsterPos;
    private Vector2 targetPos;
    private Vector2 Apos;
    public GameObject Explosion;
    public Transform Explosion01Pos;
    public Transform Explosion02Pos;
    public Transform Explosion03Pos;
    public Transform Explosion04Pos;
    public GameObject ARCamera;

    //1m辺り6秒の計算
    public float speed = 0.2f;
    private float useTime;
    private float moveTime;
    private float distanceToTarget;
    private float distanceToMe;
    private bool isMonsterAttack = false;
    private Animator animator;
    private GameDirector gameDirector;
    private GameDataSharing gameDataSharing;
    private AudioSource[] audioSources;
    private bool isMonsterMove = true;
    private bool isDying = true;

    void Start()
    {
        //距離の表示
        animator = GetComponent<Animator>();
        MonsterPos = new Vector2(transform.position.x, transform.position.z);

        //ResultDialog
        GameObject gameDirectorObj = GameObject.Find("GameDirector");
        gameDirector = gameDirectorObj.GetComponent<GameDirector>();

        //ResultDialog
        GameObject gameDataSharingObj = GameObject.Find("GameDataSharing");
        gameDataSharing = gameDataSharingObj.GetComponent<GameDataSharing>();

        audioSources = GetComponents<AudioSource>();
        audioSources[3].PlayOneShot(audioSources[3].clip);
    }
    void Update()
    {
        MonsterPos = new Vector2(transform.position.x, transform.position.z);
        monster = new Vector3(transform.position.x, -30, transform.position.z);
        targetPos = new Vector2(GameDataSharing.targetPosX, GameDataSharing.targetPosZ);
        target = new Vector3(GameDataSharing.targetPosX, -30, GameDataSharing.targetPosZ);

        Apos = new Vector2(ARCamera.transform.position.x, ARCamera.transform.position.z);
        Vector2 Bpos = new Vector2(transform.position.x, transform.position.z);
        //距離の表示
        distanceToMe = Vector2.Distance(Apos, Bpos);
        distanceToMe = distanceToMe * 0.005f;

        distanceToTarget = Vector2.Distance(MonsterPos, targetPos);
        distanceToTarget = distanceToTarget * 0.005f;
        //距離の表示
        //        Debug.Log("ターゲットまでの距離:" + distanceToTarget + "m");
        //Debug.Log("targetPosX:" + GameDataSharing.targetPosX + " targetPosZ:" + GameDataSharing.targetPosZ);
        useTime = distanceToTarget / speed;
        moveTime = 1 / useTime;
        if (0.1 < distanceToTarget)
        {
            //Debug.Log("***  Running  ***");
            //走るアニメーションを再生
            animator.SetBool("Running", true);
            animator.SetBool("Walking", false);
            animator.SetBool("Biting", false);
            animator.SetBool("Breathing", false);


            speed = 0.5f;
            isMonsterAttack = true;
        }
        else if (0.05 <= distanceToTarget && distanceToTarget < 0.1)
        {
            // Debug.Log("***  Walking  ***");
            //歩くアニメーションを再生
            animator.SetBool("Running", false);
            animator.SetBool("Walking", true);
            speed = 0.3f;
            isMonsterAttack = true;
        }
        if (0.05 <= distanceToTarget && isMonsterMove)
        {
            //            Debug.Log("LoockAt:x=" + GameDataSharing.targetPosX + ", y=" + -30 + ", z=" + GameDataSharing.targetPosZ);
            //歩くアニメーションを再生
            transform.position = Vector3.Slerp(monster, target, Time.deltaTime * speed);
            this.transform.LookAt(new Vector3(GameDataSharing.targetPosX, -30, GameDataSharing.targetPosZ));
        }
        else
        {
            animator.SetBool("Walking", false);
            if (!animator.GetBool("Biting") && !animator.GetBool("Breathing"))
            {
                if (GameDataSharing.action == "")
                {
                    animator.SetBool("Breathing", true);
                }
                else
                {
                    animator.SetBool(GameDataSharing.action, true);
                }
            }
            isMonsterAttack = true;
        }

        if (GameDataSharing.MonsterHP == 0 && isDying)
        {
            isDying = false;
            animator.SetBool("Dying", true);
            isMonsterAttack = false;
            audioSources[1].PlayOneShot(audioSources[1].clip);

        }
    }

    public void MonsterAttack()
    {
        if (GameDirector.currentHP > 0 && isMonsterAttack)
        {
            if (GameDataSharing.action == "Breathing")
            {
                audioSources[0].PlayOneShot(audioSources[0].clip);
                if (distanceToMe < 0.8)
                {
                    //     Debug.Log("Breath Hit");
                    if (!GameDataSharing.isopenSpecial && GameDirector.isARBox)
                    {
                        int damage = DLDirector.power - GameDirector.plusDefence;
                        if (damage < 0)
                        {
                            damage = 0;
                        }
                        GameDirector.currentHP = GameDirector.currentHP - damage;
                    }

                }
                Destroy(Instantiate(Explosion, Explosion01Pos.position, Quaternion.identity), 3f);
                Destroy(Instantiate(Explosion, Explosion02Pos.position, Quaternion.identity), 3f);
                Destroy(Instantiate(Explosion, Explosion03Pos.position, Quaternion.identity), 3f);
                Destroy(Instantiate(Explosion, Explosion04Pos.position, Quaternion.identity), 3f);
                animator.SetBool("Breathing", false);

            }
            else if (GameDataSharing.action == "Biting")
            {
                audioSources[2].PlayOneShot(audioSources[2].clip);

                float angle = 120.0f;
                // プレイヤーと敵を結ぶ線と視線の角度差がangle以内なら当たり
                Vector2 eyeDir = this.transform.forward;
                if (Vector2.Angle((Apos - MonsterPos).normalized, eyeDir) <= angle)
                {
                    // 範囲内の処理
                    //        Debug.Log("Bite Hit");
                    if (distanceToMe < 0.8)
                    {
                        if (!GameDataSharing.isopenSpecial && GameDirector.isARBox)
                        {
                            int damage = DLDirector.power - GameDirector.plusDefence;
                            if (damage < 0)
                            {
                                damage = 0;
                            }
                            GameDirector.currentHP = GameDirector.currentHP - damage;
                        }
                    }
                }
                animator.SetBool("Biting", false);
            }

            if (GameDirector.currentHP < 0) GameDirector.currentHP = 0;

            if (GameDirector.isBattle)
            {
                gameDirector.UpdateHP();
                gameDataSharing.receivedDamage();
            }
            if (SystemInfo.supportsVibration)
            {
                Handheld.Vibrate();
            }
        }
        isMonsterAttack = false;
        isMonsterMove = true;
        animator.SetBool("Biting", false);
        animator.SetBool("Breathing", false);
    }

    void MoveAction()
    {
        if (!isMonsterMove)
        {
            isMonsterMove = true;

        }
    }
}
