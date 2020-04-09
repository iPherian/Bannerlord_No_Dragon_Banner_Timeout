
# Description

A King is never late, nor is he early. He arrives precisely when he means to!

Now take as long as you want to form a Kingdom or do the Dragon Banner quests. They won't timeout saying "you took more than 10 years."

# Installation

You can get it from nexus: https://www.nexusmods.com/mountandblade2bannerlord/mods/370

Or, if you're building it here, during post build, the Module directory will be mirrored to the Bannerlord module directory (`C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\NoDragonBannerTimeout`). It may be necessary to set the property `GameFolder` if Bannerlord is installed at a non-standard path.

## Compatibility and Class Overrides:

You should add it to the game sometime before completion of the quest "Assemble the dragon banner". You can add it afterwards it just won't do anything.

Is probably not compatible with mods that alter the dragon banner or folly quests.

Alters the classes StoryMode.Behaviors.FirstPhaseCampaignBehavior in "Modules\StoryMode\bin\Win64_Shipping_Client\StoryMode.dll".

# Credits

This mod was only possible with these excellent guides to learning bannerlord modding:

* https://docs.bannerlordmodding.com/_tutorials/basic-csharp-mod
* https://github.com/calsev/bannerlord_smith_forever
* https://github.com/Arthur-Neto/BannerlordTemplate