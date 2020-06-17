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
            Console.WriteLine("Welcome! Please provide your Atlas connection string:", ConsoleColor.DarkBlue);
            var url = Console.ReadLine();
            Console.WriteLine($"Connecting to {url}...");

            IMongoClient client;
            IMongoCollection<BsonDocument> collection;

            try
            {
                client = new MongoClient(url);
            }
            catch (Exception e)
            {
                Console.WriteLine("Uh oh. I couldn't connect. Did you provide a valid username and password?", ConsoleColor.Red);
                Console.WriteLine($"Here's the exception: {e}", ConsoleColor.Red);
                return;
            }
            Console.WriteLine("What's the  name of the database you want to use?", ConsoleColor.DarkBlue);
            var dbName = Console.ReadLine();
            Console.WriteLine("What's the  name of the colletion you want to use?", ConsoleColor.DarkBlue);
            var collectionName = Console.ReadLine();

            try
            {
                collection = client.GetDatabase(dbName).GetCollection<BsonDocument>(collectionName);
            }catch (Exception e)
            {
                Console.WriteLine("Uh oh. I couldn't find or create the collection. Did you enter a blank value for one of the names, perhaps?", ConsoleColor.Red);
                Console.WriteLine($"Here's the exception: {e}", ConsoleColor.Red);
                return;
            }

            Console.WriteLine("I'm ready to write 4 test documents now. Press any key to continue.", ConsoleColor.DarkBlue);
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
                Console.WriteLine("Something went wrong; I couldn't insert the documents.", ConsoleColor.Red);
                Console.WriteLine($"Here's the exception: {e}", ConsoleColor.Red);

                return;
            }

            Console.WriteLine("Done. Here are the documents I created:", ConsoleColor.DarkBlue);

            foreach(BsonDocument doc in collection.Find(Builders<BsonDocument>.Filter.Empty).ToList())
            {
                Console.WriteLine(doc.ToString());
            }
           

            int id = getNumberFromUser();

            var filter = Builders<BsonDocument>.Filter.Eq("foo", id);

            var findResult = collection.Find(filter).FirstOrDefault();

            if (findResult == null)
            {
                Console.WriteLine("Something went wrong; I couldn't find that document.", ConsoleColor.Red);
                return;
            }
            Console.WriteLine($"I found the document with foo = {id}:", ConsoleColor.DarkBlue);
            Console.WriteLine($"{findResult.ToString()}.", ConsoleColor.White);
            Console.WriteLine("I will now update the info.x value on that document.", ConsoleColor.DarkBlue);

            var update = Builders<BsonDocument>.Update.Set("info.x", 420);
            var options = new FindOneAndUpdateOptions<BsonDocument, BsonDocument>() { ReturnDocument = ReturnDocument.After };
            var updatedDocument = collection.FindOneAndUpdate(filter, update, options);

            findResult = collection.Find(filter).FirstOrDefault();

            Console.WriteLine("Here's the updated document:", ConsoleColor.DarkBlue);
            Console.WriteLine(updatedDocument.ToString(), ConsoleColor.White);
            Console.WriteLine("Press any key to delete all the records, or Ctrl-C to quit without deleting.", ConsoleColor.DarkBlue);
            Console.ReadLine();

            var deleteResult = collection.DeleteMany(Builders<BsonDocument>.Filter.In("name", names));
            Console.WriteLine($"I deleted {deleteResult.DeletedCount} records.", ConsoleColor.DarkBlue);
        }

        private static int getNumberFromUser()
        {
            Console.WriteLine("Type a number between 0 and 3", ConsoleColor.DarkBlue);
            var input = Console.ReadKey().KeyChar;
            Console.WriteLine();
            int id = -1;
            int.TryParse(input.ToString(), out id);

            if (id < 0 || id > 3)
            {
                Console.WriteLine("That was not a valid number; please try again.", ConsoleColor.Red);
                return getNumberFromUser();
            }

            else return id;
        }
    }
}