
var cardFunctions = require('../cardFunctions')
var ent = require('../../entityManager');
var cardTags = require('../cardTags')

//START_OF_CARD_DATA
exports.card = {
  "type": ent.MINION,
  "cost": 3,
  "baseAttack": 3,
  "currenthHealth":0,
  "totalHealth":0,
  "baseHealth": 3,
  "set":cardTags.BASIC,
  "id":"ironfurGrizzly",
  "tags":{
      [cardTags.TAUNT]:true
  },
  "enchantments":[
    
  ]
}
//END_OF_CARD_DATA

exports.canPlay = cardFunctions.basicCanPlay

exports.attack = cardFunctions.basicAttack;

exports.canAttack = cardFunctions.basicCanAttack;

exports.takeDamage = cardFunctions.baseTakeDamage;

exports.isAlive = cardFunctions.baseIsAlive;