using System;
/*using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;*/

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace common
{
    [DataContract]
    class MouseData
    {
        [DataMember]
        public int x;

        [DataMember]
        public int y;

        public MouseData(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public string Serialize()
        {
            MemoryStream str = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(MouseData));
            ser.WriteObject(str, this);
            return System.Text.Encoding.UTF8.GetString(str.ToArray());
        }

        static public MouseData Deserialize(string data)
        {
            MemoryStream str = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(MouseData));
            MouseData ms = (MouseData)ser.ReadObject(str);
            return ms;
        }
    }
}
