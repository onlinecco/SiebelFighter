using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {
    [SerializeField]
    private PlayerWeapon primaryWeapon;

    [SerializeField]
    private Transform WeaponHolder;

    [SerializeField]
    private string weaponLayerName = "Weapon";

    private PlayerWeapon currentWeapon;
	// Use this for initialization
	void Start ()
    {
        EquipWeapon(primaryWeapon);	
	}
	
    void EquipWeapon (PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;
        //build a class and bind it as child of weapon holder
        GameObject _weaponIns = Instantiate(_weapon.graphics, WeaponHolder.position, WeaponHolder.rotation);
        _weaponIns.transform.SetParent(WeaponHolder);
        if (isLocalPlayer)
            _weaponIns.layer = LayerMask.NameToLayer(weaponLayerName);
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

}
