
# Description

A King is never late, nor is he early. He arrives precisely when he means to!

Take as long as you want to form Kingdom, or do the Dragon Banner quests, Unify/Weaken Empire, Folly, or any main story quests. Now they won't fail saying "you couldn't complete the quest in 10 years", although it's actually about 835 days.

# Usage

You can add it to the game whenever you want. However, if the quests have already failed, it can't bring them back.

# Installation

You can get it from nexus: https://www.nexusmods.com/mountandblade2bannerlord/mods/370

Or, if you're building it here, during post build, the NoDragonBannerTimeout directory will be mirrored to the Bannerlord module directory (`C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\NoDragonBannerTimeout`). It may be necessary to set the property `GameFolder` in the *.csproj file if Bannerlord is installed at a non-standard path.

## Compatibility and Class Overrides:

Is compatible with [Bannerlord Community Patch](https://www.nexusmods.com/mountandblade2bannerlord/mods/186).

Is probably not compatible with other mods that alter the main storyline quests.

Alters the classes StoryMode.Behaviors.FirstPhaseCampaignBehavior in "Modules\StoryMode\bin\Win64_Shipping_Client\StoryMode.dll". Also modifies the 'time remaining' property of certain story line quests.

# Credits

This mod was only possible with these excellent guides to learning bannerlord modding:

* https://docs.bannerlordmodding.com/_tutorials/basic-csharp-mod
* https://github.com/calsev/bannerlord_smith_forever
* https://github.com/Arthur-Neto/BannerlordTemplate