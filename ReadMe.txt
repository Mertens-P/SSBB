This repository offers a template where students can experiment with different forms of network programming.

Make sure the project compiles in your IDE (recommended: Visual Studio 2019)

1 Command line arguments:
The behavior of the project can be altered using different console arguments.
In visual studio console arguments can be set in Project -> Properties -> Debug
Alternatively, you can make a shortcut to the exe and add the console arguments in the path to the original exe.

For this project, console arguments start with a "-", and are followed by the actual command. different arguments are split by spaces.
Example:
-Client -Server

Some arguments can take parameters, in this case any number of parameters can be added after the console argument separated by spaces.
Example:
-Client -Server -BotType AT_TestBot_1

Common command line argument combinations:
Stand alone server:
-Server
Local server:
-Client -Server
Joining a server over local host:
-Client
Joining a remote server:
-Client -Ip 8.8.8.8 -Port 4805
Joining as a bot
-Client -BotType AT_TestBot_1
Local server and create new clients for all bot types
-Client -Server -AllBots

2 Assignments:
- Add interpolation to GameObjects/Components/CharacterReplicator.cs, so remote characters move smoothly.
- Use a lag simulator (eg clumsy https://jagt.github.io/clumsy/download.html ) to simulate latency, packet loss and jitter and see what kind of effect that has on the remote players.
- Add extrapolation to minimize the effects of latency, packet loss and jitter.
- Create different implementations which maximize accuracy or speed.
- Change game mechanics and see what kind of effect that has on simulations. (eg, different movement speeds, limited rotation speed, velocity ramp up/down, slower server tick etc)
- Add more bots which perform the type of behavior you want to optimize your project for.
- Add more bots which perform the the worst case type of behavior you want to optimize your project for, or even behavior that should be impossible.

3 Bots:
The project comes with several bots which are useful for prototyping network implementations.

AT_TestBot_1:
Moves back and forth between 2 points. useful to see if interpolation is smooth. The instant turn around at the ends can be used as a worst case check for extrapolation.
AT_RaceBot_1, AT_RaceBot_2:
These bots start moving at the same time with the same speed, based on the system clock. Can be used to see how the simulation differs per client due to latency.
AT_TestBot_Circle,
Moves in a near perfect circle. Depending on implementation of interpolation/extrapolation the pattern will become less and less of a circle.
AT_RandomBot
Moves in an unpredictable random pattern. Use to check how accurate the simulation is with unexpected behavior.