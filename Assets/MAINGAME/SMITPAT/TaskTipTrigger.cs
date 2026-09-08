using UnityEngine;

public class TaskTipTrigger : MonoBehaviour
{
    [TextArea]
    public string tipMessage = "Use your hands to pry open the bear trap and earn 100 points!";
    
    private bool isTaskFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        // Make sure your VR Player/Hands have the tag "Player"
        if (other.CompareTag("Player") && !isTaskFinished)
        {
            TipManager.Instance.ShowTip(tipMessage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Optional: Hide the tip if the player walks away without finishing the task
        if (other.CompareTag("Player") && !isTaskFinished)
        {
            TipManager.Instance.HideTip();
        }
    }

    // Call this from your existing interaction scripts when the task is done
    public void CompleteTask()
    {
        isTaskFinished = true;
        TipManager.Instance.HideTip();
    }
}