using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using JsonConfigRepo.Abstractions;
using JsonConfigRepo.Implement;
using Microsoft.Extensions.DependencyInjection;

namespace JsonConfigRepo.Extensions.DependencyInjection
{
    /// <summary>
    /// Add IJsonConvertRepo and IJsonSerializerOptionsRepo
    /// </summary>
    public static class ServicesCollectionExtensions
    {
        /// <summary>
        /// 添加JsonConvertRepo和JsonSerializerOptions服务
        /// </summary>
        public static IServiceCollection AddJsonConvertRepoAndJsonSerializerOptions(this IServiceCollection collection,
            IReadOnlyCollection<JsonConverter> converters)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (converters == null) throw new ArgumentNullException(nameof(converters));
            collection.AddSingleton<IJsonConvertRepo, JsonConvertRepo>(v =>
                new JsonConvertRepo(converters));

            collection.AddSingleton<IJsonSerializerOptionsRepo, JsonSerializerOptionsRepo>();
            return collection;
        }

        /// <summary>
        /// 添加JsonConvertRepo和JsonSerializerOptions服务
        /// </summary>
        public static IServiceCollection AddJsonConvertRepoAndJsonSerializerOptions(this IServiceCollection collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            // ReSharper disable once UseArrayEmptyMethod
            return collection.AddJsonConvertRepoAndJsonSerializerOptions(new JsonConverter[0]);
        }
    }
}