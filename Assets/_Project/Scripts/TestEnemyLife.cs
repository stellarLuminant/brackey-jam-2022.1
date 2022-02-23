using System;
using UnityEngine;

public class TestEnemyLife : MonoBehaviour
{
    // The current number of life points the enemy has.
    public Int32 CurrentLife = 1;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    private void ReceiveAttack()
    {
        CurrentLife = Mathf.Max(0, CurrentLife - 1);

        if (CurrentLife == 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
		if (c.gameObject.layer == Utils.PlayerAttackLayer) {
			ReceiveAttack();
			return;
		}

		// add more collision cases above
		Debug.Log($"Enemy.OnTriggerEnter2D(Collider2D): layer \"{c.gameObject.layer}\" was not processed");
    }
}
