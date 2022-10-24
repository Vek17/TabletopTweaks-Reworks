## [![Download zip](https://custom-icon-badges.herokuapp.com/badge/-Download-blue?style=for-the-badge&logo=download&logoColor=white "Download zip")](https://github.com/Vek17/TabletopTweaks-Reworks/releases/latest/download/TabletopTweaks-Reworks.zip) Latest Release 

## This is a TabletopTweaks mod and requires [TabletopTweaks-Core](https://github.com/Vek17/TabletopTweaks-Core/releases) [![Download zip](https://custom-icon-badges.herokuapp.com/badge/-Download-blue?style=for-the-badge&logo=download&logoColor=white "Download zip")](https://github.com/Vek17/TabletopTweaks-Core/releases/latest/download/TabletopTweaks-Core.zip)

This module provides non tabletop based tweaks to Owlcat's original mythic content including changes to some mythic classes and some mythic abilities.

Once a game is saved with this mod is enabled it will require this mod to be present to load so do not remove or disable the mod once enabled. You can however disable any feature of the mod at will without breaking saves.

All changes are configurable and can be disabled via the unity mod manager menu.

**How to install**

1. Download and install [Unity Mod Manager](https://github.com/newman55/unity-mod-manager), make sure it is at least version.
2. Run Unity Mod Manger and set it up to find Wrath of the Righteous.
3. Download the [TabletopTweaks-Core](https://github.com/Vek17/TabletopTweaks-Core/releases) mod from the releases page.
4. Download the [TabletopTweaks-MythicReworks](https://github.com/Vek17/TabletopTweaks-MythicReworks/releases) mod from the releases page.
5. Install the mods by dragging the zip file from step 3 & 4 into the Unity Mod Manager window under the Mods tab.

## Changes

* Mythic Feats
    * Mythic Sneak Attack
        * Your sneak attack dice are one size larger than normal. For example if you would normally roll d6s for sneak attacks you would roll d8s instead.

 * Mythic Abilities
    * Abundant Casting
		* Now grants two bonus casts instead of 4.
    * Elemental Barrage
        * Every time you deal elemental damage to a creature with a spell, you apply an elemental mark to it. If during the next three rounds the marked target takes elemental damage from any source with a different element, the target is dealt additional Divine damage. The damage is 1d6 per mythic rank of your character.
    * Dimensional Retribution
        * Every time you are hit by an enemy spell, you may teleport to the spellcaster as an immediate action and make an attack of opportunity.
	* Greater Enduring Spells
		* Now extends spells that last 10+ minutes instead of 5+ minutes.
* Aeon
    * Aeon Bane
        * Updates Aeon Bane's Icon.
        * Aeon Bane adds mythic rank to spell resistance checks.
        * Aeon Bane usages now scale at 2x Mythic level + Character level.
    * Aeon Improved Bane
        * Aeon Improved Bane now uses greater dispel magic rules to remove 1/4 CL buffs where CL is defined as Character Level + Mythic Rank.
    * Aeon Greater Bane
        * Aeon Greater Bane now allows you to cast swift action spells as a move action.
        * Aeon Greater Bane damage is now rolled into the main weapon attack instead of a separate instance.
        * Aeon Greater Bane now has the garentee'd auto dispel on first hit.
    * Aeon Gaze
        * Aeon Gaze now functions like Inquisitor Judgments where multiple can be activated for the same resouce usage.
        * Aeon Gaze DC has been adjusted from 15 + 2x Mythic Level to 10 + 1/2 Character Level + 2x Mythic level.
* Azata
    * Performances
        * Azata perforamnce usages now scale at Mythic level + Character level.
    * FavorableMagic
        * Favorable magic now works on reoccuring saves.
    * Incredible Might 
        * Incredible Might now grants a mythic bonus isntead of a morale bonus.
    * Zippy Magic
        * Zippy magic will now ignore some harmful effects.
* Trickster Rework
	* Trickster Progression
		* An improved trick is now learned at mythic rank 5.
		* The mythic rank 9 improved trick and the mythic rank 10 greater trick have been swapped.
	* Knowledge Arcana Tricks
		* Arcana 2
			* Enchantment list updated. This should be a buff.
		* Arcana 3
			* Enchantment list updated. This should be a buff.
	* Lore Nature Tricks
		* Upadted loot table. This should be a buff.
	* Lore Religion Tricks
		* Now work with domain zealot and qualifies for domain zealot.
		* Effective Cleric level is equal to character level and effective wisdom is  equal to mythic rank for the purposes of these domains and powers.
		* Now grants a domain spellbook with 1 slot per level for domain spells instead of spell like abilies. These now interact in all ways like spells.
	* Mobility Tricks
		* Mobility 3
			* Now works on all types of attacks including spell attacks.
	* Perception Tricks
		* Perception 1
			* You are under a constant effect of the see invisibility spell, auto detect stealthing creatures, and reroll all concealment rolls.
		* Perception 2
			* You ignore critical and sneak attack immunity, reroll all fortification checks and your critical hit range is increased by 2 with all weapons.
		* Perception 3
			* All allies within 60 feet of you gain the benifits of your perception tricks.
	* Persuasion Tricks
		* Persuasion 2
			* Enemies affected by your demoralize ability must succeed at a Will saving throw with a DC of 15 + your ranks in Persuasion, or become staggered for one round. Additionally, when you successfully demoralize an enemy they take an additional penalty to thier attack and damage rolls equal to 1 + half your mythic rank.
		* Persuasion 3
			* Enemies affected by your demoralize have a 50% chance to attack the nearest target instead of acting normally. Additionally, when you successfully demoralize an enemy they take an additional penalty to thier AC and saving equal to 1 + half your mythic rank.
	* Stealth Tricks
		* Stealth 1
			* This "invisibility" cannot be seen through by divination magic such as true seeing, see invisability, or thoughtsense.
		* Stealth 2
			* This "invisibility" cannot be seen through by divination magic such as true seeing, see invisability, or thoughtsense.
	* Trickery Tricks
		* Trickery 3
			* Your previous Trickery abilities can now be used as a swift action and at close range instead of touch range. This is in addition to gaining the save or die.
	* Use Magic Device Tricks
		* Use Magic Device 2
			* Now grants completly normal magic in addition to endless wands.
	* Bound of Possibility
		* This cloak allows Trickster to roll any skill check twice and take the best result.
* Lich
    * Deadly Magic
        * Deadly Magic is now usable 3 + half mythic rank rounds per day.
    * Decaying Touch
        * Decaying Touch has been rebuilt to prevent abuse cases but should work exactly as described now.
    * Eclipse Chill
        * Eclipse Chill is now usable 3 + half mythic rank rounds per day.
        * Eclipse Chill DC is now 10 + 1/2 character level + twice your mythic rank.
    * Tainted Sneak Attack
        * Tainted Sneak Attack DC is now 10 + 1/2 character level + twice your mythic rank.
        * Tainted Sneak Attack now works on spells.

Acknowledgments:  

-   Pathfinder Wrath of The Righteous Discord channel members
-   @Balkoth, @Narria, @edoipi, @SpaceHamster and the rest of our great Discord modding community - help.
-   PS: Wolfie's [Modding Wiki](https://github.com/WittleWolfie/OwlcatModdingWiki/wiki) is an excellent place to start if you want to start modding on your own.
-   Join our [Discord](https://discord.gg/owlcat)
