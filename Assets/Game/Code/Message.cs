using UnityEngine;
using System.Collections;

public class Message : MonoBehaviour
{
    [TextArea]
    public string Text;

    public void OnCollisionStay(Collision col)
    {
        if (col.collider.CompareTag("Player") && GameManager.instance.State != GameManager.GameState.MESSAGEBOX)
        {
            if (!Text.Equals(string.Empty) && Input.GetKeyDown(KeyCode.Space))
                GameManager.instance.ShowText(Text);
        }
    }
}
