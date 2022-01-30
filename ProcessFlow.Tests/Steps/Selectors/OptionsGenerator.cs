using System.Collections.Generic;
using ProcessFlow.Steps.Base;
using ProcessFlow.Tests.TestUtils;

namespace ProcessFlow.Tests.Steps.Selectors
{
    public static class OptionsGenerator
    {
        public static List<IStep<SimpleWorkflowState>> GetOptionsList() 
        {
            var firstStep = Step<SimpleWorkflowState>.Create(state => state.MyInteger++, name: StepConstants.FIRST_STEP_NAME);
            var secondStep = Step<SimpleWorkflowState>.Create(state => state.MyInteger++, name: StepConstants.SECOND_STEP_NAME);
            var thirdStep = Step<SimpleWorkflowState>.Create(state => state.MyInteger++, name: StepConstants.THIRD_STEP_NAME);

            return new List<IStep<SimpleWorkflowState>> { firstStep, secondStep, thirdStep }; 
        }

        public static Dictionary<string, IStep<SimpleWorkflowState>> GetDictionaryOptions()
        {
            var firstStep = Step<SimpleWorkflowState>.Create(state => state.MyInteger++, name: StepConstants.FIRST_STEP_NAME);
            var secondStep = Step<SimpleWorkflowState>.Create(state => state.MyInteger++, name: StepConstants.SECOND_STEP_NAME);
            var thirdStep = Step<SimpleWorkflowState>.Create(state => state.MyInteger++, name: StepConstants.THIRD_STEP_NAME);

            return new Dictionary<string, IStep<SimpleWorkflowState>>
            {
                { StepConstants.FIRST_STEP_NAME, firstStep },
                { StepConstants.SECOND_STEP_NAME, secondStep },
                { StepConstants.THIRD_STEP_NAME, thirdStep },
            };
        }
    }
}