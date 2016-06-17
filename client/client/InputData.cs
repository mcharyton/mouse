using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace client
{
    [DataContract]
    class InputData
    {
        [DataMember]
        public string type;

        [DataMember]
        public string btn;

        [DataMember]
        public int x;

        [DataMember]
        public int y;

        [DataMember]
        public int z;

        public string Serialize()
        {
            MemoryStream str = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(InputData));
            ser.WriteObject(str, this);
            return System.Text.Encoding.UTF8.GetString(str.ToArray());
        }

        static public InputData Deserialize(string data)
        {
            MemoryStream str = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(InputData));
            InputData ms = (InputData)ser.ReadObject(str);
            return ms;
        }
    }
}
