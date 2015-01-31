#region

using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

#endregion

namespace GameCore.Utils
{
    internal class SaveObjects
    {
        public static object DeserializeObject(string filename, Type aType)
        {
            XmlSerializer ser = new XmlSerializer(aType);
            XmlReader reader = new XmlTextReader(filename);
            Exception caught = null;
            try
            {
                object anObject = ser.Deserialize(reader);
                return anObject;
            }

            catch (Exception e)
            {
                caught = e;
            }
            finally
            {
                reader.Close();

                if (caught != null)
                    throw caught;
            }
            return null;
        }

        public static void SerializeObject(string filename, object anObject)
        {
            XmlSerializer ser = new XmlSerializer(anObject.GetType());
            XmlTextWriter writer = new XmlTextWriter(filename, new UTF8Encoding());
            writer.Formatting = Formatting.Indented;
            writer.IndentChar = ' ';
            writer.Indentation = 4;
            Exception caught = null;
            try
            {
                ser.Serialize(writer, anObject);
            }
            catch (Exception e)
            {
                caught = e;
            }
            finally
            {
                writer.Close();

                if (caught != null)
                    throw caught;
            }
        }
    }
}