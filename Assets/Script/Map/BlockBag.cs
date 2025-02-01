using UnityEngine;

public class BlockBag
{
    int[] blocks = new int[7];
    int top = 7;

    public int Pop()
    {
        if (top == blocks.Length)
        {
            top = 0;
            RandomRest();
        }
        int rt = blocks[top];
        top += 1;
        return rt;
    }

    private void RandomRest()
    {
        for (int i = 0; i < 7; i++)
            blocks[i] = i;
        for (int i = 0; i < blocks.Length; i++)
        {
            var index = Random.Range(i, blocks.Length);
            (blocks[i], blocks[index]) = (blocks[index], blocks[i]);
        }
    }
}
