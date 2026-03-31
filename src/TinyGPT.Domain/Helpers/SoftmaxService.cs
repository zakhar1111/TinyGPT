namespace TinyGPT.Domain.Services;

public static class SoftmaxService
{
    public static float[] Softmax(float[] x)
    {
        float max = x.Max();
        float sum = 0;
        var e = new float[x.Length];
        for (int i = 0; i < x.Length; i++)
        {
            e[i] = MathF.Exp(x[i] - max);
            sum += e[i];
        }
        for (int i = 0; i < x.Length; i++)
            e[i] /= sum;
        return e;
    }

    public static float[][] SoftmaxRows(float[][] x) => x.Select(Softmax).ToArray();
}
