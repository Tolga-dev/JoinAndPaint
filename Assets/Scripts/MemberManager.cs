using UnityEngine;

public class MemberManager : MonoBehaviour
{
    public GameManager gameManager;
    
    public void AddNewMember(Recruitment recruitment)// check for this function
    {
        Debug.Log("called!");
        var playerManager = gameManager.playerManager;
        recruitment.UnFreeze();
        recruitment.StartPlayer(playerManager);
        playerManager.members.Add(recruitment);
    }
    public void DestroyNewMember(Recruitment otherTransform)
    {
        var playerManager = gameManager.playerManager;
        
        playerManager.members.Remove(otherTransform);
        otherTransform.Die(playerManager);
    }
    
}
