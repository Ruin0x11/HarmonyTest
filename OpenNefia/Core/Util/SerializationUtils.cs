using OpenNefia.Serial;
using OpenNefia.Mod;
using System;

namespace OpenNefia
{
    internal class SerializationUtils
    {
        internal static void Serialize<T>(string filepath, T data, string tagName) where T: IDataExposable
        {
            var exposer = new DataExposer(filepath, SerialStage.Saving);
            exposer.ExposeDeep(ref data!, tagName);
            exposer.Save();
        }

        internal static T Deserialize<T>(string filepath, T data, string tagName) where T: IDataExposable
        {
            var exposer = new DataExposer(filepath, SerialStage.LoadingDeep);
            exposer.ExposeDeep(ref data, tagName);

            exposer.Stage = SerialStage.ResolvingRefs;
            exposer.ExposeDeep(ref data, tagName);

            return data;
        }
    }
}