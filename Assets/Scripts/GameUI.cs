using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;
    public RectTransform healthBar;
    public RectTransform magBar;

    public Text scoreUI;
    public Text gameOverScoreUI;
    public Text healthText;
    public Text magText;

    Player player;
    Gun playerGun;
    GunController gunController;

    void Start()
    {
        gameOverUI.SetActive(false);

        player = FindObjectOfType<Player>();
        gunController = FindObjectOfType<GunController>();

        //player.OnDeath += OnGameOver;

        Cursor.visible = false;
    }

    void Update()
    {/*
        playerGun = gunController.EquippedGun;
        float healthPercent = 0;
        float magPercent = 0;

        scoreUI.text = ScoreKeeper.score.ToString("D6");
        if (player != null)
        {
            healthPercent = player.health / player.startingHealth;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
        healthText.text = player.health + " / " + player.startingHealth;
        if (gunController != null)
        {
            magPercent = (float)playerGun.remainingBulletCount / (float)playerGun.magSize;
        }
        magBar.localScale = new Vector3(magPercent, 1, 1);
        magText.text = playerGun.remainingBulletCount + " / " + playerGun.magSize;*/
    }

    void OnGameOver()
    {
        Cursor.visible = true;
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, 0.70f), 1));
    }

    IEnumerator Fade(Color from,Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    // UI Input
    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
