using System.Collections;
using UnityEngine;
using UnityEngine.UI; // ต้องใช้เพื่อนำเข้า Slider และจัดการ UI
using UnityEngine.SceneManagement; // สำหรับการจัดการ Scene (เช่น Restart)

public class PlayerStats : MonoBehaviour
{
    public float maxHealth = 50f;
    public float health;
    public Slider healthBar;  // อ้างอิงไปยัง Slider ของ Health Bar

    // ตัวแปรสำหรับ Game Over
    public GameObject gameOverScreen; // UI ของหน้าจอ Game Over
    public Button restartButton;      // ปุ่มสำหรับการ Restart

    private Animator animator;
    private bool canTakeDamage = true;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        health = maxHealth;

        // ตั้งค่า Health Bar ให้แสดงผลสุขภาพเต็มตั้งแต่เริ่มต้น
        healthBar.maxValue = maxHealth;
        healthBar.value = health;

        // ตั้งค่า Game Over Screen และ Restart Button
        gameOverScreen.SetActive(false); // ซ่อน Game Over Panel
        restartButton.gameObject.SetActive(false); // ซ่อน Restart Button
        restartButton.onClick.AddListener(RestartGame); // เชื่อมปุ่ม Restart กับฟังก์ชัน RestartGame
    }

    public void TakeDamage(float damage)
    {
        if (!canTakeDamage) { return; }

        health -= damage;
        animator.SetBool("Damage", true);

        // อัปเดต Health Bar หลังจากได้รับความเสียหาย
        UpdateHealthBar();

        Debug.Log("Player health " + health);
        if (health <= 0)
        {
            GetComponentInParent<GatherInput>().DisableControls();
            Debug.Log("Player is dead");
            GameOver(); // เรียกฟังก์ชัน GameOver เมื่อพลังชีวิตหมด
        }

        StartCoroutine(DamagePrevention());
    }

    private void UpdateHealthBar()
    {
        // อัปเดตค่า health ใน Health Bar
        healthBar.value = health;
    }

    private IEnumerator DamagePrevention()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(0.15f);

        if (health > 0)
        {
            canTakeDamage = true;
            animator.SetBool("Damage", false); // ไปยังแอนิเมชัน idle
        }
        else
        {
            animator.SetBool("Death", true); // ไปยังแอนิเมชันตาย
        }
    }

    private void GameOver()
    {
        gameOverScreen.SetActive(true); // แสดงหน้าจอ Game Over
        restartButton.gameObject.SetActive(true); // แสดงปุ่ม Restart
        Time.timeScale = 2f; // หยุดการทำงานของเกม
    }

    private void RestartGame()
    {
        Time.timeScale = 1f; // เริ่มเกมใหม่
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // โหลดฉากใหม่
    }
}
