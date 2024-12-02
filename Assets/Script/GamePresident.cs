using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePresident : MonoBehaviour
{
    public Transform player;
    public GameObject cloud;

    public List<GameObject> PotentialEnemies = new List<GameObject>();
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    public int maxEnemyCount;
    public int initialEnemyCount;
    
    private float spawnTimer;
    public float spawnCooldown;

    public float spawnRadiusAroundPlayer;

    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 spawnLocation = Spawn();
        player.position = spawnLocation;

        spawnLocation = Spawn();
        Instantiate(cloud, spawnLocation, Quaternion.identity);

        for (int i = 0; i <= initialEnemyCount; i++)
        {
            SpawnEnemy();
        }
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnCooldown && activeEnemies.Count < maxEnemyCount)
        {
            SpawnEnemy();
            spawnTimer = 0;
        }
    }

    void SpawnEnemy()
    {
        GameObject enemy = SelectEnemyType();
        activeEnemies.Add(enemy);
    }

    GameObject SelectEnemyType()
    {
        float proximity = 0.8f;

        GameObject enemy;

        int enemyType = Random.Range(0, PotentialEnemies.Count);
        enemy = PotentialEnemies[enemyType];

        if (enemy.GetComponent<Enemy>().type == Enemy.EnemyType.Trap)
        {
            if (Random.value < proximity)
            {
                return SpawnNearOtherEnemies(Enemy.EnemyType.Trap, enemy);
            }
        }
        else if (enemy.GetComponent<Enemy>().type == Enemy.EnemyType.Mortar ||
                 enemy.GetComponent<Enemy>().type == Enemy.EnemyType.Tank)
        {
            if (Random.value < proximity)
            {
                return SpawnNearOtherEnemies(Enemy.EnemyType.Tower, enemy);
            }
        }
        else if (enemy.GetComponent<Enemy>().type == Enemy.EnemyType.Tower)
        {
            if (Random.value < proximity)
            {
                return SpawnAwayFromEnemies(Enemy.EnemyType.Tower, enemy);
            }
        }

        return Instantiate(enemy, SpawnPointAroundPlayer(), Quaternion.identity);
    }

    GameObject SpawnAwayFromEnemies(Enemy.EnemyType type, GameObject enemy)
    {
        GameObject[] existingEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        Vector3 spawnPosition = Spawn();
        bool isValidPosition = true;

        foreach(GameObject nearbyEnemy in existingEnemy) {
            if(nearbyEnemy!= null && nearbyEnemy.GetComponent<Enemy>().type == type) {
                if(Vector3.Distance(spawnPosition, nearbyEnemy.transform.position) < 20f) {
                    isValidPosition = false;
                    break;
                }
            }
        }

        if(isValidPosition && Vector3.Distance(spawnPosition, player.position) <= spawnRadiusAroundPlayer + 15f && 
           !Physics.CheckSphere(spawnPosition, 2f, LayerMask.GetMask("Obstacles"))) {
            return Instantiate(enemy, spawnPosition, Quaternion.identity);
        }

        return Instantiate(enemy, SpawnPointAroundPlayer(), Quaternion.identity);
    }
    GameObject SpawnNearOtherEnemies(Enemy.EnemyType type, GameObject enemy)
    {
        GameObject[] existingEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> matchingEnemy = new List<GameObject>();

        foreach(GameObject nearbyEnemy in existingEnemy) {
            if(nearbyEnemy!= null && nearbyEnemy.GetComponent<Enemy>().type == type) {
                matchingEnemy.Add(nearbyEnemy);
            }
        }

        if(matchingEnemy.Count > 0) {
            GameObject targetEnemy = matchingEnemy[Random.Range(0, matchingEnemy.Count)];
            Vector3 spawnPosition = targetEnemy.transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

            if(Vector3.Distance(spawnPosition, player.position)  <= spawnRadiusAroundPlayer && 
               !Physics.CheckSphere(spawnPosition, 2f, LayerMask.GetMask("Obstacle"))) {
                return Instantiate(enemy, spawnPosition, Quaternion.identity);
            }
        }
        return Instantiate(enemy, SpawnPointAroundPlayer(), Quaternion.identity);
    }

    Vector3 SpawnPointAroundPlayer()
    {
        Vector3 spawnPoint;
        bool isValidSpawnPoint = false;

        do {
            float angle = Random.Range(0f, Mathf.PI * 2);
            float distance = Random.Range(5f, spawnRadiusAroundPlayer);
            spawnPoint = new Vector3(player.position.x + Mathf.Cos(angle) * distance, 0f, player.position.z + Mathf.Sin(angle) * distance);

            isValidSpawnPoint = !Physics.CheckSphere(spawnPoint, 2f, LayerMask.GetMask("Obstacle"));
        } while (!isValidSpawnPoint);

        return spawnPoint;   
    }

    Vector3 Spawn()
    {
        Vector3 spawnPoint;
        bool isValidSpawnPoint = false;

        do
        {
            float x = Random.Range(-50, 50);
            float z = Random.Range(-50, 50);

            spawnPoint = new Vector3(x, 0, z);

            isValidSpawnPoint = !Physics.CheckSphere(spawnPoint, 2f, LayerMask.GetMask("Obstacle"));
        } while (!isValidSpawnPoint);
        
        return spawnPoint;
    }
}
