using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject player;

    private Arena[,] arenas;

    public float distance = 5;

    [Header("Score")]
    public int winScore=20;
    public int loseScore = -10;
    public int drawScore = 0;
    public int perWinShotScore = -1;
    public int perLostShotScore = 1;
    public int perDodgedScore=2;

    [Header("Choosing")]
    public float generationTime = 20;
    public int topCount = 10;
    public int randomPreviousAdded = 10;
    public int randomsAdded=5;
    public float mutation = 0.01f;
    public double randomChaos = 0f;

    public Text avgScore;
    public Text Gen;
    private int gen = 0;

    private void Start()
    {
        SetupNew();
        Invoke("Evaluate", generationTime);
    }

    public void Evaluate() {
        List<Info> players = new List<Info>();
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                players.Add(new Info
                {
                    weights = arenas[i, j].player1.WeightMatrices,
                    score = EvaluateScore(arenas[i, j].player1, arenas[i, j].player2),
                    col = arenas[i, j].player1.color
                }) ;
                players.Add(new Info
                {
                    weights = arenas[i, j].player2.WeightMatrices,
                    score = EvaluateScore(arenas[i, j].player2, arenas[i, j].player1),
                    col = arenas[i, j].player2.color
                })  ;
            }
        }
        players.Sort(delegate (Info x, Info y)
        {
            return y.score.CompareTo(x.score);
        });

        float sum = 0;
        foreach (Info item in players)
        {
            sum += item.score;
        }
        avgScore.text = (sum / 50).ToString();

        gen++;
        Gen.text = string.Format("Gen {0}",gen);

        players.Shuffle<Info>(topCount);

        players.RemoveRange(topCount+randomPreviousAdded, 50- topCount-randomPreviousAdded);
        int n = 0;
        while (players.Count < 50-randomsAdded)
        {
            players.Add(
                new Info
                {
                    weights = players[n % topCount].weights.Mutate(mutation),
                    score = 0,
                    col = ShiftHue(players[n % topCount].col, Random.Range(-mutation, mutation))
                }
            );
            n++;
        }

        while (players.Count < 50)
        {
            players.Add(
                new Info
                {
                    weights = null,
                    score = 0,
                    col = Random.ColorHSV(0, 1, 0.8f, 1, 0.8f, 1)
                }
            );
            n++;
        }

        players.Shuffle<Info>();

        SetupExisting(players);
        Invoke("Evaluate", generationTime);
    }

    private Color ShiftHue(Color col,float amount) {
        float h;
        float s;
        float v;
        Color.RGBToHSV(col, out h, out s, out v);
        return Color.HSVToRGB((h+amount)%1,s,v);
    }

    public struct Info {
        public List<double[,]> weights;
        public int score;
        public Color col;
    }

    public void SetupNew() {
        arenas = new Arena[5, 5];
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                arenas[i, j] = new Arena(new Vector3(20 * i, 20 * j, 0), player,distance, randomChaos, Random.ColorHSV(0, 1, 0.8f, 1, 0.8f, 1), Random.ColorHSV(0, 1, 0.8f, 1, 0.8f, 1));
            }
        }
    }

    private int EvaluateScore(Brain player,Brain other) {
        int score = 0;
        if (player.lost && other.lost)
        {
            score +=drawScore;
        }
        else if (player.lost)
        {
            score += loseScore;
            score += perLostShotScore;
        }
        else if (other.lost) {
            score += winScore;
            score += perWinShotScore;
        }
        score += perDodgedScore;
        return score;
    }

    public void SetupExisting(List<Info> weights) {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Info p1 = weights[2 * (i + 5*j)];
                Info p2 = weights[2 * (i + 5*j)+1];
                arenas[i, j].ResetArena(player, distance, randomChaos, p1.col, p2.col, p1.weights, p2.weights);
            }
        }
    }

    class Arena
    {
        public Brain player1;
        public Brain player2;

        Vector3 pos;

        public Arena(Vector3 position, GameObject prefab,float distance, double randomChaos, Color col1, Color col2, List<double[,]> weights1 = null, List<double[,]> weights2 = null)
        {
            pos = position;
            ResetArena(prefab,distance,randomChaos,col1,col2,weights1,weights2);

        }

        public void ResetArena(GameObject prefab,float distance,double randomChaos, Color col1, Color col2, List<double[,]> weights1 = null, List<double[,]> weights2 = null)
        {
            if (player1 != null) Destroy(player1.gameObject);
            if (player2 != null) Destroy(player2.gameObject);

            player1 = Instantiate(prefab, pos-((distance/2)*Vector3.right), Quaternion.Euler(0, 0, -90)).GetComponent<Brain>();
            player2 = Instantiate(prefab, pos + ((distance / 2) * Vector3.right), Quaternion.Euler(0, 0, 90)).GetComponent<Brain>();

            player1.color = col1;
            player2.color = col2;

            player1.target = player2.transform;
            player2.target = player1.transform;

            if (weights1 != null)
            {
                player1.WeightMatrices = weights1;
            }
            else
            {
                player1.RandomizeWeights(3, 30,randomChaos);
            }

            if (weights2 != null)
            {
                player2.WeightMatrices = weights2;
            }
            else
            {
                player2.RandomizeWeights(3, 30,randomChaos);
            }
        }
    }
}

static class MyExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0,n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void Shuffle<T>(this IList<T> list,int startingIndex)
    {
        int n = list.Count;
        while (n > startingIndex)
        {
            n--;
            int k = Random.Range(startingIndex, n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public static List<double[,]> Mutate(this IList<double[,]> list, float amount)
    {
        System.Random rng = new System.Random(Random.Range(0,1000));

        List<double[,]> output = new List<double[,]>();
        foreach (double[,] matrix in list)
        {
            double[,] newMatrix = (double[,])matrix.Clone();
            for (int i = 0; i < newMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < newMatrix.GetLength(1); j++)
                {
                    newMatrix[i, j] = Clamp( matrix[i, j] + ((rng.NextDouble()-0.5)*2*amount));
                    
                }
            }
            output.Add(newMatrix);
        }
        return output;
    }

    static double Clamp(double input) {
        if (input > 1) return 1;
        if (input < -1) return -1;
        return input;
    }
}
