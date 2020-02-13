using System;

public static class RandomManager
{
    static Random rng = null;
    public static int GetNext(int min, int max)
    {
        if (rng == null)
            rng = new Random();
        return rng.Next(min, max);
    }
}