using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Drop : MonoBehaviour
{
	Rigidbody myRigidbody;

	float forceMin = 80;
	float forceMax = 200;

	float lifetime = 10;
	float fadetime = 2;

	void Start()
	{
		myRigidbody = GetComponent<Rigidbody>();
		float force = Random.Range(forceMin, forceMax);
		myRigidbody.AddForce(transform.right * force);
		myRigidbody.AddTorque(Random.insideUnitSphere * force * 5);

		StartCoroutine(Fade());
	}

    IEnumerator Fade()
	{
		yield return new WaitForSeconds(lifetime);

		float percent = 0;
		float fadeSpeed = 1 / fadetime;
		Material mat = GetComponent<Renderer>().material;
		Color initialColour = mat.color;

		while (percent < 1)
		{
			percent += Time.deltaTime * fadeSpeed;
			mat.color = Color.Lerp(initialColour, Color.clear, percent);
			yield return null;
		}

		Destroy(gameObject);
	}
    private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.tag == "Player")
		{
			Destroy(gameObject);
		}
	}
}
