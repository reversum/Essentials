# Essentials is a powerful, all-in-one SCP: Secret Laboratory server plugin that enhances gameplay

# Plugin Features Overview

## 1. Debugging

Option to enable or disable debug logs for troubleshooting and development purposes.

## 2. SCP-3114 Integration

Adds SCP-3114 to the normal SCP pool for spawning.

Configurable spawn chance for SCP-3114.

Player preference rating for SCP-3114 from 1 to 5.

Minimum SCP count required in the round to enable SCP-3114 spawning.

## 3. Deadman Sequence Settings

Option to disable the automatic healing feature during the Deadman Sequence.

Allows editing the Deadman Sequence auto-heal percentage, which increases player HP by a configurable fraction.

## 4. Join Message

Enable or disable a welcome message when a player joins.

Customizable join message text.

Configurable duration for how long the join message is displayed.

Option to show the join message as a broadcast or a hint.

## 5. Round End Features

Enable Friendly Fire during the round end phase.

Show round statistics as a broadcast message at the end of the round.

Customizable round stats message with placeholders such as most human kills, most SCP kills, fastest escape, and most SCP items used.

Custom messages if no player qualifies for a statistic.

Localization options for specific words in the stats message ("with", "in").

## 6. Advertisements

Enable or disable random advert messages displayed periodically.

Configurable list of advert messages.

Time interval between advert messages.

Duration the advert messages are displayed.

Option to display adverts as broadcasts or hints.

## 7. Escape Mechanics

Option to keep player effects after escaping.

Custom escape role mappings when a player is cuffed, allowing role transformations between specific roles during escape.

## 8. Role Customization

Set custom starting health values for specific roles, e.g., SCP-3114 with 2500 HP.

## 9. Spawn Items

Define spawn items for specific roles.

If the item is a gun, the assigned number specifies the amount of ammo to spawn with it.

## 10. SCP-914 Custom Recipes

Customize SCP-914 machine outputs by defining the resulting item for each input item per knob setting.

## 11. SCP-914 Role Mappings

Define custom role transformations for SCP-914 knob settings, allowing specific roles to be transformed into other roles at certain knob settings.

## 12. SCP-914 Teleport Rooms

Define custom teleport rooms for SCP-914 knob settings, specifying which rooms players can be teleported to when SCP-914 is used.

## 13. Custom Info Display

Option to show player health in custom informational displays.

## 14. Weapon Damage Multipliers

Enable or disable weapon damage multipliers.

Option to allow humans to affect SCP damage dealt (more or less damage).

Default damage multipliers based on hit location (headshot, body, limb).

Weapon-specific multipliers overriding default multipliers per hit location for specific guns like GunCOM15 and GunE11SR.

## 15. Candy Mechanics

Set chance for players to get a special pink candy (percentage chance, 1 = 1%).

Set maximum number of candies a player can pick up before losing their hands.

Configurable message when a player grabs a candy, including duration and whether the message is shown as a hint or broadcast.

## 16. Spawnwave Visual Effects

Change the surface lighting color when spawn waves or mini waves spawn.

Set how long the spawn wave light color lasts.

Define the colors used for MTF (blueish) and Chaos Insurgency (greenish) spawn wave lighting.

## 17. Cleanup System

Enable or disable automatic cleanup of items on the map.

Customizable broadcast messages for cleanup start and alert (60 seconds warning before cleanup).

Configure broadcast duration for cleanup messages.

Set cleanup intervals for items and ragdolls.

Define which categories of items should be cleaned up (e.g., Armor, Radio).

Specify rooms where item cleanup should ignore items (e.g., SCP-914 room).

Enable ragdoll cleanup and configure its cleanup delay.

# Example Config

```
# Enable debug logs.
debug: false
# Adds SCP-3114 to the normal SCP-Pool.
enable_s_c_p3114: true
# Sets the chance of being SCP-3114.
s_c_p3114_spawn_chance: 0.25
# Sets the player preference of being SCP-3114 (1 to 5).
s_c_p3114_player_preference: 3
# Sets the min count of SCPs to spawn SCP-3114.
s_c_p3114_s_c_p_count: 0
# Disables the Deadman Sequence Autoheal.
disable_dead_man_sequence_auto_heal: false
# Enables editing Deadman Sequence Autoheal settings.
edit_dead_man_sequence_settings: false
# Deadman Sequence Autoheal Percentage (adds % to the hp of the players).
dead_man_sequence_auto_heal_percentage: 0.25
# Enable the join message.
should_show_join_message: false
# Set the join message text.
join_message: Welcome! Enjoy your stay!
# Set the duration of the join message.
join_message_duration: 5
# Set the join message to be a broadcast, otherwise it will be a hint.
join_message_as_broadcast: true
# Enable FriendlyFire at the round end.
enable_friendly_fire_at_round_end: true
# Show round stats at the end of the round as a broadcast.
show_round_stats: true
# Set the round stats message (You can use the following placeholders: {mosthumankills}, {mostscpkills}, {mostdamagetoscps}, {fastestescape}, {scpitemsused})
round_stats_message: >-
  <size=26>[<color=#F82A2A>R</color><color=#E62724>o</color><color=#D4241E>u</color><color=#C32119>n</color><color=#B11E13>d</color> <color=#A71C10>S</color><color=#AE1C13>t</color><color=#B51C16>a</color><color=#BC1C19>t</color><color=#C31C1B>s</color>]</size>

  <size=24>Player with the most kills (as human): <color="red">{mosthumankills}</color></size>

  <size=24>Player with the most kills (as SCP): <color="red">{mostscpkills}</color></size>

  <size=24>Player that used the most SCP items: <color="green">{scpitemsused}</color></size>
# Set the message if nobody has this stat
round_stats_no_one_message: Nobody (sad)...
# Set the message to your own language
round_stats_with: with
# Set the message to your own language
round_stats_in: in
# Enable random advert messages.
enable_advert_messages: false
# Advert messages to be displayed randomly.
advert_messages:
- Did you know that SCPs are dangerous?
- '<color=green>Info: You can upgrade items in SCP-914!</color>'
# Time in seconds between different advert messages.
advert_message_wait_time: 300
# Set the duration of the advert message.
advert_message_duration: 5
# Show advert messages as a broadcast instead of a hint.
advert_message_as_broadcast: true
# Keep Effects when escaped.
keep_effects_after_escape: true
# Define Custom Escape Scenarios (while being cuffed).
custom_cuffed_escapes:
  NtfPrivate: ChaosRifleman
  NtfSergeant: ChaosMarauder
  NtfSpecialist: ChaosMarauder
  NtfCaptain: ChaosRepressor
  ChaosRifleman: NtfPrivate
  ChaosMarauder: NtfSergeant
  ChaosRepressor: NtfCaptain
# Set custom start health to Roles on spawn.
custom_role_health:
  Scp3114: 2500
# Define spawnitems for roles. If item is a gun amount will be used as ammo.
spawn_items:
  Scientist:
    Adrenaline: 1
# Define custom output for each input item, per knob setting.
custom_item_recipes:
  Coin:
    Rough: Adrenaline
    Coarse: Adrenaline
    OneToOne: Adrenaline
    Fine: Adrenaline
    VeryFine: Adrenaline
# Define custom role output per input role, per knob setting.
custom_role_mappings:
  ClassD:
    Fine: Scientist
# Custom teleport rooms per knob setting.
teleport_rooms:
  Rough:
  - HczArmory
  - HczMicroHID
  Coarse:
  - LczGreenhouse
  - LczGlassroom
# Shows the Player-Health in the Custom-Info.
show_health_in_custom_info: false
# Enables/Disables the weapon damage multiplier.
enable_damage_multiplier: false
# Enables/Disables if a human should also give the SCPs more/less damage.
damage_s_c_ps: false
# Default multipliers used if no weapon-specific multipliers are defined.
default_multipliers:
  headshot: 1
  body: 1
  limb: 1
# Weapon-specific damage multipliers per hitbox.
weapon_multipliers:
  GunCOM15:
    headshot: 3
    body: 1
    limb: 0.800000012
  GunE11SR:
    headshot: 3
    body: 1
    limb: 0.800000012
# Set the chance to get a pink candy (1 = 1%). Set 0 to disable.
pink_candy_chance: 1
# Sets the maximum number of candies a player can pick up before the player is loosing his hands.
max_candy: 2
# Add a message when a players grab a candy. Leave empty to disable it!
candy_message: <color=yellow>You grabbed a {type} candy!</color>
# Set the duration of the grab candy message.
candy_message_duration: 2
# Show the candy message as a hint instead of a broadcast.
candy_message_as_hint: true
# Change Surface light when a spawnwave/miniwave spawns.
spawnwave_color_lights: true
# Sets the time how long the lights are changed into the team colors.
spawnwave_color_lights_duration: 15
# MTF Spawnwave Light color.
m_t_f_light_color:
  r: 0.17
  g: 0.34
  b: 0.85
  a: 1
# CI Spawnwave Light color.
c_i_light_color:
  r: 0.75
  g: 0.83
  b: 0.32
  a: 1
# Enable the cleanup item timer.
cleanup_items: true
# Set the message that is sent when cleaning up items.
cleanup_items_broadcast: >-
  <size=26>[<color=#FF0000>I</color><color=#F70303>t</color><color=#EF0707>e</color><color=#E70B0B>m</color> <color=#D71212>C</color><color=#CF1616>l</color><color=#C71D14>e</color><color=#BF2413>a</color><color=#B72B12>n</color><color=#AF3210>u</color><color=#A7390F>p</color>]</size>

  <size=24><color=#228B22>Deleted <color=#ADFF2F>{itemcount}</color> Items around the map.</color></size>
# Set the message that is sent when alerting that cleaning up items is happening in 60 seconds.
cleanup_items_alert_broadcast: >-
  <size=26>[<color=#FF0000>I</color><color=#F70303>t</color><color=#EF0707>e</color><color=#E70B0B>m</color> <color=#D71212>C</color><color=#CF1616>l</color><color=#C71D14>e</color><color=#BF2413>a</color><color=#B72B12>n</color><color=#AF3210>u</color><color=#A7390F>p</color>]</size>

  <size=24><color=#228B22>Warning! Pickups will be deleted in 60 seconds.</color></size>
# Cleanup broadcast duration.
cleanup_items_broadcast_duration: 5
# Cleanup every items X seconds.
cleanup_items_delay: 300
# Select the items that should be deleted. (Keycard, Medical, Radio, Firearm, Grenade, SCPItem, SpecialWeapon, Ammo, Armor)
cleanup_item_types:
- Armor
- Radio
# Select the rooms that should be ignored while deleting items.
cleanup_ignore_item_rooms:
- Lcz914
# Enable the cleanup ragdolls timer.
cleanup_ragdolls: true
# Cleanup every ragdoll X seconds.
cleanup_ragdolls_delay: 120
```
