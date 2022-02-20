using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeartContainer : MonoBehaviour
{
    [SerializeField] GameObject heartPrefab;
    [SerializeField] float waitTime = 6 / 60f;

    [Header("Debugging")]
    
    [SerializeField] List<Heart> hearts;
    [SerializeField] bool _triggerAddHeart;
    [SerializeField] bool _triggerRemoveHeart;
    [SerializeField] bool _triggerHeal;
    [SerializeField] private bool _triggerHurt;
    [SerializeField] bool _godMode;
    [SerializeField] int _howMuch = 1;

    private bool destroyedMockup;

    // NOTE: health is stored on the player object. 
    // This is just semi-mirrorred value to the player's health.
    private int _health;

    private int _previousHealth;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            // Sets previous health
            _previousHealth = _health;

            // Sets health
            if (!_godMode)
            {
                _health = Mathf.Clamp(value, 0, hearts.Count);
            }

            int changeInHealth = _health - _previousHealth;

            Debug.Log($"change: {changeInHealth} | **health: {_health}** | prev: {_previousHealth}");

            // Run heal or hurt animations
            if (changeInHealth > 0)
            {
                StartCoroutine(HealAnim(changeInHealth));
            }
            else if (changeInHealth < 0)
            {
                StartCoroutine(HurtAnim(changeInHealth));
            }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
            
    }

    void DestroyMockup()
    {
        if (!destroyedMockup)
        {
            // https://stackoverflow.com/questions/46358717/how-to-loop-through-and-destroy-all-children-of-a-game-object-in-unity
            int i = 0;

            // Array to hold all child obj
            GameObject[] allChildren = new GameObject[transform.childCount];

            // Find all child obj and store to that array
            foreach (Transform child in transform)
            {
                allChildren[i] = child.gameObject;
                i += 1;
            }

            // Now destroy them
            foreach (GameObject child in allChildren)
            {
                DestroyImmediate(child.gameObject);
            }
            destroyedMockup = true;
        }
    }

    public void Init(int heartAmount)
    {
        DestroyMockup();

        _health = heartAmount;
        _previousHealth = _health;

        if (hearts.Count < 1)
        {
            // Instanciates heart prefabs and puts them into a List
            //heartPrefabs = new List<GameObject>();
            for (int h = 0; h < Health; h++)
            {
                BaseAddHeart(false);
            }
        }
        else
        {
            Debug.Log("[HeartContainer] Heart Container already exists! Healing all");
            HealAllNoAnim();
        }
    }

    public void HealAllNoAnim()
    {
        for (int h = 0; h < hearts.Count; h++)
        {
            hearts[h].Heal(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        #if UNITY_EDITOR

        if (_triggerAddHeart)
        {
            Debug.Log($"Adding heart TRIGGER");
            StartCoroutine(AddHeart(_howMuch));
            _triggerAddHeart = false;
        }

        if (_triggerRemoveHeart)
        {
            Debug.Log($"Remove heart TRIGGER");
            StartCoroutine(RemoveHeart(_howMuch));
            _triggerRemoveHeart = false;
        }

        if (_triggerHeal)
        {
            Debug.Log($"Heal heart TRIGGER");
            Health += _howMuch;
            _triggerHeal = false;
        }

        if (_triggerHurt)
        {
            Debug.Log($"Hurt heart TRIGGER");
            Health -= _howMuch;
            _triggerHurt = false;
        }

        #endif
    }

    public IEnumerator AddHeart(int amount)
    {
        // Fill our HP to full first
        for (int i = 0; i < hearts.Count; i++)
        {
            if (!hearts[i].anim.GetBool("full"))
            {
                Health++;
                yield return new WaitForSecondsRealtime(waitTime);
            }
        }
            
        // Add hearts
        for (int i = 0; i < amount; i++)
        {
            BaseAddHeart();
            Health++;
            yield return new WaitForSecondsRealtime(waitTime);
        }
        yield return new WaitForSecondsRealtime(0);
    }

    private void BaseAddHeart(bool animation = true)
    {
        // Create heart object
        GameObject heartObject = Instantiate(heartPrefab, transform);
        Heart heart = heartObject.GetComponent<Heart>();

        // Offset calculation
        // https://www.desmos.com/calculator/y4ulyfwhq6
        // 0 hearts - 0.0 offset
        // 1 hearts - 0.5 offset
        // 2 hearts - 1.0 offset 
        float offset = 0.5f * hearts.Count;

        // Move its position
        heartObject.transform.position = new Vector3(heartObject.transform.position.x + offset,
            heartObject.transform.position.y, heartObject.transform.position.z);

        // Add it to a list of objects
        hearts.Add(heart);

        if (!animation) heart.Heal(false);

        Debug.Log("Added heart");
    }

    public IEnumerator RemoveHeart(int amount)
    {
        // Goes through the the amount of hearts to destroy
        // (Max is there so it never goes below 1)
        for (int i = 0; i < amount; i++)
        {
            // Makes sure there are no index out of range errors
            if (hearts.Count == 0) break;

            Heart heart = hearts[hearts.Count - 1];
            hearts.Remove(heart);
            heart.Destroy();

            // Removes health as _health and sets the 
            // previous health to current health to be safe;
            // Not using Health due to how it would accidentally trigger a hurt animation
            _health--;
            _previousHealth = _health;

            yield return new WaitForSecondsRealtime(waitTime);
        }
        yield return new WaitForSecondsRealtime(0);
    }

    /// <summary>
    /// Shows heal animation
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public IEnumerator HealAnim(int amountToAdd)
    {
        for (int i = 0; i < amountToAdd; i++)
        {
            // Goes through all the hearts to find an empty one
            for (int j = 0; j < hearts.Count; j++)
            {
                Heart heart = hearts[j]; 
                if (!heart.anim.GetBool("full"))
                {
                    Debug.Log($"{j} | Heart");
                    heart.Heal();
                    break;
                }
            }
            yield return new WaitForSecondsRealtime(waitTime);
        }
        yield return new WaitForSecondsRealtime(0);
    }

    /// <summary>
    /// Shows hurt animation
    /// </summary>
    /// <param name="amountToSubtract"></param>
    /// <returns></returns>
    public IEnumerator HurtAnim(int amountToSubtract)
    {
        for (int i = 0; i < Mathf.Abs(amountToSubtract); i++)
        {
            // Goes through all the hearts to find a full one
            for (int j = hearts.Count - 1; j > -1; j--)
            {
                Heart heart = hearts[j];
                if (heart.anim.GetBool("full"))
                {
                    heart.Hurt();
                    break;
                }
            }
            yield return new WaitForSecondsRealtime(waitTime);
        }
        yield return new WaitForSecondsRealtime(0);
    }
}
