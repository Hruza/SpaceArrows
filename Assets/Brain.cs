using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public List<double[,]> WeightMatrices;

    public Color color;

    public Transform target;

    public Movement move;

    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().color = color;
    }

    const int inputSize = 7;

    const int outpuSize = 3;


    public void RandomizeWeights(int horizontal, int vertical,double chaos)
    {
        WeightMatrices = new List<double[,]>();
        System.Random rng = new System.Random(Random.Range(0, 1000));
        double[,] A = new double[vertical, inputSize+1];

        for (int i = 0; i < A.GetLength(0); i++)
        {
            for (int j = 0; j < A.GetLength(1); j++)
            {
                A[i, j] = (rng.NextDouble() - 0.5) * 2 * chaos;
            }
        }
        WeightMatrices.Add(A);


        for (int k = 0; k < horizontal; k++)
        {
            A = new double[vertical, vertical+1];

            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    A[i, j] = (rng.NextDouble() - 0.5) * 2 * chaos;
                }
            }

            WeightMatrices.Add(A);
        }

        A = new double[outpuSize, vertical+1];

        for (int i = 0; i < A.GetLength(0); i++)
        {
            for (int j = 0; j < A.GetLength(1); j++)
            {
                A[i, j] = (rng.NextDouble() - 0.5) * 2 * chaos;
            }
        }
        WeightMatrices.Add(A);
    }

    public double[] values;
    void FixedUpdate()
    {
        values = GetInput();

        foreach (double[,] A in WeightMatrices)
        {
            values = MatrixMultiply(A, values);
        }

        move.Forward((float)values[0]);
        move.Rot((float)values[1]);
        if (values[2] > 0) move.Shoot();
    }




    Vector2 vel;
    double[] GetInput()
    {
        double[] input = new double[inputSize];

        Vector3 dir = target.position - transform.position;

        input[0] = dir.magnitude*0.2f;

        input[1] = Vector2.SignedAngle(transform.up, dir)/180;

        dir = new Vector3(UIcontroller.Round20(transform.position.x), UIcontroller.Round20(transform.position.y),0) - transform.position;
        input[2] = dir.magnitude * 0.2f;
        input[3] = Vector2.SignedAngle(transform.up, dir) / 180;


        vel = target.GetComponent<Rigidbody2D>().velocity;
        input[4] = Vector2.Dot(vel, dir)*0.2f;
        input[5] = Vector2.Dot(vel, Vector2.Perpendicular(dir) ) * 0.2f;

        input[6] = move.lastShoot;

        return input;
    }

    public static double[] MatrixMultiply(double[,] A, double[] x)
    {
        int rA = A.GetLength(0);
        int cA = A.GetLength(1);
        int n = x.Length;
        double temp = 0;
        double[] b = new double[rA];
        if (cA != n+1)
        {
            Debug.LogError("matrix can't be multiplied !!");
            return null;
        }
        else
        {
            for (int i = 0; i < rA; i++)
            {
                temp = 0;
                for (int k = 0; k < cA-1; k++)
                {
                    temp += A[i, k] * x[k];
                }
                temp += A[i, cA-1];
                b[i] = temp;
            }
            return b;
        }
    }

    public bool lost = false;
    public void Damage()
    {
        lost = true;
        //enabled=false;
        gameObject.SetActive(false);
        //target.GetComponent<Brain>().enabled = false ;
        target.gameObject.SetActive(false);
    }
    
}