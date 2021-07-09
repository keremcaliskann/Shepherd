using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    Enemy me;

    public RectTransform healthBar;
    public Text healthText;

    void Start()
    {
        me = FindObjectOfType<Enemy>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.position = me.transform.position;
            transform.rotation = Quaternion.Euler(45f, 0f, 0f);

            float healthPercent = 0;

            if (me != null)
            {
                healthPercent = me.health / me.startingHealth;
            }

            healthBar.localScale = new Vector3(healthPercent, 1, 1);
            healthText.text = me.health + " / " + me.startingHealth;
        }
    }
}
