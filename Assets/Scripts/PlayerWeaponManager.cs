using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    private IWeaponBase[] _weapons = new IWeaponBase[1];
    int _selectedWeapon = 0;
    private void Awake() {
        _weapons = GetComponentsInChildren<IWeaponBase>();
    }
    public void AttackPressed(bool pressed) {
        _weapons[_selectedWeapon].Attack(pressed);
    }

    public void SecondaryPressed() {

    }
}
