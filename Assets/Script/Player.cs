using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Player : MonoBehaviour
{
    public Transform atkPosRef;
    public Card chosenCard;
    public HealthBar healthBar;
    public TMP_Text healthText; 
    public float Health;
    public float MaxHealth;
    public AudioSource audioSource;
    public AudioClip damageClip;
    private Tweener animationTweener;

    public Player player;
    public GameManager gameManager;
    [SerializeField] float ChoosingInterval = 600;
    private float timer = 0;
    int lastSelected = 0;
    Card[] cards;

    private void Start()
    {
        Health = MaxHealth;
        cards = GetComponentsInChildren<Card>();
    }
    public Attack? AttackValue
    {
        get => chosenCard == null ? null : chosenCard.AttackValue;
        // get
        // {
        //     if (chosenCard == null)
        //         return null;
        //     else
        //         return chosenCard.attack;
        // }
    }

    void Update() {  
        if(gameManager.State != GameManager.GameState.ChooseAttack){
            timer = 0; 
            return;
        }    

        if(timer < ChoosingInterval){
            timer += Time.deltaTime;
            return;
        }
        timer = 0;
        ChooseAttack();
    }
    public void ChooseAttack(){
        var random = Random.Range(1,cards.Length);
        var selection = (lastSelected + random)% cards.Length;
        player.SetChosenCard(cards[selection]);
        lastSelected = selection;
    }

    public void Reset()
    {
        if(chosenCard != null)
        {
            chosenCard.Reset();
        }

        chosenCard = null;
    }
    public void SetChosenCard(Card newCard)
    {
        if(chosenCard != null)
        {
            chosenCard.Reset();
        }
        chosenCard = newCard;
        chosenCard.transform.DOScale(chosenCard.transform.localScale*1.2f,0.2f);
    }
    public void ChangeHealth(float amount)
    {
        Health += amount;
        Health = Mathf.Clamp(Health,0,100);

        healthBar.UpdateBar(Health/MaxHealth);

        healthText.text = Health + "/" + MaxHealth;
    }
    public void AnimateAttack()
    {
        animationTweener = chosenCard.transform
            .DOMove(atkPosRef.position,0.5f);
    }

    public void AnimateDamage()
    {
        audioSource.PlayOneShot(damageClip);
        var image = chosenCard.GetComponent<Image>();
        animationTweener = image
            .DOColor(Color.red,0.1f)
            .SetLoops(3,LoopType.Yoyo)
            .SetDelay(0.2f);
    }

    public void AnimateDraw()
    {
        animationTweener = chosenCard.transform
            .DOMove(chosenCard.OriginalPosition,1)
            .SetDelay(0.2f);
    }

    public bool IsAnimating()
    {
        return animationTweener.IsActive();
    }
    public void IsClickable(bool value)
    {
        Card [] cards = GetComponentsInChildren<Card>();
        foreach(var card in cards)
        {
            card.SetClickable(value);
        }
    }
    public void SetChooseInterval(float choosingInterval){
        ChoosingInterval = choosingInterval;
    }
}
