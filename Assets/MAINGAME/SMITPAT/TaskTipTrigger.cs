using UnityEngine;

public class TaskTipTrigger : MonoBehaviour
{
    [TextArea]
    public string tipMessage = "Use your hands to pry open the bear trap and earn 100 points!";
    
    private bool isTaskFinished = false;

    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player") && !isTaskFinished)
        {
            TipManager.Instance.ShowTip(tipMessage, transform); // Pass this trap's transform
        }
    }

    private void OnTriggerExit(Collider other)
    {
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