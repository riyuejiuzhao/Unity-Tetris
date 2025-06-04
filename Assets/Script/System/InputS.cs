using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto;

// 输入控制
public static class InputS
{
    public static KeyCode LeftMove = KeyCode.LeftArrow;
    public static KeyCode RightMove = KeyCode.RightArrow;
    public static KeyCode RightRotate = KeyCode.UpArrow;
    public static KeyCode SoftDrop = KeyCode.DownArrow;
    public static KeyCode HardDrop = KeyCode.Space;
    public static KeyCode Save = KeyCode.LeftShift;
    public static KeyCode LeftRotate = KeyCode.LeftControl;

    public static OperationType Input()
    {
        if (UnityEngine.Input.GetKeyDown(LeftMove))
            return OperationType.LeftMove;
        else if (UnityEngine.Input.GetKeyDown(RightMove))
            return OperationType.RightMove;
        else if (UnityEngine.Input.GetKeyDown(RightRotate))
            return OperationType.RightRotate;
        else if (UnityEngine.Input.GetKeyDown(LeftRotate))
            return OperationType.LeftRotate;
        else if (UnityEngine.Input.GetKeyDown(HardDrop))
            return OperationType.HardDrop;
        else if (UnityEngine.Input.GetKeyDown(Save))
            return OperationType.Save;
        else if (UnityEngine.Input.GetKeyDown(SoftDrop))
            return OperationType.SoftDrop;
        else if (UnityEngine.Input.GetKeyUp(SoftDrop))
            return OperationType.SoftDropEnd;
        return OperationType.OpUnknown;
    }

    public static void ActionProcess(BlockMap map, OperationType action)
    {
        if (map.NowBlock == null)
            return;
        switch (action)
        {
            case OperationType.LeftMove:
                BlockMoveS.MoveLeft(map.NowBlock);
                break;
            case OperationType.RightMove:
                BlockMoveS.MoveRight(map.NowBlock);
                break;
            case OperationType.LeftRotate:
                BlockRotationS.LeftRotate(map.NowBlock);
                break;
            case OperationType.RightRotate:
                BlockRotationS.RightRotate(map.NowBlock);
                break;
            case OperationType.HardDrop:
                BlockDownS.HardDown(map.NowBlock);
                break;
            case OperationType.SoftDrop:
                BlockDownS.SoftDownStart(map.NowBlock);
                break;
            case OperationType.SoftDropEnd:
                BlockDownS.SoftDownEnd(map.NowBlock);
                break;
            default:
                Debug.LogError("不支持或不识别的操作");
                break;
        }
    }
}
