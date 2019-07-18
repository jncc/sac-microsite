using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace JNCC.Microsite.SAC.Helpers
{
    public static class FileHelper
    {
        public static string GetActualFilePath(string basePath, string path, string filename = null)
        {
            if (!String.IsNullOrWhiteSpace(filename))
            {
                path = Path.Combine(path, filename);
            }

            if (String.IsNullOrWhiteSpace(basePath))
            {
                return Path.Combine(Directory.GetCurrentDirectory(), path);
            }
            else
            {
                return Path.Combine(basePath, path);
            }
        }

        public static void EnsureDirectoryExists(string path)
        {
            Directory.CreateDirectory(path);
        }

        public static void WriteToFile(string path, string content)
        {
            // Create Directory if it does not already exist
            FileHelper.EnsureDirectoryExists(Path.GetDirectoryName(path));

            // Write content to the file
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(content);
            }
        }

        public static void WriteJSONToFile(string path, object content)
        {
            // Create Directory if it does not already exist
            FileHelper.EnsureDirectoryExists(Path.GetDirectoryName(path));

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;

            using (StreamWriter sw = new StreamWriter(path))
            using (CharacterCorrectingWriter ccw = new CharacterCorrectingWriter(sw))
            using (JsonWriter writer = new JsonTextWriter(ccw))
            {
                serializer.Serialize(writer, content);
            }
        }
    }

    public class CharacterCorrectingWriter : TextWriter
    {
        readonly TextWriter inner;

        public CharacterCorrectingWriter(TextWriter inner)
        {
            this.inner = inner;
        }

        public override Encoding Encoding => inner.Encoding;

        public override void Write(char c)
        {
            if (c == '`')
                inner.Write('\'');
            else
                inner.Write(c);
        }
    }
}
