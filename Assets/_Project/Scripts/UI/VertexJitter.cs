using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Based on VertexJitter.cs example script from TextMeshPro.
/// </summary>
public class VertexJitter : MonoBehaviour
{
    [Header("Shake")]
    public float ShakeOffsetNegativeX = 0.5f;
    public float ShakeOffsetPositiveX = 0.5f;
    public float ShakeOffsetNegativeY = 0.5f;
    public float ShakeOffsetPositiveY = 0.5f;
    public float ShakeScale = 1.0f;

    [Header("Wave")]
    public float WaveSpeedMultiplier = 10f;
    public float WaveXRotationOffset = 0.5f;
    public float WaveSizeMultiplier = 1f;
    public float WaveXHeightOffset = 0.5f;

    private TMP_Text m_TextComponent;
    private bool hasTextChanged = true;

    TMP_TextInfo textInfo;
    Matrix4x4 matrix;
    TMP_MeshInfo[] cachedMeshInfo;

    [Header("Debug")]
    [SerializeField]
    bool increaseVisibleCount;
    [SerializeField]
    bool decreaseVisibleCount;
    [SerializeField]
    bool setVisibleCount;
    [SerializeField]
    bool setVisibleCountToZero;
    [SerializeField]
    int amountToVisibleCount = 1;
    [Space]
    [SerializeField]
    bool enableVisibleCountInterval = false;
    [SerializeField]
    float timeToWaitBetween = 5/60f;

    IEnumerator SetVisibleCountInterval()
    {
        while (true)
        {
            if (enableVisibleCountInterval)
            {
                m_TextComponent.maxVisibleCharacters += amountToVisibleCount;
                yield return new WaitForSeconds(timeToWaitBetween);
            } else
            {
                yield return new WaitForEndOfFrame();
            }

        }
    }

    void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        // Subscribe to event fired when text object has been regenerated.
        //TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
        StartCoroutine(SetVisibleCountInterval());
        //StartCoroutine(AnimateVertex());
    }

    void OnDisable()
    {
        //TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
    }


    void Start()
    {
        //StartCoroutine(SetVisibleCountInterval());
        //StartCoroutine(AnimateVertex());
    }

    int cachedCharacterCount;

    void Update()
    {
        #region Debug
        if (enableVisibleCountInterval)
        {
            
        }
        if (increaseVisibleCount)
        {
            m_TextComponent.maxVisibleCharacters += amountToVisibleCount;
            increaseVisibleCount = false;
        }
        if (decreaseVisibleCount)
        {
            m_TextComponent.maxVisibleCharacters -= amountToVisibleCount;
            decreaseVisibleCount = false;
        }
        if (setVisibleCount)
        {
            m_TextComponent.maxVisibleCharacters = amountToVisibleCount;
            setVisibleCount = false;
        }
        if (setVisibleCountToZero)
        {
            m_TextComponent.maxVisibleCharacters = 0;
            setVisibleCountToZero = false;
        }
        #endregion
    }

    private void LateUpdate()
    {
        // We force an update of the text object since it would only be updated at the end of the frame. Ie. before this code is executed on the first frame.
        // Alternatively, we could yield and wait until the end of the frame when the text object will be generated.
        m_TextComponent.ForceMeshUpdate();

        textInfo = m_TextComponent.textInfo;

        // Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.
        cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        // Get new copy of vertex data if the text has changed.
        if (hasTextChanged)
        {
            // Update the copy of the vertex data for the text object.
            cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

            hasTextChanged = false;
        }

        int characterCount = textInfo.characterCount;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // Skip characters that are not visible and thus have no geometry to manipulate.
            if (!charInfo.isVisible)
                continue;

            // Get the index of the material used by the current character.
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            // Get the cached vertices of the mesh used by this text element (character or sprite).
            Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

            // Determine the center point of each character at the baseline.
            //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
            // Determine the center point of each character.
            Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

            // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
            // This is needed so the matrix TRS is applied at the origin for each character.
            Vector3 offset = charMidBasline;

            Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

            // Why isn't this in a for loop? EXAMPLE CODE???
            destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
            destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
            destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
            destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

            // I understand nothing
            matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 0), Vector3.one);

            /// Alright so there's probably a bug in TMP that holds onto linkInfo even if no link exists because text change
            /// I don't know how or why, but I will just manually (but lazily) check myself if there's any link tags
            if (m_TextComponent.text.Contains("<link=\""))
            {
                // I both love and hate TextMeshPro (from GDU, from theChief)
                // https://discord.com/channels/274284473447612416/697203526530760764/860081192120352769
                foreach (var link in textInfo.linkInfo)
                {
                    //Debug.Log($"{link.linkTextfirstCharacterIndex} {link.linkTextLength} {link.GetLinkID()}");

                    // If the current character that has bene reached is not between the first aand last character index,
                    // Don't apply any text effects
                    if (i < link.linkTextfirstCharacterIndex || i > link.linkTextfirstCharacterIndex + link.linkTextLength)
                    {
                        // this might be unneeded
                        matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 0), Vector3.one);
                        //Debug.Log($"{i} | {i} < link.linkTextfirstCharacterIndex || {i} > {link.linkTextfirstCharacterIndex} + {link.linkTextLength}");
                        break;
                    }


                    switch (link.GetLinkID().ToLower())
                    {
                        case "shake":
                            Vector3 jitterOffset = new Vector3(
                                Random.Range(-ShakeOffsetNegativeX, ShakeOffsetPositiveX),
                                Random.Range(-ShakeOffsetNegativeY, ShakeOffsetPositiveY),
                                0
                            );

                            matrix = Matrix4x4.TRS(jitterOffset * ShakeScale, Quaternion.Euler(0, 0, 0), Vector3.one);
                            break;
                        case "wave":
                            // An attempt of drawing the rest of the owl: https://www.youtube.com/watch?v=FXMqUdP3XcE
                            var sinWave = new Vector3(0, Mathf.Sin(Time.time * WaveSpeedMultiplier + i * WaveXHeightOffset) * WaveSizeMultiplier, 0);
                            matrix = Matrix4x4.TRS(sinWave, Quaternion.Euler(0, 0, 0), Vector3.one);

                            // Rotational version
                            //for (int j = 0; j < 4; j++)
                            //{
                            //    var orig = destinationVertices[charInfo.vertexIndex + j];
                            //    destinationVertices[charInfo.vertexIndex + j] = orig + new Vector3(0,
                            //        Mathf.Sin(Time.time * WaveSpeedMultiplier + orig.x * WaveXRotationOffset) * WaveSizeMultiplier, 0);
                            //}
                            break;
                        default:

                            break;
                    }
                }
            }

            // I don't know how, but it changes the position of the text across the x axis and does effects if applicable?
            // Why isn't this in a for loop? EXAMPLE CODE???
            destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
            destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
            destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
            destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

            destinationVertices[vertexIndex + 0] += offset;
            destinationVertices[vertexIndex + 1] += offset;
            destinationVertices[vertexIndex + 2] += offset;
            destinationVertices[vertexIndex + 3] += offset;


            // WHAT DOES IT DO, SCHMITT? WHAT DOES IT DOOOOOOO
            //vertexAnim[i] = vertAnim;
        }

        // Push changes into meshes
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            m_TextComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    void ON_TEXT_CHANGED(Object obj)
    {
        //if (obj == m_TextComponent)
        //    hasTextChanged = true;
    }
}