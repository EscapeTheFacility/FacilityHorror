![GitHub all releases](https://img.shields.io/github/downloads/ThijsNameIsTaken/EXILED-LightsOut/total) ![Discord](https://img.shields.io/discord/656673194693885975?color=critical&label=EXILED)

# EXILED-LightsOut
The old LightsOut plugin, remade for EXILED 6.

# Features
- The lights in the facility will go out on a random interval.
- You can change the interval and CASSIE messages in the settings.

(This is my first publicly released plugin so don't expect too much of it)

# Default config:
```yaml
lights_out:
  is_enabled: true
  debug: false
  # Chance percentage that the event will activate this round, between 1 and 100. Only round numbers are accepted.
  activation_chance: 50
  # Enable if you want to keep tesla gates working while in blackout. SCP-079 is always able to trigger a tesla, even during a blackout.
  # Note: the tesla will still make the humming noise.
  keep_tesla_enabled: false
  # The CASSIE message to broadcast.
  cassie_message: generator .g3 malfunction detected .g4 .g3 .g3 .g4
  # Enable if the ding-dong at the start of the CASSIE announcement should be played.
  cassie_sound_alarm: true
  cassie_display_subtitles: true
  cassie_subtitles: Generator malfunction detected.
  # Enable if the lights should turn off after the full CASSIE announecment has played. If disabled, the lights will turn off while the announcement is playing.
  cassie_wait_for_toggle: true
  # Time in seconds after round start that the blackout cannot occur. Note: the min_random_interval will be added to this.
  # For example: min_start_offset: 60 and min_random_interval: 40 means that from the round start at least 100 seconds will pass before the lights event can occur.
  # After the first blackout the event cannot occur for only the time specified in min_random_interval.
  min_start_offset: 200
  # Randomized time interval in seconds between blackouts.
  min_random_interval: 200
  max_random_interval: 350
  # Randomized duration in seconds of each blackout.
  min_random_blackout_time: 90
  max_random_blackout_time: 180

```
