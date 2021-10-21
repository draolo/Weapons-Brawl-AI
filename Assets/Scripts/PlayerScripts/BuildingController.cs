using UnityEngine;
using UnityEngine.Networking;


public class BuildingController : NetworkBehaviour {
   [SyncVar]
    public int zRotation; //syncvar doesn't work well
    public GameObject building;
    public bool rotationLock=true;

    private GameObject spawnPoint;
    private GameObject inBuildingObject;
    public bool isBuilding;
    private PlayerWeaponManager_Inventory Inventory;

    void Start () {
        isBuilding = false;
        spawnPoint = transform.Find("FirePointPivot/FirePoint").gameObject;
        Inventory = FindObjectOfType<PlayerWeaponManager_Inventory>();
    }

	void Update () {
        
        if (hasAuthority)
        {
            if (isBuilding && rotationLock)
            {
                inBuildingObject.transform.rotation = Quaternion.Euler(0, 0, zRotation);
            }

            if (isBuilding && Input.GetButtonDown("Switch Left"))
            {
                zRotation-=45;  
            }

            if (isBuilding && Input.GetButtonDown("Switch Right"))
            {
                zRotation+=45;
            }

            if (isBuilding && Input.GetButtonDown("Fire1") && gameObject.GetComponent<PlayerManager>().isInTurn)
            {
                CmdSpawnConstruction(zRotation); //syncvar doesn't work well
            }

            if (isBuilding)
            {
                Inventory.idleByBuilding = true;
                Inventory.CmdSetActiveWeapon(false);
            }
        }
    }

    public void ChangeBuildingStatus()
    {

        isBuilding = !isBuilding;
        if (isBuilding)
        {
            inBuildingObject = Instantiate(building, spawnPoint.transform);
            Inventory.idleByBuilding = true;
            Inventory.CmdSetActiveWeapon(false);
        }
        else
        {
            Destroy(inBuildingObject);
            Inventory.idleByBuilding = false;
            Inventory.CmdSetActiveWeapon(true);
        }
    }

    [Command]
    public void CmdSpawnConstruction(int rotation)
    {
        PlayerResourceScript resource= gameObject.GetComponent<PlayerResourceScript>();

        if (resource.UseResource(10))
        {
            CmdRealBuild(rotation);          
        }

    }
    [Command]
    public void CmdRealBuild(int rotation )
    {
        GameObject toBuild = Instantiate(building, spawnPoint.transform);
        if (rotationLock)
        {
            toBuild.transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
        toBuild.transform.parent = null;        
        NetworkServer.Spawn(toBuild);
        toBuild.GetComponent<WallScript>().RpcSetup(toBuild.transform.localScale.x, toBuild.transform.localScale.y, toBuild.transform.localScale.z);

    }
}
