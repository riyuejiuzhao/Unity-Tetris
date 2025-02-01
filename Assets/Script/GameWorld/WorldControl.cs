using UnityEngine;

public partial class GameWorld : MonoBehaviour
{
    KeyCode LeftMove = KeyCode.LeftArrow;
    KeyCode RightMove = KeyCode.RightArrow;
    KeyCode RightRotate = KeyCode.UpArrow;
    KeyCode SoftDrop = KeyCode.DownArrow;
    KeyCode HardDrop = KeyCode.Space;
    KeyCode Save = KeyCode.LeftShift;
    KeyCode LeftRotate = KeyCode.LeftControl;

    void Update()
    {
        Input();
    }

    Proto.ClientAction inputBuffer = Proto.ClientAction.Nothing;

    /// <summary>
    /// 处理输入
    /// 其中软下落的实现是有点特别的，首先按下时需要下落一格
    /// 然后长按需要持续下落
    /// </summary>
    void Input()
    {
        if (UnityEngine.Input.GetKeyDown(LeftMove))
            inputBuffer = Proto.ClientAction.MoveLeft;
        else if (UnityEngine.Input.GetKeyDown(RightMove))
            inputBuffer = Proto.ClientAction.MoveRight;
        else if (UnityEngine.Input.GetKeyDown(RightRotate))
            inputBuffer = Proto.ClientAction.RotateRight;
        else if (UnityEngine.Input.GetKeyDown(LeftRotate))
            inputBuffer = Proto.ClientAction.RotateRight;
        else if (UnityEngine.Input.GetKeyDown(HardDrop))
            inputBuffer = Proto.ClientAction.HardDrop;
        else if (UnityEngine.Input.GetKeyDown(Save))
            inputBuffer = Proto.ClientAction.Save;
        else if (UnityEngine.Input.GetKeyDown(SoftDrop))
            inputBuffer = Proto.ClientAction.SoftDropDown;
        else if (UnityEngine.Input.GetKey(SoftDrop))
            inputBuffer = (inputBuffer != Proto.ClientAction.SoftDropDown) ?
                Proto.ClientAction.SoftDrop : inputBuffer;
    }

    void ClearInputBuffer()
    {
        inputBuffer = Proto.ClientAction.Nothing;
    }


}
