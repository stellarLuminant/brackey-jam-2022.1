using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    public Animator anim;
    Image image;
    SpriteRenderer sprite;

    public string fullAnimName = "Full";
    public string emptyAnimName = "Empty";

    void Start()
    {
        anim = GetComponent<Animator>();
        image = GetComponent<Image>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Rough patch for animations
        image.sprite = sprite.sprite;
    }

    public void Heal(bool animation = true)
    {
        //Debug.Log($"heal heart | anim {animation}");
        anim.SetBool("full", true);
        //anim.SetBool("flash", false);

        if (animation)
        {
            anim.SetTrigger("doAnim");
        } 
        else
        {
            anim.Play(fullAnimName);
        }
    }

    public void Hurt()
    {
        Debug.Log("hurt heart");
        anim.SetBool("full", false);
        //anim.SetBool("flash", false);

        anim.SetTrigger("doAnim");
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
