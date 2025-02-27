using UnityEngine;

[RequireComponent(typeof(BlockOverlap))]
public class BlockShow : MonoBehaviour
{
    public BlockOverlap ShowOverlap; 

    public VisualBlock Block;

    public void ResetPosition()
    {
        transform.position = Block.transform.position;
        transform.rotation = Block.transform.rotation;

        for(int i = 0; i <int.MaxValue; i++)
        {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y - 1,
                transform.position.z);
            if (ShowOverlap.OverlapSelf())
            {
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + 1,
                    transform.position.z);
                break;
            }
        }
    }
}