namespace TinyGPT.Domain.Helpers;

public static class MatrixOperations
{
    public static float[,] RandomMatrix(int rows, int cols, Random rnd)
    {
        var m = new float[rows, cols];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                m[i, j] = (float)(rnd.NextDouble() - 0.5);
        return m;
    }
    public static float[][] MatMul(float[][] A, float[][] B)
    {
        // ================= VALIDATION =================
        if (A == null || B == null)
            throw new ArgumentNullException("Matrices cannot be null.");

        if (A.Length == 0 || B.Length == 0)
            throw new ArgumentException("Matrices cannot be empty.");

        int aRows = A.Length;
        int aCols = A[0].Length;
        int bRows = B.Length;
        int bCols = B[0].Length;

        // Matrix multiplication rule: A.cols must match B.rows
        if (aCols != bRows)
            throw new ArgumentException(
                $"Invalid dimensions: A is {aRows}x{aCols}, B is {bRows}x{bCols}");

        // ================= RESULT MATRIX =================
        var result = new float[aRows][];

        for (int i = 0; i < aRows; i++)
        {
            result[i] = new float[bCols];

            for (int j = 0; j < bCols; j++)
            {
                float sum = 0;

                // ================= CORE MULTIPLICATION =================
                // Dot product of row i (A) and column j (B)
                for (int k = 0; k < aCols; k++)
                {
                    sum += A[i][k] * B[k][j];
                }

                result[i][j] = sum;
            }
        }

        return result;
    }

    public static float[][] MatMul(float[][] A, float[,] B)
    {
        int rows = A.Length;
        int cols = B.GetLength(1);
        var result = new float[rows][];
        for (int i = 0; i < rows; i++)
        {
            result[i] = new float[cols];
            for (int j = 0; j < cols; j++)
            {
                float sum = 0;
                for (int k = 0; k < A[i].Length; k++)
                    sum += A[i][k] * B[k, j];
                result[i][j] = sum;
            }
        }
        return result;
    }

    public static float[] MatMul(float[] v, float[,] M)
    {
        int cols = M.GetLength(1);
        var result = new float[cols];
        for (int j = 0; j < cols; j++)
        {
            float sum = 0;
            for (int i = 0; i < v.Length; i++)
                sum += v[i] * M[i, j];
            result[j] = sum;
        }
        return result;
    }

    public static float[][] Transpose(float[][] A)
    {
        int rows = A.Length;
        int cols = A[0].Length;
        var result = new float[cols][];
        for (int i = 0; i < cols; i++)
        {
            result[i] = new float[rows];
            for (int j = 0; j < rows; j++)
                result[i][j] = A[j][i];
        }
        return result;
    }

    public static void Scale(float[][] m, float s)
    {
        for (int i = 0; i < m.Length; i++)
            for (int j = 0; j < m[i].Length; j++)
                m[i][j] *= s;
    }

    public static float[][] ConcatHeads(List<float[][]> heads)
    {
        int seq = heads[0].Length;
        int total = heads.Count * heads[0][0].Length;
        var result = new float[seq][];
        for (int i = 0; i < seq; i++)
        {
            result[i] = new float[total];
            int offset = 0;
            foreach (var h in heads)
            {
                Array.Copy(h[i], 0, result[i], offset, h[i].Length);
                offset += h[i].Length;
            }
        }
        return result;
    }
}

