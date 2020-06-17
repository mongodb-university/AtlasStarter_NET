using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AtlasStarter
{
    class MainClass
    {
        static string[] names = new string[] { "john", "paul", "george", "ringo" };

        public static void Main(string[] args)
        {
            WritePrompt("Welcome! Please provide your Atlas connection string:");
            var url = Console.ReadLine();
            WritePrompt($"Connecting to {url}...");

            IMongoClient client;
            IMongoCollection<BsonDocument> collection;

            try
            {
                client = new MongoClient(url);
            }
            catch (Exception e)
            {
                WriteError("Uh oh. I couldn't connect. Did you provide a valid username and password?", e);
                return;
            }
            WritePrompt("What's the  name of the database you want to use?");
            var dbName = Console.ReadLine();
            WritePrompt("What's the  name of the colletion you want to use?");
            var collectionName = Console.ReadLine();

            try
            {
                collection = client.GetDatabase(dbName).GetCollection<BsonDocument>(collectionName);
            }catch (Exception e)
            {
                WriteError("Uh oh. I couldn't find or create the collection. Did you enter a blank value for one of the names, perhaps?", e);
                return;
            }

            WritePrompt("I'm ready to write 4 test documents now. Press any key to continue.");
            Console.ReadKey();

            try
            {
                var docs = new List<BsonDocument>();
                

                for (int i = 0; i < 4; i++)
                {
                    var doc = new BsonDocument
                    {
                        { "name", names[i] },
                        { "foo", i },
                        { "info", new BsonDocument
                            {
                                { "x", i * 42 },
                                { "y", i % 42 }
                            }}
                    };

                    docs.Add(doc);

                }

                collection.InsertMany(docs);

            }
            catch (Exception e)
            {
                WriteError("Something went wrong; I couldn't insert the documents.", e);
                return;
            }

            WritePrompt("Done. Here are the documents I created:");

            foreach(BsonDocument doc in collection.Find(Builders<BsonDocument>.Filter.Empty).ToList())
            {
                Console.WriteLine(doc.ToString());
            }
           

            int id = getNumberFromUser();

            var filter = Builders<BsonDocument>.Filter.Eq("foo", id);

            var findResult = collection.Find(filter).FirstOrDefault();

            if (findResult == null)
            {
                WriteError("Something went wrong; I couldn't find that document.", null);
                return;
            }
            WritePrompt($"I found the document with foo = {id}:");
            Console.WriteLine($"{findResult.ToString()}.");
            WritePrompt("I will now update the info.x value on that document.");

            var update = Builders<BsonDocument>.Update.Set("info.x", 420);
            var options = new FindOneAndUpdateOptions<BsonDocument, BsonDocument>() { ReturnDocument = ReturnDocument.After };
            var updatedDocument = collection.FindOneAndUpdate(filter, update, options);

            findResult = collection.Find(filter).FirstOrDefault();

            WritePrompt("Here's the updated document:");
            Console.WriteLine(updatedDocument.ToString());
            WritePrompt("Press any key to delete all the records, or Ctrl-C to quit without deleting.");
            Console.ReadLine();

            var deleteResult = collection.DeleteMany(Builders<BsonDocument>.Filter.In("name", names));
            WritePrompt($"I deleted {deleteResult.DeletedCount} records.");
        }

        private static int getNumberFromUser()
        {
            WritePrompt("Type a number between 0 and 3");
            var input = Console.ReadKey().KeyChar;
            Console.WriteLine();
            int id = -1;
            int.TryParse(input.ToString(), out id);

            if (id < 0 || id > 3)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("That was not a valid number; please try again.");
                Console.ForegroundColor = ConsoleColor.White;
                return getNumberFromUser();
            }

            else return id;
        }

        private static void WritePrompt(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        private static void WriteError(string text, Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.WriteLine($"Here's the exception: {e}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadLine();
            return;
        }
    }
}