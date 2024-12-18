# PoE Loading Screen Crash Fixer

This is a simple, light-weight, tool to fix PoE 2 causing Windows to freeze completely (or only be able to move your mouse) during a loading screen. This seems to only affect AMD X3D CPUs from what I can tell.

## How it works

People on the forums/Reddit discovered that if, after launching PoE 2, you go into Task Manager and remove CPU0/CPU1 from the process affinity for the PoE2 executable, the crashes go away.
That involves doing it every time you launch PoE2 though, and it also prevents your main 2 cores from being used at all times instead of just during loading screen.

What this tool does is detects when you're starting a loading screen (by reading client.txt), then changing the process affinity to not use CPU0/CPU1. Once the loading screen is complete
(again, discovers this by reading client.txt), it updates the affinity to be able to use those cores again.

Personally I've tried this for a little bit and the crashes have stopped, but I'd be interested to hear if others have had luck.

**If your issue is not Windows freezing on a loading screen, this tool will do nothing for you.**

## How to Use

1. Grab the release from the [releases page](https://github.com/Kapps/PoEUncrasher/releases/)
2. Unzip it
3. Run the executable there. It doesn't matter if you run it before or after PoE starts.
4. If you have issues where PoE still freezes but your computer doesn't, run the executable as an administrator instead. That should cause it to recover PoE.

## Is this Allowed?

The tool does not interact with the game client in any way that's not allowed. It reads from the client.txt file, which is explicitly allowed by GGG. Other than that, all it does is modify the process affinity exactly as if you were using the task manager. This means it should be completely fine to use.
