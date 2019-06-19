using System;
using System.IO;
using SQLite;
using Domotica.Droid;
using Environment = System.Environment;

[assembly: Xamarin.Forms.Dependency(typeof(DatabaseService))]

namespace Domotica.Droid
{
    public class DatabaseService : Services.IDBInterface
    {
        public SQLiteConnection CreateConnection()
        {

            var sqliteFilename = "Database.db";
            string documentsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsDirectoryPath, sqliteFilename);

            if (!File.Exists(path))
            {
                using (var binaryReader = new BinaryReader(Android.App.Application.Context.Assets.Open(sqliteFilename)))
                {
                    using (var binaryWriter = new BinaryWriter(new FileStream(path, FileMode.Create)))
                    {
                        byte[] buffer = new byte[2048];
                        int length = 0;
                        while ((length = binaryReader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            binaryWriter.Write(buffer, 0, length);
                        }
                    }
                }
            }
            return new SQLiteConnection(path, false);
        }
        void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            byte[] buffer = new byte[Length];
            int byteRead = readStream.Read(buffer, 0, Length);
            while(byteRead >= 0)
            {
                writeStream.Write(buffer, 0, byteRead);
                byteRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }
    }
}