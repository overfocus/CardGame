﻿using UnityEngine;
using System.Collections;
using System;

public class SetMana : Rune {

    public string controllerGuid { get; set; }
    public int mana { get; set; }

    public override void Execute(Action action)
    {
        Controller playerController = EntityManager.Singelton.GetEntity(controllerGuid) as Controller;
        if(playerController == null)
        {
            Debug.Log("Problem finding controller with guid" + controllerGuid);
            action();
            return;
        }

        playerController.SetMana(mana);
        action();
    }

    public override void OnGUI()
    {

    }

}
