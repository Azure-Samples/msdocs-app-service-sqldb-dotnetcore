using System.Collections.Generic;
using System.Text.Json;

namespace DotNetCoreSqlDb.Data
{
    public static class ConvertData<T>
    {
        public static List<T> ByteArrayToObjectList(byte[] inputByteArray)
        {
            var deserializedList = JsonSerializer.Deserialize<List<T>>(inputByteArray);
            if(deserializedList == null) 
            {
                throw new Exception();
            }
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
            if (deserializedList == null)
            {
                throw new Exception();
            }
            return deserializedList;
        }

        public static byte[] ObjectToByteArray(T input)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(input);

            return bytes;
        }

    }
}
