
using OpenAI_API;
using OpenAI_API.Embedding;
using OpenAI_API.Models;
using Pinecone;
using Pinecone.Grpc;
using VideoUrlToChatBot.MigratedFromSite.Services;
using VideoUrlToChatBot.Models;

namespace ChatBotCoachWebsite.Helpers
{
    public class BuildPineconeIndex
    {

        private FileDataProvider _fileDataProvider;
        private TextFileKeyProvider _keyProvider;
        private OpenAIService _openAiService;
        private CheckAsciiCompatible _checkAsciiCompatible;

        public BuildPineconeIndex(FileDataProvider fileDataProvider, TextFileKeyProvider keyProvider, OpenAIService openAIService, CheckAsciiCompatible checkAsciiCompatible)
        {
            _fileDataProvider = fileDataProvider;
            _keyProvider = keyProvider;
            _openAiService = openAIService;
            _checkAsciiCompatible = checkAsciiCompatible;
        }

        public async Task UpsertPineconeIndexAsync()
        {
            //get vectors for index
            Vector[] vectors = await CreateVectorsAsync();

            //get pinecone index
            Index<GrpcTransport> index = await GetPineconeIndexAsync();

            //configure index
            //Configuring indexes is NOT supported in free tier.
            //calling ConfigureIndex is not necessary unless explicitly required. creation of index comes with default configuration
            //await pinecone.ConfigureIndex(indexName, replicas: 1, podType: "p1");

            //upsert vectors into index
            await index.Upsert(vectors);
            Console.WriteLine("Upsert to Pinecone index complete.");
        }

        public async Task<Index<GrpcTransport>> GetPineconeIndexAsync()
        {
            //get instance of pinecone client
            var pinecone = InitializePineCone();

            //get list of pinecone indexes
            var indexes = await pinecone.ListIndexes();

            //index name (only have 1 index for now)
            var indexName = "coachdb";

            //create a new pinecone index incase it doesn't exist
            await CreatePineconeIndexAsync(pinecone, indexes, indexName);

            //get an index
            var index = await pinecone.GetIndex(indexName);

            return index;
        }

        private async Task<Vector[]> CreateVectorsAsync()
        {
            Task<List<VectorData>> embeddings = CreateOpenAiEmbeddingsAsync();

            //create vectors to be upserted later into pinecone database
            List<Vector> vectorList = new();
            foreach (var embedding in await embeddings)
            {
                //first check if file name is Ascii compatible
                embedding.FileName = _checkAsciiCompatible.RemoveNonAsciiCharacters(embedding.FileName);

                var intent = await SummarizeMessage(embedding.FileData);

                Vector vector = new Vector
                {
                    Id = embedding.FileName,
                    Metadata = new MetadataMap
                    {
                        ["name"] = embedding.FileName,
                        ["text"] = embedding.FileData,
                        ["intent"] = intent,
                    },
                    Values = embedding.EmbeddingResult,
                  
                };

                vectorList.Add(vector);
            }

            //amount of custom data will always change so Vector[] vectors must start as a list, then converted to an array here
            Vector[] vectors = vectorList.ToArray();

            return vectors;
        }

        public async Task<string> SummarizeMessage(string fileData)
        {
            //get openai instance
            var openAi = _openAiService.GetOpenAI();

            //create message with prompt
            string promptedMsg = SummaryPrompt() + fileData;

            //format promptedMsg to be inputted to openai chat completion
            OpenAI_API.Chat.ChatMessage promptedChatMsg = new()
            {
                Content = promptedMsg,
            };

            List<OpenAI_API.Chat.ChatMessage> openAiChatMsg = new();

            openAiChatMsg.Add(promptedChatMsg);

            //get summary of message
            var result = await openAi.Chat.CreateChatCompletionAsync(openAiChatMsg, model: OpenAI_API.Models.Model.ChatGPTTurbo);

            var response = result.Choices[0].Message.Content;

            return response;
        }

        private string SummaryPrompt() => "Evaluate the given text to see if it can be described by one of these categories: Map, Character, Role, Ability usage, Aim. " +
                                          "Choose up to 3 of those categories which best describe the text, and number them from best description to least. " +
                                          "Follow this example structure and print your response on ONLY one line:" +
                                          "1. {category} 2. {category} 3. {category}. Here is the given text to evaluate:\n";

        private async Task<List<VectorData>> CreateOpenAiEmbeddingsAsync()
        {
            //get openai instance
            OpenAIAPI openAi = _openAiService.GetOpenAI();

            //get custom data (the string text files)
            //<key, value> = <fileName, fileData>
            Dictionary<string, string> customData = _fileDataProvider.ReadFileText();

            //create dictionary of embedding results by embedding custom data using openAi
            List<VectorData> embeddingResults = new();
            foreach (var data in customData)
            {
                var result = await openAi.Embeddings.CreateEmbeddingAsync(new EmbeddingRequest(Model.AdaTextEmbedding, data.Value));

                VectorData vectorData = new()
                {
                    FileName = data.Key,
                    FileData = data.Value,
                    EmbeddingResult = result,
                };
                embeddingResults.Add(vectorData);
            }

            return embeddingResults;
        }

        private async Task CreatePineconeIndexAsync(PineconeClient pinecone, IndexName[] indexes, string indexName)
        {
            //create new index if it doesn't exist
            if (!indexes.Contains(indexName))
            {
                await pinecone.CreateIndex(indexName, 1536, Metric.Cosine);
            }
        }

        private PineconeClient InitializePineCone()
        {
            //get pinecone API key
            var pineconeApiKey = _keyProvider.GetKey("pineconekey");

            var pineconeEnvironment = "asia-southeast1-gcp-free";
            var pinecone = new PineconeClient(pineconeApiKey, pineconeEnvironment);

            return pinecone;
        }
    }
}
