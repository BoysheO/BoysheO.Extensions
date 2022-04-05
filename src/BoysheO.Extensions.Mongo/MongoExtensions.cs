using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace BoysheO.Extensions.Mongo
{
    public static class MongoExtensions
    {
        public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IAsyncCursor<T> cursor,
            [EnumeratorCancellation] CancellationToken token)
        {
            while (await cursor.MoveNextAsync(token))
            {
                foreach (var v in cursor.Current)
                {
                    yield return v;
                }
            }
        }

        public static async IAsyncEnumerable<T> GetAllAsync<T>(this IMongoCollection<T> collection,
            [EnumeratorCancellation] CancellationToken token)
        {
            var cursor = await collection.FindAsync(_ => true, cancellationToken: token);
            while (await cursor.MoveNextAsync(token))
            {
                foreach (var v in cursor.Current)
                {
                    yield return v;
                }
            }
        }

        public static async ValueTask<bool> IsExist<T>(this IMongoCollection<T> collection,Expression<Func<T, bool>> filter)
        {
            var count = await collection.CountDocumentsAsync(filter);
            return count > 0;
        }
    }
}