using UnityEngine;
using System.Collections.Generic;


public class WeaponCollider : MonoBehaviour {

    //Weapon Collider
    [SerializeField] public BoxCollider2D weaponCol;
    public int damage;

    public void Start()
    {
        weaponCol = GetComponent<BoxCollider2D>();
    }
    //lista de inimigos atingidos
    //lista utilizada para impedir que o mesmo ataque seja contabilizado duas vezes no mesmo inimigo

    //public List<Collider2D> hitList = new List<Collider2D>();

    private void OnTriggerEnter2D(Collider2D other)
    {        
      if (other.tag == "Enemy")
            //& !hitList.Contains(other))
      {
        var hit = other.gameObject.GetComponentInParent<EnemyHealthClass>();
        hit.HitCheck(weaponCol, damage);
      }          

    //hitList.Add(other);
    }

    //Limpa lista de inimigos
    public void ClearList()
    {
        //hitList.Clear();
    }
}
