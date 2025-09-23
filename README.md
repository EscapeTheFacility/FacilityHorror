![GitHub all releases](https://img.shields.io/github/downloads/EscapeTheFacility/FacilityHorror/total)

# FacilityHorror
The old LightsOut plugin, remade for LabAPI, and including more features!

### Features
- Customizable "round events" that trigger a certain game event on a random interval.
- You can set up as much events as you want with as much combinations as you want.

### Known issues:
- When disabling tesla gates in an event, the tesla gate will not activate (as is intended), however the idle "buzz" will still activate when in range of the tesla.

### Config

```
debug: false
# Chance percentage that the event system will activate this round, between 1 and 100
activation_chance: 50
# Time in seconds after round start that the event system is disabled. Note: the min_random_interval will be added to this.
# For example: min_start_offset: 60 and min_random_interval: 40 means that from the round start at least 100 seconds will pass before first event can occur.
# After the first event cannot occur for only the time specified in min_random_interval.
min_start_offset: 60
# Randomized time interval in seconds between events
min_random_interval: 40
max_random_interval: 400
# List of events
event_list:
  NormalLightsOut:
  # Randomized duration in seconds of this event. Not used when lights_out is false and tesla_enabled is true
    min_random_event_duration: 10
    max_random_event_duration: 100
    # Should the event run while the warhead is active (might cause issues)
    run_when_warhead: false
    # Should CASSIE broadcast a message when an event starts
    cassie_text_enabled: true
    # The text CASSIE needs to broadcast if cassie_text_enabled is true
    cassie_text: Generator malfunction detected
    # Should CASSIE subtitles be shown
    cassie_show_subtitles: true
    # Should CASSIE play the announcement jingle before announcing
    cassie_run_jingle: true
    # Should CASSIE wait before the announcement is finished before executing settings below
    cassie_wait_for_message: false
    # Should tesla gates be enabled during the event. SCP-079 will always be able to use tesla, even during an event
    tesla_enabled: false
    # Should the lights go out during the event
    lights_out: true
    # Should the doors lock during the event
    doors_locked: false
  DoorRestart:
    min_random_event_duration: 10
    max_random_event_duration: 100
    run_when_warhead: false
    cassie_text_enabled: true
    cassie_text: Door system malfunction detected
    cassie_show_subtitles: true
    cassie_run_jingle: true
    cassie_wait_for_message: false
    tesla_enabled: true
    lights_out: false
    doors_locked: true
  CassieBleep:
    min_random_event_duration: 10
    max_random_event_duration: 15
    run_when_warhead: true
    cassie_text_enabled: true
    cassie_text: pitch_0.15 .g7
    cassie_show_subtitles: false
    cassie_run_jingle: false
    cassie_wait_for_message: false
    tesla_enabled: true
    lights_out: false
    doors_locked: false
```