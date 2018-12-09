using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AllScenes{
	Home = 0,
	Monkeys = 1,
	First = 2,
	Second = 3,
	Third = 4
}
public class MenuChoser : MonoBehaviour {


	public void ChoseHome(){
		SceneManager.LoadScene( (int) AllScenes.Home);
	}
	public void ChoseMonkey(){
		SceneManager.LoadScene((int) AllScenes.Monkeys);
	}
	public void ChoseFirst(){
		SceneManager.LoadScene((int) AllScenes.First);
	}
	public void ChoseSecond(){
		SceneManager.LoadScene((int) AllScenes.Second);
	}
	public void ChoseThird(){
		SceneManager.LoadScene((int) AllScenes.Third);
	}
}
