using StoryMode;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NoDragonBannerTimeout
{
  public class NoDragonBannerTimeoutSubModule : MBSubModuleBase
  {
    private const string MODULE_NAME = "No Dragon Banner Timeout";

    private StoryMode.Behaviors
        .FirstPhaseCampaignBehavior _vanillaDragonBannerBehavior = null;

    protected override void OnGameStart(Game game,
                                        IGameStarter gameStarterObject)
    {
      if (!(game.GameType is CampaignStoryMode))
        return;
      StoreDragonBannerBehavior((CampaignGameStarter) gameStarterObject);
      CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(
          this, new Action<CampaignGameStarter>(OnAfterGameLoaded));
      CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(
          this, new Action<CampaignGameStarter>(OnAfterGameLoaded));
    }

    private void OnAfterGameLoaded(CampaignGameStarter campaignGameStarter)
    {
      // original class fails the Dragon Banner quests after an amount of time
      // inside it's WeeklyTickEvent handler (and does nothing else there), so
      // unregister that and the Dragon Banner quests will never timeout.
      CampaignEvents.WeeklyTickEvent.ClearListeners(
          _vanillaDragonBannerBehavior);
    }

    private void StoreDragonBannerBehavior(CampaignGameStarter gameInitializer)
    {
      if (!(gameInitializer.CampaignBehaviors is
                IEnumerable<CampaignBehaviorBase> behaviors))
      {
        // TODO: display a long lasting notification on screen, as these
        // information messages are sometimes drowned out amongst many others.
        DisplayInfoMsgQuestModifyFailureUserShouldReport(
            "Behavior container is wrong type");
        return;
      }

      bool found = false;
      foreach (CampaignBehaviorBase behavior in behaviors)
      {
        if (behavior is StoryMode.Behaviors
                .FirstPhaseCampaignBehavior localVanillaDragonBannerBehavior)
        {
          _vanillaDragonBannerBehavior = localVanillaDragonBannerBehavior;
          found = true;
        }
      }
      if (!found)
      {
        DisplayInfoMsgQuestModifyFailureUserShouldReport(
            "Vanilla quest behavior not found");
      }
    }

    public static void DisplayInfoMsg(string msg)
    {
      InformationManager.DisplayMessage(new InformationMessage(msg));
    }

    public static void DisplayInfoMsgUserShouldReport(string msg)
    {
      DisplayInfoMsg(MODULE_NAME + ": " + msg +
                     " Please report this problem on nexus mods page.");
    }

    public static void
    DisplayInfoMsgQuestModifyFailureUserShouldReport(string msg)
    {
      DisplayInfoMsgUserShouldReport(
          "Could not alter Dragon Banner quest: " + msg + ".");
    }
  }
}
