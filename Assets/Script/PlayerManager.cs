using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] PlayerSettings settings;
    protected Transform myTransform { get; private set; }

    // Start is called before the first frame update

    protected virtual void Awake()
    {
        myTransform = transform;
    }
    void Start()
    {
        GameObject ball = settings.ballSelected();
        if (ball)
        {
            Instantiate(ball, myTransform).transform.localPosition = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
