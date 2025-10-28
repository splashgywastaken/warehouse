using UnityEngine;
using Warehouse.Character;

public class AttackController : MonoBehaviour, IAttackController 
{
    public void StartAttack()
    {
        // Debug.Log("Attack started");
    }

    public void StopAttack()
    {
        // Debug.Log("Attack ended");
    }

    public void FixedUpdate()
    {
        
    }
}
