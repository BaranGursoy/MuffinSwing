using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandom : MonoBehaviour
{
    // Variables and constants
    public Transform first;
    public GameObject boost;
    public GameObject enemy;
    public GameObject mouth;
    public Transform player;
    Vector2 enemyFirstPosition;

    bool canDecide = true;
    bool enemySpawn = false;

    int random;
    int mouthRandom;


    int decider;
    int counter = 0;
    int enemyCounter = 0;
    int mouthCounter = 0;
    // Variables and constants are over

    private void Start()
    {
        mouthRandom = Random.Range(75, 137);
    }

    void Update()
    {
        GameObject temp = ObjectPooler.SharedInstance.GetPooledObject();
        GameObject enemyTemp = ObjectPoolerEnemy.SharedInstance.GetPooledObject();

        if (temp != null)
        {
            random = Random.Range(12, 16);
            Vector3 newPos = new Vector3(first.position.x + random, first.position.y, first.position.z);
            temp.transform.position = newPos;
            temp.transform.rotation = first.rotation;
            first = temp.transform;
            temp.SetActive(true);
        }

        if (canDecide)
        {
            decider = Random.Range(50, 100);
            canDecide = false;
        }

        if (SetScore.distance % 175 == decider) // Randomly generate boosts
        {
            float x = Random.Range(15f, 25f);
            float y = Random.Range(-3f, 2f);

            Vector2 boostPosition = new Vector2(player.position.x + x, y);

            if (counter == 0)
            {
               Instantiate(boost, boostPosition, Quaternion.identity);
               counter++;
            }   

            canDecide = true;
        }

        float x_enemy;
        float y_enemy;

        if (SetScore.distance % 200 == 0 && SetScore.distance != 0) // In every 200 score an obstacle will be spawned in a random position
        {
            x_enemy = Random.Range(15f, 25f);
            y_enemy = Random.Range(-5f, 5f);
            Mathf.Clamp(y_enemy, -4f, first.position.y - 3f);
            enemySpawn = true;
            enemyFirstPosition = new Vector2(player.position.x + x_enemy, y_enemy);
        }

        if(enemyTemp!=null && enemySpawn && enemyFirstPosition!= null) // Other 6 obstacles will be spawned in here
        {
            enemyTemp.transform.position = enemyFirstPosition;
            float x_distance = Random.Range(2f, 4f);
            float y_distance = Random.Range(2f, 3.5f);
            float new_y = enemyTemp.transform.position.y + y_distance;
            Mathf.Clamp(new_y, -4f, first.position.y - 3f);
            enemyFirstPosition = new Vector3(enemyTemp.transform.position.x + x_distance, new_y, enemyTemp.transform.position.z);
            enemyTemp.SetActive(true);
            enemyCounter++;          
        }

        if (enemyCounter == 7) // If there ara 7 obstacles on the screen, there will be no more generated
        {
            enemySpawn = false;
            StartCoroutine(Enemy());
        }

        if(counter != 0)
        {
            StartCoroutine(Boost());
        }
        
        
        if(SetScore.distance % mouthRandom == 0 && SetScore.distance != 0 && mouthCounter == 0) // Randomly spawn enemy mouthes
        {
            float x = Random.Range(35f, 40f);
            float y = Random.Range(-3.82f, 2.83f);

            Vector2 mouthPosition = new Vector2(player.position.x + x, y);

            if (mouthCounter == 0)
            {
                Instantiate(mouth, mouthPosition, Quaternion.identity);
                mouthCounter++;
                mouthRandom = Random.Range(75, 137);
            }

            StartCoroutine(Mouth());
        }
    }


    IEnumerator Enemy()
    {
        yield return new WaitForSeconds(3);
        enemyCounter = 0;
    }

    IEnumerator Boost()
    {
        yield return new WaitForSeconds(3);
        counter = 0;
    }

    IEnumerator Mouth()
    {
        yield return new WaitForSeconds(7);
        mouthCounter = 0;
    }

}


