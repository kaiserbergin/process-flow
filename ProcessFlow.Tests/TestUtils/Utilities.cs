using Newtonsoft.Json;

namespace ProcessFlow.Tests.TestUtils
{
    public static class Utilities
    {
        public static T DeepCopy<T>(this T objectToClone) where T : class
        {
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(objectToClone, settings);

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
