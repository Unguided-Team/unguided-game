using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
  public bool jumping = false;
  public bool dashing = false;

  public bool recoilingX, recoilingY;
  public bool lookingRight;
  public bool invincible;
  
  public bool takingDamage = false; 
  public bool dead = false;

  public bool immobile = false;
  public bool canRest = false;
  public bool resting = false;

  [SerializeField] public int currentLevel; 
}
