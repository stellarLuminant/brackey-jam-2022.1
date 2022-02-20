using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWithCustomRange : MonoBehaviour
{
    public string talkToNode = "";
    public Animator inRangeAnim;
    public string animShowName = "ShowInteraction";
    public string animHideName = "HideInteraction";

    public bool endGame;

    [Header("Range")]
    public float interactionRadius = 2.0f;
    public Collider2D optionalCollider;

    Collider2D playerCollider;

    /// Draw the range at which we'll start talking to people.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        // Flatten the sphere into a disk, which looks nicer in 2D games
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 1, 0));

        // Need to draw at position zero because we set position in the line above
        Gizmos.DrawWireSphere(Vector3.zero, interactionRadius);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(inRangeAnim, "inRangeAnim wasn't found");

        // playerCollider = FindObjectOfType<Player>().GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        IsInRange(playerCollider);
    }

    public bool IsInRange(Collider2D player)
    {
        if (optionalCollider)
        {
            return optionalCollider.IsTouching(player);
        } else
        {
            // Is in the interaction radius
            var inRange = (player.transform.position - transform.position)
                .magnitude <= interactionRadius;

            if (animShowName.Length > 0)
            {
                if (inRange)
                    inRangeAnim.Play(animShowName);
                else inRangeAnim.Play(animHideName);
            }

            return inRange;
        }
    }
}
