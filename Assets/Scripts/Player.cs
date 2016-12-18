using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {
    [SyncVar]
    private bool _isDead = false;
    public bool isdead
    {
        get { return _isDead; }
        //only player class or class derived from player class can change it
        protected set { _isDead = value; }
    }


    [SerializeField]
    private int maxHealth = 100;

    //when var changed, it will be push to client
    [SyncVar]
    private int currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;


    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        SetDefaults();
    }

    //void Update()
    //{
    //    if (!isLocalPlayer)
    //        return;
    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        RpcTakeDamange(9999);
    //    }
    //}

    public void SetDefaults()
    {
        isdead = false;
        currentHealth = maxHealth;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;

    }
    
    //compute in any client
    [ClientRpc]
    public void RpcTakeDamange(int _amount)
    {
        if (isdead)
            return;

        currentHealth -= _amount;
        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isdead = true;

        //DISABLE COMPONENTS
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;


        Debug.Log(transform.name + " is dead!.");

        //CALL RESPAWN METHOD
        //The execution of a coroutine can be paused at any point using the yield statement. The yield return value specifies when the coroutine is resumed. 
        StartCoroutine(Respawn());

    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        Debug.Log(transform.name + " Respawned!!");
    }

}
