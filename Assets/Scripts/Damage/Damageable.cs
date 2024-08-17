using UnityEngine;

namespace Damage
{
    public class Damageable : DamageComponent
    {
        [SerializeField] private int health = 100;
        [SerializeField] private string tag = "Player";
        public void ApplyDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Debug.Log("Taken damage");
                Destroy(gameObject);
            }
        }
        /**
         * To differentiate enemy or player
         */
        public string GetTag()
        {
            return tag;
        }
    }
}