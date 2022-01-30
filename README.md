# Process.Flow

Process.Flow is intended to help with orchestrating individual processes
that operate from a shared state. It also tracks outcomes of each step
and optionally tracks state throughout the flow if you want to check which
processes ran or how each one affected your state. 

## Installation

Simply install via nuget in your favorite IDE (it's Rider), or use the command line.

```powershell
Install-Package Process.Flow -Version 0.1.0
```

## Release 0.1.0 Release Notes:
It's been too long, but I'm finally releasing version 0.1.0! What's new you ask? Fluent Steps, better
cancellation token support, publicly exposed interfaces, Dictionary support for selector steps, and... maybe more. I don't remember. 
But let's take a look at how this might shape your usage of the library.

### Better Cancellation Token Support
The `ProcessAsync` method has replaced the `Process` method in the new `AbstractStep` class.
This is a breaking change as you will need to inherit from `AbstractStep` instead of `Step` 
and change your method signature, which now has a `CancellationToken` like a real async method.
I apologize for the inconvenience of this breaking change, but I think it's a much healthier
and clearer direction for the library. I know folks who have been using it have been forced
to persist the CancellationToken in state, and that's... just weird. I've also retooled things
to hopefully protect against major breaking changes in the future. We'll see how that goes.

### Fluent Steps

Previously, in order to create a step, you often _needed_ to implement the abstract step class.
This still works, and is great for projects where your steps are leveraging dependency injection.
But there are other use cases where you may not need to implement something that requires DI, or
you may not even want to return the end state And why would you in the first place since it's persisted)
across steps? Yeah, that requirement is gone now, too. So if you just need a light touch 
implementation of steps and processes, you might end up with something like this:

```c#
// Async example
var myAsyncStep = Step<SomeStateClass>.Create(async (state, terminate, cancellationToken) =>
{
    ... Do some async work ...
});

// Sync example
var mySyncStep = Ste<SomeStateClass>.Create((state, terminate) => { .. some sync work ... });
```

Not to shabby, eh? You can still chain steps fluently with `SetNextStep()`, `Fork()`, and all
your favorites. I'm hoping this addition gives y'all some flexibility to do more with the lib.
Enjoy!

### Selector updated to support Dictionaries

Another big upgrade is that the `StepSector` now works with a `Dictionary` instead of
the basic `List` implementation which will allow you to more easily identify which step(s) you
want to execute based on your decision logic!

## Usage

### Create a WorkflowState

Each step you create will have access to the `WorkflowState`. Process.Flow uses
it to track the workflow chain (what steps you've executed) and the state. Simply
create a class for keeping state and wrap it up! 
Keep in mind that if you do async forks in your flow that operate off the
same property, you'll want to use a thread safe data structure.

```c#
public class PokeState
{
    public List<Pokemon> MyPokemon { get; set; } = new List<Pokemon>();
    public int PokeBallCount { get; set; } = 6;
    public Pokemon EncounteredMon { get; set; }
    public int DesiredPokemon { get; set; } = 6;
}
```
```c#
var pokeState = new PokeState { ... set some stuff ... };
var workflowState = new WorkflowState<PokeState> { State =  pokeState };
```

### Plan out your flow

Probably best to plan out your flow before you start. Let's say I want to represent my Pokemon
adventures and represent my simple adventure as a series of steps. What am I trying to do?
Something like this:

- Find a Pokemon
- Try to catch it (but it runs away after three attempts)
- If you ever run out of Pokeballs, stop trying to catch Pokemon and go get some more
- If you catch the desired amount of pokemon, release them to the wild, because you're a Poketarian
  (like a humanitarian, but Poke-er)
- Once you've released your Pokemon, enjoy a stroll while chewing gum

### Steps at your fingertips

Now, you can create some steps to use in your flow. Maybe they are reusable steps that
you could use for multiple flows. Maybe some of them are ones you'd want to use at 
multiple spots within your flow. Maybe you want the flexibility to quickly swap steps.
I don't know what your use case, is I'm not your dad!

We've got plenty to choose from (but probably not enough yet). Keep in mind, all steps can set
their next and previous steps and have access to `WorkflowState`!

- **Step:** Your basic _do a thing_ step.
- **Sequencer:** Do a bunch of steps in a row. This is more for organization, you can still set next 
and previous steps on all step types!
- **SingleStepSelector:** Pick a step from a list of steps and execute one based on logic
- **Fork:** Set up some steps to run in parallel
- **ForLoop:** Run a step or steps for a number of times
- **WhileLoop:** Run a step or steps until a condition is met
- **LoopStep:** A basic step, but it gets the iteration count of the currently executing loop

With these steps, you can do all you ever dreamed of, and probably less! Let's do an example (can be found in 
`ProcessFlow.Tests/PokeTests`):

### Now, let's map my desired flow to steps:

For implementations of each, you can reference [the test project](/tree/master/ProcessFlow.Tests/PokeTests/PokeSteps)

Well, let's map the flow we came up with the options we have:

```
Find a Pokemon => Step (It's simple enough... right?)
Catch a Pokemon ... WAIT! Try three times? Let's wrap this in a ForLoop!
Catch a Pokemon => LoopStep
After loop... I might need to buy Pokeballs, find a new Pokemon, or release my Pokemon... sounds like...
SingleStepSelector
Buy Pokeballs => Step
Releasing Pokemon => Step
Walk => Step
Chew Gum => Step
... Wait, I can do those at the same time ... I should probably FORK those!
```

All right, let's make that codey:


```c#
var pokeState = new PokeState();
var catchAttemptsPerMon = 3;
var settings = new StepSettings { AutoProgress = true }; // This way, each step continues to the next!

var findPokemonStep = new FindPokemonStep(stepSettings: settings); // If you're curious about the implementation, check the test project!
var catchPokemonStep = new CatchPokemonStep(stepSettings: settings); // This is a loop step, so it has access to the current iteration count

var catchLoop = new ForLoop<PokeState>(
    iterations: catchAttemptsPerMon,
    steps: new List<Step<PokeState>> { catchPokemonStep }, // We're only do one thing, but if you had more stuff to do...
    stepSettings: settings);

var getMorePokeBallsStep = new GetMorePokeBallsStep(stepSettings: settings);
var releaseEamAllStep = new ReleaseEmAllStep(stepSettings: settings);

var pickYourPathStep = new PickYourPathSelector(stepSettings: settings);

// Implementation will decide which step to take
pickYourPathStep.SetOptions(new List<Step<PokeState>> { findPokemonStep, getMorePokeBallsStep, releaseEamAllStep });

var walkStep = new WalkStep(stepSettings: settings);
var chewGumStep = new ChewGumStep(stepSettings: settings);

findPokemonStep
    .SetNextStep(catchLoop)
    .SetNextStep(pickYourPathStep);

getMorePokeBallsStep.SetNextStep(findPokemonStep);

releaseEamAllStep.Fork(name: "someName", stepSettings: settings, walkStep, chewGumStep);

await findPokemonStep.Execute(new WorkflowState<PokeState> { State = pokeState });

// Done!
```

## Road Map
Here's where my heads at and where I want to take this
- [x] ~~Create Basic Steps~~
- [x] ~~Loops~~
- [x] ~~Forks~~
- [x] ~~Add WorkflowState settings to effectively act as a default for step settings~~
- [x] ~~Better nullable support~~
- [x] ~~Fluent step creation (I don't wanna make classes all the time)~~
- [ ] Split DI into it's own library like other normal libs
- [ ] Other cool stuff? Let me know what you want! 

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
