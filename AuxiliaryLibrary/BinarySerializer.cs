using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AuxiliaryLibrary
{
    public static class BinarySerializer
    {
        static readonly BinaryFormatter formatter = new BinaryFormatter();

        /// <summary>
        /// Сериализовать объект
        /// </summary>
        /// <param name="obj">Сериализуемый объект</param>
        /// <returns>Поток, в котором находится сериализованный объект</returns>
        public static MemoryStream Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                formatter.Serialize(stream, obj);
            }
            catch { throw; }
            return stream;
        }
        /// <summary>
        /// Сериализовать объект в заданный поток
        /// </summary>
        /// <param name="obj">Сериализуемый объект</param>
        /// <returns>Поток, в котором находится сериализованный объект</returns>
        public static void Serialize(object obj, Stream destination)
        {
            try
            {
                destination.Position = 0;
                formatter.Serialize(destination, obj);
            }
            catch { throw; }
        }
        /// <summary>
        /// Десериализовать объект
        /// </summary>
        /// <param name="stream">Десериализуемый поток</param>
        /// <returns>Десериализованный объект</returns>
        public static object Deserialize(Stream stream)
        {
            try
            {
                stream.Position = 0;
                return formatter.Deserialize(stream);
            }
            catch { throw; }
        }
    }
}