using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondCameraView : MonoBehaviour
{

    [SerializeField] private Camera main;
    [SerializeField] private Camera orhogSeconds;
    [SerializeField] private GameObject MazeObject;

    private bool inView = false;

    [Space]
    [SerializeField]
    private Rigidbody player;

    [SerializeField]
    private UnityEngine.UI.Text textForRows;
    [SerializeField]
    private UnityEngine.UI.Button buttonGravity;
    public void EnableGravity()
    {
        if(player == null){
            return;
        }
        if (player.useGravity)
        {
            player.useGravity = false;
            player.gameObject.transform.position = Vector3.up;
            return;
        }
        player.useGravity = true;
        buttonGravity.interactable = false;
        StartCoroutine(waitAbit(buttonGravity));
    }
    private IEnumerator waitAbit(UnityEngine.UI.Button button){
        yield return null;
        button.interactable = true;
    }

    public void ChangeRowsCount(UnityEngine.UI.Slider single)
    {
        var script = MazeObject.GetComponent<MazeSpawner>();
        script.Rows = (int)single.value;
        textForRows.text = "" + (int)single.value;
    }

    public void ChangeTypeTo(int option)
    {
        Debug.Log(option);
        var script = MazeObject.GetComponent<MazeSpawner>();
        var Algorithm = script.Algorithm;
        switch (option)
        {
            case 0:
                Algorithm = MazeSpawner.MazeGenerationAlgorithm.PureRecursive;
                break;
            case 1:
                Algorithm = MazeSpawner.MazeGenerationAlgorithm.RandomTree;
                break;
            case 2:
                Algorithm = MazeSpawner.MazeGenerationAlgorithm.RecursiveDivision;

                break;
        }
        MazeObject.GetComponent<MazeSpawner>().Algorithm = Algorithm;
    }
    void Update()
    {
        // Right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            if (inView)
            {
                inView = false;
                main.gameObject.SetActive(true);
                return;
            }
            if (MazeObject.transform.childCount == 0)
                return;
            main.gameObject.SetActive(false);
            inView = true;
            Bounds newBounds = new Bounds();
            for (int i = 0; i < MazeObject.transform.childCount; i++)
            {
                if (MazeObject.transform.GetChild(i).name.Substring(0, 5) == "Floor")
                {
                    // Debug.Log(MazeObject.transform.GetChild(i).name.Substring(0,5));
                    newBounds.Encapsulate(MazeObject.transform.GetChild(i).GetComponent<Renderer>().bounds);
                }
            }
            orhogSeconds.transform.position = newBounds.center + new Vector3(0, 10, 0);
            orhogSeconds.orthographicSize = (newBounds.max.z - newBounds.min.z) / 2;
        }
    }


}
