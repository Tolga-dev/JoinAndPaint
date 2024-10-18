using UnityEngine;

public class MemberManager : MonoBehaviour
{
    public GameManager gameManager;
    
    public void AddNewMember(Transform otherTransform)
    {
        var playerManager = gameManager.playerManager;
        var recruitment = otherTransform.GetComponent<Recruitment>();
        recruitment.UnFreeze();
        recruitment.StartPlayer(playerManager);
        playerManager.members.Add(recruitment);
    }
    public void DestroyNewMember(Transform otherTransform)
    {
        var playerManager = gameManager.playerManager;
        var recruitment = otherTransform.GetComponent<Recruitment>();
        
        recruitment.Die(playerManager);
        playerManager.members.Remove(recruitment);
    }
    
}
