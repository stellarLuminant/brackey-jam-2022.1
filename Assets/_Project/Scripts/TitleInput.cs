using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleInput : MonoBehaviour
{
	public string GameSceneName = "Template";
	public KeyCode[] Input_Attack = { KeyCode.Space, KeyCode.Return, KeyCode.Z };

	private bool IsAttackPressed => Utils.CheckInputsPressed(Input_Attack);

	private bool GoingToNewScene = false;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (IsAttackPressed && !GoingToNewScene)
		{
			GoingToNewScene = true;
			SceneManager.LoadScene(GameSceneName);
			UIManager.Instance.State = UIManager.GameState.Game;
		}
	}
}
