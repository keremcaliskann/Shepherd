using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
	public GameObject flashHolder;
	public Sprite[] flashSprites;
	public SpriteRenderer[] spriteRenderers;

	public ParticleSystem flashEffect;

	public float flashTime;

	void Start()
	{
		Deactivate();
	}

	public void Activate()
	{
		Destroy(Instantiate(flashEffect.gameObject, flashHolder.transform.position, flashHolder.transform.rotation) as GameObject, flashEffect.main.startLifetime.constant);
		flashHolder.SetActive(true);
		Invoke("Deactivate", flashTime);
		/*
		int flashSpriteIndex = Random.Range(0, flashSprites.Length);
		for (int i = 0; i < spriteRenderers.Length; i++)
		{
			spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
		}

		*/
	}

	public void Deactivate()
	{
		flashHolder.SetActive(false);
	}
}
