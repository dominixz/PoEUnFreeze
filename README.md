# Update
When back from loading screen instead of turn all cores back I just change to turn off one core because it is better for make it not freeze when loading shader etc.



# PoE Loading Screen Crash Fixer

This is a simple, light-weight, tool to fix PoE 2 causing Windows to freeze completely (or only be able to move your mouse) during a loading screen. This seems to mostly, but not only, affect AMD X3D CPUs.

## Table of Contents

1. [How it works](#how-it-works)
2. [How to Use](#how-to-use)
3. [FAQ](#faq)
   - [Does this work for Path of Exile 1?](#does-this-work-for-path-of-exile-1)
   - [Is this allowed?](#is-this-allowed)
   - [How do I know if it's working?](#how-do-i-know-if-its-working)

## How it works

People on the forums/Reddit discovered that if, after launching PoE 2, you go into Task Manager and remove CPU0/CPU1 from the process affinity for the PoE2 executable, the crashes go away.
That involves doing it every time you launch PoE2 though, and it also prevents your main 2 cores from being used at all times instead of just during loading screen.

What this tool does is detects when you're starting a loading screen (by reading client.txt), then changing the process affinity to not use CPU0/CPU1. Once the loading screen is complete
(again, discovers this by reading client.txt), it updates the affinity to be able to use those cores again.

This causes Windows to no longer freeze up, but it looks like for some people the game still freezes instead. I've found that when this happens, if you set the PoE process priority to realtime,
you can make PoE recover and unfreeze. For people who still experience the game (but not their PC) freezing after using this tool, run it as an administrator and it will attempt to recover
PoE by setting the process to realtime when it detects the game freezes. After the game recovers, it reverts it back to normal priority.

**If your issue is not Windows freezing on a loading screen, this tool will do nothing for you.**

## How to Use

1. Grab the release from the [releases page](https://github.com/Kapps/PoEUncrasher/releases/)
2. Unzip it
3. Run the executable there. It doesn't matter if you run it before or after PoE starts.
4. If you have issues where PoE still freezes but your computer doesn't, run the executable as an administrator instead. That should enable it to recover PoE.

## FAQ

### Does this work for Path of Exile 1?
As of v1.4.0, it works for PoE 1, however the program can only support one game at a time. If you launch the program, then launch PoE 2, and you want to play PoE 1, you will need to close PoE 2, restart the program, and then launch PoE 1.

### Is this allowed?
The tool does not interact with the game client in any way that's not allowed. It reads from the client.txt file, which is explicitly allowed by GGG. Other than that, all it does is modify the process affinity/priority exactly as if you were using the task manager. This means it should be completely fine to use.

### How do I know if it's working?
You should see something like this.
![image](https://github.com/user-attachments/assets/03e95e15-bbb1-4ebd-8c2f-345c37ab4643)
