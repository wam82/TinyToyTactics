using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelection : MonoBehaviour
{
    public enum WeaponType
    {
        Cannon,
        Missile,
        MachineGun
    }

    public WeaponType currentWeapon;

    void Start()
    {
        // Set the default weapon to Cannon (Weapon 1)
        currentWeapon = WeaponType.Cannon;
        Debug.Log("Weapon Selected: " + currentWeapon.ToString());
    }

    void Update()
    {
        HandleWeaponSelection();
    }

    private void HandleWeaponSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && currentWeapon != WeaponType.Cannon)
        {
            SelectWeapon(WeaponType.Cannon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && currentWeapon != WeaponType.Missile)
        {
            SelectWeapon(WeaponType.Missile);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && currentWeapon != WeaponType.MachineGun)
        {
            SelectWeapon(WeaponType.MachineGun);
        }
    }

    private void SelectWeapon(WeaponType weapon)
    {
        OverlayManager.Instance.ChangeWeapon(weapon, currentWeapon);
        currentWeapon = weapon;
        // Log the selected weapon to the console
        Debug.Log("Weapon Selected: " + currentWeapon.ToString());
    }
}
