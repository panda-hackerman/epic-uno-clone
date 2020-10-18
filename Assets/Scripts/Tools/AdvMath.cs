using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvacedMathStuff
{
    public static class AdvMath
    {
        //Clamps rotation using Euler angles
        public static float ClampEuler(this float value, float min, float max, float middle = 180)
        {
            if (value < middle)
            {
                if (value > max)
                    return max;
                else
                    return value;
            }
            else
            {
                float n = value - 360;

                if (n < min)
                    return min;
                else
                    return value;
            }
        }

        #region Roulette
        //Random with weighted probability, using an Array of doubles
        /*Usage: int prefabNum = Roulette(weights)
                 Instatiate(prefabList[prefabNum])*/
        public static int Roulette(double[] weights)
        {
            double totalWeight = 0;

            foreach (double item in weights)
            {
                totalWeight += item;
            }

            double randomPoint = Random.value * totalWeight;

            for (int i = 0; i < weights.Length; i++)
            {
                if (randomPoint < weights[i])
                    return i;
                else
                    randomPoint -= weights[i];
            }

            return weights.Length - 1;
        }

        //Same thing but with a list instead of an array
        public static int Roulette(List<double> weights)
        {
            double totalWeight = 0;

            foreach (double item in weights)
            {
                totalWeight += item;
            }

            double randomPoint = Random.value * totalWeight;

            for (int i = 0; i < weights.Count; i++)
            {
                if (randomPoint < weights[i])
                    return i;
                else
                    randomPoint -= weights[i];
            }

            return weights.Count - 1;
        }
        #endregion

        #region Give or take
        //Add or take a little from a variable
        /*Usage: float someVariable = 20.GiveOrTake(3);
         returns a float from 17 to 23*/

        public static float GiveOrTake(this float f, float n)
        {
            f += Random.Range(-n, n);
            return f;
        }

        public static int GiveOrTake(this int i, int n)
        {
            i += Random.Range(-n, n);
            return i;
        }
        #endregion

        #region Vector math
        //Change a single variable in a Vector3
        //Usage: transform.position = transform.position.X(1);

        public static Vector3 X(this Vector3 v, float x)
        {
            v.x = x;
            return v;
        }

        public static Vector3 Y(this Vector3 v, float y)
        {
            v.y = y;
            return v;
        }

        public static Vector3 Z(this Vector3 v, float z)
        {
            v.z = z;
            return v;
        }

        //In a Vector2
        public static Vector2 X(this Vector2 v, float x)
        {
            v.x = x;
            return v;
        }

        public static Vector2 Y(this Vector2 v, float y)
        {
            v.y = y;
            return v;
        }
        #endregion
    }
}