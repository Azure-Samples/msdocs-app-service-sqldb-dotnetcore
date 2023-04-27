using System.Collections.Generic;
using System.Text.Json;

namespace DotNetCoreSqlDb
{
    public static class ConvertData<T>
    {
        public static List<T> ByteArrayToObjectList(byte[] inputByteArray)
        {
            var deserializedList = JsonSerializer.Deserialize<List<T>>(inputByteArray);
            return deserializedList;
        }

        public static byte[] ObjectListToByteArray(List<T> inputList)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(inputList);

            return bytes;
        }

        public static T ByteArrayToObject(byte[] inputByteArray)
        {
            var deserializedList = JsonSerializer.Deserialize<T>(inputByteArray);
            return deserializedList;
        }

        public static byte[] ObjectToByteArray(T input)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(input);

            return bytes;
        }

    }
}
