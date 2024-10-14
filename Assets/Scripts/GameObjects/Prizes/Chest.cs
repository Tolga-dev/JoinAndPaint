
namespace GameObjects.Prizes
{
    public class Chest : Prize
    {
        public override void FoundPlayerHit()
        {
            base.FoundPlayerHit();
            var playerController = gameManager.playerManager;
            playerController.foundChest = this;
        }
    }
}