﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Vida vida;
    public float speed  = 5f;
    
    public Rigidbody2D rb;
    Vector2 movement;
    public Animator animator;
    public Text restart;
    bool vivo;
    //["Crosshair objetos"]
    public GameObject crosshair;
    float crosshairdist ;
    //["Arma 1 objetos"]
    public GameObject bulletPrefab;
    bool arma1Active;
    float speed_bullet = 100f;

    //["Arma 2 objetos"]
    public GameObject bullet2Prefab;
    float speed_bullet2 = 100f;
    bool arma2Active;
    int arma2Ammo;
    Vector2 damage;
    IEnumerator disparoRutina;
    public GameObject explosion;

    void Start()
    {
        crosshairdist = 1f;
        restart.gameObject.SetActive(false);
        vivo = true;
        arma2Active = false;
        arma1Active = true;
        arma2Ammo =20;
    }

    void Update()
    {
        if(vivo){
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            animator.SetFloat("Horizontal",movement.x);
            animator.SetFloat("Vertical",movement.y);
            animator.SetFloat("Speed",movement.sqrMagnitude);
            Aim();
            Shoot();
        }else{
            SceneManager.LoadScene(3);
        }
         /*if(Input.GetKeyDown(KeyCode.Space)){
            vivo = true;
            vida.VidaCont = 100;
            restart.gameObject.SetActive(false);
         }*/

         if(vida.VidaCont <=0)
            {
                CancelInvoke("bajarVida");
                vivo = false;
                vida.VidaCont = 0;
                //restart.gameObject.SetActive(true);
            }
         
    }
 
    void FixedUpdate()
    {
        if(vivo){
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }
    }

    void Aim()
    {   
        if(movement != Vector2.zero){
             crosshair.transform.localPosition = movement * crosshairdist;
        }
    }
    void Shoot()
    {  
        Vector2 shootdir = crosshair.transform.localPosition;
        shootdir.Normalize();
        if(arma2Active ==true){
            if(arma2Ammo >0){
                if(Input.GetButtonDown("Fire1")){
                    disparoRutina = disparar(shootdir,bullet2Prefab, speed_bullet2, true);
                    StartCoroutine(disparoRutina);
                    arma2Ammo -=1;
                }
                if(Input.GetButtonUp("Fire1")){
                    StopCoroutine(disparoRutina);
                }
            }else{
                arma2Active = false;
                StopCoroutine(disparoRutina);
                arma1Active = true;
            }
        }
        if(arma1Active == true){
            if(Input.GetButtonDown("Fire1")){
                disparoRutina = disparar(shootdir,bulletPrefab, speed_bullet, false);
                StartCoroutine(disparoRutina);
            }
            if(Input.GetButtonUp("Fire1")){
                StopCoroutine(disparoRutina);
            }

        }
    }
    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Enemy1")){
            InvokeRepeating("bajarVida",0,0.7f);
            //Vida.VidaCont -= 5;
            //Vector2 difference = transform.position - collision.transform.position;
            //transform.position = new Vector2(transform.position.x +Random.Range(0,2), transform.position.y + Random.Range(0,2));
            
        }
        if (collision.CompareTag("Arma2")){
            arma2Active = true;
            arma1Active = false;
            Debug.Log("Recoje arma");
        }

        if(collision.tag == "BalaEnemy2"){
            Destroy(collision.gameObject);
            bajarVida();
            GameObject effect = Instantiate(explosion,collision.transform.position,Quaternion.identity);
            Destroy(effect,.5f);
        }
    }

    void OnTriggerExit2D(Collider2D collider){
        CancelInvoke("bajarVida");
    }

    void bajarVida(){
        vida.VidaCont -= 5;
    }

    IEnumerator disparar(Vector2 shootdir, GameObject bulletPrefab, float speed, bool secondArma){
        while(true){
            GameObject bullet = Instantiate(bulletPrefab,new Vector2(transform.position.x + shootdir.x, transform.position.y+shootdir.y), Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = shootdir * speed;
            bullet.transform.Rotate(0,0,Mathf.Atan2(shootdir.y,shootdir.x) * Mathf.Rad2Deg);
            Destroy(bullet,2f);
            if(secondArma)
                arma2Ammo -=1;
            yield  return new WaitForSeconds(0.5f);
        }
    }

}
