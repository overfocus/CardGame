
var cardFunctions = require('../cardFunctions')
var ent = require('../../entityManager');
var cardTags = require('../cardTags')

//START_OF_CARD_DATA
exports.card = {
  "type": ent.MINION,
  "cost": 4,
  "baseAttack": 2,
  "currentHealth":0,
  "totalHealth":0,
  "baseHealth": 4,
  "set":cardTags.BASIC,
  "id":"gnomishInventor",
  "tags":{
      [cardTags.BATTLE_CRY]:true
  },
  "enchantments":[
    
  ]
}
//END_OF_CARD_DATA

exports.onBattleCry = function (card, controller, state) {
    
    var deck = controller.deck;
    if(deck.length <= 0)
    {
        return null;   
    }
    var index = Math.floor(Math.random() * deck.length);
    
    var dealCard = {
        "runeType":"DealCard",
        "controllerGuid":controller.guid,
        "cardGuid":deck[index].cardGuid
    }
    rune.executeRune(dealCard, state);
}

exports.canPlay = cardFunctions.basicCanPlay

exports.attack = cardFunctions.basicAttack;

exports.canAttack = cardFunctions.basicCanAttack;

exports.takeDamage = cardFunctions.baseTakeDamage;

exports.isAlive = cardFunctions.baseIsAlive;