using FeedbackApi.Domain;
using FeedbackApi.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace FeedbackApi.Infrastructure.Data
{
    public static class MongoConfig
    {
        public const string FeedbacksCollection = "feedbacks";

        /// <summary>Mapeia a entidade (que não conhece o Mongo) e garante os índices; uma vez só, na subida.</summary>
        public static void Configure(IMongoDatabase database)
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Feedback)))
            {
                BsonClassMap.RegisterClassMap<Feedback>(map =>
                {
                    map.AutoMap();
                    map.MapIdMember(f => f.Id)
                       .SetSerializer(new StringSerializer(BsonType.ObjectId))
                       .SetIdGenerator(StringObjectIdGenerator.Instance);
                    map.MapMember(f => f.PretendeVoltar).SetSerializer(new EnumSerializer<EPretendeVoltar>(BsonType.String));
                });
            }

            var feedbacks = database.GetCollection<Feedback>(FeedbacksCollection);

            // Um feedback por doação: o índice único é a garantia final contra corrida entre duas requisições
            feedbacks.Indexes.CreateOne(new CreateIndexModel<Feedback>(
                Builders<Feedback>.IndexKeys.Ascending(f => f.IdDoacao),
                new CreateIndexOptions { Unique = true }));
            feedbacks.Indexes.CreateOne(new CreateIndexModel<Feedback>(
                Builders<Feedback>.IndexKeys.Ascending(f => f.IdDoador)));
            feedbacks.Indexes.CreateOne(new CreateIndexModel<Feedback>(
                Builders<Feedback>.IndexKeys.Ascending(f => f.IdCampanha)));
        }
    }
}
