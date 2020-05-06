using ProcessFlow.Extensions;
using ProcessFlow.Tests.TestUtils;
using Xunit;

namespace ProcessFlow.Tests.Extensions
{
    public class GzipperTests
    {
        [Fact]       
        public void GzipSimpleState()
        {
            // Arrange
            var simpleState = new SimpleWorkflowState();

            // Act
            var zipped = simpleState.Zippify();
            var unzipped = zipped.Unzippify<SimpleWorkflowState>();

            // Assert
            Assert.Equal(simpleState.MyInteger, unzipped.MyInteger);
        }

        [Fact]
        public void GzipSelfReferencedState()
        {
            // Arrange
            var beginningState = StateGenerator.GenerateSelfRefereningState();

            // Act
            var zipped = beginningState.Zippify();
            
            var unzipped = zipped.Unzippify<SelfReferencingState>();
        }
    }
}
