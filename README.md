# process-flow
Complicated processes are gross to code and hard to follow. This package attempts to make coding them less gross and less hard by piping application state through some standardized processes that allow you to change the flow of your application at runtime. Along with that, Process.Flow tracks which steps were taken as well as a snapshot of the workflow state after each successful step processes.

How does it work? Glad you asked!

### Basic Concepts
**Steps:**
Process.Flow is built around Steps - basic ones that process data and pass it on, and other types that do a bit more work. Each Step has a Next and Previous Step as well as settings that allow you to do stuff like automatically progress to the next step upon successful completion of the current Step. 

Right now, we have got Selectors for picking a path through your flow and Sequencers which execute a list of Steps before passing your wofklow state to the next Step.

**Workflow State:**
Workflow state (`WorkflowState<T>`) is a standardized object that holds a linked list that will store the path your process executed along with the application state you choose to pipe through the process. It also keeps a snapshot of your state at each step of the way! Just beware of circular references in your state, we will be adding support for that Tomorrow™!

**Workflow Action Factory:** 
Your Steps do stuff. How? By executing classes you define that adhere to some quasi useful interfaces. Want to get them? Register them with a `WorfklowActionFactory` and pull them out later so you can DI your factory instead of figuring out how to support multiple implementations of a single interface in your DI of choice.

More documentation to come! Sorry the initial doc is sparse, we&apos;ll get there!

