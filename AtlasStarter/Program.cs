using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace AtlasStarter
{
    class MainClassNoPrompt
    {
        static string[] names = new string[] { "John", "Paul", "George", "Ringo" };

        public static void Main(string[] args)
        {
            // Provide your Atlas connection string here:

            var mongoUri = "";

            IMongoClient client;

            // Note below that you must define the *type* of data stored in the
            // collection. Using mapping classes is strongly advised, but if you
            // don't create them, you can always use the more generic BsonDocument
            // type.
            IMongoCollection<TestDocument> collection;

            try
            {
                client = new MongoClient(mongoUri);
            }
            catch (Exception e)
            {
                return;
            }

            // Provide the name of the database and collection you want to use.
            // If they don't already exist, the driver and Atlas will create them
            // automatically when you first write data.
            var dbName = "";
            var collectionName = "";

            try
            {
                collection = client.GetDatabase(dbName).GetCollection<TestDocument>(collectionName);
            }
            catch (Exception e)
            {
                return;
            }

            /*      *** INSERT ***
             * 
             * You can insert individual documents using collection.Insert(). 
             * In this example, we're going to create 4 documents and then 
             * insert them all in one call with InsertMany().
             */
            try
            {
                var docs = new List<TestDocument>();

                for (int i = 0; i < 4; i++)
                {
                    var doc = new TestDocument(names[i], i, i * 42, i % 42);
                    docs.Add(doc);
                }

                // Now that we have a collection of documents, let's write them
                // to our Atlas data store:
                collection.InsertMany(docs);
            }
            catch (Exception e)
            {
                return;
            }

            /*      *** FIND ***
             * 
             * Now that we have data in Atlas, we can read it. To retrieve all of
             * the data in a collection, we call Find() with an empty filter. 
             * The Builders class is very helpful when building complex 
             * filters, and is used here to show its most basic use.
             */

            var allDocs = collection.Find(Builders<TestDocument>.Filter.Empty)
                .ToList();

            foreach (TestDocument doc in allDocs)
            {
                // Do something with the documents.
            }

            // I will now find the first document what has a value of 3 for
            // the Type property. Again we use the Builders class to create
            // the filter, and a simple LINQ statement to define the property
            // and value we're after:

            var findFilter = Builders<TestDocument>.Filter.Eq(t => t.Type, 3);

            var findResult = collection.Find(findFilter).FirstOrDefault();

            if (findResult == null)
            {
                return;
            }

            /*      *** UPDATE **
             * 
             * You can update a single document, as shown here, or multiple 
             * documents in a single call.
             * 
             * Here we update the Coordinates.X value on the document we just found
             */

            var updateFilter = Builders<TestDocument>.Update.Set(t => t.Coordinates.X, 420);

            // These FindOneAndUpdateOptions specify that we want the *updated* document
            // to be returned to us. By default, we get the document as it was *before*
            // the update.

            var options = new FindOneAndUpdateOptions<TestDocument, TestDocument>()
            {
                ReturnDocument = ReturnDocument.After
            };

            // the updatedDocument object is a TestDocument that refelcts the
            // changes.
            var updatedDocument = collection.FindOneAndUpdate(findFilter,
                updateFilter, options);

            /*      *** DELETE ***
             *      
             *      As with other CRUD methods, you can delete a single document 
             *      or all documents that match a specified filter. To delete all 
             *      of the documents in a collection, pass an empty filter to 
             *      the DeleteMany() method:
             */

            var deleteResult = collection
                .DeleteMany(Builders<TestDocument>
                .Filter.In("name", names));

            //You can get the count of deleted records with {deleteResult.DeletedCount}
        }

    }
}