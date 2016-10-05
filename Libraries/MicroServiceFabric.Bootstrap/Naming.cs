namespace MicroServiceFabric.Bootstrap
{
    public static class Naming
    {
        public static string GetServiceName<TService>()
        {
            return typeof(TService).Name;
        }

        public static string GetServiceTypeName<TService>()
        {
            return $"{GetServiceName<TService>()}Type";
        }
    }
}
