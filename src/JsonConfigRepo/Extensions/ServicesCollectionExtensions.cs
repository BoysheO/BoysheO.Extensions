using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using JsonConfigRepo.Abstractions;
using JsonConfigRepo.Implement;
using Microsoft.Extensions.DependencyInjection;

namespace JsonConfigRepo.Extensions
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
            ImmutableArray<JsonConverter> converters)
        {
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
            return collection.AddJsonConvertRepoAndJsonSerializerOptions(ImmutableArray<JsonConverter>.Empty);
        }
    }
}