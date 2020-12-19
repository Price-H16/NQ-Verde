using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Extension
{
    public static class ExtraExtension
    {
        // This method will take the object and copy it without any reference. A clean copy.
        public static T CopyObjectWithoutReferences<T>(this T originalObject)
        {
            // Not working for nested objects. Have to find another way to do this.

            //var objectSerialized = Newtonsoft.Json.JsonConvert.SerializeObject<T>(originalObject);
            //var objectDeserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(objectSerialized);

            return originalObject;
        }
    }
}
