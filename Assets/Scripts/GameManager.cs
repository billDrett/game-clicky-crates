using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<GameObject> gameObjects;
    float spawnRateSec = 1f;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] UnityEngine.UI.Button restartButton;
    [SerializeField] GameObject titleScreen;
    const float gridSizeX = 5;
    const float spawnY = -4;

    public bool isGameActive = false;
    int score = 0;

    public void StartGame(int difficulty)
    {
        isGameActive = true;
        score = 0;
        spawnRateSec /= difficulty;
        UpdateScore(0);
        titleScreen.SetActive(false);
        StartCoroutine(SpawnObject());
    }

    IEnumerator SpawnObject()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRateSec);


            int idx = Random.Range(0, gameObjects.Count);
            Vector3 spawnPos = generateUniquePos(gameObjects[idx]);
            if(spawnPos == new Vector3(0,0,0))
            {
                continue;
            }

            Instantiate(gameObjects[idx], spawnPos, transform.rotation);
        }
    }

    //Logic to avoid spawning the object at the same position as another item.
    // In case we decide to add force with an account it won't work, this logic applies only in case of
    // vertical force
    // TODO the logic still doesn't work sometimes and objects are bumping into each other.
    Vector3 generateUniquePos(GameObject generatedObject)
    {
        // We first find the ranges where an object is currently spawned based on the collider size.
        Target[] aliveTargets = FindObjectsOfType<Target>();
        List<System.Tuple<float, float>> existingTargetPos = new List<System.Tuple<float, float>>();
        foreach(var target in aliveTargets)
        {
            float xSize = target.gameObject.GetComponent<BoxCollider>().size.x;
            float ySize = target.gameObject.GetComponent<BoxCollider>().size.y;
            float maxSize = Mathf.Max(xSize, ySize);
            float posX = target.gameObject.transform.position.x;
            existingTargetPos.Add(new System.Tuple<float, float>(posX - maxSize, posX+maxSize));
        }

        if (existingTargetPos.Count == 0)
        {
            //easy just create one anywhere
            return new Vector3(Random.Range(-gridSizeX, gridSizeX), spawnY);
        }

        foreach(var value in existingTargetPos)
        {
            Debug.Log("Occupied range is " + value.Item1 + " " + value.Item2);
        }

        

        List<float> randomValues = new List<float>();
        existingTargetPos.Sort((lh, rh) => lh.Item1.CompareTo(rh.Item1));
        //Iterate through the range and check if it's possible to spawn an item in that range.
        for (int i = 0; i <= existingTargetPos.Count; i++)
        {
            float start;
            float end;
            if (i == 0)
            {
                start = -gridSizeX;
                end = existingTargetPos[i].Item1;
            }
            else if (i == existingTargetPos.Count)
            {
                start = existingTargetPos[i -1].Item2;
                end = gridSizeX;
            }
            else
            {
                start = existingTargetPos[i - 1].Item2;
                end = existingTargetPos[i].Item1;
            }

            // take into account the size of the collider
            float colliderXSize = generatedObject.GetComponent<BoxCollider>().size.x;
            float colliderYSize = generatedObject.GetComponent<BoxCollider>().size.y;

            start +=  Mathf.Max(colliderXSize, colliderYSize);
            end -= Mathf.Max(colliderXSize, colliderYSize); 

            if (end < start)
            {
                Debug.Log("Impossible to spawn object in range " + start + " " + end);
                //impossible to generate an object in that range
                continue;
            }
            randomValues.Add(Random.Range(start, end));
        }

        //impossible to create one without collision
        if(randomValues.Count == 0)
        {
            Debug.Log("Impossible to spawn object all screen is full");
            return new Vector3(0,0,0);
        }

        // from all the selected ranges choose one
        float selectedX = randomValues[Random.Range(0, randomValues.Count)];
        return new Vector3(selectedX, spawnY);
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        Debug.Log("Game OVER");
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        isGameActive = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
