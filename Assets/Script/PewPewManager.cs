using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PewPewManager : MonoBehaviour
{
    private enum shotType
    {
        cannon,
        missile,
        machineGun
    }
    private WeaponSelection _weaponManager;

    public GameObject cannonProjectilePrefab;
    public Transform Cannon;

    public GameObject missileProjectilePrefab;
    public Transform MissileLauncher;

    public GameObject machineGunProjectilePrefab;
    public Transform machineGun;

    private float cannonCooldown;
    private float missileCooldown;
    private float machineGunCooldown;

    private float cannonLastFireTime = 0f; // Track the last time the cannon was fired
    private float missileLastFireTime = 0f;
    private float machineGunLastFireTime = 0f;
    private float overheatMeter;
    public float overheatThreshold;
    private bool overheated;
    public float overheatCooldown;
    private float overheatTime;
    
    private Camera camera;

    private shotType shot;
    
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        _weaponManager = FindObjectOfType<WeaponSelection>();
        cannonCooldown = cannonProjectilePrefab.GetComponent<Projectile>().fireRate;
        missileCooldown = missileProjectilePrefab.GetComponent<Projectile>().fireRate;
        machineGunCooldown = machineGunProjectilePrefab.GetComponent<Projectile>().fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_weaponManager.currentWeapon == WeaponSelection.WeaponType.Missile && MissileLauncher != null)
            {
                if (Time.time > missileLastFireTime + missileCooldown)
                {
                    StartCoroutine(FireMissile());
                    missileLastFireTime = Time.time;
                }
            }
            else if (_weaponManager.currentWeapon == WeaponSelection.WeaponType.Cannon && Cannon != null)
            {
                if (Time.time > cannonLastFireTime + cannonCooldown)
                {
                    camera.GetComponent<CameraFollow>().TriggerShark();
                    FireCannon();
                    cannonLastFireTime = Time.time;
                }
            }
        }
        
        // Cooldown UI for Missile Launcher
        if (Time.time < missileLastFireTime + missileCooldown && missileLastFireTime != 0)
        {
            float timeSinceLastMissileShot = Time.time - missileLastFireTime;
            OverlayManager.Instance.UpdateWeaponCooldown(WeaponSelection.WeaponType.Missile, timeSinceLastMissileShot, missileCooldown);
        }
        if (Time.time < cannonLastFireTime + cannonCooldown && cannonLastFireTime != 0) // Given its time to reload cannon
        {
            float timeSinceLastCannonShot = Time.time - cannonLastFireTime;
            OverlayManager.Instance.UpdateWeaponCooldown(WeaponSelection.WeaponType.Cannon, timeSinceLastCannonShot, cannonCooldown);
        }
        
        if (Input.GetMouseButton(0))
        {
            if (_weaponManager.currentWeapon == WeaponSelection.WeaponType.MachineGun && machineGun != null)
            {
                if (Time.time > machineGunLastFireTime + machineGunCooldown)
                {
                    if (!overheated)
                    {
                        FireMachineGun();
                        overheatMeter ++;
                        OverlayManager.Instance.UpdateOverHeat(overheatMeter, overheatThreshold, overheatMeter);
                        if (overheatMeter > overheatThreshold)
                        {
                            Debug.Log("Overheating...");
                            overheated = true;
                            overheatTime = Time.time;
                            OverlayManager.Instance.ActivateOverheat();
                        }
                    }
                    machineGunLastFireTime = Time.time;
                }
            }
        }
        if (overheated)
        {
            // Calculate how much time has passed since overheat started
            float elapsedTimeSinceOverheat = Time.time - overheatTime;

            // Linearly interpolate overheatMeter from overheatThreshold to 0 over the cooldown duration
            overheatMeter = Mathf.Lerp(overheatThreshold, 0, elapsedTimeSinceOverheat / overheatCooldown);
            OverlayManager.Instance.UpdateOverHeat((overheatCooldown + overheatTime - Time.time), overheatCooldown, overheatMeter);
            if (Time.time > overheatCooldown + overheatTime)
            {
                overheated = false;
                overheatMeter = 0;
                OverlayManager.Instance.DeactivateOverheat();
            }
        }
        else
        {
            if (Time.time < machineGunLastFireTime + machineGunCooldown && machineGunLastFireTime != 0) // Given its time to reload
            {
                float timeSinceLastShot = Time.time - machineGunLastFireTime;
                OverlayManager.Instance.UpdateWeaponCooldown(WeaponSelection.WeaponType.MachineGun, timeSinceLastShot, machineGunCooldown);
            }
            // Check if more than 1 second has passed since the last machine gun shot and overheat is not active
            if (Time.time - machineGunLastFireTime > 1f)
            {
                // Gradually reduce the overheatMeter if the cooldown period has passed
                if (overheatMeter > 0)
                {
                    float elapsedTimeSinceLastShot = Time.time - machineGunLastFireTime + 1;
                    overheatMeter = Mathf.Lerp(overheatMeter, 0, elapsedTimeSinceLastShot / (overheatCooldown * overheatMeter) / overheatThreshold);
                    OverlayManager.Instance.UpdateOverHeat(overheatMeter, overheatThreshold, overheatMeter);
                    if (Time.time > overheatCooldown + machineGunLastFireTime)
                    {
                        overheated = false;
                        overheatMeter = 0;
                    }
                }
            }
        }
    }
    
    private void FireCannon()
    {
        Quaternion adjustedRotation = Cannon.rotation * Quaternion.Euler(-90f, 0f, 0f);
        // Instantiate the projectile at the cannon's position, facing the turret's forward direction
        GameObject cannonShell = Instantiate(cannonProjectilePrefab, Cannon.position - Cannon.forward * 1.2f,
            adjustedRotation);

        // Set the speed of the shell
        Rigidbody rb = cannonShell.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = -transform.forward * cannonShell.GetComponent<Projectile>().projectileSpeed; // Shoot in the direction the turret is facing
        }
    }
    
    private void FireMachineGun()
    {
        Quaternion newRotation = machineGun.rotation * Quaternion.Euler(-90f, 0f, 0f);
        // Instantiate the projectile at the cannon's position, facing the turret's forward direction
        GameObject bullet = Instantiate(machineGunProjectilePrefab, machineGun.position - machineGun.forward * 0.6f,
            newRotation);

        // Set the speed of the shell
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = -transform.forward * bullet.GetComponent<Projectile>().projectileSpeed; // Shoot in the direction the turret is facing
        }
    }

    IEnumerator  FireMissile()
    {
        for (int i = 0; i < 4; i++)
        {
            camera.GetComponent<CameraFollow>().TriggerShark();
            Quaternion rotation = MissileLauncher.rotation * Quaternion.Euler(-35f, 0f, 0f);
            GameObject missile = Instantiate(missileProjectilePrefab, MissileLauncher.position - MissileLauncher.forward * 1.2f, rotation);

            Rigidbody rb = missile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = -transform.forward * missile.GetComponent<Projectile>().projectileSpeed; // Shoot in the direction the turret is facing
            }

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);
    }

}
