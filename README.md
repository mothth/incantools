A dependency mod for Of Incandescence's regions which also happens to contain some useful features. **NOTE** - This is currently **VERY experimental** and will be filled with bugs! It is not designed for others to use at this moment, so suggestions will not be taken. Bug reports are appreciated though. Currently not much is included, but more may be added in the future.

## Features
### Palette Banks
A palette bank is a custom alternate set of palettes. It works as a solution to mod palette compatibility and convenience.
They can be used by creating a folder prefixed with `bank-` in your mod's palettes folder. (For example, our region palettes use bank-incan). To use palettes from the bank in-game, add the **IT_PaletteSettings** effect into the room and enter the bank name (EXCLUDING prefix) in the Bank field. Save and reload the room (or press T) for changes to apply.

Example - if a custom palette in present in your bank folder with the name `palette0.png`, the game will use that palette in the bank rather than the vanilla palette0. This also works for effect colours, copy the default `effectcolors.png` into your bank folder and edit as you wish. (Be sure to toggle the "Effect A/B" fields in IT_PaletteSettings)

### Custom Region Properties
IncanTools adds means of implementing custom region properties. Custom region properties go in a text file named `incan_properties.txt`, where your regular `properties.txt` would go. Formatting is the same as [properties.txt](https://rainworldmodding.miraheze.org/wiki/Properties_File). A few built-in custom properties are also available:
- `lightrodColor: <color>` - changes colour of SSLightRods
- `overrideSSMusic: <string>` - overrides the song for SSMusic 
- `SSBroken: <float>` - overrides default SSBroken value for SuperStructureFuses (default is full broken, so this is useful for iterator regions!)
- `rotImmunity: <bool` - grants a region sentient rot immunity
- **[DISABLED]** `oneWayWarp: <bool>` - forces player-created warps in the region to be single-use, like in Ancient Urban. This was once implemented, but is currently disabled. sorry!
A tutorial for implementing your own properties may be available in the future.

### Devtools Changes
IncanTools adds three new keybinds to devtools:
- **T** - reload room palette and fullscreen effects
- **Y** - reload placed objects*
- **/** - increase karma/ripple

*Y destroys and readds the entities corresponding to certain placed objects in the room. Particularly useful for placed objects that you need to reload the room to see in-game and adjust. Currently only the following objects are supported:
- LightFixture
- SuperStructureFuses
- SSLightRod
- LanternOnStick

### Other
Other minor changes / additions include:
- "No threat drone vol" slider controls volume for threat theme preview in sound page (it's nice to finally silence that when doing sounds if your region has a threat theme!)
