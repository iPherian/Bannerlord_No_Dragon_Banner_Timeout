using StoryMode;
using StoryMode.StoryModePhases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
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
        string errorMsg = MakeMsgQuestModifyFailureUserShouldReport(
            "Behavior container is wrong type");
        LogError(errorMsg);
        throw new NoDragonBannerTimeoutLoadError(errorMsg);
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
        string errorMsg = MakeMsgQuestModifyFailureUserShouldReport(
            "Vanilla quest behavior not found");
        LogError(errorMsg);
        throw new NoDragonBannerTimeoutNotLoadedAfterStoryModeException(
            errorMsg);
      }
    }

    private static bool IsFirstStoryPhase()
    {
      return FirstPhase.Instance != null && SecondPhase.Instance == null;
    }

    private static void DisableStoryDirectTimeoutAllQuests()
    {
      foreach (QuestBase quest in Campaign.Current.QuestManager.Quests
                   .ToList<QuestBase>())
      {
        DisableStoryDirectTimeoutIfNeeded(quest);
      }
    }

    private void InitDisableStoryDirectTimeout(Game game)
    {
      if (!(game.GameType is StoryMode.CampaignStoryMode))
        return;

      CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(
          this, new Action<QuestBase>(OnQuestStarted));
      DisableStoryDirectTimeoutAllQuests();
      // We try to be compatible with Community Patch mod, which also modifies
      // these quests in it's initialization, and we try not to rely on the
      // order of loading, so redo the quest changes on the next hourly
      // tick as well.
      new NextHourlyTickListener(
          (quest) => { DisableStoryDirectTimeoutAllQuests(); }, null);
    }

    private static void DisableStoryDirectTimeoutIfNeeded(QuestBase quest)
    {
      if (!IsFirstStoryPhase() || !quest.IsSpecialQuest || !quest.IsOngoing)
        return;

      CampaignTime newDueTime = CampaignTime.Never;
      if (quest.QuestDueTime != newDueTime)
      {
        quest.ChangeQuestDueTime(newDueTime);
        ShowNotification(
            new TextObject("{=!}Quest time remaining was updated."),
            "event:/ui/notification/quest_update");
      }
    }

    private static void ShowNotification(TextObject message,
                                         string soundEventPath = "")
    {
      InformationManager.AddQuickInformation(message, 0, null, soundEventPath);
    }

    class NextHourlyTickListener
    {
      private Action<QuestBase> _func;
      private QuestBase _quest;
      private int _count = 0;
      private int _max;

      public NextHourlyTickListener(Action<QuestBase> func, QuestBase quest,
                                    int max = 1)
      {
        _func = func;
        _quest = quest;
        _max = max;
        CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this,
                                                                OnHourlyTick);
      }

      private void OnHourlyTick()
      {
        _count++;
        if (_count >= _max)
        {
          _func.Invoke(_quest);
          CampaignEvents.HourlyTickEvent.ClearListeners(this);
        }
      }
    };

    private void OnQuestStarted(QuestBase quest)
    {
      // defer our updates until the next next hourlytick, because we are trying
      // to undo the changes made by e.g. BannerlordCommunityPatch, who does
      // them in the next hourly tick after this event.
      new NextHourlyTickListener(
          new Action<QuestBase>(DisableStoryDirectTimeoutIfNeeded), quest, 2);
    }

    public override void OnGameInitializationFinished(Game game)
    {
      base.OnGameInitializationFinished(game);

      InitDisableStoryDirectTimeout(game);
    }

    public static string
    MakeMsgQuestModifyFailureUserShouldReport(string baseMsg) =>
        MakeMsgUserShouldReport(
            "Could not disable timeout on early story quests: " + baseMsg +
            ".");

    public static string MakeMsgUserShouldReport(string baseMsg) =>
        MODULE_NAME + ": " + baseMsg +
        " Please report this problem on nexus mods page.";

    public static void DisplayInfoMsg(string msg)
    {
      InformationManager.DisplayMessage(new InformationMessage(msg));
    }

    public static void LogError(string msg)
    {
      DisplayInfoMsg(msg);
      Debugger.Log(3, nameof(NoDragonBannerTimeoutSubModule), msg + "\n");
      MBDebug.ConsolePrint(msg);
    }

    public static void DisplayInfoMsgUserShouldReport(string msg)
    {
      LogError(MakeMsgUserShouldReport(msg));
    }

    public static void
    DisplayInfoMsgQuestModifyFailureUserShouldReport(string msg)
    {
      LogError(MakeMsgQuestModifyFailureUserShouldReport(msg));
    }
  }

  public class NoDragonBannerTimeoutNotLoadedAfterStoryModeException : Exception
  {
    public NoDragonBannerTimeoutNotLoadedAfterStoryModeException() {}

    public NoDragonBannerTimeoutNotLoadedAfterStoryModeException(string msg)
        : base(MakeMsg(msg))
    {
    }

    public NoDragonBannerTimeoutNotLoadedAfterStoryModeException(
        string msg, Exception inner)
        : base(MakeMsg(msg), inner)
    {
    }

    private static string MakeMsg(string msg) =>
        msg +
        " This is usually because this mod wasn't loaded after StoryMode.";
  }

  public class NoDragonBannerTimeoutLoadError : Exception
  {
    public NoDragonBannerTimeoutLoadError() {}

    public NoDragonBannerTimeoutLoadError(string message) : base(message) {}

    public NoDragonBannerTimeoutLoadError(string message, Exception inner)
        : base(message, inner)
    {
    }
  }
}
