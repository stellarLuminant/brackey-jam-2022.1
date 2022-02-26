using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
	public Animator anim;
	Image image;
	SpriteRenderer sprite;

	public string fillAnimName = "heart refilled";
	public string fullAnimName = "heart filled";
	public string extraHeartAnimName = "extra heart";
	public string lostAnimName = "heart lost";

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

	public void Create(bool animation)
	{
		anim.SetBool("Full", true);
		if (animation)
			anim.Play(extraHeartAnimName);
	}

	public void Heal(bool animation)
	{
		//Debug.Log($"heal heart | anim {animation}");
		anim.SetBool("Full", true);
		if (animation)
			anim.Play(fillAnimName);
		else
			anim.Play(fullAnimName);
	}

	public void Hurt()
	{
		// Debug.Log("hurt heart");
		anim.SetBool("Full", false);
		anim.Play(lostAnimName);

	}

	public void Destroy()
	{
		Destroy(gameObject);
	}
}
