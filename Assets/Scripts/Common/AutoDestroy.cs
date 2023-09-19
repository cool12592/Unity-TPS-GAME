using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
	public ObjectPool myPool;
	public float destroyTime = 5;
	private float time = 0;
	[SerializeField] bool isDeactive = false;

    private void OnEnable()
    {
		time = 0f;
    }

    // Update is called once per frame
    void Update()
	{

		time += Time.deltaTime;
		if (destroyTime <= time)
		{
			if (myPool)
				myPool.ReturnObject(gameObject);
			else if (isDeactive)
				gameObject.SetActive(false);
			else
				Destroy(gameObject);
		}
	}
}
