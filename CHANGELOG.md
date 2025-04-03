## Version 1.4.7
* Trickster
	* Fixed some tooltiping issues with Trickster Presausion.

## Version 1.4.6
* Azata
	* Aivu now supports mounting while Huge or larger
	
## Version 1.4.5
* Azata
	* Breath weapon damage has been slightly increased as mythic ranks increase.
	* Now grows to Huge at mythic rank 9.

## Version 1.4.4
* Trickster
	* Added support for Scalykind domain.

## Version 1.4.3
* Mythic Abiltiies
	* Rework of Spell Caster's Onslaught to make it more appealing for gish builds
* Mantis Zealot
	* Replaced Deadly Fascination with Dazzling Blade work.
		* This is an intentional power down.	 

## Version 1.4.2
* Azata
	* Updated Aivu's breath weapon DC to 10 + 1/2 Hit Dice + Consitution Modifier + Azata Mythic Level
	* Updated Aivu's stat scaling again to match the Havoc Dragon progression talbe based on the Hit Dice given by Owlcat
	* Updated Aivu's weapon scaling

## Version 1.4.1
* Support for 2.3.0
* Trickster
	* Added support for new subdomains

## Version 1.3.1
* Basic patch for 2.2.0as

## Version 1.2.10
* Mythic Abilities
	* Abundant Casting
		* Now grants 4 bonus casts instead of 4.
	* Improved Abundant Casting
		* Now grants 3 bonus casts instead of 4.
	* Greater Abundant Casting
		* Now grants 2 bonus casts instead of 4.

## Version 1.2.9
* Fix for Aivu's progression happening faster than intended.

## Version 1.2.8
* Mythic Feats
	* Mythic Improved Critical
		* When you score a critical hit with your chosen weapon double the amount of weapon dice rolled.

## Version 1.2.7
* Azata
	* Aivu 
		* Stats now upgrade as mythic rank increases, these are based on tabletop Havoc dragon stat blocks.
		* Now gets natural spell resistance at mythic 5 (27), upgrading at mythic 9 (32).
		* Now gets a trip attack on her tail attack at mythic 5.
		* Tail attack gets a x3 crit multiplier at mythic 7.
		* Breath weapon has been fully reimplemented to better refelect havoc dragons.
		* Breath weapon now deals 6d10 + 2d10 per additional mythic rank. (Up from 2d10 + 2d10 per additional mythic rank).
		* Breath weapon now applies confusion on failed save at mythic rank 7+.
	* Favorable Magic
		* Now works on spells, spell-like abilities, and supernatural abiltiies only. This covers basically everything that is magic related.
	* Zippy Magic
		* Now works on spells, spell-like abilities, and supernatural abiltiies only. This covers basically everything that is magic related.
		* Can now be toggled off to make some buffing setups less annoying.

## Version 1.2.6
* Mythic Abilities
	* Archmage Armor
		* No longer works when cast from items.
		* Now requires knowing Mage Armor as a prerequisite.

## Version 1.2.5
* Aeon
	* Aeon Bane now once again supports dispel on non attack roll spells.

## Version 1.2.4
* Mythic Abilities
	* Elemental Barrage
		* Significant improvements to existing rework's trigger mechanism.

## Version 1.2.3
* Mythic Abilities
	* Greater Enduring Spells
		* Now extends spells that last 8+ minutes instead of 5+ minutes.
	* Unreleanting Assault
		* Damage now increases by 4 per round instead of 2. Damage cap increase from 10 to 20.
* Aeon
	* Improved stacking with Inquisitor Bane.
	* Improved interactions with natural attacks (now correctly applies to all nautral attacks instead of just primary hand).
	* Improved damage breakdown.

## Version 1.2.2
* Updated for 2.1.0

## Version 1.2.1a
* Added missing config options

## Version 1.2.1
* Azata
	* Update Azata spell list to contain "subpath" spell nativly.
	* Rainbow Arrows has been buffed to apply random debuffs on failed will save.
	* Songs of Steel now lasts 10 minutes/CL instead of 1 round/CL and is a communal buff.
	* Supersonic Speed now grants an additonal attack in addtion to haste
	* Life Bonding Friendship now scales with Mythic Rank instead of Charisma.
	* Zippy Magic now only works on spells.
* Bugfixes
	* Fixed issue where sonic damage was applying the wrong elemental barrage mark.

## Version 1.2.0
* Release for 2.0.0
* Mythic Feats
	* School Mastery
		* Now increases CL by 2 instead of 1 for selected school.
* Azata
	* Azata songs are now move actions instead of standard actions.
* Aeon
	* Aeon Gazes are now a swift action but function like judement allowing multiple gazes to be activated with the same action.
	* Aeon Bane is now a free action.

## Version 1.1.2
* Release for 1.4.0
* Trickster Persuasion DCs adjusted

## Version 1.1.1
* Bugfixes
	* Fixed issue with some trickster domain powers granting bonuses of the wrong type.
* Lich
	* Fixed Fear Control to use new owlcat DC logic.
* Aeon
	* Gazes now use new owlcat DC logic.

## Version 1.1.0
* Mythic Abilities
	* Abundant Casting
		* Now grants two bonus casts instead of 4.
	* Greater Enduring Spells
		* Now extends spells that last 10+ minutes instead of 5+ minutes.
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
			* Enemies affected by your demoralize ability must succeed at a Will saving throw with a DC of 10 + your ranks in Persuasion, or become staggered for one round. Additionally, when you successfully demoralize an enemy they take an additional penalty to thier attack and damage rolls equal to 1 + half your mythic rank.
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

## Version 1.0.0
This has been split and now depends on [TabletopTweaks-Core](https://github.com/Vek17/TabletopTweaks-Core/releases).

* Initial Release of the module