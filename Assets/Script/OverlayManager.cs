using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class OverlayManager : MonoBehaviour
{
    public static OverlayManager Instance
    {
        get; 
        private set;
    }

    public Player player;
    
    public UIDocument doc;
    
    private Label timeElapsed;
    private Label score;
    
    private Label difficulty;
    
    private Label cannonText;
    private Label missileText;
    private Label machineGunText;
    
    private ProgressBar healthBar;
    private ProgressBar overheatBar;
    
    private VisualElement cannonBlur;
    private VisualElement missileBlur;
    private VisualElement machineGunBlur;

    private VisualElement cannonContainer;
    private VisualElement missileContainer;
    private VisualElement machineGunContainer;

    private VisualElement overheatImage;

    private float borderWidth = 5;
    private Color weaponColour = Color.white;

    public float currentTime = 0;

    private float scoreCounter = 0;
    
    private float survivalTimer = 0f;
    private float survivalPointInterval = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var overlay = doc.rootVisualElement;

        healthBar = overlay.Q<ProgressBar>("HealthBar");
        overheatBar = overlay.Q<ProgressBar>("OverHeatBar");

        timeElapsed = overlay.Q<Label>("TimeElapsed");
        score = overlay.Q<Label>("Score");
        
        difficulty = overlay.Q<Label>("DifficultyText");
        
        cannonText = overlay.Q<Label>("Text1");
        missileText = overlay.Q<Label>("Text2");
        machineGunText = overlay.Q<Label>("Text3");

        cannonBlur = overlay.Q<VisualElement>("ImageBlur1");
        missileBlur = overlay.Q<VisualElement>("ImageBlur2");
        machineGunBlur = overlay.Q<VisualElement>("ImageBlur3");
        
        cannonContainer = overlay.Q<VisualElement>("ImageContainer1");
        missileContainer = overlay.Q<VisualElement>("ImageContainer2");
        machineGunContainer = overlay.Q<VisualElement>("ImageContainer3");
        
        difficulty.visible = false;

        overheatImage = overlay.Q<VisualElement>("OverheatImage");
        overheatImage.visible = false;
        
        cannonBlur.visible = false;
        missileBlur.visible = false;
        machineGunBlur.visible = false;
        SetUpWeapon(WeaponSelection.WeaponType.Cannon);
        SetUpBar(overheatBar, "Overheat: 0");
        SetUpBar(healthBar, "Health: ");
    }

    void Update()
    {
        survivalTimer += Time.deltaTime;
        if (survivalTimer >= survivalPointInterval)
        {
            AddScore(1f);
            survivalTimer = 0f;
        }
        currentTime += Time.deltaTime;

        // Convert current time to a TimeSpan for easy formatting
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);

        // Dynamically build the time string based on elapsed time
        string formattedTime;
        if (timeSpan.Hours > 0)
        {
            formattedTime = $"Time Elapsed: {timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        else if (timeSpan.Minutes > 0)
        {
            formattedTime = $"Time Elapsed: {timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        else
        {
            formattedTime = $"Time Elapsed: {timeSpan.Seconds:D2}";
        }
        
        // Display the formatted time
        timeElapsed.text = formattedTime;

        score.text = "Score: " + scoreCounter.ToString();
    }

    public void ModifyHealth(float value, float max)
    {
        float fillValue = value / max;
        healthBar.value = Mathf.Clamp(fillValue, 0, 1);
        healthBar.title = "Health: " + value;
        VisualElement fillElement = healthBar.Q(className: "unity-progress-bar__progress");
        if (fillValue > 0.5f)
        {
            fillElement.style.backgroundColor = Color.green;
        }
        else if (fillValue > 0.2f)
        {
            fillElement.style.backgroundColor = Color.yellow;
        }
        else
        {
            fillElement.style.backgroundColor = Color.red;
            StartCoroutine(Pulse(max, fillElement));
        }
    }

    private IEnumerator Pulse(float maxHealth, VisualElement e)
    {
        while (true)
        {
            if ((player.currentHealth / maxHealth) <= 0.2f)
            {
                e.style.backgroundColor = e.style.backgroundColor == Color.red ? Color.white : Color.red;
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                if ((player.currentHealth / maxHealth) > 0.5f)
                {
                    e.style.backgroundColor = Color.green;
                }
                else if ((player.currentHealth / maxHealth) > 0.2f)
                {
                    e.style.backgroundColor = Color.yellow;
                }
                yield return null;
            }
        }
    }
    
    public void AddScore(float ammount)
    {
        scoreCounter += ammount;
    }

    public void ActivateOverheat()
    {
        overheatImage.visible = true;
    }

    public void DeactivateOverheat()
    {
        overheatImage.visible = false;
    }
    
    public void UpdateWeaponCooldown(WeaponSelection.WeaponType weapon, float elapsedTime, float cooldown)
    {
        // Debug.Log("Fired: " + weapon.GetType());   
        if (WeaponSelection.WeaponType.Cannon.Equals(weapon))
        {
            cannonBlur.visible = true;
            interpolate(elapsedTime, cooldown, cannonBlur);
        }
        else if (WeaponSelection.WeaponType.Missile.Equals(weapon))
        {
            missileBlur.visible = true;
            interpolate(elapsedTime, cooldown, missileBlur);
        }
        else if (WeaponSelection.WeaponType.MachineGun.Equals(weapon))
        {
            machineGunBlur.visible = true;
            interpolate(elapsedTime, cooldown, machineGunBlur);
        }
    }

    public void interpolate(float elapsedTime, float cooldown, VisualElement e)
    {
           float interpolationMeter = Mathf.Lerp(0, e.resolvedStyle.height, elapsedTime / cooldown);
           UpdateCooldown(interpolationMeter, e.resolvedStyle.height, e);
    }
    
    private void UpdateCooldown(float currentStatus, float threshold, VisualElement e)
    {
        float fillValue = 1- (currentStatus / threshold);
        e.style.height = new Length(fillValue * 70, LengthUnit.Percent);
    }

    private void SetUpBar(ProgressBar bar, string title)
    {
        bar.title = title;
    }
    
    public void SetUpWeapon(WeaponSelection.WeaponType weapon)
    {
        if (WeaponSelection.WeaponType.Cannon.Equals(weapon))
        {
            cannonContainer.style.borderTopWidth = borderWidth;
            cannonContainer.style.borderRightWidth = borderWidth;
            cannonContainer.style.borderBottomWidth = borderWidth;
            cannonContainer.style.borderLeftWidth = borderWidth;
            cannonText.style.color = weaponColour;
        }
        else if (WeaponSelection.WeaponType.Missile.Equals(weapon))
        {
            missileContainer.style.borderTopWidth = borderWidth;
            missileContainer.style.borderRightWidth = borderWidth;
            missileContainer.style.borderBottomWidth = borderWidth;
            missileContainer.style.borderLeftWidth = borderWidth;
            missileText.style.color = weaponColour;
        }
        else if (WeaponSelection.WeaponType.MachineGun.Equals(weapon))
        {
            machineGunContainer.style.borderTopWidth = borderWidth;
            machineGunContainer.style.borderRightWidth = borderWidth;
            machineGunContainer.style.borderBottomWidth = borderWidth;
            machineGunContainer.style.borderLeftWidth = borderWidth;
            machineGunText.style.color = weaponColour;
        }
    }
    public void UpdateOverHeat(float status, float threshold, float printValue)
    {
        float fillValue = status / threshold;
        overheatBar.value = Mathf.Clamp(fillValue, 0, 1);
        overheatBar.title = "Overheat: " + Mathf.FloorToInt(printValue).ToString() + "/30";
        var fillElement = overheatBar.Q(className: "unity-progress-bar__progress");
        if (fillValue < 0.5f)
        {
            fillElement.style.backgroundColor = Color.green;
        }
        else if (fillValue < 0.8f)
        {
            fillElement.style.backgroundColor = Color.yellow;
        }
        else
        {
            fillElement.style.backgroundColor = Color.red;
        }
    }

    public void ChangeWeapon(WeaponSelection.WeaponType weapon, WeaponSelection.WeaponType previousWeapon)
    {
        if (WeaponSelection.WeaponType.Cannon.Equals(weapon))
        {
            cannonContainer.style.borderTopWidth = borderWidth;
            cannonContainer.style.borderRightWidth = borderWidth;
            cannonContainer.style.borderBottomWidth = borderWidth;
            cannonContainer.style.borderLeftWidth = borderWidth;
            cannonText.style.color = weaponColour;
        }
        else if (WeaponSelection.WeaponType.Missile.Equals(weapon))
        {
            missileContainer.style.borderTopWidth = borderWidth;
            missileContainer.style.borderRightWidth = borderWidth;
            missileContainer.style.borderBottomWidth = borderWidth;
            missileContainer.style.borderLeftWidth = borderWidth;
            missileText.style.color = weaponColour;
        }
        else if (WeaponSelection.WeaponType.MachineGun.Equals(weapon))
        {
            machineGunContainer.style.borderTopWidth = borderWidth;
            machineGunContainer.style.borderRightWidth = borderWidth;
            machineGunContainer.style.borderBottomWidth = borderWidth;
            machineGunContainer.style.borderLeftWidth = borderWidth;
            machineGunText.style.color = weaponColour;
        }
        
        if (WeaponSelection.WeaponType.Cannon.Equals(previousWeapon))
        {
            cannonContainer.style.borderTopWidth = 0;
            cannonContainer.style.borderRightWidth = 0;
            cannonContainer.style.borderBottomWidth = 0;
            cannonContainer.style.borderLeftWidth = 0;
            cannonText.style.color = Color.black;
        }
        else if (WeaponSelection.WeaponType.Missile.Equals(previousWeapon))
        {
            missileContainer.style.borderTopWidth = 0;
            missileContainer.style.borderRightWidth = 0;
            missileContainer.style.borderBottomWidth = 0;
            missileContainer.style.borderLeftWidth = 0;
            missileText.style.color = Color.black;
        }
        else if (WeaponSelection.WeaponType.MachineGun.Equals(previousWeapon))
        {
            machineGunContainer.style.borderTopWidth = 0;
            machineGunContainer.style.borderRightWidth = 0;
            machineGunContainer.style.borderBottomWidth = 0;
            machineGunContainer.style.borderLeftWidth = 0;
            machineGunText.style.color = Color.black;
        }
    }
}
