﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public enum CardAvatarState
{
    inHand,
    inPlay,
    inGraveyad,
    inTransit,
    waitingForTarget
}
public class CardAvatar : MonoBehaviour, entity
{

    public GameObject nameText;
    public GameObject healthText;
    public GameObject attackText;
    public GameObject costText;
    public GameObject cardText;
    public CardAvatarState cardAvatarState;


    public GameObject Front;
    public GameObject Back;

    //Used for moving the card aroubd the field
    private float speed = 1.0f;
    private float startTime;
    private float journeyLength;
    private float distCovered;
    private float fracJounery;

    protected string guid;
    protected string playerGuid;
    protected string controllerGuid;
    protected Vector3 dest;
    protected List<Text> textOfCard;
    protected Action action;

    protected Card card;

    // Use this for initialization
    void Start()
    {
        cardAvatarState = CardAvatarState.inHand;
        RuneManager.Singelton.AddListener(typeof(PlayCard), OnCardPlay);
        textOfCard = new List<Text>(GetComponentsInChildren<Text>());
        
    }

    public void Setup(Card card, string guid, string playerGuid)
    {
        Front.SetActive(true);
        this.guid = guid;
        this.card = card;
        this.playerGuid = playerGuid;
        nameText.GetComponent<Text>().text = card.GetName();
        costText.GetComponent<Text>().text = card.GetMana().ToString();
        cardText.GetComponent<Text>().text = card.GetCardText();
        if (card.GetCardType() == CardType.minion)
        {
            MinionCard mc = card as MinionCard;
            healthText.GetComponent<Text>().text = mc.GetBaseHealth().ToString();
            attackText.GetComponent<Text>().text = mc.GetBaseAttack().ToString();
        }
    }

    public void SetupBlankCard(string guid, string playerGuid)
    {
        Front.SetActive(false);
        this.guid = guid;
        card = null;
        this.playerGuid = playerGuid;
        nameText.GetComponent<Text>().text = "";
        healthText.GetComponent<Text>().text = "";
        attackText.GetComponent<Text>().text = "";
        costText.GetComponent<Text>().text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (cardAvatarState == CardAvatarState.inTransit)
        {
            Vector3 transPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 7));
            transform.position = new Vector3(transPoint.x, transPoint.y, transform.position.z);
        }
        else if(cardAvatarState == CardAvatarState.inPlay || cardAvatarState == CardAvatarState.inHand)
        {
            if(dest != transform.position)
            {
                PositionLerp();        
            }
        }
    }

    private void PositionLerp()
    {
        distCovered = (Time.time - startTime) * speed;
        fracJounery = distCovered / journeyLength;
        transform.position = Vector3.Lerp(transform.position, dest, fracJounery);
        if(transform.position == dest)
        {
            if (action != null)
            {
                Debug.Log("end", gameObject);
                action();
                action = null;
            }
        }
    }

    

    //and this
    //Entry point for the mesh to tell the whole card it is being clicked
    public void OnMouseDownOnMesh()
    {
        if (!PlayArea.Singelton.GetGameStart())
        {
            Controller ctr = EntityManager.Singelton.GetEntity(playerGuid) as Controller;
            PlayArea.Singelton.OnCardAvatarClickedForMulligan(ctr.GetCardIndexInHand(card));
        //need to add in mulligna effect later
            return;
        }
        var getcontroller = EntityManager.Singelton.GetEntity(PlayArea.Singelton.HomeGuid) as Controller;
        getcontroller.OnCardAvatarClicked(this);

        return;
    }

    public void OnTargetPicked(string targetGuid)
    {
        
    }

    //This has to be better
    public void OnMouseUpOnMesh()
    {
        if (controllerGuid != PlayArea.Singelton.HomeGuid)
            return;

        if (cardAvatarState == CardAvatarState.inTransit)
        {
            if (PlayArea.Singelton.InPlayArea(transform.position))
            {
                if(OptionsManager.Singleton.options.ContainsKey(card.GetGuid()))
                {
                    var Options = OptionsManager.Singleton.options[card.GetGuid()];
                    foreach(Option op in Options)
                    {
                         
                        if(op.GetType() == typeof(PlayCardOption))
                        {
                            OptionsManager.Singleton.PickUpOption(op);
                            break;
                        }
                        else if(op.GetType() == typeof(PlaySpellOption))
                        {
                            PlaySpellOption pso = op as PlaySpellOption;
                            if(pso.target == "-1")
                            {
                                OptionsManager.Singleton.PickUpOption(op);
                                gameObject.SetActive(false);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    cardAvatarState = CardAvatarState.inHand;
                }
            }
            else
            {
                cardAvatarState = CardAvatarState.inHand;
            }
        }
        if(cardAvatarState == CardAvatarState.inPlay)
        {
            
            return;
        }
        if (cardAvatarState == CardAvatarState.waitingForTarget)
            return;

        cardAvatarState = CardAvatarState.inHand;
    }

    public void OnCardPlay(Rune rune, System.Action action)
    {
        PlayCard pc = rune as PlayCard;
        if (card == null)
        {
            action();
            return;
        }

        if(pc.cardGuid == card.GetGuid())
        {
            cardAvatarState = CardAvatarState.inPlay;
        } 
        action();
    }

    public void SetDestination(Vector3 dest, Action action = null)
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(dest, transform.position);
        this.dest = dest;
        this.action = action;
    }

    public void SetCardText(string text)
    {
        cardText.GetComponent<Text>().text = text;
    }


    public void DeckIt()
    {
        RuneManager.Singelton.RemoveListener(typeof(PlayCard), OnCardPlay);
        gameObject.SetActive(false);
        enabled = false;
    }


    public void SetCard(Card card)
    {
        if (this.card == null)
        {
            this.card = card;
        }
    }

    public Card GetCard()
    {
        return card;
    }

    public string GetGuid()
    {
        return guid;
    }

    public void SetHealth(int amount)
    {
        healthText.GetComponent<Text>().text = amount.ToString();
    }

    public void ModifyHealth(int amount)
    {
        int current = int.Parse(healthText.GetComponent<Text>().text);
        current = current + amount;
        healthText.GetComponent<Text>().text = current.ToString() + " ";
    }

    public void SetControllerGuid(string controllerGuid)
    {
        this.controllerGuid = controllerGuid;
    }

    public string GetControllerGuid()
    {
        return controllerGuid;
    }
}
