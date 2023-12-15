namespace QnA_App
{
    public class Program
    {
        protected Program() { }

        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string aiSvcEndpoint = configuration["AIServicesEndpoint"];
                string aiSvcKey = configuration["AIServicesKey"];
                string projectName = configuration["QAProjectName"];
                string deploymentName = configuration["QADeploymentName"];

                // Create client using endpoint and key
                AzureKeyCredential credentials = new AzureKeyCredential(aiSvcKey);
                Uri endpoint = new Uri(aiSvcEndpoint);
                QuestionAnsweringClient aiClient = new QuestionAnsweringClient(endpoint, credentials);

                // Submit a question and display the answer
                string user_question = "";

                while (user_question.ToLower() != "quit") {
                    Console.WriteLine("Question: ");
                    user_question = Console.ReadLine();
                    QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);
                    Response<AnswersResult> response = await aiClient.GetAnswersAsync(user_question,project);

                    foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
                    {
                        await Console.Out.WriteLineAsync(answer.Answer);
                        await Console.Out.WriteLineAsync($"Confidence: {answer.Confidence:P2}");
                        await Console.Out.WriteLineAsync($"Source: {answer.Source}");
                        await Console.Out.WriteLineAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
