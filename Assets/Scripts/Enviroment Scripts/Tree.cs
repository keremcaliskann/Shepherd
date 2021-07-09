using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public GameObject drop;
    Collider collider;

    public int health;
    int dropRate;
    float random;

    private void Start()
    {
        collider = GetComponent<Collider>();

        dropRate = 5;
        random = Random.Range(0.5f, 1.5f);

        transform.localScale *= random;
        health = Mathf.RoundToInt(health * random);
        dropRate = Mathf.RoundToInt(dropRate * random);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon")
        {
            transform.localScale *= 1.05f;
            Invoke("HitEffect", 0.05f);
            health--;
            if (health <= 0)
            {
                transform.LookAt(2 * transform.position - other.transform.position);
                Die();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Weapon")
        {
            transform.localScale *= 1.05f;
            Invoke("HitEffect", 0.05f);
            health--;
            if (health <= 0)
            {
                transform.LookAt(2 * transform.position - collision.transform.position);
                Die();
            }
        }

    }

    void HitEffect()
    {
        transform.localScale /= 1.05f;
    }

    void Die()
    {
        for (int i = 0; i < dropRate; i++)
        {
            Instantiate(drop, transform.position + Vector3.up * 2f * random, Quaternion.Euler(0, Random.Range(0, 360), 0));
        }
        StartCoroutine(Crash());
        Destroy(collider);
        Destroy(gameObject, 3f);
    }

    IEnumerator Crash()
    {
        float time = 0;
        while (time < 2.5f)
        {
            transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(90,transform.eulerAngles.y, transform.eulerAngles.z), Time.deltaTime*time);
            time += Time.deltaTime;
            yield return null;
        }
        StopCoroutine("Crash");
    }
}
